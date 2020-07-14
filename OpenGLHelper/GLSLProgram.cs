using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;

namespace OpenGLHelper
{
    public class GLSLProgram
    {
        private readonly int _handle;
        private Dictionary<string, int> _uniformLocations;

        public GLSLProgram()
        {
            _handle = GL.CreateProgram();
        }

        public int GetHandle()
        {
            return _handle;
        }

        public void Link()
        {
            // We link the program
            GL.LinkProgram(_handle);
            CheckErrors.OpenGl();
            // Check for linking errors
            CheckErrors.LinkStatus(_handle);

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
            _uniformLocations = new Dictionary<string, int>();
            GL.GetProgram(_handle, GetProgramParameterName.ActiveUniforms, out var numberOfUniforms);
            // Loop over all the uniforms,
            for (var i = 0; i < numberOfUniforms; i++)
            {
                // get the name of this uniform,
                var key = GL.GetActiveUniform(_handle, i, out _, out _);

                // get the location,
                var location = GL.GetUniformLocation(_handle, key);

                // and then add it to the dictionary.
                _uniformLocations.Add(key, location);
            }

            return _uniformLocations;
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

        public void SetInt(string name, int data)
        {
            Use();
            GL.Uniform1(_uniformLocations[name], data);
        }

        /// <summary>
        /// Set a uniform float on this shader.
        /// </summary>
        /// <param name="name">The name of the uniform</param>
        /// <param name="data">The data to set</param>
        public void SetFloat(string name, float data)
        {
            Use();
            GL.Uniform1(_uniformLocations[name], data);
        }

        //public void SetBool(string name, bool data)
        //{
        //    Use();
        //    GL.Uniform1(_uniformLocations[name], data);
        //}

        /// <summary>
        /// Set a uniform Matrix4 on this shader
        /// </summary>
        /// <param name="name">The name of the uniform</param>
        /// <param name="data">The data to set</param>
        /// <remarks>
        ///   <para>
        ///   The matrix is transposed before being sent to the shader.
        ///   </para>
        /// </remarks>
        public void SetMatrix4(string name, Matrix4 data)
        {
            Use();
            GL.UniformMatrix4(_uniformLocations[name], true, ref data);
        }

        public void SetMatrix3(string name, Matrix3 data)
        {
            Use();
            GL.UniformMatrix3(_uniformLocations[name], true, ref data);
        }

        /// <summary>
        /// Set a uniform Vector3 on this shader.
        /// </summary>
        /// <param name="name">The name of the uniform</param>
        /// <param name="data">The data to set</param>
        public void SetVector3(string name, Vector3 data)
        {
            Use();
            GL.Uniform3(_uniformLocations[name], data);
        }

        public void SetVector4(string name, Vector4 data)
        {
            Use();
            GL.Uniform4(_uniformLocations[name], data);
        }
    }
}
