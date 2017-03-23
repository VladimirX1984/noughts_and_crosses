/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package ncserver.utils;

import java.io.UnsupportedEncodingException;
import java.util.Arrays;
import java.util.concurrent.atomic.AtomicReference;

/**
 *
 * @author Vladimir
 */
public final class DataReader {

    private byte[] data;
    private int pos;
    private int begin;
    private int end;
    private boolean isReadMode = true;

    // <editor-fold defaultstate="collapsed" desc="Инициализация">
    public DataReader() {
        attachData(new byte[]{});
    }

    public DataReader(byte[] data) {
        attachData(data, 0, data.length);
    }

    public DataReader(byte[] data, int offset, int dataSize) {
        attachData(data, offset, dataSize);
    }

    public DataReader(DataBuffer dataBuffer) {
        attachData(dataBuffer);
    }

    public DataReader(DataBuffer dataBuffer, int offset) {
        attachData(dataBuffer, offset);
    }

    public void attachData(byte[] data) {
        attachData(data, 0, data.length);
    }

    public void attachData(DataBuffer dataBuffer) {
        attachData(dataBuffer, 0);
    }

    public void attachData(byte[] data, int offset, int dataSize) {
        this.data = data;
        this.begin = Math.max(0, offset);
        this.end = this.begin + Math.max(0, dataSize);
        reset();
    }

    public void attachData(DataBuffer dataBuffer, int offset) {
        attachData(dataBuffer.getBuffer(), offset, dataBuffer.getBufferSize());
    }
    // </editor-fold>

    // <editor-fold defaultstate="collapsed" desc="Позиционирование">
    public void reset() {
        pos = begin;
        isReadMode = true;
    }

    public void seek(int apos) {
        pos = Math.max(begin, Math.min(end, apos));
    }

    public int getPosition() {
        return pos;
    }

    public int getDataSize() {
        return end - begin;
    }

    public boolean endOfData() {
        return pos >= end;
    }
    // </editor-fold>

    // <editor-fold defaultstate="collapsed" desc="Чтение в заданной позиции">
    public void setReadMode() {
        isReadMode = true;
    }

    public void setTestMode() {
        isReadMode = false;
    }

    protected boolean mayBeReadAt(int pos, int size) {
        if (pos < begin) {
            return false;
        }
        if (size < 0) {
            return false;
        }
        if (pos + size > end) {
            return false;
        }
        return true;
    }

    public boolean readAtByte(int pos, AtomicReference<Byte> resValue) {
        if (!mayBeReadAt(pos, 1)) {
            return false;
        }
        if (isReadMode) {
            resValue.set(data[pos]);
        }
        return true;
    }

    public boolean readAtChar(int pos, AtomicReference<Character> resValue) {
        if (!mayBeReadAt(pos, 2)) {
            return false;
        }
        if (isReadMode) {
            resValue.set(BitConverter.toChar(data, pos));
        }
        return true;
    }

    public boolean readAtBoolean(int pos, AtomicReference<Boolean> resValue) {
        if (!mayBeReadAt(pos, 1)) {
            return false;
        }
        if (isReadMode) {
            resValue.set((data[pos] > 0));
        }
        return true;
    }

    public boolean readAtShort(int pos, AtomicReference<Short> resValue) {
        if (!mayBeReadAt(pos, 2)) {
            return false;
        }
        if (isReadMode) {
            resValue.set(BitConverter.toInt16(data, pos));
        }
        return true;
    }

    public boolean readAtInteger(int pos, AtomicReference<Integer> resValue) {
        if (!mayBeReadAt(pos, 4)) {
            return false;
        }
        if (isReadMode) {
            resValue.set(BitConverter.toInt32(data, pos));
        }
        return true;
    }

    public boolean readAtLong(int pos, AtomicReference<Long> resValue) {
        if (!mayBeReadAt(pos, 8)) {
            return false;
        }
        if (isReadMode) {
            resValue.set(BitConverter.toInt64(data, pos));
        }
        return true;
    }

    public boolean readAtFloat(int pos, AtomicReference<Float> resValue) {
        if (!mayBeReadAt(pos, 4)) {
            return false;
        }
        if (isReadMode) {
            resValue.set(BitConverter.toSingle(data, pos));
        }
        return true;
    }

    public boolean readAtDouble(int pos, AtomicReference<Double> resValue) {
        if (!mayBeReadAt(pos, 8)) {
            return false;
        }
        if (isReadMode) {
            resValue.set(BitConverter.toDouble(data, pos));
        }
        return true;
    }

    public boolean readAtUnicode(int pos, int size, AtomicReference<String> resValue) {
        if (!mayBeReadAt(pos, size * 2)) {
            return false;
        }
        if (isReadMode) {
            try {
                resValue.set(BitConverter.toUnicodeString(data, pos, size * 2));
            }
            catch (UnsupportedEncodingException ex) {
                return false;
            }
        }
        return true;
    }

    protected boolean readAtLength(int pos, AtomicReference<Short> resValue) {
        boolean savedReadMode = isReadMode;
        try {
            isReadMode = true;
            if (!readAtShort(pos, resValue)) {
                return false;
            }
        }
        finally {
            isReadMode = savedReadMode;
        }
        return true;
    }

    protected boolean readAtLargeLength(int pos, AtomicReference<Integer> resValue) {
        boolean savedReadMode = isReadMode;
        try {
            isReadMode = true;
            if (!readAtInteger(pos, resValue)) {
                return false;
            }
        }
        finally {
            isReadMode = savedReadMode;
        }
        return true;
    }

    protected boolean readAtUnicode(int pos, AtomicReference<String> resValue,
                                    AtomicReference<Short> resLength) {
        if (!readAtLength(pos, resLength)) {
            return false;
        }
        return readAtUnicode(pos + 2, resLength.get(), resValue);
    }

    public boolean readAtUnicode(int pos, AtomicReference<String> resValue) {
        short len = 0;
        AtomicReference<Short> slen = new AtomicReference<>(len);
        return readAtUnicode(pos, resValue, slen);
    }

    protected boolean readAtUnicodeLarge(int pos, AtomicReference<String> resValue,
                                         AtomicReference<Integer> resLength) {
        if (!readAtLargeLength(pos, resLength)) {
            return false;
        }
        return readAtUnicode(pos + 4, resLength.get(), resValue);
    }

    public boolean readAtUnicodeLarge(int pos, AtomicReference<String> resValue) {
        int len = 0;
        AtomicReference<Integer> ilen = new AtomicReference<>(len);
        return readAtUnicodeLarge(pos, resValue, ilen);
    }

    public boolean readAtASCII(int pos, int size, AtomicReference<String> resValue) {
        if (!mayBeReadAt(pos, size)) {
            return false;
        }
        if (isReadMode) {
            resValue.set(BitConverter.toString(data, pos, size));
        }
        return true;
    }

    protected boolean readAtASCII(int pos, AtomicReference<String> resValue,
                                  AtomicReference<Short> resLength) {
        if (!readAtLength(pos, resLength)) {
            return false;
        }
        return readAtASCII(pos + 2, resLength.get(), resValue);
    }

    public boolean readAtASCII(int pos, AtomicReference<String> resValue) {
        short len = 0;
        AtomicReference<Short> slen = new AtomicReference<>(len);
        return readAtASCII(pos, resValue, slen);
    }

    protected boolean readAtASCIILarge(int pos, AtomicReference<String> resValue,
                                       AtomicReference<Integer> resLength) {
        if (!readAtLargeLength(pos, resLength)) {
            return false;
        }
        return readAtASCII(pos + 4, resLength.get(), resValue);
    }

    public boolean readAtASCIILarge(int pos, AtomicReference<String> resValue) {
        int len = 0;
        AtomicReference<Integer> ilen = new AtomicReference<>(len);
        return readAtASCIILarge(pos, resValue, ilen);
    }

    public boolean readAt(int pos, int size, AtomicReference<byte[]> resValue) {
        if (!mayBeReadAt(pos, size)) {
            return false;
        }
        if (isReadMode) {
            byte[] bytes = Arrays.copyOfRange(data, pos, pos + size);
            resValue.set(bytes);
        }
        return true;
    }

    public boolean readAtArray(int pos, int size, AtomicReference<byte[]> resValue) {
        return readAt(pos, size, resValue);
    }

    public boolean readAtArray(int pos, AtomicReference<byte[]> resValue,
                               AtomicReference<Short> resLength) {
        if (!readAtLength(pos, resLength)) {
            return false;
        }
        return readAt(pos + 2, resLength.get(), resValue);
    }

    public boolean readAtArray(int pos, AtomicReference<byte[]> resValue) {
        short len = 0;
        AtomicReference<Short> slen = new AtomicReference<>(len);
        return readAtArray(pos, resValue, slen);
    }

    public boolean readAtArray(int pos, DataBuffer resValue, AtomicReference<Short> resLength) {
        if (!readAtLength(pos, resLength)) {
            return false;
        }
        if (!mayBeReadAt(pos + 2, resLength.get())) {
            return false;
        }
        if (isReadMode) {
            resValue.addArray(data, pos + 2, resLength.get());
        }
        return true;
    }

    public boolean readAtArray(int pos, DataBuffer resValue) {
        short len = 0;
        AtomicReference<Short> slen = new AtomicReference<>(len);
        return readAtArray(pos, resValue, slen);
    }

    public boolean readAtArrayLarge(int pos, AtomicReference<byte[]> resValue,
                                    AtomicReference<Integer> resLength) {
        if (!readAtLargeLength(pos, resLength)) {
            return false;
        }
        return readAt(pos + 4, resLength.get(), resValue);
    }

    public boolean readAtArrayLarge(int pos, AtomicReference<byte[]> resValue) {
        int len = 0;
        AtomicReference<Integer> ilen = new AtomicReference<>(len);
        return readAtArrayLarge(pos, resValue, ilen);
    }

    public boolean readAtArrayLarge(int pos, DataBuffer resValue, AtomicReference<Integer> resLength) {
        if (!readAtLargeLength(pos, resLength)) {
            return false;
        }
        pos += 4;
        if (!mayBeReadAt(pos, resLength.get())) {
            return false;
        }
        if (isReadMode) {
            resValue.addLargeArray(data, pos, resLength.get());
        }
        return true;
    }

    public boolean readAtArrayLarge(int pos, DataBuffer resValue) {
        int len = 0;
        AtomicReference<Integer> ilen = new AtomicReference<>(len);
        return readAtArrayLarge(pos, resValue, ilen);
    }
    // </editor-fold>

    // <editor-fold defaultstate="collapsed" desc="Чтение">
    public boolean readByte(AtomicReference<Byte> resValue) {
        if (!readAtByte(pos, resValue)) {
            return false;
        }
        pos += 1;
        return true;
    }

    public boolean readChar(AtomicReference<Character> resValue) {
        if (!readAtChar(pos, resValue)) {
            return false;
        }
        pos += 2;
        return true;
    }

    public boolean readBoolean(AtomicReference<Boolean> resValue) {
        if (!readAtBoolean(pos, resValue)) {
            return false;
        }
        pos += 1;
        return true;
    }

    public boolean readShort(AtomicReference<Short> resValue) {
        if (!readAtShort(pos, resValue)) {
            return false;
        }
        pos += 2;
        return true;
    }

    public boolean readInteger(AtomicReference<Integer> resValue) {
        if (!readAtInteger(pos, resValue)) {
            return false;
        }
        pos += 4;
        return true;
    }

    public boolean readLong(AtomicReference<Long> resValue) {
        if (!readAtLong(pos, resValue)) {
            return false;
        }
        pos += 8;
        return true;
    }

    public boolean readFloat(AtomicReference<Float> resValue) {
        if (!readAtFloat(pos, resValue)) {
            return false;
        }
        pos += 4;
        return true;
    }

    public boolean readDouble(AtomicReference<Double> resValue) {
        if (!readAtDouble(pos, resValue)) {
            return false;
        }
        pos += 8;
        return true;
    }

    public boolean readUnicode(int size, AtomicReference<String> resValue) {
        if (!readAtUnicode(pos, size, resValue)) {
            return false;
        }
        pos += size * 2;
        return true;
    }

    public boolean readUnicode(AtomicReference<String> resValue) {
        short len = 0;
        AtomicReference<Short> slen = new AtomicReference<>(len);
        if (!readAtUnicode(pos, resValue, slen) || slen.get() < 0) {
            return false;
        }
        pos += 2 + slen.get() * 2;
        return true;
    }

    public boolean readUnicodeLarge(AtomicReference<String> resValue) {
        int len = 0;
        AtomicReference<Integer> ilen = new AtomicReference<>(len);
        if (!readAtUnicodeLarge(pos, resValue, ilen) || ilen.get() < 0) {
            return false;
        }
        pos += 4 + ilen.get() * 2;
        return true;
    }

    public boolean readASCII(int size, AtomicReference<String> resValue) {
        if (!readAtASCII(pos, size, resValue)) {
            return false;
        }
        pos += size;
        return true;
    }

    public boolean readASCII(AtomicReference<String> resValue) {
        short len = 0;
        AtomicReference<Short> slen = new AtomicReference<>(len);
        if (!readAtASCII(pos, resValue, slen) || slen.get() < 0) {
            return false;
        }
        pos += 2 + slen.get();
        return true;
    }

    public boolean readASCIILarge(AtomicReference<String> resValue) {
        int len = 0;
        AtomicReference<Integer> ilen = new AtomicReference<>(len);
        if (!readAtASCIILarge(pos, resValue, ilen) || ilen.get() < 0) {
            return false;
        }
        pos += 4 + ilen.get();
        return true;
    }

    public boolean read(int size, AtomicReference<byte[]> resValue) {
        if (!readAt(pos, size, resValue)) {
            return false;
        }
        pos += size;
        return true;
    }

    public boolean readArray(int size, AtomicReference<byte[]> resValue) {
        return read(size, resValue);
    }

    public boolean readArray(AtomicReference<byte[]> resValue) {
        short len = 0;
        AtomicReference<Short> slen = new AtomicReference<>(len);
        if (!readAtArray(pos, resValue, slen)) {
            return false;
        }
        pos += 2 + slen.get();
        return true;
    }

    public boolean readArray(DataBuffer dataBuffer) {
        short len = 0;
        AtomicReference<Short> slen = new AtomicReference<>(len);
        if (!readAtArray(pos, dataBuffer, slen)) {
            return false;
        }
        pos += 2 + slen.get();
        return true;
    }

    public boolean readArrayLarge(AtomicReference<byte[]> resValue) {
        int len = 0;
        AtomicReference<Integer> ilen = new AtomicReference<>(len);
        if (!readAtArrayLarge(pos, resValue, ilen)) {
            return false;
        }
        pos += 4 + ilen.get();
        return true;
    }

    public boolean readArrayLarge(DataBuffer dataBuffer) {
        int len = 0;
        AtomicReference<Integer> ilen = new AtomicReference<>(len);
        if (!readAtArrayLarge(pos, dataBuffer, ilen)) {
            return false;
        }
        pos += 2 + ilen.get();
        return true;
    }

    public boolean readString(AtomicReference<String> resValue) {
        return readAtASCII(pos, end, resValue);
    }
    // </editor-fold>
}
