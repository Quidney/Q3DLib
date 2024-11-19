namespace Q3DLib.Geometry
{
    public readonly struct Shape3D(Vector3[] vertices, int[] indices, Vector3 position, Vector3 rotation)
    {
        public Vector3 Position { get; } = position;
        public Vector3 Rotation { get;  } = rotation;
        public Vector3[] Vertices { get; init; } = vertices;
        public int[] Indices { get; init; } = indices;

        public Shape3D Rotate(float x = 0f, float y = 0f, float z = 0f) =>  Rotate(new Vector3(x, y, z));
        public Shape3D Rotate(Vector3 rotationDelta) => new(Vertices, Indices, Position, Rotation + rotationDelta);

        public Vector3[] GetRotatedVertices()
        {
            Vector3[] rotatedVertices = new Vector3[Vertices.Length];
            for (int i = 0; i < Vertices.Length; i++)
            {
                Vector3 vertex = Vertices[i];

                vertex = vertex.RotateY(Rotation.Y);
                vertex = vertex.RotateX(Rotation.X);
                vertex = vertex.RotateZ(Rotation.Z);

                rotatedVertices[i] = vertex + Position;
            }
            return rotatedVertices;
        }

        public static Shape3D FromObj(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("File not found", filePath);

            List<Vector3> vertices = [];
            List<int> indices = [];

            string[] lines = File.ReadAllLines(filePath);

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                    continue;

                string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                if (parts[0] == "v" && parts.Length == 4)
                {
                    float x = float.Parse(parts[1]);
                    float y = float.Parse(parts[2]);
                    float z = float.Parse(parts[3]);
                    vertices.Add(new Vector3(x, y, z));
                }

                else if (parts[0] == "f")
                {
                    for (int i = 1; i < parts.Length; i++)
                    {
                        int index = int.Parse(parts[i]) - 1;
                        indices.Add(index);
                    }
                }
            }

            if (vertices.Count == 0 || indices.Count == 0)
                throw new InvalidOperationException("Invalid OBJ file: no vertices or faces found.");

            return new Shape3D(vertices.ToArray(), indices.ToArray(), new Vector3(0, 0, 500), Vector3.Zero);
        }

    }
}
