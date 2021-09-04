using GlmNet;
using OpenGLHelper;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using System;
using TextureWrapMode = OpenTK.Graphics.OpenGL4.TextureWrapMode;

namespace ParticlesFeedBack
{
    public class Game : GameWindow
    {

        private Shader _flatShader;
        private Shader _particleShaderWithFeedBack;
        private ArrayBuffer[] _posBuf = new ArrayBuffer[2];
        private ArrayBuffer[] _velBuf = new ArrayBuffer[2];
        private ArrayBuffer[] _age = new ArrayBuffer[2];

        private VertexArray[] _particleVertexArray = new VertexArray[2];
        private TransformFeedback[] _feedback = new TransformFeedback[2];

        private int _drawBuf = 1;
        private int _nParticles = 5400;
        private float _angle;
        private float _time;
        private float _deltaT;
        private Grid _grid;

        private Texture2D _texture;
        private float particleLifetime = 4;
        private vec3 emitterPos = new vec3(0, 0, 0);
        private vec3 emitterDir = new vec3(-1, 6, 0);

        private float t;
        private float dt = 0.01f;

        private mat4 _projection;
        private mat4 _view;
        private mat4 _model;
        private Shader _triangleShader;

        public Game(int width, int height, string title) : base(width, height, GraphicsMode.Default, title)
        {

        }

        protected override void OnLoad(EventArgs e)
        {
            _flatShader = new Shader("../../Shaders/flat.vert", "../../Shaders/flat.frag");
            string[] feedbackVariables =  { "Position", "Velocity", "Age" };
            _particleShaderWithFeedBack = new Shader("../../Shaders/particles.vert", "../../Shaders/particles.frag",
                feedbackVariables);
            _triangleShader = new Shader("../../Shaders/triangle.vert", "../../Shaders/triangle.frag");

            _model = new mat4(1.0f);
            //const string texName = "../../Textures/bluewater.png";
            // const string texName = "../../Textures/water2.jpg";
            const string texName = "../../Textures/bubble.png";
            _texture = new Texture2D(texName, TextureWrapMode.Repeat);
            _texture.Use();

            Random rand = new Random();
            var size = 3 * _nParticles;
            float[] randData = new float[size];
            for (int i = 0; i < randData.Length; i++)
            {
                randData[i] = (float)rand.NextDouble();
            }

            var randomTexture = new Texture1D32(size, randData, TextureUnit.Texture1);

            //ParticleUtils.CreateRandomTexture1D(_nParticles * 3);

            GL.ClearColor(0.1f, 0.1f, 0.1f, 0.0f);

            //GL.PointSize(10.0f);
           // GL.Enable(EnableCap.ProgramPointSize);
           // GL.PointSize(5.0f);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            InitBuffers();
            _particleShaderWithFeedBack.SetInt("RandomTex", 1);
            _particleShaderWithFeedBack.SetInt("ParticleTexture", 0);
            _particleShaderWithFeedBack.SetFloat("ParticleLifetime", particleLifetime);
            _particleShaderWithFeedBack.SetVector3("Acceleration", new Vector3(0.0f, -0.5f, 0.0f));
            _particleShaderWithFeedBack.SetVector3("Emitter", emitterPos.ConvertToVector3());
            _particleShaderWithFeedBack.SetMatrix3("EmitterBasis", MakeArbitraryBasis(emitterDir));

            _particleShaderWithFeedBack.SetFloat("ParticleSize", 0.08f);

            _flatShader.SetVector4("Color", new Vector4(0.3f, 0.3f, 0.3f, 1.0f));

            _triangleShader.SetFloat("ParticleSize", 0.5f);

            base.OnLoad(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            t += dt;
            _deltaT = t - _time;
            _time = t;
            _angle = (float)((_angle + 0.01f) % (2 * Math.PI));
            _view = glm.lookAt(new vec3(4.0f * glm.cos(_angle), 1.5f, 4.0f * glm.sin(_angle)),
                new vec3(0.0f, 1.5f, 0.0f),
                new vec3(0.0f, 1.0f, 0.0f));

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            _flatShader.Use();


            SetMatrices(_flatShader);
            _grid.Render();

            _particleShaderWithFeedBack.SetFloat("DeltaT", _deltaT);

            // Update pass
            _particleShaderWithFeedBack.SetInt("Pass", 1);

            GL.Enable(EnableCap.RasterizerDiscard);

            _feedback[_drawBuf].Begin();
            _particleVertexArray[1 - _drawBuf].Bind();
            GL.VertexAttribDivisor(0, 0);
            GL.VertexAttribDivisor(1, 0);
            GL.VertexAttribDivisor(2, 0);
            _particleVertexArray[1 - _drawBuf].Draw(PrimitiveType.Points, 0, _nParticles);
            _feedback[_drawBuf].End();
            GL.Disable(EnableCap.RasterizerDiscard);

            // Render pass
            _particleShaderWithFeedBack.SetInt("Pass", 2);

            SetMatrices(_particleShaderWithFeedBack);

            GL.DepthMask(false);
            _particleVertexArray[_drawBuf].Bind();
            GL.VertexAttribDivisor(0, 1);
            GL.VertexAttribDivisor(1, 1);
            GL.VertexAttribDivisor(2, 1);
            GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, 6, _nParticles);
            //_particleVertexArray[_drawBuf].Draw(PrimitiveType.Points, 0, _nParticles);
            _particleVertexArray[_drawBuf].Unbind();

            SetMatrices(_triangleShader);
            GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, 6, 3);

            GL.DepthMask(true);

            // Swap _buffers
            _drawBuf = 1 - _drawBuf;
            Context.SwapBuffers();
        }

        /// <summary>
        /// Return a rotation matrix that rotates the y-axis to be parallel to direction.
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        public Matrix3 MakeArbitraryBasis(vec3 direction)
        {
            mat3 basis = new mat3(1);
            vec3 u, v, n;
            v = direction;
            n = glm.cross(new vec3(1, 0, 0), v);
            if ((float)Math.Sqrt(glm.dot(n, n)) < 0.00001f)
            {
                n = glm.cross(new vec3(0, 1, 0), v);
            }
            u = glm.cross(v, n);
            basis[0] = glm.normalize(u);
            basis[1] = glm.normalize(v);
            basis[2] = glm.normalize(n);
            return basis.ConvertToMatrix3();
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

        

        private void InitBuffers()
        {
            _grid = new Grid(10.0f, 10);
            int size = _nParticles * 3 * sizeof(float);
            // Generate the _buffers
            _posBuf[0] = new ArrayBuffer(BufferUsageHint.DynamicCopy);
            _posBuf[0].Allocate(size);

            _posBuf[1] = new ArrayBuffer(BufferUsageHint.DynamicCopy);
            _posBuf[1].Allocate(size);

            _velBuf[0] = new ArrayBuffer(BufferUsageHint.DynamicCopy);
            _velBuf[0].Allocate(size);

            _velBuf[1] = new ArrayBuffer(BufferUsageHint.DynamicCopy);
            _velBuf[1].Allocate(size);

            _age[0] = new ArrayBuffer(BufferUsageHint.DynamicCopy);
            //_age[0].Allocate(size / 3);
            var tempData = CreateData();
            _age[0].SetData(tempData);

            _age[1] = new ArrayBuffer(BufferUsageHint.DynamicCopy);
            _age[1].Allocate(size / 3);
            // Allocate space for all _buffers
            
            // Create vertex arrays for each set of _buffers
            _particleVertexArray[0] = new VertexArray();
                _posBuf[0].SetAttribPointer(0, 3);
                _velBuf[0].SetAttribPointer(1, 3);
                _age[0].SetAttribPointer(2, 1);
            _particleVertexArray[0].Unbind();



            _particleVertexArray[1] = new VertexArray();
                _posBuf[1].SetAttribPointer(0, 3);
                _velBuf[1].SetAttribPointer(1, 3);
                _age[1].SetAttribPointer(2, 1);
            _particleVertexArray[1].Unbind();
            

            // Transform _feedback 0
            _feedback[0] = new TransformFeedback();
                _posBuf[0].BindBase(BufferRangeTarget.TransformFeedbackBuffer, 0);
                _velBuf[0].BindBase(BufferRangeTarget.TransformFeedbackBuffer, 1);
                _age[0].BindBase(BufferRangeTarget.TransformFeedbackBuffer, 2);
            _feedback[0].UnBind();

            // Transform _feedback 1
            _feedback[1] = new TransformFeedback();
                _posBuf[1].BindBase(BufferRangeTarget.TransformFeedbackBuffer, 0);
                _velBuf[1].BindBase(BufferRangeTarget.TransformFeedbackBuffer, 1);
                _age[1].BindBase(BufferRangeTarget.TransformFeedbackBuffer, 2);
            _feedback[1].UnBind();

            int value = GL.GetInteger(GetPName.MaxTransformFeedbackBuffers);
            Console.WriteLine("MAX_TRANSFORM_FEEDBACK_BUFFERS = " + value);
        }

        private float[] CreateData()
        {
            var tempData = new float[_nParticles];
            float rate = particleLifetime / _nParticles;

            for (int i = 0; i < _nParticles; i++)
            {
                tempData[i] = rate * (i - _nParticles);
            }

            return tempData;
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

        private void SetMatrices(Shader shader)
        {
            shader.SetMatrix4("model", _model.ConvertToMatrix4());
            shader.SetMatrix4("view", _view.ConvertToMatrix4());
            shader.SetMatrix4("projection", _projection.ConvertToMatrix4());
        }

    }
}
