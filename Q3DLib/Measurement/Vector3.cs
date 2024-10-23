namespace Q3DLib.Measurement
{
    public readonly struct Vector3(float x, float y, float z)
    {
        #region Constants
        public static readonly Vector3 Zero = new(x: 0, y: 0, z: 0);
        public static readonly Vector3 One = new(x: 1, y: 1, z: 1);
        public static readonly Vector3 UnitX = new(x: 1, y: 0, z: 0);
        public static readonly Vector3 UnitY = new(x: 0, y: 1, z: 0);
        public static readonly Vector3 UnitZ = new(x: 0, y: 0, z: 1);
        #endregion

        #region Properties
        public float X { get; init; } = x;
        public float Y { get; init; } = y;
        public float Z { get; init; } = z;

        public float Length => MathF.Sqrt((X * X) + (Y * Y) + (Z * Z));
        public Vector3 Normalized => Normalize();
        #endregion

        #region Operators
        public static Vector3 operator +(Vector3 left, Vector3 right) => new((left.X + right.X), (left.Y + right.Y), (left.Z + right.Z));
        public static Vector3 operator -(Vector3 left, Vector3 right) => new((left.X - right.X), (left.Y - right.Y), (left.Z - right.Z));
        public static Vector3 operator *(Vector3 vector, float value) => new((vector.X * value), (vector.Y * value), (vector.Z * value));
        public static Vector3 operator /(Vector3 vector, float value) => new((vector.X / value), (vector.Y / value), (vector.Z / value));

        public static Vector3 operator -(Vector3 vector) => new(-vector.X, -vector.Y, -vector.Z);

        public static bool operator ==(Vector3 left, Vector3 right) => (left.X == right.X) && (left.Y == right.Y) && (left.Z == right.Z);
        public static bool operator !=(Vector3 left, Vector3 right) => !(left == right);
        #endregion

        #region Methods
        public Vector3 RotateX(float theta) =>
            new(
                x: X,
                y: (MathF.Cos(theta) * Y) + (-MathF.Sin(theta) * Z),
                z: (MathF.Sin(theta) * Y) + (MathF.Cos(theta) * Z)
            );

        public Vector3 RotateY(float theta) =>
            new(
                x: (MathF.Cos(theta) * X) + (MathF.Sin(theta) * Z),
                y: Y,
                z: (-MathF.Sin(theta) * X) + (MathF.Cos(theta) * Z)
            );

        public Vector3 RotateZ(float theta) =>
            new(
                x: (MathF.Cos(theta) * X) + (-MathF.Sin(theta) * Y),
                y: (MathF.Sin(theta) * X) + (MathF.Cos(theta) * Y),
                z: Z
            );

        public Vector3 Normalize() => (Length == 0) ? Zero : (this / Length);
        public Vector3 Cross(Vector3 other) =>
            new(
                x: (Y * other.Z) - (Z * other.Y),
                y: (Z * other.X) - (X * other.Z),
                z: (X * other.Y) - (Y * other.X)
               );
        public Vector3 Lerp(Vector3 to, float weight) => this + (to - this) * weight;
        public float Dot(Vector3 other) => (X * other.X) + (Y * other.Y) + (Z * other.Z);
        public override bool Equals(object? obj) => (obj is Vector3 vector) && (this == vector);
        public override int GetHashCode() => HashCode.Combine(X, Y, Z);
        public override string ToString() => $"{{ X:{X}, Y:{Y}, Z:{Z} }}";
        #endregion
    }
}