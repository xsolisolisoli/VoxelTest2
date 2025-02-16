using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;


namespace VoxelTest2.Shading {
    public class Shader : IDisposable
    {
        private int handle;

        public Shader(string vertPath, string fragPath)
        {
            // Load and compile shaders
            var vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, File.ReadAllText(vertPath));
            CompileShader(vertexShader);

            var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, File.ReadAllText(fragPath));
            CompileShader(fragmentShader);

            // Create program
            handle = GL.CreateProgram();
            GL.AttachShader(handle, vertexShader);
            GL.AttachShader(handle, fragmentShader);
            LinkProgram(handle);

            // Cleanup
            GL.DetachShader(handle, vertexShader);
            GL.DetachShader(handle, fragmentShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);
        }

        public void Use()
        {
            GL.UseProgram(handle);
        }

        public void SetMatrix4(string name, Matrix4 matrix)
        {
            var location = GL.GetUniformLocation(handle, name);
            GL.UniformMatrix4(location, true, ref matrix);
        }

        private static void CompileShader(int shader)
        {
            GL.CompileShader(shader);
            GL.GetShader(shader, ShaderParameter.CompileStatus, out var code);
            if (code != (int)All.True)
                throw new Exception($"Shader compilation error: {GL.GetShaderInfoLog(shader)}");
        }

        private static void LinkProgram(int program)
        {
            GL.LinkProgram(program);
            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out var code);
            if (code != (int)All.True)
                throw new Exception($"Program link error: {GL.GetProgramInfoLog(program)}");
        }

        public void Dispose()
        {
            GL.DeleteProgram(handle);
            GC.SuppressFinalize(this);
        }
    }
}
