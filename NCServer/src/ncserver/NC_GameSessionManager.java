/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package ncserver;

import ncserver.game.GameContext;
import ncserver.game.GameSessionManager;
import ncserver.game.IGameSession;

/**
 *
 * @author Vladimir
 */
public class NC_GameSessionManager extends GameSessionManager {

    @Override
    protected IGameSession createSession(GameContext context, String accessToken, String gameToken) {
        return new NC_GameSession(context, accessToken, gameToken);
    }

    public NC_GameSessionManager(GameContext context) {
        super(context);
    }
    
}