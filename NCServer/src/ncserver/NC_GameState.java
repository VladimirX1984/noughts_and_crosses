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
    private char[] _data;

    private int _size;

    public NC_GameState() {
        _data = new char[9];
        _size = 3;
    }

    public void clear() {
        if (_data != null) {
            _data = null;
            _size = 0;
        }
    }

    public void init(int count) {
        clear();
        _data = new char[count * count];
        for (int i = 0; i < _data.length; i++) {
            _data[i] = '?';
        }
        _size = count;
    }

    public void setCellValue(int number, char cellValue) {
        _data[number] = cellValue;
    }

    public void setCellValue(int row, int coll, char cellValue) {
        _data[row * _size + coll] = cellValue;
    }

    public boolean isCellValid(int number) {
        if (number < 0 || number >= _data.length) {
            return false;
        }
        return true;
    }

    public char getCellValue(int number) {
        if (!isCellValid(number)) {
            return 'n';
        }
        return _data[number];
    }

    public char getCellValue(int row, int coll) {
        int number = row * _size + coll;
        if (!isCellValid(number)) {
            return 'n';
        }
        return _data[number];
    }

    public char[] getData() {
        return _data;
    }

    public void setData(char[] data) {
        _data = data;
        _size = (int)Math.sqrt(_data.length);
    }

    public String getString() {
        String str = new String(_data);
        return str;
    }

    public String[] getStringArray() {
        String[] strs = new String[_size];
        for (int i = 0; i < _size; ++i) {
            char[] cells = Arrays.copyOfRange(_data, i * _size, (i + 1) * _size);
            strs[i] = new String(cells);
        }
        return strs;
    }

    public void setData(String str) {
        clear();
        _data = str.toCharArray();
        _size = (int)Math.sqrt(_data.length);
    }

    public int count() {
        return _data.length;
    }

    public int size() {
        return _size;
    }

    public void print() {
        for (int i = 0; i < _data.length; ++i) {
            System.out.print(_data[i]);
        }
        System.out.println(" ");
    }
}
