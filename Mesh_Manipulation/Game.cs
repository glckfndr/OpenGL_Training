using GlmNet;
using OpenGLHelper;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using System;

namespace Mesh_Manipulation
{
    public class Game : GameWindow
    {
        private string _type = "t";
        private Shader _shader;
        private ArrayBuffer _elementBufferObject;
        private int _vertexArrayObject;
        //private int _vertexBufferObject;
        private ArrayBuffer _vertexBufferObject;
        private mat4 _view;
        private mat4 _model;
        private mat4 _projection;
        //private (float[] vertices, uint[] indices) plain;
        private float time = 0.0f;
        private float _angle = 0.0f;
        private Plane _plane;

        public Game(int width, int height, string title) :
            base(width, height, GraphicsMode.Default, title)
        {
        }

        protected override void OnLoad(EventArgs e)
        {
            GL.ClearColor(0.5f, 0.5f, 0.5f, 1.0f);
            GL.Enable(EnableCap.DepthTest);

            //  plain = Mesh.PlainXY(new Vector3(0, 0, 0), 12.0f, 0.1f, 200, 200);
            _plane = new Plane(13.0f, 10.0f, 200, 200);

            _shader = new Shader("../../shader.vert", "../../shader.frag");

            _shader.SetVector3("Material.Kd", new Vector3(0.9f, 0.5f, 0.3f));
            _shader.SetVector3("Material.Ks", new Vector3(0.8f, 0.8f, 0.8f));
            _shader.SetVector3("Material.Ka", new Vector3(0.2f, 0.2f, 0.2f));
            _shader.SetFloat("Material.Shininess", 100.0f);

            _shader.SetVector3("Light.Intensity", new Vector3(1.0f, 1.0f, 1.0f));
            _shader.SetVector4("Light.Position", new Vector4(0.0f, 2.0f, 0.0f, 1));
            _angle = (float)Math.PI / 2;

            _shader.SetFloat("Velocity", 0.08f);
            _shader.SetFloat("Amp", 0.5f);
            _shader.SetFloat("Freq", 2.0f);

            //_vertexArrayObject = GL.GenVertexArray();
            //GL.BindVertexArray(_vertexArrayObject);

            //_vertexBufferObject = new ArrayBuffer(BufferUsageHint.StaticDraw);
            //_vertexBufferObject.SetData(plain.vertices);
            //_vertexBufferObject.SetAttribPointer(_shader, "VertexPosition", 3, 0);

            //_elementBufferObject = new ArrayBuffer(BufferUsageHint.StaticDraw, BufferTarget.ElementArrayBuffer);
            //_elementBufferObject.SetData(plain.indices);

            //GL.BindVertexArray(0);

            base.OnLoad(e);
        }



        protected override void OnRenderFrame(FrameEventArgs e)
        {
            // GL.Clear(ClearBufferMask.ColorBufferBit);
            time += 0.5f;
            switch (_type)
            {

                case "t":
                    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                    _shader.SetFloat("Time", time);
                    _view = glm.lookAt(new vec3(10.0f * glm.cos(_angle), 4.0f, 10.0f * glm.sin(_angle)),
                                   new vec3(0.0f, 0.0f, 0.0f), new vec3(0.0f, 1.0f, 0.0f));
                    _projection = glm.perspective(glm.radians(60.0f), (float)Width / Height, 0.3f, 100.0f);
                    _model = new mat4(1.0f);
                    _model = glm.rotate(_model, glm.radians(-10.0f), new vec3(0.0f, 0.0f, 1.0f));
                    _model = glm.rotate(_model, glm.radians(50.0f), new vec3(1.0f, 0.0f, 0.0f));
                    SetMatrices();
                    _plane.Render();
                    //GL.BindVertexArray(_vertexArrayObject);
                    //GL.DrawElements(PrimitiveType.Triangles, plain.indices.Length, 
                    //  DrawElementsType.UnsignedInt, 0);

                    break;
                case "r":
                    //_shader.Use();
                    //_shader.SetFloat("Time", time);
                    //GL.BindVertexArray(_vertexArrayObject);
                    //GL.DrawElements(PrimitiveType.Triangles, plain.indices.Length, DrawElementsType.UnsignedInt, 0);

                    break;

            }

            Context.SwapBuffers();
            base.OnRenderFrame(e);
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

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            _projection = glm.perspective(glm.radians(60.0f), (float)Width / Height, 0.3f, 100.0f);
            base.OnResize(e);
        }



        private void SetMatrices()
        {
            mat4 mv = _view  * _model   ;
            _shader.SetMatrix4("ModelViewMatrix", mv.ConvertToMatrix4());
            _shader.SetMatrix3("NormalMatrix", (new mat3(new vec3(mv[0]), new vec3(mv[1]), new vec3(mv[2]))).ConvertToMatrix3());
            _shader.SetMatrix4("MVP", (_projection * mv).ConvertToMatrix4());
        }


        protected override void OnUnload(EventArgs e)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);
            _plane.DeleteBuffers();
            //GL.DeleteBuffer(_vertexBufferObject);
            // _vertexBufferObject.Destroy();
            // _elementBufferObject?.Destroy();
            // GL.DeleteVertexArray(_vertexArrayObject);
            _shader.Handle.Delete();
            base.OnUnload(e);
        }
    }
}
