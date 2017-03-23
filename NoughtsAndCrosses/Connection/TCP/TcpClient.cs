using System;
using System.Net.Sockets;
using System.Threading;
using NoughtsAndCrosses.Utils;

namespace NoughtsAndCrosses.Connection.TCP {
  public class TcpClient : SocketHandler, IClient {

    #region Реализация интерфейса IClient

    public void SetSession(Session aSession) {
      session = (TcpSession)aSession;
    }

    public bool Connect(string sIpAddr, int aPort) {
      port = aPort;
      return Connect(sIpAddr);
    }

    public bool Connect(string sIpAddr) {
      if (session == null || session.GetConnectionInfo() == null) {
        OnConnectionError("Внутрення ошибка программы");
        return false;
      }

      socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
      socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.TypeOfService, 8);
      try {
        IPAddr = sIpAddr;
        socket.Connect(sIpAddr, port);

        if (!socket.Connected) {
          OnConnectionError("Не могу соединится с сервером = " + sIpAddr + " порт=" +
                            port.ToString());
          return false;
        }

        connectInfo = (TcpConnectionInfo)session.GetConnectionInfo();

        connectInfo.socket = socket;
        connectInfo.buffer = new byte[1024];
        connectInfo.session = session;

        connectionCheckThread = new Thread(ConnectionCheck);
        connectionCheckThread.Start();

        AsyncCallback receiver = new AsyncCallback(OnReceive);
        connectInfo.socket.BeginReceive(connectInfo.buffer, 0, connectInfo.buffer.Length, SocketFlags.None, receiver,
                                        connectInfo);
      }
      catch (Exception e) {
        OnConnectionError("Не могу соединится с сервером = " + e.Message + " порт=" +
                          port.ToString());
        return false;
      }
      return true;
    }

    public void DisConnect(bool byUser, bool bAppExiting) {
      if (byUser) {
        DataBuffer dataBuffer = new DataBuffer();
        dataBuffer.Add(1);
        session.SendData(3, 0, dataBuffer.GetBuffer(), (ushort)dataBuffer.GetBufferSize());
        Thread.Sleep(250);
      }
      DisConnectImpl();
      if (connectionCheckThread != null) {
        int count = 0;
        while (connectionCheckThread.IsAlive) {
          Thread.Sleep(100);
          count++;
          if (count > 5) {
            connectionCheckThread.Abort();
          }
        }
        connectionCheckThread = null;
      }
    }

    #endregion

    #region Реализация абстрактного класса ConnectManager

    public override void CloseConnection(IConnectionInfo connect, string sMsg) {
      TcpConnectionInfo connectInfo = (TcpConnectionInfo)connect;
      if (connectInfo != null && connectInfo.socket != null) {
        try {
          if (connectInfo.socket.Handle.ToInt32() >= 0 && connectInfo.socket.Connected) {
            connectInfo.socket.Shutdown(SocketShutdown.Both);
          }

          connectInfo.socket.Close();
          connectInfo.socket = null;
          connectInfo.session = null;
          connectInfo = null;
        }
        catch (Exception exc) {
          try {
            connectInfo.socket.Close();
          }
          catch (Exception exc1) {
          }
          if (connectInfo != null) {
            connectInfo.socket = null;
            connectInfo.session = null;
          }
          connectInfo = null;
        }
      }

    }

    #endregion

    /// <summary>
    /// Сокет, слушающий запросы на соединение
    /// </summary>
    private Socket socket;

    /// <summary>
    /// Информация о соединения
    /// </summary>
    private TcpConnectionInfo connectInfo;

    /// <summary>
    /// адрес
    /// </summary>
    private string IPAddr;

    /// <summary>
    /// порт
    /// </summary>
    private int port;

    /// <summary>
    /// сессия клиента
    /// </summary>
    private TcpSession session;

    /// <summary>
    /// поток проверяющий соединение со сервером
    /// </summary>
    private Thread connectionCheckThread;

    public TcpClient() {
      socket = null;
      connectInfo = null;
      session = null;
    }

    ~TcpClient() {
      DisConnect(false, true);
    }

    protected void DisConnectImpl() {
      bTerminating = true;
      CloseConnection(connectInfo, "");
      connectInfo = null;
    }

    private void OnConnectionError(string asError) {
      session.OnConnectionError("", asError);
      DisConnectImpl();
    }

    protected volatile bool bTerminating = false;

    protected void ConnectionCheck(object data) {
      while (!bTerminating) {
        if (connectInfo.socket == null || !connectInfo.socket.Connected) {
          this.OnConnectionError("Невозможно соединится с сервером или соединение со сервером утеряно");
          connectionCheckThread = null;
          return;
        }

        Thread.Sleep(300);
      }
    }
  }
}
