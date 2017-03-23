using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NoughtsAndCrosses.Connection;
using NoughtsAndCrosses.Connection.HTTP;
using NoughtsAndCrosses.Game;
using NoughtsAndCrosses.Utils;

namespace NoughtsAndCrosses {
  public class HttpClientSession : HttpSessionBasedOnJson {

    /// <summary>
    /// контекст игры
    /// </summary>
    private GameContext context;

    private const string inGameCreated = "/new_game";
    private const string inGameJoined = "/join_game";
    private const string inMoveMaked = "/make_a_move";
    private const string inGameState = "/state";
    private const string inExit = "/exit";

    public HttpClientSession(IConnectManager connectHandler, IConnectionInfo connection, GameContext aContext)
    : base(connectHandler, connection) {
      context = aContext;

      addMessageHandler(inGameCreated, OnGameCreated);
      addMessageHandler(inGameJoined, OnGameJoined);
      addMessageHandler(inMoveMaked, OnMakeMove);
      addMessageHandler(inGameState, OnGameState);
      addMessageHandler(inExit, OnExit);
    }

    public event Action<string> OnReceiveFirstData;

    private string accessToken = string.Empty;
    private string gameToken = string.Empty;
    private bool bGameBeginEnabled = false;

    #region Прием сообщений

    private bool OnGameCreated(JObject jsonObject) {
      accessToken = (string)jsonObject["access_token"];
      gameToken = (string)jsonObject["game_token"];
      context.gameCtrl.IsGameCreator = true;
      if (OnReceiveFirstData != null) {
        OnReceiveFirstData(gameToken);
      }
      bCheckGameState = false;
      bGameBeginEnabled = false;
      TimeLimitedOperationsServiceSingleton.AddOperation(SetStateGameQuery);
      return true;
    }

    private bool OnGameJoined(JObject jsonObject) {
      accessToken = (string)jsonObject["access_token"];
      int mode = (int)jsonObject["mode"];
      int rowCellCount = (int)jsonObject["size"];
      int numberToWin = 3;
      JToken jNumberToWin;
      if (jsonObject.TryGetValue("number_to_win", out jNumberToWin)) {
        numberToWin = (int)jNumberToWin;
      }
      bool bMyFirstMove = false;
      JToken jbMyFirstMove;
      if (jsonObject.TryGetValue("your_first_turn", out jbMyFirstMove)) {
        bMyFirstMove = (bool)jbMyFirstMove;
      }
      context.gameCtrl.Init(rowCellCount);
      context.gameCtrl.IsObserver = mode == 2;
      context.gameCtrl.SetYourMove(bMyFirstMove, false);
      context.game.SetRowCellCount((ushort)rowCellCount);
      context.game.SetNumberToWin((ushort)numberToWin);
      context.game.OnUpdateMyFirstMove();
      if (OnReceiveFirstData != null) {
        OnReceiveFirstData(gameToken);
      }
      TimeLimitedOperationsServiceSingleton.AddOperation(SetStateGameQuery);
      return true;
    }

    private bool OnMakeMove(JObject jsonObject) {
      bCheckGameState = false;

      if (context.gameCtrl.IsGameEnded()) {
        context.gameCtrl.FisishGame(1, string.Empty);
      }

      return true;
    }

    private bool OnGameState(JObject jsonObject) {
      if (!bCheckGameState) {
        bCheckGameState = true;
        return true;
      }

      bool bMyMove = (bool)jsonObject["you_turn"];
      long timeDuration = (long)jsonObject["game_duration"];
      JArray jField = (JArray)jsonObject["field"];
      string sdata = string.Empty;
      foreach (var jRow in jField) {
        sdata += (string)jRow;
      }

      context.game.OnDuration(timeDuration);
      JToken winner = GameStateChecker.NONE;
      bool bSucc = jsonObject.TryGetValue("winner", out winner);
      if (!bSucc || (int)winner == GameStateChecker.NONE) {
        if (context.gameCtrl.IsGameCreator && context.gameCtrl.IsMyFirstMove()) {
          if (!bGameBeginEnabled && !bMyMove) {
            return true;
          }
          if (bMyMove) {
            bGameBeginEnabled = true;
          }
        }
        context.gameCtrl.SetYourMove(context.gameCtrl.IsMyFirstMove(), bMyMove);
        context.gameCtrl.SetGameState(sdata);
        return true;
      }

      bCheckGameState = false;
      JToken jwinUserName;
      string winUserName = string.Empty;
      if (jsonObject.TryGetValue("winner_name", out jwinUserName)) {
        winUserName = (string)jwinUserName;
      }
      int id = -1;
      switch ((int)winner) {
        case GameStateChecker.MATCH_DRAWN: {
          id = 0;
        }
        break;
        case GameStateChecker.WIN_X: {
          id = context.gameCtrl.MyCellValue == 'X' ? 1 : -1;
        }
        break;
        case GameStateChecker.WIN_0: {
          id = context.gameCtrl.MyCellValue == '0' ? 1 : -1;
        }
        break;
      }
      TimeLimitedOperationsServiceSingleton.RemoveOperation(SetStateGameQuery);
      context.gameCtrl.SetGameState(sdata);
      context.gameCtrl.FisishGame(id, winUserName);
      return true;
    }

    private bool OnExit(JObject jsonObject) {
      if (context.gameCtrl.IsObserver && !context.gameCtrl.IsGameFinished) {
        context.gameCtrl.FisishGame(-2, string.Empty);
      }
      return true;
    }

    public override void OnConnectionError(string type, string errorMsg) {
      if (bExiting) {
        int id = (context.gameCtrl.IsObserver || context.gameCtrl.IsGameFinished) ? -2 : -1;
        context.gameCtrl.FisishGame(id, string.Empty);
        TimeLimitedOperationsServiceSingleton.RemoveOperation(SetStateGameQuery);
        bExiting = false;
      }
      else {
        if (type == outCreateGame || type == outJoinGame) {
          context.game.OnConnectionError(errorMsg);
        }
        else {
          context.game.OnReceivingError(type, errorMsg);
        }
      }
    }

    protected override void OnReceivingError(string type, string errorMsg) {
      if (type == RECEIVE_FATAL_ERROR || type == outCreateGame || type == outJoinGame) {
        TimeLimitedOperationsServiceSingleton.RemoveOperation(SetStateGameQuery);
        context.game.OnReceivingError(RECEIVE_FATAL_ERROR, errorMsg);
      }
      else {
        context.game.OnReceivingError(type, errorMsg);
      }
    }

    protected override void OnReceivingError(string type, int msgId, string errorMsg) {
      if (type == RECEIVE_FATAL_ERROR || type == outCreateGame || type == outJoinGame) {
        TimeLimitedOperationsServiceSingleton.RemoveOperation(SetStateGameQuery);
        context.game.OnReceivingError(RECEIVE_FATAL_ERROR, errorMsg);
      }
      else  if (type == outGameState && msgId == 1) {
        TimeLimitedOperationsServiceSingleton.RemoveOperation(SetStateGameQuery);
        context.game.OnReceivingError(RECEIVE_FATAL_ERROR, errorMsg);
      }
      else {
        context.game.OnReceivingError(type, errorMsg);
      }
    }

    #endregion

    #region Отправка сообщений

    private const string outCreateGame = "/new_game";
    private const string outJoinGame = "/join_game";
    private const string outMoveInfo = "/make_a_move";
    private const string outGameState = "/state";
    private const string outGameSessionsInfo = "/get_sessions";
    private const string outSurrender = "/surrender";
    private const string outExit = "/exit";

    private volatile bool bCheckGameState = true;
    private volatile bool bExiting = false;

    private class CreateGameInfo {
      public string user_name;
      public int size;
      public int number_to_win;
      public int my_turn;
    }

    public void SendCreateGame() {
      CreateGameInfo createGameInfo = new CreateGameInfo();
      createGameInfo.user_name = context.gameCtrl.UserName;
      createGameInfo.my_turn = context.gameCtrl.IsMyFirstMove() ? 1 : 0;
      createGameInfo.number_to_win = context.gameCtrl.NumberToWin;
      createGameInfo.size = context.game.GetRowCellCount();
      string json = JsonConvert.SerializeObject(createGameInfo, Formatting.Indented);
      SendJsonData("POST", "/new_game", null, json);
    }

    public void SendJoinToGame(string token) {
      JTokenWriter writer = new JTokenWriter();
      writer.WriteStartObject();
      writer.WritePropertyName("game_token");
      writer.WriteValue(token);
      writer.WritePropertyName("user_name");
      writer.WriteValue(context.gameCtrl.UserName);
      writer.WriteEndObject();
      SendJsonData("POST", "/join_game", null, writer.Token.ToString());
    }

    public void SendMakeMove(short cellNumber, char cellValue) {
      int count = context.gameCtrl.GetGameState().Size;
      Headers headers = new Headers();
      headers.Add("access_token", accessToken);
      JObject rss = new JObject(new JProperty("row", cellNumber / count),
                                new JProperty("coll", cellNumber % count),
                                new JProperty("cellValue", String.Format("{0}", cellValue)));
      SendJsonData("POST", "/make_a_move", headers, rss.ToString());
    }

    public void SetStateGameQuery() {
      if (!bCheckGameState) {
        bCheckGameState = true;
        return;
      }
      Headers headers = new Headers();
      headers.Add("access_token", accessToken);
      headers.Add("game_creator", (context.gameCtrl.IsGameCreator ? 1 : 0).ToString());
      SendJsonData("GET", "/state", headers, string.Empty);
    }

    private volatile bool bExitSending = false;
    public void SetExit(bool bAppExiting) {
      if (bExitSending || context.gameCtrl.IsGameFinished) {
        return;
      }
      bExitSending = true;
      bExiting = true;
      Headers headers = new Headers();
      headers.Add("access_token", accessToken);
      headers.Add("game_creator", (context.gameCtrl.IsGameCreator ? 1 : 0).ToString());
      headers.Add("observer", (context.gameCtrl.IsObserver ? 1 : 0).ToString());
      JObject rss = new JObject();
      rss.Add("user_name", context.gameCtrl.UserName);
      rss.Add("exit", bAppExiting);
      if (context.gameCtrl.IsObserver) {
        TimeLimitedOperationsServiceSingleton.RemoveOperation(SetStateGameQuery);
      }
      SendJsonData("POST", "/exit", headers, rss.ToString());
      bExitSending = false;
    }

    #endregion
  }
}
