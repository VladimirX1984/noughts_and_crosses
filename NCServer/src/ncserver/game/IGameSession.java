/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package ncserver.game;

import ncserver.utils.IListenerManager;

/**
 *
 * @author Vladimir
 */
public interface IGameSession extends IListenerManager {

    void init(Object data);

    void reset();

    boolean isInited();

    String getAccessToken();

    String getGameToken();

    long getDuration();

    boolean isGameEnded();

    void finish();
}
