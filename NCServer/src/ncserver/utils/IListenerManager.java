/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package ncserver.utils;

/**
 *
 * @author Vladimir
 */
public interface IListenerManager extends IListener, ISender {

    void addListener(IListener listener);

    void removeListener(IListener listener);

    void removeAllListener();
}
