namespace NoughtsAndCrosses.Connection {
  public interface IConnectManager {
    void Send(IConnectionInfo connect, byte[] data, uint size);

    void CloseConnection(IConnectionInfo connect, string sMsg);
  }
}
