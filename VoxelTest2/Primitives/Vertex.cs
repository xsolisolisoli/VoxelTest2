using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace VoxelTest2.Primitives
{
    public struct Vertex
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Color4 Color;

        public Vertex(Vector3 position, Vector3 normal, Color4 color)
        {
            Position = position;
            Normal = normal;
            Color = color;
        }

        public static int SizeInBytes => (3 + 3 + 4) * sizeof(float);
    }
}
