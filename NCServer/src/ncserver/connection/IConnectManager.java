/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package ncserver.connection;

import ncserver.utils.IBaseObject;

/**
 *
 * @author Vladimir
 */
public interface IConnectManager extends IBaseObject {

    boolean closeConnection(IConnectionInfo connect, String msg);

    boolean closeSession(TcpSession session);

    void send(IConnectionInfo connect, boolean back, byte[] data, int size);
}
