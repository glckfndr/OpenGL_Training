using DiscreteVortexLibrary;
using GlmNet;
using OpenGLHelper;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;

namespace DemoFlowVisualization
{
    internal class VortexDynamic2D: ISimulator
    {
        //  shaders for rendering
        private Shader _vortexShader;
        private Shader _renderShader;

        //  shaders for computing;
        private Shader _velocityComputeShader;
        private Shader _positionComputeShader;
        private Shader _positionComputeShader05;

        private Shader _clearTextureComputeShader;

        private StorageBuffer _vortexBuffer05;
        private StorageBuffer _vortexBuffer;
        private StorageBuffer _velocityBuffer;

        private VertexArray _vortexVAO;

        private vec3 nVortexes = new vec3(32, 16, 16);
        //private int _totalParticles;
        private int _vortexNumber;
        private float _eyePos = 2.0f;


        
        // private float _gamma = 0.1f;

        private float _time;
        private float _deltaTime = 0.0002f;
        //private float _speed = 35.0f;
        private float _angle;

        //  private VertexArray _particleVAO;
        
        private mat4 _projection;
        private mat4 _view;
        private mat4 _model;

        private Texture2D _texture;
        private int _textureHeight = 1024;
        private int _textureWidth = 1024;

        private int Height;
        private int Width;

        private Plane _plane;
        private float _xCenter;


        public VortexDynamic2D(int width, int height)
        {
            Width = width;
            Height = height;
            _vortexNumber = (int)(nVortexes.x * nVortexes.y * nVortexes.z);
             
            Console.WriteLine("Vortex Number: " + _vortexNumber);
            CreateShaders();

            _texture = new Texture2D(_textureWidth, _textureHeight, 0);
            _plane = new Plane(3.0f, 3.0f, 2, 2);
            _angle = 90.0f;

            // List<VortexStruct> vortexes = VortexInitializer.GetVortexesInCircle(nParticles);
             List<VortexStruct> vortexes = VortexInitializer.GetVortexesInLayer(nVortexes);
            
            _vortexBuffer = new StorageBuffer(BufferUsageHint.DynamicDraw);
            _vortexBuffer.SetData(vortexes.ToArray(), 0);

            _vortexBuffer05 = new StorageBuffer(BufferUsageHint.DynamicDraw);
            _vortexBuffer05.SetData(vortexes.ToArray(), 2);

            var initialVelocity = VortexInitializer.GetVelocity(nVortexes);
            _velocityBuffer = new StorageBuffer(BufferUsageHint.DynamicDraw);
            _velocityBuffer.SetData(initialVelocity.ToArray(), 1);

            // Set up a buffer and a VAO for drawing the vortexes
            _vortexVAO = new VertexArray(); ;
            _vortexBuffer.SetAttribPointer(0, 4);
            _velocityBuffer.SetAttribPointer(1, 2);
            _vortexVAO.Unbind();

            SetOpenGlParameters();
            SetInitialMatrix();
        }

        private void SetInitialMatrix()
        {
            _model = new mat4(1.0f);
            _projection = glm.perspective(glm.radians(60.0f), (float)Width / Height, 0.1f, 100.0f);
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

        public void ComputeAndDraw(bool isPause, bool is3D, bool isDrawVortex)
        {
            if (!isPause)
            {
                _time += _deltaTime;
                    //  if (_time % 0.1 <= _deltaTime)
                    Console.WriteLine("Time: " + _time);

                _clearTextureComputeShader.Compute(_textureWidth / 16, _textureHeight / 16, 1, MemoryBarrierFlags.ShaderImageAccessBarrierBit);

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

            if (is3D)
            {
                SetVortexShader3D();
              //  _velocityBuffer.SetAttribPointer(2, _vortexNumber);
                _vortexVAO.Draw(PrimitiveType.Points, 0, _vortexNumber);
            }
            else
            {
                _renderShader.Use();
                SetRenderShader();
                _plane.Render();
            }
        }

        private void SetOpenGlParameters()
        {
            GL.ClearColor(0, 0.0f, 0.0f, 1);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
            GL.PointSize(1.0f);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        }

        private void SetVortexShader3D()
        {
            _model = new mat4(1.0f);
            _view = glm.lookAt(new vec3(0, 0, 2.0f), new vec3(0, 0, 0), new vec3(0, 1, 0));
            _vortexShader.Use();
            _vortexShader.SetMatrix4("model", _model.ConvertToMatrix4());
            _vortexShader.SetMatrix4("projection", _projection.ConvertToMatrix4());
            _vortexShader.SetMatrix4("view", _view.ConvertToMatrix4());
            _vortexShader.SetVector4("Color", new Vector4(0.7f, 0.9f, 0.3f, 0.8f));
        }

        private void SetRenderShader()
        {
            //_angle = 90;
            _view = glm.lookAt(new vec3(0, 0, _eyePos), new vec3(0, 0, 0), new vec3(0, 1, 0));
            _model = glm.rotate(new mat4(1.0f), glm.radians(_angle), new vec3(1, 0.0f, 0.0f));
            mat4 mv = _view * _model;
            mat3 norm = new mat3(new vec3(mv[0]), new vec3(mv[1]), new vec3(mv[2]));
            mat4 proj = glm.perspective(glm.radians(60.0f), (float)Width / Height, 1.0f, 100.0f);

            _renderShader.SetMatrix4("model", _model.ConvertToMatrix4());
            _renderShader.SetMatrix4("view", _view.ConvertToMatrix4());
            _renderShader.SetMatrix4("projection", proj.ConvertToMatrix4());
            _renderShader.SetMatrix3("NormalMatrix", norm.ConvertToMatrix3());
        }

        public void SetEye(float eyePos)
        {
            _eyePos = eyePos;
        }

        public void SetHorizontal(float xPosition)
        {
            _xCenter = xPosition;
        }
    }
}
