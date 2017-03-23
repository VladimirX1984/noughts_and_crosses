using System;
using System.Collections.Generic;
using System.Net;
using NoughtsAndCrosses.Utils;

namespace NoughtsAndCrosses.Connection.HTTP {
  public class HttpClient : ConnectManager, IHttpClient {


    #region Реализация интерфейса IClient

    public void SetSession(Session aSession) {
      session = aSession;
    }

    public bool Connect(string sIpAddr, int aPort) {
      port = aPort;
      return Connect(sIpAddr);
    }

    public bool Connect(string sIpAddr) {
      if (session == null || session.GetConnectionInfo() == null) {
        OnConnectionError("Внутрення ошибка программы");
        return false;
      }

      httpConnection = new HttpConnection(String.Format("http://{0}:{1}", sIpAddr, port));
      connectInfo = (HttpConnectionInfo)session.GetConnectionInfo();
      connectInfo.httpConnection = httpConnection;
      connectInfo.session = session;
      return true;
    }

    public void DisConnect(bool byUser, bool bAppExiting) {
      if (byUser) {
        ((HttpClientSession)session).SetExit(bAppExiting);
      }
      CloseConnection(connectInfo, "");
    }

    #endregion

    #region Реализация интерфейса IHttpClient

    public void SendData(IConnectionInfo connect, string method, string command, Headers headers,
                         DataBuffer data) {
      try {
        httpConnection.SendData(method, command, headers, data, session.ReceiveData);
        if (httpConnection.StatusCode != HttpStatusCode.OK) {
          session.OnConnectionError(command, httpConnection.StatusDescription);
          return;
        }
      }
      catch (WebException ex) {
        session.OnConnectionError(command, ex.Message);
      }
      catch (ProtocolViolationException ex) {
        session.OnConnectionError(command, ex.Message);
      }
    }

    public void SendJson(IConnectionInfo connect, string method, string command, Headers headers,
                         string json) {
      try {
        httpConnection.SendJsonData(method, command, headers, json, session.ReceiveData);
        if (httpConnection.StatusCode != HttpStatusCode.OK) {
          session.OnConnectionError(command, httpConnection.StatusDescription);
          return;
        }
      }
      catch (WebException ex) {
        session.OnConnectionError(command, ex.Message);
      }
      catch (ProtocolViolationException ex) {
        session.OnConnectionError(command, ex.Message);
      }
    }

    #endregion

    #region Реализация абстрактного класса ConnectManager

    /// <summary>
    /// Посылает данные клиенту
    /// </summary>
    /// <param name="connect">Содинение</param>
    /// <param name="data">Массив с данными</param>
    public override void Send(IConnectionInfo connect, byte[] data, uint size) {
      try {
        httpConnection.Send(new DataBuffer(data, size), session.ReceiveData);
        if (httpConnection.StatusCode != HttpStatusCode.OK) {
          session.OnConnectionError("", httpConnection.StatusDescription);
          return;
        }
      }
      catch (WebException ex) {
        session.OnConnectionError("", ex.Message);
      }
      catch (ProtocolViolationException ex) {
        session.OnConnectionError("", ex.Message);
      }
    }



    public override void CloseConnection(IConnectionInfo connect, string sMsg) {
      HttpConnectionInfo connectInfo = (HttpConnectionInfo)connect;
      if (connectInfo != null && connectInfo.httpConnection != null) {
        connectInfo.session = null;
        connectInfo.httpConnection = null;
      }
    }

    #endregion

    private IHttpConnection httpConnection;

    /// <summary>
    /// Информация о соединения
    /// </summary>
    private HttpConnectionInfo connectInfo;

    /// <summary>
    /// порт
    /// </summary>
    private int port;

    /// <summary>
    /// сессия клиента
    /// </summary>
    private Session session;


    private void OnConnectionError(string asError) {
      session.OnConnectionError("", asError);
    }
  }
}
