/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package ncserver.game;

import java.util.HashMap;
import ncserver.utils.IListener;

/**
 *
 * @author Vladimir
 */
public abstract class GameSessionManager implements IGameSessionManager, IListener {

    // <editor-fold defaultstate="collapsed" desc="Реализация интерфейса IGameSessionManager">
    @Override
    public final void initGameSession(String accessToken, String gameToken) {
        IGameSession gameSession = gameSessionByAccessToken.get(accessToken);
        if (gameSession == null) {
            gameSession = createSession(context, accessToken, gameToken);
            gameSessionByAccessToken.put(accessToken, gameSession);
            gameSession = gameSessionByAccessToken.get(accessToken);
            gameSession.addListener(this);
        }
        if (!gameSessionByGameToken.containsKey(gameToken)) {
            gameSessionByGameToken.put(gameToken, (GameSession)gameSession);
        }
    }

    @Override
    public final void initGameSession(String accessToken, String gameToken, Object data) {
        initGameSession(accessToken, gameToken);
        IGameSession gameSession = gameSessionByAccessToken.get(accessToken);
        gameSession.init(data);
    }

    @Override
    public final boolean removeGameSession(String accessToken) {
        IGameSession gameSession = gameSessionByAccessToken.get(accessToken);
        if (gameSession == null) {
            return false;
        }
        gameSession.reset();
        gameSessionByAccessToken.remove(accessToken);
        gameSessionByGameToken.remove(gameSession.getGameToken());
        return true;
    }

    @Override
    public final boolean removeGameSession(IGameSession gameSession) {
        return removeGameSession(gameSession.getAccessToken());
    }

    @Override
    public final void removeAllGameSessions() {
        gameSessionByAccessToken.clear();
        gameSessionByGameToken.clear();
    }

    @Override
    public final <TGameSession extends IGameSession> TGameSession getGameSession(String id) {
        return (TGameSession)gameSessionByAccessToken.get(id);
    }

    @Override
    public final <TGameSession extends IGameSession> TGameSession getGameSessionByToken(String id) {
        return (TGameSession)gameSessionByGameToken.get(id);
    }

    @Override
    public final boolean isInited(String id) {
        IGameSession gameSession = getGameSession(id);
        if (gameSession == null) {
            return false;
        }
        return gameSession.isInited();
    }
    // </editor-fold>

    // <editor-fold defaultstate="collapsed" desc="Реализация интерфейса IListener">
    @Override
    public final void onEvent(int event, Object data) {
        if (event == GameMessageId.GAME_SESSION_ENDED) {
            IGameSession gameSession = (IGameSession)((GameMessage)data).getData();
            removeGameSession(gameSession);
        }
        if (context != null) {
            context.GetGame().sendEvent(event, data);
        }
    }
    // </editor-fold>

    /**
     * контекст игры
     */
    private final GameContext context;

    private final HashMap<String, IGameSession> gameSessionByAccessToken;
    private final HashMap<String, IGameSession> gameSessionByGameToken;

    public GameSessionManager(GameContext acontext) {
        context = acontext;
        gameSessionByAccessToken = new HashMap<String, IGameSession>();
        gameSessionByGameToken = new HashMap<String, IGameSession>();
    }

    protected abstract IGameSession createSession(GameContext context, String accessToken,
                                                  String gameToken);
}
