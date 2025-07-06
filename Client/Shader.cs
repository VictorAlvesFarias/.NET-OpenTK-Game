using Kingdom_of_Creation.Dtos;
using Kingdom_of_Creation.Entities;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Client
{
    public class Shader
    {
        public int Handle { get; private set; }
        private int VertexShader;
        private int FragmentShader;
        public Shader(string vertexPath, string fragmentPath)
        {
            string VertexShaderSource = File.ReadAllText(vertexPath);
            string FragmentShaderSource = File.ReadAllText(fragmentPath);

            VertexShader = GL.CreateShader(ShaderType.VertexShader);

            GL.ShaderSource(VertexShader, VertexShaderSource);
            GL.CompileShader(VertexShader);
            GL.GetShader(VertexShader, ShaderParameter.CompileStatus, out int vertexSuccess);

            FragmentShader = GL.CreateShader(ShaderType.FragmentShader);

            GL.ShaderSource(FragmentShader, FragmentShaderSource);
            GL.CompileShader(FragmentShader);
            GL.GetShader(FragmentShader, ShaderParameter.CompileStatus, out int fragmentSuccess);

            Handle = GL.CreateProgram();

            GL.AttachShader(Handle, VertexShader);
            GL.AttachShader(Handle, FragmentShader);
            GL.LinkProgram(Handle);
            GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out int linkSuccess);

            GL.DetachShader(Handle, VertexShader);
            GL.DetachShader(Handle, FragmentShader);
            GL.DeleteShader(FragmentShader);
            GL.DeleteShader(VertexShader);
        }

        public void Use()
        {
            GL.UseProgram(Handle);
        }
        public void SetMatrix4(string name, Matrix4 matrix)
        {
            int location = GL.GetUniformLocation(Handle, name);

            if (location == -1)
            {
                throw new FileNotFoundException($"Uniform {name} not found.");
            }

            GL.UniformMatrix4(location, false, ref matrix);
        }
        public void SetColor4(string name, Color_4 color)
        {
            int location = GL.GetUniformLocation(Handle, name);

            if (location == -1)
            {
                throw new FileNotFoundException($"Uniform {name} not found.");
            }

            GL.Uniform4(location, new Color4(color.R, color.G, color.B, color.A));
        }
    }
}
