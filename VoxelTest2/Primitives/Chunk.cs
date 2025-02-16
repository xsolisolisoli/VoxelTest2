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
        const int CHUNK_SIZE = 16;
        const int BLOCK_RENDER_SIZE = 2;
        private Block[,,] _data; // Fixed the multidimensional array declaration
        private int vertexArrayObject;
        private int vertexBufferObject;
        private int elementBufferObject;
        private int elementCount;

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


        public void CreateMesh()
        {
            List<Vertex> vertices = new List<Vertex>();
            List<uint> indices = new List<uint>();
            uint indexOffset = 0;

            for (int x = 0; x < CHUNK_SIZE; x++)
            {
                for (int y = 0; y < CHUNK_SIZE; y++)
                {
                    for (int z = 0; z < CHUNK_SIZE; z++)
                    {
                        if (!_data[x, y, z]?.isActive ?? true) continue;

                        AddCubeVertices(x, y, z, vertices, indices, ref indexOffset);
                    }
                }
            }
            UploadToGPU(vertices, indices);
        }

        private void AddCubeVertices(int x, int y, int z, List<Vertex> vertices, List<uint> indices, ref uint indexOffset)
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
                    vertices.Add(new Vertex(
                        cubeVertices[i],
                        face.Normal,
                        new Color4(1.0f, 1.0f, 1.0f, 1.0f) // White color
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
        public void Render()
        {
            GL.BindVertexArray(vertexArrayObject);
            GL.DrawElements(PrimitiveType.Triangles, elementCount,
                            DrawElementsType.UnsignedInt, 0);
        }
        public void Dispose()
        {
            GL.DeleteVertexArray(vertexArrayObject);
            GL.DeleteBuffer(vertexBufferObject);
            GL.DeleteBuffer(elementBufferObject);
            vertexArrayObject = vertexBufferObject = elementBufferObject = 0;
        }
        private void UploadToGPU(List<Vertex> vertices, List<uint> indices)
        {
            // Delete previous buffers if they exist
            Dispose();

            // Create vertex array
            vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(vertexArrayObject);

            // Create and upload vertex buffer
            vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Count * Vertex.SizeInBytes,
                         vertices.ToArray(), BufferUsageHint.StaticDraw);

            // Create and upload element buffer
            elementCount = indices.Count;
            elementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, elementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, elementCount * sizeof(uint),
                         indices.ToArray(), BufferUsageHint.StaticDraw);

            // Set vertex attributes
            GL.EnableVertexAttribArray(0); // Position
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false,
                                 Vertex.SizeInBytes, 0);

            GL.EnableVertexAttribArray(1); // Normal
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false,
                                 Vertex.SizeInBytes, 3 * sizeof(float));

            GL.EnableVertexAttribArray(2); // Color
            GL.VertexAttribPointer(2, 4, VertexAttribPointerType.Float, false,
                                 Vertex.SizeInBytes, 6 * sizeof(float));

            GL.BindVertexArray(0);
        }

        public void CreateChunk()
        {

        }
    }
}
