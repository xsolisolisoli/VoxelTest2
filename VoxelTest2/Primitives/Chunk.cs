using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;

namespace VoxelTest2.Primitives
{
    public class Chunk
    {
        public 
        const int CHUNK_SIZE = 16;
        const int BLOCK_RENDER_SIZE = 1;
        private Block[,,] _data; // Fixed the multidimensional array declaration

        public Chunk()
        {
            GenerateRandomChunk();
        }
        public void GenerateRandomChunk()
        {
            _data = new Block[CHUNK_SIZE, CHUNK_SIZE, CHUNK_SIZE];
            Random rand = new Random();
            for (int x = 0; x < CHUNK_SIZE; x++)
            {
                for (int y = 0; y < CHUNK_SIZE; y++)
                {
                    for (int z = 0; z < CHUNK_SIZE; z++)
                    {
                        _data[x, y, z] = new Block(rand.Next(0, 2));
                    }
                }
            }
        }
        void render()
        {

        }


        public void CreateMesh(Program program)
        {
            List<Vector3d> vertices = new List<Vector3d>();
            List<uint> indices = new List<uint>();
            uint indexOffset = 0;

            for (int x = 0; x < CHUNK_SIZE; x++)
            {
                for (int y = 0; y < CHUNK_SIZE; y++)
                {
                    for (int z = 0; z < CHUNK_SIZE; z++)
                    {
                        if (!_data[x, y, z]?.isActive ?? true) continue; // Fixed the property name to match the Block class

                        AddCubeVertices(x, y, z, vertices, indices, ref indexOffset);
                    }
                }
            }

            program.UploadToGPU(vertices, indices); // Fixed the method call
        }

        private void AddCubeVertices(int x, int y, int z, List<Vector3d> vertices, List<uint> indices, ref uint indexOffset)
        {
            float size = BLOCK_RENDER_SIZE;
            Vector3 center = new Vector3(x, y, z);

            // Define cube vertices relative to block position
            Vector3[] cubeVertices = {
                center + new Vector3(-size, -size,  size), // Front-left-bottom
                center + new Vector3( size, -size,  size), // Front-right-bottom
                center + new Vector3( size,  size,  size), // Front-right-top
                center + new Vector3(-size,  size,  size), // Front-left-top
                center + new Vector3( size, -size, -size), // Back-right-bottom
                center + new Vector3(-size, -size, -size), // Back-left-bottom
                center + new Vector3(-size,  size, -size), // Back-left-top
                center + new Vector3( size,  size, -size)  // Back-right-top
            };

            // Define faces with normals and vertex indices
            var faces = new[] {
                new { Normal = Vector3.UnitZ,  Vertices = new[] { 0, 1, 2, 3 } },  // Front
                new { Normal = -Vector3.UnitZ, Vertices = new[] { 4, 5, 6, 7 } },  // Back
                new { Normal = Vector3.UnitX,  Vertices = new[] { 1, 4, 7, 2 } },  // Right
                new { Normal = -Vector3.UnitX, Vertices = new[] { 5, 0, 3, 6 } },  // Left
                new { Normal = Vector3.UnitY,  Vertices = new[] { 3, 2, 7, 6 } },  // Top
                new { Normal = -Vector3.UnitY, Vertices = new[] { 5, 4, 1, 0 } },  // Bottom
            };

            foreach (var face in faces)
            {
                // Add vertices for this face
                foreach (int i in face.Vertices)
                {
                    vertices.Add(new Vector3d(
                        cubeVertices[i].X,
                        cubeVertices[i].Y,
                        cubeVertices[i].Z
                    ));
                }

                // Add indices for two triangles
                indices.AddRange(new[] {
                    indexOffset, indexOffset + 1, indexOffset + 2,
                    indexOffset, indexOffset + 2, indexOffset + 3
                });

                indexOffset += 4;
            }
        }


        public void CreateChunk()
        {

        }
    }
}
