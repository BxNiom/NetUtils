namespace BxNiom.Math;

public class Easing {
    private const float PI = MathF.PI;
    private const float c1 = 1.70158f;
    private const float c2 = c1 * 1.525f;
    private const float c3 = c1 + 1f;
    private const float c4 = 2f * PI / 3f;
    private const float c5 = 2f * PI / 4.5f;

    public static float Linear(float x) {
        return x;
    }

    public static float EaseInQuad(float x) {
        return x * x;
    }

    public static float EaseOutQuad(float x) {
        return 1.0f - (1.0f - x) * (1.0f - x);
    }

    public static float EaseInOutQuad(float x) {
        return x < 0.5f ? 2.0f * x * x : 1.0f - MathF.Pow(-2 * x + 2.0f, 2.0f) / 2.0f;
    }

    public static float EaseInCubic(float x) {
        return x * x * x;
    }

    public static float EaseOutCubic(float x) {
        return 1.0f - MathF.Pow(1.0f - x, 3.0f);
    }

    public static float EaseInOutCubic(float x) {
        return x < 0.5f ? 4.0f * x * x * x : 1.0f - MathF.Pow(-2 * x + 2.0f, 3.0f) / 2.0f;
    }

    public static float EaseInQuart(float x) {
        return x * x * x * x;
    }

    public static float EaseOutQuart(float x) {
        return 1.0f - MathF.Pow(1.0f - x, 4.0f);
    }

    public static float EaseInOutQuart(float x) {
        return x < 0.5f ? 8.0f * x * x * x * x : 1.0f - MathF.Pow(-2 * x + 2.0f, 4.0f) / 2.0f;
    }

    public static float EaseInQuint(float x) {
        return x * x * x * x * x;
    }

    public static float EaseOutQuint(float x) {
        return 1.0f - MathF.Pow(1.0f - x, 5.0f);
    }

    public static float EaseInOutQuint(float x) {
        return x < 0.5f ? 16.0f * x * x * x * x * x : 1.0f - MathF.Pow(-2 * x + 2.0f, 5.0f) / 2.0f;
    }

    public static float EaseInSine(float x) {
        return 1.0f - MathF.Cos(x * PI / 2.0f);
    }

    public static float EaseOutSine(float x) {
        return MathF.Sin(x * PI / 2.0f);
    }

    public static float EaseInOutSine(float x) {
        return -(MathF.Cos(PI * x) - 1.0f) / 2.0f;
    }

    public static float EaseInExpo(float x) {
        return x == 0.0f ? 0.0f : MathF.Pow(2.0f, 10.0f * x - 10.0f);
    }

    public static float EaseOutExpo(float x) {
        return x == 1.0f ? 1.0f : 1.0f - MathF.Pow(2.0f, -10 * x);
    }

    public static float EaseInOutExpo(float x) {
        return x == 0
            ? 0
            : x == 1
                ? 1
                : x < 0.5
                    ? MathF.Pow(2.0f, 20.0f * x - 10.0f) / 2
                    : (2.0f - MathF.Pow(2.0f, -20 * x + 10.0f)) / 2.0f;
    }

    public static float EaseInCirc(float x) {
        return 1.0f - MathF.Sqrt(1.0f - MathF.Pow(x, 2.0f));
    }

    public static float EaseOutCirc(float x) {
        return MathF.Sqrt(1.0f - MathF.Pow(x - 1.0f, 2.0f));
    }

    public static float EaseInOutCirc(float x) {
        return x < 0.5
            ? (1.0f - MathF.Sqrt(1.0f - MathF.Pow(2.0f * x, 2.0f))) / 2
            : (MathF.Sqrt(1.0f - MathF.Pow(-2 * x + 2.0f, 2.0f)) + 1.0f) / 2.0f;
    }

    public static float EaseInBack(float x) {
        return c3 * x * x * x - c1 * x * x;
    }

    public static float EaseOutBack(float x) {
        return 1.0f + c3 * MathF.Pow(x - 1.0f, 3.0f) + c1 * MathF.Pow(x - 1.0f, 2.0f);
    }

    public static float EaseInOutBack(float x) {
        return x < 0.5
            ? MathF.Pow(2.0f * x, 2.0f) * ((c2 + 1.0f) * 2.0f * x - c2) / 2
            : (MathF.Pow(2.0f * x - 2.0f, 2.0f) * ((c2 + 1.0f) * (x * 2.0f - 2.0f) + c2) + 2.0f) / 2.0f;
    }

    public static float EaseInElastic(float x) {
        return x == 0
            ? 0
            : x == 1
                ? 1
                : -MathF.Pow(2.0f, 10.0f * x - 10.0f) * MathF.Sin((x * 10.0f - 10.75f) * c4);
    }

    public static float EaseOutElastic(float x) {
        return x == 0
            ? 0
            : x == 1
                ? 1
                : MathF.Pow(2.0f, -10 * x) * MathF.Sin((x * 10.0f - 0.75f) * c4) + 1.0f;
    }

    public static float EaseInOutElastic(float x) {
        return x == 0
            ? 0
            : x == 1
                ? 1
                : x < 0.5
                    ? -(MathF.Pow(2.0f, 20.0f * x - 10.0f) * MathF.Sin((20.0f * x - 11.125f) * c5)) / 2
                    : MathF.Pow(2.0f, -20 * x + 10.0f) * MathF.Sin((20.0f * x - 11.125f) * c5) / 2.0f + 1.0f;
    }

    public static float EaseInBounce(float x) {
        return 1.0f - BounceOut(1.0f - x);
    }


    public static float EaseInOutBounce(float x) {
        return x < 0.5
            ? (1.0f - BounceOut(1.0f - 2.0f * x)) / 2
            : (1.0f + BounceOut(2.0f * x - 1.0f)) / 2.0f;
    }

    private static float BounceOut(float x) {
        const float n1 = 7.5625f;
        const float d1 = 2.75f;

        if (x < 1 / d1) {
            return n1 * x * x;
        }

        if (x < 2 / d1) {
            return n1 * (x -= 1.5f / d1) * x + 0.75f;
        }

        if (x < 2.5 / d1) {
            return n1 * (x -= 2.25f / d1) * x + 0.9375f;
        }

        return n1 * (x -= 2.625f / d1) * x + 0.984375f;
    }
}