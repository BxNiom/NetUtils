namespace BxNiom.Math;

public class Affine {
    public Affine() {
        Reset();
    }

    public float M00 { get; set; } = 1.0f;
    public float M01 { get; set; }
    public float M02 { get; set; }
    public float M10 { get; set; }
    public float M11 { get; set; } = 1.0f;
    public float M12 { get; set; }

    public bool    IsIdentity    => M00 == 1.0f && M11 == 1.0f && M01 == 0.0f && M10 == 0.0f;
    public bool    IsTranslation => M00 == 1.0f && M11 == 1.0f && M01 == 0.0f && M10 == 0.0f;
    public Vector2 Translation   => new(M02, M12);
    public float   Determinant   => M00 * M11 - M01 * M10;

    public Affine Set(Affine other) {
        M00 = other.M00;
        M01 = other.M01;
        M02 = other.M02;
        M10 = other.M10;
        M11 = other.M11;
        M12 = other.M12;
        return this;
    }

    public Affine Reset() {
        M00 = 1.0f;
        M01 = 0.0f;
        M02 = 0.0f;
        M10 = 0.0f;
        M11 = 1.0f;
        M12 = 0.0f;
        return this;
    }

    public Affine SetToTranslation(float x, float y) {
        M00 = 1.0f;
        M01 = 0.0f;
        M02 = x;
        M10 = 0.0f;
        M11 = 1.0f;
        M12 = y;
        return this;
    }

    public Affine SetToScaling(float scaleX, float scaleY) {
        M00 = scaleX;
        M01 = 0.0f;
        M02 = 0.0f;
        M10 = 0.0f;
        M11 = scaleY;
        M12 = 0.0f;
        return this;
    }

    public Affine SetToRotation(float degrees) {
        return SetToRotation(MathF.Cos(degrees * MathUtils.DegToRad), MathF.Sin(degrees * MathUtils.DegToRad));
    }

    public Affine SetToRotationRad(float radians) {
        return SetToRotation(MathF.Cos(radians), MathF.Sin(radians));
    }

    public Affine SetToRotation(float cos_, float sin_) {
        M00 = cos_;
        M01 = -sin_;
        M02 = 0.0f;
        M10 = sin_;
        M11 = cos_;
        M12 = 0.0f;
        return this;
    }

    public Affine SetToShearing(float shearX, float shearY) {
        M00 = 1.0f;
        M01 = shearX;
        M02 = 0.0f;
        M10 = shearY;
        M11 = 1.0f;
        M12 = 0.0f;
        return this;
    }

    public Affine SetToTranslationScale(float x, float y, float scaleX, float scaleY) {
        M00 = scaleX;
        M01 = 0.0f;
        M02 = x;
        M10 = 0.0f;
        M11 = scaleY;
        M12 = y;
        return this;
    }

    public Affine SetToTranslationRotationScale(float x, float y, float degree, float scaleX, float scaleY) {
        M02 = x;
        M12 = y;

        if (degree == 0f) {
            M00 = scaleX;
            M01 = 0.0f;
            M10 = 0.0f;
            M11 = scaleY;
        } else {
            var sin = MathF.Sin(degree * MathUtils.DegToRad);
            var cos = MathF.Cos(degree * MathUtils.DegToRad);

            M00 = cos * scaleX;
            M01 = -sin * scaleY;
            M10 = sin * scaleX;
            M11 = cos * scaleY;
        }

        return this;
    }

    public Affine SetToProduct(Affine l, Affine r) {
        M00 = l.M00 * r.M00 + l.M01 * r.M10;
        M01 = l.M00 * r.M01 + l.M01 * r.M11;
        M02 = l.M00 * r.M02 + l.M01 * r.M12 + l.M02;
        M10 = l.M10 * r.M00 + l.M11 * r.M10;
        M11 = l.M10 * r.M01 + l.M11 * r.M11;
        M12 = l.M10 * r.M02 + l.M11 * r.M12 + l.M12;
        return this;
    }

    public Affine Invert() {
        var det = Determinant;

        if (det == 0f) {
            throw new InvalidOperationException("Can't invert a singular affine matrix");
        }

        var invDet = 1.0f / det;

        var tmp00 = M11;
        var tmp01 = -M01;
        var tmp02 = M01 * M12 - M11 * M02;
        var tmp10 = -M10;
        var tmp11 = M00;
        var tmp12 = M10 * M02 - M00 * M12;

        M00 = invDet * tmp00;
        M01 = invDet * tmp01;
        M02 = invDet * tmp02;
        M10 = invDet * tmp10;
        M11 = invDet * tmp11;
        M12 = invDet * tmp12;
        return this;
    }

    public Affine Multiply(Affine other) {
        var tmp00 = M00 * other.M00 + M01 * other.M10;
        var tmp01 = M00 * other.M01 + M01 * other.M11;
        var tmp02 = M00 * other.M02 + M01 * other.M12 + M02;
        var tmp10 = M10 * other.M00 + M11 * other.M10;
        var tmp11 = M10 * other.M01 + M11 * other.M11;
        var tmp12 = M10 * other.M02 + M11 * other.M12 + M12;

        M00 = tmp00;
        M01 = tmp01;
        M02 = tmp02;
        M10 = tmp10;
        M11 = tmp11;
        M12 = tmp12;
        return this;
    }

    public Affine Translate(float x, float y) {
        M02 += M00 * x + M01 * y;
        M12 += M10 * x + M11 * y;
        return this;
    }

    public Affine Scale(float scaleX, float scaleY) {
        M00 *= scaleX;
        M01 *= scaleY;
        M10 *= scaleX;
        M11 *= scaleY;
        return this;
    }

    public Affine Rotate(float degrees) {
        if (degrees == 0f) {
            return this;
        }

        var cos = MathF.Cos(degrees * MathUtils.DegToRad);
        var sin = MathF.Sin(degrees * MathUtils.DegToRad);

        var tmp00 = M00 * cos + M01 * sin;
        var tmp01 = M00 * -sin + M01 * cos;
        var tmp10 = M10 * cos + M11 * sin;
        var tmp11 = M10 * -sin + M11 * cos;

        M00 = tmp00;
        M01 = tmp01;
        M10 = tmp10;
        M11 = tmp11;

        return this;
    }

    public Affine RotateRad(float radians) {
        if (radians == 0f) {
            return this;
        }

        var cos = MathF.Cos(radians);
        var sin = MathF.Sin(radians);

        var tmp00 = M00 * cos + M01 * sin;
        var tmp01 = M00 * -sin + M01 * cos;
        var tmp10 = M10 * cos + M11 * sin;
        var tmp11 = M10 * -sin + M11 * cos;

        M00 = tmp00;
        M01 = tmp01;
        M10 = tmp10;
        M11 = tmp11;
        return this;
    }

    public Affine Shear(float shearX, float shearY) {
        var tmp0 = M00 + shearY * M01;
        var tmp1 = M01 + shearX * M00;
        M00 = tmp0;
        M01 = tmp1;

        tmp0 = M10 + shearY * M11;
        tmp1 = M11 + shearX * M10;
        M10  = tmp0;
        M11  = tmp1;
        return this;
    }

    public Vector2 ApplyTo(Vector2 v) {
        return ApplyTo(v.X, v.Y);
    }

    public Vector2 ApplyTo(float x, float y) {
        return new Vector2(M00 * x + M01 * y + M02, M10 * x + M11 * y + M12);
    }

    public Affine SetToTranslation(Vector2 v) {
        return SetToTranslation(v.X, v.Y);
    }

    public Affine SetToScaling(Vector2 scale) {
        return SetToScaling(scale.X, scale.Y);
    }

    public Affine SetToShearing(Vector2 shear) {
        return SetToShearing(shear.X, shear.Y);
    }

    public Affine Translate(Vector2 v) {
        return Translate(v.X, v.Y);
    }

    public Affine Scale(Vector2 s) {
        return Scale(s.X, s.Y);
    }

    public Affine Shear(Vector2 s) {
        return Shear(s.X, s.Y);
    }

    public static Affine TranslationRotationScale(float x, float y, float degree, float scaleX, float scaleY) {
        return new Affine().SetToTranslationRotationScale(x, y, degree, scaleX, scaleY);
    }
}