/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package ncserver.connection;

/**
 *
 * @author Vladimir
 */
public abstract class AbstractSession implements ISession {

    // <editor-fold defaultstate="collapsed" desc="Реализация интерфейса ISession">    
    @Override
    public final IConnectionInfo getConnectionInfo() {
        return connectInfo;
    }

    // <editor-fold defaultstate="collapsed" desc="Прием сообщений">
    @Override
    public abstract boolean receiveData(Object data, int dataSize);
    // </editor-fold>

    // <editor-fold defaultstate="collapsed" desc="Отправка сообщения">
    @Override
    public void sendData(boolean back, byte[] data, short size) {
        if (connectManager == null) {
            return;
        }
        if (connectInfo == null) {
            return;
        }
        if (isClosed()) {
            return;
        }
        connectManager.send(connectInfo, back, data, size);
    }
    // </editor-fold>

    @Override
    public final void closeSession() {
        onSessionClosing();
        if (connectManager != null && connectInfo != null) {
            connectManager.closeConnection(connectInfo, "closeSession");
        }
        closed = true;
        connectManager = null;
        connectInfo = null;
    }

    @Override
    public boolean isClosed() {
        return closed;
    }
    // </editor-fold>

    protected IConnectionInfo connectInfo;

    protected IConnectManager connectManager;

    protected void onSessionClosing() {

    }

    private boolean closed = false;
}
