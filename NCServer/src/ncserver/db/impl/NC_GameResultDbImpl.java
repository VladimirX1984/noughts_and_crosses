/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package ncserver.db.impl;

import java.util.ArrayList;
import java.util.Collection;
import java.util.List;
import ncserver.NC_GameResult;
import ncserver.db.NC_GameResultDb;
import ncserver.db.HibernateUtil;
import ncserver.utils.MessageBox;
import org.hibernate.query.Query;
import org.hibernate.Session;

/**
 *
 * @author Vladimir
 */
public class NC_GameResultDbImpl implements NC_GameResultDb {

    // <editor-fold defaultstate="collapsed" desc="Реализация интерфейса NC_GameResultDb">
    @Override
    public void addGameResult(NC_GameResult gameResult) {
        Session session = null;
        try {
            session = HibernateUtil.getSessionFactory().openSession();
            session.beginTransaction();
            session.save(gameResult);
            session.getTransaction().commit();
        }
        catch (Exception e) {
            MessageBox.showError(e.getMessage(), "Ошибка при вставке");
        }
        finally {
            if (session != null && session.isOpen()) {
                session.close();
            }
        }
    }

    @Override
    public void updateGameResult(Long gameResult_id, NC_GameResult gameResult) {
        Session session = null;
        try {
            session = HibernateUtil.getSessionFactory().openSession();
            session.beginTransaction();
            session.update(gameResult);
            session.getTransaction().commit();
        }
        catch (Exception e) {
            MessageBox.showError(e.getMessage(), "Ошибка при вставке");
        }
        finally {
            if (session != null && session.isOpen()) {
                session.close();
            }
        }
    }

    @Override
    public NC_GameResult getGameResultById(Long gameResult_id) {
        Session session = null;
        NC_GameResult gameResult = null;
        try {
            session = HibernateUtil.getSessionFactory().openSession();
            gameResult = (NC_GameResult)session.load(NC_GameResult.class, gameResult_id);
        }
        catch (Exception e) {
            MessageBox.showError(e.getMessage(), "Ошибка 'findById'");
        }
        finally {
            if (session != null && session.isOpen()) {
                session.close();
            }
        }
        return gameResult;
    }

    @Override
    public Collection getAllGameResults() {
        Session session = null;
        List gameResults = new ArrayList<NC_GameResult>();
        try {
            session = HibernateUtil.getSessionFactory().openSession();
            //gameResults = session.createCriteria(NC_GameResult.class).list();
            session.beginTransaction();
            Query query = session.
                createQuery("select nc_game_results.* from NC_GameResult nc_game_results");
            //query.addEntity("nc_game_results", NC_GameResult.class);
//            query.getResultList()
            gameResults = query.list();
            session.getTransaction().commit();
        }
        catch (Exception e) {
            MessageBox.showError(e.getMessage(), "Ошибка 'getAll'");
        }
        finally {
            if (session != null && session.isOpen()) {
                session.close();
            }
        }
        return gameResults;
    }

    @Override
    public void deleteGameResult(NC_GameResult gameResult) {
        Session session = null;
        try {
            session = HibernateUtil.getSessionFactory().openSession();
            session.beginTransaction();
            session.delete(gameResult);
            session.getTransaction().commit();
        }
        catch (Exception e) {
            MessageBox.showError(e.getMessage(), "Ошибка при удалении");
        }
        finally {
            if (session != null && session.isOpen()) {
                session.close();
            }
        }
    }
    // </editor-fold>
    
}
