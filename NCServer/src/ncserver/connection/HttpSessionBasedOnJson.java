/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package ncserver.connection;

import com.sun.net.httpserver.Headers;
import java.util.HashMap;
import java.util.concurrent.atomic.AtomicReference;
import ncserver.utils.DataReader;
import org.json.JSONException;
import org.json.JSONObject;

/**
 *
 * @author Vladimir
 */
public class HttpSessionBasedOnJson extends HttpSession {

    @Override
    public boolean receiveData(String command, Headers headers, Object data, int size) {
        JsonMessageHandler handler = inJsonMessageHandlers.get(command);
        // Проверяем есть ли обработчик для сообщения такого типа
        if (handler == null) {
            onReceivingError("Internal error: unknown command");
            return false;
        }
        try {
            DataReader dataReader = new DataReader((byte[])data, 0, size);
            String json = "";
            AtomicReference<String> jsonRef = new AtomicReference<>(json);
            if (!dataReader.readString(jsonRef)) {
                onReceivingError("Json string is invalid");
                return false;
            }
            json = jsonRef.get();
            JSONObject jsonObject = json.length() > 1 ? new JSONObject(json) : new JSONObject("{}");
            // Обрабатываем сообщение - вызываем соответствующий обработчик                        
            return handler.call(headers, jsonObject);
        }
        catch (JSONException ex) {
            onReceivingError(ex.getMessage());
            return false;
        }
        catch (Exception exc) {
            String s = String.format("Internal error %s", exc.getMessage());
            onReceivingError(s);
            return false;
        }
    }

    public HttpSessionBasedOnJson(IConnectManager connManager, IConnectionInfo connection) {
        super(connManager, connection);
    }

    protected abstract class JsonMessageHandler {
        protected abstract boolean call(Headers headers, JSONObject jsonObject);
    }

    private final HashMap<Object, JsonMessageHandler> inJsonMessageHandlers = new HashMap<Object, JsonMessageHandler>();

    protected final void addMessageHandler(Object type, JsonMessageHandler messageHandlers) {
        inJsonMessageHandlers.put(type, messageHandlers);
    }

    @Override
    protected void onSessionClosing() {
        super.onSessionClosing();
        inJsonMessageHandlers.clear();
    }
}
