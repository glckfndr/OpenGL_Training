using GlmNet;
using OpenGLHelper;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using System;

namespace ComputeShaderFractal
{

    public class Game : GameWindow
    {
        private Shader _renderShader;
        private Shader _computeShader;

        private int dataBuf = 0;
        private int fsQuad = 0;
        private Cube cube;
        private vec2 center = new vec2(0.001643721971153f, 0.822467633298876f);
        private float cheight = 2.0f;
        private float time = 0.0f;
        private float deltaT = 0.0f;
        private float speed = 200;
        private float angle = 0.0f;
        private float rotSpeed = 60.0f;
        private float t = 0;
        private int _imgTex;
        private Texture2D _texture;
        private int _width = 512;
        private int _height = 512;



        public Game(int width, int height, string title) : 
            base(width, height, GraphicsMode.Default, title)
        {

        }

        protected override void OnLoad(EventArgs e)
        {
            GL.Enable(EnableCap.DepthTest);
            cube = new Cube(2.0f);
            _renderShader = new Shader("../../Shaders/ads.vert", "../../Shaders/ads.frag");
            _computeShader = new Shader("../../Shaders/mandelbrot.comp");
            _texture = new Texture2D(_width, _height, 0);
            //CreateTexture();
            SetWindow();

            _renderShader.Use();
            _renderShader.SetVector4("LightPosition", new Vector4(0.0f, 0.0f, -1.0f, 1.0f));
            _renderShader.SetVector3("LightIntensity", new Vector3(1.0f));
            _renderShader.SetVector3("Kd", new Vector3(0.8f));
            _renderShader.SetVector3("Ka", new Vector3(0.2f));
            _renderShader.SetVector3("Ks", new Vector3(0.2f));
            _renderShader.SetFloat("Shininess", 180.0f);

            base.OnLoad(e);
        }

        //private void CreateTexture()
        //{
        //    _imgTex = GL.GenTexture();
        //    GL.ActiveTexture(TextureUnit.Texture0);
        //    GL.BindTexture(TextureTarget.Texture2D, _imgTex);
        //    GL.TexStorage2D(TextureTarget2d.Texture2D, 1, SizedInternalFormat.Rgba8, 256, 256);
        //    GL.BindImageTexture(0, _imgTex, 0, false, 0, TextureAccess.ReadWrite, SizedInternalFormat.Rgba8);
        //}

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
            t += 0.005f;
            if (time == 0.0f)
            {
                deltaT = 0.0f;
            }
            else
            {
                deltaT = t - time;
            }
            time = t;

            //if (!animating()) return;

            float dy = cheight / Height;

            cheight -= deltaT * speed * dy;
            SetWindow();
            angle += rotSpeed * deltaT;
            if (angle > 360.0f) angle -= 360.0f;


            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            //_computeShader.Use();
            _computeShader.Compute( _width / 32, _height / 32, 1, 
                MemoryBarrierFlags.ShaderImageAccessBarrierBit);
            //GL.DispatchCompute(_width / 32, _height / 32, 1);
            //GL.MemoryBarrier(MemoryBarrierFlags.ShaderImageAccessBarrierBit);

            _renderShader.Use();
            mat4 view = glm.lookAt(new vec3(0, 0, -4), new vec3(0, 0, 0), new vec3(0, 1, 0));
            mat4 model = glm.rotate(new mat4(1.0f), glm.radians(angle), new vec3(1, 1.5f, 0.5f));
            mat4 mv = view * model;
            mat3 norm = new mat3(new vec3(mv[0]), new vec3(mv[1]), new vec3(mv[2]));
            mat4 proj = glm.perspective(glm.radians(60.0f), (float)Width / Height, 1.0f, 100.0f);

            //_renderShader.SetMatrix4("ModelViewMatrix", mv.ConvertToMatrix4());
            _renderShader.SetMatrix4("model", model.ConvertToMatrix4());
            _renderShader.SetMatrix4("view", view.ConvertToMatrix4());
            _renderShader.SetMatrix4("projection", proj.ConvertToMatrix4());
            _renderShader.SetMatrix3("NormalMatrix", norm.ConvertToMatrix3());
            //_renderShader.SetMatrix4("MVP", (proj * mv).ConvertToMatrix4());

            cube.Render();
            Context.SwapBuffers();
            base.OnRenderFrame(e);
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            base.OnResize(e);
        }

        private void SetWindow()
        {
            _computeShader.Use();
            float ar = 1.0f;
            float cwidth = cheight * ar;

            Vector4 bbox = new Vector4(center.x - cwidth / 2.0f, center.y - cheight / 2.0f,
            center.x + cwidth / 2.0f, center.y + cheight / 2.0f);
            _computeShader.SetVector4("CompWindow", bbox);
        }

        protected override void OnUnload(EventArgs e)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);
            //_shape.DeleteBuffers();
            _renderShader.Handle.Delete();
            _computeShader.Handle.Delete();
            base.OnUnload(e);
        }
    }
}
