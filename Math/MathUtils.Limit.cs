namespace BxNiom.Math;

public static partial class MathUtils {
    public static byte Limit(byte val, byte max) {
        if (val > max) {
            return (byte)(val - max);
        }

        return val;
    }


    public static short Limit(short val, short max) {
        if (val < 0) {
            return (short)(max - System.Math.Abs(val));
        }

        if (val > max) {
            return (short)(val - max);
        }

        return val;
    }


    public static int Limit(int val, int max) {
        if (val < 0) {
            return max - System.Math.Abs(val);
        }

        if (val > max) {
            return val - max;
        }

        return val;
    }


    public static long Limit(long val, long max) {
        if (val < 0) {
            return max - System.Math.Abs(val);
        }

        if (val > max) {
            return val - max;
        }

        return val;
    }


    public static ushort Limit(ushort val, ushort max) {
        if (val > max) {
            return (ushort)(val - max);
        }

        return val;
    }


    public static uint Limit(uint val, uint max) {
        return val > max ? val - max : val;
    }


    public static ulong Limit(ulong val, ulong max) {
        if (val > max) {
            return val - max;
        }

        return val;
    }


    public static float Limit(float val, float max) {
        if (val < 0) {
            return max - System.Math.Abs(val);
        }

        if (val > max) {
            return val - max;
        }

        return val;
    }


    public static double Limit(double val, double max) {
        if (val < 0) {
            return max - System.Math.Abs(val);
        }

        if (val > max) {
            return val - max;
        }

        return val;
    }
}