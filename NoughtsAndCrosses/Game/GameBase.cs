namespace NoughtsAndCrosses.Game {
  public abstract class GameBase {
    public abstract void OnDuration(long time);

    public abstract void OnSetGameState();

    public abstract void OnUpdateMyFirstMove();

    public abstract void OnGameEnded(int gameEndingID, string winUserName);

    public abstract void OnConnectionError(string errorMsg);

    public abstract void OnReceivingError(string type, string errorMsg);

    public abstract void OnServerError(string errorMsg);

    public abstract ushort GetRowCellCount();

    public abstract void SetRowCellCount(ushort rowCellsCount);

    public abstract void SetNumberToWin(ushort numberToWin);
  }
}
