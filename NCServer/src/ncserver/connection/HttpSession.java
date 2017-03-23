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

/**
 *
 * @author Vladimir
 */
public abstract class HttpSession extends AbstractSession implements IHttpSession {

    // <editor-fold defaultstate="collapsed" desc="Реализация абстрактного AbstractSession">        
    @Override
    public boolean receiveData(Object data, int size) {
        // Принимаем данные - копируем данные в буфер            
        DataReader dataReader = new DataReader((byte[])data, 0, size);

        if (dataReader.getDataSize() < 3) {
            onReceivingError("Internal error: no message");
            return false;
        }
        String command = "";
        AtomicReference<String> commandRef = new AtomicReference<>(command);
        dataReader.readASCII(commandRef);
        command = commandRef.get();
        int headerSize = command.length();

        MessageHandler handler = inMessageHandlers.get(command);

        // Проверяем есть ли обработчик для сообщения такого типа
        if (handler == null) {
            onReceivingError("Internal error: unknown command");
            return false;
        }

        if (dataReader.getDataSize() < headerSize + 2) {
            onReceivingError("Internal error: no data");
            return false;
        }

        // сообщения с переменным размером - длину сообщения определяем из поля <размер данных>
        byte[] dataBuffer = new byte[dataReader.getDataSize() - dataReader.getPosition()];
        AtomicReference<byte[]> dataBufferRef = new AtomicReference<>(dataBuffer);
        dataReader.readArray(dataBufferRef);
        try {
            // Обрабатываем сообщение - вызываем соответствующий обработчик                        
            return handler.call(null, dataBufferRef.get(), dataBuffer.length);
        }
        catch (Exception exc) {
            String s = String.format("Internal error %s", exc.getMessage());
            onReceivingError(s);
            return false;
        }
    }
    // </editor-fold>

    // <editor-fold defaultstate="collapsed" desc="Реализация интерфейса IHttpSession">     
    @Override
    public boolean receiveData(String command, Headers headers, Object data, int size) {
        MessageHandler handler = inMessageHandlers.get(command);
        // Проверяем есть ли обработчик для сообщения такого типа
        if (handler == null) {
            onReceivingError("Internal error: unknown command");
            return false;
        }
        try {
            // Обрабатываем сообщение - вызываем соответствующий обработчик                        
            return handler.call(headers, (byte[])data, size);
        }
        catch (Exception exc) {
            String s = String.format("Internal error %s", exc.getMessage());
            onReceivingError(s);
            return false;
        }
    }
    // </editor-fold>

    public HttpSession(IConnectManager connManager, IConnectionInfo connection) {
        connectManager = connManager;
        connectInfo = connection;
    }

    protected abstract class MessageHandler {
        protected abstract boolean call(Headers headers, byte[] data, int size);
    }

    private final HashMap<Object, MessageHandler> inMessageHandlers = new HashMap<Object, MessageHandler>();

    protected final void addMessageHandler(Object type, MessageHandler messageHandlers) {
        inMessageHandlers.put(type, messageHandlers);
    }

    protected void onReceivingError(String errorMsg) {
        System.err.println(errorMsg);
    }

    @Override
    protected void onSessionClosing() {
        super.onSessionClosing();
        inMessageHandlers.clear();
    }
}
