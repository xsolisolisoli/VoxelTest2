using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace VoxelTest2.Primitives
{
    public class Geometry
    {
        public Vector3[] getBlockVertices()
        {
            return blockVertices;
        }        
        public Vector3[] getBlockIndices()
        {
            return blockVertices;
        }
        public Vector3[] blockVertices = new Vector3[]
        {
            new Vector3(-0.5f, -0.5f, -0.5f), // Bottom-left-back
            new Vector3( 0.5f, -0.5f, -0.5f), // Bottom-right-back
            new Vector3( 0.5f,  0.5f, -0.5f), // Top-right-back
            new Vector3(-0.5f,  0.5f, -0.5f), // Top-left-back
            new Vector3(-0.5f, -0.5f,  0.5f), // Bottom-left-front
            new Vector3( 0.5f, -0.5f,  0.5f), // Bottom-right-front
            new Vector3( 0.5f,  0.5f,  0.5f), // Top-right-front
            new Vector3(-0.5f,  0.5f,  0.5f)  // Top-left-front
        };
        public int[] blockIndices = new int[]
        {
            // Back face
            0, 1, 2, 2, 3, 0,
            // Front face
            4, 5, 6, 6, 7, 4,
            // Left face
            0, 3, 7, 7, 4, 0,
            // Right face
            1, 2, 6, 6, 5, 1,
            // Bottom face
            0, 1, 5, 5, 4, 0,
            // Top face
            2, 3, 7, 7, 6, 2
        };
    }
}
