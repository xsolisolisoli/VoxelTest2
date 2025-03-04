﻿using System;
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
using VoxelTest2.Entity;
using VoxelTest2.Entity.Components;
using VoxelTest2.Rendering;
using VoxelTest2.Entity.Systems;
using static VoxelTest2.Primitives.Geometry;
using VoxelTest2.Camera;
using Vector3 = OpenTK.Mathematics.Vector3;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Common.Input;

namespace VoxelTest2
{
    public class Program : GameWindow
    {
        private ComponentManager _componentManager;
        private Renderer _renderer;
        private Movement _movementSystem;
        private Chunk _chunk;
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
            Env.EnvSetup();
            runGame(new Program(800, 600, Environment.GetEnvironmentVariable("TITLE")));
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
            _componentManager = new ComponentManager();
            _renderer = new Renderer(_componentManager);
            _movementSystem = new Movement(_componentManager);
            _chunk = new Chunk(_componentManager);

            _camera = new Camera.Camera(Vector3.UnitZ * 3, Size.X / (float)Size.Y);
            // Initialize OpenGL settings
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            CursorState = CursorState.Grabbed;

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

            _renderer.Render();

            SwapBuffers();
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

            _movementSystem.Update((float)e.Time);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            // Check if the left mouse button was pressed
            if (e.Button == MouseButton.Left)
            {
                try
                {
                    // Calculate the center of the screen
                    Vector2 screenCenter = new Vector2(Size.X / 2, Size.Y / 2);

                    // Get the ray direction from the camera through the center of the screen
                    Vector3 rayDirection = _camera.ScreenToWorldRay(screenCenter, new Vector2(Size.X, Size.Y));
                    Vector3 rayOrigin = _camera.Position;

                    // Check if the ray intersects with any blocks in the chunk
                    if (chunk.RayIntersectsBlock(rayOrigin, rayDirection, out Vector3 hitPosition, out Block hitBlock))
                    {
                        // Output the block information to the console
                        Console.WriteLine($"Hit block at {hitPosition} with ID: {hitBlock.blockType}");

                        // Optional: Highlight the selected block or perform additional actions
                        HighlightBlock(hitPosition);
                    }
                    else
                    {
                        Console.WriteLine("No block hit.");
                    }
                }
                catch (Exception ex)
                {
                    // Handle any exceptions that occur
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
        }

        // Optional: Function to highlight the selected block
        private void HighlightBlock(Vector3 position)
        {
            // Implement block highlighting logic here
            // For example, change the block color or add a visual indicator
            Console.WriteLine($"Highlighting block at {position}");
        }
    }
}