/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package ncserver;

import ncserver.game.GameMessage;
import java.util.concurrent.atomic.AtomicReference;
import ncserver.connection.*;
import ncserver.game.*;
import ncserver.utils.*;

/**
 *
 * @author Vladimir
 */
public final class NC_TcpSession extends TcpSession {

    // <editor-fold defaultstate="collapsed" desc="Инициализация">
    private class MessageGameInfoHandler extends MessageHandler {
        @Override
        protected boolean call(short messageID, byte[] data, int dataOffset, int dataSize) {
            return NC_TcpSession.this.onGameInfo(messageID, data, dataOffset, dataSize);
        }
    }

    private class MessageGameMoveHandler extends MessageHandler {
        @Override
        protected boolean call(short messageID, byte[] data, int dataOffset, int dataSize) {
            return NC_TcpSession.this.onMoveInfo(messageID, data, dataOffset, dataSize);
        }
    }

    private class MessageSurrenderHandler extends MessageHandler {
        @Override
        protected boolean call(short messageID, byte[] data, int dataOffset, int dataSize) {
            return NC_TcpSession.this.onSurrenderInfo(messageID, data, dataOffset, dataSize);
        }
    }

    private GameContext context;

    /**
     * идентификатор пакета
     */
    private short messageID;

    private final int inMessageCount = 4;

    private String userName = "";

    private boolean isObserver;

    private String gameSessionId;

    public String getGameSessionId() {
        return gameSessionId;
    }

    public boolean getObserver() {
        return isObserver;
    }

    public void setObserver(boolean bObserver) {
        isObserver = bObserver;
    }

    public String getUserName() {
        return userName;
    }

    public NC_TcpSession(IConnectManager connectHandler, IConnectionInfo connection,
                         GameContext aContext, long sessionID) {
        super(connectHandler, connection, sessionID);
        context = aContext;
        signature = 20100;
        messageID = 0;

        MessageHandler[] inHandlers = new MessageHandler[inMessageCount];
        inHandlers[inGameInfo] = new MessageGameInfoHandler();
        inHandlers[inMoveInfo] = new MessageGameMoveHandler();
        inHandlers[inSurrender] = new MessageSurrenderHandler();
        setInMessageHandlers(inHandlers);
    }

    
    @Override
    protected void onSessionClosing() {
        context.GetGame().sendEvent(GameMessageId.CLIENT_DISCONNECTED, userName);
    }
    // </editor-fold>

    // <editor-fold defaultstate="collapsed" desc="Прием сообщений">    
    private final int inGameInfo = 1;
    private final int inMoveInfo = 2;
    private final int inSurrender = 3;
    private final byte inQuery = 1;

    private boolean onGameInfo(short messageID, byte[] data, int offset, int dataSize) {

        System.out.println("create or join to game: " + BitConverter.
            toString(data));

        DataReader dataReader = new DataReader(data, 0, dataSize);
        byte type = 0;
        AtomicReference<Byte> typeRef = new AtomicReference<>(type);
        if (!dataReader.readByte(typeRef)) {
            onReceivingError("Server onGameInfo dataReader.ReadByte(typeRef)");
            return false;
        }

        if (typeRef.get() != inQuery) {
            onReceivingError("Server onGameInfo aType != inQuery");
            return false;
        }

        AtomicReference<String> userNameRef = new AtomicReference<>(userName);
        if (!dataReader.readASCII(userNameRef)) {
            onReceivingError("Server onGameInfo dataReader.readUnicode(userNameRef)");
            return false;
        }
        userName = userNameRef.get();

        byte mode = 0;
        AtomicReference<Byte> modeRef = new AtomicReference<>(mode);
        if (!dataReader.readByte(modeRef)) {
            onReceivingError("Server onGameInfo dataReader.readByte(modeRef)");
            return false;
        }

        mode = modeRef.get();
        if (mode == 1) {
            // создание игры
        }
        else if (mode == 0) {
            // присоединение к игре
        }

        boolean bMyFirstMove = false;
        AtomicReference<Boolean> bMyFirstMoveRef = new AtomicReference<>(bMyFirstMove);
        if (!dataReader.readBoolean(bMyFirstMoveRef)) {
            onReceivingError("Server onGameInfo dataReader.readBoolean(bMyFirstMoveRef)");
            return false;
        }
        bMyFirstMove = bMyFirstMoveRef.get();

        short rowCellCount = 0;
        AtomicReference<Short> rowCellCountRef = new AtomicReference<>(rowCellCount);
        if (!dataReader.readShort(rowCellCountRef)) {
            onReceivingError("Server onGameInfo dataReader.readShort(rowCellCountRef)");
            return false;
        }
        rowCellCount = rowCellCountRef.get();

        short numberToWin = 0;
        AtomicReference<Short> numberToWinRef = new AtomicReference<>(numberToWin);
        if (!dataReader.readShort(numberToWinRef)) {
            onReceivingError("Server onGameInfo dataReader.readShort(numberToWinRef)");
            return false;
        }
        numberToWin = numberToWinRef.get();

        gameSessionId = TcpServer.GAME_ID;
        boolean bGameInit = context.getGameSessionManager().isInited(gameSessionId);

        if (!bGameInit) {
            context.getGameSessionManager().initGameSession(gameSessionId, gameSessionId,
                new Short(rowCellCount).intValue());
            NC_GameSession gameSession = context.getGameSessionManager().getGameSession(gameSessionId);
            gameSession.setUserNameX(userName);
            gameSession.setNumberToWin(numberToWin);
            gameSession.setMyFirstMove(bMyFirstMove);
            gameSession.setYourMove(bMyFirstMove, bMyFirstMove);

            context.GetGame().sendEvent(GameMessageId.CLIENT_CONNECTED, new GameMessage(0, gameSession));

            DataBuffer dataBuffer = new DataBuffer();
            dataBuffer.add(outConfirmation);
            dataBuffer.add((byte)-2);
            sendData(true, outGameInfo, messageID, dataBuffer.getBuffer(),
                (short)dataBuffer.getBufferSize());
        }
        else if (bGameInit) {
            NC_GameSession gameSession = context.getGameSessionManager().getGameSession(gameSessionId);
            gameSession.setUserName0(userName);

            context.GetGame().sendEvent(GameMessageId.CLIENT_CONNECTED, new GameMessage(1, gameSession));

            bMyFirstMove = gameSession.isMyFirstMove();
            boolean bMyMove = gameSession.isMyMove();

            if (!getObserver()) {
                DataBuffer dataBuffer = new DataBuffer();
                dataBuffer.add(outConfirmation);
                dataBuffer.add((byte)0);
                dataBuffer.add(bMyFirstMove);
                dataBuffer.add(bMyMove);
                sendData(false, outGameInfo, messageID, dataBuffer.getBuffer(),
                    (short)dataBuffer.getBufferSize());
            }

            DataBuffer backDataBuffer = new DataBuffer();
            backDataBuffer.add(outConfirmation);
            if (getObserver()) {
                backDataBuffer.add((byte)2);
            }
            else {
                backDataBuffer.add((byte)1);
            }
            backDataBuffer.add(!bMyFirstMove);
            backDataBuffer.add(!bMyMove);
            backDataBuffer.add((short)gameSession.getGameState().size());
            backDataBuffer.add((short)gameSession.getNumberToWin());
            backDataBuffer.addASCII(gameSession.getGameState().getString());
            sendData(true, outGameInfo, messageID, backDataBuffer.getBuffer(),
                (short)backDataBuffer.getBufferSize());
        }
        return true;
    }

    /**
     * Прием данных хода
     *
     * @param aMessageID
     * @param data
     * @param offset
     * @param size
     * @return
     */
    private boolean onMoveInfo(short aMessageID, byte[] data, int offset, int size) {
        if (aMessageID != messageID) {
            onReceivingError("OnMoveInfo aMessageID != messageID");
            return false;
        }

        DataReader dataReader = new DataReader(data, 0, size);
        byte type = 0;
        AtomicReference<Byte> typeRef = new AtomicReference<>(type);
        if (!dataReader.readByte(typeRef)) {
            onReceivingError("OnMoveInfo dataReader.ReadByte(typeRef)");
            return false;
        }

        if (typeRef.get() != inQuery) {
            onReceivingError("OnMoveInfo aType != inQuery");
            return false;
        }

        short cellNumber = 0;
        AtomicReference<Short> cellNumberRef = new AtomicReference<>(cellNumber);
        if (!dataReader.readShort(cellNumberRef)) {
            onReceivingError("dataReader.Read(ref aCellNumber)");
            return false;
        }
        cellNumber = cellNumberRef.get();

        char cellValue = '?';
        AtomicReference<Character> cellValueRef = new AtomicReference<>(cellValue);
        if (!dataReader.readChar(cellValueRef)) {
            onReceivingError("dataReader.ReadChar(ref aCellValue)");
            return false;
        }
        cellValue = cellValueRef.get();

        NC_GameSession gameSession = context.getGameSessionManager().getGameSession(TcpServer.GAME_ID);
        gameSession.makeMove(cellNumber, cellValue);

        int winner = gameSession.finishGameIfNeeded();
        if (winner > GameStateChecker.NONE) {
            sendInfo(outQuery, cellNumber, cellValue);
            sendEndGame((byte)winner);
            context.GetGame().
                sendEvent(GameMessageId.WIN_CLIENT, winner == GameStateChecker.MATCH_DRAWN ? null : userName);
        }
        else {
            sendInfo(outQuery, cellNumber, cellValue);
        }
        return true;
    }

    private boolean onSurrenderInfo(short aMessageID, byte[] data, int offset, int size) {
        if (aMessageID != messageID) {
            onReceivingError("onSurrenderInfo aMessageID != messageID");
            return false;
        }
        DataReader dataReader = new DataReader(data, 0, size);
        byte type = 0;
        AtomicReference<Byte> typeRef = new AtomicReference<>(type);
        if (!dataReader.readByte(typeRef)) {
            onReceivingError("onSurrenderInfo dataReader.ReadByte(typeRef)");
            return false;
        }
        if (typeRef.get() != inQuery) {
            onReceivingError("onSurrenderInfo aType != inQuery");
            return false;
        }
        return true;
    }

    @Override
    protected void onReceivingError(String errorMsg) {
        System.err.println(errorMsg);
        if (connectManager != null) {
            connectManager.closeConnection(connectInfo, "ReceivingError");
            connectManager = null;
        }
    }
    // </editor-fold>

    // <editor-fold defaultstate="collapsed" desc="Отправка сообщений">
    private final byte outGameInfo = 1;
    private final byte outMoveInfo = 2;
    private final byte outEndGame = 3;
    private final byte outInterruptGame = 4;

    private final byte outQuery = 1;          // запрос и команда
    private final byte outConfirmation = 2;   // подтверждение
    
    public void sendInterruptGame(byte id, String winUserName) {
        DataBuffer backDataBuffer = new DataBuffer();
        backDataBuffer.add(outConfirmation);
        backDataBuffer.add(id);
        backDataBuffer.addASCII(winUserName);
        sendData(true, outInterruptGame, messageID, backDataBuffer.getBuffer(),
            (short)backDataBuffer.getBufferSize());
    }

    private void sendInfo(byte type, short cellNumber, char cellValue) {
        DataBuffer dataBuffer = new DataBuffer();
        dataBuffer.add(type);
        dataBuffer.add(cellNumber);
        dataBuffer.add(cellValue);
        sendData(false, outMoveInfo, messageID, dataBuffer.getBuffer(),
            (short)dataBuffer.getBufferSize());
    }

    private void sendEndGame(byte winner) {
        DataBuffer dataBuffer = new DataBuffer();
        dataBuffer.add(outConfirmation);
        if (winner != GameStateChecker.MATCH_DRAWN) {
            dataBuffer.add((byte)1);
            dataBuffer.addASCII(userName);
        }
        else {
            dataBuffer.add((byte)0);
        }
        sendData(true, outEndGame, messageID, dataBuffer.getBuffer(),
            (short)dataBuffer.getBufferSize());

        dataBuffer = new DataBuffer();
        dataBuffer.add(outConfirmation);
        if (winner != GameStateChecker.MATCH_DRAWN) {
            dataBuffer.add((byte)-1);
            dataBuffer.addASCII(userName);
        }
        else {
            dataBuffer.add((byte)0);
        }
        sendData(false, outEndGame, messageID, dataBuffer.getBuffer(),
            (short)dataBuffer.getBufferSize());
    }
    // </editor-fold>

}
