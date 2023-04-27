namespace BxNiom.Math;

/// <summary>
/// A fast random number generator for .NET
/// Colin Green, January 2005
/// 
/// September 4th 2005
///	 Added NextBytesUnsafe() - commented out by default.
///	 Fixed bug in Reinitialise() - y,z and w variables were not being reset.
/// 
/// Key points:
///  1) Based on a simple and fast xor-shift pseudo random number generator (RNG) specified in: 
///  Marsaglia, George. (2003). Xorshift RNGs.
///  http://www.jstatsoft.org/v08/i14/xorshift.pdf
///  
///  This particular implementation of xorshift has a period of 2^128-1. See the above paper to see
///  how this can be easily extened if you need a longer period. At the time of writing I could find no 
///  information on the period of System.Random for comparison.
/// 
///  2) Faster than System.Random. Up to 8x faster, depending on which methods are called.
/// 
///  3) Direct replacement for System.Random. This class implements all of the methods that System.Random 
///  does plus some additional methods. The like named methods are functionally equivalent.
///  
///  4) Allows fast re-initialisation with a seed, unlike System.Random which accepts a seed at construction
///  time which then executes a relatively expensive initialisation routine. This provides a vast speed improvement
///  if you need to reset the pseudo-random number sequence many times, e.g. if you want to re-generate the same
///  sequence many times. An alternative might be to cache random numbers in an array, but that approach is limited
///  by memory capacity and the fact that you may also want a large number of different sequences cached. Each sequence
///  can each be represented by a single seed value (int) when using FastRandom.
///  
///  Notes.
///  A further performance improvement can be obtained by declaring local variables as static, thus avoiding 
///  re-allocation of variables on each call. However care should be taken if multiple instances of
///  FastRandom are in use or if being used in a multi-threaded environment.
/// 
/// </summary>
public class FastRandom {
    // The +1 ensures NextDouble doesn't generate 1.0
    private const double REAL_UNIT_INT  = 1.0 / ((double)int.MaxValue + 1.0);
    private const double REAL_UNIT_UINT = 1.0 / ((double)uint.MaxValue + 1.0);
    private const uint   Y              = 842502087, Z = 3579807591, W = 273326509;

    private uint x, y, z, w;

    #region Public Methods [Reinitialisation]

    /// <summary>
    /// Reinitialises using an int value as a seed.
    /// </summary>
    /// <param name="seed"></param>
    public void Reinitialise(int seed) {
        // The only stipulation stated for the xorshift RNG is that at least one of
        // the seeds x,y,z,w is non-zero. We fulfill that requirement by only allowing
        // resetting of the x seed
        x = (uint)seed;
        y = Y;
        z = Z;
        w = W;
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Initialises a new instance using time dependent seed.
    /// </summary>
    public FastRandom() {
        // Initialise using the system tick count.
        Reinitialise((int)Environment.TickCount);
    }

    /// <summary>
    /// Initialises a new instance using an int value as seed.
    /// This constructor signature is provided to maintain compatibility with
    /// System.Random
    /// </summary>
    public FastRandom(int seed) {
        Reinitialise(seed);
    }

    #endregion

    #region Public Methods [System.Random functionally equivalent methods]

    /// <summary>
    /// Generates a random int over the range 0 to int.MaxValue-1.
    /// MaxValue is not generated in order to remain functionally equivalent to System.Random.Next().
    /// This does slightly eat into some of the performance gain over System.Random, but not much.
    /// For better performance see:
    /// 
    /// Call NextInt() for an int over the range 0 to int.MaxValue.
    /// 
    /// Call NextUInt() and cast the result to an int to generate an int over the full Int32 value range
    /// including negative values. 
    /// </summary>
    /// <returns></returns>
    public int Next() {
        var t = x ^ (x << 11);
        x = y;
        y = z;
        z = w;
        w = w ^ (w >> 19) ^ t ^ (t >> 8);

        // Handle the special case where the value int.MaxValue is generated. This is outside of 
        // the range of permitted values, so we therefore call Next() to try again.
        var rtn = w & 0x7FFFFFFF;
        if (rtn == 0x7FFFFFFF) {
            return Next();
        }

        return (int)rtn;
    }

    /// <summary>
    /// Generates a random int over the range 0 to upperBound-1, and not including upperBound.
    /// </summary>
    /// <param name="upperBound"></param>
    /// <returns></returns>
    public int Next(int upperBound) {
        if (upperBound < 0) {
            throw new ArgumentOutOfRangeException("upperBound", upperBound, "upperBound must be >=0");
        }

        var t = x ^ (x << 11);
        x = y;
        y = z;
        z = w;

        // The explicit int cast before the first multiplication gives better performance.
        // See comments in NextDouble.
        return (int)(REAL_UNIT_INT * (int)(0x7FFFFFFF & (w = w ^ (w >> 19) ^ t ^ (t >> 8))) * upperBound);
    }

    /// <summary>
    /// Generates a random int over the range lowerBound to upperBound-1, and not including upperBound.
    /// upperBound must be >= lowerBound. lowerBound may be negative.
    /// </summary>
    /// <param name="lowerBound"></param>
    /// <param name="upperBound"></param>
    /// <returns></returns>
    public int Next(int lowerBound, int upperBound) {
        if (lowerBound > upperBound) {
            throw new ArgumentOutOfRangeException("upperBound", upperBound, "upperBound must be >=lowerBound");
        }

        var t = x ^ (x << 11);
        x = y;
        y = z;
        z = w;

        // The explicit int cast before the first multiplication gives better performance.
        // See comments in NextDouble.
        var range = upperBound - lowerBound;
        if (range < 0) {
            // If range is <0 then an overflow has occured and must resort to using long integer arithmetic instead (slower).
            // We also must use all 32 bits of precision, instead of the normal 31, which again is slower.	
            return lowerBound + (int)(REAL_UNIT_UINT * (double)(w = w ^ (w >> 19) ^ t ^ (t >> 8)) *
                                      (double)((long)upperBound - (long)lowerBound));
        }

        // 31 bits of precision will suffice if range<=int.MaxValue. This allows us to cast to an int and gain
        // a little more performance.
        return lowerBound + (int)(REAL_UNIT_INT * (double)(int)(0x7FFFFFFF & (w = w ^ (w >> 19) ^ t ^ (t >> 8))) *
                                  (double)range);
    }

    /// <summary>
    /// Generates a random double. Values returned are from 0.0 up to but not including 1.0.
    /// </summary>
    /// <returns></returns>
    public double NextDouble() {
        var t = x ^ (x << 11);
        x = y;
        y = z;
        z = w;

        // Here we can gain a 2x speed improvement by generating a value that can be cast to 
        // an int instead of the more easily available uint. If we then explicitly cast to an 
        // int the compiler will then cast the int to a double to perform the multiplication, 
        // this final cast is a lot faster than casting from a uint to a double. The extra cast
        // to an int is very fast (the allocated bits remain the same) and so the overall effect 
        // of the extra cast is a significant performance improvement.
        //
        // Also note that the loss of one bit of precision is equivalent to what occurs within 
        // System.Random.
        return REAL_UNIT_INT * (int)(0x7FFFFFFF & (w = w ^ (w >> 19) ^ t ^ (t >> 8)));
    }


    public byte[] NextBytes(int count) {
        var data = new byte[count];
        NextBytes(data);
        return data;
    }

    /// <summary>
    /// Fills the provided byte array with random bytes.
    /// This method is functionally equivalent to System.Random.NextBytes(). 
    /// </summary>
    /// <param name="buffer"></param>
    public void NextBytes(byte[] buffer) {
        // Fill up the bulk of the buffer in chunks of 4 bytes at a time.
        uint x = this.x, y = this.y, z = this.z, w = this.w;
        var  i = 0;
        uint t;
        for (var bound = buffer.Length - 3; i < bound;) {
            // Generate 4 bytes. 
            // Increased performance is achieved by generating 4 random bytes per loop.
            // Also note that no mask needs to be applied to zero out the higher order bytes before
            // casting because the cast ignores thos bytes. Thanks to Stefan Trosch�tz for pointing this out.
            t = x ^ (x << 11);
            x = y;
            y = z;
            z = w;
            w = w ^ (w >> 19) ^ t ^ (t >> 8);

            buffer[i++] = (byte)w;
            buffer[i++] = (byte)(w >> 8);
            buffer[i++] = (byte)(w >> 16);
            buffer[i++] = (byte)(w >> 24);
        }

        // Fill up any remaining bytes in the buffer.
        if (i < buffer.Length) {
            // Generate 4 bytes.
            t = x ^ (x << 11);
            x = y;
            y = z;
            z = w;
            w = w ^ (w >> 19) ^ t ^ (t >> 8);

            buffer[i++] = (byte)w;
            if (i < buffer.Length) {
                buffer[i++] = (byte)(w >> 8);
                if (i < buffer.Length) {
                    buffer[i++] = (byte)(w >> 16);
                    if (i < buffer.Length) {
                        buffer[i] = (byte)(w >> 24);
                    }
                }
            }
        }

        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
    }

    #endregion

    #region Public Methods [Methods not present on System.Random]

    /// <summary>
    /// Generates a uint. Values returned are over the full range of a uint, 
    /// uint.MinValue to uint.MaxValue, inclusive.
    /// 
    /// This is the fastest method for generating a single random number because the underlying
    /// random number generator algorithm generates 32 random bits that can be cast directly to 
    /// a uint.
    /// </summary>
    /// <returns></returns>
    public uint NextUInt() {
        var t = x ^ (x << 11);
        x = y;
        y = z;
        z = w;
        return w = w ^ (w >> 19) ^ t ^ (t >> 8);
    }

    /// <summary>
    /// Generates a random int over the range 0 to int.MaxValue, inclusive. 
    /// This method differs from Next() only in that the range is 0 to int.MaxValue
    /// and not 0 to int.MaxValue-1.
    /// 
    /// The slight difference in range means this method is slightly faster than Next()
    /// but is not functionally equivalent to System.Random.Next().
    /// </summary>
    /// <returns></returns>
    public int NextInt() {
        var t = x ^ (x << 11);
        x = y;
        y = z;
        z = w;
        return (int)(0x7FFFFFFF & (w = w ^ (w >> 19) ^ t ^ (t >> 8)));
    }


    // Buffer 32 bits in bitBuffer, return 1 at a time, keep track of how many have been returned
    // with bitBufferIdx.
    private uint bitBuffer;
    private uint bitMask = 1;

    /// <summary>
    /// Generates a single random bit.
    /// This method's performance is improved by generating 32 bits in one operation and storing them
    /// ready for future calls.
    /// </summary>
    /// <returns></returns>
    public bool NextBool() {
        if (bitMask == 1) {
            // Generate 32 more bits.
            var t = x ^ (x << 11);
            x         = y;
            y         = z;
            z         = w;
            bitBuffer = w = w ^ (w >> 19) ^ t ^ (t >> 8);

            // Reset the bitMask that tells us which bit to read next.
            bitMask = 0x80000000;
            return (bitBuffer & bitMask) == 0;
        }

        return (bitBuffer & (bitMask >>= 1)) == 0;
    }

    #endregion
}