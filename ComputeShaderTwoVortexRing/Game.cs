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
        private Shader _particalComputeShader;
        private vec3 _nParticles = new vec3(16, 128, 48);
        private int _totalParticles;

        private StorageBuffer _particlePositionBuffer;
        private StorageBuffer _initialParticlePositionBuffer;
        private StorageBuffer _particleVelocityBuffer;

        private StorageBuffer _ring1Buffer;
        private StorageBuffer _ring2Buffer;

        private RingModel _ringModel;

        private VertexArray _particleVAO;
        private VertexArray _vortexVAO1;
        private VertexArray _vortexVAO2;

        private VisualModel _visualModel;

        private Torus _torus1;
        private Torus _torus2;
        private Controller _controller;

        public Game(int width, int height, string title) : base(width, height, GraphicsMode.Default, title)
        {

        }

        protected override void OnLoad(EventArgs e)
        {
            Console.WriteLine("Control key:" +
                "\n Arrow UP, DOWN  - rotate around X " +
                "\n W,S             - Zoom " +
                "\n A,D - move left, right " +
                "\n ESC - exit ");
            _totalParticles = (int)(_nParticles.x * _nParticles.y * _nParticles.z);
            _ringModel = new RingModel(0.001);
            _visualModel = new VisualModel(Width, Height);

            _adsShader = new Shader("../../Shaders/ads.vert", "../../Shaders/ads.frag");
            _adsShader.SetVector3("LightIntensity", new Vector3(0.95f, 0.95f, 2.0f));
            _adsShader.SetVector3("Ka", new Vector3(0.1f, 0.1f, 0.2f));
            _adsShader.SetVector3("Ks", new Vector3(1.0f, 0.5f, 0.0f));
            _adsShader.SetFloat("Shininess", 30.0f);

            var lp = new Vector4(10.0f, 10.0f, 10.0f, 1.0f);

            _adsShader.SetVector4("LightPosition", _visualModel.GetView() * lp);
            _particleShader = new Shader("../../Shaders/particles.vert", "../../Shaders/particles.frag");
            _vortexShader = new Shader("../../Shaders/vortex.vert", "../../Shaders/vortex.frag");

            _particalComputeShader = new Shader("../../Shaders/twoRing.comp");
            _particalComputeShader.SetInt("ringPointsNumber", _ringModel.GetPointNumber() + 1);
            _controller = new Controller(_visualModel);

            InitBuffers();
            base.OnLoad(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            KeyboardState input = Keyboard.GetState();
            
            if (input.IsKeyDown(Key.Escape))
            {
                Exit();
            }
            _controller.GetInput(input);

            base.OnUpdateFrame(e);
        }
               

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            _ringModel.SetSelfVelocity();
            _ringModel.SetSubStep();
            _ringModel.MoveStep();
            _particalComputeShader.Use();

            //  _particlePositionBuffer.BindLayout(0);
            //  _particleVelocityBuffer.BindLayout(1);

            _ring1Buffer.SubData(_ringModel.GetRingVortexPoints(1), 2);
            _ring2Buffer.SubData(_ringModel.GetRingVortexPoints(2), 3);
            //  _initialParticlePositionBuffer.BindLayout(4);

            _particalComputeShader.SetInt("ringPointsNumber", _ringModel.GetPointNumber());
            _particalComputeShader.SetFloat("DeltaT", _ringModel.TimeStep);
            _particalComputeShader.Compute(_totalParticles / 64, 1, 1,
                    MemoryBarrierFlags.ShaderStorageBarrierBit);

            // Draw the scene
            _visualModel.SetMatricesForShader(_particleShader);
            _visualModel.SetMatricesForShader(_vortexShader);
            _particleShader.Use();


            // Draw the particles
            _particleShader.SetVector4("Color", new Vector4(0.9f, 0.7f, 0, 0.8f));
            _particleVAO.Draw(PrimitiveType.Points, 0, _totalParticles);
            // Draw the vortexes
            //_vortexShader.Use();
            _adsShader.Use();

            _adsShader.SetVector3("Kd", new Vector3(0.95f, 0.95f, 2.0f));
            _torus1 = new Torus(_ringModel.GetRadius(1), 0.005f, 18, 72);
            SetThorusPosition(_ringModel.GetRing(1));
            _torus1.Render();

            _adsShader.SetVector3("Kd", new Vector3(0.95f, 0.0f, 0.50f));
            _torus2 = new Torus(_ringModel.GetRadius(2), 0.005f, 18, 72);
            SetThorusPosition(_ringModel.GetRing(2));
            _torus2.Render();

            Context.SwapBuffers();
            base.OnRenderFrame(e);
        }

        private void InitBuffers()
        {
            // Initial positions of the particles
            List<float> initialPosition = InitialPosition.InTwoCylinderOrdered(_nParticles,
                                            _ringModel.GetRadius(1), _ringModel.GetRadius(2), -0.15, -0.05);

            _particlePositionBuffer = new StorageBuffer(BufferUsageHint.DynamicDraw);
            _particlePositionBuffer.SetData(initialPosition.ToArray(), 0);

            var particleVelocity = GetInitialVelocity();
            _particleVelocityBuffer = new StorageBuffer(BufferUsageHint.DynamicDraw);
            _particleVelocityBuffer.SetData(particleVelocity.ToArray(), 1);


            _ring1Buffer = new StorageBuffer(BufferUsageHint.DynamicDraw);
            _ring1Buffer.SetData(_ringModel.GetRingVortexPoints(1), 2);

            _ring2Buffer = new StorageBuffer(BufferUsageHint.DynamicDraw);
            _ring2Buffer.SetData(_ringModel.GetRingVortexPoints(2), 3);

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

        private List<float> GetInitialVelocity()
        {
            List<float> lst = new List<float>();

            for (int i = 0; i < _nParticles.x; i++)
            {
                for (int j = 0; j < _nParticles.y; j++)
                {
                    for (int k = 0; k < _nParticles.z; k++)
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


        private void SetThorusPosition(VortexCurve ring)
        {
            var yTranslate = ring.GetCenter();//(float)center.Y;

            (var norm, var model) = _visualModel.GetNormal(yTranslate);

            _adsShader.Use();

            _adsShader.SetMatrix3("NormalMatrix", norm);
            _adsShader.SetMatrix4("model", model);
            _adsShader.SetMatrix4("projection", _visualModel.GetProjection());
            _adsShader.SetMatrix4("view", _visualModel.GetView());
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
