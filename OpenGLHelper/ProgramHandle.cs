using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace OpenGLHelper
{
    public class ProgramHandle
    {
        private readonly int _handle;

        public ProgramHandle()
        {
            _handle = GL.CreateProgram();
        }

        public void Link()
        {
            // We link the program
            GL.LinkProgram(_handle);

            // Check for linking errors
            GL.GetProgram(_handle, GetProgramParameterName.LinkStatus, out var code);
            if (code != (int)All.True)
            {
                // We can use `GL.GetProgramInfoLog(program)` to get information about the error.
                throw new Exception($"Error occurred whilst linking Program({_handle})");
            }
        }

        public void Attach(params int[] shaders)
        {
            foreach (var shader in shaders)
            {
                GL.AttachShader(_handle, shader);
            }
            
        }

        public void ClearShaders(params int[] shaders)
        {
            foreach (var shader in shaders)
            {
                GL.DetachShader(_handle, shader);
            }

            foreach (var shader in shaders)
            {
                GL.DeleteShader(shader);
            }
        }

        public Dictionary<string, int> GetUniforms()
        {
            var uniformLocations = new Dictionary<string, int>();
            GL.GetProgram(_handle, GetProgramParameterName.ActiveUniforms, out var numberOfUniforms);
            // Loop over all the uniforms,
            for (var i = 0; i < numberOfUniforms; i++)
            {
                // get the name of this uniform,
                var key = GL.GetActiveUniform(_handle, i, out _, out _);

                // get the location,
                var location = GL.GetUniformLocation(_handle, key);

                // and then add it to the dictionary.
                uniformLocations.Add(key, location);
            }

            return uniformLocations;
        }

        public void Delete()
        {
            GL.DeleteProgram(_handle);
        }

        public void Use()
        {
            GL.UseProgram(_handle);
        }

        //The shader sources provided with this project use hardcoded layout(location)-s.If you want to do it dynamically,
        //you can omit the layout(location= X) lines in the vertex shader, and use this in VertexAttribPointer instead of the hardcoded values.
        public int GetAttribLocation(string attribName)
        {
            return GL.GetAttribLocation(_handle, attribName);
        }

    }
}
