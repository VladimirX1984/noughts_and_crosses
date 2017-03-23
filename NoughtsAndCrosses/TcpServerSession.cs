using System;
using NoughtsAndCrosses.Connection;
using NoughtsAndCrosses.Connection.TCP;
using NoughtsAndCrosses.Utils;
using NoughtsAndCrosses.Game;

namespace NoughtsAndCrosses {
  public class TcpServerSession : TcpSession {

    /// <summary>
    ///
    /// </summary>
    /// <param name="server"></param>
    /// <param name="aConnection"></param>
    /// <param name="aContext"></param>
    public TcpServerSession(IServer server, IConnectionInfo connection, GameContext aContext)
    : base(server, connection, aContext) {

    }

    ~TcpServerSession() {
      CloseHandlers();
    }

    public override void Close() {

    }

    /// <summary>
    /// Запрос на получение информации о игре
    /// </summary>
    /// <param name="aData"></param>
    /// <param name="aDataOffset"></param>
    /// <param name="aDataSize"></param>
    protected override void OnGameInfo(ushort aMessageID, byte[] aData, int aDataOffset, int aDataSize) {
      DataReader dataReader = new DataReader(aData, 0, aDataSize);
      byte aType = 0;
      if (!dataReader.Read(ref aType)) {
        this.OnReceivingError(RECEIVE_FATAL_ERROR, "Server OnGameInfo dataReader.Read(ref type)");
        return;
      }

      if (aType != inQuery) {
        this.OnReceivingError(RECEIVE_FATAL_ERROR, "Server OnGameInfo type != inQuery");
        return;
      }

      DataBuffer dataBuffer = new DataBuffer();
      bool bMyFirstMove = !context.gameCtrl.IsMyFirstMove();
      bool bMyMove = !context.gameCtrl.IsMyMove();
      dataBuffer.Add(outConfirmation);
      dataBuffer.Add(bMyFirstMove);
      dataBuffer.Add(bMyMove);
      dataBuffer.Add(context.game.GetRowCellCount());
      dataBuffer.Add((ushort)context.gameCtrl.NumberToWin);
      dataBuffer.AddASCII(context.gameCtrl.GetGameState().DataString);

      SendData(outGameInfo, aMessageID, dataBuffer.GetBuffer(), (ushort)dataBuffer.GetBufferSize());
    }
  }
}
