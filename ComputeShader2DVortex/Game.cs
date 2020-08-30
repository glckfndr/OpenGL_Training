using GlmNet;
using OpenGLHelper;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using System;
using System.Collections.Generic;


namespace ComputeShader2DVortex
{
    

    public class Game : GameWindow
    {

        //  private Shader _particleShader;
        private Shader _vortexShader;
        //  private Shader _adsShader;
        private Shader _computeShaderVelocity;
        private Shader _computeShaderPosition;
        private vec3 nParticles = new vec3(16, 16, 16);
        private int _totalParticles;
        private int vortexNumber;
        private float eyePos = 3.0f;

        private StorageBuffer _vortexBuffer;
        private StorageBuffer _velocityBuffer;

        private float _gamma = 0.1f;

        private float _time;
        private float _deltaT;
        private float _speed = 35.0f;
        private float _angle;

        //  private VertexArray _particleVAO;
        private VertexArray _vortexVAO;

        private float t;
        private float dt;
        private mat4 _projection;
        private mat4 _view;
        private mat4 _model;
        private Sphere _sphere1;
        private Sphere _sphere2;
        private double outDeltaR = 0.15f;
        private double innerDeltaR = 0.15f;

        private double _ringRadius = 1.0;

        public Game(int width, int height, string title) : base(width, height, GraphicsMode.Default, title)
        {

        }

        protected override void OnLoad(EventArgs e)
        {


            _totalParticles = (int)(nParticles.x * nParticles.y * nParticles.z);
            vortexNumber = _totalParticles;
            CreateShaders();
            InitializeBuffers();
            SetOpenGlParameters();
            SetInitialMatrix();
            base.OnLoad(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            _time += _deltaT;
            // Execute the compute shader
            _computeShaderVelocity.Use();
            _computeShaderVelocity.SetInt("vortexNumber", vortexNumber);
            _vortexBuffer.Bind(0);
            _velocityBuffer.Bind(1);
            _computeShaderVelocity.Compute(MemoryBarrierFlags.ShaderStorageBarrierBit,
                    _totalParticles / 128, 1, 1);

            _computeShaderPosition.Use();
            _vortexBuffer.Bind(0);
            _velocityBuffer.Bind(1);
            _computeShaderPosition.Compute(MemoryBarrierFlags.ShaderStorageBarrierBit,
                _totalParticles / 128, 1, 1);

            // Draw the scene
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            SetShaderMatrices();
            
            _vortexShader.Use();
            _vortexShader.SetVector4("Color", new Vector4(0.7f, 0.9f, 0.3f, 0.8f));
            //_velocityBuffer.SetAttribPointer(2, _totalParticles);
            _vortexVAO.Draw(PrimitiveType.Points, 0, _totalParticles);


            Context.SwapBuffers();
            base.OnRenderFrame(e);
        }

        private void SetInitialMatrix()
        {
            _model = new mat4(1.0f);
            _projection = glm.perspective(glm.radians(50.0f), (float)Width / Height, 0.1f, 100.0f);
        }

        private void SetOpenGlParameters()
        {
            GL.ClearColor(0, 0.0f, 0.0f, 1);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
            GL.PointSize(1.0f);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        }

        private void CreateShaders()
        {
            //_particleShader = new Shader("../../Shaders/particles.vert", "../../Shaders/particles.frag");
            _vortexShader = new Shader("../../Shaders/vortex.vert", "../../Shaders/vortex.frag");

            _computeShaderVelocity = new Shader("../../Shaders/vortexVelocity2D.comp");
            _computeShaderVelocity.SetInt("vortexNumber", vortexNumber);

            _computeShaderPosition = new Shader("../../Shaders/vortexPosition2D.comp");
            //_computeShaderPosition.SetInt("vortexNumber", vortexNumber);
        }



        private void InitializeBuffers()
        {
            // Initial positions of the particles

            // We need _buffers for position , and velocity.
            //int bufSize = _totalParticles * 4 * sizeof(float);

            // The _buffers for positions
            List<Vortex> vortexes = GetVortexes();

            _vortexBuffer = new StorageBuffer(BufferUsageHint.DynamicDraw);
            _vortexBuffer.SetData(vortexes.ToArray(), 0);

            var vel = GetVelocity();
            _velocityBuffer = new StorageBuffer(BufferUsageHint.DynamicDraw);
            _velocityBuffer.SetData(vel.ToArray(), 1);

            // Set up the VAO
            // Set up a buffer and a VAO for drawing the vortexes
            _vortexVAO = new VertexArray(); ;
            _vortexBuffer.SetAttribPointer(0, 4);
            _velocityBuffer.SetAttribPointer(1, 2);
            _vortexVAO.Unbind();
        }



        private List<Vortex> GetVortexes()
        {
            List<Vortex> list = new List<Vortex>();
            var rnd = new Random();
            //var deltaR = outRadius - innerRadius; 
            float R = 1.0f;
            for (int i = 0; i < nParticles.x; i++)
            {
                for (int j = 0; j < nParticles.y; j++)
                {
                    for (int k = 0; k < nParticles.z; k++)
                    {
                        var phi = rnd.NextDouble() * 2.0 * Math.PI;

                        var r = R * rnd.NextDouble();
                        var x = (float)(r * Math.Cos(phi));
                        var y = (float)(r * Math.Sin(phi));
                        var gamma = (float)(-0.1 + 0.2 * rnd.NextDouble());
                        var rankine = 0.01f;
                        var vr = new Vortex(new vec2(x, y), gamma, rankine);
                        list.Add(vr);
                    }
                }
            }

            return list;
        }

        private List<vec2> GetVelocity()
        {
            List<vec2> list = new List<vec2>();
            var rnd = new Random();
            //var deltaR = outRadius - innerRadius; 
            float R = 1.0f;
            for (int i = 0; i < nParticles.x; i++)
            {
                for (int j = 0; j < nParticles.y; j++)
                {
                    for (int k = 0; k < nParticles.z; k++)
                    {

                        var vel = new vec2(0);
                        list.Add(vel);

                    }
                }
            }

            return list;
        }


        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            KeyboardState input = Keyboard.GetState();

            if (input.IsKeyDown(Key.Escape))
            {
                Exit();
            }

            if (input.IsKeyDown(Key.Up))
            {
                eyePos += 0.1f;
            }

            if (input.IsKeyDown(Key.Down))
            {
                eyePos -= 0.1f;
            }

            base.OnUpdateFrame(e);
        }



        private void SetShaderMatrices()
        {
            _view = glm.lookAt(new vec3(0, 0, 5), new vec3(0, 0, 0), new vec3(0, 1, 0));

            _vortexShader.Use();
            _vortexShader.SetMatrix4("model", _model.ConvertToMatrix4());
            _vortexShader.SetMatrix4("projection", _projection.ConvertToMatrix4());
            _vortexShader.SetMatrix4("view", _view.ConvertToMatrix4());
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
            //_shape.DeleteBuffers();
            //_particleShader.Handle.Delete();
            base.OnUnload(e);
        }
    }
}
