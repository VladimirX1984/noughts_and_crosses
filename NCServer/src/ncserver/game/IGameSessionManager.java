/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package ncserver.game;

import ncserver.utils.IBaseObject;

/**
 *
 * @author Vladimir
 */
public interface IGameSessionManager extends IBaseObject {

    <TGameSession extends IGameSession> TGameSession createGameSession(String accessToken,
                                                                       String gameToken);

    <TGameSession extends IGameSession> TGameSession createGameSession(String accessToken,
                                                                       String gameToken, Object data);

    boolean removeGameSession(String accessToken);

    boolean removeGameSession(IGameSession gameSession);

    void removeAllGameSessions();

    <TGameSession extends IGameSession> TGameSession getGameSession(String id);

    <TGameSession extends IGameSession> TGameSession getGameSessionByToken(String id);

    boolean isInited(String id);
}
