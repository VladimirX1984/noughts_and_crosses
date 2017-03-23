using System;
using System.Collections.Generic;
using System.Net;
using NoughtsAndCrosses.Utils;

namespace NoughtsAndCrosses.Connection.HTTP {
  public interface IHttpConnection {
    string Url { get; set; }

    DataBuffer Data { get; set; }

    string Response { get; }

    int Timeout { get; set; }

    HttpStatusCode StatusCode { get; }

    string StatusDescription { get; }

    void Send();

    void Send(DataBuffer data);

    void Send(DataBuffer data, Action<string, byte[], int> onReceive);

    void SendData(string method, string command, DataBuffer data, Action<string, byte[], int> onReceive);

    void SendData(string method, string command, Headers headers, DataBuffer data,
                  Action<string, byte[], int> onReceive);

    void SendData(string method, string command, Headers headers, DataBuffer data,
                  Action<string, DataBuffer> onReceive);

    void SendJsonData(string method, string command, string json, Action<string, byte[], int> onReceive);

    void SendJsonData(string method, string command, Headers headers, string json,
                      Action<string, byte[], int> onReceive);

    void SendJsonData(string method, string command, Headers headers, string json,
                      Action<string, DataBuffer> onReceive);
  }
}
