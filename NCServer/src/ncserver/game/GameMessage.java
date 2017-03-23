/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package ncserver.game;

import ncserver.utils.IBaseObject;

/**
 *
 * @author Vladimir
 */
public class GameMessage {

    private int id;
    private String msg;
    private IBaseObject data;

    public int getId() {
        return id;
    }

    public String getMsg() {
        return msg;
    }

    public IBaseObject getData() {
        return data;
    }

    public GameMessage(int aId, String amsg) {
        id = aId;
        msg = amsg;
    }

    public GameMessage(int aId, IBaseObject adata) {
        id = aId;
        data = adata;
    }
    
    public GameMessage(int aId, IBaseObject adata, String amsg) {
        id = aId;
        data = adata;
        msg = amsg;
    }
}
