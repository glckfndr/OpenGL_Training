using GlmNet;
using OpenGLHelper;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using System;
using System.Threading;

namespace ParticlesSmoke
{
    public class Game : GameWindow
    {

        private ArrayBuffer[] _posBuf;// = new ArrayBuffer[2];
        private ArrayBuffer[] _velBuf = new ArrayBuffer[2];
        private ArrayBuffer[] _startTime = new ArrayBuffer[2];
        private VertexArray[] _particleVertexArray;// = new VertexArray[2];
        private TransformFeedback[] _feedback = new TransformFeedback[2];
        private ArrayBuffer _initVel;

        private int _drawBuf = 1;
        private int _nParticles = 5000;
        private float _angle;
        private float _time;
        private float _deltaT;

        private Texture _texture;


        private float t;
        private float dt = 0.05f;

        private mat4 _projection;
        private mat4 _view;
        private mat4 _model;
        private Shader _shader;
        private int _updateSub;
        private int _renderSub;
        private float pi = (float) Math.PI;


        public Game(int width, int height, string title) : base(width, height, GraphicsMode.Default, title)
        {

        }

        protected override void OnLoad(EventArgs e)
        {
            // compileAndLinkShader();
            string[] outputNames = new string[] { "Position", "Velocity", "StartTime" };
            _shader = new Shader("../../Shaders/smoke.vert", "../../Shaders/smoke.frag", outputNames);

            //int programHandle = _shader.GetHandle();
            _renderSub = _shader.GetSubroutineIndex(ShaderType.VertexShader, "render");
            _updateSub = _shader.GetSubroutineIndex(ShaderType.VertexShader, "update");

            GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);

            GL.Enable(EnableCap.ProgramPointSize);
            GL.PointSize(10.0f);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            _angle = (float)(Math.PI / 2);
            _model = new mat4(1.0f);
            _view = glm.lookAt(new vec3(3.0f * glm.cos(_angle), 1.5f, 3.0f * glm.sin(_angle)),
                            new vec3(0.0f, 1.5f, 0.0f),
                new vec3(0.0f, 1.0f, 0.0f));
            _projection = glm.perspective(glm.radians(60.0f), (float)Width / Height, 0.3f, 100.0f);

            InitBuffers();

            const string texName = "../../Textures/smoke.png";
            //const string texName = "../../Textures/water2.jpg";
            _texture = new Texture(texName, TextureWrapMode.Repeat, TextureUnit.Texture0, TextureMinFilter.Nearest, TextureMagFilter.Nearest);
            GL.Enable(EnableCap.PointSprite);
            GL.PointParameter(PointParameterName.PointSpriteCoordOrigin, (int)PointSpriteCoordOriginParameter.LowerLeft);
            _texture.Use();
            
            //GL.ActiveTexture(TextureUnit.Texture0);
            //var tex = Texture.LoadTexture(texName);
            _shader.SetInt("ParticleTex", 0);
            _shader.SetFloat("ParticleLifetime", 10.0f);
            _shader.SetVector3("Accel", new Vector3(0.0f, 0.1f, 0.0f));
            SetMatrices(_shader);

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
           // Thread.Sleep(200);

            t += dt;
            _deltaT = t - _time;
            _time = t;
            _angle = (float)((_angle + 0.01f) % (2 * Math.PI));

            // Update pass
            GL.UniformSubroutines(ShaderType.VertexShader, 1, ref _updateSub);
            _shader.SetFloat("Time", _time);
            _shader.SetFloat("H", _deltaT);

            GL.Enable(EnableCap.RasterizerDiscard);

            _feedback[_drawBuf].Bind();
            _feedback[_drawBuf].Begin();
            _particleVertexArray[1 - _drawBuf].Draw(PrimitiveType.Points, 0, _nParticles);
            _feedback[_drawBuf].End();
            GL.Disable(EnableCap.RasterizerDiscard);

            // Render pass
            GL.UniformSubroutines(ShaderType.VertexShader, 1, ref _renderSub);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            _view = glm.lookAt(new vec3(3.0f * glm.cos(_angle), 1.5f, 3.0f * glm.sin(_angle)),
                            new vec3(0.0f, 1.5f, 0.0f),
                        new vec3(0.0f, 1.0f, 0.0f));
            SetMatrices(_shader);

            _particleVertexArray[_drawBuf].Bind();
            _feedback[_drawBuf].Draw();

            // Swap _buffers
            _drawBuf = 1 - _drawBuf;
            Context.SwapBuffers();
        }

        private void InitBuffers()
        {
            // Generate the _buffers
            // Allocate space for all _buffers
            int size = _nParticles * 3 * sizeof(float);
            _posBuf = new[] {new ArrayBuffer(BufferUsageHint.DynamicCopy), new ArrayBuffer(BufferUsageHint.DynamicCopy) };
            //_posBuf[0].Allocate(size);
            //_posBuf[1] = new ArrayBuffer(BufferUsageHint.DynamicCopy);
            //_posBuf[1].Allocate(size);
            foreach (var buffer in _posBuf)
            {
                buffer.Allocate(size);
            }

            _velBuf = new[] { new ArrayBuffer(BufferUsageHint.DynamicCopy), new ArrayBuffer(BufferUsageHint.DynamicCopy) };
            //_velBuf[0] = new ArrayBuffer(BufferUsageHint.DynamicCopy);
            //_velBuf[0].Allocate(size);
            //_velBuf[1] = new ArrayBuffer(BufferUsageHint.DynamicCopy);
            //_velBuf[1].Allocate(size);
            foreach (var buffer in _velBuf)
            {
                buffer.Allocate(size);
            }

            _initVel = new ArrayBuffer(BufferUsageHint.StaticDraw);
            _initVel.Allocate(size);


            _startTime = new[] { new ArrayBuffer(BufferUsageHint.DynamicCopy), new ArrayBuffer(BufferUsageHint.DynamicCopy) };
            //_startTime[0] = new ArrayBuffer(BufferUsageHint.DynamicCopy);
            //_startTime[0].Allocate(size / 3);
            //_startTime[1] = new ArrayBuffer(BufferUsageHint.DynamicCopy);
            //_startTime[1].Allocate(size / 3);
            foreach (var buffer in _startTime)
            {
                buffer.Allocate(size/3);
            }

            // Fill the first position buffer with zeroes
            var tempData = new float[_nParticles * 3];
            var rnd = new Random();
            for (int i = 0; i < _nParticles * 3; i += 3)
            {
                tempData[i] = 0.0f;
                tempData[i + 1] = 0.0f;
                tempData[i + 2] = 0.0f;
            }

            _posBuf[0].SetData(tempData);
            // Fill the first velocity buffer with random velocities
            float theta, phi, velocity;
            vec3 v = new vec3();
            for (int i = 0; i < _nParticles * 3; i += 3)
            {
                theta = Mix(0.0f, pi / 1.5f, rnd.NextDouble());
                phi = Mix(0.0f, 2*pi, rnd.NextDouble());

                v.x = glm.sin(theta) * glm.cos(phi);
                v.y = glm.cos(theta);
                v.z = glm.sin(theta) * glm.sin(phi);

                velocity = Mix(0.1f, 0.2f, rnd.NextDouble());
                v = glm.normalize(v) * velocity;

                tempData[i] = v.x;
                tempData[i + 1] = v.y;
                tempData[i + 2] = v.z;
            }

            _velBuf[0].SetData(tempData);
            _initVel.SetData(tempData);

            // Fill the first start time buffer
            tempData = new float[_nParticles];
            float time = 0.0f;
            float rate = 0.01f;
            for (int i = 0; i < _nParticles; i++)
            {
                tempData[i] = time;
                time += rate;
            }
            _startTime[0].SetData(tempData);
            // Create vertex arrays for each set of _buffers
            _particleVertexArray = new[] { new VertexArray(), new VertexArray()};
            // Set up particle array 0
            _particleVertexArray[0].Bind();

            _posBuf[0].SetAttribPointer(0, 3);
            _velBuf[0].SetAttribPointer(1, 3);
            _startTime[0].SetAttribPointer(2, 1);
            _initVel.SetAttribPointer(3, 3);

            _particleVertexArray[0].Unbind();

            // Set up particle array 1
            _particleVertexArray[1].Bind();

            _posBuf[1].SetAttribPointer(0, 3);
            _velBuf[1].SetAttribPointer(1, 3);
            _startTime[1].SetAttribPointer(2, 1);
            _initVel.SetAttribPointer(3, 3);

            _particleVertexArray[1].Unbind();

            // Setup the _feedback objects
            _feedback[0] = new TransformFeedback();
            _feedback[1] = new TransformFeedback();

            // Transform _feedback 0
            _feedback[0].Bind();

            _posBuf[0].BindBase(BufferRangeTarget.TransformFeedbackBuffer, 0);
            _velBuf[0].BindBase(BufferRangeTarget.TransformFeedbackBuffer, 1);
            _startTime[0].BindBase(BufferRangeTarget.TransformFeedbackBuffer, 2);

            _feedback[0].UnBind();

            // Transform _feedback 1

            _feedback[1].Bind();

            _posBuf[1].BindBase(BufferRangeTarget.TransformFeedbackBuffer, 0);
            _velBuf[1].BindBase(BufferRangeTarget.TransformFeedbackBuffer, 1);
            _startTime[1].BindBase(BufferRangeTarget.TransformFeedbackBuffer, 2);

            _feedback[1].UnBind();

            int value = GL.GetInteger(GetPName.MaxTransformFeedbackBuffers);
            Console.WriteLine("MAX_TRANSFORM_FEEDBACK_BUFFERS = " + value);
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

            base.OnUnload(e);
        }

        private float Mix(double x, double y, double a)
        {
            return (float)((1.0 - a) * x + a * y);
        }

        private void SetMatrices(Shader prog)
        {
            prog.Use();
            mat4 mv = _view * _model;
            prog.SetMatrix4("MVP", (_projection * mv).ConvertToMatrix4());
        }
    }
}

