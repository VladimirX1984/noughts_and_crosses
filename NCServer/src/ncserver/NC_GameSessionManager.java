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

    public boolean getMyMove(String id) {
        NC_GameSession gameSession = getGameSession(id);
        if (gameSession == null) {
            return false;
        }
        return gameSession.getMyMove();
    }

    public void setMyMove(String id, boolean b) {
        NC_GameSession gameSession = getGameSession(id);
        if (gameSession == null) {
            return;
        }
        gameSession.setMyMove(b);
    }

    public int getNumberToWin(String id) {
        NC_GameSession gameSession = getGameSession(id);
        if (gameSession == null) {
            return -1;
        }
        return gameSession.getNumberToWin();
    }

    public void setNumberToWin(String id, int val) {
        NC_GameSession gameSession = getGameSession(id);
        if (gameSession == null) {
            return;
        }
        gameSession.setNumberToWin(val);
    }

    public void setGameState(String id, String data) {
        NC_GameSession gameSession = getGameSession(id);
        if (gameSession == null) {
            return;
        }
        gameSession.setGameState(data);
    }

    /**
     * сделан ход
     * @param id
     * @param number
     * @param cellValue
     * @return
     */
    public boolean makeMove(String id, int number, char cellValue) {
        NC_GameSession gameSession = getGameSession(id);
        if (gameSession == null) {
            return false;
        }
        return gameSession.makeMove(number, cellValue);
    }

    public boolean makeMove(String id, int row, int coll, char cellValue) {
        NC_GameSession gameSession = getGameSession(id);
        if (gameSession == null) {
            return false;
        }
        return gameSession.makeMove(row, coll, cellValue);
    }

    public void setYourMove(String id, boolean abMyFirstMove, boolean abYourMove) {
        NC_GameSession gameSession = getGameSession(id);
        if (gameSession == null) {
            return;
        }
        gameSession.setYourMove(abMyFirstMove, abYourMove);
    }

    public void setMyFirstMove(String id, boolean abMyFirstMove) {
        NC_GameSession gameSession = getGameSession(id);
        if (gameSession == null) {
            return;
        }
        gameSession.setMyFirstMove(abMyFirstMove);
    }

    public boolean isMyFirstMove(String id) {
        NC_GameSession gameSession = getGameSession(id);
        if (gameSession == null) {
            return false;
        }
        return gameSession.isMyFirstMove();
    }

    public boolean isMyMove(String id) {
        NC_GameSession gameSession = getGameSession(id);
        if (gameSession == null) {
            return false;
        }
        return gameSession.isMyMove();
    }

    public NC_GameState getGameState(String id) {
        NC_GameSession gameSession = getGameSession(id);
        if (gameSession == null) {
            return null;
        }
        return gameSession.getGameState();
    }

    public int finishGameIfNeeded(String id) {
        NC_GameSession gameSession = getGameSession(id);
        if (gameSession == null) {
            return -1;
        }
        return gameSession.finishGameIfNeeded();
    }
}
