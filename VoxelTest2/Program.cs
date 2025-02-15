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
        //int VertexBufferObject;
        //int VertexArrayObject;

        private int _vertexBufferObject;
        private int _vertexArrayObject;
        private int _elementBufferObject;
        private int _shaderProgram;

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

            // Create shaders
            string vertexShaderSource = @"#version 330 core
        layout(location = 0) in vec3 aPosition;
        uniform mat4 mvp;
        void main()
        {
            gl_Position = mvp * vec4(aPosition, 1.0);
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
            float time = (float)Environment.TickCount / 1000;
            Matrix4 model = Matrix4.CreateRotationX(0) * Matrix4.CreateRotationY(0);
            Matrix4 view = Matrix4.LookAt(new Vector3(0, 0, 5), Vector3.Zero, Vector3.UnitY);
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(45),
                (float)Size.X / Size.Y,
                0.1f,
                100.0f);
            Matrix4 mvp = model * view * projection;

            // Set shader parameters
            GL.UseProgram(_shaderProgram);
            int mvpLocation = GL.GetUniformLocation(_shaderProgram, "mvp");
            GL.UniformMatrix4(mvpLocation, false, ref mvp);

            // Draw cube
            GL.BindVertexArray(_vertexArrayObject);
            GL.DrawElements(PrimitiveType.Lines, indices.Length, DrawElementsType.UnsignedInt, 0);

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