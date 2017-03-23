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
public interface IConnectionInfo extends IBaseObject {

    public ISession getSession();

    public void setSession(ISession session);

    public boolean getClosing();

    public void close();
}
