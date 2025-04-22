namespace Kingdom_of_Creation.Dtos
{
    public class Vector_2
    {
        public float X { get; set; }
        public float Y { get; set; }

        public Vector_2() { }

        public Vector_2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public static Vector_2 operator +(Vector_2 a, Vector_2 b)
        {
            return new Vector_2(a.X + b.X, a.Y + b.Y);
        }

        public static Vector_2 operator -(Vector_2 a, Vector_2 b)
        {
            return new Vector_2(a.X - b.X, a.Y - b.Y);
        }

        public static Vector_2 operator *(Vector_2 a, float scalar)
        {
            return new Vector_2(a.X * scalar, a.Y * scalar);
        }

        public static Vector_2 operator /(Vector_2 a, float scalar)
        {
            return new Vector_2(a.X / scalar, a.Y / scalar);
        }

        public float Length()
        {
            return MathF.Sqrt(X * X + Y * Y);
        }

        public Vector_2 Normalize()
        {
            float length = Length();
            if (length == 0) return new Vector_2(0, 0);
            return new Vector_2(X / length, Y / length);
        }

        public float Dot(Vector_2 other)
        {
            return X * other.X + Y * other.Y;
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }
}
