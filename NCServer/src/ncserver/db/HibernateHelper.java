/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package ncserver.db;

import org.hibernate.Session;
import org.hibernate.SessionFactory;
import org.hibernate.cfg.Configuration;

/**
 *
 * @author Vladimir
 */
public class HibernateHelper {
    private HibernateHelper() {
    }

    public static Session getSession() {
        Session session = (Session)HibernateHelper.SESSION.get();
        if (session == null) {
            session = SESSION_FACTORY.openSession();
            HibernateHelper.SESSION.set(session);
        }
        return session;
    }

    private static final ThreadLocal SESSION = new ThreadLocal();
    private static final ThreadLocal TRANSACTION = new ThreadLocal();
    private static final SessionFactory SESSION_FACTORY = new Configuration().configure().
        buildSessionFactory();
}
