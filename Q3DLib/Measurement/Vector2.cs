namespace Q3DLib.Measurement
{
    public readonly struct Vector2(float x, float y)
    {
        #region Constants
        public static readonly Vector2 Zero = new(x: 0, y: 0);
        public static readonly Vector2 One = new(x: 1, y: 1);
        public static readonly Vector2 UnitX = new(x: 1, y: 0);
        public static readonly Vector2 UnitY = new(x: 0, y: 1);
        #endregion

        #region Properties
        public float X { get; init; } = x;
        public float Y { get; init; } = y;

        public float Length => MathF.Sqrt((X * X) + (Y * Y));
        public Vector2 Normalized => Normalize();
        #endregion

        #region Operators
        public static Vector2 operator +(Vector2 left, Vector2 right) => new((left.X + right.X), (left.Y + right.Y));
        public static Vector2 operator -(Vector2 left, Vector2 right) => new((left.X - right.X), (left.Y - right.Y));
        public static Vector2 operator *(Vector2 vector, float value) => new((vector.X * value), (vector.Y * value));
        public static Vector2 operator /(Vector2 vector, float value) => new((vector.X / value), (vector.Y / value));

        public static Vector2 operator -(Vector2 vector) => new(-vector.X, -vector.Y);

        public static bool operator ==(Vector2 left, Vector2 right) => (left.X == right.X) && (left.Y == right.Y);
        public static bool operator !=(Vector2 left, Vector2 right) => !(left == right);
        #endregion

        #region Methods
        public Vector2 Normalize() => this / Length;
        public float Dot(Vector2 other) => (X * other.X) + (Y * other.Y);

        public override bool Equals(object? obj) => (obj is Vector2 vector) && (this == vector);
        public override int GetHashCode() => HashCode.Combine(X, Y);
        public override string ToString() => $"{{ X:{X}, Y:{Y} }}";
        #endregion
    }
}