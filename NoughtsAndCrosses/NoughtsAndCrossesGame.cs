using System;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using NoughtsAndCrosses.Connection;
using NoughtsAndCrosses.Connection.TCP;
using NoughtsAndCrosses.Game;

namespace NoughtsAndCrosses {
  public class NoughtsAndCrossesGame : GameBase {

    private NoughtsAndCrossesFormData noughtsAndCrossesFormData;
    private GameContext context;
    private Connector connector;
    private GameCtrl gameCtrl;

    private const string YOUR_SELECT = "Игра";
#if FOR_JAVA
    private const string YOUR_SELECT_FOR_GAME =
      "Чтобы снова начать игру, cоединитесь со сервером";
    private const string CLIENT_SELECTED = "Ожидайте второго игрока";
#else
    private const string YOUR_SELECT_FOR_GAME =
      "Чтобы снова начать игру, выберите роль: сервер или клиент";
    private const string CLIENT_SELECTED = "Ваша роль клиент";
#endif
    private const string SERVER_SELECTED = "Ваша роль сервер";
    private const string YOUR_MOVE = "Ваш ход";
    private const string OTHER_MOVE = "Ход соперника";
    private const string OBSERVER = "Вы наблюдатель";

    private const string YOU_WON = "Вы победили!";
    private const string YOU_LOSE = "Вы проиграли!";
    private const string MATCH_DRAWN = "Ничья!";

    public NoughtsAndCrossesGame() {
      context = new GameContext();
      context.game = this;
      connector = new Connector(context);
      connector.OnReceiveFirstData += (token) => OnReceiveFirstData(token);
      connector.OnClientConnected += (session) => OnInternalClientConnected(session);
      connector.OnDisConnected += (byUser) => {
        OnInternalDisConnected(byUser && context.gameCtrl.IsGameFinished);
      };
      connector.OnServerStarted += () => OnInternalServerStarted();
      connector.OnServerStopped += () => OnInternalServerStopped();
      context.Connector = connector;
      gameCtrl = new GameCtrl(context);
      gameCtrl.OnCellValueChanged += (number, cellValue, myMove) => OnInternalCellValueChanged(number, cellValue, myMove);
      context.gameCtrl = gameCtrl;
    }

    ~NoughtsAndCrossesGame() {
      gameCtrl = null;
      connector.Stop();
      connector = null;
      context = null;
    }

    public event Action<int, int> OnGameDurationChanged;
    public event Action<string> OnGameStatusChanged;
    public event Action<string> OnConnStatusChanged;
    public event Action<GameState> OnGameStateChanged;
    public event Action OnConnecting;
    public event Action<string, bool> OnFirstDataReceived;
    public event Action OnDisConnected;
    public event Action OnServerStarted;
    public event Action OnServerStopped;
    public event Action<ushort> OnRowCellCountChanged;
    public event Action<ushort> OnNumberToWinChanged;
    public event Action<int, char> OnCellValueChanged;
    public event Action<bool> OnMyFirstMoveChanged;

    public void SetFormData(NoughtsAndCrossesFormData aNoughtsAndCrossesFormData) {
      noughtsAndCrossesFormData = aNoughtsAndCrossesFormData;

      noughtsAndCrossesFormData.bCheckMyFirstMove = true;
      noughtsAndCrossesFormData.rowCellsCount = gameCtrl.NumberToWin;
      noughtsAndCrossesFormData.textIPAddress = "Ваш IP-адрес: " + TcpServer.GetAddress();

      noughtsAndCrossesFormData.bCheckMyFirstMove = true;
      noughtsAndCrossesFormData.rowCellsCount = gameCtrl.NumberToWin;
      noughtsAndCrossesFormData.textIPAddress = "Ваш IP-адрес: " + TcpServer.GetAddress();
      if (OnGameStatusChanged != null) {
        OnGameStatusChanged(YOUR_SELECT);
      }

      gameCtrl.Init(3);
    }

    #region Обработка действий пользователя

    /// <summary>
    /// Применить новые настройки игры
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void Apply() {
      gameCtrl.Init(noughtsAndCrossesFormData.rowCellsCount);
    }

    public void SetMyFirstMove(bool bMyFirstMove) {
      gameCtrl.SetMyFirstMove(bMyFirstMove);
    }

    public void MakeMove(string asNumber) {
      if (gameCtrl.IsObserver || !gameCtrl.MyMove) {
        return;
      }
      char cellValue = 'n';
      if (!gameCtrl.GetCellValue(asNumber, ref cellValue)) {
        return;
      }
      if (cellValue == GameCtrl.CELL_X || cellValue == GameCtrl.CELL_0) {
        return;
      }
      gameCtrl.MakeMove(asNumber, gameCtrl.MyCellValue);
    }

    public void Close() {
      connector.Stop();
    }

    public void SetCountToWin(int numberToWin) {
      gameCtrl.NumberToWin = numberToWin;
    }

#if FOR_JAVA
    public void SetUserName(string userName) {
      gameCtrl.UserName = userName;
    }
#endif

    public void Connect(bool bCreateGame, string token) {
#if FOR_JAVA
      if (String.IsNullOrEmpty(gameCtrl.UserName)) {
        MessageBox.Show("Введите имя пользователя", "Ошибка");
        return;
      }
      string chars = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM_123456789";
      foreach (char ch in gameCtrl.UserName) {
        if (!chars.Any(it => it == ch)) {
          MessageBox.Show("Имя пользователя должно состоят из английских букв или цифр или символа подчеркивания",
                          "Ошибка");
          return;
        }
      }
#endif

      string sIPAddr = string.Format("{0}.{1}.{2}.{3}",
                                     noughtsAndCrossesFormData.textIPAddress1,
                                     noughtsAndCrossesFormData.textIPAddress2,
                                     noughtsAndCrossesFormData.textIPAddress3,
                                     noughtsAndCrossesFormData.textIPAddress4
                                    );

      int port = 80;
#if FOR_JAVA
      if (!Int32.TryParse(noughtsAndCrossesFormData.textClientPort, out port)) {
        MessageBox.Show("Порт не корректный. Порт доолжен состят только из цифр",
                        "Ошибка");
        return;
      }
      if (!bCreateGame && String.IsNullOrEmpty(token)) {
        MessageBox.Show("Введите токен игры", "Ошибка");
        return;
      }
#endif
      if (OnConnecting != null) {
        OnConnecting();
      }
      if (OnGameStatusChanged != null) {
        OnGameStatusChanged(CLIENT_SELECTED);
      }

      connector.StartClient(sIPAddr, port, bCreateGame, token);
    }

    public void DisConnect() {
      connector.StopClient(true);
      gameCtrl.MyMove = false;
    }

    public void StartServer() {
      gameCtrl.SetMyFirstMove(noughtsAndCrossesFormData.bCheckMyFirstMove);
      gameCtrl.Init(noughtsAndCrossesFormData.rowCellsCount);
      connector.StartServer(80, "Server");
    }

    public void StopServer() {
      connector.StopServer();
      if (OnServerStopped != null) {
        OnServerStopped();
      }
    }

    public string Protocol {
      get { return connector.Protocol; }
      set { connector.Protocol = value; }
    }

    #endregion

    #region Параметры игры: получение и установка

    public override ushort GetRowCellCount() {
      return (ushort)noughtsAndCrossesFormData.rowCellsCount;
    }

    public override void SetRowCellCount(ushort rowCellsCount) {
      if (OnRowCellCountChanged != null) {
        OnRowCellCountChanged(rowCellsCount);
      }
      noughtsAndCrossesFormData.rowCellsCount = rowCellsCount;
    }

    public override void SetNumberToWin(ushort numberToWin) {
      gameCtrl.NumberToWin = numberToWin;
      if (OnNumberToWinChanged != null) {
        OnNumberToWinChanged(numberToWin);
      }
    }

    public void SetGameState(string data) {
      gameCtrl.SetGameState(data);
    }

    #endregion

    #region События коннектора

    private void OnInternalClientConnected(Session session) {
      try {
        bool bMyMove = gameCtrl.IsMyMove();
        gameCtrl.SetYourMove(noughtsAndCrossesFormData.bCheckMyFirstMove, bMyMove);
      }
      catch (System.Exception ex) {
        MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK);
      }
    }

    private void OnInternalServerStarted() {
      if (OnServerStarted != null) {
        OnServerStarted();
      }
      if (OnGameStatusChanged != null) {
        OnGameStatusChanged(SERVER_SELECTED);
      }
    }

    private void OnInternalServerStopped() {
      if (OnServerStopped != null) {
        OnServerStopped();
      }
    }

    private void OnReceiveFirstData(string token) {
      if (OnFirstDataReceived != null) {
        OnFirstDataReceived(token, gameCtrl.IsObserver);
      }
    }

    private void OnInternalDisConnected(bool byUser) {
      if (OnDisConnected != null) {
        OnDisConnected();
      }
      if (!byUser) {
        if (OnGameStatusChanged != null) {
          OnGameStatusChanged(YOUR_SELECT_FOR_GAME);
        }
      }
    }

    public override void OnServerError(string errorMsg) {
      MessageBox.Show(errorMsg, "Ошибка", MessageBoxButtons.OK);
    }

    public override void OnConnectionError(string errorMsg) {
      try {
        //MessageBox.Show(errorMsg, "Ошибка", MessageBoxButtons.OK);
        if (OnDisConnected != null) {
          OnDisConnected();
        }
        if (OnGameStatusChanged != null) {
          OnGameStatusChanged(YOUR_SELECT_FOR_GAME);
        }
        if (OnConnStatusChanged != null) {
          OnConnStatusChanged(errorMsg);
        }
#if FOR_JAVA
        if (OnMyFirstMoveChanged != null) {
          OnMyFirstMoveChanged(context.gameCtrl.IsMyFirstMove());
        }
#endif
      }
      catch (System.Exception ex) {
        MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK);
      }
    }

    public override void OnReceivingError(string type, string errorMsg) {
      if (type == Session.RECEIVE_FATAL_ERROR) {
        OnConnectionError(errorMsg);
        return;
      }
      //MessageBox.Show(errorMsg, "Ошибка", MessageBoxButtons.OK);
      if (OnConnStatusChanged != null) {
        OnConnStatusChanged(errorMsg);
      }
    }

    #endregion

    #region События контроллера игры

    public override void OnDuration(long time) {
      long gameTime = time / 1000;
      if (OnGameDurationChanged != null) {
        OnGameDurationChanged((int)(gameTime / 60), (int)(gameTime % 60));
      }
    }

    public override void OnSetGameState() {
      if (OnGameStateChanged != null) {
        OnGameStateChanged(gameCtrl.GetGameState());
      }
    }

    private void OnInternalCellValueChanged(int number, char cellValue, bool bMyMoved) {
      if (OnCellValueChanged != null) {
        OnCellValueChanged(number, cellValue);
      }
      if (gameCtrl.IsObserver) {
        if (OnGameStatusChanged != null) {
          OnGameStatusChanged(OBSERVER);
        }
      }
      else {
        if (OnGameStatusChanged != null) {
          OnGameStatusChanged(!bMyMoved ? YOUR_MOVE : OTHER_MOVE);
        }
        if (bMyMoved) {
          connector.SendMoveInfo((byte)number, cellValue);
        }
      }
    }

    public override void OnUpdateMyFirstMove() {
      if (OnMyFirstMoveChanged != null) {
        OnMyFirstMoveChanged(gameCtrl.IsMyFirstMove());
      }
      noughtsAndCrossesFormData.bCheckMyFirstMove = gameCtrl.IsMyFirstMove();
      if (gameCtrl.IsObserver) {
        if (OnGameStatusChanged != null) {
          OnGameStatusChanged(OBSERVER);
        }
      }
      else {
        if (OnGameStatusChanged != null) {
          OnGameStatusChanged(gameCtrl.MyMove ? YOUR_MOVE : OTHER_MOVE);
        }
      }
    }

    public override void OnGameEnded(int gameEndingID, string winUserName) {
      if (gameEndingID == -2) {
        if (OnGameStatusChanged != null) {
          OnGameStatusChanged(YOUR_SELECT_FOR_GAME);
        }
        return;
      }
      if (winUserName != null) {
        Thread.Sleep(100);
        connector.Stop();
      }
#if FOR_JAVA
      if (gameCtrl.IsObserver) {
        if (!String.IsNullOrEmpty(winUserName)) {
          if (OnGameStatusChanged != null) {
            OnGameStatusChanged(String.Format("Выиграл игрок '{0}'", winUserName));
          }
        }
        else {
          if (OnGameStatusChanged != null) {
            OnGameStatusChanged("Ничья");
          }
        }

        return;
      }
#endif
      string asMsg = gameEndingID == 1 ? YOU_WON : gameEndingID == -1 ? YOU_LOSE : MATCH_DRAWN;
      if (OnGameStatusChanged != null) {
        OnGameStatusChanged(String.Format("{0} {1}", asMsg, YOUR_SELECT_FOR_GAME));
      }
    }

    #endregion
  }
}
