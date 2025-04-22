namespace Kingdom_of_Creation.Dtos
{
    public class Color_4
    {
        public float R { get; set; }
        public float G { get; set; }
        public float B { get; set; }
        public float A { get; set; }

        public Color_4() { }

        public Color_4(float r, float g, float b, float a = 1f)
        {
            R = r;
            G = g;
            B = b;
            A = a;
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
}
