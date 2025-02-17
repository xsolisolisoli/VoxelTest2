using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using VoxelTest2.Camera;
using Vector3 = OpenTK.Mathematics.Vector3;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace VoxelTest2
{
    public class Program : GameWindow
    {
        private Chunk chunk;
        private Camera.Camera _camera;
        public Shader _shader;
        private int _vertexBufferObject;
        private int _vertexArrayObject;
        private int _elementBufferObject;
        private int elementCount;
        private int _instanceBufferObject;
        private int _shaderProgram;
        private Matrix4 view;
        private Matrix4 projection;
        private Vector2 _lastPos;
        private bool _firstMove = true;
        private Matrix4[] instanceMatrices;

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
            //_shader.Dispose();
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            _camera = new Camera.Camera(Vector3.UnitZ * 3, Size.X / (float)Size.Y);
            // Initialize OpenGL settings
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            // Create and initialize chunk
            chunk = new Chunk();
            chunk.CreateMesh();

            initializeShaders();
            // Set up camera matrices
            view = Matrix4.LookAt(
                new Vector3(0, 0, 3), // Camera position
                new Vector3(0, 0, 0), // Camera target
                Vector3.UnitY);       // Up vector

            // Set up projection matrix (aspect ratio should match window size)
            projection = Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(45.0f),
                (float)Size.X / Size.Y,
                0.1f,
                100.0f);
        }
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _shader.Use();

            // Set up model-view-projection matrix
            var model = Matrix4.Identity; // Adjust this if you want transformations
            var mvp = model * _camera.GetViewMatrix() * _camera.GetProjectionMatrix();

            //_shader.SetMatrix4("mvp", mvp);
            _shader.SetMatrix4("model", model);
            _shader.SetMatrix4("view", _camera.GetViewMatrix());
            _shader.SetMatrix4("projection", _camera.GetProjectionMatrix());


            chunk.Render();

            SwapBuffers();
        }


        protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
        {
            base.OnFramebufferResize(e);
            GL.Viewport(0, 0, e.Width, e.Height);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            if (!IsFocused) // Check to see if the window is focused
            {
                return;
            }

            var input = KeyboardState;

            if (input.IsKeyDown(Keys.Escape))
            {
                Close();
            }

            const float cameraSpeed = 1.5f;
            const float sensitivity = 0.2f;

            if (input.IsKeyDown(Keys.W))
            {
                _camera.Position += _camera.Front * cameraSpeed * (float)e.Time; // Forward
            }

            if (input.IsKeyDown(Keys.S))
            {
                _camera.Position -= _camera.Front * cameraSpeed * (float)e.Time; // Backwards
            }
            if (input.IsKeyDown(Keys.A))
            {
                _camera.Position -= _camera.Right * cameraSpeed * (float)e.Time; // Left
            }
            if (input.IsKeyDown(Keys.D))
            {
                _camera.Position += _camera.Right * cameraSpeed * (float)e.Time; // Right
            }
            if (input.IsKeyDown(Keys.Space))
            {
                _camera.Position += _camera.Up * cameraSpeed * (float)e.Time; // Up
            }
            if (input.IsKeyDown(Keys.LeftShift))
            {
                _camera.Position -= _camera.Up * cameraSpeed * (float)e.Time; // Down
            }

            // Get the mouse state
            var mouse = MouseState;

            if (_firstMove) // This bool variable is initially set to true.
            {
                _lastPos = new OpenTK.Mathematics.Vector2(mouse.X, mouse.Y);
                _firstMove = false;
            }
            else
            {
                // Calculate the offset of the mouse position
                var deltaX = mouse.X - _lastPos.X;
                var deltaY = mouse.Y - _lastPos.Y;
                _lastPos = new Vector2(mouse.X, mouse.Y);

                // Apply the camera pitch and yaw (we clamp the pitch in the camera class)
                _camera.Yaw += deltaX * sensitivity;
                _camera.Pitch -= deltaY * sensitivity; // Reversed since y-coordinates range from bottom to top
            }
        }
        public   void UploadToGPU(List<Vertex> vertices, List<uint> indices)
        {
            // Delete previous buffers if they exist
            Dispose();

            // Create vertex array
            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);

            // Create and upload vertex buffer
            _vertexBufferObject = GL.GenBuffer(); // Corrected line
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Count * Vertex.SizeInBytes,
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
                                 Vertex.SizeInBytes, 0);

            GL.EnableVertexAttribArray(1); // Normal
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false,
                                 Vertex.SizeInBytes, 3 * sizeof(float));

            GL.EnableVertexAttribArray(2); // Color
            GL.VertexAttribPointer(2, 4, VertexAttribPointerType.Float, false,
                                 Vertex.SizeInBytes, 6 * sizeof(float));

            GL.BindVertexArray(0);
        }
        public void initializeShaders()
        {
            _shader = new Shader("shading/shaders/shader.vert", "shading/shaders/shader.frag");
        }
    }
}