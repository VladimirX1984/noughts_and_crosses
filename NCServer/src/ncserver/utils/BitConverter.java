/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package ncserver.utils;

import java.io.UnsupportedEncodingException;
import java.nio.charset.Charset;
import java.util.Arrays;

/**
 *
 * @author Vladimir
 */
public class BitConverter {

    public static byte[] getBytes(boolean x) {
        return new byte[]{(byte)(x ? 1 : 0)};
    }

    public static byte[] getBytes(char c) {
        return new byte[]{(byte)(c & 0xff), (byte)(c >> 8 & 0xff)};
    }

    public static byte[] getBytes(short x) {
        return new byte[]{(byte)x, (byte)(x >>> 8)};
    }

    public static byte[] getBytes(int x) {
        return new byte[]{(byte)x, (byte)(x >>> 8), (byte)(x >>> 16), (byte)(x >>> 24)};
    }

    public static byte[] getBytes(long x) {
        return new byte[]{(byte)x, (byte)(x >>> 8), (byte)(x >>> 16), (byte)(x >>> 24),
            (byte)(x >>> 32), (byte)(x >>> 40), (byte)(x >>> 48), (byte)(x >>> 56)};
    }

    public static byte[] getBytes(float x) {
        return getBytes(Float.floatToRawIntBits(x));
    }

    public static byte[] getBytes(double x) {
        return getBytes(Double.doubleToRawLongBits(x));
    }

    public static byte[] getBytes(String x) {
        return x.getBytes();
    }

    public static long doubleToInt64Bits(double x) {
        return Double.doubleToRawLongBits(x);
    }

    public static double int64BitsToDouble(long x) {
        return (double)x;
    }

    public static boolean toBoolean(byte[] bytes, int index) {
        return bytes[index] != 0;
    }

    public static char toChar(byte[] bytes, int index) {
        return (char)((0xff & bytes[index + 1]) << 8 | (0xff & bytes[index]));
    }

    public static double toDouble(byte[] bytes, int index) {
        return Double.longBitsToDouble(toInt64(bytes, index));
    }

    public static short toInt16(byte[] bytes, int index) {
        return (short)((0xff & bytes[index + 1]) << 8 | (0xff & bytes[index]));
    }

    public static int toInt32(byte[] bytes, int index) {
        int pos = index;
        return (int)((int)(0xff & bytes[pos]) << 24
            | (int)(0xff & bytes[--pos]) << 16
            | (int)(0xff & bytes[--pos]) << 8
            | (int)(0xff & bytes[--pos]));
    }

    public static long toInt64(byte[] bytes, int index) {
        int pos = index + 7;
        return (long)((long)(0xff & bytes[pos]) << 56
            | (long)(0xff & bytes[--pos]) << 48
            | (long)(0xff & bytes[--pos]) << 40
            | (long)(0xff & bytes[--pos]) << 32
            | (long)(0xff & bytes[--pos]) << 24
            | (long)(0xff & bytes[--pos]) << 16
            | (long)(0xff & bytes[--pos]) << 8
            | (long)(0xff & bytes[--pos]));
    }

    public static float toSingle(byte[] bytes, int index) {
        return Float.intBitsToFloat(toInt32(bytes, index));
    }

    public static String toString(byte[] bytes) {
        return toString(bytes, 0, bytes.length);
    }

    public static String toString(byte[] bytes, int pos, int len) {
        byte[] data = Arrays.copyOfRange(bytes, pos, pos + len);
        if (Charset.isSupported("UTF-8")) {
            Charset charset = Charset.forName("UTF-8");
            return new String(data, charset);
        }
        return new String(data);
    }

    public static String toUnicodeString(byte[] bytes) throws UnsupportedEncodingException {
        return toUnicodeString(bytes, 0, bytes.length);
    }

    public static String toUnicodeString(byte[] bytes, int pos, int len) throws UnsupportedEncodingException {
        byte[] data = Arrays.copyOfRange(bytes, pos, pos + len);
        return new String(data, "UTF-16");
    }

    public static int unsignedByteToInt(byte b) {
        return b & 0xFF;
    }
}
