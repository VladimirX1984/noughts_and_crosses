/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package ncserver.utils;

import java.security.SecureRandom;
import java.util.Random;

/**
 *
 * @author Vladimir
 */
public final class RandomString {

    private static final char[] symbols;

    static {
        StringBuilder tmp = new StringBuilder();
        for (char ch = '0'; ch <= '9'; ++ch) {
            tmp.append(ch);
        }
        for (char ch = 'a'; ch <= 'z'; ++ch) {
            tmp.append(ch);
        }
        symbols = tmp.toString().toCharArray();
    }

    private Random random = null;

    private char[] buf;

    public RandomString(int length) {
        if (length < 1) {
            throw new IllegalArgumentException("length < 1: " + length);
        }
        try {
            random = new SecureRandom();
        }
        catch (Exception ex) {
            System.err.println(ex);
            random = new Random();
        }
        buf = new char[length];
    }

    public String nextString() {
        random.
            setSeed(random.nextLong() ^ System.currentTimeMillis() ^ hashCode()
                ^ Runtime.getRuntime().freeMemory());
        for (int idx = 0; idx < buf.length; ++idx) {
            buf[idx] = symbols[random.nextInt(symbols.length)];
        }
        return new String(buf);
    }
}
