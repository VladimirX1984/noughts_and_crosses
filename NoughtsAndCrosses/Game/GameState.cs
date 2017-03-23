using System;
namespace NoughtsAndCrosses.Game {
  public class GameState {
    /// <summary>
    /// массив данных о клетках
    /// '?' - не заполнена клетка
    /// 'X' - крестик
    /// '0' - нолик
    /// </summary>
    private char[] _data;

    public GameState() {
      _data = new char[9];
      _size = 3;
    }

    ~GameState() {
      Clear();
    }

    public void Clear() {
      if (_data != null) {
        _data = null;
      }
    }

    public void Init(int count) {
      Clear();
      _data = new char[count * count];
      for (int i = 0; i < _data.Length; ++i) {
        _data[i] = '?';
      }
      _size = count;
    }

    #region методы работы с ячейками

    public void SetCellValue(int number, char cellValue) {
      _data[number] = cellValue;
    }

    public void SetCellValue(int row, int coll, char cellValue) {
      _data[row * _size + coll] = cellValue;
    }

    public bool IsCellValid(int number) {
      if (number < 0 || number >= _data.Length) {
        return false;
      }
      return true;
    }

    public char GetCellValue(int number) {
      if (!IsCellValid(number)) {
        return 'n';
      }
      return _data[number];
    }

    public char GetCellValue(int row, int coll) {
      int number = row * _size + coll;
      if (!IsCellValid(number)) {
        return 'n';
      }
      return _data[number];
    }

    public char[] Data {
      get { return _data; }
      set {
        _data = value;
        _size = (int)Math.Sqrt(_data.Length);
      }
    }

    public string DataString {
      get {
        string s = "";
        foreach (char ch in _data) {
          s += ch;
        }
        return s;
      }
      set {
        _data = value.ToCharArray();
        _size = (int)Math.Sqrt(_data.Length);
      }
    }

    public int Count {
      get { return _data.Length; }
    }

    public int Size { get { return _size; } }
    private int _size;

    #endregion
  }
}
