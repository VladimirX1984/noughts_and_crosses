/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package ncserver;

import java.util.ArrayList;
import java.util.List;
import ncserver.connection.*;
import ncserver.game.GameContext;
import ncserver.utils.MessageBox;

/**
 *
 * @author Vladimir
 */
public class NC_Connector implements SessionFactory {

    // <editor-fold defaultstate="collapsed" desc="Реализация интерфейса SessionFactory">
    @Override
    public ISession сreateSession(IConnectManager connectHandler, IConnectionInfo connection) {
        if (connectHandler == server) {
            ISession session = null;
            if (protocol.equals(protocols[0])) {
                session = new NC_TcpSession((IServer)connectHandler, connection, context, clientSessions.
                    size() + 1);
            }
            else if (protocol.equals(protocols[1])) {
                session = new NC_HttpSession((IServer)connectHandler, connection, context);
            }
            else {
                return null;
            }
            synchronized (clientSessions) {
                clientSessions.add(session);
                if (protocol.equals(protocols[0])) {
                    ((NC_TcpSession)session).setObserver(clientSessions.size() > server.
                        getMaxConnectionNumber());
                }
            }
            return session;
        }

        return null;
    }

    @Override
    public void destroySession(ISession session) {
        if (session != null) {
            session.closeSession();
            synchronized (clientSessions) {
                clientSessions.remove(session);
            }
        }
    }

    @Override
    public void destroyAllSession() {
        synchronized (clientSessions) {
            while (clientSessions.size() > 0) {
                ISession session = clientSessions.get(0);
                session.closeSession();
            }
            clientSessions.clear();
        }
    }
    // </editor-fold>

    // <editor-fold defaultstate="collapsed" desc="Инициализация">
    private IServer server;

    private final GameContext context;

    private String protocol;

    private static final String[] protocols = new String[]{"TCP", "HTTP"};

    public NC_Connector(GameContext aContext) {
        context = aContext;
        protocol = protocols[0];
    }
    // </editor-fold>

    public boolean startServer(int port, String serverName) {
        if (protocol == null) {
            MessageBox.showError("Не определен протокол", "Ошибка");
            return false;
        }
        if (protocol.equals(protocols[0])) {
            server = new NC_TcpServer(context.getGameSessionManager(), serverName, port, this, 2);
            if (!server.isRunning()) {
                MessageBox.showError("При запуске сервера произошла ошибка", "Ошибка");
                return false;
            }
            server.addListener(context.GetGame());
            clientSessions = new ArrayList<>();
            (new Thread((TcpServer)server)).start();
            return true;
        }
        if (protocol.equals(protocols[1])) {
            server = new HTTPServer(context.getGameSessionManager(), serverName, port, this, 2);
            server.addListener(context.GetGame());
            clientSessions = new ArrayList<>();
            server.start();
            if (!server.isRunning()) {
                MessageBox.showError("При запуске сервера произошла ошибка", "Ошибка");
                return false;
            }
            return true;
        }

        MessageBox.showError("Не определен протокол", "Ошибка");
        return false;
    }

    public void stop() {
        if (server != null) {
            server.stop();
            server = null;
        }
    }

    public String getProtocol() {
        return protocol;
    }

    public void setProtocol(String aprotocol) {
        protocol = aprotocol;
    }

    // <editor-fold defaultstate="collapsed" desc="Клиентские сессии">
    private List<ISession> clientSessions;

    public GameContext getContext() {
        return context;
    }

    // </editor-fold>
}
