using Q3DLib.Geometry;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Q3DLib
{
    [SupportedOSPlatform(nameof(OSPlatform.Windows))]
    public class Camera(Vector3 position, int fov, int renderDistance, float clipPlane)
    {
        public Vector3 Position { get; private set; } = position;
        public Vector3 Rotation { get; private set; } = new(0, 0, 0);
        public int Fov { get; private set; } = fov;
        public int RenderDistance { get; private set; } = renderDistance;
        public float ClipPlane { get; private set; } = clipPlane;

        private float[,] zBuffer = null!;

        public void Render(Graphics g, Shape3D shape, int width, int height, RenderMode renderMode)
        {
            Vector3[] rotatedVertices = shape.GetRotatedVertices();
            Vector3[] projectedVertices = new Vector3[rotatedVertices.Length];

            zBuffer = new float[width, height];

            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    zBuffer[x, y] = float.MaxValue;

            for (int i = 0; i < shape.Vertices.Length; i++)
            {
                Vector3 relativePosition = rotatedVertices[i] - Position;
                relativePosition = RotatePosition(relativePosition, -Rotation);

                projectedVertices[i] = Project(relativePosition, width, height);
            }

            for (int i = 0; i + 2 < shape.Indices.Length; i += 3)
            {
                try
                {
                    Vector3 vertex1 = projectedVertices[shape.Indices[i]];
                    Vector3 vertex2 = projectedVertices[shape.Indices[i + 1]];
                    Vector3 vertex3 = projectedVertices[shape.Indices[i + 2]];

                    if (!vertex1.Valid || !vertex2.Valid || !vertex3.Valid)
                        continue;

                    if (renderMode == RenderMode.Wireframe)
                        RenderWireFrame(g, vertex1, vertex2, vertex3);
                    else if (renderMode == RenderMode.Rasterize)
                        throw new NotSupportedException("Rasterization is not yet supported.");
                            //RasterizeTriangle(g, vertex1, vertex2, vertex3);
                }
                catch (OverflowException)
                {
                    continue;
                }
            }
        }

        private void RenderWireFrame(Graphics g, Vector3 vertex1, Vector3 vertex2, Vector3 vertex3)
        {
            using Pen pen = new(Color.White, .1f);

            g.DrawLine(pen, vertex1.X, vertex1.Y, vertex2.X, vertex2.Y);
            g.DrawLine(pen, vertex2.X, vertex2.Y, vertex3.X, vertex3.Y);
            g.DrawLine(pen, vertex3.X, vertex3.Y, vertex1.X, vertex1.Y);
        }

        /*private void RasterizeTriangle(Graphics g, Vector3 vertex1, Vector3 vertex2, Vector3 vertex3)
        {
            int minX = (int)MathF.Min(MathF.Min(vertex1.X, vertex2.X), vertex3.X);
            int maxX = (int)MathF.Max(MathF.Max(vertex1.X, vertex2.X), vertex3.X);
            int minY = (int)MathF.Min(MathF.Min(vertex1.Y, vertex2.Y), vertex3.Y);
            int maxY = (int)MathF.Max(MathF.Max(vertex1.Y, vertex2.Y), vertex3.Y);

            minX = (int)MathF.Max(minX, 0);
            maxX = (int)MathF.Min(maxX, zBuffer.GetLength(0) - 1);
            minY = (int)MathF.Max(minY, 0);
            maxY = (int)MathF.Min(maxY, zBuffer.GetLength(1) - 1);

            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    Vector3 barycentric = GetBarycentric(vertex1, vertex2, vertex3, x, y);

                    if (barycentric.X >= 0 && barycentric.Y >= 0 && barycentric.Z >= 0)
                    {
                        float z = barycentric.X * vertex1.Z + barycentric.Y * vertex2.Z + barycentric.Z * vertex3.Z;

                        if (z < zBuffer[x, y])
                        {
                            zBuffer[x, y] = z;

                            g.FillRectangle(Brushes.White, x, y, 1, 1);
                        }
                    }
                }
            }
        }

        private Vector3 GetBarycentric(Vector3 vertex1, Vector3 vertex2, Vector3 vertex3, float px, float py)
        {
            float denom = (vertex2.Y - vertex3.Y) * (vertex1.X - vertex3.X) + (vertex3.X - vertex2.X) * (vertex1.Y - vertex3.Y);
            float w1 = ((vertex2.Y - vertex3.Y) * (px - vertex3.X) + (vertex3.X - vertex2.X) * (py - vertex3.Y)) / denom;
            float w2 = ((vertex3.Y - vertex1.Y) * (px - vertex3.X) + (vertex1.X - vertex3.X) * (py - vertex3.Y)) / denom;
            float w3 = 1 - w1 - w2;

            return new Vector3(w1, w2, w3);
        }*/


        public Vector3 Project(Vector3 vector, int width, int height)
        {
            float aspect = (float)width / height;
            float fovFactor = MathF.Tan(Fov * MathF.PI / 180f / 2f);

            float xClip = vector.X / (aspect * fovFactor);
            float yClip = vector.Y / fovFactor;
            float zClip = (vector.Z * (RenderDistance + ClipPlane) + 2 * RenderDistance * ClipPlane) / (RenderDistance - ClipPlane);
            float wClip = vector.Z;

            if (wClip <= 0)
                return new Vector3(float.NaN, float.NaN, float.NaN);

            float xNdc = xClip / wClip;
            float yNdc = yClip / wClip;
            float zNdc = zClip / wClip;

            return new Vector3(
                x: ((width / 2f) * (xNdc + 1)),
                y: ((height / 2f) * (1 - yNdc)),
                z: zNdc
            );
        }

        private Vector3 RotatePosition(Vector3 position, Vector3 rotation)
        {
            position = position.RotateY(rotation.Y);
            position = position.RotateX(rotation.X);
            position = position.RotateZ(rotation.Z);

            return position;
        }

        public void Move(Vector3 direction, float moveSpeed) => Position += direction * moveSpeed;
        public void MoveForward(float moveSpeed)
        {
            Vector3 forward = new(
                MathF.Sin(Rotation.Y),
                0,
                MathF.Cos(Rotation.Y)
            );

            Move(forward, moveSpeed);
        }
        public void MoveRight(float moveSpeed)
        {
            Vector3 right = new(
                MathF.Cos(Rotation.Y),
                0,
                -MathF.Sin(Rotation.Y)
            );

            Move(right, moveSpeed);
        }

        public void MoveBackward(float moveSpeed) => MoveForward(-moveSpeed);
        public void MoveLeft(float moveSpeed) => MoveRight(-moveSpeed);

        public void Rotate(float x = 0f, float y = 0f, float z = 0f) => Rotate(new Vector3(x, y, z));
        public void Rotate(Vector3 rotationDelta) => Rotation += rotationDelta;
        public void TranslatePosition(float x = 0, float y = 0, float z = 0) => Position += new Vector3(x, y, z);
        public void SetPosition(Vector3 pos) => Position = pos;
    }
}