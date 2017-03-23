/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package ncserver.connection;

import com.sun.net.httpserver.Headers;

/**
 *
 * @author Vladimir
 */
public interface IHttpSession extends ISession {

    boolean receiveData(String command, Headers headers, Object data, int size);
}
