﻿using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OpenGLHelper
{
    // A simple class meant to help create shaders.
    public class Shader
    {
        public readonly GLSLProgram Handle;

        private readonly Dictionary<string, int> _uniformLocations;

        // This is how you create a simple shader.
        // Shaders are written in GLSL, which is a language very similar to C in its semantics.
        // The GLSL source is compiled *at runtime*, so it can optimize itself for the graphics card it's currently being used on.
        // A commented example of GLSL can be found in shader.vert
        public Shader(string vertexShaderPath, string fragmentShaderPath)
        {
            var vertexShader = CreateShaderFromFile(vertexShaderPath, ShaderType.VertexShader);
            CompileShader(vertexShader);

            var fragmentShader = CreateShaderFromFile(fragmentShaderPath, ShaderType.FragmentShader);
            CompileShader(fragmentShader);

            Handle = new GLSLProgram();
            Handle.Attach(vertexShader, fragmentShader);
            Handle.Link();
            Handle.ClearShaders(vertexShader, fragmentShader);
            _uniformLocations = Handle.GetUniforms();
        }

        public Shader(string vertexShaderPath, string fragmentShaderPath, string[] feedbackVariables)
        {
            var vertexShader = CreateShaderFromFile(vertexShaderPath, ShaderType.VertexShader);
            CompileShader(vertexShader);

            var fragmentShader = CreateShaderFromFile(fragmentShaderPath, ShaderType.FragmentShader);
            CompileShader(fragmentShader);

            Handle = new GLSLProgram();
            Handle.Attach(vertexShader, fragmentShader);

            Handle.TransformFeedbackVaryings(feedbackVariables);

            Handle.Link();
            Handle.ClearShaders(vertexShader, fragmentShader);
            _uniformLocations = Handle.GetUniforms();
        }



        public Shader(string computeShaderPath)
        {
            var computeShader = CreateShaderFromFile(computeShaderPath, ShaderType.ComputeShader);
            CompileShader(computeShader);

            Handle = new GLSLProgram();
            Handle.Attach(computeShader);

            Handle.Link();
            Handle.ClearShaders(computeShader);
            _uniformLocations = Handle.GetUniforms();
        }



        public static int CreateShaderFromFile(string shaderPath, ShaderType type)
        {
            // LoadSource is a simple function that just loads all text from the file whose path is given.
            var shaderSource = LoadSource(shaderPath);

            // GL.CreateShaderFromFile will create an empty shader (obviously). The ShaderType enum denotes which type of shader will be created.
            var shader = GL.CreateShader(type);

            // Now, bind the GLSL source code
            GL.ShaderSource(shader, shaderSource);
            return shader;
        }

        public static void CompileShader(int shader)
        {
            // Try to compile the shader
            GL.CompileShader(shader);
            // Check for OpenGl errors
            CheckErrors.OpenGl();
            // Check for compilation errors
            CheckErrors.CompileStatus(shader);

        }

        // A wrapper function that enables the shader program.
        public void Use()
        {
            Handle.Use();
        }

        public void Compute(int num_groups_x, int num_groups_y, int num_groups_z, 
            MemoryBarrierFlags flag = MemoryBarrierFlags.ShaderStorageBarrierBit)
        {
            Use();
            GL.DispatchCompute(num_groups_x, num_groups_y, num_groups_z);
            GL.MemoryBarrier(flag);
        }

        // Just loads the entire file into a string.
        private static string LoadSource(string path)
        {
            using (var streamReader = new StreamReader(path, Encoding.UTF8))
            {
                return streamReader.ReadToEnd();
            }
        }

        /// <summary>
        /// Set a uniform int on this shader.
        /// </summary>
        /// <param name="name">The name of the uniform</param>
        /// <param name="data">The data to set</param>
        public void SetInt(string name, int data)
        {
            Handle.SetInt(name, data);
        }

        /// <summary>
        /// Set a uniform float on this shader.
        /// </summary>
        /// <param name="name">The name of the uniform</param>
        /// <param name="data">The data to set</param>
        public void SetFloat(string name, float data)
        {
            Handle.SetFloat(name, data);
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
            Handle.SetMatrix4(name, data);
        }

        public void SetMatrix3(string name, Matrix3 data)
        {
            Handle.SetMatrix3(name, data);
        }

        public void SetMvpMatrix(Matrix4 model, Matrix4 view, Matrix4 projection)
        {
            SetMatrix4("model", model);
            SetMatrix4("view", view);
            SetMatrix4("projection", projection);
        }

        /// <summary>
        /// Set a uniform Vector3 on this shader.
        /// </summary>
        /// <param name="name">The name of the uniform</param>
        /// <param name="data">The data to set</param>
        public void SetVector3(string name, Vector3 data)
        {
            Handle.SetVector3(name, data);
        }

        public void SetVector4(string name, Vector4 data)
        {
            Handle.SetVector4(name, data);
        }

        public int GetAttributeLocation(string attributeName)
        {

            return Handle.GetAttribLocation(attributeName);

        }

        public int GetSubroutineIndex(ShaderType shaderType, string subroutineName)
        {
            
            return Handle.GetSubroutineIndex(shaderType, subroutineName);
        }

        public void SetSubroutineIndex(int subroutineIndex, int count)
        {
            GL.UniformSubroutines(ShaderType.VertexShader, count, ref subroutineIndex);
            //return Handle.GetSubroutineIndex(shaderType, subroutineName);
        }



    }
}
