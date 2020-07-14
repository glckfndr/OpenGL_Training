using GlmNet;
using OpenGLHelper;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using System;
using System.Threading;
using TextureWrapMode = OpenTK.Graphics.OpenGL4.TextureWrapMode;

namespace ParticlesFeedBack
{
    public class Game : GameWindow
    {

        private Shader _shader;


        private int[] _posBuf = new int[2];
        private int[] _velBuf = new int[2];
        private int[] _age = new int[2];

        private int[] _particleVertexArray = new int[2];
        private int[] _feedback = new int[2];
       // private int _initVel;

        private int _drawBuf = 1;
        private int _query;
        private int _renderSub;
        private int _updateSub;
        private int _nParticles = 5400;
        private float _angle = 0;
        private float _time = 0;
        private float _deltaT = 0;
        private Grid _grid;

        private Texture _texture;
        private float particleLifetime = 6;
        private vec3 emitterPos = new vec3(0, 0, 0);
        private vec3 emitterDir = new vec3(-1, 2, 0);

        private float t = 0;
        private float dt = 0.01f;

        private mat4 _projection;
        private mat4 _view;
        private mat4 _model;
        private GLSLProgram _prog;
        private GLSLProgram _flatProg;

        public Game(int width, int height, string title) : base(width, height, GraphicsMode.Default, title)
        {

        }

        protected override void OnLoad(EventArgs e)
        {
            compileAndLinkShader();

            _model = new mat4(1.0f);
            const string texName = "../../Textures/bluewater.png";
            _texture = new Texture(texName, TextureWrapMode.Repeat);
            _texture.Use();

            GL.ActiveTexture(TextureUnit.Texture1);
            ParticleUtils.CreateRandomTex1D(_nParticles * 3);

            GL.ClearColor(0.1f, 0.1f, 0.1f, 0.0f);

            //GL.PointSize(10.0f);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            InitBuffers();

            _prog.Use();
            _prog.SetInt("RandomTex", 1);
            _prog.SetInt("ParticleTex", 0);
            _prog.SetFloat("ParticleLifetime", particleLifetime);
            _prog.SetVector3("Accel", new Vector3(0.0f, -0.5f, 0.0f));
            _prog.SetFloat("ParticleSize", 0.03f);
            _prog.SetVector3("Emitter", emitterPos.ConvertToVector3());
            _prog.SetMatrix3("EmitterBasis", ParticleUtils.MakeArbitraryBasis(emitterDir).ConvertToMatrix3());

            _flatProg.Use();
            _flatProg.SetVector4("Color", new Vector4(0.3f, 0.3f, 0.3f, 1.0f));
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

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            _flatProg.Use();

            _view = glm.lookAt(new vec3(4.0f * glm.cos(_angle), 1.5f, 4.0f * glm.sin(_angle)),
                new vec3(0.0f, 1.5f, 0.0f),
                new vec3(0.0f, 1.0f, 0.0f));
            SetMatrices(_flatProg);
           _grid.Render();

            _prog.Use();
            //  _prog.SetFloat("Time", _time);
            _prog.SetFloat("DeltaT", _deltaT);

            // Update pass
            _prog.SetInt("Pass", 1);

            GL.Enable(EnableCap.RasterizerDiscard);
            GL.BindTransformFeedback(TransformFeedbackTarget.TransformFeedback, _feedback[_drawBuf]);
            GL.BeginTransformFeedback(TransformFeedbackPrimitiveType.Points);

            GL.BindVertexArray(_particleVertexArray[1 - _drawBuf]);
                GL.VertexAttribDivisor(0, 0);
                GL.VertexAttribDivisor(1, 0);
                GL.VertexAttribDivisor(2, 0);
                GL.DrawArrays(PrimitiveType.Points, 0, _nParticles);
            GL.BindVertexArray(0);

            GL.EndTransformFeedback();
            GL.Disable(EnableCap.RasterizerDiscard);

            // Render pass
            _prog.SetInt("Pass", 2);

            SetMatrices(_prog);

            GL.DepthMask(false);

            GL.BindVertexArray(_particleVertexArray[_drawBuf]);
                GL.VertexAttribDivisor(0, 1);
                GL.VertexAttribDivisor(1, 1);
                GL.VertexAttribDivisor(2, 1);
                GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, 6, _nParticles);
            GL.BindVertexArray(0);

            GL.DepthMask(true);

            // Swap _buffers
            _drawBuf = 1 - _drawBuf;
            Context.SwapBuffers();
        }

        private void InitBuffers()
        {
           // _nParticles = 4000;
            _grid = new Grid(10.0f, 10);

            // Generate the _buffers
            GL.GenBuffers(2, _posBuf);    // position _buffers
            GL.GenBuffers(2, _velBuf);    // velocity _buffers
            GL.GenBuffers(2, _age); // Start _time _buffers

            // Allocate space for all _buffers
            int size = _nParticles * 3 * sizeof(float);
            AllocateArrayBuffer(size, _posBuf[0], BufferUsageHint.DynamicCopy);
            AllocateArrayBuffer(size, _posBuf[1], BufferUsageHint.DynamicCopy);
            AllocateArrayBuffer(size, _velBuf[0], BufferUsageHint.DynamicCopy);
            AllocateArrayBuffer(size, _velBuf[1], BufferUsageHint.DynamicCopy);

            AllocateArrayBuffer(size / 3, _age[0], BufferUsageHint.DynamicCopy);
            AllocateArrayBuffer(size / 3, _age[1], BufferUsageHint.DynamicCopy);

            var tempData = new float[_nParticles];
            float rate = particleLifetime / _nParticles;

            for (int i = 0; i < _nParticles; i++)
            {
                tempData[i] = rate *(i - _nParticles);

            }

            CopyDataToArrayBuffer(tempData, _age[0]);

            // Create vertex arrays for each set of _buffers
            GL.GenVertexArrays(2, _particleVertexArray);

            // Set up particle array 0
            GL.BindVertexArray(_particleVertexArray[0]);

            SetFloatPointer(_posBuf[0], 0, 3);
            SetFloatPointer(_velBuf[0], 1, 3);
            SetFloatPointer(_age[0], 2, 1);
           // SetFloatPointer(_initVel, 3, 3);

            GL.BindVertexArray(0);

            // Set up particle array 1

            GL.BindVertexArray(_particleVertexArray[1]);

            SetFloatPointer(_posBuf[1], 0, 3);
            SetFloatPointer(_velBuf[1], 1, 3);
            SetFloatPointer(_age[1], 2, 1);
            //SetFloatPointer(_initVel, 3, 3);

            GL.BindVertexArray(0);

            // Setup the _feedback objects
            GL.GenTransformFeedbacks(2, _feedback);

            // Transform _feedback 0
            GL.BindTransformFeedback(TransformFeedbackTarget.TransformFeedback, _feedback[0]);

            GL.BindBufferBase(BufferRangeTarget.TransformFeedbackBuffer, 0, _posBuf[0]);
            GL.BindBufferBase(BufferRangeTarget.TransformFeedbackBuffer, 1, _velBuf[0]);
            GL.BindBufferBase(BufferRangeTarget.TransformFeedbackBuffer, 2, _age[0]);

            GL.BindTransformFeedback(TransformFeedbackTarget.TransformFeedback, 0);

            // Transform _feedback 1
            GL.BindTransformFeedback(TransformFeedbackTarget.TransformFeedback, _feedback[1]);

            GL.BindBufferBase(BufferRangeTarget.TransformFeedbackBuffer, 0, _posBuf[1]);
            GL.BindBufferBase(BufferRangeTarget.TransformFeedbackBuffer, 1, _velBuf[1]);
            GL.BindBufferBase(BufferRangeTarget.TransformFeedbackBuffer, 2, _age[1]);

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
            //_shape.DeleteBuffers();
            //_shader._handle.Delete();
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

            // mat4 mv = _model * _view;
            _prog.Use();
            prog.SetMatrix4("model", _model.ConvertToMatrix4());
            prog.SetMatrix4("view", _view.ConvertToMatrix4());
            prog.SetMatrix4("projection", _projection.ConvertToMatrix4());

            //_prog.Use();
            ////mat4 mv = _model * _view; 
            ////_shader.SetMatrix4("MVP", (mv * _projection).ConvertToMatrix4());

            //float cosa = (float)(float)Math.Cos(Math.PI / 3);
            //float sina = (float)Math.Sin(Math.PI / 3);

            //Matrix4 _projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60.0f),
            //    (float)Width / (float)Height, 0.1f, 100.0f);
            //Matrix4 _model = Matrix4.Identity * Matrix4.CreateRotationY((float)MathHelper.DegreesToRadians(30)); ;
            //Matrix4 _view = Matrix4.LookAt(new Vector3(4.0f * cosa, 0.0f, 4.0f * sina),
            //    new Vector3(0.0f, 1.5f, 0.0f),
            //    new Vector3(0.0f, 1.0f, 0.0f));


            //var MVP = _model;// * _view;
            ////MVP = MVP * _projection;
            //_shader.SetMatrix4("MVP", MVP);

        }

        private void compileAndLinkShader()
        {
            // LoadSource is a simple function that just loads all text from the file whose path is given.

            var vertexShader = Shader.CreateShader("../../Shaders/particles.vert", ShaderType.VertexShader);
            Shader.CompileShader(vertexShader);
            var fragmentShader = Shader.CreateShader("../../Shaders/particles.frag", ShaderType.FragmentShader);
            Shader.CompileShader(fragmentShader);

            _prog = new GLSLProgram();
            _prog.Attach(vertexShader, fragmentShader);
            //////////////////////////////////////////////////////
            // Setup the transform feedback (must be done before linking the program)
            string[] outputNames = new string[] { "Position", "Velocity", "Age" };
            GL.TransformFeedbackVaryings(_prog.GetHandle(), 3, outputNames, TransformFeedbackMode.SeparateAttribs);

            _prog.Link();
            _prog.Use();
            //_handle.ClearShaders(computeShader);
            _prog.GetUniforms();
            

            vertexShader = Shader.CreateShader("../../Shaders/flat.vert", ShaderType.VertexShader);
            Shader.CompileShader(vertexShader);
            fragmentShader = Shader.CreateShader("../../Shaders/flat.frag", ShaderType.FragmentShader);
            Shader.CompileShader(fragmentShader);
            _flatProg = new GLSLProgram();
            _flatProg.Attach(vertexShader, fragmentShader);
            _flatProg.Link();
            _flatProg.GetUniforms();



        }
    }
}
