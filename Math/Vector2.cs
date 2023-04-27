namespace BxNiom.Math;

public readonly struct Vector2 {
    public static Vector2 One   { get; } = new(1);
    public static Vector2 Zero  { get; } = new(0);
    public static Vector2 Up    { get; } = new(0, -1);
    public static Vector2 Down  { get; } = new(0, 1);
    public static Vector2 Left  { get; } = new(-1, 0);
    public static Vector2 Right { get; } = new(1, 0);

    private const float DegreeToRadians = MathF.PI / 180.0f;
    private const float RadiansToDegree = 180.0f / MathF.PI;

    public float X { get; }
    public float Y { get; }

    public float Angle {
        get {
            var a = MathF.Atan2(Y, X) * MathUtils.RadToDeg;
            return a < 0 ? a + 360 : a;
        }
    }

    public float Length  => MathF.Sqrt(X * X + Y * Y);
    public float Length2 => X * X + Y * Y;

    internal int XInt => (int)MathF.Round(X);
    internal int YInt => (int)MathF.Round(Y);

    public Vector2(float v) : this(v, v) { }

    public Vector2(float x, float y) {
        X = x;
        Y = y;
    }

    internal (int x, int y) ToInt() {
        return (XInt, YInt);
    }

    public float Distance(Vector2 v) {
        var xD = v.X - X;
        var yD = v.Y - Y;
        return MathF.Sqrt(xD * xD + yD * yD);
    }

    public Vector2 Normalize() {
        var len = Length;
        return len != 0 ? new Vector2(X / len, Y / len) : this;
    }

    public Vector2 Clamp(float min, float max) {
        var len2 = Length2;
        if (len2 == 0) {
            return this;
        }

        var max2 = max * max;
        if (len2 > max2) {
            return this * MathF.Sqrt(max2 / len2);
        }

        var min2 = min * min;
        if (len2 < min2) {
            return this * MathF.Sqrt(min2 / len2);
        }

        return this;
    }

    public Vector2 RotateAround(Vector2 around, float degrees) {
        var cos = MathF.Cos(degrees * DegreeToRadians);
        var sin = MathF.Sin(degrees * DegreeToRadians);

        var _x = X - around.X;
        var _y = Y - around.Y;

        var nX = _x * cos - _y * sin;
        var nY = _x * sin + _y * cos;

        return new Vector2(nX + around.X, nY + around.Y);
    }

    public float Cross(Vector2 other) {
        return Cross(other.X, other.Y);
    }

    public float Cross(float x, float y) {
        return X * y - Y * x;
    }

    public float Dot(Vector2 other) {
        return Dot(other.X, other.Y);
    }

    public float Dot(float x, float y) {
        return X * x + Y * y;
    }

    public float AngleTo(Vector2 other) {
        return AngleTo(other.X, other.Y);
    }

    public float AngleTo(float x, float y) {
        return MathF.Atan2(Cross(x, y), Dot(x, y) * RadiansToDegree);
    }

    public Vector2 SetLength(float length) {
        length *= length;
        var oldLen = Length2;
        return oldLen == 0 || oldLen == length ? this : this * MathF.Sqrt(length / oldLen);
    }

    public Vector2 SetAngle(float degrees) {
        return SetAngleRadians(degrees * DegreeToRadians);
    }

    public Vector2 SetAngleRadians(float radians) {
        return new Vector2(Length, 0f).RotateRadians(radians);
    }

    public Vector2 Rotate(float degrees) {
        return RotateRadians(degrees * DegreeToRadians);
    }

    public Vector2 RotateRadians(float radians) {
        var cos = MathF.Cos(radians);
        var sin = MathF.Sin(radians);
        return new Vector2(X * cos - Y * sin, X * sin + Y - cos);
    }

    public static Vector2 Clamp(Vector2 value, Vector2 min, Vector2 max) {
        return new Vector2(
            value.X < min.X ? min.X : value.X > max.X ? max.X : value.X,
            value.Y < min.Y ? min.Y : value.Y > max.Y ? max.Y : value.Y
        );
    }

    public override string ToString() {
        return $"{nameof(X)}: {X}, {nameof(Y)}: {Y}";
    }

    public static Vector2 operator +(Vector2 a, Vector2 b) {
        return new Vector2(a.X + b.X, a.Y + b.Y);
    }

    public static Vector2 operator -(Vector2 a, Vector2 b) {
        return new Vector2(a.X - b.X, a.Y - b.Y);
    }

    public static Vector2 operator *(Vector2 a, Vector2 b) {
        return new Vector2(a.X * b.X, a.Y * b.Y);
    }

    public static Vector2 operator /(Vector2 a, Vector2 b) {
        return new Vector2(a.X / b.X, a.Y / b.Y);
    }

    public static Vector2 operator +(Vector2 a, float b) {
        return new Vector2(a.X + b, a.Y + b);
    }

    public static Vector2 operator -(Vector2 a, float b) {
        return new Vector2(a.X - b, a.Y - b);
    }

    public static Vector2 operator *(Vector2 a, float b) {
        return new Vector2(a.X * b, a.Y * b);
    }

    public static Vector2 operator /(Vector2 a, float b) {
        return new Vector2(a.X / b, a.Y / b);
    }

    public static Vector2 Lerp(Vector2 start, Vector2 end, float t) {
        return new Vector2(MathUtils.Lerp(start.X, end.X, t), MathUtils.Lerp(start.Y, end.Y, t));
    }
}