/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package ncserver;

import java.util.HashSet;
import java.util.Set;

/**
 *
 * @author Vladimir
 */
public class NC_GameResult {
    
    private Long id;
    private String token;
    private String gameCreatorName;
    private Set userNames = new HashSet<String>();
    
    public NC_GameResult() {
        
    }
    
    public Long getId() {
        return id;
    }
    
    public void setId(Long id) {
        this.id = id;
    }
    
    public String getToken() {
        return token;
    }
    
    public void setToken(String token) {
        this.token = token;
    }
    
    public String getGameCreatorName() {
        return gameCreatorName;
    }
    
    public void setGameCreatorName(String gameCreatorName) {
        this.gameCreatorName = gameCreatorName;
    }
    
    public Set getUserNames(Set gameCreatorName) {
        return userNames;
    }
    
    public void setUserNames(Set userNames) {
        this.userNames = userNames;
    }
}
