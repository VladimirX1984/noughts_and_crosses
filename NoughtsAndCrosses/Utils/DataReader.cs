using System;
using System.Text;

namespace NoughtsAndCrosses.Utils {
  /// <summary>
  /// Типизированное чтение данных из заданного массива
  /// </summary>
  public sealed class DataReader {

    private byte[] data;
    private int pos;
    private int begin;
    private int end;
    private bool isReadMode = true;

    #region Инициализация

    public DataReader() {
      AttachData(new byte[] {});
    }

    public DataReader(byte[] data) {
      AttachData(data, 0, data.Length);
    }

    public DataReader(byte[] data, int offset, int size) {
      AttachData(data, offset, size);
    }

    public DataReader(DataBuffer dataBuffer) {
      AttachData(dataBuffer);
    }

    public DataReader(DataBuffer dataBuffer, int offset) {
      AttachData(dataBuffer, offset);
    }

    public void AttachData(byte[] data) {
      AttachData(data, 0, data.Length);
    }

    public void AttachData(DataBuffer dataBuffer) {
      AttachData(dataBuffer, 0);
    }

    public void AttachData(byte[] aData, int offset, int size) {
      data = aData;
      begin = Math.Max(0, offset);
      end = begin + Math.Max(0, size);
      Reset();
    }

    public void AttachData(DataBuffer dataBuffer, int offset) {
      AttachData(dataBuffer.GetBuffer(), offset, (int)dataBuffer.GetBufferSize());
    }

    #endregion

    #region Позиционирование

    public void Reset() {
      pos = begin;
      isReadMode = true;
    }

    public void Seek(int apos) {
      pos = Math.Max(begin, Math.Min(end, apos));
    }

    public int GetPosition() {
      return pos;
    }

    public int GetDataSize() {
      return end - begin;
    }

    public bool EndOfData() {
      return pos >= end;
    }

    #endregion

    #region Чтение в заданной позиции

    public void SetReadMode() {
      isReadMode = true;
    }

    public void SetTestMode() {
      isReadMode = false;
    }

    private bool MayBeReadAt(int pos, int size) {
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

    public bool ReadAt(int pos, ref byte val) {
      if (!MayBeReadAt(pos, 1)) {
        return false;
      }
      if (isReadMode) {
        val = data[pos];
      }
      return true;
    }

    public bool ReadAt(int pos, ref sbyte val) {
      if (!MayBeReadAt(pos, 1)) {
        return false;
      }
      if (isReadMode) {
        val = (sbyte)data[pos];
      }
      return true;
    }

    public bool ReadAt(int pos, ref char val) {
      if (!MayBeReadAt(pos, 2)) {
        return false;
      }
      if (isReadMode) {
        val = BitConverter.ToChar(data, pos);
      }
      return true;
    }

    public bool ReadAt(int pos, ref bool val) {
      if (!MayBeReadAt(pos, 1)) {
        return false;
      }
      if (isReadMode) {
        val =  BitConverter.ToBoolean(data, pos);
      }
      return true;
    }

    public bool ReadAt(int pos, ref short val) {
      if (!MayBeReadAt(pos, 2)) {
        return false;
      }
      if (isReadMode) {
        val = BitConverter.ToInt16(data, pos);
      }
      return true;
    }

    public bool ReadAt(int pos, ref ushort val) {
      if (!MayBeReadAt(pos, 2)) {
        return false;
      }
      if (isReadMode) {
        val = BitConverter.ToUInt16(data, pos);
      }
      return true;
    }

    public bool ReadAt(int pos, ref int val) {
      if (!MayBeReadAt(pos, 4)) {
        return false;
      }
      if (isReadMode) {
        val = BitConverter.ToInt32(data, pos);
      }
      return true;
    }

    public bool ReadAt(int pos, ref uint val) {
      if (!MayBeReadAt(pos, 4)) {
        return false;
      }
      if (isReadMode) {
        val = BitConverter.ToUInt32(data, pos);
      }
      return true;
    }

    public bool ReadAt(int pos, ref long val) {
      if (!MayBeReadAt(pos, 8)) {
        return false;
      }
      if (isReadMode) {
        val = BitConverter.ToInt64(data, pos);
      }
      return true;
    }

    public bool ReadAt(int pos, ref ulong val) {
      if (!MayBeReadAt(pos, 8)) {
        return false;
      }
      if (isReadMode) {
        val = BitConverter.ToUInt64(data, pos);
      }
      return true;
    }

    public bool ReadAt(int pos, ref float val) {
      if (!MayBeReadAt(pos, 4)) {
        return false;
      }
      if (isReadMode) {
        val = BitConverter.ToSingle(data, pos);
      }
      return true;
    }

    public bool ReadAt(int pos, ref double val) {
      if (!MayBeReadAt(pos, 8)) {
        return false;
      }
      if (isReadMode) {
        val = BitConverter.ToDouble(data, pos);
      }
      return true;
    }

    public bool ReadAtUnicode(int pos, int size, ref string val) {
      if (!MayBeReadAt(pos, size * 2)) {
        return false;
      }
      if (isReadMode) {
        val = Encoding.Unicode.GetString(data, pos, size * 2);
      }
      return true;
    }

    private bool ReadAtLength(int pos, ref ushort val) {
      bool savedReadMode = isReadMode;
      try {
        isReadMode = true;
        if (!ReadAt(pos, ref val)) {
          return false;
        }
      }
      finally {
        isReadMode = savedReadMode;
      }

      return true;
    }

    private bool ReadAtLength(int pos, ref int val) {
      bool savedReadMode = isReadMode;
      try {
        isReadMode = true;
        if (!ReadAt(pos, ref val)) {
          return false;
        }
      }
      finally {
        isReadMode = savedReadMode;
      }

      return true;
    }

    private bool ReadAtUnicode(int pos, ref string val, ref ushort resLength) {
      if (!ReadAtLength(pos, ref resLength)) {
        return false;
      }
      return ReadAtUnicode(pos + 2, resLength, ref val);
    }

    public bool ReadAtUnicode(int pos, ref string val) {
      ushort len = 0;
      return ReadAtUnicode(pos, ref val, ref len);
    }

    private bool ReadAtUnicodeLarge(int pos, ref string val, ref int resLength) {
      if (!ReadAtLength(pos, ref resLength)) {
        return false;
      }
      return ReadAtUnicode(pos + 4, resLength, ref val);
    }

    public bool ReadAtUnicodeLarge(int pos, ref string val) {
      int len = 0;
      return ReadAtUnicodeLarge(pos, ref val, ref len);
    }

    public bool ReadAtASCII(int pos, int size, ref string val) {
      if (!MayBeReadAt(pos, size)) {
        return false;
      }
      if (isReadMode) {
        val = Encoding.ASCII.GetString(data, pos, size);
      }
      return true;
    }

    private bool ReadAtASCII(int pos, ref string val, ref ushort resLength) {
      if (!ReadAtLength(pos, ref resLength)) {
        return false;
      }

      return ReadAtASCII(pos + 2, resLength, ref val);
    }

    public bool ReadAtASCII(int pos, ref string val) {
      ushort len = 0;
      return ReadAtASCII(pos, ref val, ref len);
    }

    private bool ReadAtASCIILarge(int pos, ref string val, ref int resLength) {
      if (!ReadAtLength(pos, ref resLength)) {
        return false;
      }
      return ReadAtASCII(pos + 4, resLength, ref val);
    }

    public bool ReadAtASCIILarge(int pos, ref string val) {
      int len = 0;
      return ReadAtASCIILarge(pos, ref val, ref len);
    }

    public bool ReadAt(int pos, int size, ref byte[] val) {
      if (!MayBeReadAt(pos, size)) {
        return false;
      }
      if (isReadMode) {
        val = new byte[size];
        Array.Copy(data, pos, val, 0, size);
      }
      return true;
    }

    public bool ReadAtArray(int pos, int size, ref byte[] val) {
      return ReadAt(pos, size, ref val);
    }

    public bool ReadAtArray(int pos, ref byte[] val, ref ushort resLength) {
      if (!ReadAtLength(pos, ref resLength)) {
        return false;
      }

      return ReadAt(pos + 2, resLength, ref val);
    }

    public bool ReadAtArray(int pos, ref byte[] val) {
      ushort len = 0;
      return ReadAtArray(pos, ref val, ref len);
    }

    public bool ReadAtArray(int pos, DataBuffer val, ref ushort resLength) {
      if (!ReadAtLength(pos, ref resLength)) {
        return false;
      }

      if (!MayBeReadAt(pos + 2, resLength)) {
        return false;
      }

      if (isReadMode) {
        val.AddArray(data, (uint)pos + 2, resLength);
      }
      return true;
    }

    public bool ReadAtArray(int pos, DataBuffer val) {
      ushort len = 0;
      return ReadAtArray(pos, val, ref len);
    }

    public bool ReadAtArrayLarge(int pos, ref byte[] val, ref int resLength) {
      if (!ReadAtLength(pos, ref resLength)) {
        return false;
      }

      return ReadAt(pos + 4, resLength, ref val);
    }

    public bool ReadAtArrayLarge(int pos, ref byte[] val) {
      int len = 0;
      return ReadAtArrayLarge(pos, ref val, ref len);
    }

    public bool ReadAtArrayLarge(int pos, DataBuffer val, ref int resLength) {
      if (!ReadAtLength(pos, ref resLength)) {
        return false;
      }
      pos += 4;

      if (!MayBeReadAt(pos, resLength)) {
        return false;
      }

      if (isReadMode) {
        val.AddLargeArray(data, (uint)pos, (uint)resLength);
      }
      return true;
    }

    public bool ReadAtArrayLarge(int pos, DataBuffer val) {
      int len = 0;
      return ReadAtArrayLarge(pos, val, ref len);
    }

    #endregion

    #region Чтение

    public bool Read(ref byte val) {
      if (!ReadAt(pos, ref val)) {
        return false;
      }
      pos += 1;
      return true;
    }

    public bool Read(ref sbyte val) {
      if (!ReadAt(pos, ref val)) {
        return false;
      }
      pos += 1;
      return true;
    }

    public bool Read(ref char val) {
      if (!ReadAt(pos, ref val)) {
        return false;
      }
      pos += 2;
      return true;
    }

    public bool Read(ref bool val) {
      if (!ReadAt(pos, ref val)) {
        return false;
      }
      pos += 1;
      return true;
    }

    public bool Read(ref short val) {
      if (!ReadAt(pos, ref val)) {
        return false;
      }
      pos += 2;
      return true;
    }

    public bool Read(ref ushort val) {
      if (!ReadAt(pos, ref val)) {
        return false;
      }
      pos += 2;
      return true;
    }

    public bool Read(ref int val) {
      if (!ReadAt(pos, ref val)) {
        return false;
      }
      pos += 4;
      return true;
    }

    public bool Read(ref uint val) {
      if (!ReadAt(pos, ref val)) {
        return false;
      }
      pos += 4;
      return true;
    }

    public bool Read(ref long val) {
      if (!ReadAt(pos, ref val)) {
        return false;
      }
      pos += 8;
      return true;
    }

    public bool Read(ref ulong val) {
      if (!ReadAt(pos, ref val)) {
        return false;
      }
      pos += 8;
      return true;
    }

    public bool Read(ref float val) {
      if (!ReadAt(pos, ref val)) {
        return false;
      }
      pos += 4;
      return true;
    }

    public bool Read(ref double val) {
      if (!ReadAt(pos, ref val)) {
        return false;
      }
      pos += 8;
      return true;
    }

    public bool ReadUnicode(int size, ref string val) {
      if (!ReadAtUnicode(pos, size, ref val)) {
        return false;
      }
      pos += size * 2;
      return true;
    }

    public bool ReadUnicode(ref string val) {
      ushort len = 0;
      if (!ReadAtUnicode(pos, ref val, ref len) || len < 0) {
        return false;
      }
      pos += 2 + len * 2;
      return true;
    }

    public bool ReadUnicodeLarge(ref string val) {
      int len = 0;
      if (!ReadAtUnicodeLarge(pos, ref val, ref len) || len < 0) {
        return false;
      }
      pos += 4 + len * 2;
      return true;
    }

    public bool ReadASCII(int size, ref string val) {
      if (!ReadAtASCII(pos, size, ref val)) {
        return false;
      }
      pos += size;
      return true;
    }

    public bool ReadASCII(ref string val) {
      ushort len = 0;
      if (!ReadAtASCII(pos, ref val, ref len)) {
        return false;
      }
      pos += 2 + len;
      return true;
    }

    public bool ReadASCIILarge(ref string val) {
      int len = 0;
      if (!ReadAtASCIILarge(pos, ref val, ref len) || len < 0) {
        return false;
      }
      pos += 4 + len;
      return true;
    }

    public bool Read(int size, ref byte[] val) {
      if (!ReadAt(pos, size, ref val)) {
        return false;
      }
      pos += size;
      return true;
    }

    public bool ReadArray(int size, ref byte[] val) {
      return Read(size, ref val);
    }

    public bool ReadArray(ref byte[] val) {
      ushort len = 0;
      if (!ReadAtArray(pos, ref val, ref len)) {
        return false;
      }
      pos += 2 + len;
      return true;
    }

    public bool ReadArray(DataBuffer aDataBuffer) {
      ushort len = 0;
      if (!ReadAtArray(pos, aDataBuffer, ref len)) {
        return false;
      }
      pos += 2 + len;
      return true;
    }

    public bool ReadArrayLarge(ref byte[] val) {
      int len = 0;
      if (!ReadAtArrayLarge(pos, ref val, ref len)) {
        return false;
      }
      pos += 4 + len;
      return true;
    }

    public bool ReadString(ref string resValue) {
      return ReadAtASCII(pos, end, ref resValue);
    }

    #endregion
  }
}
