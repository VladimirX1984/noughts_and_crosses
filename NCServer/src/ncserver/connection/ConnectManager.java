/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package ncserver.connection;

import ncserver.utils.ListenerManager;

/**
 *
 * @author Vladimir
 */
public abstract class ConnectManager extends ListenerManager implements IConnectManager {

    @Override
    public abstract boolean closeConnection(IConnectionInfo connect, String msg);

    @Override
    public abstract boolean closeSession(TcpSession session);

    // <editor-fold defaultstate="collapsed" desc="Отправка данных">
    @Override
    public void send(IConnectionInfo connectInfo, boolean back, byte[] data, int size) {
        if (size == 0) {
            return;
        }
        if (connectInfo == null) {
            return;
        }

        sendTo(connectInfo, back, data, size);
    }

    protected abstract void sendTo(IConnectionInfo connectInfo, boolean back, byte[] data, int size);
    // </editor-fold>

    // <editor-fold defaultstate="collapsed" desc="Прием данных">
    protected final boolean onReceive(byte[] bytes, ISession session) {
        if (bytes == null) {
            return false;
        }
        if (session == null) {
            return false;
        }
        int readBytes = bytes.length;
        return session.receiveData(bytes, readBytes);
    }
    // </editor-fold>

}
