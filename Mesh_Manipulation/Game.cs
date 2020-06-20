﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGLHelper;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;

namespace Mesh_Manipulation
{
    public class Game : GameWindow
    {
        private string _type = "t";
        private Shader _shader;
        int _elementBufferObject;
        int _vertexArrayObject;
        int _vertexBufferObject;
        private Matrix4 _view;
        private Matrix4 _projection;

        (float[] vertices, uint[] indices) plain;
        float time = 0.0f;

        public Game(int width, int height, string title) :
            base(width, height, GraphicsMode.Default, title)
        {
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            KeyboardState input = Keyboard.GetState();
            if (input.IsKeyDown(Key.Escape))
            {
                Exit();
            }
            else if (input.IsKeyDown(Key.T))
            {
                _type = "t";

            }
            else if (input.IsKeyDown(Key.R))
            {
                _type = "r";

            }

            base.OnUpdateFrame(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            time += 0.03f;
            switch (_type)
            {

                case "t":
                    _shader.Use();
                    _shader.SetFloat("Time", time);
                    var model = Matrix4.Identity * Matrix4.CreateRotationY((float)MathHelper.DegreesToRadians(time*10));
                    _shader.SetMatrix4("model", model);
                    GL.BindVertexArray(_vertexArrayObject);
                    GL.DrawElements(PrimitiveType.Triangles, plain.indices.Length, DrawElementsType.UnsignedInt, 0);

                    break;
                case "r":
                    _shader.Use();
                    _shader.SetFloat("Time", time);
                    GL.BindVertexArray(_vertexArrayObject);
                    GL.DrawElements(PrimitiveType.Triangles, plain.indices.Length, DrawElementsType.UnsignedInt, 0);

                    break;

            }

            Context.SwapBuffers();
            base.OnRenderFrame(e);
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            base.OnResize(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            
            plain = Mesh.PlainXY(new Vector3(0, 0, 0), 2.0f, 0.1f, 200, 200);

            _shader = new Shader("../../shader.vert", "../../shader.frag");

            _shader.SetFloat("Velocity", 0.04f);
            _shader.SetFloat("Amp", 0.3f);
            _shader.SetFloat("Freq", 2.0f);
            
            _view = Matrix4.CreateTranslation(0.0f, 0.0f, -3.0f);
            _shader.SetMatrix4("view", _view);

            _projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f), Width / (float)Height, 0.1f, 100.0f);
            _shader.SetMatrix4("projection", _projection);

            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);

            _vertexBufferObject = CopyData.ToArrayBuffer(plain.vertices, _shader, "VertexPosition", 3);
            _elementBufferObject = CopyData.ToElementBuffer(plain.indices);
            
            ////uniform vec4 LightPosition;
            //shader.SetVector4("LightPosition", new Vector4(0, 3, 1, 0));
            ////uniform vec3 LightIntensity;
            //shader.SetVector3("LightIntensity", new Vector3(1, 1, 1));
            ////uniform vec3 Kd; // коеф відбиття розсіяного світла
            //shader.SetVector3("Kd", new Vector3(0.4f, 0.4f, 0.4f));
            ////uniform vec3 Ka; // коеф відбиття розсіяного світла
            //shader.SetVector3("Ka", new Vector3(0.4f, 0.4f, 0.4f));
            ////uniform vec3 Ks; // коеф зеркального відбиття
            //shader.SetVector3("Ks", new Vector3(0.4f, 0.4f, 0.4f));
            ////uniform float Shininess; // показник ступеня зеркального відбиття
            //shader.SetFloat("Shininess", 0.5f);
            

            base.OnLoad(e);
        }

        protected override void OnUnload(EventArgs e)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            GL.DeleteBuffer(_vertexBufferObject);
            GL.DeleteBuffer(_elementBufferObject);
            GL.DeleteVertexArray(_vertexArrayObject);
            _shader.Handle.Delete();
            base.OnUnload(e);
        }
    }
}
