using System;
using System.Net.Sockets;
using System.Windows.Forms;

namespace NoughtsAndCrosses.Connection.TCP {
  public abstract class SocketHandler : ConnectManager {

    #region Прием данных
    /// <summary>
    /// Обработчик получения данных по соединению
    /// </summary>
    /// <param name="ar">Параметры получения данных</param>
    protected void OnReceive(IAsyncResult ar) {
      // Ссылка на соединение
      TcpConnectionInfo connect = null;
      try {
        connect = (TcpConnectionInfo)ar.AsyncState;

        if (connect == null) {
          return;
        }

        // Завершаем операцию получения данных
        int readBytes = 0;
        if (connect.socket != null && connect.socket.Handle.ToInt32() >= 0) {
          readBytes = connect.socket.EndReceive(ar);
        }
        if (readBytes <= 0) {
          CloseConnection(connect, "");
        }
        // Обрабатывает полученные данные
        if (connect.session == null) {
          return;
        }
        if (connect.session.IsClosed()) {
          return;
        }

        connect.session.ReceiveData(connect.buffer, readBytes);
        connect.socket.BeginReceive(connect.buffer, 0, connect.buffer.Length, SocketFlags.None, new AsyncCallback(OnReceive),
                                    connect);
      }
      catch (SocketException exc) {
        CloseConnection(connect, exc.Message);
      }
      catch (Exception exc) {
        CloseConnection(connect, exc.Message);
      }
    }

    #endregion

    #region Отправка данных

    #region Реализация абстрактного класса ConnectManager

    /// <summary>
    /// Посылает данные клиенту
    /// </summary>
    /// <param name="connect">Содинение</param>
    /// <param name="data">Массив с данными</param>
    public override void Send(IConnectionInfo connect, byte[] data, uint size) {
      if (size == 0) {
        return;
      }
      if (connect == null) {
        return;
      }

      bool needClose = false;

      if (!(connect is TcpConnectionInfo)) {
        return;
      }

      TcpConnectionInfo connectInfo = (TcpConnectionInfo)connect;

      if (connectInfo.socket != null) {
        if (connectInfo.socket.Handle.ToInt32() < 0) {
          return;
        }

        if (!connectInfo.socket.Connected) {
          needClose = true;
        }
        else {
          try {
            connectInfo.socket.BeginSend(data, 0, (int)size, 0, new AsyncCallback(OnSend), connectInfo);
          }
          catch (Exception ex) {
            MessageBox.Show(ex.Message, "Ошибка отправки данных");
          }
        }
      }

      if (needClose) {
        CloseConnection(connectInfo, "");
      }
    }

    #endregion

    /// <summary>
    /// Обработчик посылки данных по соединению
    /// </summary>
    /// <param name="ar"></param>
    protected void OnSend(IAsyncResult ar) {
      // Retrieve the socket from the async client object.
      TcpConnectionInfo connectInfo = (TcpConnectionInfo)ar.AsyncState;

      try {
        Socket handler = connectInfo.socket;
        if (handler != null) {
          if (handler.Handle.ToInt32() < 0) {
            return;
          }
          if (!handler.Connected) {
            return;
          }
          // Complete sending the data to the remote device.
          int bytesSent = handler.EndSend(ar);
        }
      }
      catch (SocketException ex) {
        CloseConnection(connectInfo, ex.Message);
      }
      catch (Exception ex) {
        CloseConnection(connectInfo, ex.Message);
      }
    }

    #endregion
  }
}
