/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package ncserver.connection;

import java.io.IOException;
import java.net.InetSocketAddress;
import java.nio.ByteBuffer;
import java.nio.channels.SelectionKey;
import java.nio.channels.Selector;
import java.nio.channels.ServerSocketChannel;
import java.nio.channels.SocketChannel;
import java.util.IdentityHashMap;
import java.util.Iterator;
import java.util.Set;
import ncserver.game.GameMessageId;
import ncserver.utils.MessageBox;
import ncserver.utils.DataBuffer;
import ncserver.game.IGameSessionManager;

/**
 *
 * @author Vladimir
 */
public class TcpServer extends Server implements IServer, Runnable {

    // <editor-fold defaultstate="collapsed" desc="Реализация абстрактного класса Server">
    @Override
    public final boolean isRunning() {
        return bRunning;
    }

    @Override
    public final void start() {
        if (bRunning) {
            return;
        }
        try {
            bRunning = true;
            serverSocket = ServerSocketChannel.open();
            String host = "localhost";
            InetSocketAddress sAddr = new InetSocketAddress(host, portNumber);
            serverSocket.socket().bind(sAddr);
            serverSocket.configureBlocking(false);
            selector = Selector.open();
            serverSocket.register(selector, SelectionKey.OP_ACCEPT);
            System.out.format("TCP Server is listening at %s%n", sAddr);
        }
        catch (Exception ex) {
            System.err.println(ex);
            bRunning = false;
            System.err.
                println("Сервер не запущен. Скорее всего данный порт уже занят другим сервером");
            MessageBox.
                showError("Сервер не запущен. Скорее всего данный порт уже занят другим сервером: "
                    + ex.getMessage(), "Ошибка");
        }
    }

    @Override
    public final void stop() {
        if (!bRunning) {
            return;
        }
        bRunning = false;

        try {
            super.stop();
            gameSessionMan.removeAllGameSessions();
            sendEvent(GameMessageId.SERVER_STOPPED, null);
            if (serverSocket != null) {
                serverSocket.socket().close();
                serverSocket.close();
                serverSocket = null;
            }
            if (selector != null) {
                selector.close();
            }
        }
        catch (IOException ex) {
            System.err.println(ex);
        }
        finally {

        }
    }
    // </editor-fold>

    // <editor-fold defaultstate="collapsed" desc="Реализация абстрактного класса ConnectHandler">
    @Override
    protected final void onCloseConnection(IConnectionInfo connectInfo, boolean bClientClose) {
        if (bClientClose) {
            TcpConnectionInfo tcpConnectInfo = (TcpConnectionInfo)connectInfo;
            if (tcpConnectInfo.getSocketChannel() != null) {
                SocketChannel socketChannel = tcpConnectInfo.getSocketChannel();
                if (socketChannel != null && socketChannel.isConnected()) {
                    mapConnections.remove(socketChannel);
                    try {
                        socketChannel.close();
                    }
                    catch (IOException ex) {
                        System.err.println(ex);
                        MessageBox.showError(ex.getMessage(), "Ошибка закрытия соединения");
                    }
                }
            }
        }
        if (connections.isEmpty()) {
            gameSessionMan.removeGameSession(TcpServer.GAME_ID);
        }
    }

    @Override
    protected final void sendTo(IConnectionInfo connectInfo, byte[] data, boolean back) {
        TcpConnectionInfo connectInfo_ = (TcpConnectionInfo)connectInfo;
        SocketChannel socketChannel = connectInfo_.getSocketChannel();
        ByteBuffer buf = ByteBuffer.wrap(data);
        try {
            socketChannel.write(buf);
        }
        catch (IOException ex) {
            System.err.println(ex);
            MessageBox.showError(ex.getMessage(), "Ошибка отправка данных");
        }
        buf.rewind();
    }
    // </editor-fold>    

    // <editor-fold defaultstate="collapsed" desc="Реализация интерфейса Runnable">
    @Override
    public void run() {
        try {
            System.out.println("TCP Server starting on port " + this.portNumber);
            sendEvent(GameMessageId.SERVER_STARTED, this);

            Iterator<SelectionKey> iter;
            SelectionKey key;
            while (bRunning && serverSocket != null && serverSocket.isOpen()) {
                selector.select();
                if (selector == null || !bRunning) {
                    break;
                }
                Set<SelectionKey> set = selector.selectedKeys();
                if (set == null) {
                    break;
                }
                iter = set.iterator();
                while (iter.hasNext()) {
                    key = iter.next();
                    iter.remove();
                    if (key.isAcceptable()) {
                        handleAccept(key);
                    }
                    if (key.isReadable()) {
                        handleRead(key);
                    }
                }
            }
            sendEvent(0, this);
        }
        catch (IOException ex) {
            System.err.println(ex);
        }
        catch (NullPointerException ex) {
            System.err.println(ex);
        }
    }
    // </editor-fold>

    // <editor-fold defaultstate="collapsed" desc="Инициализация">
    public static final String GAME_ID = "abs123";

    private volatile boolean bRunning = false;

    private ServerSocketChannel serverSocket;
    private Selector selector;

    public TcpServer(IGameSessionManager gameSessionMan, int portNumber, SessionFactory sessionManager) {
        this(gameSessionMan, "Сервер: " + portNumber, portNumber, sessionManager, 0);
    }

    public TcpServer(IGameSessionManager gameSessionMan, String name, int portNumber,
                     SessionFactory sessionManager) {
        this(gameSessionMan, name, portNumber, sessionManager, 0);
    }

    public TcpServer(IGameSessionManager gameSessionMan, String name, int portNumber,
                     SessionFactory sessionManager, int maxConnectionNumber) {
        super(gameSessionMan, sessionManager, name, portNumber, maxConnectionNumber);
        start();
    }
    // </editor-fold>

    protected final IdentityHashMap<SocketChannel, TcpConnectionInfo> mapConnections = new IdentityHashMap<SocketChannel, TcpConnectionInfo>();
    private final ByteBuffer buf = ByteBuffer.allocate(1024);

    private void handleAccept(SelectionKey key) throws IOException {
        SocketChannel sc = ((ServerSocketChannel)key.channel()).accept();
        StringBuilder sb = new StringBuilder(sc.socket().getInetAddress().
            toString());
        String address = sb.append(":").append(sc.socket().getPort()).toString();
        sc.configureBlocking(false);
        sc.register(selector, SelectionKey.OP_READ, address);
        accept(sc);
        System.out.println("accepted connection from: " + address);
    }

    protected void onSocketClosed(final TcpConnectionInfo connInfo) {

    }

    private boolean handleRead(SelectionKey key) throws IOException {
        SocketChannel ch = (SocketChannel)key.channel();
        DataBuffer data = new DataBuffer();

        buf.clear();
        int read = 0;
        while ((read = ch.read(buf)) > 0) {
            buf.flip();
            byte[] bytes = new byte[buf.limit()];
            buf.get(bytes);
            data.add(bytes);
            buf.clear();
        }
        String msg;
        if (!mapConnections.containsKey(ch)) {
            msg = key.attachment() + " left the chat (unknown session).\n";
            System.err.println(msg);
            ch.close();
        }
        else if (read < 0) {
            final TcpConnectionInfo connInfo = mapConnections.get(ch);
            msg = key.attachment() + " left the chat";//" session = " + connInfo.getSession().getSessionID() + " left the chat";
            onSocketClosed(connInfo);
            closeConnection(connInfo, msg);
            System.out.println(msg);
        }
        else {
            onReceive(data.getBuffer(), mapConnections.get(ch).getSession());
        }
        return true;
    }

    private TcpConnectionInfo accept(SocketChannel sc) {
        TcpConnectionInfo connect = new TcpConnectionInfo(sc);
        ISession session = sessionFactory.сreateSession(this, connect);
        connect.setSession(session);
        synchronized (connections) {
            connections.add(connect);
        }
        mapConnections.put(sc, connect);
        return connect;
    }

    @Override
    protected void finalize() throws Throwable {
        stop();
        super.finalize();
    }

}
