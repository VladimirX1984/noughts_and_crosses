using System;

namespace NoughtsAndCrosses.Game {
  public class GameCtrl {
    /// <summary>
    /// состояние игры
    /// </summary>
    private GameState _gameState;

    /// <summary>
    /// определение состояние игры
    /// </summary>
    private GameStateChecker _gameStateChecker;

    /// <summary>
    /// контекст игры
    /// </summary>
    private GameContext _context;

    /// <summary>
    /// определяет чей первый ход
    /// </summary>
    private bool _bMyFirstMove;

    public const char CELL_X = 'X';
    public const char CELL_0 = '0';

    public GameCtrl(GameContext aContext) {
      _bMyFirstMove = true;
      _numberToWin = 3;
      _bMyMove = false;
      _context = aContext;
      _gameState = new GameState();
      _gameStateChecker = new GameStateChecker();
    }

    public Action<int, char, bool> OnCellValueChanged;

    public string UserName { get; set; }

    /// <summary>
    /// мой ход, если значение true
    /// </summary>
    public bool MyMove {
      get { return _bMyMove; }
      set { _bMyMove = value; }
    }
    private bool _bMyMove;

    /// <summary>
    /// содержимое ячейки
    /// </summary>
    public char MyCellValue {
      get { return _myCellValue; }
      set { _myCellValue = value; }
    }
    private char _myCellValue;

    /// <summary>
    /// Число отмеченных клетов в ряду или диагонале для достижения победы
    /// </summary>
    public int NumberToWin {
      get { return _numberToWin; }
      set { _numberToWin = value; }
    }
    private int _numberToWin;

    public bool IsGameCreator { get; set; }

    public bool IsObserver {
      get { return _isObserver; }
      set { _isObserver = value; }
    }
    private bool _isObserver;

    public bool IsGameFinished { get; set; }

    public void Init(int count) {
      IsGameFinished = false;
      _isObserver = false;
      _gameState.Init(count);
      IsGameCreator = false;
      _context.game.OnSetGameState();
    }

    public void SetGameState(string data) {
      if (_gameState.DataString != data) {
        _gameState.DataString = data;
        _context.game.OnSetGameState();
#if !FOR_JAVA
        CheckGameEnded(null);
#endif
      }
    }

    public void FisishGame(int id, string winUserName) {
      if (IsGameFinished) {
        _bMyMove = false;
        return;
      }
      IsGameFinished = true;
      OnGameEnded(id, winUserName);
      _bMyMove = false;
    }

    /// <summary>
    /// сделать ход: мой или чужой
    /// </summary>
    /// <param name="asNumber"></param>
    /// <param name="cellValue"></param>
    /// <returns></returns>
    public bool MakeMove(string asNumber, char cellValue) {
      SetCellValue(asNumber, cellValue);
      _bMyMove = !_bMyMove;
#if FOR_JAVA
      if (!IsObserver) {
        CheckGameEnded(null);
      }
#else
      CheckGameEnded(null);
#endif
      return true;
    }

    /// <summary>
    /// сделать ход: мой или чужой
    /// </summary>
    /// <param name="asNumber"></param>
    /// <param name="cellValue"></param>
    /// <returns></returns>
    public bool MakeMove(int number, char cellValue) {
      SetCellValue(number, cellValue);
      _bMyMove = !_bMyMove;
#if !FOR_JAVA
      CheckGameEnded(null);
#endif
      return true;
    }

    public bool SetCellValue(string asNumber, char cellValue) {
      int number = -1;
      bool b = Int32.TryParse(asNumber, out number);
      if (!b) {
        return false;
      }
      SetCellValue(number, cellValue);
      return true;
    }

    public void SetCellValue(int number, char cellValue) {
      _gameState.SetCellValue(number, cellValue);
      if (OnCellValueChanged != null) {
        OnCellValueChanged(number, cellValue, _bMyMove);
      }
      if (IsGameEnded()) {
        _bMyMove = false;
      }
    }

    public bool GetCellValue(string asNumber, ref char cellValue) {
      int number = -1;
      bool b = Int32.TryParse(asNumber, out number);
      if (!b) {
        return false;
      }
      cellValue = _gameState.GetCellValue(number);
      return true;
    }

    public char GetCellValue(int number) {
      return _gameState.GetCellValue(number);
    }

    public void SetYourMove(bool abMyFirstMove, bool abYourMove) {
      _bMyMove = abYourMove;
      SetMyFirstMove(abMyFirstMove);
      _myCellValue = abMyFirstMove ? CELL_X : CELL_0;
      _context.game.OnUpdateMyFirstMove();
    }

    public void SetMyFirstMove(bool abMyFirstMove) {
      _bMyFirstMove = abMyFirstMove;
    }

    public bool IsMyFirstMove() {
      return _bMyFirstMove;
    }

    public bool IsMyMove() {
      return _gameStateChecker.IsAnyCellFilled(_gameState) ? _bMyMove : IsMyFirstMove();
    }

    public GameState GetGameState() {
      return _gameState;
    }

    public bool IsGameEnded() {
      int iWinner = _gameStateChecker.GetWinner(_gameState, _numberToWin);
      if (iWinner == GameStateChecker.WIN_X && IsMyFirstMove()
          || iWinner == GameStateChecker.WIN_0 && !IsMyFirstMove()) {
        return true;
      }
      if (iWinner == GameStateChecker.WIN_X && !IsMyFirstMove()
          || iWinner == GameStateChecker.WIN_0 && IsMyFirstMove()) {
        return true;
      }
      if (iWinner == GameStateChecker.MATCH_DRAWN) {
        return true;
      }
      return false;
    }

    public void CheckGameEnded(string winUserName) {
      int iWinner = _gameStateChecker.GetWinner(_gameState, _numberToWin);
      if (iWinner == GameStateChecker.WIN_X && IsMyFirstMove()
          || iWinner == GameStateChecker.WIN_0 && !IsMyFirstMove()) {
        OnGameEnded(1, winUserName);
#if FOR_JAVA
        _bMyMove = false;
#endif
        return;
      }
      if (iWinner == GameStateChecker.WIN_X && !IsMyFirstMove()
          || iWinner == GameStateChecker.WIN_0 && IsMyFirstMove()) {
        OnGameEnded(-1, winUserName);
#if FOR_JAVA
        _bMyMove = false;
#endif
        return;
      }
      if (iWinner == GameStateChecker.MATCH_DRAWN) {
        OnGameEnded(0, winUserName);
#if FOR_JAVA
        _bMyMove = false;
#endif
        return;
      }
    }

    protected void OnGameEnded(int gameEndingID, string winUserName) {
      _context.game.OnGameEnded(gameEndingID, winUserName);
    }
  }
}
