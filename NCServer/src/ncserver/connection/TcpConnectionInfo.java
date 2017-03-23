/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package ncserver.connection;

import java.nio.channels.SocketChannel;

/**
 *
 * @author Vladimir
 */
public class TcpConnectionInfo implements IConnectionInfo {

    // <editor-fold defaultstate="collapsed" desc="Реализация интерфейса IConnectionInfo">
    @Override
    public ISession getSession() {
        return session;
    }

    @Override
    public void setSession(ISession asession) {
        session = asession;
    }

    @Override
    public boolean getClosing() {
        return isClosing;
    }

    @Override
    public void close() {
        isClosing = true;
    }
    // </editor-fold>

    public TcpConnectionInfo(SocketChannel asocketChannel) {
        socketChannel = asocketChannel;
    }

    public SocketChannel getSocketChannel() {
        return socketChannel;
    }

    private final SocketChannel socketChannel;

    private ISession session;

    private boolean isClosing;
}
