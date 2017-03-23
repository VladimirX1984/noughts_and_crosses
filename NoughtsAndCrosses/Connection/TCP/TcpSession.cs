using System;
using NoughtsAndCrosses.Utils;
namespace NoughtsAndCrosses.Connection.TCP {
  public class TcpSession : Session {

    public TcpSession(IConnectManager connectHandler, IConnectionInfo connection)
    : base(connectHandler, connection) {
      SetInMessageHandlers(null);
    }

    #region Прием сообщений

    /// <summary>
    /// Делегат обработчика сообщения
    /// </summary>
    /// <param name="data">Массив с данными</param>
    /// <param name="offset">Смещение начала данных</param>
    /// <param name="size">Размер данных</param>
    protected delegate void MessageHandler(ushort messageID, byte[] data, int offset, int size);

    /// <summary>
    /// Массив обработчиков принятых сообщений
    /// i-ый элемент ссылается на обработчик сообщения i-ого типа
    /// </summary>
    protected MessageHandler[] inMessageHandlers;

    /// <summary>
    /// Обработчик сообщения с недостоверной сигнатурой
    /// </summary>
    /// <param name="aData">Массив полученных данных</param>
    /// <param name="aDataSize">Размер данных</param>
    protected override void OnSignatureError() {
      // По-умолчанию закрываем сессию
      CloseSession();
    }

    /// <summary>
    /// Установливает обработчики принятых сообщений
    /// </summary>
    /// <param name="aMessageHandlers">Массив обработчиков принятых сообщений</param>
    protected void SetInMessageHandlers(MessageHandler[] aMessageHandlers) {
      inMessageHandlers = aMessageHandlers;
      if (inMessageHandlers == null) {
        inMessageHandlers = new MessageHandler[0];
      }
    }

    /// <summary>
    /// Прием данных
    /// </summary>
    /// <param name="data">данные</param>
    /// <param name="size">размер данных</param>
    public sealed override void ReceiveData(byte[] data, int size) {
      // Принимаем данные - копируем данные в буфер
      DataReader dataReader = new DataReader(data, 0, size);

      if (dataReader.GetDataSize() >= 3) {
        // Формируем сообщение
        // по сигнатуре проверяем достоверность полученного сообщения

        short usignature = 0;
        dataReader.Read(ref usignature);
        if (signature != usignature) {
          OnSignatureError();
        }

        // Определяем тип сообщения
        byte type = 0;
        dataReader.Read(ref type);
        int headerSize = 5;
        ushort packetID = 0;
        dataReader.Read(ref packetID);

        // Проверяем есть ли обработчик для сообщения такого типа
        if (type < inMessageHandlers.Length && inMessageHandlers[type] != null) {
          if (dataReader.GetDataSize() < headerSize + 2) {
            OnReceivingError(RECEIVE_FATAL_ERROR, "Internal error: no data");
            return;
          }

          // сообщения с переменным размером - длину сообщения определяем из поля <размер данных>
          byte[] dataBuffer = new byte[dataReader.GetDataSize() - dataReader.GetPosition()];
          dataReader.ReadArray(ref dataBuffer);
          try {
            // Обрабатываем сообщение - вызываем соответствующий обработчик
            inMessageHandlers[type](packetID, dataBuffer, headerSize, dataBuffer.Length);
          }
          catch (Exception exc) {
            string s = string.Format("Internal error {0}", exc.Message);
            OnReceivingError(RECEIVE_FATAL_ERROR, s);
            return;
          }
        }
        else {
          OnReceivingError(RECEIVE_FATAL_ERROR, "Internal error: no command");
          return;
        }
      }
    }

    public sealed override void ReceiveData(DataBuffer buffer) {
      ReceiveData(buffer.GetBuffer(), (int)buffer.GetBufferSize());
    }

    public override void ReceiveData(string command, byte[] data, int size) {
    }

    #endregion

    #region Отправка сообщения

    public void SendData(byte type, ushort messageID, byte[] data, ushort size) {
      DataBuffer dataBuffer = new DataBuffer();
      dataBuffer.Add(signature);
      dataBuffer.Add(type);
      dataBuffer.Add(messageID);
      dataBuffer.AddArray(data, 0, size);
      SendData(dataBuffer.GetBuffer(), (ushort)dataBuffer.GetBufferSize());
    }

    #endregion
  }
}