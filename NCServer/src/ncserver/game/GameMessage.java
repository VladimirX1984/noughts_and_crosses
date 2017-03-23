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

    private final int id;
    private final String msg;
    private final IBaseObject data;

    public int getId() {
        return id;
    }

    public String getMsg() {
        return msg;
    }

    public IBaseObject getData() {
        return data;
    }

    public GameMessage(int id, String msg) {
        this.id = id;
        this.msg = msg;
        this.data = null;
    }

    public GameMessage(int id, IBaseObject data) {
        this.id = id;
        this.msg = null;
        this.data = data;
    }
    
    public GameMessage(int id, IBaseObject data, String msg) {
        this.id = id;
        this.data = data;
        this.msg = msg;
    }
}
