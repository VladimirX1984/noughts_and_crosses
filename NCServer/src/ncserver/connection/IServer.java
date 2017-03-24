/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package ncserver.connection;

import ncserver.utils.IListenerManager;

/**
 *
 * @author Vladimir
 */
public interface IServer extends IConnectManager, IListenerManager {

    boolean isRunning();

    void start();

    void stop();
    
    String getName();

    int getMaxConnectionNumber();
}
