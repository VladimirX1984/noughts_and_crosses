using System;
using System.Collections;
using NoughtsAndCrosses.Connection;
using NoughtsAndCrosses.Connection.HTTP;
using NoughtsAndCrosses.Connection.TCP;
using NoughtsAndCrosses.Game;

namespace NoughtsAndCrosses {
  public class Connector : ISessionFactory {

    #region Реализация интерфейса ISessionFactory

    /// <summary>
    /// Создает сессию для данного соединения
    /// </summary>
    /// <param name="aServer">Сервер</param>
    /// <param name="aConnection">Соединение</param>
    /// <returns>Возвращает сессию</returns>
    public Session CreateSession(ConnectManager connectHandler, IConnectionInfo connection) {
      if (connectHandler == server) {
        TcpServerSession session = new TcpServerSession(connectHandler as IServer, connection, context);
        lock (clientSessions) {
          clientSessions.Add(session);
          if (OnClientConnected != null) {
            OnClientConnected(session);
          }
        }
        return session;
      }

      return null;
    }

    /// <summary>
    /// Уничтожает сессию
    /// </summary>
    /// <param name="aSession">Сессия</param>
    public void DestroySession(Session session) {
      if (session is TcpSession) {
        session.CloseSession();
      }

      lock (clientSessions) {
        clientSessions.Remove(session);
      }
    }

    #endregion

    #region Инициализация

    /// <summary>
    /// Сервер для связи с клиентами
    /// </summary>
    private IServer server;

    /// <summary>
    /// Клиент для связи со сервером
    /// </summary>
    private IClient client;

    /// <summary>
    /// контекст игры
    /// </summary>
    private GameContext context;

    /// <summary>
    /// сессия от клиента
    /// </summary>
    private Session gameClientSession;

    public string Protocol { get; set; }

    /// <summary>
    /// константы 0 - неопределено
    /// </summary>
    private const int NONE = 0;
    private const int CLIENT = 1;
    private const int SERVER = 2;

    public static string[] Protocols = new string[] { "TCP", "HTTP" };

    /// <summary>
    /// режим игрока - сервер или клиент
    /// </summary>
    private int mode;

    public Connector(GameContext aContext) {
      gameClientSession = null;
      context = aContext;
      mode = NONE;
      Protocol = "HTTP";
    }

    ~Connector() {
      Stop();
    }

    #endregion

    public event Action<string> OnReceiveFirstData;
    public event Action<Session> OnClientConnected;
    public event Action<bool> OnDisConnected;
    public event Action OnServerStarted;
    public event Action OnServerStopped;

    #region Сервер и клиент

    /// <summary>
    /// Запуск сервера. Инициализация
    /// </summary>
    public void StartServer(int port, string serverName) {
      mode = SERVER;
      server = new TcpServer(serverName, port, this, 1);
      if (!server.IsRunning()) {
        context.game.OnServerError("Сервер уже запущен на данной машине");
        return;
      }
      clientSessions = new ArrayList();
      if (OnServerStarted != null) {
        OnServerStarted();
      }
    }

    /// <summary>
    /// Запуск клиента
    /// </summary>
    public void StartClient(string sIpAddr, int port, bool bCreateGame, string token) {
      mode = CLIENT;
      if (Protocol == "TCP") {
        client = new TcpClient();
        TcpConnectionInfo connectInfo = new TcpConnectionInfo();
        gameClientSession = new TcpClientSession(client, connectInfo, context);
        client.SetSession(gameClientSession);
        (gameClientSession as TcpClientSession).OnReceiveFirstData += OnReceiveFirstData;
        if (client.Connect(sIpAddr, port)) {
          (gameClientSession as TcpClientSession).SendQueryInfo();
        }
        return;
      }
      if (Protocol == "HTTP") {
        client = new HttpClient();
        HttpConnectionInfo connectInfo = new HttpConnectionInfo();
        gameClientSession = new HttpClientSession(client, connectInfo, context);
        client.SetSession(gameClientSession);
        (gameClientSession as HttpClientSession).OnReceiveFirstData += OnReceiveFirstData;
        if (client.Connect(sIpAddr, port)) {
          if (bCreateGame) {
            (gameClientSession as HttpClientSession).SendCreateGame();
          }
          else {
            (gameClientSession as HttpClientSession).SendJoinToGame(token);
          }
        }
      }
    }

    /// <summary>
    /// Остановка сервера
    /// </summary>
    public void StopServer() {
      if (server != null) {
        server.Stop();
        server = null;
        mode = NONE;
      }
      if (clientSessions != null) {
        foreach (TcpServerSession session in clientSessions) {
          session.CloseHandlers();
        }
        clientSessions.Clear();
        clientSessions = null;
        if (OnServerStopped != null) {
          OnServerStopped();
        }
      }
    }

    /// <summary>
    /// Остановка клиента
    /// </summary>
    public void StopClient(bool byUser) {
      InternalStopClient(byUser, false);
    }

    public void Stop() {
      if (mode == CLIENT) {
        InternalStopClient(true, true);
      }
      else if (mode == SERVER) {
        StopServer();
      }
    }

    #endregion

    public void InternalStopClient(bool byUser, bool bAppExiting) {
      if (client != null) {
        byUser = byUser && !context.gameCtrl.IsGameEnded();
        client.DisConnect(byUser, bAppExiting);
        client = null;
        mode = NONE;
        if (OnDisConnected != null) {
          OnDisConnected(byUser);
        }
      }
    }

    #region Клиентские сессии

    /// <summary>
    /// Массив клиентских сессий
    /// </summary>
    protected ArrayList clientSessions;

    /// <summary>
    /// передать информацию о ходе игры
    /// </summary>
    public void SendMoveInfo(byte number, char cellValue) {
      if (mode == CLIENT) {
        if (gameClientSession is TcpClientSession) {
          (gameClientSession as TcpClientSession).SendMakeMove(1, number, cellValue);
        }
        else {
          (gameClientSession as HttpClientSession).SendMakeMove(number, cellValue);
        }
      }
      else if (mode == SERVER) {
        foreach (TcpServerSession session in clientSessions) {
          session.SendMakeMove(1, number, cellValue);
        }
      }
    }

    public GameContext GetContext() {
      return context;
    }

    #endregion
  }
}
