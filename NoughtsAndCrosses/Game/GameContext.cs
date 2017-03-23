using NoughtsAndCrosses.Connection;

namespace NoughtsAndCrosses.Game {
  public class GameContext {
    public ISessionFactory Connector;
    public GameBase game;
    public GameCtrl gameCtrl;
  }
}
