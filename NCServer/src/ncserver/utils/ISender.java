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
public interface ISender extends IBaseObject {

    void sendEvent(int event, Object data);
}
