namespace BxNiom.Math;

public static partial class MathUtils {
    public const float DegToRad = MathF.PI / 180.0f;
    public const float RadToDeg = 180.0f / MathF.PI;

    public static FastRandom Random { get; } = new();

    public static float Lerp(float b, float e, float t) {
        return b + t * (e - b);
    }

    public static Vector2 Bezier(IEnumerable<Vector2> points, float t, Func<float, float>? easeFunc = null) {
        easeFunc ??= Easing.Linear;
        var et = easeFunc(t);

        var curPoints  = new List<Vector2>(points);
        var nextPoints = new List<Vector2>();

        while (curPoints.Count > 2) {
            for (var i = 1; i < curPoints.Count; i++) {
                nextPoints.Add(Vector2.Lerp(curPoints[i - 1], curPoints[i], et));
            }

            curPoints.Clear();
            curPoints.AddRange(nextPoints);
            nextPoints.Clear();
        }

        return curPoints.Count == 2 ? Vector2.Lerp(curPoints[0], curPoints[1], et) : Vector2.Zero;
    }
}