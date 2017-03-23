using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using NoughtsAndCrosses.Utils;

namespace NoughtsAndCrosses.Connection.HTTP {
  public class HttpConnection : IHttpConnection {

    #region Реализация интерфейса IHttpConnection

    public string Url { get; set; }

    public DataBuffer Data { get; set; }

    public string Response { get; private set; }

    public int Timeout { get; set; }

    public DataBuffer ResultData {
      get {
        DataBuffer dataBuffer = new DataBuffer();
        dataBuffer.AddASCII(Response);
        return dataBuffer;
      }
    }

    public HttpStatusCode StatusCode { get; private set; }

    public string StatusDescription { get; private set; }

    public void Send() {
      SendData("POST", null, Data, null);
    }

    public void Send(DataBuffer data) {
      SendData("POST", null, data, null);
    }

    public void Send(DataBuffer data, Action<string, byte[], int> onReceive) {
      SendData("POST", null, data, onReceive);
    }

    public void SendData(string method, string path, DataBuffer data, Action<string, byte[], int> onReceive) {
      SendData(method, path, null, data, onReceive);
    }

    public void SendData(string method, string path, Headers headers, DataBuffer data,
                         Action<string, byte[], int> onReceive) {
      var dataBuffer = SendData(method, path, headers, data, "application/x-www-form-urlencoded");
      if (StatusCode == HttpStatusCode.OK && dataBuffer != null) {
        if (onReceive != null) {
          onReceive(path, dataBuffer.GetBuffer(), (int)dataBuffer.GetBufferSize());
        }
        else {
          SetResponse(dataBuffer);
        }
      }
    }

    public void SendData(string method, string path, Headers headers, DataBuffer data,
                         Action<string, DataBuffer> onReceive) {
      var dataBuffer = SendData(method, path, headers, data, "application/x-www-form-urlencoded");
      if (StatusCode == HttpStatusCode.OK && dataBuffer != null) {
        if (onReceive != null) {
          onReceive(path, dataBuffer);
        }
        else {
          SetResponse(dataBuffer);
        }
      }
    }

    public void SendJsonData(string method, string path, string json, Action<string, byte[], int> onReceive) {
      SendJsonData(method, path, null, json, onReceive);
    }

    public void SendJsonData(string method, string path, Headers headers, string json,
                             Action<string, byte[], int> onReceive) {
      if (String.IsNullOrEmpty(json)) {
        json = "{ }";
      }
      var dataBuffer = SendStringData(method, path, headers, json, "application/json");
      if (StatusCode == HttpStatusCode.OK && dataBuffer != null) {
        if (onReceive != null) {
          onReceive(path, dataBuffer.GetBuffer(), (int)dataBuffer.GetBufferSize());
        }
        else {
          SetResponse(dataBuffer);
        }
      }
    }

    public void SendJsonData(string method, string path, Headers headers, string json,
                             Action<string, DataBuffer> onReceive) {
      if (String.IsNullOrEmpty(json)) {
        json = "{ }";
      }
      var dataBuffer = SendStringData(method, path, headers, json, "application/json");
      if (StatusCode == HttpStatusCode.OK && dataBuffer != null) {
        if (onReceive != null) {
          onReceive(path, dataBuffer);
        }
        else {
          SetResponse(dataBuffer);
        }
      }
    }

    #endregion

    public HttpConnection(string url) {
      Url = url;
      Init();
    }

    public HttpConnection(string url, DataBuffer data) {
      Url = url;
      Data = data;
      Init();
    }

    public HttpConnection(string url, string data) {
      Url = url;
      Data = new DataBuffer();
      Data.AddASCII(data);
      Init();
    }

    private void Init() {
      Timeout = 1000;
    }

    private HttpWebRequest CreateWebRequest(string method, string path, Headers headers, string contentType, int size) {
      string url = Url;
      if (!String.IsNullOrEmpty(path)) {
        url = path[0] == '/' ? String.Format("{0}{1}", Url, path) : String.Format("{0}/{1}", Url, path);
      }
      HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.CreateHttp(url);
      httpWReq.Timeout = Timeout;
      httpWReq.Method = method;
      if (httpWReq.Method.ToUpper() == "GET") {
        httpWReq.Accept = contentType;
      }
      else {
        httpWReq.ContentType = contentType;
        httpWReq.ContentLength = size;
      }
      if (headers != null) {
        foreach (var pair in headers) {
          httpWReq.Headers.Add(pair.Key, pair.Value);
        }
      }
      return httpWReq;
    }

    private DataBuffer SendData(string method, string path, Headers headers, DataBuffer data, string contentType) {
      HttpWebRequest httpWReq = CreateWebRequest(method, path, headers, contentType, (int)data.GetBufferSize());
      if (httpWReq.Method.ToUpper() != "GET") {
        using (Stream stream = httpWReq.GetRequestStream()) {
          stream.Write(data.GetBuffer(), 0, (int)data.GetBufferSize());
        }
      }

      HttpWebResponse response = (HttpWebResponse)httpWReq.GetResponse();
      StatusCode = response.StatusCode;
      StatusDescription = response.StatusDescription;
      if (StatusCode == HttpStatusCode.OK) {
        var stream = response.GetResponseStream();
        var binStream = new BinaryReader(stream);
        var dataBuffer = new DataBuffer();
        string jsonRes = String.Empty;
        dataBuffer.AddData(binStream);
        return dataBuffer;
      }
      return null;
    }

    private DataBuffer SendStringData(string method, string path, Headers headers, string json, string contentType) {
      HttpWebRequest httpWReq = CreateWebRequest(method, path, headers, contentType, json.Length);
      if (httpWReq.Method.ToUpper() != "GET") {
        using (Stream stream = httpWReq.GetRequestStream()) {
          var dataBuffer = new DataBuffer(json);
          stream.Write(dataBuffer.GetBuffer(), 0, (int)dataBuffer.GetBufferSize());
        }
      }

      HttpWebResponse response = (HttpWebResponse)httpWReq.GetResponse();
      StatusCode = response.StatusCode;
      StatusDescription = response.StatusDescription;
      if (StatusCode == HttpStatusCode.OK) {
        var stream = response.GetResponseStream();
        var binStream = new BinaryReader(stream);
        var dataBuffer = new DataBuffer();
        string jsonRes = String.Empty;
        dataBuffer.AddData(binStream);
        return dataBuffer;
      }
      return null;
    }

    private void SetResponse(DataBuffer dataBuffer) {
      var dataReader = new DataReader(dataBuffer);
      string res = string.Empty;
      dataReader.ReadString(ref res);
      Response = res;
    }
  }
}
