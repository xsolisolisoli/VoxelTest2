using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using VoxelTest2.Primitives;
using VoxelTest2.Shading;
using static VoxelTest2.Primitives.Geometry;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace VoxelTest2
{
    public class Program : GameWindow
    {
        public Shader _shader;
        private int _vertexBufferObject;
        private int _vertexArrayObject;
        private int _elementBufferObject;
        private int elementCount;
        private int _instanceBufferObject;
        private int _shaderProgram;

        private Matrix4[] instanceMatrices;

        private float[] vertices = {
            // Front face
            -1.0f, -1.0f, -1.0f,
            1.0f, -1.0f, -1.0f,
            1.0f, 1.0f, -1.0f,
            -1.0f, 1.0f, -1.0f,
            // Back face
            -1.0f, -1.0f, 1.0f,
            1.0f, -1.0f, 1.0f,
            1.0f, 1.0f, 1.0f,
            -1.0f, 1.0f, 1.0f
        };

        public float[] indices = new float[]
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

        public Program(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings() { Size = (width, height), Title = title }) { }

        public static void Main()
        {
            runGame(new Program(800, 600, "VoxelTest2"));
        }

        public static void runGame(Program game)
        {
            game.Run();
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            _shader.Dispose();
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            _shader = new Shader("shader.vert", "shader.frag");

            GL.ClearColor(Color4.Black);
            GL.Enable(EnableCap.DepthTest);

            // Create shaders
            string vertexShaderSource = @"#version 330 core
                layout(location = 0) in vec3 aPosition;
                layout(location = 1) in mat4 instanceMatrix;
                uniform mat4 view;
                uniform mat4 projection;
                void main()
                {
                    gl_Position = projection * view * instanceMatrix * vec4(aPosition, 1.0);
                }";

                    string fragmentShaderSource = @"#version 330 core
                out vec4 FragColor;
                void main()
                {
                    FragColor = vec4(1.0, 1.0, 1.0, 1.0);
                }";

            // Compile shaders
            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexShaderSource);
            GL.CompileShader(vertexShader);

            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentShaderSource);
            GL.CompileShader(fragmentShader);

            // Create shader program
            _shaderProgram = GL.CreateProgram();
            GL.AttachShader(_shaderProgram, vertexShader);
            GL.AttachShader(_shaderProgram, fragmentShader);
            GL.LinkProgram(_shaderProgram);

            // Cleanup shaders
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);

            // Set vertex attributes
            //GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            //GL.EnableVertexAttribArray(0);
            Chunk _test = new Chunk();
            _test.CreateMesh(this);

        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Matrix4 view = Matrix4.LookAt(new Vector3(0, 0, 20), Vector3.Zero, Vector3.UnitY);
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(45),
                (float)Size.X / Size.   Y,
                0.1f,
                100.0f);

            GL.UseProgram(_shaderProgram);
            int viewLocation = GL.GetUniformLocation(_shaderProgram, "view");
            GL.UniformMatrix4(viewLocation, false, ref view);
            int projectionLocation = GL.GetUniformLocation(_shaderProgram, "projection");
            GL.UniformMatrix4(projectionLocation, false, ref projection);

            GL.BindVertexArray(_vertexArrayObject);
            GL.DrawElements(PrimitiveType.Triangles, elementCount,
                            DrawElementsType.UnsignedInt, 0);
            //GL.BindVertexArray(_vertexArrayObject);
            //GL.DrawElementsInstanced(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, IntPtr.Zero, instanceMatrices.Length);

            SwapBuffers();
        }

        protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
        {
            base.OnFramebufferResize(e);
            GL.Viewport(0, 0, e.Width, e.Height);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
        }
        public void UploadToGPU(List<Vector3d> vertices, List<uint> indices)
        {
            // Delete previous buffers if they exist
            Dispose();

            // Create vertex array
            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);

            // Create and upload vertex buffer
            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Count * Vector3.SizeInBytes,
                         vertices.ToArray(), BufferUsageHint.StaticDraw);

            // Create and upload element buffer
            elementCount = indices.Count;
            _elementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, elementCount * sizeof(uint),
                         indices.ToArray(), BufferUsageHint.StaticDraw);

            // Set vertex attributes
            GL.EnableVertexAttribArray(0); // Position
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false,
                                 Vector3.SizeInBytes, 0);

            GL.EnableVertexAttribArray(1); // Normal
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false,
                                 Vector3.SizeInBytes, 3 * sizeof(float));

            GL.EnableVertexAttribArray(2); // Color
            GL.VertexAttribPointer(2, 4, VertexAttribPointerType.Float, false,
                                 Vector3.SizeInBytes, 6 * sizeof(float));

            GL.BindVertexArray(0);
        }
    }
}