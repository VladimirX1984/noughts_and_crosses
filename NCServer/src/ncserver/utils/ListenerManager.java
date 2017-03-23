/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package ncserver.utils;

import java.util.ArrayList;
import java.util.List;

/**
 *
 * @author Vladimir
 */
public abstract class ListenerManager implements IListenerManager {

    // <editor-fold defaultstate="collapsed" desc="Реализация интерфейса IListener">
    @Override
    public void onEvent(int event, Object data) {

    }
    // </editor-fold>

    // <editor-fold defaultstate="collapsed" desc="Реализация интерфейса IListenerManager">    
    @Override
    public final void addListener(IListener listener) {
        listeners.add(listener);
    }

    @Override
    public final void removeListener(IListener listener) {
        listeners.remove(listener);
    }

    @Override
    public final void removeAllListener() {
        listeners.clear();
    }
    // </editor-fold>

    // <editor-fold defaultstate="collapsed" desc="Реализация интерфейса ISender"> 
    @Override
    public final void sendEvent(int event, Object data) {
        listeners.forEach((listener) -> {
            listener.onEvent(event, data);
        });
    }
    // </editor-fold>

    private final List<IListener> listeners = new ArrayList<>();
}
