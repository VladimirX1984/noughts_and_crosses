namespace NoughtsAndCrosses.Connection {
  public abstract class ConnectManager : IConnectManager {

    public abstract void Send(IConnectionInfo connect, byte[] data, uint size);

    public abstract void CloseConnection(IConnectionInfo connect, string sMsg);
  }
}
