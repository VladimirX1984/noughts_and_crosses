/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package ncserver;

import java.util.Arrays;

/**
 *
 * @author Vladimir
 */
public class NC_GameState {

    /**
     * массив данных о клетках
     * '?' - не заполнена клетка
     * 'X' - крестик
     * '0' - нолик
     */
    private char[] data;

    private int size;

    public NC_GameState() {
        data = new char[9];
        size = 3;
    }

    public void clear() {
        if (data != null) {
            data = null;
            size = 0;
        }
    }

    public void init(int count) {
        clear();
        data = new char[count * count];
        for (int i = 0; i < data.length; i++) {
            data[i] = '?';
        }
        size = count;
    }

    public void setCellValue(int number, char cellValue) {
        data[number] = cellValue;
    }

    public void setCellValue(int row, int coll, char cellValue) {
        data[row * size + coll] = cellValue;
    }

    public boolean isCellValid(int number) {
        if (number < 0 || number >= data.length) {
            return false;
        }
        return true;
    }

    public char getCellValue(int number) {
        if (!isCellValid(number)) {
            return 'n';
        }
        return data[number];
    }

    public char getCellValue(int row, int coll) {
        int number = row * size + coll;
        if (!isCellValid(number)) {
            return 'n';
        }
        return data[number];
    }

    public char[] getData() {
        return data;
    }

    public void setData(char[] data) {
        this.data = data;
        size = (int)Math.sqrt(this.data.length);
    }

    public String getString() {
        String str = new String(data);
        return str;
    }

    public String[] getStringArray() {
        String[] strs = new String[size];
        for (int i = 0; i < size; ++i) {
            char[] cells = Arrays.copyOfRange(data, i * size, (i + 1) * size);
            strs[i] = new String(cells);
        }
        return strs;
    }

    public void setData(String str) {
        clear();
        data = str.toCharArray();
        size = (int)Math.sqrt(data.length);
    }

    public int count() {
        return data.length;
    }

    public int getSize() {
        return size;
    }
}
