/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package ncserver.utils;

import java.security.SecureRandom;
import java.util.HashMap;
import java.util.Random;

/**
 *
 * @author Vladimir
 */
public final class SessionIdentifierGenerator {

    private Random random = new SecureRandom();
    private boolean weakRandom = false;
    private final RandomString rndString = new RandomString(37);
    private final HashMap<String, Boolean> _sessions = new HashMap<String, Boolean>();

    public String nextSessionId() {
        synchronized (this) {
            String newId = rndString.nextString();

            String id = null;
            while (id == null || id.length() == 0 || idInUse(id)) {
                long r0 = weakRandom ? (hashCode()
                    ^ Runtime.getRuntime().freeMemory() ^ random.nextInt()
                    ^ (((long)newId.hashCode()) << 32)) : random.nextLong();
                if (r0 < 0) {
                    r0 = -r0;
                }
                long r1 = weakRandom ? (hashCode()
                    ^ Runtime.getRuntime().freeMemory() ^ random.nextInt()
                    ^ (((long)newId.hashCode()) << 32)) : random.nextLong();
                if (r1 < 0) {
                    r1 = -r1;
                }
                id = Long.toString(r0, 36) + Long.toString(r1, 36);
            }
            _sessions.put(id, weakRandom);
            return id;
        }
    }

    public SessionIdentifierGenerator() {
    }

    public void initRandom() {
        if (random == null) {
            try {
                random = new SecureRandom();
            }
            catch (Exception ex) {
                System.err.println(ex);
                random = new Random();
                weakRandom = true;
            }
        }
        random.
            setSeed(random.nextLong() ^ System.currentTimeMillis() ^ hashCode()
                ^ Runtime.getRuntime().freeMemory());
    }

    private boolean idInUse(String id) {
        return _sessions.containsKey(id);
    }
}
