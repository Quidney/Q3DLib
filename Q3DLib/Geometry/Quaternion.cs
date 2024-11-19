using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Q3DLib.Geometry
{
    public readonly struct Quaternion(float w, float x, float y, float z)
    {
        public float X { get; } = x;
        public float Y { get; } = y;
        public float Z { get; } = z;
        public float W { get; } = w;

        public readonly float Magnitude => MathF.Sqrt(x * x + y * y + z * z + w * w);

        public Quaternion Normalize()
        {
            float magnitude = Magnitude;
            return new Quaternion(x / magnitude, y / magnitude, z / magnitude, w / magnitude);
        }

        public static Quaternion operator +(Quaternion q1, Quaternion q2)
        {
            return new Quaternion(q1.W + q2.X, q1.Y + q2.Y, q1.Z + q2.Z, q1.W + q2.W);
        }

        public static Quaternion operator -(Quaternion q1, Quaternion q2)
        {
            return new Quaternion(q1.X - q2.X, q1.Y - q2.Y, q1.Z - q2.Z, q1.W - q2.W);
        }

        public static Quaternion operator *(Quaternion q1, Quaternion q2)
        {
            float newX = q1.W * q2.X + q1.X * q2.W + q1.Y * q2.Z - q1.Z * q2.Y;
            float newY = q1.W * q2.Y - q1.X * q2.Z + q1.Y * q2.W + q1.Z * q2.X;
            float newZ = q1.W * q2.Z + q1.X * q2.Y - q1.Y * q2.X + q1.Z * q2.W;
            float newW = q1.W * q2.W - q1.X * q2.X - q1.Y * q2.Y - q1.Z * q2.Z;

            return new Quaternion(newX, newY, newZ, newW);
        }
    }
}
