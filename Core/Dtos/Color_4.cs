public struct Color_4
{
    public float R { get; set; }
    public float G { get; set; }
    public float B { get; set; }
    public float A { get; set; }

    public Color_4(float r, float g, float b, float a = 1f)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }

    public static bool operator ==(Color_4 c1, Color_4 c2)
    {
        return c1.R == c2.R && c1.G == c2.G && c1.B == c2.B && c1.A == c2.A;
    }

    public static bool operator !=(Color_4 c1, Color_4 c2)
    {
        return !(c1 == c2);
    }

    public override bool Equals(object obj)
    {
        return obj is Color_4 other && this == other;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(R, G, B, A);
    }

    public static Color_4 operator +(Color_4 a, Color_4 b)
    {
        return new Color_4(a.R + b.R, a.G + b.G, a.B + b.B, a.A + b.A);
    }

    public static Color_4 operator *(Color_4 color, float scalar)
    {
        return new Color_4(color.R * scalar, color.G * scalar, color.B * scalar, color.A * scalar);
    }

    public static Color_4 operator *(float scalar, Color_4 color)
    {
        return color * scalar;
    }
}
