using System;
using NoughtsAndCrosses.Utils;

namespace NoughtsAndCrosses.Connection {
  public abstract class Session {
    /// <summary>
    /// Сигнатура сообщения
    /// </summary>
    protected short signature;

    /// <summary>
    /// Сервер, который создал сессию
    /// </summary>
    protected IConnectManager connectHandler;

    /// <summary>
    /// Объект соединение
    /// </summary>
    protected IConnectionInfo connectInfo;

    /// <summary>
    /// Идентификатор сессии
    /// </summary>
    protected ulong sessionID;

    /// <summary>
    /// Порт коннекта
    /// </summary>
    protected ushort connectionPort = 0;

    /// <summary>
    /// Адрес конекта
    /// </summary>
    protected string connectionAddress;

    #region Создание сессии

    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="asocketHandler">Сервер, который содал сессию</param>
    /// <param name="aConnection">Объект соединение</param>
    public Session(IConnectManager connectHandler, IConnectionInfo connection) {
      Open(connectHandler, connection);
    }

    public void Open(IConnectManager aConnectHandler, IConnectionInfo connection) {
      connectHandler = aConnectHandler;
      connectInfo = connection;
      closed = false;
    }

    public ulong GetSessionID() {
      return sessionID;
    }

    public IConnectionInfo GetConnectionInfo() {
      return connectInfo;
    }

    #endregion

    #region Закрытие сессии

    /// <summary>
    /// Флаг закрытия сессии
    /// </summary>
    private bool closed = false;


    public virtual void Close() {

    }

    public void CloseSession() {
      if (connectHandler != null && connectInfo != null) {
        connectHandler.CloseConnection(connectInfo, "");
      }
    }

    /// <summary>
    /// Закрываем сессию
    /// </summary>
    internal void CloseHandlers() {
      if (!closed) {
        closed = true;
        Close();
        connectHandler = null;
        connectInfo = null;
      }
    }

    /// <summary>
    /// Деструктор
    /// </summary>
    ~Session() {
      if (!IsClosed()) {
        CloseHandlers();
      }
    }

    /// <summary>
    /// Проверяет закрыта ли сессия
    /// </summary>
    /// <returns>Сессия закрыта?</returns>
    public bool IsClosed() {
      return closed;
    }
    #endregion

    #region Прием сообщений

    public static string RECEIVE_ERROR = "recever_error";
    public static string RECEIVE_FATAL_ERROR = "recever_fatal_error";

    /// <summary>
    /// Обработчик сообщения с недостоверной сигнатурой
    /// </summary>
    /// <param name="aData">Массив полученных данных</param>
    /// <param name="aDataSize">Размер данных</param>
    protected virtual void OnSignatureError() {

    }

    public virtual void OnConnectionError(string type, string errorMsg) {

    }

    protected virtual void OnReceivingError(string type, string errorMsg) {

    }

    /// <summary>
    /// Прием данных
    /// </summary>
    /// <param name="data">данные</param>
    /// <param name="size">размер данных</param>
    public abstract void ReceiveData(byte[] data, int size);

    public abstract void ReceiveData(DataBuffer buffer);

    public abstract void ReceiveData(string command, byte[] data, int size);

    #endregion

    #region Отправка сообщения

    public void SendData(byte[] data, ushort size) {
      if (connectHandler == null) {
        return;
      }
      if (connectInfo == null) {
        return;
      }
      if (IsClosed()) {
        return;
      }
      connectHandler.Send(connectInfo, data, size);
    }

    #endregion
  }
}
