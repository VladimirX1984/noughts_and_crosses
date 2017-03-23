/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package ncserver.game;

import ncserver.utils.ListenerManager;

/**
 *
 * @author Vladimir
 */
public abstract class GameSession extends ListenerManager implements IGameSession {

    // <editor-fold defaultstate="collapsed" desc="Реализация интерфейса IGameSession">
    @Override
    public void init(Object data) {
        bInit = true;
    }

    @Override
    public void reset() {
        boolean bInit_ = bInit;
        bInit = false;
        if (bInit_) {
            sendEvent(GameMessageId.GAME_SESSION_ENDED, new GameMessage(0, this));
        }
    }

    @Override
    public boolean isInited() {
        return bInit;
    }

    @Override
    public String getAccessToken() {
        return accessToken;
    }

    @Override
    public String getGameToken() {
        return gameToken;
    }

    @Override
    public abstract long getDuration();

    @Override
    public boolean isGameEnded() {
        return bGameEnded;
    }

    @Override
    public final void finish() {
        bGameEnded = true;
    }
    // </editor-fold>

    // <editor-fold defaultstate="collapsed" desc="Инициализация">
    /**
     * контекст игры
     */
    protected final GameContext context;

    private boolean bInit = false;

    private boolean bGameEnded = false;

    private final String accessToken;
    private final String gameToken;

    public GameSession(GameContext context, String accessToken, String gameToken) {
        this.context = context;
        this.accessToken = accessToken;
        this.gameToken = gameToken;
    }
    // </editor-fold>
}
