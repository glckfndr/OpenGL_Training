using GlmNet;
using OpenGLHelper;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using TextureWrapMode = OpenTK.Graphics.OpenGL4.TextureWrapMode;



namespace ComputeShader1
{

    public class Game : GameWindow
    {
        private Shader _renderShader;
        private Shader _computeShader;
        private Shader _computeNormalShader;
        private VertexArray _clothVao;
        private int _numberOfElements = 0;
        private vec2 _numberOfParticles = new vec2(50, 50);  // Number of particles in each dimension
        private vec2 _clothSize = new vec2(4, 3);    // Size of cloth in x and y

        private float _time = 0;
        private float _deltaT = 0;
        private float t = 0.0f;
        private float _speed = 200;
        private int _readBuffer = 0;
        private readonly StorageBuffer[] positionBuffers = new StorageBuffer[2];// = new int[2];
        private readonly StorageBuffer[] _velocityBuffers = new StorageBuffer[2];  //new int[2];
        private StorageBuffer _normalBuffer;
        private ArrayBuffer _elementBuffer;
        private ArrayBuffer _textureBuffer;
        private float _angle = 0;
        private mat4 _projection;
        private mat4 _view;
        private mat4 _model;
        private Texture _texture;
        private const int PRIM_RESTART = 0xffffff;

        public Game(int width, int height, string title) : base(width, height, GraphicsMode.Default, title)
        {

        }

        protected override void OnLoad(EventArgs e)
        {
            _model = new mat4(1.0f);
            GL.Enable(EnableCap.PrimitiveRestart);
            GL.PrimitiveRestartIndex(PRIM_RESTART);
            _renderShader = new Shader("../../Shaders/ads.vert", "../../Shaders/ads.frag");

            _computeShader = new Shader("../../Shaders/cloth.comp");
            _computeNormalShader = new Shader("../../Shaders/cloth_normal.comp");

            InitBuffers();
            _projection = glm.perspective(glm.radians(60.0f),
                (float)Width / (float)Height, 0.1f, 100.0f);

            _renderShader.Use();
            _renderShader.SetVector4("LightPosition", new Vector4(0.0f, 0.0f, 0.0f, 1.0f));
            _renderShader.SetVector3("LightIntensity", new Vector3(1.0f));
            _renderShader.SetVector3("Kd", new Vector3(0.8f));
            _renderShader.SetVector3("Ka", new Vector3(0.2f));
            _renderShader.SetVector3("Ks", new Vector3(0.2f));
            _renderShader.SetFloat("Shininess", 80.0f);

            _computeShader.Use();
            float dx = _clothSize.x / (_numberOfParticles.x - 1);
            float dy = _clothSize.y / (_numberOfParticles.y - 1);
            _computeShader.SetFloat("RestLengthHoriz", dx);
            _computeShader.SetFloat("RestLengthVert", dy);
            _computeShader.SetFloat("RestLengthDiag", (float)Math.Sqrt(dx * dx + dy * dy));

            //glActiveTexture(GL_TEXTURE0);
            _texture = new Texture("../../Textures/me_textile.png", TextureWrapMode.Repeat);
            _texture.Use();
            base.OnLoad(e);
        }

        private void InitBuffers()
        {
            // Initial transform
            mat4 transf = glm.translate(new mat4(1.0f), new vec3(0, _clothSize.y, 0));
            transf = glm.rotate(transf, glm.radians(85.0f), new vec3(1, 0, 0));
            transf = glm.translate(transf, new vec3(0, -_clothSize.y, 0));

            // Initial positions of the particles
            float[] initialPosition = GetInitialPosition(transf);
            
            float[] initTextureCoords = GetTextureCoords();

            // Every row is one triangle strip
            var elements = GetElements();
            

            int totalNumber = (int)(_numberOfParticles.x * _numberOfParticles.y);
            int sizeInByte = totalNumber * 4 * sizeof(float);

            // The _buffers for positions

            positionBuffers[1] = new StorageBuffer(BufferUsageHint.DynamicCopy);
            positionBuffers[1].Allocate(initialPosition, 1);

            // Velocities
            float[] initialVelocity = Enumerable.Repeat(0.0f, (int)(_numberOfParticles.x * _numberOfParticles.y * 4) * 4).ToArray();
            _velocityBuffers[0] = new StorageBuffer(BufferUsageHint.DynamicDraw);
            _velocityBuffers[0].SetData(initialVelocity, 2);

            _velocityBuffers[1] = new StorageBuffer(BufferUsageHint.DynamicCopy);
            _velocityBuffers[1].Allocate(initialVelocity, 3);

            // Set up the VAO
            _clothVao = new VertexArray();
            _clothVao.Bind();
            positionBuffers[0] = new StorageBuffer(BufferUsageHint.DynamicDraw);
            positionBuffers[0].SetData(initialPosition, 0);
            positionBuffers[0].SetAttribPointer(0, 4);

            // Normal buffer
            _normalBuffer = new StorageBuffer(BufferUsageHint.DynamicCopy);
            _normalBuffer.Allocate(sizeInByte, 4);
            _normalBuffer.SetAttribPointer(1, 4);

            // Texture coordinates
            _textureBuffer = new ArrayBuffer(BufferUsageHint.StaticDraw);
            _textureBuffer.SetData(initTextureCoords);
            _textureBuffer.SetAttribPointer(2, 2);

            // Element indicies
            _elementBuffer = new ArrayBuffer(BufferUsageHint.DynamicCopy, BufferTarget.ElementArrayBuffer);
            _numberOfElements = elements.Length;
            _elementBuffer.SetData(elements);
            

            _elementBuffer.Bind();

            _clothVao.Unbind();
        }

        private int[] GetElements()
        {
            List<int> elementList = new List<int>();
            for (int row = 0; row < _numberOfParticles.y - 1; row++)
            {
                for (int col = 0; col < _numberOfParticles.x; col++)
                {
                    elementList.Add((int) ((row + 1) * _numberOfParticles.x + (col)));
                    elementList.Add((int) ((row) * _numberOfParticles.x + (col)));
                }

                elementList.Add(PRIM_RESTART);
            }

            return elementList.ToArray();
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

            if (_time == 0.0f)
            {
                _deltaT = 0.0f;
            }
            else
            {
                _deltaT = t - _time;
            }
            _time = t;
            t += 0.01f;
            
            for (int i = 0; i < 1000; i++)
            {
                _computeShader.Compute(MemoryBarrierFlags.ShaderStorageBarrierBit, (int)(_numberOfParticles.x / 10), (int)(_numberOfParticles.y / 10), 1);
                // Swap _buffers
                _readBuffer = 1 - _readBuffer;
                positionBuffers[_readBuffer].Bind(0);
                positionBuffers[1 - _readBuffer].Bind(1);
                _velocityBuffers[_readBuffer].Bind(2);
                _velocityBuffers[1 - _readBuffer].Bind(3);
            }

            // Compute the normals
            _computeNormalShader.Compute(MemoryBarrierFlags.ShaderStorageBarrierBit, (int)(_numberOfParticles.x / 10), (int)(_numberOfParticles.y / 10), 1);

            // Now draw the scene
            //_renderShader.Use();
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            //_angle += 0.005f;
            //_view = glm.lookAt(new vec3(7*glm.cos(_angle), 3, 7*glm.sin(_angle)), 
            //                new vec3(2, 1.5f, 0), 
            //                    new vec3(0, 1, 0));

            _view = glm.lookAt(new vec3(7, 3, 2),
                                    new vec3(2, 1.5f, 0),
                                        new vec3(0, 1, 0));

            SetMatrices();

            // Draw the cloth
            _renderShader.Use();
            _clothVao.Bind();
            GL.DrawElements(PrimitiveType.TriangleStrip, _numberOfElements, DrawElementsType.UnsignedInt, 0);
            _clothVao.Unbind();

            Context.SwapBuffers();
            base.OnRenderFrame(e);
        }

        private float[] GetInitialPosition(mat4 transf)
        {
            var initialPositionList = new List<float>();
            float dx = _clothSize.x / (_numberOfParticles.x - 1);
            float dy = _clothSize.y / (_numberOfParticles.y - 1);
            vec4 p = new vec4(0.0f, 0.0f, 0.0f, 1.0f);

            for (int i = 0; i < _numberOfParticles.y; i++)
            {
                for (int j = 0; j < _numberOfParticles.x; j++)
                {
                    p.x = dx * j;
                    p.y = dy * i;
                    p.z = 0.0f;
                    p = transf * p;
                    initialPositionList.Add(p.x);
                    initialPositionList.Add(p.y);
                    initialPositionList.Add(p.z);
                    initialPositionList.Add(1.0f);
                }
            }

            return initialPositionList.ToArray();
        }

        private float[] GetTextureCoords()
        {
            var textureCoords = new List<float>();

            float ds = 1.0f / (_numberOfParticles.x - 1);
            float dt = 1.0f / (_numberOfParticles.y - 1);

            for (int i = 0; i < _numberOfParticles.y; i++)
            {
                for (int j = 0; j < _numberOfParticles.x; j++)
                {

                    textureCoords.Add(ds * j);
                    textureCoords.Add(dt * i);
                }
            }

            return textureCoords.ToArray();
        }

        private void SetMatrices()
        {
            _renderShader.Use();
            mat4 mv = _view * _model;
            mat3 norm = new mat3(new vec3(mv[0]), new vec3(mv[1]), new vec3(mv[2]));
            _renderShader.SetMatrix3("NormalMatrix", norm.ConvertToMatrix3());
            _renderShader.SetMatrix4("model", _model.ConvertToMatrix4());
            _renderShader.SetMatrix4("projection", _projection.ConvertToMatrix4());
            _renderShader.SetMatrix4("view", _view.ConvertToMatrix4());
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            _projection = glm.perspective(glm.radians(60.0f),
                (float)Width / (float)Height, 0.1f, 100.0f);
            base.OnResize(e);
        }

        protected override void OnUnload(EventArgs e)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);
            _renderShader.Handle.Delete();
            _computeShader.Handle.Delete();
            _computeNormalShader.Handle.Delete();
            base.OnUnload(e);
        }
    }
}