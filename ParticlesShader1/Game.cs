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
        private Shader _particleShader;
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

        private Texture _texture;
        private float particleLifetime = 4;
        private vec3 emitterPos = new vec3(0, 0, 0);
        private vec3 emitterDir = new vec3(-1, 6, 0);

        private float t;
        private float dt = 0.01f;

        private mat4 _projection;
        private mat4 _view;
        private mat4 _model;
       
        public Game(int width, int height, string title) : base(width, height, GraphicsMode.Default, title)
        {

        }

        protected override void OnLoad(EventArgs e)
        {
            _flatShader = new Shader("../../Shaders/flat.vert", "../../Shaders/flat.frag");
            string[] feedbackVariables =  { "Position", "Velocity", "Age" };
            _particleShader = new Shader("../../Shaders/particles.vert", "../../Shaders/particles.frag",
                feedbackVariables);
            
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
            _particleShader.SetInt("RandomTex", 1);
            _particleShader.SetInt("ParticleTex", 0);
            _particleShader.SetFloat("ParticleLifetime", particleLifetime);
            _particleShader.SetVector3("Accel", new Vector3(0.0f, -0.5f, 0.0f));
            _particleShader.SetFloat("ParticleSize", 0.03f);
            _particleShader.SetVector3("Emitter", emitterPos.ConvertToVector3());
            _particleShader.SetMatrix3("EmitterBasis", ParticleUtils.MakeArbitraryBasis(emitterDir).ConvertToMatrix3());

            _flatShader.SetVector4("Color", new Vector4(0.3f, 0.3f, 0.3f, 1.0f));
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
            t += dt;
            _deltaT = t - _time;
            _time = t;
            _angle = (float)((_angle + 0.01f) % (2 * Math.PI));

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            _flatShader.Use();

            _view = glm.lookAt(new vec3(4.0f * glm.cos(_angle), 1.5f, 4.0f * glm.sin(_angle)),
                            new vec3(0.0f, 1.5f, 0.0f),
                                new vec3(0.0f, 1.0f, 0.0f));
            SetMatrices(_flatShader);
            _grid.Render();

            _particleShader.SetFloat("DeltaT", _deltaT);

            // Update pass
            _particleShader.SetInt("Pass", 1);

            GL.Enable(EnableCap.RasterizerDiscard);
            _feedback[_drawBuf].Bind();
            _feedback[_drawBuf].Begin();
            _particleVertexArray[1 - _drawBuf].Bind();
            GL.VertexAttribDivisor(0, 0);
            GL.VertexAttribDivisor(1, 0);
            GL.VertexAttribDivisor(2, 0);
            _particleVertexArray[1 - _drawBuf].Draw(PrimitiveType.Points, 0, _nParticles);

            _feedback[_drawBuf].End();
            GL.Disable(EnableCap.RasterizerDiscard);

            // Render pass
            _particleShader.SetInt("Pass", 2);

            SetMatrices(_particleShader);

            GL.DepthMask(false);
            _particleVertexArray[_drawBuf].Bind();
            GL.VertexAttribDivisor(0, 1);
            GL.VertexAttribDivisor(1, 1);
            GL.VertexAttribDivisor(2, 1);
            GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, 6, _nParticles);
            _particleVertexArray[_drawBuf].Unbind();

            GL.DepthMask(true);

            // Swap _buffers
            _drawBuf = 1 - _drawBuf;
            Context.SwapBuffers();
        }

        private void InitBuffers()
        {
            _grid = new Grid(10.0f, 10);

            // Generate the _buffers
            _posBuf[0] = new ArrayBuffer(BufferUsageHint.DynamicCopy);
            _posBuf[1] = new ArrayBuffer(BufferUsageHint.DynamicCopy);

            _velBuf[0] = new ArrayBuffer(BufferUsageHint.DynamicCopy);
            _velBuf[1] = new ArrayBuffer(BufferUsageHint.DynamicCopy);

            _age[0] = new ArrayBuffer(BufferUsageHint.DynamicCopy);
            _age[1] = new ArrayBuffer(BufferUsageHint.DynamicCopy);

            // Allocate space for all _buffers
            int size = _nParticles * 3 * sizeof(float);
            _posBuf[0].Allocate(size);
            _posBuf[1].Allocate(size);
            _velBuf[0].Allocate(size);
            _velBuf[1].Allocate(size);
            _age[0].Allocate(size / 3);
            _age[1].Allocate(size / 3);

            var tempData = new float[_nParticles];
            float rate = particleLifetime / _nParticles;

            for (int i = 0; i < _nParticles; i++)
            {
                tempData[i] = rate * (i - _nParticles);

            }

            _age[0].SetData(tempData);

            // Create vertex arrays for each set of _buffers
            _particleVertexArray[0] = new VertexArray();
            _particleVertexArray[1] = new VertexArray();

            // Set up particle array 0

            _particleVertexArray[0].Bind();

            _posBuf[0].SetAttribPointer(0, 3);
            _velBuf[0].SetAttribPointer(1, 3);
            _age[0].SetAttribPointer(2, 1);
            _particleVertexArray[0].Unbind();

            // Set up particle array 1
            _particleVertexArray[1].Bind();

            _posBuf[1].SetAttribPointer(0, 3);
            _velBuf[1].SetAttribPointer(1, 3);
            _age[1].SetAttribPointer(2, 1);
            _particleVertexArray[1].Unbind();

            // Setup the _feedback objects
            _feedback[0] = new TransformFeedback();

            // Transform _feedback 0
            _feedback[0].Bind();
            _posBuf[0].BindBase(BufferRangeTarget.TransformFeedbackBuffer, 0);
            _velBuf[0].BindBase(BufferRangeTarget.TransformFeedbackBuffer, 1);
            _age[0].BindBase(BufferRangeTarget.TransformFeedbackBuffer, 2);
            _feedback[0].UnBind();

            // Transform _feedback 1
            _feedback[1] = new TransformFeedback();
            _feedback[1].Bind();
            _posBuf[1].BindBase(BufferRangeTarget.TransformFeedbackBuffer, 0);
            _velBuf[1].BindBase(BufferRangeTarget.TransformFeedbackBuffer, 1);
            _age[1].BindBase(BufferRangeTarget.TransformFeedbackBuffer, 2);
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

        private void SetMatrices(Shader shader)
        {
            shader.SetMatrix4("model", _model.ConvertToMatrix4());
            shader.SetMatrix4("view", _view.ConvertToMatrix4());
            shader.SetMatrix4("projection", _projection.ConvertToMatrix4());
        }

    }
}
