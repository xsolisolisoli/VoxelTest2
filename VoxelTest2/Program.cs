using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using VoxelTest2.Shading;
using static VoxelTest2.Primitives.Geometry;
namespace VoxelTest2
{
    public class Program : GameWindow
    {
        public Shader _shader;
        int VertexBufferObject;
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
            VertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, blockIndices.Length * sizeof(float), blockIndices, BufferUsageHint.StaticDraw);

            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            int VertexArrayObject = GL.GenVertexArray();
            _shader.Use();
            GL.BindVertexArray(VertexArrayObject);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, blockIndices.Length * sizeof(float), blockIndices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

            // 3. now draw the object


        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit);

            //Code goes here.

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
        #region GeometryPrimitives
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
        #endregion

    }
}