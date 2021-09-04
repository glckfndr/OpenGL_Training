using GlmNet;
using OpenGLHelper;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using System;
using System.Drawing;

namespace Texture_FBO
{
    public class Game : GameWindow
    {
        private string _type = "t";
        private Shader _shader;
        private Texture2D _whiteTexHandle;
        private Texture2D _renderTex;
        private float _angle;

        private mat4 _projection;
        private const float twoPi = (float)(2 * Math.PI);
        private float _rotSpeed = twoPi / 16.0f;
        private float time = 0.0f;

        private Point _mouse;
        private float _tPrev;
        private mat4 _view;
        private mat4 _model;
        private TeaPot _teaPot;
        private Cube _cube;
        //private int _fboHandle;
        private FrameBuffer _fboHandle;
        private RenderBuffer _depthBuf;


        public Game(int width, int height, string title) :
            base(width, height, GraphicsMode.Default, title)
        {
        }

        protected override void OnLoad(EventArgs e)
        {
            _teaPot = new TeaPot(14, new mat4(1.0f));
            _cube = new Cube(1.0f);
            _shader = new Shader("../../Shaders/rendertotex.vert", "../../Shaders/rendertotex.frag");

            GL.ClearColor(1.0f, 1.0f, 1.0f, 1.0f);
            GL.Enable(EnableCap.DepthTest);

            _projection = new mat4(1.0f);
            _angle = glm.radians(140.0f);
            _shader.SetVector3("Light.Intensity", new Vector3(1.0f, 1.0f, 1.0f));
            


            // One pixel white texture
            byte[] whiteTex = { 5, 55, 255, 255 };
            _whiteTexHandle = new Texture2D(1, 1, whiteTex, TextureUnit.Texture1);
            // Create the texture object
            _renderTex = new Texture2D(512, 512, TextureUnit.Texture0);
            // Create the depth buffer
            _depthBuf = new RenderBuffer(512, 512);

            SetupFBO();

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

                    //GL.BindFramebuffer(FramebufferTarget.Framebuffer, _fboHandle);
                    _fboHandle.Bind();
                    RenderToTexture();
                    GL.Flush();
                    //GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
                    _fboHandle.UnBind();
                    RenderScene();

                    break;
                case "r":


                    break;
            }

            Context.SwapBuffers();
            base.OnRenderFrame(e);
        }

        private void RenderToTexture()
        {
            _shader.SetInt("RenderTex", 1);
            GL.Viewport(0, 0, 512, 512);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _view = glm.lookAt(new vec3(0.0f, 0.0f, 7.0f), new vec3(0.0f, 0.0f, 0.0f), new vec3(0.0f, 1.0f, 0.0f));
            _projection = glm.perspective(glm.radians(60.0f), 1.0f, 0.3f, 100.0f);

            _shader.SetVector4("Light.Position", new Vector4(0.0f, 0.0f, 0.0f, 1.0f));
            _shader.SetVector3("Material.Kd", new Vector3(0.9f, 0.9f, 0.9f));
            _shader.SetVector3("Material.Ks", new Vector3(0.95f, 0.95f, 0.95f));
            _shader.SetVector3("Material.Ka", new Vector3(0.1f, 0.1f, 0.1f));
            _shader.SetFloat("Material.Shininess", 100.0f);


            _model = new mat4(1.0f);
            _model = glm.translate(_model, new vec3(0.0f, -1.5f, 0.0f));
            _model = glm.rotate(_model, glm.radians(-90.0f), new vec3(1.0f, 0.0f, 0.0f));
            SetMatrices();
            _teaPot.Render();
        }

        private void RenderScene()
        {
            _shader.SetInt("RenderTex", 0);
            GL.Viewport(0, 0, Width, Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            vec3 cameraPos = new vec3(2.0f * glm.cos(_angle), 1.5f, 2.0f * glm.sin(_angle));
            _view = glm.lookAt(cameraPos, new vec3(0.0f, 0.0f, 0.0f), new vec3(0.0f, 1.0f, 0.0f));
            _projection = glm.perspective(glm.radians(45.0f), (float)Width / Height, 0.3f, 100.0f);

            _shader.SetVector4("Light.Position", new Vector4(0, 0, 0, 1));
            _shader.SetVector3("Material.Kd", new Vector3(0.9f, 0.9f, 0.9f));
            _shader.SetVector3("Material.Ks", new Vector3(0.0f, 0.0f, 0.0f));
            _shader.SetVector3("Material.Ka", new Vector3(0.1f, 0.1f, 0.1f));
            _shader.SetFloat("Material.Shininess", 1.0f);


            _model = new mat4(1.0f);
            SetMatrices();
            _cube.Render();
        }

        private void SetMatrices()
        {
            mat4 mv = _view * _model;
            _shader.SetMatrix4("ModelViewMatrix", mv.ConvertToMatrix4());
            _shader.SetMatrix3("NormalMatrix",
                (new mat3(new vec3(mv[0]), new vec3(mv[1]), new vec3(mv[2]))).ConvertToMatrix3());
            _shader.SetMatrix4("MVP", (_projection * mv).ConvertToMatrix4());
        }

        private void SetupFBO()
        {
            // Generate and bind the framebuffer
            _fboHandle = new FrameBuffer();
            // Bind the texture to the FBO
            _renderTex.BindToFrameBuffer();
            // Bind the depth buffer to the FBO
            _depthBuf.BindToFrameBuffer();

            // Set the targets for the fragment output variables
            DrawBuffersEnum[] drawBuffers = { DrawBuffersEnum.ColorAttachment0 };
            GL.DrawBuffers(1, drawBuffers);

            _fboHandle.CheckStatus();

            // Unbind the framebuffer, and revert to default framebuffer
            _fboHandle.UnBind();
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
