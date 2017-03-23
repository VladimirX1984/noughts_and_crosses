using System.Collections.Generic;
using NoughtsAndCrosses.Utils;

namespace NoughtsAndCrosses.Connection.HTTP {
  public interface IHttpClient : IClient {

    void SendData(IConnectionInfo connect, string method, string command, Headers headers, DataBuffer data);

    void SendJson(IConnectionInfo connect, string method, string command, Headers headers, string json);
  }
}
