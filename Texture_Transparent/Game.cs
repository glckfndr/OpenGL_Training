using OpenGLHelper;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using System;
using System.Drawing;
using GlmNet;

namespace Texture_Transparent
{
    public class Game : GameWindow
    {
        private string _type = "t";

        private Shader _refractShader;
        private float _angle;
        
        private const float twoPi = (float) (2 * Math.PI);
        private const float _rotSpeed = twoPi/16.0f;

        private int _cubeTex;
        private float time = 0.0f;
        
        private Point _mouse;
        private float _tPrev;

        private mat4 _projection;
        private mat4 _view;
        private mat4 _model;

        TeaPot _teaPot;
        SkyBox _sky;


        public Game(int width, int height, string title) :
            base(width, height, GraphicsMode.Default, title)
        {
        }

        protected override void OnLoad(EventArgs e)
        {

            _refractShader = new Shader("../../Shaders/refract.vert", "../../Shaders/refract.frag");
            //_skyShader = new Shader("../../Shaders/_sky.vert", "../../Shaders/_sky.frag");
            GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            
            GL.Enable(EnableCap.DepthTest);
            _angle = glm.radians(90.0f);
            _projection = new mat4(1.0f);
            _teaPot = new TeaPot(14, new mat4(1.0f));
            _sky = new SkyBox();
            GL.ActiveTexture(TextureUnit.Texture0);
            _cubeTex = Texture.LoadCubeMap("../../Textures/cubemap_night/night");
            //_cubeTex = Texture.LoadCubeMap("../../Textures/Cube/pisa");
            
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

                    vec3 cameraPos = new vec3(7.0f * glm.cos(_angle), 2.0f, 7.0f * glm.sin(_angle));
                    _view = glm.lookAt(cameraPos, new vec3(0.0f, 0.0f, 0.0f), 
                                                    new  vec3(0.0f, 1.0f, 0.0f));
                    _refractShader.SetVector3("WorldCameraPosition", cameraPos.ConvertToVector3());
                    _refractShader.SetInt("DrawSkyBox", 1);

                    _model = new mat4(1.0f);
                    SetMatrices();
                    _sky.Render();
                    _refractShader.SetInt("DrawSkyBox", 0);

                    _refractShader.SetFloat("Material.Eta", 0.94f);
                    _refractShader.SetFloat("Material.ReflectionFactor", 0.1f);

                    _model = new mat4(1.0f);
                    _model = glm.translate(_model, new vec3(0.0f, -1.0f, 0.0f));
                    _model = glm.rotate(_model, glm.radians(-90.0f), new vec3(1.0f, 0.0f, 0.0f));
                    SetMatrices();
                    _teaPot.Render();


                    break;
                case "r":
                    

                    break;
            }

            //_texture.Copy();
            // Thread.Sleep(500);
            Context.SwapBuffers();
            base.OnRenderFrame(e);
        }

        private void SetMatrices()
        {
            mat4 mv = _view * _model;
            _refractShader.SetMatrix4("ModelMatrix", _model.ConvertToMatrix4());
            _refractShader.SetMatrix4("MVP", (_projection * mv).ConvertToMatrix4());
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
            _projection = glm.perspective(glm.radians(50.0f), (float)Width / Height, 0.3f, 100.0f);
            base.OnResize(e);
        }

        

        protected override void OnUnload(EventArgs e)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            

            _refractShader.Handle.Delete();
            

            base.OnUnload(e);
        }
    }
}
