using System.Collections;

namespace BxNiom.IO;

public class ByteBuffer : IEnumerable<byte> {
    private byte[] _data = Array.Empty<byte>();

    public ByteBuffer(IEnumerable<byte> data) {
        var inData = data.ToArray();
        _data = new byte[inData.Length];
        Buffer.BlockCopy(inData, 0, _data, 0, inData.Length);

        IsFixedCapacity = true;
        Capacity        = _data.Length;
    }

    public ByteBuffer(int capacity = 0) {
        IsFixedCapacity = capacity != 0;
        Capacity        = capacity == 0 ? 8 : capacity;
        Clear();
    }

    public int  Capacity        { get; private set; }
    public int  Position        { get; private set; }
    public int  Length          { get; private set; }
    public bool IsFixedCapacity { get; }

    public byte this[int index] {
        get {
            if (index >= Length) {
                throw new IndexOutOfRangeException();
            }

            return _data[index];
        }
    }

    public IEnumerator<byte> GetEnumerator() {
        return _data.AsEnumerable().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return _data.AsEnumerable().GetEnumerator();
    }

    public void Clear() {
        Position = 0;
        Capacity = IsFixedCapacity ? Capacity : 8;
        _data    = new byte[Capacity];
        Array.Fill(_data, (byte)0x00);
    }

    public void Seek(int offset, SeekOrigin origin) {
        var newPos = origin switch {
            SeekOrigin.Begin   => offset,
            SeekOrigin.Current => Position + offset,
            _                  => Length + offset
        };

        if (newPos >= Length) {
            throw new IndexOutOfRangeException();
        }

        Position = newPos;
    }

    private void EnsureSize(int req) {
        if (Position + req >= Capacity) {
            if (IsFixedCapacity) {
                throw new IndexOutOfRangeException();
            }

            var newCapacity = Capacity;
            while (newCapacity < Position + req) {
                newCapacity *= 2;
            }

            var newArray = new byte[newCapacity];
            Buffer.BlockCopy(_data, 0, newArray, 0, Capacity);
            _data    = newArray;
            Capacity = newCapacity;
        }
    }

    private ByteBuffer Write<T>(T v, Func<T, byte[]> converter) {
        var bytes = converter(v);
        EnsureSize(bytes.Length);
        Array.Copy(bytes, 0, _data, Position, bytes.Length);
        Position += bytes.Length;
        Length   =  System.Math.Max(Position, Length);

        return this;
    }

    public ByteBuffer Write(byte v) {
        return Write(v, b => new byte[1] { b });
    }

    public ByteBuffer Write(short v) {
        return Write(v, BitConverter.GetBytes);
    }

    public ByteBuffer Write(int v) {
        return Write(v, BitConverter.GetBytes);
    }

    public ByteBuffer Write(long v) {
        return Write(v, BitConverter.GetBytes);
    }

    public ByteBuffer Write(float v) {
        return Write(v, BitConverter.GetBytes);
    }

    public ByteBuffer Write(double v) {
        return Write(v, BitConverter.GetBytes);
    }

    public ByteBuffer Write(ushort v) {
        return Write(v, BitConverter.GetBytes);
    }

    public ByteBuffer Write(uint v) {
        return Write(v, BitConverter.GetBytes);
    }

    public ByteBuffer Write(ulong v) {
        return Write(v, BitConverter.GetBytes);
    }

    public ByteBuffer Write(char v) {
        return Write(v, BitConverter.GetBytes);
    }

    public ByteBuffer Write(bool v) {
        return Write(v, BitConverter.GetBytes);
    }

    public ByteBuffer Write(byte[] v) {
        return Write(v, 0, v.Length);
    }

    public ByteBuffer Write(Span<byte> v) {
        return Write(v.ToArray(), 0, v.Length);
    }

    public ByteBuffer Write(byte[] v, int offset, int len) {
        EnsureSize(len);
        Array.Copy(v, offset, _data, Position, len);
        Position += len;
        Length   =  System.Math.Max(Position, Length);

        return this;
    }

    private byte[] ReadBytes(int length) {
        EnsureSize(length);
        var result = new byte[length];
        Array.Copy(_data, Position, result, 0, length);
        Position += length;
        return result;
    }

    public byte ReadByte() {
        return ReadBytes(1)[0];
    }

    public short ReadInt16() {
        return BitConverter.ToInt16(ReadBytes(sizeof(short)), 0);
    }

    public int ReadInt32() {
        return BitConverter.ToInt32(ReadBytes(sizeof(int)), 0);
    }

    public long ReadInt64() {
        return BitConverter.ToInt64(ReadBytes(sizeof(long)), 0);
    }

    public ushort ReadUInt16() {
        return BitConverter.ToUInt16(ReadBytes(sizeof(ushort)), 0);
    }

    public uint ReadUInt32() {
        return BitConverter.ToUInt32(ReadBytes(sizeof(uint)), 0);
    }

    public ulong ReadUInt64() {
        return BitConverter.ToUInt64(ReadBytes(sizeof(ulong)), 0);
    }

    public float ReadSingle() {
        return BitConverter.ToSingle(ReadBytes(sizeof(float)), 0);
    }

    public double ReadDouble() {
        return BitConverter.ToDouble(ReadBytes(sizeof(double)), 0);
    }

    public char ReadChar() {
        return BitConverter.ToChar(ReadBytes(sizeof(char)), 0);
    }

    public bool ReadBool() {
        return BitConverter.ToBoolean(ReadBytes(sizeof(bool)), 0);
    }

    public byte[] ToArray() {
        var res = new byte[Length];
        Buffer.BlockCopy(_data, 0, res, 0, Length);
        return res;
    }

    public static implicit operator ByteBuffer(byte[] arr) {
        return new ByteBuffer(arr);
    }
}