using System.Net.Sockets;

namespace NoughtsAndCrosses.Connection.TCP {
  public class TcpConnectionInfo : IConnectionInfo {

    #region Реализация интерфейса IConnectionInfo

    public Session session { get; set; }

    #endregion

    public Socket socket;
    public byte[] buffer;
    public bool isClosing;
  }
}
