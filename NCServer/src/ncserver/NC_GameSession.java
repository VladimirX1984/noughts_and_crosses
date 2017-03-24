/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package ncserver;

import java.util.logging.Level;
import java.util.logging.Logger;
import ncserver.game.GameContext;
import ncserver.game.GameSession;
import ncserver.game.GameStateChecker;

/**
 *
 * @author Vladimir
 */
public class NC_GameSession extends GameSession implements Runnable {

    // <editor-fold defaultstate="collapsed" desc="Реализация интерфейса Runnable">
    @Override
    public void run() {
        startTime = System.currentTimeMillis();
        startGameTime = System.currentTimeMillis();
        while (isInited()) {
            try {
                thread.sleep(100);
                long time = System.currentTimeMillis();
                if ((time - startTime) >= 1000 * 60 * 5) {
                    // активность игры пропала - выходим
                    reset();
                }
            }
            catch (InterruptedException ex) {
                Logger.getLogger(GameSession.class.getName()).log(Level.SEVERE, null, ex);
            }
        }
    }
    // </editor-fold>

    @Override
    public void init(Object data) {
        super.init(data);
        gameState.init((Integer)data);
        startTime = System.currentTimeMillis();
        startGameTime = System.currentTimeMillis();
        thread = new Thread(this);
        thread.start();
    }

    @Override
    public long getDuration() {
        if (isGameEnded()) {
            return duration;
        }
        if (isPlayerJoined()) {
            return System.currentTimeMillis() - startGameTime;
        }
        return 0;
    }

    // <editor-fold defaultstate="collapsed" desc="Инициализация">
    /**
     * состояние игры
     */
    private final NC_GameState gameState;

    /**
     * определение состояние игры
     */
    private final GameStateChecker gameStateChecker;

    /**
     * мой ход, если значение true
     */
    private boolean bMyMove;

    /**
     * Число отмеченных клеток в ряду или диагонале для достижения победы
     */
    private int numberToWin;

    private boolean bMyFirstMove;

    private int winner = GameStateChecker.NONE;

    private int playerNumber = 1;
    private String userNameX;
    private String userName0;

    private Thread thread;
    private volatile long startTime;
    private volatile long startGameTime;
    private long duration;

    public final String CELL_X = "X";
    public final String CELL_0 = "0";

    public NC_GameSession(GameContext context, String accessToken, String gameToken) {
        super(context, accessToken, gameToken);
        bMyFirstMove = true;
        numberToWin = 3;
        bMyMove = false;
        gameState = new NC_GameState();
        gameStateChecker = new GameStateChecker();
    }
    // </editor-fold>

    // <editor-fold defaultstate="collapsed" desc="Методы работы с пользователями игры">
    public String getUserNameX() {
        return userNameX;
    }

    public void setUserNameX(String userName) {
        userNameX = userName;
    }

    public String getUserName0() {
        return userName0;
    }

    public void setUserName0(String userName) {
        userName0 = userName;
    }

    public boolean isPlayerJoined() {
        return playerNumber > 1;
    }

    public void addPlayer() {
        ++playerNumber;
        if (playerNumber == 2) {
            startGameTime = System.currentTimeMillis();
        }
    }

    public void removePlayer() {
        --playerNumber;
        if (playerNumber <= 0) {
            reset();
        }
    }

    public int getPlayerNumber() {
        return playerNumber;
    }
    // </editor-fold>

    // <editor-fold defaultstate="collapsed" desc="Методы работы с состоянием (процесса) игры">
    public boolean getMyMove() {
        return bMyMove;
    }

    public void setMyMove(boolean b) {
        bMyMove = b;
    }

    public int getNumberToWin() {
        return numberToWin;
    }

    public void setNumberToWin(int val) {
        numberToWin = val;
    }

    public NC_GameState getGameState() {
        return gameState;
    }

    public void setGameState(String data) {
        gameState.setData(data);
    }

    public boolean makeMove(int number, char cellValue) {
        startTime = System.currentTimeMillis();
        gameState.setCellValue(number, cellValue);
        bMyMove = !bMyMove;
        if (!isGameEnded()) {
            finishGameIfNeeded();
        }
        return true;
    }

    public boolean makeMove(int row, int coll, char cellValue) {
        int number = row * gameState.getSize() + coll;
        return makeMove(number, cellValue);
    }

    public void setMyFirstMove(boolean abMyFirstMove) {
        bMyFirstMove = abMyFirstMove;
    }

    public boolean isMyFirstMove() {
        return bMyFirstMove;
    }

    public boolean isMyMove() {
        return gameStateChecker.isAnyCellFilled(gameState) ? bMyMove : isMyFirstMove();
    }

    public int getWinner() {
        return winner;
    }

    public String getWinnerUser() {
        switch (winner) {
            case GameStateChecker.NONE:
            case GameStateChecker.MATCH_DRAWN: {
                return "";
            }
            case GameStateChecker.WIN_X: {
                return isMyFirstMove() ? getUserNameX() : getUserName0();
            }
            case GameStateChecker.WIN_0: {
                return isMyFirstMove() ? getUserName0() : getUserNameX();
            }
        }
        return "";
    }

    public void surrender(boolean gameCreator) {
        if (!isGameEnded()) {
            duration = getDuration();
            finish();
            winner = gameCreator ? GameStateChecker.WIN_0 : GameStateChecker.WIN_X;
            if (!isMyFirstMove()) {
                winner = winner == GameStateChecker.WIN_X ? GameStateChecker.WIN_0 : GameStateChecker.WIN_X;
            }
        }
    }

    private void finishGameIfNeeded() {
        winner = gameStateChecker.getWinner(gameState, numberToWin);
        boolean bGameEnded = winner != GameStateChecker.NONE;
        if (bGameEnded) {
            duration = getDuration();
            finish();
        }
    }
    // </editor-fold>
}
