/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package ncserver.connection;

import com.sun.net.httpserver.*;
import com.sun.net.httpserver.Authenticator.*;
import java.io.IOException;
import java.io.OutputStream;
import java.net.InetSocketAddress;
import java.net.URI;
import ncserver.game.GameMessageId;
import ncserver.utils.DataBuffer;
import ncserver.utils.MessageBox;
import ncserver.game.IGameSessionManager;

/**
 *
 * @author Vladimir
 */
public class HTTPServer extends Server {

    // <editor-fold defaultstate="collapsed" desc="Инициализация">
    private String name;

    private int portNumber;

    private boolean bRunning = false;

    protected IGameSessionManager gameCtrl;

    private HttpServer server;

    public HTTPServer(IGameSessionManager agameCtrl, int port, SessionFactory sessionManager) {
        this(agameCtrl, "Сервер: " + port, port, sessionManager, 0);
    }

    public HTTPServer(IGameSessionManager agameCtrl, String serverName, int port,
                      SessionFactory sessionManager) {
        this(agameCtrl, serverName, port, sessionManager, 0);
    }

    public HTTPServer(IGameSessionManager agameCtrl, String serverName, int aPort,
                      SessionFactory aSessionManager, int maxConnectionNumber) {
        super(maxConnectionNumber);
        gameCtrl = agameCtrl;
        name = serverName;
        portNumber = aPort;
        sessionFactory = aSessionManager;
    }
    // </editor-fold>

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
            HttpConnectionInfo connect = new HttpConnectionInfo();
            ISession session = sessionFactory.сreateSession(this, connect);
            if (!IHttpSession.class.isInstance(session)) {
                System.err.
                    println("Сервер не запущен. Объект сессия доолжен реализовать интерфейс IHttpSession");
                MessageBox.
                    showError("Сервер не запущен. Объект сессия доолжен реализовать интерфейс IHttpSession ", "Ошибка");
                bRunning = false;
                return;
            }
            bRunning = true;
            String host = "localhost";
            InetSocketAddress sAddr = new InetSocketAddress(host, portNumber);

            server = HttpServer.create();
            server.bind(sAddr, 0);
            HttpContext context = server.createContext("/", new EchoHandler());
            context.setAuthenticator(new Auth());
            server.setExecutor(null);
            server.start();
            accept(session, connect);
            System.out.format("HTTP Server is listening at %s%n", sAddr);
            sendEvent(GameMessageId.SERVER_STARTED, this);
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
            gameCtrl.removeAllGameSessions();
            sendEvent(GameMessageId.SERVER_STOPPED, null);
            if (server != null) {
                server.stop(1);
            }
        }
        catch (Exception ex) {
            System.err.println(ex);
        }
    }
    // </editor-fold>

    // <editor-fold defaultstate="collapsed" desc="Реализация абстрактного класса ConnectHandler">
    @Override
    protected void onCloseConnection(IConnectionInfo connectInfo, boolean bClientClose) {
        if (connections.isEmpty()) {
            gameCtrl.removeAllGameSessions();
        }
    }

    @Override
    protected final void sendTo(IConnectionInfo connectInfo, byte[] data, boolean back) {
        if (!back) {
            return;
        }
        try {
            currExchange.sendResponseHeaders(200, data.length);
            try (OutputStream os = currExchange.getResponseBody()) {
                os.write(data);
            }
        }
        catch (IOException ex) {
            System.err.println(ex);
        }
    }
    // </editor-fold>

    private HttpExchange currExchange;

    private void accept(ISession session, HttpConnectionInfo connect) {
        connect.setSession(session);
        synchronized (connections) {
            connections.add(connect);
        }
    }

    // <editor-fold defaultstate="collapsed" desc="Прием данных">
    private void handleRead(HttpExchange exchange) {
        URI uri = exchange.getRequestURI();
        String command = uri.getPath();
        DataBuffer dataBuffer = new DataBuffer();
        if (!"GET".equals(exchange.getRequestMethod().toUpperCase())) {
            dataBuffer.addData(exchange.getRequestBody());
        }
        currExchange = exchange;
        onReceive(command, exchange.getRequestHeaders(), dataBuffer.getBuffer(),
            (IHttpSession)connections.get(0).getSession());
    }

    private void onReceive(String command, Headers headers, byte[] bytes, IHttpSession session) {
        if (bytes == null) {
            return;
        }
        if (session == null) {
            return;
        }
        int readBytes = bytes.length;
        session.receiveData(command, headers, bytes, readBytes);
    }

    class EchoHandler implements HttpHandler {

        @Override
        public void handle(HttpExchange exchange) throws IOException {
            HTTPServer.this.handleRead(exchange);
        }
    }
    // </editor-fold>

    class Auth extends Authenticator {

        @Override
        public Result authenticate(HttpExchange httpExchange) {
            if ("/forbidden".equals(httpExchange.getRequestURI().toString())) {
                return new Failure(403);
            }
            else {
                return new Success(new HttpPrincipal("c0nst", "realm"));
            }
        }
    }

}
