﻿using System;
using OpenGLHelper;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace OpenGL_Training
{
    public class Game: GameWindow
    {
        private Matrix4 _rotation;
        Matrix4 trans = Matrix4.CreateTranslation(0.1f, 0.1f, 0.0f);
        private int _vertexBufferObject; 
        private int _colorBufferObject;
        private int _vertexArrayObject;
        private int _elementBufferObject;
        private Shader _shader;
        private int location;

        float[] verticesDataTriangle =
        {
            -0.5f, -0.5f, 0.0f, //Bottom-left vertex
            0.5f, -0.5f, 0.0f, //Bottom-right vertex
            0.0f,  0.5f, 0.0f  //Top vertex
        };

        private readonly float[] verticesData =
        {
            0.5f,  0.5f, 0.0f, // top right
            0.5f, -0.5f, 0.0f, // bottom right
            -0.5f, -0.5f, 0.0f, // bottom left
            -0.5f,  0.5f, 0.0f, // top left
        };

        float[] colorData = 
        {
            1.0f, 0.0f, 0.0f,
            0.0f, 1.0f, 0.0f,
            0.0f, 1.0f, 1.0f,
            0.0f, 0.0f, 1.0f
        };

        private readonly uint[] _indices =
        {
            // Note that indices start at 0!
            0, 1, 3, // The first triangle will be the bottom-right half of the triangle
            1, 2, 3  // Then the second will be the top-right half of the triangle
        };

        private float _angle;

        public Game(int width, int height, string title) : base(width, height, GraphicsMode.Default, title)
        {

        }

        protected override void OnLoad(EventArgs e)
        {
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);

            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);

            _vertexBufferObject = CopyDataToBuffer(verticesData, 0);
            _colorBufferObject = CopyDataToBuffer(colorData, 1);
            _elementBufferObject = CopyElementDataToBuffer(_indices);
            
            _shader = new Shader("../../shader.vert", "../../shader.frag");
            _shader.Use();
            _shader.SetVector4("blob.InnerColor", new Vector4(1.0f, 0.0f, 0.0f, 1));
            _shader.SetVector4("blob.OuterColor", new Vector4(0.0f, 0.0f, 0.0f, 1));
            _shader.SetFloat("blob.RadiusInner", 0.05f);
            _shader.SetFloat("blob.RadiusOuter", 0.15f);
            
            base.OnLoad(e);
        }

        private int CopyDataToBuffer(float[] data, int index)
        {
            int handle = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, handle);
            GL.BufferData(BufferTarget.ArrayBuffer, data.Length * sizeof(float), data, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, handle);
            GL.VertexAttribPointer(index, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            return handle;
        }

        private int CopyElementDataToBuffer(uint[] data)
        {
            int handle = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, handle);
            GL.BufferData(BufferTarget.ElementArrayBuffer, data.Length * sizeof(uint), data, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, handle);
            return handle;
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            KeyboardState input = Keyboard.GetState();

            if (input.IsKeyDown(Key.Escape))
            {
                Exit();
            }

            base.OnUpdateFrame(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            _shader.Use();
            _angle += 0.1f;

            _rotation = Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(_angle));
            _shader.SetMatrix4("RotationMatrix", _rotation);
            GL.BindVertexArray(_vertexArrayObject);
            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
            //GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

            Context.SwapBuffers();
            base.OnRenderFrame(e);
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            base.OnResize(e);
        }

        protected override void OnUnload(EventArgs e)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            // Delete all the resources.
            GL.DeleteBuffer(_vertexBufferObject);
            GL.DeleteBuffer(_colorBufferObject);
            GL.DeleteBuffer(_elementBufferObject);
            GL.DeleteVertexArray(_vertexArrayObject);

            _shader.Handle.Delete();
            base.OnUnload(e);
        }

        
    }
}