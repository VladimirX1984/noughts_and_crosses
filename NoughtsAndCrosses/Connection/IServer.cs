namespace NoughtsAndCrosses.Connection {
  public interface IServer : IConnectManager {
    bool IsRunning();

    void Start();

    void Stop();
  }
}
