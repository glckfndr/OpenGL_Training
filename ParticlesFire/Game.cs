using GlmNet;
using OpenGLHelper;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using System;
using TextureWrapMode = OpenTK.Graphics.OpenGL4.TextureWrapMode;

namespace ParticlesFire
{
    public class Game : GameWindow
    {

        private Shader _shader;


        private int[] _posBuf = new int[2];
        private int[] _velBuf = new int[2];
        private int[] _startTime = new int[2];

        private int[] _particleVertexArray = new int[2];
        private int[] _feedback = new int[2];
        private int _initVel;

        private int _drawBuf = 1;
        private int _query;
        private int _renderSub;
        private int _updateSub;
        private int _nParticles = 4000;
        private float _angle = 0;
        private float _time = 0;
        private float _deltaT = 0;
        //private Grid _grid;

        private Texture _texture;
        private float particleLifetime = 6;

        private float t = 0;
        private float dt = 0.05f;

        private mat4 _projection;
        private mat4 _view;
        private mat4 _model;
        private GLSLProgram _prog;
        private int updateSub;
        private int renderSub;



        public Game(int width, int height, string title) : base(width, height, GraphicsMode.Default, title)
        {

        }

        protected override void OnLoad(EventArgs e)
        {
            compileAndLinkShader();

            int programHandle = _prog.GetHandle();
            renderSub = GL.GetSubroutineIndex(programHandle, ShaderType.VertexShader, "render");
            updateSub = GL.GetSubroutineIndex(programHandle, ShaderType.VertexShader, "update");

            GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            GL.PointSize(50.0f);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            _angle = (float)(Math.PI / 2);
            _model = new mat4(1.0f);
            _view = glm.lookAt(new vec3(3.0f * glm.cos(_angle), 1.5f, 3.0f * glm.sin(_angle)),
                new vec3(0.0f, 1.5f, 0.0f),
                new vec3(0.0f, 1.0f, 0.0f));
            _projection = glm.perspective(glm.radians(60.0f), (float)Width / Height, 0.3f, 100.0f);

            InitBuffers();

            //const string texName = "../../Textures/water2.jpg";
            const string texName = "../../Textures/fire.png";
            //_texture = new Texture(texName, TextureWrapMode.ClampToEdge);
            //_texture.Use();
            GL.ActiveTexture(TextureUnit.Texture0);
            var tex = Texture.LoadTexture(texName);

            _prog.Use();
            _prog.SetInt("ParticleTex", 0);
            _prog.SetFloat("ParticleLifetime", 4.0f);
            _prog.SetVector3("Accel", new Vector3(0.0f, 0.1f, 0.0f));
            SetMatrices(_prog);

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
            //   Thread.Sleep(500);

            t += dt;
            _deltaT = t - _time;
            _time = t;
            _angle = (float)((_angle + 0.01f) % (2 * Math.PI));

            // Update pass
            GL.UniformSubroutines(ShaderType.VertexShader, 1, ref updateSub);
            _prog.SetFloat("Time", _time);
            _prog.SetFloat("H", _deltaT);
            
            GL.Enable(EnableCap.RasterizerDiscard);

            GL.BindTransformFeedback(TransformFeedbackTarget.TransformFeedback, _feedback[_drawBuf]);
            GL.BeginTransformFeedback(TransformFeedbackPrimitiveType.Points);
            GL.BindVertexArray(_particleVertexArray[1 - _drawBuf]);
            GL.DrawArrays(PrimitiveType.Points, 0, _nParticles);
            GL.EndTransformFeedback();
            GL.Disable(EnableCap.RasterizerDiscard);

            // Render pass
            GL.UniformSubroutines(ShaderType.VertexShader, 1, ref renderSub);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            _view = glm.lookAt(new vec3(3.0f * glm.cos(_angle), 1.5f, 3.0f * glm.sin(_angle)),
                        new vec3(0.0f, 1.5f, 0.0f),
                        new vec3(0.0f, 1.0f, 0.0f));
            SetMatrices(_prog);

            GL.BindVertexArray(_particleVertexArray[_drawBuf]);
            GL.DrawTransformFeedback(PrimitiveType.Points, _feedback[_drawBuf]);

            // Swap _buffers
            _drawBuf = 1 - _drawBuf;
            Context.SwapBuffers();
        }

        private void InitBuffers()
        {
            _nParticles = 4000;


            // Generate the _buffers
            GL.GenBuffers(2, _posBuf);    // position _buffers
            GL.GenBuffers(2, _velBuf);    // velocity _buffers
            GL.GenBuffers(2, _startTime); // Start _time _buffers
            _initVel = GL.GenBuffer();

            // Allocate space for all _buffers
            int size = _nParticles * 3 * sizeof(float);
            AllocateArrayBuffer(size, _posBuf[0], BufferUsageHint.DynamicCopy);
            AllocateArrayBuffer(size, _posBuf[1], BufferUsageHint.DynamicCopy);

            AllocateArrayBuffer(size, _velBuf[0], BufferUsageHint.DynamicCopy);
            AllocateArrayBuffer(size, _velBuf[1], BufferUsageHint.DynamicCopy);

            AllocateArrayBuffer(size, _initVel, BufferUsageHint.StaticDraw);

            AllocateArrayBuffer(size / 3, _startTime[0], BufferUsageHint.DynamicCopy);
            AllocateArrayBuffer(size / 3, _startTime[1], BufferUsageHint.DynamicCopy);

            // Fill the first position buffer with zeroes
            var tempData = new float[_nParticles * 3];
            var rnd = new Random();
            for (int i = 0; i < _nParticles * 3; i += 3)
            {
                tempData[i] = Mix(-2.0f, 2.0f, rnd.NextDouble());
                tempData[i + 1] = 0.0f;
                tempData[i + 2] = 0.0f;

            }

            CopyDataToArrayBuffer(tempData, _posBuf[0]);
            // Fill the first velocity buffer with random velocities

            for (int i = 0; i < _nParticles * 3; i += 3)
            {
                tempData[i] = 0.0f;
                tempData[i + 1] = Mix(0.1f, 0.5f, rnd.NextDouble()); ;
                tempData[i + 2] = 0.0f;

            }
            CopyDataToArrayBuffer(tempData, _velBuf[0]);
            CopyDataToArrayBuffer(tempData, _initVel);

            // Fill the first start time buffer
            tempData = new float[_nParticles];
            float time = 0.0f;
            float rate = 0.001f;
            for (int i = 0; i < _nParticles; i++)
            {
                tempData[i] = time;
                time += rate;
            }
            CopyDataToArrayBuffer(tempData, _startTime[0]);
            // Create vertex arrays for each set of _buffers
            GL.GenVertexArrays(2, _particleVertexArray);

            // Set up particle array 0
            GL.BindVertexArray(_particleVertexArray[0]);

            SetFloatPointer(_posBuf[0], 0, 3);
            SetFloatPointer(_velBuf[0], 1, 3);
            SetFloatPointer(_startTime[0], 2, 1);
            SetFloatPointer(_initVel, 3, 3);

            GL.BindVertexArray(0);

            // Set up particle array 1

            GL.BindVertexArray(_particleVertexArray[1]);

            SetFloatPointer(_posBuf[1], 0, 3);
            SetFloatPointer(_velBuf[1], 1, 3);
            SetFloatPointer(_startTime[1], 2, 1);
            SetFloatPointer(_initVel, 3, 3);

            GL.BindVertexArray(0);

            // Setup the _feedback objects
            GL.GenTransformFeedbacks(2, _feedback);

            // Transform _feedback 0
            GL.BindTransformFeedback(TransformFeedbackTarget.TransformFeedback, _feedback[0]);

            GL.BindBufferBase(BufferRangeTarget.TransformFeedbackBuffer, 0, _posBuf[0]);
            GL.BindBufferBase(BufferRangeTarget.TransformFeedbackBuffer, 1, _velBuf[0]);
            GL.BindBufferBase(BufferRangeTarget.TransformFeedbackBuffer, 2, _startTime[0]);

            GL.BindTransformFeedback(TransformFeedbackTarget.TransformFeedback, 0);

            // Transform _feedback 1
            GL.BindTransformFeedback(TransformFeedbackTarget.TransformFeedback, _feedback[1]);

            GL.BindBufferBase(BufferRangeTarget.TransformFeedbackBuffer, 0, _posBuf[1]);
            GL.BindBufferBase(BufferRangeTarget.TransformFeedbackBuffer, 1, _velBuf[1]);
            GL.BindBufferBase(BufferRangeTarget.TransformFeedbackBuffer, 2, _startTime[1]);

            GL.BindTransformFeedback(TransformFeedbackTarget.TransformFeedback, 0);

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

        private void SetFloatPointer(int bufferIndex, int index, int size)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, bufferIndex);
            GL.VertexAttribPointer(index, size, VertexAttribPointerType.Float, false, 0, IntPtr.Zero);
            GL.EnableVertexAttribArray(index);
        }

        private void CopyDataToArrayBuffer(float[] data, int bufferIndex)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, bufferIndex);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, data.Length * sizeof(float), data);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        private void AllocateArrayBuffer(int size, int bufferIndex, BufferUsageHint hint)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, bufferIndex);
            GL.BufferData(BufferTarget.ArrayBuffer, size, IntPtr.Zero, hint);
        }

        private float Mix(double x, double y, double a)
        {
            return (float)((1.0 - a) * x + a * y);
        }

        private void SetMatrices(GLSLProgram prog)
        {


            prog.Use();


            mat4 mv = _view * _model;
            prog.SetMatrix4("MVP", (_projection * mv).ConvertToMatrix4());


        }

        private void compileAndLinkShader()
        {
            // LoadSource is a simple function that just loads all text from the file whose path is given.

            var vertexShader = Shader.CreateShader("../../Shaders/fire.vert", ShaderType.VertexShader);
            Shader.CompileShader(vertexShader);
            var fragmentShader = Shader.CreateShader("../../Shaders/fire.frag", ShaderType.FragmentShader);
            Shader.CompileShader(fragmentShader);

            _prog = new GLSLProgram();
            _prog.Attach(vertexShader, fragmentShader);
            //////////////////////////////////////////////////////
            // Setup the transform feedback (must be done before linking the program)
            string[] outputNames = new string[] { "Position", "Velocity", "StartTime" };
            GL.TransformFeedbackVaryings(_prog.GetHandle(), 3, outputNames, TransformFeedbackMode.SeparateAttribs);

            _prog.Link();
            _prog.Use();
            _prog.GetUniforms();






        }
    }
}
