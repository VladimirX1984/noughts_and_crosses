using System;
using NoughtsAndCrosses.Connection;
using NoughtsAndCrosses.Connection.TCP;
using NoughtsAndCrosses.Utils;
using NoughtsAndCrosses.Game;

namespace NoughtsAndCrosses {
  public class TcpClientSession : TcpSession {

    #region Инициализация

    public TcpClientSession(IClient client, IConnectionInfo connection, GameContext context)
    : base(client, connection, context) {

    }

    #endregion

    public event Action<string> OnReceiveFirstData;

    #region Прием сообщений

    /// <summary>
    /// Информация о игре
    /// </summary>
    /// <param name="aData"></param>
    /// <param name="aDataOffset"></param>
    /// <param name="aDataSize"></param>
    protected override void OnGameInfo(ushort aMessageID, byte[] aData, int aDataOffset, int aDataSize) {
      if (aMessageID != messageID) {
        OnReceivingError(RECEIVE_FATAL_ERROR, "OnGameInfo aMessageID != messageID");
        return;
      }

      DataReader dataReader = new DataReader(aData, 0, aDataSize);
      byte type = 0;
      if (!dataReader.Read(ref type)) {
        OnReceivingError(RECEIVE_FATAL_ERROR, "OnGameInfo dataReader.Read(ref type)");
        return;
      }

      if (type == inQuery) {
        OnReceivingError(RECEIVE_FATAL_ERROR, "OnGameInfo type == inQuery");
        return;
      }

#if FOR_JAVA
      sbyte mode = -1;
      if (!dataReader.Read(ref mode)) {
        OnReceivingError(RECEIVE_FATAL_ERROR, "dataReader.Read(ref mode)");
        return;
      }
      if (mode == -2) {
        return;
      }
#endif

      bool bMyFirstMove = false;
      if (!dataReader.Read(ref bMyFirstMove)) {
        OnReceivingError(RECEIVE_FATAL_ERROR, "dataReader.Read(ref bMyFirstMove)");
        return;
      }

      bool bMyMove = false;
      if (!dataReader.Read(ref bMyMove)) {
        OnReceivingError(RECEIVE_FATAL_ERROR, "dataReader.Read(ref bMyMove)");
        return;
      }

#if FOR_JAVA
      if (mode == 0) {
        context.gameCtrl.SetYourMove(bMyFirstMove, bMyMove);
        if (OnReceiveFirstData != null) {
          OnReceiveFirstData("");
        }
        return;
      }
#endif

      ushort rowCellCount = 0;
      if (!dataReader.Read(ref rowCellCount)) {
        OnReceivingError(RECEIVE_FATAL_ERROR, "dataReader.Read(ref rowCellCount)");
        return;
      }

      ushort numberToWin = 0;
      if (!dataReader.Read(ref numberToWin)) {
        OnReceivingError(RECEIVE_FATAL_ERROR, "dataReader.Read(ref numberToWin)");
        return;
      }

      ushort size = 0;
      if (!dataReader.Read(ref size)) {
        OnReceivingError(RECEIVE_FATAL_ERROR, "dataReader.Read(size)");
        return;
      }

      string sdata = null;
      if (!dataReader.ReadASCII(size, ref sdata)) {
        OnReceivingError(RECEIVE_FATAL_ERROR, "dataReader.ReadArray(size, ref data)");
        return;
      }
#if FOR_JAVA
      context.gameCtrl.IsObserver = mode == 2;
#endif
      context.gameCtrl.SetGameState(sdata);
      if (!context.gameCtrl.IsObserver) {
        context.gameCtrl.SetYourMove(bMyFirstMove, bMyMove);
      }
      else {
        context.gameCtrl.SetYourMove(false, false);
      }
#if FOR_JAVA
      context.game.SetRowCellCount(rowCellCount);
#else
      context.game.SetRowCellCount(rowCellCount);
#endif
      context.game.SetNumberToWin(numberToWin);
      if (OnReceiveFirstData != null) {
        OnReceiveFirstData("");
      }
    }

    #endregion

    #region Отправка сообщений

    public void SendQueryInfo() {
      DataBuffer dataBuffer = new DataBuffer();
      dataBuffer.Add(outQuery);
#if FOR_JAVA
      dataBuffer.AddASCII(context.gameCtrl.UserName);
      dataBuffer.Add((byte)1);
      bool bMyFirstMove = context.gameCtrl.IsMyFirstMove();
      bool bMyMove = context.gameCtrl.IsMyMove();
      dataBuffer.Add(bMyFirstMove);
      dataBuffer.Add(context.game.GetRowCellCount());
      dataBuffer.Add((ushort)context.gameCtrl.NumberToWin);
#endif
      SendData(outGameInfo, messageID, dataBuffer.GetBuffer(), (ushort) dataBuffer.GetBufferSize());
    }

    #endregion
  }
}
