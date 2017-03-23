using System;
using System.IO;
using System.Text;

namespace NoughtsAndCrosses.Utils {
  public sealed class DataBuffer {

    #region Инициализация

    private byte[] buffer;
    private uint bufferSize;
    private uint size;

    public DataBuffer(int aCapacity) {
      buffer = new byte[aCapacity];
      bufferSize = 0;
    }

    public DataBuffer() : this(1) {
    }

    public DataBuffer(byte[] data, uint size) {
      this.buffer = data;
      this.size = size;
    }

    public DataBuffer(string str) : this(str.Length) {
      SetAt(0, Encoding.Default.GetBytes(str));
    }

    public uint GetBufferSize() {
      return bufferSize;
    }

    public void Clear() {
      bufferSize = 0;
    }

    public byte[] GetBufferCopy() {
      byte[] bufferCopy = new byte[bufferSize];
      lock (buffer)
        Array.Copy(buffer, 0, bufferCopy, 0, bufferSize);
      return bufferCopy;
    }

    public byte[] GetBuffer() {
      return buffer;
    }

    private void Grow(uint aCapacity) {
      if (buffer.Length < aCapacity) {
        uint newSize = (uint)Math.Max(aCapacity, buffer.Length * 2);
        byte[] newBuffer = new byte[newSize];
        Array.Copy(buffer, 0, newBuffer, 0, buffer.Length);
        buffer = newBuffer;
      }
    }
    #endregion

    #region Добавление в конец

    public void Add(byte[] data, uint dataOffset, uint aSize) {
      SetAt(bufferSize, data, dataOffset, aSize);
    }

    public void Add(byte[] data, uint aSize) {
      SetAt(bufferSize, data, 0, aSize);
    }

    public void Add(byte[] data) {
      SetAt(bufferSize, data, 0, (uint)data.Length);
    }

    public void Add(byte val) {
      SetAt(bufferSize, val);
    }

    public void Add(bool val) {
      SetAt(bufferSize, val);
    }

    public void Add(char val) {
      SetAt(bufferSize, BitConverter.GetBytes(val));
    }

    public void Add(short val) {
      SetAt(bufferSize, BitConverter.GetBytes(val));
    }

    public void Add(ushort val) {
      SetAt(bufferSize, BitConverter.GetBytes(val));
    }

    public void Add(int val) {
      SetAt(bufferSize, BitConverter.GetBytes(val));
    }

    public void Add(uint val) {
      SetAt(bufferSize, BitConverter.GetBytes(val));
    }

    public void Add(long val) {
      SetAt(bufferSize, BitConverter.GetBytes(val));
    }

    public void Add(ulong val) {
      SetAt(bufferSize, BitConverter.GetBytes(val));
    }

    public void Add(float val) {
      SetAt(bufferSize, BitConverter.GetBytes(val));
    }

    public void Add(double val) {
      SetAt(bufferSize, BitConverter.GetBytes(val));
    }

    public void AddUnicode(string str) {
      SetAtUnicode(bufferSize, str);
    }

    public void AddLargeUnicode(string str) {
      SetAtLargeUnicode(bufferSize, str);
    }

    public void AddASCII(string str) {
      SetAtASCII(bufferSize, str);
    }

    public void AddLargeASCII(string str) {
      SetAtLargeASCII(bufferSize, str);
    }

    public void AddArray(byte[] data, uint dataOffset, ushort aSize) {
      SetAtArray(bufferSize, data, dataOffset, aSize);
    }

    public void AddArray(byte[] data) {
      SetAtArray(bufferSize, data, 0, (ushort)data.Length);
    }

    public void AddLargeArray(byte[] data) {
      SetAtArray(bufferSize, data, 0, (uint)data.Length);
    }

    public void AddLargeArray(byte[] data, uint dataOffset, uint aSize) {
      SetAtArray(bufferSize, data, dataOffset, aSize);
    }

    public void AddData(DataBuffer data) {
      SetAtArray(bufferSize, data.GetBuffer(), 0, data.GetBufferSize());
    }

    public bool AddData(BinaryReader ins) {
      return SetAt(bufferSize, ins);
    }

    public void Truncate(uint offset) {
      bufferSize = Math.Min(bufferSize, offset);
    }
    #endregion

    #region Вставка по заданному смещению

    public void SetAt(uint offset, byte[] data, uint dataOffset, uint aSize) {
      lock (buffer) {
        Grow(offset + aSize);
        Array.Copy(data, dataOffset, buffer, offset, aSize);
        bufferSize = Math.Max(bufferSize, offset + aSize);
      }
    }

    public void SetAt(uint offset, byte[] data, uint aSize) {
      SetAt(offset, data, 0, aSize);
    }

    public void SetAt(uint offset, byte[] data) {
      SetAt(offset, data, 0, (uint)data.Length);
    }

    public void SetAt(uint offset, byte val) {
      lock (buffer) {
        Grow(offset + 1);
        buffer[offset] = val;
        bufferSize = Math.Max(bufferSize, offset + 1);
      }
    }

    public void SetAt(uint offset, bool val) {
      SetAt(offset, BitConverter.GetBytes(val)) ;
    }

    public void SetAt(uint offset, char val) {
      SetAt(offset, BitConverter.GetBytes(val));
    }

    public void SetAt(uint offset, short val) {
      SetAt(offset, BitConverter.GetBytes(val));
    }

    public void SetAt(uint offset, ushort val) {
      SetAt(offset, BitConverter.GetBytes(val));
    }

    public void SetAt(uint offset, int val) {
      SetAt(offset, BitConverter.GetBytes(val));
    }

    public void SetAt(uint offset, uint val) {
      SetAt(offset, BitConverter.GetBytes(val));
    }

    public void SetAt(uint offset, long val) {
      SetAt(offset, BitConverter.GetBytes(val));
    }

    public void SetAt(uint offset, ulong val) {
      SetAt(offset, BitConverter.GetBytes(val));
    }

    public void SetAt(uint offset, float val) {
      SetAt(offset, BitConverter.GetBytes(val));
    }

    public void SetAt(uint offset, double val) {
      SetAt(offset, BitConverter.GetBytes(val));
    }

    public void SetAtUnicode(uint offset, string str) {
      lock (buffer) {
        ushort len = (ushort)str.Length;
        SetAt(offset, BitConverter.GetBytes(len));
        if (str.Length == 0) {
          return;
        }
        SetAt(offset + 2, Encoding.Unicode.GetBytes(str));
      }
    }

    public void SetAtLargeUnicode(uint offset, string str) {
      lock (buffer) {
        SetAt(offset, BitConverter.GetBytes(str.Length));
        if (str.Length == 0) {
          return;
        }
        SetAt(offset + 4, Encoding.Unicode.GetBytes(str));
      }
    }

    public void SetAtASCII(uint offset, string str) {
      lock (buffer) {
        ushort len = (ushort)str.Length;
        SetAt(offset, BitConverter.GetBytes(len));
        if (str.Length == 0) {
          return;
        }
        SetAt(offset + 2, Encoding.Default.GetBytes(str));
      }
    }

    public void SetAtLargeASCII(uint offset, string str) {
      lock (buffer) {
        SetAt(offset, BitConverter.GetBytes(str.Length));
        if (str.Length == 0) {
          return;
        }
        SetAt(offset + 4, Encoding.Default.GetBytes(str));
      }
    }

    public void SetAtArray(uint offset, byte[] data, uint dataOffset, ushort aSize) {
      lock (buffer) {
        SetAt(offset, aSize);
        if (aSize == 0) {
          return;
        }
        SetAt(offset + 2, data, dataOffset, aSize);
      }
    }

    public void SetAtArray(uint offset, byte[] data) {
      SetAtArray(offset, data, 0, (ushort)data.Length);
    }

    public void SetAtArray(uint offset, byte[] data, uint dataOffset, uint aSize) {
      lock (buffer) {
        SetAt(offset, aSize);
        if (aSize == 0) {
          return;
        }
        SetAt(offset + 4, data, dataOffset, aSize);
      }
    }

    public bool SetAt(uint offset, BinaryReader ins) {
      try {
        uint i = 0;
        int c = 0;
        while (true) {
          Grow(offset + i + 1);
          c = ins.Read();
          if (c == -1) {
            break;
          }
          buffer[offset + i] = Convert.ToByte(c);// ins.ReadByte();
          ++i;
        }
        bufferSize = Math.Max(bufferSize, offset + i);
        return true;
      }
      catch (IOException ex) {
        return false;
      }
    }

    #endregion

  }
}
