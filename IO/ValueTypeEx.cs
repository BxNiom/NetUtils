using BxNiom.Math;

namespace BxNiom.IO;

public static class ValueTypeEx {
    public static byte[] ToBytes(this byte value) {
        return new byte[] { value };
    }

    public static byte[] ToBytes(this char value) {
        return BitConverter.GetBytes(value);
    }

    public static byte[] ToBytes(this short value) {
        return BitConverter.GetBytes(value);
    }

    public static byte[] ToBytes(this ushort value) {
        return BitConverter.GetBytes(value);
    }

    public static byte[] ToBytes(this int value) {
        return BitConverter.GetBytes(value);
    }

    public static byte[] ToBytes(this uint value) {
        return BitConverter.GetBytes(value);
    }

    public static byte[] ToBytes(this long value) {
        return BitConverter.GetBytes(value);
    }

    public static byte[] ToBytes(this ulong value) {
        return BitConverter.GetBytes(value);
    }

    public static byte[] ToBytes(this float value) {
        return BitConverter.GetBytes(value);
    }

    public static byte[] ToBytes(this double value) {
        return BitConverter.GetBytes(value);
    }

    public static byte[] ToBytes(this bool value) {
        return BitConverter.GetBytes(value);
    }

    public static byte[] ToBytes(this Half value) {
        return BitConverter.GetBytes(value);
    }

    public static char GetChar(this byte[] array, int offset = 0) {
        return BitConverter.ToChar(array, offset);
    }

    public static short GetInt16(this byte[] array, int offset = 0) {
        return BitConverter.ToInt16(array, offset);
    }

    public static ushort GetUInt16(this byte[] array, int offset = 0) {
        return BitConverter.ToUInt16(array, offset);
    }

    public static int GetInt32(this byte[] array, int offset = 0) {
        return BitConverter.ToInt32(array, offset);
    }

    public static uint GetUInt32(this byte[] array, int offset = 0) {
        return BitConverter.ToUInt32(array, offset);
    }

    public static long GetInt64(this byte[] array, int offset = 0) {
        return BitConverter.ToInt64(array, offset);
    }

    public static ulong GetUInt64(this byte[] array, int offset = 0) {
        return BitConverter.ToUInt64(array, offset);
    }

    public static float GetSingle(this byte[] array, int offset = 0) {
        return BitConverter.ToSingle(array, offset);
    }

    public static double GetDouble(this byte[] array, int offset = 0) {
        return BitConverter.ToDouble(array, offset);
    }

    public static bool GetBool(this byte[] array, int offset = 0) {
        return BitConverter.ToBoolean(array, offset);
    }

    public static Half GetHalf(this byte[] array, int offset = 0) {
        return BitConverter.ToHalf(array, offset);
    }

    public static byte Limit(this byte v, byte max) {
        return MathUtils.Limit(v, max);
    }

    public static short Limit(this short v, short max) {
        return MathUtils.Limit(v, max);
    }

    public static int Limit(this int v, int max) {
        return MathUtils.Limit(v, max);
    }

    public static long Limit(this long v, long max) {
        return MathUtils.Limit(v, max);
    }

    public static ushort Limit(this ushort v, ushort max) {
        return MathUtils.Limit(v, max);
    }

    public static uint Limit(this uint v, uint max) {
        return MathUtils.Limit(v, max);
    }

    public static ulong Limit(this ulong v, ulong max) {
        return MathUtils.Limit(v, max);
    }

    public static float Limit(this float v, float max) {
        return MathUtils.Limit(v, max);
    }

    public static double Limit(this double v, double max) {
        return MathUtils.Limit(v, max);
    }

    public static int Clamp(this int d, int min, int max) {
        return d < min ? min : d > max ? max : d;
    }

    public static long Clamp(this long d, long min, long max) {
        return d < min ? min : d > max ? max : d;
    }

    public static float Clamp(this float d, float min, float max) {
        return d < min ? min : d > max ? max : d;
    }

    public static uint Clamp(this uint d, uint min, uint max) {
        return d < min ? min : d > max ? max : d;
    }

    public static ulong Clamp(this ulong d, ulong min, ulong max) {
        return d < min ? min : d > max ? max : d;
    }
}