/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package ncserver.db;

import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.ResultSet;
import java.sql.ResultSetMetaData;
import java.sql.SQLException;
import java.sql.Statement;

import org.hibernate.HibernateException;
import org.hibernate.Session;
import org.hibernate.SessionFactory;
import org.hibernate.cfg.Configuration;

/**
 *
 * @author Vladimir
 */
public class HibernateUtil {
    private static SessionFactory sessionFactory;

    static {
        try {
            sessionFactory = new Configuration().configure("ncserver/db/nc_game.cfg.xml").
                buildSessionFactory();
            //sessionFactory = new Configuration().configure("ncserver/db/nc_game.cfg.xml").
            //buildSessionFactory(new StandardServiceRegistryBuilder().build());
        }
        catch (Throwable ex) {
            System.err.println("Initial SessionFactory creation failed." + ex);
            throw new ExceptionInInitializerError(ex);
        }
    }

    public static SessionFactory getSessionFactory() {
        return sessionFactory;
    }

    public static final ThreadLocal SESSION = new ThreadLocal();

    public static Session currentSession() throws HibernateException {
        Session s = (Session)SESSION.get();
        // Open a new Session, if this thread has none yet
        if (s == null) {
            s = sessionFactory.openSession();
            // Store it in the ThreadLocal variable
            SESSION.set(s);
        }
        return s;
    }

    public static void closeSession() throws HibernateException {
        Session s = (Session)SESSION.get();
        if (s != null) {
            s.close();
        }
        SESSION.set(null);
    }

    static Connection conn;
    static Statement st;

    public static boolean connectToDb(int portNumber, String dbName) {
        try {
            // Step 1: Load the JDBC driver.
            Class.forName("com.mysql.jdbc.Driver");
            System.out.println("Driver Loaded.");
            // Step 2: Establish the connection to the database.
            String url = "jdbc:mysql://localhost:" + portNumber + "/" + dbName;
            conn = DriverManager.getConnection(url, "root", "");
            System.out.println("Got Connection.");
            return true;
        }
        catch (ClassNotFoundException | SQLException e) {
            System.err.println("Got an exception! " + e);
            return false;
        }
    }

    public static ResultSet executeAndGetResult(String sql) {
        try {
            if (conn == null) {
                return null;
            }
            st = conn.createStatement();
            st.execute(sql);
            ResultSet res = st.getResultSet();
            return res;
        }
        catch (SQLException e) {
            System.err.println("Got an exception! " + e);
            return null;
        }
    }

    public static boolean executeUpdate(String sql) {
        try {
            if (conn == null) {
                return false;
            }
            st = conn.createStatement();
            st.executeUpdate(sql);
            return true;
        }
        catch (Exception e) {
            System.err.println("Got an exception! " + e);
            return false;
        }
    }

    public static void closeConnection() {
        try {
            if (conn != null) {
                conn.close();
                conn = null;
            }
        }
        catch (SQLException e) {
            System.err.println("Got an exception! " + e);
        }
    }

    public static void checkData(String sql) {
        try {
            HibernateUtil.outputResultSet(st.executeQuery(sql));
//			conn.close();
        }
        catch (Exception e) {
            System.err.println(e);
        }
    }

    public static void outputResultSet(ResultSet rs) throws Exception {
        ResultSetMetaData metadata = rs.getMetaData();

        int numcols = metadata.getColumnCount();
        String[] labels = new String[numcols];
        int[] colwidths = new int[numcols];
        int[] colpos = new int[numcols];
        int linewidth;

        linewidth = 1;
        for (int i = 0; i < numcols; i++) {
            colpos[i] = linewidth;
            labels[i] = metadata.getColumnLabel(i + 1); // get its label
            int size = metadata.getColumnDisplaySize(i + 1);
            if (size > 30 || size == -1) {
                size = 30;
            }
            int labelsize = labels[i].length();
            if (labelsize > size) {
                size = labelsize;
            }
            colwidths[i] = size + 1; // save the column the size
            linewidth += colwidths[i] + 2; // increment total size
        }

        StringBuffer divider = new StringBuffer(linewidth);
        StringBuffer blankline = new StringBuffer(linewidth);
        for (int i = 0; i < linewidth; i++) {
            divider.insert(i, '-');
            blankline.insert(i, " ");
        }
        // Put special marks in the divider line at the column positions
        for (int i = 0; i < numcols; i++) {
            divider.setCharAt(colpos[i] - 1, '+');
        }
        divider.setCharAt(linewidth - 1, '+');

        // Begin the table output with a divider line
        System.out.println(divider);

        // The next line of the table contains the column labels.
        // Begin with a blank line, and put the column names and column
        // divider characters "|" into it. overwrite() is defined below.
        StringBuffer line = new StringBuffer(blankline.toString());
        line.setCharAt(0, '|');
        for (int i = 0; i < numcols; i++) {
            int pos = colpos[i] + 1 + (colwidths[i] - labels[i].length()) / 2;
            overwrite(line, pos, labels[i]);
            overwrite(line, colpos[i] + colwidths[i], " |");
        }
        System.out.println(line);
        System.out.println(divider);

        while (rs.next()) {
            line = new StringBuffer(blankline.toString());
            line.setCharAt(0, '|');
            for (int i = 0; i < numcols; i++) {
                Object value = rs.getObject(i + 1);
                overwrite(line, colpos[i] + 1, value.toString().trim());
                overwrite(line, colpos[i] + colwidths[i], " |");
            }
            System.out.println(line);
        }
        System.out.println(divider);

    }

    static void overwrite(StringBuffer b, int pos, String s) {
        int len = s.length();
        for (int i = 0; i < len; i++) {
            b.setCharAt(pos + i, s.charAt(i));
        }
    }

}
