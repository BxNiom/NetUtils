namespace BxNiom.Collections;

public class Bits {
    private long[] _bits = Array.Empty<long>();

    public Bits() { }

    public Bits(int capacity) {
        CheckCapacity(capacity >> 6);
    }

    public Bits(Bits other) {
        _bits = new long[other._bits.Length];
        Array.Copy(other._bits, _bits, _bits.Length);
    }

    public int Count => _bits.Length << 6;

    public int LogicLength {
        get {
            var bits = _bits;
            for (var word = bits.Length - 1; word >= 0; --word) {
                var bitsAtWord = bits[word];
                if (bitsAtWord != 0) {
                    for (var bit = 63; bit >= 0; --bit) {
                        if ((bitsAtWord & (1L << (bit & 0x3F))) != 0L) {
                            return (word << 6) + bit + 1;
                        }
                    }
                }
            }

            return 0;
        }
    }

    public bool this[int index] {
        get {
            var word = index >> 6;
            if (word >= _bits.Length) {
                return false;
            }

            return (_bits[word] & (1L << (index & 0x3F))) != 0L;
        }
    }

    public bool IsEmpty {
        get {
            var bits   = _bits;
            var length = bits.Length;
            for (var i = 0; i < length; i++) {
                if (_bits[i] != 0L) {
                    return false;
                }
            }

            return true;
        }
    }

    public static IEqualityComparer<Bits> BitsComparer { get; } = new BitsEqualityComparer();

    public bool GetAndSet(int index) {
        var word = index >> 6;
        CheckCapacity(word);
        var oldBits = _bits[word];
        _bits[word] |= 1L << (index & 0x3F);
        return _bits[word] == oldBits;
    }

    public bool GetAndClear(int index) {
        var word = index >> 6;
        if (word >= _bits.Length) {
            return false;
        }

        var oldBits = _bits[word];
        _bits[word] &= ~(1L << (index & 0x3F));
        return _bits[word] != oldBits;
    }

    public void Set(int index) {
        var word = index >> 6;
        CheckCapacity(word);
        _bits[word] |= 1L << (index & 0x3F);
    }

    private void CheckCapacity(int len) {
        if (len >= _bits.Length) {
            var newBits = new long[len + 1];
            Array.Copy(_bits, newBits, _bits.Length);
            _bits = newBits;
        }
    }

    public void Flip(int index) {
        var word = index >> 6;
        CheckCapacity(word);
        _bits[word] ^= 1L << (index & 0x3F);
    }

    public void Clear(int index) {
        var word = index >> 6;
        if (word >= _bits.Length) {
            return;
        }

        _bits[word] &= ~(1L << (index & 0x3F));
    }

    public void Clear() {
        Array.Fill(_bits, 0);
    }

    public int NextSetBit(int fromIndex) {
        var bits       = _bits;
        var word       = fromIndex >> 6;
        var bitsLength = bits.Length;
        if (word >= bitsLength) {
            return -1;
        }

        var bitsAtWord = bits[word];
        if (bitsAtWord != 0) {
            for (var i = fromIndex & 0x3f; i < 64; i++) {
                if ((bitsAtWord & (1L << (i & 0x3F))) != 0L) {
                    return (word << 6) + i;
                }
            }
        }

        for (word++; word < bitsLength; word++) {
            if (word != 0) {
                bitsAtWord = bits[word];
                if (bitsAtWord != 0) {
                    for (var i = 0; i < 64; i++) {
                        if ((bitsAtWord & (1L << (i & 0x3F))) != 0L) {
                            return (word << 6) + i;
                        }
                    }
                }
            }
        }

        return -1;
    }

    public int NextClearBit(int fromIndex) {
        var bits       = _bits;
        var word       = fromIndex >> 6;
        var bitsLength = bits.Length;
        if (word >= bitsLength) {
            return bits.Length << 6;
        }

        var bitsAtWord = bits[word];
        for (var i = fromIndex & 0x3f; i < 64; i++) {
            if ((bitsAtWord & (1L << (i & 0x3F))) == 0L) {
                return (word << 6) + i;
            }
        }

        for (word++; word < bitsLength; word++) {
            if (word == 0) {
                return word << 6;
            }

            bitsAtWord = bits[word];
            for (var i = 0; i < 64; i++) {
                if ((bitsAtWord & (1L << (i & 0x3F))) == 0L) {
                    return (word << 6) + i;
                }
            }
        }

        return bits.Length << 6;
    }

    public void And(Bits other) {
        var commonWords = System.Math.Min(_bits.Length, other._bits.Length);
        for (var i = 0; commonWords > i; i++) {
            _bits[i] &= other._bits[i];
        }

        if (_bits.Length > commonWords) {
            for (int i = commonWords, s = _bits.Length; s > i; i++) {
                _bits[i] = 0L;
            }
        }
    }

    public void AndNot(Bits other) {
        for (int i = 0, j = _bits.Length, k = other._bits.Length; i < j && i < k; i++) {
            _bits[i] &= ~other._bits[i];
        }
    }

    public void Or(Bits other) {
        var commonWords = System.Math.Min(_bits.Length, other._bits.Length);
        for (var i = 0; commonWords > i; i++) {
            _bits[i] |= other._bits[i];
        }

        if (commonWords < other._bits.Length) {
            CheckCapacity(other._bits.Length);
            for (int i = commonWords, s = other._bits.Length; s > i; i++) {
                _bits[i] = other._bits[i];
            }
        }
    }

    public void XOr(Bits other) {
        var commonWords = System.Math.Min(_bits.Length, other._bits.Length);

        for (var i = 0; commonWords > i; i++) {
            _bits[i] ^= other._bits[i];
        }

        if (commonWords < other._bits.Length) {
            CheckCapacity(other._bits.Length);
            for (int i = commonWords, s = other._bits.Length; s > i; i++) {
                _bits[i] = other._bits[i];
            }
        }
    }

    public bool Intersects(Bits other) {
        var bits      = _bits;
        var otherBits = other._bits;
        for (var i = System.Math.Min(_bits.Length, otherBits.Length) - 1; i >= 0; i--) {
            if ((_bits[i] & otherBits[i]) != 0) {
                return true;
            }
        }

        return false;
    }

    public bool ContainsAll(Bits other) {
        var bits            = _bits;
        var otherBits       = other._bits;
        var otherBitsLength = otherBits.Length;
        var bitsLength      = bits.Length;

        for (var i = bitsLength; i < otherBitsLength; i++) {
            if (otherBits[i] != 0) {
                return false;
            }
        }

        for (var i = System.Math.Min(bitsLength, otherBitsLength) - 1; i >= 0; i--) {
            if ((_bits[i] & otherBits[i]) != otherBits[i]) {
                return false;
            }
        }

        return true;
    }

    protected bool Equals(Bits other) {
        return Equals(_bits, other._bits);
    }

    public override bool Equals(object? obj) {
        if (ReferenceEquals(null, obj)) {
            return false;
        }

        if (ReferenceEquals(this, obj)) {
            return true;
        }

        if (obj.GetType() != GetType()) {
            return false;
        }

        return Equals((Bits)obj);
    }

    public override int GetHashCode() {
        return _bits != null ? _bits.GetHashCode() : 0;
    }

    public static bool operator ==(Bits a, Bits b) {
        return a.Equals(b);
    }

    public static bool operator !=(Bits a, Bits b) {
        return !a.Equals(b);
    }

    public static Bits operator +(Bits a, int bit) {
        a.Set(bit);
        return a;
    }

    public static Bits operator -(Bits a, int bit) {
        a.Clear(bit);
        return a;
    }

    private sealed class BitsEqualityComparer : IEqualityComparer<Bits> {
        public bool Equals(Bits? x, Bits? y) {
            if (ReferenceEquals(x, y)) {
                return true;
            }

            if (ReferenceEquals(x, null)) {
                return false;
            }

            if (ReferenceEquals(y, null)) {
                return false;
            }

            if (x.GetType() != y.GetType()) {
                return false;
            }

            return Equals(x._bits, y._bits);
        }

        public int GetHashCode(Bits obj) {
            return obj._bits != null ? obj._bits.GetHashCode() : 0;
        }
    }
}