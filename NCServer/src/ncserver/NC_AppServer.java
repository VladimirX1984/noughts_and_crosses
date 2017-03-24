/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package ncserver;

import java.sql.ResultSet;
import java.sql.SQLException;
import ncserver.db.HibernateUtil;
import ncserver.game.*;

/**
 *
 * @author Vladimir
 */
public class NC_AppServer extends BaseAppGame {

    /**
     * @param args the command line arguments
     */
    public static void main(String[] args) {
        // TODO code application logic here
        try {
            NC_AppServer ncServer = new NC_AppServer();
            ncServer.run();
        }
        catch (Exception ex) {
            System.err.println(ex);
        }
    }

    public void run() {
        if (HibernateUtil.connectToDb(3306, "nc_game")) {
            ResultSet res = HibernateUtil.executeAndGetResult("SHOW TABLES LIKE 'nc_game_results'");
            if (res != null) {
                try {
                    if (!res.first()) {
                        String sQuery = "CREATE TABLE nc_game_results (id INT NOT NULL, token VARCHAR(255), creator_name VARCHAR(255), user_names VARCHAR(255), winName VARCHAR(255), duration VARCHAR(8));";
                        if (!HibernateUtil.executeUpdate(sQuery)) {
                            System.err.println("Таблица не создана");
                        }
                    }
                }
                catch (SQLException ex) {
                    System.err.println(ex);
                }
            }
            HibernateUtil.closeConnection();
        }
        System.out.println("run");

        javax.swing.SwingUtilities.invokeLater(new Runnable() {
            public void run() {
                createAndShowGUI();
            }
        });
    }

    public void start(int port) {
        System.out.println("start");
        connector.startServer(port, "Server");
    }

    public void stop() {
        connector.stop();
    }

    public String getProtocol() {
        return connector.getProtocol();
    }

    public void setProtocol(String protocol) {
        connector.setProtocol(protocol);
    }

    @Override
    public void onEvent(int event, Object data) {
        sendEvent(event, data);
    }

    public NC_AppServer() {
        context = new GameContext(this);
        gameSessionMan = new NC_GameSessionManager(context);
        context.setGameSessionManager(gameSessionMan);
        context.setSessionFactory(new NC_Connector(context));
        connector = context.getSessionFactory();
    }

    private final GameContext context;
    private final NC_Connector connector;
    private final GameSessionManager gameSessionMan;

    /**
     * Create the GUI and show it. For thread safety, this method should be
     * invoked from the event-dispatching thread.
     */
    private void createAndShowGUI() {
        NC_Frame frame = new NC_Frame(this);
        addListener(frame);
        frame.pack();
        frame.setVisible(true);
    }
}
