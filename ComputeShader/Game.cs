using OpenGLHelper;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using TextureWrapMode = OpenTK.Graphics.OpenGL4.TextureWrapMode;

namespace ParticlesShader
{
    public class Game : GameWindow
    {

        private Shader _shader;
        private int location;
        private uint nParticles = 8000;
        private VertexObject _particles;
        private float t = 0;
        private float dt;
        private Matrix4 MVP;

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
                theta = (float)(Math.PI / 16.0 * rnd.NextDouble());
                phi = (float)(2 * Math.PI * rnd.NextDouble());
                v.X = (float)(Math.Sin(theta) * Math.Cos(phi));
                v.Y = (float)(Math.Cos(theta));
                v.Z = (float)(Math.Sin(theta) * Math.Sin(phi));
                // Масштабировать величину скорости
                velocity = (float)(1.9 + 0.5 * rnd.NextDouble());
                v = v.Normalized() * velocity;
                velocityData[3 * i] = v.X;
                velocityData[3 * i + 1] = v.Y;
                velocityData[3 * i + 2] = v.Z;
            }


            float[] timeData = new float[nParticles];
            float time = 0.0f;
            float rate = 0.00075f;
            for (uint i = 0; i < nParticles; i++)
            {
                timeData[i] = time;
                time += rate;
            }

            _shader = new Shader("../../Shaders/particles.vert", "../../Shaders/particles.frag");
            _shader.SetInt("ParticleTex", 0);
            _shader.SetFloat("ParticleLifetime", 3.5f);
            _shader.SetVector3("Gravity", new Vector3(0.0f, -0.9f, 0.0f));
            float cosa =(float)(float)Math.Cos(Math.PI / 3);
            float sina = (float) Math.Sin(Math.PI / 3);

            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60.0f),
                (float)Width / (float)Height, 0.1f, 100.0f);
            Matrix4 model = Matrix4.Identity * Matrix4.CreateRotationY((float)MathHelper.DegreesToRadians(30)); ;
            Matrix4 view = Matrix4.LookAt(new Vector3(4.0f*cosa, 0.0f, 4.0f*sina),
                                        new Vector3(0.0f, 1.5f, 0.0f),
                                        new Vector3(0.0f, 1.0f, 0.0f));

            
            MVP =  model * view;
            MVP = MVP * projection;
            _shader.SetMatrix4("MVP", MVP);

            Texture texture = new Texture("../../Textures/water2.jpg", TextureWrapMode.Repeat);
            this.ClientSize = texture.GetSize();

            _particles = new VertexObject(_shader, texture,
                new string[] { "InitialVelocity", "StartTime" },
                new int[] { 3, 1 },
                new int[] { 0, 0 },
                velocityData, timeData);

            GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);

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
            GL.Clear(ClearBufferMask.ColorBufferBit);
            //  Thread.Sleep(500);
            _shader.SetFloat("Time", t);
            
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
