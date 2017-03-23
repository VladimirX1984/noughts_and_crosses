/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package ncserver;

import ncserver.game.GameMessage;
import com.sun.net.httpserver.Headers;
import ncserver.connection.HttpSessionBasedOnJson;
import ncserver.connection.IConnectionInfo;
import ncserver.connection.IServer;
import ncserver.game.GameContext;
import ncserver.game.GameMessageId;
import ncserver.game.GameStateChecker;
import ncserver.utils.*;
import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

/**
 *
 * @author Vladimir
 */
public final class NC_HttpSession extends HttpSessionBasedOnJson {

    // <editor-fold defaultstate="collapsed" desc="Инициализация">
    private final GameContext context;

    private final RandomString rnd = new RandomString(6);
    private final SessionIdentifierGenerator generator = new SessionIdentifierGenerator();

    private class MessageCreateGameHandler extends JsonMessageHandler {
        @Override
        protected boolean call(Headers headers, JSONObject jsonObject) {
            return NC_HttpSession.this.onCreateGame(headers, jsonObject);
        }
    }

    private class MessageJoinGameHandler extends JsonMessageHandler {
        @Override
        protected boolean call(Headers headers, JSONObject jsonObject) {
            return NC_HttpSession.this.onJoinGame(headers, jsonObject);
        }
    }

    private class MessageGameMoveHandler extends JsonMessageHandler {
        @Override
        protected boolean call(Headers headers, JSONObject jsonObject) {
            return NC_HttpSession.this.onMakeMove(headers, jsonObject);
        }
    }

    private class MessageGameStateHandler extends JsonMessageHandler {
        @Override
        protected boolean call(Headers headers, JSONObject jsonObject) {
            return NC_HttpSession.this.onGameState(headers, jsonObject);
        }
    }

    private class MessagePlayerExitHandler extends JsonMessageHandler {
        @Override
        protected boolean call(Headers headers, JSONObject jsonObject) {
            return NC_HttpSession.this.onPlayerExit(headers, jsonObject);
        }
    }

    private final String inCreateGame = "/new_game";
    private final String inJoinGame = "/join_game";
    private final String inMoveInfo = "/make_a_move";
    private final String inGameState = "/state";
    private final String inPlayerExit = "/exit";

    public NC_HttpSession(IServer server, IConnectionInfo connection, GameContext aContext) {
        super(server, connection);
        context = aContext;

        addMessageHandler(inCreateGame, new MessageCreateGameHandler());
        addMessageHandler(inJoinGame, new MessageJoinGameHandler());
        addMessageHandler(inMoveInfo, new MessageGameMoveHandler());
        addMessageHandler(inGameState, new MessageGameStateHandler());
        addMessageHandler(inPlayerExit, new MessagePlayerExitHandler());
    }
    // </editor-fold>

    // <editor-fold defaultstate="collapsed" desc="Прием сообщений">
    private boolean onCreateGame(Headers headers, JSONObject jsonObject) {
        try {
            if (!jsonObject.has("user_name")) {
                onReceivingError("User name is undefined");
                return false;
            }
            String userName = jsonObject.getString("user_name");

            boolean bMyFirstMove = true;
            if (jsonObject.has("my_turn")) {
                bMyFirstMove = (jsonObject.getInt("my_turn") == 1);
            }

            if (!jsonObject.has("size")) {
                onReceivingError("Field size is undefined");
                return false;
            }
            int rowCellCount = jsonObject.getInt("size");

            int numberToWin = 3;
            if (jsonObject.has("number_to_win")) {
                numberToWin = jsonObject.getInt("number_to_win");
            }

            String accessToken = generator.nextSessionId();
            String gameToken = rnd.nextString();

            context.getGameSessionManager().initGameSession(accessToken, gameToken, rowCellCount);
            NC_GameSession gameSession = context.getGameSessionManager().getGameSession(accessToken);
            gameSession.setUserNameX(userName);
            gameSession.setNumberToWin(numberToWin);
            gameSession.setYourMove(bMyFirstMove, bMyFirstMove);

            context.GetGame().sendEvent(GameMessageId.CLIENT_CONNECTED, new GameMessage(0, gameSession));

            JSONObject obj = new JSONObject();
            obj.put("status", "ok");
            obj.put("access_token", accessToken);
            obj.put("game_token", gameToken);
            sendData(obj);
            return true;
        }
        catch (JSONException ex) {
            onReceivingError(ex.getMessage());
            return false;
        }
    }

    private boolean onJoinGame(Headers headers, JSONObject jsonObject) {
        try {
            if (!jsonObject.has("user_name")) {
                onReceivingError("User name is undefined");
                return false;
            }
            String userName = jsonObject.getString("user_name");

            if (!jsonObject.has("game_token")) {
                onReceivingError("Game token is undefined");
                return false;
            }
            String gameToken = jsonObject.getString("game_token");

            NC_GameSessionManager gameMan = context.getGameSessionManager();
            NC_GameSession gameSession = gameMan.getGameSessionByToken(gameToken);
            if (gameSession == null) {
                onReceivingError("Game token is invalid");
                return false;
            }
            if (gameSession.isGameEnded()) {
                onReceivingError("This game session is ended");
                return false;
            }
            gameSession.addPlayer();
            int mode = gameSession.getPlayerNumber() <= 2 ? 1 : 2;
            if (mode == 1) {
                gameSession.setUserName0(userName);
                context.GetGame().
                    sendEvent(GameMessageId.CLIENT_CONNECTED, new GameMessage(1, gameSession));
            }
            else {
                context.GetGame().
                    sendEvent(GameMessageId.CLIENT_CONNECTED, new GameMessage(2, gameSession, userName));
            }

            JSONObject obj = new JSONObject();
            obj.put("status", "ok");
            obj.put("access_token", gameSession.getAccessToken());
            obj.put("mode", mode);
            obj.put("size", gameSession.getGameState().size());
            obj.put("number_to_win", gameSession.getNumberToWin());
            obj.put("your_first_turn", !gameSession.isMyFirstMove());
            sendData(obj);
            return true;
        }
        catch (JSONException ex) {
            onReceivingError(ex.getMessage());
            return false;
        }
    }

    /**
     * Прием данных хода
     *
     * @param data
     * @param offset
     * @param size
     * @return
     */
    private boolean onMakeMove(Headers headers, JSONObject jsonObject) {
        try {
            if (!headers.containsKey("access_token")) {
                onReceivingError("access token is undefined");
                return false;
            }
            String accessToken = headers.getFirst("access_token");
            NC_GameSession gameSession = context.getGameSessionManager().getGameSession(accessToken);
            if (gameSession == null) {
                sendGameSessionClosed(1, "Game session is finished");
                return false;
            }
            int row = jsonObject.getInt("row");
            int coll = jsonObject.getInt("coll");
            String cellValue = jsonObject.getString("cellValue");
            gameSession.makeMove(row, coll, cellValue.charAt(0));
            int winner = gameSession.finishGameIfNeeded();
            if (winner > GameStateChecker.NONE && winner != GameStateChecker.MATCH_DRAWN) {
                context.GetGame().sendEvent(GameMessageId.WIN_CLIENT, gameSession.getWinnerUser());
            }
            JSONObject obj = new JSONObject();
            obj.put("status", "ok");
            sendData(obj);
            return true;
        }
        catch (JSONException ex) {
            onReceivingError(ex.getMessage());
            return false;
        }
    }

    private boolean onGameState(Headers headers, JSONObject jsonObject) {
        try {
            if (!headers.containsKey("access_token")) {
                onReceivingError("access token is undefined");
                return false;
            }
            String accessToken = headers.getFirst("access_token");            
            if (!headers.containsKey("game_creator")) {
                onReceivingError("header is invalid");
            }
            String s = headers.getFirst("game_creator");
            boolean isGameCreator = Integer.parseInt(s) == 1;

            NC_GameSession gameSession = context.getGameSessionManager().getGameSession(accessToken);
            if (gameSession == null) {
                sendGameSessionClosed(1, "Game session is finished");
                return false;
            }
            boolean bMyMove = gameSession.isMyMove();
            int winner = gameSession.finishGameIfNeeded();
            JSONObject obj = new JSONObject();
            obj.put("status", "ok");
            boolean bYouTurn = gameSession.isPlayerJoined()
                && (isGameCreator && bMyMove || !isGameCreator && !bMyMove);
            obj.put("you_turn", bYouTurn);
            obj.put("game_duration", gameSession.getDuration());
            JSONArray field = new JSONArray(gameSession.getGameState().getStringArray());
            obj.put("field", field);
            if (winner > GameStateChecker.NONE) {
                String userName = gameSession.getWinnerUser();
                gameSession.removePlayer();
                obj.put("winner", winner);
                if (!"".equals(userName)) {
                    obj.put("winner_name", userName);
                }
            }
            sendData(obj);
            return true;
        }
        catch (JSONException ex) {
            onReceivingError(ex.getMessage());
            return false;
        }
        catch (NumberFormatException ex) {
            onReceivingError(ex.getMessage());
            return false;
        }
        catch (Exception ex) {
            onReceivingError(ex.getMessage());
            return false;
        }
    }

    private boolean onPlayerExit(Headers headers, JSONObject jsonObject) {
        try {
            if (!headers.containsKey("access_token")) {
                onReceivingError("access token is undefined");
                return false;
            }
            String accessToken = headers.getFirst("access_token");            
            if (!headers.containsKey("game_creator")) {
                onReceivingError("header is invalid");
                return false;
            }
            String sGameCreator = headers.getFirst("game_creator");
            boolean isGameCreator = Integer.parseInt(sGameCreator) == 1;            
            if (!headers.containsKey("observer")) {
                onReceivingError("header is invalid");
                return false;
            }
            String sObserver = headers.getFirst("observer");
            boolean isObserver = Integer.parseInt(sObserver) == 1;

            NC_GameSession gameSession = context.getGameSessionManager().getGameSession(accessToken);
            if (gameSession == null) {
                onReceivingError("Access token is invalid");
                return false;
            }
            if (gameSession.isGameEnded()) {
                JSONObject obj = new JSONObject();
                obj.put("status", "ok");
                sendData(obj);
                return true;
            }
            if (jsonObject.has("user_name")) {
                String userName = jsonObject.getString("user_name");
                context.GetGame().sendEvent(GameMessageId.CLIENT_DISCONNECTED, userName);
            }
            boolean exit = false;
            if (jsonObject.has("exit")) {
                exit = jsonObject.getBoolean("exit");
            }
            if (!isObserver) {
                gameSession.surrenderGame(isGameCreator);
                context.GetGame().sendEvent(GameMessageId.WIN_CLIENT, gameSession.getWinnerUser());
                if (exit) {
                    gameSession.removePlayer();
                }
            }
            else {
                gameSession.removePlayer();
            }
            JSONObject obj = new JSONObject();
            obj.put("status", "ok");
            sendData(obj);
            return true;
        }
        catch (NumberFormatException | JSONException ex) {
            onReceivingError(ex.getMessage());
            return false;
        }
        catch (Exception ex) {
            onReceivingError(ex.getMessage());
            return false;
        }
    }

    @Override
    protected void onReceivingError(String errorMsg) {
        try {
            JSONObject obj = new JSONObject();
            obj.put("status", "error");
            obj.put("message", errorMsg);
            sendData(obj);
        }
        catch (JSONException ex) {
            System.err.println(ex.getMessage());
        }
    }
    // </editor-fold>

    // <editor-fold defaultstate="collapsed" desc="Отправка сообщений">            

    private void sendData(JSONObject jsonObject) {
        DataBuffer dataBuffer = new DataBuffer(jsonObject.toString());
        sendData(true, dataBuffer.getBuffer(), (short)dataBuffer.getBufferSize());
    }

    private void sendGameSessionClosed(int id, String errorMsg) throws JSONException {
        JSONObject obj = new JSONObject();
        obj.put("status", "error");
        obj.put("msg_id", id);
        obj.put("message", errorMsg);
        sendData(obj);
    }
    // </editor-fold>

}
