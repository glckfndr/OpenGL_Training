using OpenGLHelper;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Threading;
using GlmNet;
using TextureWrapMode = OpenTK.Graphics.OpenGL4.TextureWrapMode;

namespace ParticlesShader
{
    public class Game : GameWindow
    {

        private Shader _shader;
        //private int location;
        private uint nParticles = 8000;
        private VertexObject _particles;
        private float t = 0;
        private float dt;
        private mat4 _MVP;
        private mat4 _view;
        private mat4 _model;
        private mat4 _projection;
        private float _angle;


        public Game(int width, int height, string title) : base(width, height, GraphicsMode.Default, title)
        {

        }

        protected override void OnLoad(EventArgs e)
        {
            var rnd = new Random();
            Vector3 v = new Vector3(0.0f, 0.0f, 0.0f);
            float velocity;
            float theta;
            float phi;
            float[] velocityData = new float[nParticles * 3];
            for (uint i = 0; i < nParticles; i++)
            {
                // Выбрать вектор, определяющий направление и скорость
                theta = (float)( Math.PI / 24.0 * rnd.NextDouble());
                phi = (float)(2* Math.PI * rnd.NextDouble());
                v.X = (float)(Math.Sin(theta) * Math.Cos(phi));
                v.Y = (float)(Math.Cos(theta));
                v.Z = (float)(Math.Sin(theta) * Math.Sin(phi));
                // Масштабировать величину скорости
                velocity = (float)(1.3 + 0.5 * rnd.NextDouble());
                v = v.Normalized() * velocity;
                velocityData[3 * i] = v.X + 0.3f;
                velocityData[3 * i + 1] = v.Y;
                velocityData[3 * i + 2] = v.Z +  0.3f;
            }
            
            float[] timeData = new float[nParticles];
            float time = 0.0f;
            float rate = 0.001f;
            for (uint i = 0; i < nParticles; i++)
            {
                timeData[i] = time;
                time += rate;
            }

            _shader = new Shader("../../Shaders/particles.vert", "../../Shaders/particles.frag");
            _shader.Use();
            _shader.SetInt("ParticleTex", 0);
            _shader.SetFloat("ParticleLifetime", 3.5f);
            _shader.SetVector3("Gravity", new Vector3(0.0f, -0.9f, 0.0f));
            _angle =(float)(Math.PI / 3);
            _model = new mat4(1.0f);
            _projection = glm.perspective(glm.radians(60.0f), (float)Width / Height, 0.3f, 100.0f);

            Texture2D texture = new Texture2D("../../Textures/water2.jpg", TextureWrapMode.Repeat);
            this.ClientSize = texture.GetSize();

            _particles = new VertexObject(_shader, texture,
                new string[] { "InitialVelocity", "StartTime" },
                new int[] { 3, 1 },
                new int[] { 0, 0 },
                velocityData, timeData);

            GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.PointSize(3.0f);
            base.OnLoad(e);
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
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            //  Thread.Sleep(500);
            _shader.Use();
            _shader.SetFloat("Time", t);
            _angle += 0.015f;
            _view = glm.lookAt(new vec3(4.0f * glm.cos(_angle), 1.5f, 4.0f * glm.sin(_angle)),
                new vec3(0.0f, 1.5f, 0.0f),
                new vec3(0.0f, 1.0f, 0.0f));
            _MVP = _view * _model;
            _MVP = _projection * _MVP;
            _shader.SetMatrix4("MVP", _MVP.ConvertToMatrix4());
            _particles.Draw(PrimitiveType.Points);
            dt = 0.01f;
            t += dt;
            //t = (float)e.Time;
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
            _particles.DeleteBuffers();
            _shader.Handle.Delete();
            base.OnUnload(e);
        }
    }
}
