using OpenGLHelper;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using System;
using System.Drawing;
using GlmNet;

namespace Texture_Parallax
{
    public class Game : GameWindow
    {
        private string _type = "t";
        private Shader _shader;
        //private Shader _skyShader;
        private float _angle;
        
        private mat4 _projection;
        private const float twoPi = (float) (2 * Math.PI);
        private float _rotSpeed = twoPi/16.0f;

        // private Texture  normalMap, heightMap, colorMap;
        private int normalMap, heightMap, colorMap;
        private float time = 0.0f;
        
        private Point _mouse;
        private float _tPrev;
        private mat4 _view;
        private mat4 _model;
        Plane _plane;
        


        public Game(int width, int height, string title) :
            base(width, height, GraphicsMode.Default, title)
        {
        }

        protected override void OnLoad(EventArgs e)
        {
            
            _plane = new Plane(8,8,1,1);
            _shader = new Shader("../../Shaders/steep-parallax.vert", "../../Shaders/steep-parallax.frag");
            //_shader = new Shader("../../Shaders/parallax.vert", "../../Shaders/parallax.frag");

            GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Multisample);

            
            _model = new mat4(1);
            _angle = glm.radians(90.0f);

            // Load textures
            var normalMap = new Texture2D("../../Textures/mybrick-normal.png", TextureWrapMode.ClampToBorder, TextureUnit.Texture1);
            var heightMap = new Texture2D("../../Textures/mybrick-height.png", TextureWrapMode.ClampToBorder, TextureUnit.Texture2);
            var colorMap = new Texture2D("../../Textures/mybrick-color.png");
            //normalMap = Texture.LoadTexture("../../Textures/mybrick-normal.png");
            //heightMap = Texture.LoadTexture("../../Textures/mybrick-height.png", TextureUnit.Texture1);
            //colorMap = Texture.LoadTexture("../../Textures/mybrick-color.png", TextureUnit.Texture0);


            _view = glm.lookAt(new vec3(-1.0f, 0.0f, 8.0f), 
                                new vec3(-1.0f, 0.0f, 0.0f), 
                                new vec3(0.0f, 1.0f, 0.0f));
            
            _shader.SetVector3("Light.L", new Vector3(0.7f));
            _shader.SetVector3("Light.La", new Vector3(0.01f));
            _shader.SetVector3("Material.Ks", new Vector3(0.7f));
            _shader.SetFloat("Material.Shininess", 40.0f);

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
            
            float deltaT = time - _tPrev;
            if (_tPrev == 0.0f) deltaT = 0.0f;
            _tPrev = time;

            _angle += _rotSpeed * deltaT;
            if (_angle > twoPi)
                _angle -= twoPi;


            float dt = 0.01f;
            time += dt;
            
            switch (_type)
            {

                case "t":
                    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                    var newPos = _view * (new vec4(2.0f, 2.0f, 1.0f, 1.0f));
                    _shader.SetVector4("Light.Position", new Vector4(newPos.x, newPos.y, newPos.z, newPos.w));
                    _model = new mat4(1.0f);
                    
                    _model = glm.rotate(_model, glm.radians(65.0f), new vec3(0.0f, 1.0f, 0.0f));
                    _model = glm.rotate(_model, glm.radians(90.0f), new vec3(1.0f, 0.0f, 0.0f));
                    SetMatrices();
                    _plane.Render();


                    break;
                case "r":
                    

                    break;
            }
            
            Context.SwapBuffers();
            base.OnRenderFrame(e);
        }

        

        

        private void SetMatrices()
        {
            mat4 mv = _view * _model;
            _shader.SetMatrix4("ModelViewMatrix", mv.ConvertToMatrix4());
            _shader.SetMatrix3("NormalMatrix", 
                (new mat3(new vec3(mv[0]), new vec3(mv[1]), new vec3(mv[2]))).ConvertToMatrix3());
            _shader.SetMatrix4("MVP", (_projection * mv).ConvertToMatrix4());
        }


        


        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (Focused)
            {
                _mouse = e.Position;
                //Mouse.SetPosition(X + Width / 2f, Y + Height / 2f);
            }

            base.OnMouseDown(e);
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            _projection = glm.perspective(glm.radians(60.0f), (float)Width / Height, 0.3f, 100.0f);
            base.OnResize(e);
        }

        

        protected override void OnUnload(EventArgs e)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            

            _shader.Handle.Delete();
            

            base.OnUnload(e);
        }
    }
}
