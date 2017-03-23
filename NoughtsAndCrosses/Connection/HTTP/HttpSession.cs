using System;
using System.Collections.Generic;
using NoughtsAndCrosses.Utils;

namespace NoughtsAndCrosses.Connection.HTTP {
  public class HttpSession : Session {

    /// <summary>
    /// Делегат обработчика сообщения
    /// </summary>
    /// <param name="data">Массив с данными</param>
    /// <param name="offset">Смещение начала данных</param>
    /// <param name="size">Размер данных</param>
    protected delegate bool MessageHandler(byte[] data, int offset, int size);

    private readonly Dictionary<object, MessageHandler> inMessageHandlers = new Dictionary<object, MessageHandler>();

    public HttpSession(IConnectManager connectHandler, IConnectionInfo connection)
    : base(connectHandler, connection) {

    }

    protected void addMessageHandler(object type, MessageHandler messageHandlers) {
      inMessageHandlers.Add(type, messageHandlers);
    }

    #region Прием сообщений

    /// <summary>
    /// Прием данных
    /// </summary>
    /// <param name="data">данные</param>
    /// <param name="size">размер данных</param>
    public sealed override void ReceiveData(byte[] data, int size) {
      // Принимаем данные - копируем данные в буфер
      DataReader dataReader = new DataReader(data, 0, size);

      if (dataReader.GetDataSize() < 3) {
        OnReceivingError(RECEIVE_FATAL_ERROR, "Internal error: no message");
        return;
      }
      // Формируем сообщение
      // по сигнатуре проверяем достоверность полученного сообщения

      // Определяем тип сообщения
      byte type = 255;
      dataReader.Read(ref type);

      if (!inMessageHandlers.ContainsKey(type)) {
        OnReceivingError(RECEIVE_FATAL_ERROR, "Internal error: unknown command");
        return;
      }

      int headerSize = 1;

      // Проверяем есть ли обработчик для сообщения такого типа
      if (dataReader.GetDataSize() < headerSize + 2) {
        OnReceivingError(RECEIVE_FATAL_ERROR, "Internal error: no data");
        return;
      }

      // сообщения с переменным размером - длину сообщения определяем из поля <размер данных>
      byte[] dataBuffer = new byte[dataReader.GetDataSize() - dataReader.GetPosition()];
      dataReader.ReadArray(ref dataBuffer);
      try {
        // Обрабатываем сообщение - вызываем соответствующий обработчик
        inMessageHandlers[type](dataBuffer, headerSize, dataBuffer.Length);
      }
      catch (Exception exc) {
        string s = string.Format("Internal error {0}", exc.Message);
        OnReceivingError(Session.RECEIVE_FATAL_ERROR, s);
        return;
      }
    }

    public sealed override void ReceiveData(DataBuffer buffer) {
      ReceiveData(buffer.GetBuffer(), (int)buffer.GetBufferSize());
    }

    public override void ReceiveData(string command, byte[] data, int size) {
      if (!inMessageHandlers.ContainsKey(command)) {
        OnReceivingError(RECEIVE_FATAL_ERROR, "Internal error: unknown command");
        return;
      }
      try {
        // Обрабатываем сообщение - вызываем соответствующий обработчик
        inMessageHandlers[command](data, 0, size);
      }
      catch (Exception exc) {
        string s = string.Format("Internal error {0}", exc.Message);
        OnReceivingError(Session.RECEIVE_FATAL_ERROR, s);
        return;
      }
    }

    #endregion

    #region Отправка сообщения

    public void SendData(string method, string command, Headers headers, DataBuffer data) {
      if (connectHandler == null) {
        return;
      }
      if (connectInfo == null) {
        return;
      }
      if (IsClosed()) {
        return;
      }
      (connectHandler as IHttpClient).SendData(connectInfo, method, command, headers, data);
    }

    public void SendJsonData(string method, string command, Headers headers, string json) {
      if (connectHandler == null) {
        return;
      }
      if (connectInfo == null) {
        return;
      }
      if (IsClosed()) {
        return;
      }
      (connectHandler as IHttpClient).SendJson(connectInfo, method, command, headers, json);
    }

    #endregion
  }
}
