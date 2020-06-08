using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace OpenGLHelper
{
    // A simple class meant to help create shaders.
    public class Shader
    {
        public readonly ProgramHandle Handle;

        private readonly Dictionary<string, int> _uniformLocations;

        // This is how you create a simple shader.
        // Shaders are written in GLSL, which is a language very similar to C in its semantics.
        // The GLSL source is compiled *at runtime*, so it can optimize itself for the graphics card it's currently being used on.
        // A commented example of GLSL can be found in shader.vert
        public Shader(string vertPath, string fragPath)
        {
            var vertexShader = CreateShader(vertPath, ShaderType.VertexShader);
            CompileShader(vertexShader);

            var fragmentShader = CreateShader(fragPath, ShaderType.FragmentShader);
            CompileShader(fragmentShader);
            
            Handle = new ProgramHandle();
            Handle.Attach(vertexShader, fragmentShader);
            Handle.Link();
            Handle.ClearShaders(vertexShader, fragmentShader);
            _uniformLocations =  Handle.GetUniforms();
        }
        
        private static int CreateShader(string shaderPath, ShaderType type)
        {
            // LoadSource is a simple function that just loads all text from the file whose path is given.
            var shaderSource = LoadSource(shaderPath);

            // GL.CreateShader will create an empty shader (obviously). The ShaderType enum denotes which type of shader will be created.
            var shader = GL.CreateShader(type);

            // Now, bind the GLSL source code
            GL.ShaderSource(shader, shaderSource);
            return shader;
        }

        private static void CompileShader(int shader)
        {
            // Try to compile the shader
            GL.CompileShader(shader);

            // Check for compilation errors
            GL.GetShader(shader, ShaderParameter.CompileStatus, out var code);
            if (code != (int)All.True)
            {
                // We can use `GL.GetShaderInfoLog(shader)` to get information about the error.
                var infoLog = GL.GetShaderInfoLog(shader);
                throw new Exception($"Error occurred whilst compiling Shader({shader}).\n\n{infoLog}");
            }
        }
        
        // A wrapper function that enables the shader program.
        public void Use()
        {
            Handle.Use();
        }
        
        // Just loads the entire file into a string.
        private static string LoadSource(string path)
        {
            using (var sr = new StreamReader(path, Encoding.UTF8))
            {
                return sr.ReadToEnd();
            }
        }
        
        /// <summary>
        /// Set a uniform int on this shader.
        /// </summary>
        /// <param name="name">The name of the uniform</param>
        /// <param name="data">The data to set</param>
        public void SetInt(string name, int data)
        {
            Handle.Use();
            GL.Uniform1(_uniformLocations[name], data);
        }

        /// <summary>
        /// Set a uniform float on this shader.
        /// </summary>
        /// <param name="name">The name of the uniform</param>
        /// <param name="data">The data to set</param>
        public void SetFloat(string name, float data)
        {
            Handle.Use();
            GL.Uniform1(_uniformLocations[name], data);
        }

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
            Handle.Use();
            GL.UniformMatrix4(_uniformLocations[name], true, ref data);
        }

        /// <summary>
        /// Set a uniform Vector3 on this shader.
        /// </summary>
        /// <param name="name">The name of the uniform</param>
        /// <param name="data">The data to set</param>
        public void SetVector3(string name, Vector3 data)
        {
            Handle.Use();
            GL.Uniform3(_uniformLocations[name], data);
        }

        public void SetVector4(string name, Vector4 data)
        {
            Handle.Use();
            GL.Uniform4(_uniformLocations[name], data);
        }

        public int GetAttribLocation(string attribName)
        {
            return Handle.GetAttribLocation(attribName);
        }

    }
}
