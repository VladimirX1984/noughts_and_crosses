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
public interface ISession extends IBaseObject {

    IConnectionInfo getConnectionInfo();

    boolean receiveData(Object data, int size);

    void sendData(boolean back, byte[] data, short size);

    void closeSession();

    boolean isClosed();
}
