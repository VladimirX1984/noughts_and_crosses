using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using NoughtsAndCrosses.Utils;

namespace NoughtsAndCrosses.Connection.HTTP {
  public class HttpSessionBasedOnJson : HttpSession {

    public override void ReceiveData(string command, byte[] data, int size) {
      if (!inMessageHandlers.ContainsKey(command)) {
        OnReceivingError(RECEIVE_FATAL_ERROR, "Internal error: unknown command");
        return;
      }
      try {
        DataReader dataReader = new DataReader(data, 0, size);
        string json = String.Empty;
        dataReader.ReadString(ref json);
        var jsonObject = JObject.Parse(json);
        string status = (string)jsonObject["status"];
        if (status == "error") {
          JToken token = null;
          if (jsonObject.TryGetValue("msg_id", out token)) {
            OnReceivingError(command, (int)token, (string)jsonObject["message"]);
            return;
          }
          OnReceivingError(command, (string)jsonObject["message"]);
          return;
        }
        if (status != "ok") {
          OnReceivingError(command, "Unknown error");
          return;
        }
        // Обрабатываем сообщение - вызываем соответствующий обработчик
        inMessageHandlers[command](jsonObject);
      }
      catch (Exception exc) {
        string s = string.Format("Internal error {0}", exc.Message);
        OnReceivingError(Session.RECEIVE_FATAL_ERROR, s);
        return;
      }
    }

    public HttpSessionBasedOnJson(IConnectManager connManager, IConnectionInfo connection)
    : base(connManager, connection) {

    }

    protected delegate bool JSONMessageHandler(JObject jsonObject);

    private Dictionary<object, JSONMessageHandler> inMessageHandlers = new Dictionary<object, JSONMessageHandler>();

    protected void addMessageHandler(object type, JSONMessageHandler messageHandlers) {
      inMessageHandlers.Add(type, messageHandlers);
    }

    protected virtual void OnReceivingError(string type, int msgId, string errorMsg) {

    }
  }
}
