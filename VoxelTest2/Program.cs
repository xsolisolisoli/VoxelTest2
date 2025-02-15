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

        private uint[] indices = {
            // Front face edges
            0, 1, 1, 2, 2, 3, 3, 0,
            // Back face edges
            4, 5, 5, 6, 6, 7, 7, 4,
            // Connecting edges
            0, 4, 1, 5, 2, 6, 3, 7
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

            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);

            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            _elementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            int instanceCount = 16;
            instanceMatrices = new Matrix4[instanceCount];
            for (int i = 0; i < instanceCount; i++)
            {
                float x = (i % 10) * 2.0f;
                float y = ((i / 10) % 10) * 2.0f;
                float z = (i / 100) * 2.0f;
                instanceMatrices[i] = Matrix4.CreateTranslation(x, y, z);
            }

            int matrix4SizeInBytes = Marshal.SizeOf<Matrix4>();

            _instanceBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _instanceBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, instanceMatrices.Length * matrix4SizeInBytes, instanceMatrices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            for (int i = 0; i < 4; i++)
            {
                GL.VertexAttribPointer(1 + i, 4, VertexAttribPointerType.Float, false, matrix4SizeInBytes, i * 16);
                GL.EnableVertexAttribArray(1 + i);
                GL.VertexAttribDivisor(1 + i, 1);
            }

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
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Matrix4 view = Matrix4.LookAt(new Vector3(0, 0, 20), Vector3.Zero, Vector3.UnitY);
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(45),
                (float)Size.X / Size.Y,
                0.1f,
                100.0f);

            GL.UseProgram(_shaderProgram);
            int viewLocation = GL.GetUniformLocation(_shaderProgram, "view");
            GL.UniformMatrix4(viewLocation, false, ref view);
            int projectionLocation = GL.GetUniformLocation(_shaderProgram, "projection");
            GL.UniformMatrix4(projectionLocation, false, ref projection);

            GL.BindVertexArray(_vertexArrayObject);
            GL.DrawElementsInstanced(PrimitiveType.Lines, indices.Length, DrawElementsType.UnsignedInt, IntPtr.Zero, instanceMatrices.Length);

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
    }
}