/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package ncserver.db;

import java.util.Collection;
import ncserver.NC_GameResult;

/**
 *
 * @author Vladimir
 */
public interface NC_GameResultDb {
    
    public void addGameResult(NC_GameResult gameResult);

    public void updateGameResult(Long gameResult_id, NC_GameResult gameResult);

    public NC_GameResult getGameResultById(Long gameResult_id);

    public Collection getAllGameResults();

    public void deleteGameResult(NC_GameResult gameResult);
}
