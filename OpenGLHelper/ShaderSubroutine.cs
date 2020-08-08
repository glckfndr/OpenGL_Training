using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace OpenGLHelper
{
    public class ShaderSubroutine
    {
        private int _index;
        private ShaderType _shaderType;
        public ShaderSubroutine(Shader shader, ShaderType shaderType , string subroutineName)
        {
            _shaderType = shaderType;
            _index = shader.GetSubroutineIndex(shaderType, subroutineName);
        }

        public void Use()
        {
            GL.UniformSubroutines(_shaderType, 1, ref _index);
        }
    }
}
