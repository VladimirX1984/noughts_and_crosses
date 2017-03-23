namespace NoughtsAndCrosses.Connection {
  public interface IClient : IConnectManager {
    void SetSession(Session aSession);

    bool Connect(string sIpAddr, int aPort);

    bool Connect(string sIpAddr);

    void DisConnect(bool byUser, bool bAppExiting);
  }
}
