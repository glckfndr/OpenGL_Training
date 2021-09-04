using OpenGLHelper;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using System;
using System.Drawing;

namespace Texture_Manipulation
{
    public class Game : GameWindow
    {
        private string _type = "t";
        private Shader _textureShader;
        private Shader _colorShader;
        private int[] _elementBufferObject = new int[2];
        private int[] _vertexArrayObject = new int[2];
        private int[] _vertexBufferObject = new int[2];
        private int _colorBufferObject;
        private Texture2D _texture;
        private (float[] vertices, uint[] indices) plain;
        private (float[] vertices, uint[] indices) triangle;
        private float time = 0.0f;
        private VertexObject _colorTriangle1;
        private VertexObject _colorTriangle2;
        private VertexObject _textureRectangle;
        private Vector3 _center0 = new Vector3(0.15f, -1.0f, 0);
        private Vector3 _center1 = new Vector3(-0.15f, -1.0f, 0);
        private Vortex vortex0;
        private Vortex vortex1;
        private float[] colorData =
        {
            1.0f, 0.0f, 1.0f,
            1.0f, 1.0f, 0.0f,
            0.0f, 1.0f, 1.0f,

        };

        private Point _mouse;
        private float _gamma = 0.03f;

        private bool _isTriangle = false;

        public Game(int width, int height, string title) :
            base(width, height, GraphicsMode.Default, title)
        {
        }

        protected override void OnLoad(EventArgs e)
        {
            vortex0 = new Vortex(_center0, new Vector3(0, 0, _gamma));
            vortex1 = new Vortex(_center1, new Vector3(0, 0, -_gamma));
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            _texture = new Texture2D("../../Resources/water2.jpg");
            this.ClientSize = _texture.GetSize();

            _textureShader = new Shader("../../Shaders/textureShader.vert", "../../Shaders/textureShader.frag");
            _textureShader.Use();
            _textureShader.SetFloat("gamma", _gamma);

            plain = Mesh.PlainXYUV(new Vector3(0, 0, 0), 2.0f, 2.0f, 64, 64);
            _textureRectangle = new VertexObject(_textureShader, plain, _texture);

            _colorShader = new Shader("../../Shaders/colorShader.vert", "../../Shaders/colorShader.frag");

            triangle = Mesh.TriangleXYUV(new Vector2(0.0f, 0.0f), 0.75f);
            _colorTriangle1 = new VertexObject(_colorShader, triangle, colorData);

            triangle = Mesh.TriangleXYUV(new Vector2(0.5f, -0.1f), 0.25f);
            colorData[3] = 1;
            colorData[4] = 0;
            colorData[5] = 0;
            _colorTriangle2 = new VertexObject(_colorShader, triangle, colorData);

            base.OnLoad(e);
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
            ;
            float dt = 0.01f; //1.0f/ (float)this.RenderFrequency;
            time += dt;
            _center0 = vortex0.GetCenter(dt, vortex1.velocity(_center0));
            _center1 = vortex1.GetCenter(dt, vortex0.velocity(_center1));


            _textureShader.SetVector3("center0", _center0);
            _textureShader.SetVector3("center1", _center1);
            _textureShader.SetFloat("dt", dt);

            _textureRectangle.Draw();
            switch (_type)
            {

                case "t":
                    

                    if (_isTriangle)
                    {
                        //_colorShader.SetFloat("yCoord", 0.5f * (float)Math.Cos(2 * time));
                        _colorShader.SetFloat("yCoord", -1.0f + 2.0f*(1.0f -((float)_mouse.Y)/ClientSize.Height));
                        _colorShader.SetFloat("xCoord", -1.0f + 2.0f * ((float)_mouse.X / ClientSize.Width));
                        _colorTriangle1.Draw();
                        _isTriangle = false;
                    }
                    
                //    _colorShader.SetFloat("yCoord", 0.5f * (float)Math.Sin(2 * time));
                //    _colorShader.SetFloat("xCoord",0.0f);
                //    _colorTriangle2.Draw();

                    break;
                case "r":
                    

                    break;
            }

            _texture.Copy();
            // Thread.Sleep(500);
            Context.SwapBuffers();
            base.OnRenderFrame(e);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (Focused)
            {
                _isTriangle = true;
                _mouse = e.Position;
                //Mouse.SetPosition(X + Width / 2f, Y + Height / 2f);
            }

            base.OnMouseDown(e);
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

            _colorTriangle1.DeleteBuffers();
            _colorTriangle2.DeleteBuffers();
            _textureRectangle.DeleteBuffers();


            _textureShader.Handle.Delete();
            _colorShader.Handle.Delete();

            base.OnUnload(e);
        }
    }
}
