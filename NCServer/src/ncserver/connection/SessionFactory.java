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
public interface SessionFactory {

    ISession сreateSession(IConnectManager socketHandler, IConnectionInfo connection);

    void destroySession(ISession session);

    void destroyAllSession();
}
