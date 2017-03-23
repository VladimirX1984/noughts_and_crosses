using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace NoughtsAndCrosses.Connection.TCP {
  public class TcpServer : SocketHandler, IServer {
    /// <summary>
    /// Название сервера
    /// </summary>
    protected string name;

    /// <summary>
    /// Номер порта
    /// </summary>
    protected int portNumber;

    /// <summary>
    /// Объект создания и удаления сессий
    /// </summary>
    protected ISessionFactory sessionFactory;

    /// <summary>
    /// Сокет, слушающий запросы на соединение
    /// </summary>
    //protected TcpListener socketListener;
    protected Socket socketListener;

    /// <summary>
    /// Массив клиентских сессий
    /// </summary>
    protected ArrayList connections;

    /// <summary>
    /// Флаг работы сервера
    /// </summary>
    protected bool bRunning = false;

    /// <summary>
    ///
    /// </summary>
    protected int maxConnectionNumber;

    #region Инциализация

    public TcpServer(int aPort, ISessionFactory aSessionManager)
    : this("Сервер: " + aPort, aPort, aSessionManager, 0) {
    }

    public TcpServer(string aServerName, int aPort, ISessionFactory aSessionManager)
    : this(aServerName, aPort, aSessionManager, 0) {
    }

    public TcpServer(string aServerName, int aPort, ISessionFactory aSessionManager, int aMaxConnectionNumber) {
      name = aServerName;

      connections = new ArrayList();

      portNumber = aPort;
      sessionFactory = aSessionManager;

      maxConnectionNumber = aMaxConnectionNumber;

      Start();
    }

    ~TcpServer() {
      Stop();
    }

    public string Name {
      get { return name; }
    }

    public static string GetAddress() {
      // Получаем информацию о локальном компьютере
      string strHostName = Dns.GetHostName();
      IPHostEntry localMachineInfo = Dns.GetHostByName(strHostName);
      IPAddress[] addr = localMachineInfo.AddressList;
      string ipAddr = Convert.ToString(addr[0]);
      return ipAddr;
    }

    #endregion

    #region Реализация интерфейса IServer

    public bool IsRunning() {
      return bRunning;
    }

    public void Start() {
      if (bRunning) {
        return;
      }

      try {
        bRunning = true;

        IPAddress address = IPAddress.Any;
        IPEndPoint serverEndpoint = new IPEndPoint(address, portNumber);

        // Создаем сокет, привязываем его к адресу и начинаем прослушивание
        socketListener = new Socket(serverEndpoint.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        socketListener.Bind(serverEndpoint);
        socketListener.Listen(10000);

        socketListener.BeginAccept(new AsyncCallback(OnAccept), socketListener);

      }
      catch (Exception ex) {
        bRunning = false;
        MessageBox.Show(ex.Message, "Ошибка запуска сервера");
      }
    }

    public void Stop() {
      if (!bRunning) {
        return;
      }

      bRunning = false;

      if (socketListener != null) {
        socketListener.Close();
        socketListener = null;
      }

      lock (connections) {
        ArrayList connectionsCopy = (ArrayList)connections.Clone();
        foreach (IConnectionInfo connect in connectionsCopy) {
          CloseConnection(connect, "");
        }
        connections.Clear();
      }
    }

    #endregion

    public ushort GetConnectPort(object connect) {
      if (connect == null) {
        return 0;
      }
      if (!(connect is TcpConnectionInfo)) {
        return 0;
      }
      TcpConnectionInfo connectInfo = (TcpConnectionInfo)connect;
      if (connectInfo.socket == null) {
        return 0;
      }
      string connectionAddress = connectInfo.socket.RemoteEndPoint.ToString();
      int colonPos = connectionAddress.IndexOf(':');
      if (colonPos >= 0) {
        connectionAddress = connectionAddress.Substring(colonPos + 1, connectionAddress.Length - colonPos - 1);
      }
      try {
        return UInt16.Parse(connectionAddress);
      }
      catch (System.Exception e) {
        return 0;
      }
    }

    public string GetConnectionAddress(object connect) {
      if (connect == null) {
        return "";
      }
      if (!(connect is TcpConnectionInfo)) {
        return "";
      }
      TcpConnectionInfo connectInfo = (TcpConnectionInfo)connect;
      if (connectInfo.socket == null) {
        return  "";
      }
      string connectionAddress = connectInfo.socket.RemoteEndPoint.ToString();
      int colonPos = connectionAddress.IndexOf(':');
      if (colonPos >= 0) {
        connectionAddress = connectionAddress.Substring(0, colonPos);
      }
      return connectionAddress;
    }

    public bool IsConnectionIP(object connect, string aClientIP) {
      if (connect == null) {
        return false;
      }
      if (!(connect is TcpConnectionInfo)) {
        return false;
      }
      TcpConnectionInfo connectInfo = (TcpConnectionInfo)connect;
      if (connectInfo.socket == null) {
        return false;
      }
      SocketAddress addr = connectInfo.socket.RemoteEndPoint.Serialize();
      string connectIP = "";
      for (int i = 0; i < addr.Size; ++i) {
        connectIP += addr[i].ToString() + ".";
      }
      if (connectIP.Length > 0) {
        connectIP.Remove(connectIP.Length - 1, 1);
      }

      return connectIP.Equals(connect);
    }

    #region Прием запроса на соединение
    /// <summary>
    /// Обработчик запроса на соединение
    /// </summary>
    /// <param name="ar">Параметры запроса на соединение</param>
    protected void OnAccept(IAsyncResult ar) {
      if (socketListener == null) {
        return;
      }

      if (socketListener.Handle.ToInt32() < 0) {
        return;
      }

      // Создаем соединение
      TcpConnectionInfo connect = new TcpConnectionInfo();
      try {
        // Завершаем операцию Accept и доопределяем описатель соединения
        Socket s = (Socket)ar.AsyncState;

        if (s.Handle.ToInt32() < 0) {
          return;
        }

        connect.socket = s.EndAccept(ar);

        if (maxConnectionNumber > 0 && connections.Count >= maxConnectionNumber) {
          connect.socket.Close();
          connect.socket = null;
          return;
        }

        connect.buffer = new byte[1024];

        // Создаем соединение
        if (sessionFactory != null) {
          connect.session = sessionFactory.CreateSession(this, connect);
          lock (connections) {
            connections.Add(connect);
          }

          // асинхронно
          if (connect.socket != null) {
            AsyncCallback receiver = new AsyncCallback(OnReceive);
            connect.socket.BeginReceive(connect.buffer, 0, connect.buffer.Length,
                                        SocketFlags.None, receiver, connect);
          }
        }
      }
      catch (SocketException exc) {
        CloseConnection(connect, "");
      }
      catch (Exception exc) {
        CloseConnection(connect, "");
      }

      // Начинаем асинхронную новую операцию Accept
      if (socketListener == null) {
        return;
      }
      socketListener.BeginAccept(new AsyncCallback(OnAccept), socketListener);
    }

    #endregion

    #region Закрытие соединения

    /// <summary>
    /// Закрывает соединение
    /// </summary>
    /// <param name="connect"></param>
    public override void CloseConnection(IConnectionInfo connect, string sMsg) {
      try {
        if (connect == null) {
          return;
        }

        TcpConnectionInfo connectInfo = null;
        if (connect is TcpConnectionInfo) {
          connectInfo = (TcpConnectionInfo)connect;
        }

        if (connectInfo == null) {
          return;
        }

        if (connectInfo.isClosing) {
          lock (connections) {
            connections.Remove(connectInfo);
          }
          return;
        }
        connectInfo.isClosing = true;

        if (sessionFactory != null && connectInfo.session != null) {
          sessionFactory.DestroySession(connectInfo.session);
        }

        if (connectInfo.session != null && !connectInfo.session.IsClosed()) {
          connectInfo.session.CloseHandlers();
        }

        if (connectInfo.socket != null) {
          connectInfo.socket.Close();
          connectInfo.socket = null;
        }

        lock (connections) {
          connections.Remove(connectInfo);
        }
      }
      catch (Exception ex) {
        MessageBox.Show(ex.Message, "Ошибка при закрытии соединения");
      }
    }
    #endregion
  }
}
