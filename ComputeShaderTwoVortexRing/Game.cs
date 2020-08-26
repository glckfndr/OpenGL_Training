using GlmNet;
using OpenGLHelper;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using System;
using System.Collections.Generic;

namespace ComputeShaderTwoVortexRing
{
    public class Game : GameWindow
    {

        private Shader _particleShader;
        private Shader _vortexShader;
        private Shader _adsShader;
        private Shader _computeShader;
        private vec3 nParticles = new vec3(50, 50, 50);
        private int totalParticles;
        private int _ringPointsNumber = 72*2;
        

        private StorageBuffer _particlePositionBuffer;
        private StorageBuffer _initialParticlePositionBuffer;
        private StorageBuffer _particleVelocityBuffer;

        private StorageBuffer _ring1Buffer;
        private StorageBuffer _ring2Buffer;

        private VortexCurve _ring1;
        private VortexCurve _ring2;

        private float _gamma = 0.1f;

        private float _time;
        private float _deltaT;
        private float _speed = 35.0f;
        private float _angle;

        private VertexArray _particleVAO;
        private VertexArray _vortexVAO1;
        private VertexArray _vortexVAO2;

        private float t;
        private float dt;
        private mat4 _projection;
        private mat4 _view;
        private mat4 _model;
        private Sphere _sphere1;
        private Sphere _sphere2;
        private double outDeltaR = 0.15f;
        private double innerDeltaR = 0.5f;
        private float eyePos = 2.0f;

        private double _ring1Radius = 0.6;
        private double _ring2Radius = 0.4;

        public Game(int width, int height, string title) : base(width, height, GraphicsMode.Default, title)
        {

        }

        protected override void OnLoad(EventArgs e)
        {
            _ring1 = new VortexCurve(-1.0, _ringPointsNumber, _ring1Radius,-0.1);
            _ring2 = new VortexCurve(0.5, _ringPointsNumber, _ring2Radius, 0.0);
            _model = new mat4(1.0f);
            GL.Enable(EnableCap.DepthTest);
            totalParticles = (int)(nParticles.x * nParticles.y * nParticles.z);
            _particleShader = new Shader("../../Shaders/particles.vert", "../../Shaders/particles.frag");
            _vortexShader = new Shader("../../Shaders/vortex.vert", "../../Shaders/vortex.frag");

            _computeShader = new Shader("../../Shaders/twoRing.comp");
            _computeShader.SetInt("ringPointsNumber", _ringPointsNumber + 1);
          //  _computeShader.SetFloat("ring1Radius",(float) _ring1Radius);
          //  _computeShader.SetFloat("ring2Radius", (float)_ring2Radius);
            //_computeShader.SetFloat("gamma", _gamma);
            InitBuffers();

            GL.ClearColor(0, 0.0f, 0.0f, 1);

            _projection = glm.perspective(glm.radians(50.0f),
                                        (float)Width / Height, 0.1f, 100.0f);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            base.OnLoad(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {

            if (_time == 0.0f)
            {
                _deltaT = 0.0f;
            }
            else
            {
                _deltaT = t - _time;
            }
            _time = t;
            if (true)
            {
                _angle += _speed * _deltaT;
                if (_angle > 360.0f) _angle -= 360.0f;
            }
            t += 0.005f;

            // Execute the compute shader
            _ring1.InVelocity(_ring2);
            _ring2.InVelocity(_ring1);
            _ring1.Move();
            _ring2.Move();

            _computeShader.Use();


            _particlePositionBuffer.Bind(0);
            _particleVelocityBuffer.Bind(1);

            _ring1Buffer.SubData(_ring1.ToVortexPointArray(),2);
            _ring2Buffer.SubData(_ring2.ToVortexPointArray(),3);
            

            _initialParticlePositionBuffer.Bind(4);

            _computeShader.SetInt("ringPointsNumber", _ringPointsNumber);
           // _computeShader.SetFloat("gamma", _gamma);

           for (int k = 0; k < 2; k++)
                _computeShader.Compute(MemoryBarrierFlags.ShaderStorageBarrierBit, 
                                totalParticles / 100, 1, 1);

            // Draw the scene
            SetMatrices();
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            _particleShader.Use();
            // Draw the particles
            _particleShader.SetVector4("Color", new Vector4(0.9f, 0.7f, 0, 0.8f));
            GL.PointSize(1.0f);
            _particleVAO.Draw(PrimitiveType.Points, 0, totalParticles);
            // Draw the vortexes

            _vortexShader.Use();
            _vortexShader.SetVector4("Color", new Vector4(0.7f, 0.0f, 0.3f, 0.8f));
            //_particleVelocityBuffer.SetAttribPointer(2, totalParticles);
            _vortexVAO1.Draw(PrimitiveType.LineLoop, 0, _ringPointsNumber);

            _vortexShader.SetVector4("Color", new Vector4(0.0f, 0.9f, 0.8f, 0.8f));
            _vortexVAO2.Draw(PrimitiveType.LineLoop, 0, _ringPointsNumber);


            Context.SwapBuffers();
            base.OnRenderFrame(e);
        }

        private void InitBuffers()
        {
            // Initial positions of the particles
            
            // We need _buffers for position , and velocity.
            //int bufSize = totalParticles * 4 * sizeof(float);

            // The _buffers for positions
            List<float> initialPosition = GetInitialPositionForParticles(_ring1Radius);

            _particlePositionBuffer = new StorageBuffer(BufferUsageHint.DynamicDraw);
            _particlePositionBuffer.SetData(initialPosition.ToArray(), 0);

            var vel = GetInitialVelocity();
            _particleVelocityBuffer = new StorageBuffer(BufferUsageHint.DynamicDraw);
            _particleVelocityBuffer.SetData(vel.ToArray(), 1);

            //_ring1Position = GetVortexRing(_ringPointsNumber, (float)_ringRadius1);
            _ring1Buffer = new StorageBuffer(BufferUsageHint.DynamicDraw);
            _ring1Buffer.SetData(_ring1.ToVortexPointArray(), 2);

           // _ring2Buffer = GetVortexRing(_ringPointsNumber, (float)_ring2Radius, 1.0f);
            _ring2Buffer = new StorageBuffer(BufferUsageHint.DynamicDraw);
            _ring2Buffer.SetData(_ring2.ToVortexPointArray(), 3);

            _initialParticlePositionBuffer = new StorageBuffer(BufferUsageHint.StaticDraw);
            _initialParticlePositionBuffer.SetData(initialPosition.ToArray(), 4);


            // Set up the VAO
            _particleVAO = new VertexArray();
            _particleVAO.Bind();

            _particlePositionBuffer.SetAttribPointer(0, 4);
            _particleVelocityBuffer.SetAttribPointer(1, 4);

            _particleVAO.Unbind();


            // Set up a buffer and a VAO for drawing the vortexes
            _vortexVAO1 = new VertexArray(); ;
            _ring1Buffer.SetAttribPointer(0, 4);
            
            _vortexVAO1.Unbind();

            _vortexVAO2 = new VertexArray(); ;
            _ring2Buffer.SetAttribPointer(0, 4);
            _vortexVAO2.Unbind();
        }

        
        private List<float> GetInitialPositionForParticles(double ringRadius)
        {
            List<float> initialPosition = new List<float>();
            var rnd = new Random();
            //var deltaR = outRadius - innerRadius; 

            for (int i = 0; i < nParticles.x; i++)
            {
                for (int j = 0; j < nParticles.y; j++)
                {
                    for (int k = 0; k < nParticles.z; k++)
                    {
                        var phi = rnd.NextDouble() * 2.0 * Math.PI;

                        
                        var r = ringRadius - innerDeltaR + (innerDeltaR + outDeltaR) * rnd.NextDouble();
                        var x = (float)(r * Math.Cos(phi));
                        var z = (float)(r * Math.Sin(phi));
                        var y = 0.0f;
                        var w = 1.0f;

                        initialPosition.Add(x);
                        initialPosition.Add(y);
                        initialPosition.Add(z);
                        initialPosition.Add(w);
                    }
                }
            }

            return initialPosition;
        }

        private List<float> GetInitialVelocity()
        {
            List<float> lst = new List<float>();

            for (int i = 0; i < nParticles.x; i++)
            {
                for (int j = 0; j < nParticles.y; j++)
                {
                    for (int k = 0; k < nParticles.z; k++)
                    {

                        var x = 0.0f;
                        var y = 0.0f;
                        var z = 0.0f;
                        var w = 1.0f;

                        lst.Add(x);
                        lst.Add(y);
                        lst.Add(z);
                        lst.Add(w);
                    }
                }
            }

            return lst;
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



        private void SetMatrices()
        {

            _view = glm.lookAt(new vec3((float)(4*_ring1Radius), eyePos, 0), new vec3(0, 0, 0), new vec3(0, 1, 0));
            //_model = new mat4(1.0f);
            _particleShader.Use();
            _particleShader.SetMatrix4("model", _model.ConvertToMatrix4());
            _particleShader.SetMatrix4("projection", _projection.ConvertToMatrix4());
            _particleShader.SetMatrix4("view", _view.ConvertToMatrix4());

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
            _particleShader.Handle.Delete();
            base.OnUnload(e);
        }
    }
}
