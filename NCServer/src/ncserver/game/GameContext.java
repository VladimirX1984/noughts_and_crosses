/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package ncserver.game;

import ncserver.connection.SessionFactory;

/**
 *
 * @author Vladimir
 */
public class GameContext {

    public <TAppGame extends IAppGame> TAppGame GetGame() {
        return (TAppGame)game;
    }

    public <TGameSessionManager extends IGameSessionManager> TGameSessionManager getGameSessionManager() {
        return (TGameSessionManager)gameSessionMan;
    }

    public void setGameSessionManager(IGameSessionManager agameSessionMan) {
        gameSessionMan = agameSessionMan;
    }

    public <TSessionFactory extends SessionFactory> TSessionFactory getSessionFactory() {
        return (TSessionFactory)connector;
    }

    public void setSessionFactory(SessionFactory aconnector) {
        connector = aconnector;
    }

    public GameContext(IAppGame agame) {
        game = agame;
    }

    private final IAppGame game;
    private IGameSessionManager gameSessionMan;
    private SessionFactory connector;
}
