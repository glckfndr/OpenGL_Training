using GlmNet;
using OpenGLHelper;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using System;

namespace Particles_Clone
{
    public class Game : GameWindow
    {

        private Shader _shader;
        private ArrayBuffer _initialVelocityBuffer;
        private ArrayBuffer _startTimeBuffer;
        private VertexArray _particleVertexArray;
        
        private int _nParticles = 5000;
        private float _angle;
        private float _time;
        private float _deltaT;

        private Texture _texture;
        private float particleLifetime = 6;

        private float t;
        private float dt = 0.01f;

        private mat4 _projection;
        private mat4 _view;
        private mat4 _model;
        private Torus _torus;

        public Game(int width, int height, string title) : base(width, height, GraphicsMode.Default, title)
        {

        }

        protected override void OnLoad(EventArgs e)
        {
            _torus = new Torus(0.5f * 0.1f, 0.2f * 0.1f, 20, 20);
            _shader = new Shader("../../Shaders/particleinstanced.vert", "../../Shaders/particleinstanced.frag");

            GL.ClearColor(0.5f, 0.5f, 0.5f, 1.0f);
            GL.Enable(EnableCap.DepthTest);
            _angle = (float)(Math.PI / 2);
            InitBuffers();
            _shader.SetVector3("Light.Intensity", new Vector3(1.0f, 1.0f, 1.0f));
            _shader.SetFloat("ParticleLifetime", 3.5f);
            _shader.SetVector3("Gravity", new Vector3(0.0f, -0.2f, 0.0f));

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

            _shader.SetFloat("Time", _time);

            _view = glm.lookAt(new vec3(3.0f * glm.cos(_angle), 1.5f, 3.0f * glm.sin(_angle)),
                            new vec3(0.0f, 1.5f, 0.0f),
                                new vec3(0.0f, 1.0f, 0.0f));

            _shader.SetVector4("Light.Position", new Vector4(0.0f, 0.0f, 0.0f, 1.0f));
            _shader.SetVector3("Material.Kd", new Vector3(0.9f, 0.5f, 0.2f));
            _shader.SetVector3("Material.Ks", new Vector3(0.95f, 0.95f, 0.95f));
            _shader.SetVector3("Material.Ka", new Vector3(0.1f, 0.1f, 0.1f));
            _shader.SetFloat("Material.Shininess", 100.0f);
            _model = new mat4(1.0f);
            SetMatrices(_shader);

            //GL.BindVertexArray(_torus.GetVao());
            _particleVertexArray.Bind();
            GL.DrawElementsInstanced(PrimitiveType.Triangles, 6 * 20 * 20,
                                        DrawElementsType.UnsignedInt, IntPtr.Zero, _nParticles);

            Context.SwapBuffers();
        }

        private void InitBuffers()
        {
            

            // Fill the first velocity buffer with random velocities
            vec3 v = new vec3(0.0f);
            float velocity, theta, phi;
            float[] data = new float[_nParticles * 3];
            Random rnd = new Random();
            for (int i = 0; i < _nParticles; i++)
            {

                theta = Mix(0.0f, Math.PI / 6.0f, rnd.NextDouble());
                phi = Mix(0.0f, 2 * Math.PI, rnd.NextDouble());

                v.x = glm.sin(theta) * glm.cos(phi);
                v.y = glm.cos(theta);
                v.z = glm.sin(theta) * glm.sin(phi);

                velocity = Mix(1.25f, 1.5f, rnd.NextDouble());
                v = glm.normalize(v) * velocity;

                data[3 * i] = v.x;
                data[3 * i + 1] = v.y;
                data[3 * i + 2] = v.z;
            }

            // Allocate space for all _buffers
            int size = _nParticles * 3 * sizeof(float);
            _initialVelocityBuffer = new ArrayBuffer(BufferUsageHint.StaticDraw);
            _initialVelocityBuffer.Allocate(size);
            _initialVelocityBuffer.SetData(data);

            var tempData = new float[_nParticles];
            _time = 0.0f;
            float rate = 0.01f;
            for (int i = 0; i < _nParticles; i++)
            {
                tempData[i] = _time;
                _time += rate;

            }
            _startTimeBuffer = new ArrayBuffer(BufferUsageHint.StaticDraw);
            _startTimeBuffer.Allocate(size / 3);
            _startTimeBuffer.SetData(tempData);
            _time = 0;

            // Create vertex arrays for each set of _buffers
            _particleVertexArray = new VertexArray(_torus.GetVao());
            //GL.BindVertexArray(_torus.GetVao());

            _initialVelocityBuffer.SetAttribPointer(3, 3);
            _startTimeBuffer.SetAttribPointer(4, 1);
            GL.VertexAttribDivisor(3, 1);
            GL.VertexAttribDivisor(4, 1);

            //GL.BindVertexArray(0);
            _particleVertexArray.Unbind();
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
            mat4 mv = _view * _model;
            shader.SetMatrix4("ModelViewMatrix", mv.ConvertToMatrix4());
            shader.SetMatrix3("NormalMatrix",
                            (new mat3(new vec3(mv[0]), new vec3(mv[1]), new vec3(mv[2]))).ConvertToMatrix3());
            shader.SetMatrix4("ProjectionMatrix", _projection.ConvertToMatrix4());
        }
    }
}
