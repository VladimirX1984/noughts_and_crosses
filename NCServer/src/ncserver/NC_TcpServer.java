/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package ncserver;

import java.util.Collection;
import ncserver.connection.SessionFactory;
import ncserver.connection.TcpConnectionInfo;
import ncserver.connection.TcpServer;
import ncserver.game.GameSessionManager;
import ncserver.game.IGameSessionManager;

/**
 *
 * @author Vladimir
 */
public class NC_TcpServer extends TcpServer {

    public NC_TcpServer(IGameSessionManager agameCtrl, int port, SessionFactory sessionManager) {
        this(agameCtrl, "Сервер: " + port, port, sessionManager, 0);
    }

    public NC_TcpServer(IGameSessionManager agameCtrl, String serverName, int port,
                        SessionFactory sessionManager) {
        this(agameCtrl, serverName, port, sessionManager, 0);
    }

    public NC_TcpServer(IGameSessionManager gameCtrl, String serverName, int port,
                        SessionFactory sessionManager, int maxConnectionNumber) {
        super(gameCtrl, serverName, port, sessionManager, maxConnectionNumber);
    }

    @Override
    protected final void onSocketClosed(final TcpConnectionInfo connInfo) {
        NC_TcpSession session = (NC_TcpSession)connInfo.getSession();
        if (!session.getObserver()) {
            NC_GameSession gameSession = ((GameSessionManager)gameCtrl).getGameSession(session.
                getGameSessionId());
            if (gameSession.isGameEnded()) {
                return;
            }
            Collection<TcpConnectionInfo> collect = mapConnections.values();
            TcpConnectionInfo connectInfoOther = null;
            for (TcpConnectionInfo conn : collect) {
                if (conn != connInfo && !((NC_TcpSession)conn.getSession()).
                    getObserver()) {
                    connectInfoOther = conn;
                    break;
                }
            }
            if (connectInfoOther != null) {
                final TcpConnectionInfo _connectInfoOther = connectInfoOther;
                NC_TcpSession sessionOther = (NC_TcpSession)connectInfoOther.getSession();
                String userName = sessionOther.getUserName();
                sessionOther.sendInterruptGame((byte)1, userName);
                ((NC_TcpSession)connInfo.getSession()).sendInterruptGame((byte)-1, userName);
                if (collect.size() > 2) {
                    collect.forEach((conn) -> {
                        if (conn != _connectInfoOther && conn != connInfo) {
                            ((NC_TcpSession)conn.getSession()).
                                sendInterruptGame((byte)1, userName);
                        }
                    });
                }
            }
        }
    }
}
