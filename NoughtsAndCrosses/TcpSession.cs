using System;
using NoughtsAndCrosses.Connection;
using NoughtsAndCrosses.Game;
using NoughtsAndCrosses.Utils;

namespace NoughtsAndCrosses {
  public class TcpSession : NoughtsAndCrosses.Connection.TCP.TcpSession {

    #region Инициализация

    /// <summary>
    /// контекст игры
    /// </summary>
    protected GameContext context;

    /// <summary>
    /// идентификатор пакета
    /// </summary>
    protected ushort messageID;

    /// <summary>
    /// Типы получаемых от клиентов сообщений
    /// </summary>
    protected const int inMessageCount = 5;

    public TcpSession(IConnectManager connectHandler, IConnectionInfo connection, GameContext aContext)
    : base(connectHandler, connection) {
      context = aContext;
      signature = 20100;
      messageID = 0;

      MessageHandler[] inHandlers = new MessageHandler[inMessageCount];
      inHandlers[inGameInfo]      = new MessageHandler(OnGameInfo);
      inHandlers[inMoveInfo]      = new MessageHandler(OnMoveInfo);
      inHandlers[inEndGame]       = new MessageHandler(OnEndGame);
      inHandlers[inInterruptGame] = new MessageHandler(OnInterruptGame);
      SetInMessageHandlers(inHandlers);
    }

    #endregion

    #region Прием сообщений

    // Получить информацию о игре, кто первым ходить
    protected const int inGameInfo = 1;
    protected const int inMoveInfo = 2;                 // Получить информацию о ходе игры
    protected const int inEndGame = 3;                  // Игра закончена
    protected const int inInterruptGame = 4;            // Игра прервана игроком
    protected const byte inQuery = 1;                   // запрос и команда
    protected const byte inGameEnded = 2;               // конец игры

    /// <summary>
    /// Пустое сообщение
    /// </summary>
    /// <param name="data"></param>
    /// <param name="dataOffset"></param>
    /// <param name="dataSize"></param>
    protected void OnEmptyMessage(ushort aMessageID, byte[] data, int dataOffset, int dataSize) {

    }

    /// <summary>
    /// Запрос на получение информации о игре
    /// </summary>
    /// <param name="data"></param>
    /// <param name="dataOffset"></param>
    /// <param name="dataSize"></param>
    protected virtual void OnGameInfo(ushort aMessageID, byte[] data, int dataOffset, int dataSize) {
      this.OnReceivingError(RECEIVE_FATAL_ERROR, "OnGameInfo!!!");
    }

    /// <summary>
    /// Ход
    /// </summary>
    /// <param name="data"></param>
    /// <param name="dataOffset"></param>
    /// <param name="dataSize"></param>
    protected virtual void OnMoveInfo(ushort aMessageID, byte[] data, int dataOffset, int dataSize) {
      if (aMessageID != messageID) {
        this.OnReceivingError(RECEIVE_ERROR, "OnMoveInfo aMessageID != messageID");
        return;
      }

      DataReader dataReader = new DataReader(data, 0, dataSize);
      byte type = 0;
      if (!dataReader.Read(ref type)) {
        this.OnReceivingError(RECEIVE_ERROR, "OnMoveInfo dataReader.Read(ref type)");
        return;
      }

      if (type != inQuery) {
        this.OnReceivingError(RECEIVE_ERROR, "OnMoveInfo type != inQuery");
        return;
      }

      short cellNumber = 0;
      if (!dataReader.Read(ref cellNumber)) {
        this.OnReceivingError(RECEIVE_ERROR, "dataReader.Read(ref cellNumber)");
        return;
      }

      char cellValue = '?';
      if (!dataReader.Read(ref cellValue)) {
        this.OnReceivingError(RECEIVE_ERROR, "dataReader.Read(ref cellValue)");
        return;
      }

      context.gameCtrl.MakeMove(cellNumber, cellValue);
    }

    protected virtual void OnEndGame(ushort aMessageID, byte[] data, int dataOffset, int dataSize) {
      if (aMessageID != messageID) {
        this.OnReceivingError(RECEIVE_ERROR, "OnEndGame aMessageID != messageID");
        return;
      }

      DataReader dataReader = new DataReader(data, 0, dataSize);
      byte type = 0;
      if (!dataReader.Read(ref type)) {
        this.OnReceivingError(RECEIVE_ERROR, "OnEndGame dataReader.Read(ref type)");
        return;
      }

      if (type != inGameEnded) {
        this.OnReceivingError(RECEIVE_ERROR, "OnEndGame type != inGameEnded");
        return;
      }

      sbyte winner = 0;
      if (!dataReader.Read(ref winner)) {
        this.OnReceivingError(RECEIVE_ERROR, "OnEndGame dataReader.Read(ref winner)");
        return;
      }

      string winUserName = String.Empty;
      if (winner != 0) {
        if (!dataReader.ReadASCII(ref winUserName)) {
          this.OnReceivingError(RECEIVE_ERROR, "OnEndGame dataReader.ReadArray(ref winUserName)");
          return;
        }
      }

      context.gameCtrl.FisishGame(winner, winUserName);
    }

    protected virtual void OnInterruptGame(ushort aMessageID, byte[] data, int dataOffset, int dataSize) {
      if (aMessageID != messageID) {
        this.OnReceivingError(RECEIVE_ERROR, "OnInterruptGame aMessageID != messageID");
        return;
      }

      DataReader dataReader = new DataReader(data, 0, dataSize);
      byte type = 0;
      if (!dataReader.Read(ref type)) {
        this.OnReceivingError(RECEIVE_ERROR, "OnInterruptGame dataReader.Read(ref type)");
        return;
      }

      if (type != inGameEnded) {
        this.OnReceivingError(RECEIVE_ERROR, "OnInterruptGame type != inGameEnded");
        return;
      }

      sbyte id = 0;
      if (!dataReader.Read(ref id)) {
        this.OnReceivingError(RECEIVE_ERROR, "OnInterruptGame dataReader.Read(ref id)");
        return;
      }

      string winUserName = String.Empty;
      if (!dataReader.ReadASCII(ref winUserName)) {
        this.OnReceivingError(RECEIVE_ERROR, "OnInterruptGame dataReader.ReadArray(ref winUserName)");
        return;
      }

      context.gameCtrl.FisishGame(id, winUserName);
    }

    #endregion

    #region Отправка сообщений

    /// <summary>
    /// Типы отправляемых клиенту сообщений
    /// </summary>
    protected const byte outGameInfo = 1;       //
    protected const byte outMoveInfo = 2;       //
    protected const byte outSurrender = 3;       //

    protected const byte outQuery = 1;          // запрос и команда
    protected const byte outConfirmation = 2;   // подтверждение

    public void SendMakeMove(byte type, short cellNumber, char cellValue) {
      DataBuffer dataBuffer = new DataBuffer();
      dataBuffer.Add(type);
      dataBuffer.Add(cellNumber);
      dataBuffer.Add(cellValue);
      SendData(outMoveInfo, messageID, dataBuffer.GetBuffer(), (ushort)dataBuffer.GetBufferSize());
    }

    #endregion

    public override void OnConnectionError(string type, string errorMsg) {
      context.game.OnConnectionError(errorMsg);
    }

    protected override void OnReceivingError(string type, string errorMsg) {
      context.game.OnReceivingError(type, errorMsg);
    }

  }
}
