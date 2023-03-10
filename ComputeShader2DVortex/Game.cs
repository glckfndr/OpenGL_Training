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

        //  shaders for rendering
        private Shader _vortexShader;
        private Shader _renderShader;

        //  shaders for computing;
        private Shader _velocityComputeShader;
        private Shader _positionComputeShader;
        private Shader _clearTextureComputeShader;

        private readonly vec3 _nParticles = new vec3(128, 32, 1);
        //private int _totalParticles;
        private float _totalVorticity = 60.0f;
        private int _vortexNumber;
        private float _eyePos = 2.0f;

        private StorageBuffer _vortexBuffer;
        private StorageBuffer _velocityBuffer;

        // private float _gamma = 0.1f;

        private float _time = 0;
        private float _deltaTime = 0.00005f;
        private float _speed = 35.0f;
        private float _angle;

        //  private VertexArray _particleVAO;
        private VertexArray _vortexVAO;


        private float dt;
        private mat4 _projection;
        private mat4 _view;
        private mat4 _model;

        private Texture2D _texture;
        private int _height = 1024;
        private int _width = 1024;
        private Plane _plane;
        private StorageBuffer _vortexBuffer05;
        private Shader _positionComputeShader05;
        private bool _is3D = true;
        private bool _isPause = false;


        public Game(int width, int height, string title) : base(width, height, GraphicsMode.Default, title)
        {

        }

        protected override void OnLoad(EventArgs e)
        {
            _vortexNumber = (int)(_nParticles.x * _nParticles.y * _nParticles.z);

            CreateShaders();

            _texture = new Texture2D(_width, _height, 0);
            _plane = new Plane(3.0f, 3.0f, 2, 2);
            _angle = 90.0f;

            // List<Vortex> vortexes = VortexInitializer.GetVortexesInCircle(nParticles);
            //List<Vortex> vortexes = VortexInitializer.GetVortexesInLayer(nParticles);
            List<Vortex> vortexes = VortexInitializer.GetVortexesInLayerOrdered(_nParticles, _totalVorticity / _vortexNumber);
            _vortexBuffer = new StorageBuffer(BufferUsageHint.DynamicDraw);
            _vortexBuffer.SetData(vortexes.ToArray(), 0);

            _vortexBuffer05 = new StorageBuffer(BufferUsageHint.DynamicDraw);
            _vortexBuffer05.SetData(vortexes.ToArray(), 2);

            var initialVelocity = VortexInitializer.GetVelocity(_nParticles);
            _velocityBuffer = new StorageBuffer(BufferUsageHint.DynamicDraw);
            _velocityBuffer.SetData(initialVelocity.ToArray(), 1);

            // Set up a buffer and a VAO for drawing the vortexes
            _vortexVAO = new VertexArray(); ;
            _vortexBuffer.SetAttribPointer(0, 4);
            _velocityBuffer.SetAttribPointer(1, 2);
            _vortexVAO.Unbind();

            SetOpenGlParameters();
            SetInitialMatrix();
            base.OnLoad(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            if (!_isPause)
            {
                _time += _deltaTime;
                if (_time % 0.1 <= _deltaTime)
                {
                    Console.WriteLine("Vortex Number: " + _vortexNumber);
                    Console.WriteLine("Time: " + _time);
                }


                _clearTextureComputeShader.Compute(_width / 16, _height / 16, 1, MemoryBarrierFlags.ShaderImageAccessBarrierBit);

                //_velocityComputeShader.Use();
                _velocityComputeShader.SetInt("vortexNumber", _vortexNumber);

                // Bind buffers to compute shader
                //_vortexBuffer.Bind(0);
                _vortexBuffer.BindLayout(0);
                _velocityBuffer.BindLayout(1);
                // start compute shader
                _velocityComputeShader.Compute(_vortexNumber / 128, 1, 1, MemoryBarrierFlags.ShaderStorageBarrierBit);

                // Bind buffers to compute shader
                _vortexBuffer05.BindLayout(2);
                _positionComputeShader05.SetFloat("deltaTime", _deltaTime / 2);
                // start compute shader
                _positionComputeShader05.Compute(_vortexNumber / 128, 1, 1, MemoryBarrierFlags.ShaderStorageBarrierBit);

                // Bind buffers to compute shader
                _vortexBuffer05.BindLayout(0);
                // start compute shader
                _velocityComputeShader.Compute(_vortexNumber / 128, 1, 1, MemoryBarrierFlags.ShaderStorageBarrierBit);

                _vortexBuffer.BindLayout(0);
                _positionComputeShader.SetFloat("deltaTime", _deltaTime);
                _positionComputeShader.Compute(_vortexNumber / 128, 1, 1,
                    MemoryBarrierFlags.ShaderStorageBarrierBit | MemoryBarrierFlags.ShaderImageAccessBarrierBit);
            }
            // Draw the scene
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            if (_is3D)
            {
                SetShaderMatrices();
                _vortexShader.Use();
                _vortexShader.SetVector4("Color", new Vector4(0.7f, 0.9f, 0.3f, 0.8f));
                
                _vortexVAO.Draw(PrimitiveType.Points, 0, _vortexNumber);
            }
            else
            {
                _renderShader.Use();

                // mat4 view = glm.lookAt(new vec3(0, _eyePos, 2), new vec3(0, 0, 0), new vec3(0, 1, 0));
                // mat4 model = glm.rotate(new mat4(1.0f), glm.radians(_angle), new vec3(1, 0.0f, 0.0f));
                mat4 view = glm.lookAt(new vec3(0, _eyePos, 0), new vec3(0, 0, 0), new vec3(0, 0, -1));
                var model = new mat4(1.0f);
                mat4 mv = view * model;
                mat3 norm = new mat3(new vec3(mv[0]), new vec3(mv[1]), new vec3(mv[2]));
                mat4 proj = glm.perspective(glm.radians(60.0f), (float)Width / Height, 0.1f, 100.0f);

                _renderShader.SetMatrix4("model", model.ConvertToMatrix4());
                _renderShader.SetMatrix4("view", view.ConvertToMatrix4());
                _renderShader.SetMatrix4("projection", proj.ConvertToMatrix4());
                _renderShader.SetMatrix3("NormalMatrix", norm.ConvertToMatrix3());
                _plane.Render();
            }

            Context.SwapBuffers();
            //Thread.Sleep(500);
            base.OnRenderFrame(e);
        }

        private void SetInitialMatrix()
        {
            _model = new mat4(1.0f);
            _projection = glm.perspective(glm.radians(60.0f), (float)Width / Height, 0.1f, 100.0f);
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
            _renderShader = new Shader("../../Shaders/ads.vert", "../../Shaders/ads.frag");
            _renderShader.Use();
            _renderShader.SetVector4("LightPosition", new Vector4(2.0f, 2.0f, 2.0f, 1.0f));
            _renderShader.SetVector3("LightIntensity", new Vector3(1.0f));
            _renderShader.SetVector3("Kd", new Vector3(0.8f));
            _renderShader.SetVector3("Ka", new Vector3(0.2f));
            _renderShader.SetVector3("Ks", new Vector3(0.2f));
            _renderShader.SetFloat("Shininess", 180.0f);
            
            _velocityComputeShader = new Shader("../../Shaders/vortexVelocity2D.comp");
            _velocityComputeShader.SetInt("vortexNumber", _vortexNumber);
            _positionComputeShader = new Shader("../../Shaders/vortexPosition2D.comp");
            _positionComputeShader05 = new Shader("../../Shaders/vortexPosition2D05.comp");
            //_positionComputeShader.SetInt("_vortexNumber", _vortexNumber);
            _clearTextureComputeShader = new Shader("../../Shaders/clearTexture.comp");
        }


        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            KeyboardState input = Keyboard.GetState();

            if (input.IsKeyDown(Key.Escape)){
                Exit();
            }

            if (input.IsKeyDown(Key.Up))
            {
                _eyePos += 0.1f;
            }

            if (input.IsKeyDown(Key.Number3))
            {
                _is3D = true;
            }

            if (input.IsKeyDown(Key.Space))
            {
                _isPause = true;
            }

            if (input.IsKeyDown(Key.C))
            {
                _isPause = false;
            }


            if (input.IsKeyDown(Key.Number2))
            {
                _is3D = false;
            }

            if (input.IsKeyDown(Key.Down))
            {
                _eyePos -= 0.1f;
            }

            base.OnUpdateFrame(e);
        }



        private void SetShaderMatrices()
        {
            _view = glm.lookAt(new vec3(0, 0, _eyePos),
                new vec3(0, 0, 0), new vec3(0, 1, 0));
            _model = new mat4(1.0f);

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
