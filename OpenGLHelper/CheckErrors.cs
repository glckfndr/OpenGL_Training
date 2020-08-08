using OpenTK.Graphics.OpenGL;
using System;

namespace OpenGLHelper
{
    public static class CheckErrors
    {
        public static bool OpenGl()
        {
            bool foundError = false;
            ErrorCode glError = GL.GetError();
            while (glError != ErrorCode.NoError)
            {
                Console.WriteLine("GlError: " + glError);
                foundError = true;
                glError = GL.GetError();
            }

            return foundError;
        }

        public static void LinkStatus(int program)
        {
            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out var code);
            if (code != (int)All.True)
            {
                int bufSize = 0;
                int length = 0;
                string infoLog;
                GL.GetProgram(program, GetProgramParameterName.InfoLogLength, out bufSize);
                if (bufSize > 0)
                {
                    GL.GetProgramInfoLog(program, bufSize, out length, out infoLog);
                    Console.WriteLine($"Program Info Log: {infoLog}");
                    throw new Exception($"Error occurred whilst linking Program({program}).\n\n{infoLog}");
                }
            }
        }

        public static void CompileStatus(int shader)
        {
            GL.GetShader(shader, ShaderParameter.CompileStatus, out var code);
            if (code != (int) All.True)
            {
                int bufSize = 0;
                int length = 0;
                string infoLog;
                GL.GetShader(shader, ShaderParameter.InfoLogLength, out bufSize);
                if (bufSize > 0)
                {
                    GL.GetShaderInfoLog(shader, bufSize, out length, out infoLog);
                    Console.WriteLine($"Shader Info Log: {infoLog}");
                    throw new Exception($"Error occurred whilst compiling Shader({shader}).\n\n{infoLog}");
                }
            }
        }
    }
}
