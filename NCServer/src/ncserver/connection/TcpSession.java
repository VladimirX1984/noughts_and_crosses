package ncserver.connection;

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
import java.util.concurrent.atomic.AtomicReference;
import ncserver.utils.DataBuffer;
import ncserver.utils.DataReader;

/**
 *
 * @author Vladimir
 */
public class TcpSession extends AbstractSession {

    // <editor-fold defaultstate="collapsed" desc="Реализация абстрактного AbstractSession">    
    @Override
    public boolean receiveData(Object data, int dataSize) {
        // Принимаем данные - копируем данные в буфер            
        DataReader dataReader = new DataReader((byte[])data, 0, dataSize);

        if (dataReader.getDataSize() < 3) {
            onReceivingError("Internal error: no message");
            return false;
        }
        // Формируем сообщение
        // по сигнатуре проверяем достоверность полученного сообщения
        short usignature = 0;
        AtomicReference<Short> usignatureRef = new AtomicReference<>(usignature);
        dataReader.readShort(usignatureRef);
        if (signature != usignatureRef.get()) {
            onSignatureError();
            return false;
        }

        // Определяем тип сообщения
        byte type = 0;
        AtomicReference<Byte> typeRef = new AtomicReference<>(type);
        dataReader.readByte(typeRef);
        type = typeRef.get();
        int headerSize = 5;
        short packetID = 0;
        AtomicReference<Short> packetIDRef = new AtomicReference<>(packetID);
        dataReader.readShort(packetIDRef);

        // Проверяем есть ли обработчик для сообщения такого типа
        if (type < inMessageHandlers.length && inMessageHandlers[type] != null) {
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
                return inMessageHandlers[type].
                    call(packetID, dataBufferRef.get(), headerSize, dataBuffer.length);
            }
            catch (Exception exc) {
                String s = String.format("Internal error %s", exc.getMessage());
                onReceivingError(s);
                return false;
            }
        }
        else {
            onReceivingError("Internal error: unknown command");
            return false;
        }
    }
    // </editor-fold>

    /**
     * Сигнатура сообщения
     */
    protected short signature;

    private final long sessionId;

    // <editor-fold defaultstate="collapsed" desc="Создание сессии">
    public TcpSession(IConnectManager connectManager, IConnectionInfo connectInfo, long sessionId) {
        this.connectManager = connectManager;
        this.connectInfo = connectInfo;
        this.sessionId = sessionId;
    }
    
    public long getSessionID() {
        return sessionId;
    }
    // </editor-fold>

    // <editor-fold defaultstate="collapsed" desc="Закрытие сессии">  
    protected final void closeHandlers() {
        if (!isClosed()) {
            closeSession();
        }
    }

    @Override
    protected void finalize() throws Throwable {
        closeHandlers();
        super.finalize();
    }
    // </editor-fold>

    protected abstract class MessageHandler {
        protected abstract boolean call(short messageID, byte[] data, int dataOffset, int dataSize);
    }

    private MessageHandler[] inMessageHandlers;

    protected void onSignatureError() {
        // По-умолчанию закрываем сессию            
        closeSession();
    }

    protected void onConnectionError(String errorMsg) {
        System.err.println(errorMsg);
    }

    protected void onReceivingError(String errorMsg) {
        System.err.println(errorMsg);
    }

    protected final void setInMessageHandlers(MessageHandler[] aMessageHandlers) {
        inMessageHandlers = aMessageHandlers;
        if (inMessageHandlers == null) {
            inMessageHandlers = new MessageHandler[0];
        }
    }

    // <editor-fold defaultstate="collapsed" desc="Отправка сообщения">
    protected void sendData(boolean back, byte type, short messageID, byte[] data, short size) {
        if (isClosed()) {
            return;
        }
        DataBuffer dataBuffer = new DataBuffer();
        dataBuffer.add(signature);
        dataBuffer.add(type);
        dataBuffer.add(messageID);
        dataBuffer.addArray(data, 0, size);
        sendData(back, dataBuffer.getBuffer(), (short)dataBuffer.getBufferSize());
    }
    // </editor-fold> 
}
