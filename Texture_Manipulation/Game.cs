using OpenGLHelper;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using System;
using System.Threading;

namespace Texture_Manipulation
{
    public class Game : GameWindow
    {
        private string _type = "t";
        private Shader[] _shader = new Shader[2];
        private int[] _elementBufferObject = new int[2];
        private int[] _vertexArrayObject = new int[2];
        private int[] _vertexBufferObject = new int[2];
        private int _colorBufferObject;
        private Texture _texture;
        private (float[] vertices, uint[] indices) plain;
        private (float[] vertices, uint[] indices) triangle;
        private float time = 0.0f;
        private ShapeGL trn;
        private ShapeGL trn1;
        private ShapeGL txtr;
        private Vector3 _center0 = new Vector3(0.5f,0.0f, 0);
        private Vector3 _center1 = new Vector3(-0.5f, 0.0f, 0);
        private Vortex vortex0;
        private Vortex vortex1;


        float[] colorData =
        {
            1.0f, 0.0f, 1.0f,
            1.0f, 1.0f, 0.0f,
            0.0f, 1.0f, 1.0f,
          
        };

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
            float dt = 0.03f;
            time += dt;
            _center0 = vortex0.GetCenter(dt, vortex1.velocity(_center0));
            _center1 = vortex1.GetCenter(dt, vortex0.velocity(_center1));



            switch (_type)
            {

                case "t":
                    
                    //_texture.Use();
                    //_shader[0].Use();

                    _shader[0].SetVector3("center0", _center0);
                    _shader[0].SetVector3("center1", _center1);

                    //_shader.SetFloat("Time", time);
                    //GL.BindVertexArray(_vertexArrayObject[0]);
                    //GL.DrawElements(PrimitiveType.Triangles, plain.indices.Length, DrawElementsType.UnsignedInt, 0);
                    txtr.Use();

                    // _shader[1].Use();
                    // GL.BindVertexArray(_vertexArrayObject[1]);
                    // GL.DrawElements(PrimitiveType.Triangles, triangle.indices.Length, DrawElementsType.UnsignedInt, 0);
                    _shader[1].SetFloat("yCoord", 0.5f * (float)Math.Cos(2 * time));
                    trn.Use();

                    _shader[1].SetFloat("yCoord", 0.5f*(float)Math.Sin(2*time));
                    trn1.Use();

                    break;
                case "r":
                    //_texture.Use();
                    //_shader[0].Use();
                    //_shader[0].SetFloat("Time", time);
                    _shader[0].SetVector3("center0", _center0);
                    _shader[0].SetVector3("center1", _center1);
                    //GL.BindVertexArray(_vertexArrayObject[0]);
                    //GL.DrawElements(PrimitiveType.Triangles, plain.indices.Length, DrawElementsType.UnsignedInt, 0);
                    txtr.Use();

                    break;
            }

            _texture.CopyTexture();
           // Thread.Sleep(500);
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
            vortex0 = new Vortex(_center0, new Vector3(0,0,0.04f) );
            vortex1 = new Vortex(_center1, new Vector3(0, 0, -0.04f));
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            _texture = new Texture("../../Resources/container.png");
            //_texture.Use();

            _shader[0] = new Shader("../../shader.vert", "../../shader.frag");
            _shader[0].Use();

            plain = Mesh.PlainXYUV(new Vector3(0, 0, 0), 2.0f, 2.0f, 32, 32);

           // _vertexArrayObject[0] = GL.GenVertexArray();
           // GL.BindVertexArray(_vertexArrayObject[0]);

            //string[] names = { "VertexPosition", "TexturePosition" };
            //int[] size = { 3, 2 };
            //_vertexBufferObject[0] = CopyData.ToArrayBufferForTexture(plain.vertices, _shader[0], names, size);
            //_elementBufferObject[0] = CopyData.ToElementBuffer(plain.indices);
            txtr = new ShapeGL(_shader[0], plain, _texture);

            _shader[1] = new Shader("../../Shaders/shader.vert", "../../Shaders/shader.frag");
            //_shader[1].Use();

            triangle = Mesh.TriangleXYUV(new Vector2(-0.5f, -0.1f), 0.25f);
            trn = new ShapeGL(_shader[1], triangle, colorData);

            triangle = Mesh.TriangleXYUV(new Vector2(0.5f, -0.1f), 0.25f);
            colorData[3] = 1;
            colorData[4] = 0;
            colorData[5] = 0;
            trn1 = new ShapeGL(_shader[1], triangle, colorData);
            //_vertexArrayObject[1] = GL.GenVertexArray();
            //GL.BindVertexArray(_vertexArrayObject[1]);
            //_vertexBufferObject[1] = CopyData.ToArrayBufferForTexture(triangle.vertices, _shader[1], "VertexPosition", 3);
            //_colorBufferObject = CopyData.ToArrayBufferForTexture(colorData, _shader[1], "VertexColor", 3);
            //_elementBufferObject[1] = CopyData.ToElementBuffer(triangle.indices);


            base.OnLoad(e);
        }

        protected override void OnUnload(EventArgs e)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            //GL.DeleteBuffer(_vertexBufferObject[0]);
            //GL.DeleteBuffer(_elementBufferObject[0]);
            //GL.DeleteBuffer(_vertexBufferObject[1]);
            //GL.DeleteBuffer(_elementBufferObject[1]);
            //GL.DeleteVertexArray(_vertexArrayObject[0]);
            //GL.DeleteVertexArray(_vertexArrayObject[1]);
            trn.DeleteBuffers();
            txtr.DeleteBuffers();

            
            _shader[0].Handle.Delete();
            _shader[1].Handle.Delete();

            base.OnUnload(e);
        }
    }
}
