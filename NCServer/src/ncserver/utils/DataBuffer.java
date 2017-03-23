/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package ncserver.utils;

import java.io.IOException;
import java.io.InputStream;

/**
 *
 * @author Vladimir
 */
public final class DataBuffer {

    // <editor-fold defaultstate="collapsed" desc="Инициализация">
    protected byte[] buffer;
    protected int bufferSize;

    public DataBuffer(int capacity) {
        buffer = new byte[capacity];
        bufferSize = 0;
    }

    public DataBuffer() {
        buffer = new byte[1];
        bufferSize = 0;
    }

    public DataBuffer(String str) {
        this(str.length());
        setAt(0, BitConverter.getBytes(str));
    }

    public int getBufferSize() {
        return bufferSize;
    }

    public void clear() {
        bufferSize = 0;
    }

    public byte[] getBufferCopy() {
        byte[] bufferCopy = new byte[bufferSize];
        synchronized (buffer) {
            System.arraycopy(buffer, 0, bufferCopy, 0, bufferSize);
        }
        return bufferCopy;
    }

    public byte[] getBuffer() {
        return buffer;
    }

    protected void grow(int capacity) {
        if (buffer.length < capacity) {
            int newSize = Math.max(capacity, buffer.length * 2);
            byte[] newBuffer = new byte[newSize];
            System.arraycopy(buffer, 0, newBuffer, 0, buffer.length);
            buffer = newBuffer;
        }
    }
    // </editor-fold>

    // <editor-fold defaultstate="collapsed" desc="Добавление в конец">
    public void add(byte[] data, int dataOffset, int size) {
        setAt(bufferSize, data, dataOffset, size);
    }

    public void add(byte[] data, int size) {
        setAt(bufferSize, data, 0, size);
    }

    public void add(byte[] data) {
        setAt(bufferSize, data, 0, (int)data.length);
    }

    public void add(byte number) {
        setAt(bufferSize, number);
    }

    public void add(boolean number) {
        setAt(bufferSize, number);
    }

    public void add(char number) {
        setAt(bufferSize, BitConverter.getBytes(number));
    }

    public void add(short number) {
        setAt(bufferSize, BitConverter.getBytes(number));
    }

    public void add(int number) {
        setAt(bufferSize, BitConverter.getBytes(number));
    }

    public void add(long number) {
        setAt(bufferSize, BitConverter.getBytes(number));
    }

    public void add(float number) {
        setAt(bufferSize, BitConverter.getBytes(number));
    }

    public void add(double number) {
        setAt(bufferSize, BitConverter.getBytes(number));
    }

    public void addUnicode(String str) {
        setAtUnicode(bufferSize, str);
    }

    public void addLargeUnicode(String str) {
        setAtLargeUnicode(bufferSize, str);
    }

    public void addASCII(String str) {
        setAtASCII(bufferSize, str);
    }

    public void addLargeASCII(String str) {
        setAtLargeASCII(bufferSize, str);
    }

    public void addArray(byte[] data, int dataOffset, short size) {
        setAtArray(bufferSize, data, dataOffset, size);
    }

    public void addArray(byte[] data) {
        setAtArray(bufferSize, data, 0, (short)data.length);
    }

    public void addLargeArray(byte[] data) {
        setAtArray(bufferSize, data, 0, (int)data.length);
    }

    public void addLargeArray(byte[] data, int dataOffset, int size) {
        setAtArray(bufferSize, data, dataOffset, size);
    }

    public void addData(DataBuffer data) {
        setAtArray(bufferSize, data.getBuffer(), 0, data.getBufferSize());
    }

    public boolean addData(InputStream ins) {
        return setAt(bufferSize, ins);
    }

    public void truncate(int offset) {
        bufferSize = Math.min(bufferSize, offset);
    }
    // </editor-fold>

    // <editor-fold defaultstate="collapsed" desc="Вставка по заданному смещению">
    public void setAt(int offset, byte[] data, int dataOffset, int size) {
        synchronized (buffer) {
            grow(offset + size);
            System.arraycopy(data, dataOffset, buffer, offset, size);
            bufferSize = Math.max(bufferSize, offset + size);
        }
    }

    public void setAt(int offset, byte[] data, int size) {
        setAt(offset, data, 0, size);
    }

    public void setAt(int offset, byte[] data) {
        setAt(offset, data, 0, (int)data.length);
    }

    public void setAt(int offset, byte number) {
        synchronized (buffer) {
            grow(offset + 1);
            buffer[offset] = number;
            bufferSize = Math.max(bufferSize, offset + 1);
        }
    }

    public void setAt(int offset, boolean number) {
        setAt(offset, BitConverter.getBytes(number));
    }

    public void setAt(int offset, char number) {
        setAt(offset, BitConverter.getBytes(number));
    }

    public void setAt(int offset, short number) {
        setAt(offset, BitConverter.getBytes(number));
    }

    public void setAt(int offset, int number) {
        setAt(offset, BitConverter.getBytes(number));
    }

    public void setAt(int offset, long number) {
        setAt(offset, BitConverter.getBytes(number));
    }

    public void setAt(int offset, float number) {
        setAt(offset, BitConverter.getBytes(number));
    }

    public void setAt(int offset, double number) {
        setAt(offset, BitConverter.getBytes(number));
    }

    public void setAtUnicode(int offset, String str) {
        synchronized (buffer) {
            short len = (short)str.length();
            setAt(offset, BitConverter.getBytes(len));
            if (str.length() == 0) {
                return;
            }
            setAt(offset + 2, BitConverter.getBytes(str));
        }
    }

    public void setAtLargeUnicode(int offset, String str) {
        synchronized (buffer) {
            setAt(offset, BitConverter.getBytes(str.length()));
            if (str.length() == 0) {
                return;
            }
            setAt(offset + 4, BitConverter.getBytes(str));
        }
    }

    public void setAtASCII(int offset, String str) {
        synchronized (buffer) {
            short len = (short)str.length();
            setAt(offset, BitConverter.getBytes(len));
            if (str.length() == 0) {
                return;
            }
            setAt(offset + 2, BitConverter.getBytes(str));
        }
    }

    public void setAtLargeASCII(int offset, String str) {
        synchronized (buffer) {
            setAt(offset, BitConverter.getBytes(str.length()));
            if (str.length() == 0) {
                return;
            }
            setAt(offset + 4, BitConverter.getBytes(str));
        }
    }

    public void setAtArray(int offset, byte[] data, int dataOffset, short size) {
        synchronized (buffer) {
            setAt(offset, size);
            if (size == 0) {
                return;
            }
            setAt(offset + 2, data, dataOffset, size);
        }
    }

    public void setAtArray(int offset, byte[] data) {
        setAtArray(offset, data, 0, (short)data.length);
    }

    public void setAtArray(int offset, byte[] data, int dataOffset, int size) {
        synchronized (buffer) {
            setAt(offset, size);
            if (size == 0) {
                return;
            }
            setAt(offset + 4, data, dataOffset, size);
        }
    }

    public boolean setAt(int offset, InputStream ins) {
        try {
            int i = 0, c = 0;
            while (true) {
                grow(offset + i + 1);
                c = ins.read();
                if (c == -1) {
                    break;
                }
                buffer[offset + i] = (byte)c;
                ++i;
            }
            bufferSize = Math.max(bufferSize, offset + i);
            return true;
        }
        catch (IOException ex) {
            System.err.println(ex);
            return false;
        }
    }

    // </editor-fold>
}
