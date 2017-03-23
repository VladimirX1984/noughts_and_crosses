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
public class HttpConnectionInfo implements IConnectionInfo {

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

    public void close() {
        isClosing = true;
    }
    // </editor-fold>

    private ISession session;

    private boolean isClosing;
}
