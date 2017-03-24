/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package ncserver.connection;

import java.util.ArrayList;
import ncserver.game.IGameSessionManager;

/**
 *
 * @author Vladimir
 */
public abstract class Server extends ConnectManager implements IServer {

    // <editor-fold defaultstate="collapsed" desc="Реализация интерфейса IServer">
    @Override
    public abstract boolean isRunning();

    @Override
    public abstract void start();

    @Override
    public void stop() {
        synchronized (connections) {
            while (!connections.isEmpty()) {
                closeConnection(connections.get(0), "Остановка сервера");
            }
        }
    }

    @Override
    public String getName() {
        return name;
    }

    @Override
    public int getMaxConnectionNumber() {
        return maxConnectionNumber;
    }
    // </editor-fold>

    protected final IGameSessionManager gameSessionMan;
    protected final SessionFactory sessionFactory;
    protected final int portNumber;

    private final String name;
    private final int maxConnectionNumber;

    public Server(IGameSessionManager gameSessionMan, SessionFactory sessionFactory, String name,
                  int portNumber, int maxConnectionNumber) {
        this.gameSessionMan = gameSessionMan;
        this.sessionFactory = sessionFactory;
        this.name = name;
        this.portNumber = portNumber;
        this.maxConnectionNumber = maxConnectionNumber;
        this.connections = new ArrayList<IConnectionInfo>();
    }

    protected final ArrayList<IConnectionInfo> connections;

    @Override
    protected final void sendTo(IConnectionInfo connectInfo, boolean back, byte[] data, int size) {
        if (back) {
            sendTo(connectInfo, data, true);
            return;
        }

        ArrayList<IConnectionInfo> __connections;
        synchronized (connections) {
            __connections = (ArrayList<IConnectionInfo>)connections.clone();
        }
        for (IConnectionInfo connInfo : __connections) {
            if (connInfo != connectInfo) {
                sendTo(connInfo, data, false);
            }
        }
    }

    protected abstract void sendTo(IConnectionInfo connectInfo, byte[] data, boolean back);

    @Override
    public final boolean closeConnection(IConnectionInfo connectInfo, String msg) {
        if (connectInfo == null) {
            return false;
        }
        if (connectInfo.getClosing()) {
            synchronized (connections) {
                connections.remove(connectInfo);
            }
            onCloseConnection(connectInfo, false);
            return true;
        }
        connectInfo.close();
        ISession session = connectInfo.getSession();
        if (sessionFactory != null && connectInfo.getSession() != null) {
            sessionFactory.destroySession(session);
        }
        synchronized (connections) {
            connections.remove(connectInfo);
        }
        onCloseConnection(connectInfo, true);
        return true;
    }

    @Override
    public final boolean closeSession(TcpSession session) {
        IConnectionInfo connectionInfo = null;
        synchronized (connections) {
            for (IConnectionInfo connInfo : connections) {
                if (connInfo.getSession() == session) {
                    connectionInfo = connInfo;
                    break;
                }
            }
        }
        if (connectionInfo != null) {
            return closeConnection(connectionInfo, "Close session");
        }
        return false;
    }

    protected abstract void onCloseConnection(IConnectionInfo connectInfo, boolean bClientClose);
}
