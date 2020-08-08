using GlmNet;
using OpenGLHelper;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using System;

namespace ComputeShaderFindEdge
{
    public class Game : GameWindow
    {
        private string _type = "c";
        private Shader _shader;
        private Shader _computeShader;
        
        private mat4 _view;
        private mat4 _model;
        private mat4 _projection;
        private float _time;
        private float _angle;
        private Plane _plane;
        
        private VertexArray _fsQuadVAO;
        private ShaderSubroutine _passSub1;
        private ShaderSubroutine _passSub2;
        private FrameBuffer _fbo;
        private Torus _torus;
        private TeaPot _teapot;
        private Sphere _sphere;
        Texture _renderTexture;
        Texture _edgeTexture;
        RenderBuffer _depthBuffer;
        private float _tPrev;
        private float _rotSpeed = ((float)Math.PI / 4.0f);


        public Game(int width, int height, string title) :
            base(width, height, GraphicsMode.Default, title)
        {
        }

        protected override void OnLoad(EventArgs e)
        {
            _shader = new Shader("../../Shaders/edge.vert", "../../Shaders/edge.frag");
            // Set up the subroutine indexes
            _passSub1 = new ShaderSubroutine(_shader, ShaderType.FragmentShader, "pass1");
            _passSub2 = new ShaderSubroutine(_shader, ShaderType.FragmentShader, "pass2");
            _shader.SetVector3("Light.Intensity", new Vector3(1.0f, 1.0f, 1.0f));

            _computeShader = new Shader("../../Shaders/edge.comp");

            GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            GL.Enable(EnableCap.DepthTest);

            //  plain = Mesh.PlainXY(new Vector3(0, 0, 0), 12.0f, 0.1f, 200, 200);
            _plane = new Plane(50.0f, 50.0f, 1, 1);
            _teapot = new TeaPot(28, new mat4(1));
            _torus = new Torus(0.7f * 1.5f, 0.3f * 1.5f, 50, 50);
            _sphere = new Sphere(0.75f, 32, 32);

            _projection = new mat4(1.0f);
            _angle = (float)Math.PI / 2;
            // Create the texture object
            _edgeTexture = new Texture(Width, Height, TextureWrapMode.ClampToBorder, 1);
            _renderTexture = new Texture(Width, Height, TextureWrapMode.ClampToBorder, 0, TextureUnit.Texture1);
            // Create the depth buffer
            _depthBuffer = new RenderBuffer(Width, Height);
            _fbo = CreateFBO();

            // Array for full-screen quad
            float[] vertices =
            {
                -1.0f, -1.0f, 0.0f, 1.0f, -1.0f, 0.0f, 1.0f, 1.0f, 0.0f,
                -1.0f, -1.0f, 0.0f, 1.0f, 1.0f, 0.0f, -1.0f, 1.0f, 0.0f
            };

            float[] textureCoordinates =
            {
                0.0f, 0.0f, 1.0f, 0.0f, 1.0f, 1.0f,
                0.0f, 0.0f, 1.0f, 1.0f, 0.0f, 1.0f
            };
            
            // Set up the vertex array object
            _fsQuadVAO = new VertexArray();//GL.GenVertexArray();

            // Set up the _buffers
            ArrayBuffer[] handle = new ArrayBuffer[2];
            handle[0] = new ArrayBuffer(BufferUsageHint.StaticDraw);
            handle[0].SetData(vertices);
            handle[0].SetAttribPointer(0, 3);
            
            handle[1] = new ArrayBuffer(BufferUsageHint.StaticDraw);
            handle[1].SetData(textureCoordinates);
            handle[1].SetAttribPointer(2, 2);
            
            _fsQuadVAO.Unbind();
            base.OnLoad(e);
        }

        private FrameBuffer CreateFBO()
        {
            // Generate and bind the framebuffer
            var fbo = new FrameBuffer();
            // Bind the texture to the FBO
            _renderTexture.BindToFrameBuffer();
            // Bind the depth buffer to the FBO
            _depthBuffer.BindToFrameBuffer();
            fbo.CheckStatus();
            
            // Set the targets for the fragment output variables
            DrawBuffersEnum[] drawBuffers = new[] { DrawBuffersEnum.ColorAttachment0 };
            GL.DrawBuffers(1, drawBuffers);

            // Unbind the framebuffer, and revert to default framebuffer
            fbo.UnBind();
            return fbo;
        }


        protected override void OnRenderFrame(FrameEventArgs e)
        {
            // GL.Clear(ClearBufferMask.ColorBufferBit);
            _time += 0.01f;
            float deltaT = _time - _tPrev;
            if (_tPrev == 0.0f)
                deltaT = 0.0f;
            _tPrev = _time;

            _angle += _rotSpeed * deltaT;
            if (_angle > (float)(Math.PI * 2))
                _angle -= (float)(Math.PI * 2);
            switch (_type)
            {
                case "c":
                    _shader.Use();
                    Pass1();
                    break;
                case "f":
                    _shader.Use();
                    _fbo.Bind();
                    Pass1();
                    _computeShader.Compute(MemoryBarrierFlags.ShaderImageAccessBarrierBit, Width / 25, Height / 25, 1);
                    _shader.Use();
                    _fbo.UnBind();
                    Pass2();

                    break;

            }

            Context.SwapBuffers();
            base.OnRenderFrame(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            KeyboardState input = Keyboard.GetState();
            if (input.IsKeyDown(Key.Escape))
            {
                Exit();
            }
            else if (input.IsKeyDown(Key.C))
            {
                _type = "c";

            }
            else if (input.IsKeyDown(Key.F))
            {
                _type = "f";

            }

            base.OnUpdateFrame(e);
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            _projection = glm.perspective(glm.radians(60.0f),
                                    (float)Width / Height, 0.3f, 100.0f);
            base.OnResize(e);
        }

        private void Pass1()
        {
            //GL.BindFramebuffer(FramebufferTarget.Framebuffer, _fbo);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            //GL.UniformSubroutines(ShaderType.FragmentShader, 1, ref pass1Index);
            _passSub1.Use();

            _view = glm.lookAt(new vec3(7.0f * glm.cos(_angle), 4.0f, 7.0f * glm.sin(_angle)),
                            new vec3(0.0f, 0.0f, 0.0f),
                                new vec3(0.0f, 1.0f, 0.0f));
            _projection = glm.perspective(glm.radians(60.0f), (float)Width / Height, 0.3f, 100.0f);

            _shader.SetVector4("Light.Position", new Vector4(0.0f, 0.0f, 0.0f, 1.0f));
            _shader.SetVector3("Material.Kd", new Vector3(0.9f, 0.9f, 0.9f));
            _shader.SetVector3("Material.Ks", new Vector3(0.95f, 0.95f, 0.95f));
            _shader.SetVector3("Material.Ka", new Vector3(0.1f, 0.1f, 0.1f));
            _shader.SetFloat("Material.Shininess", 100.0f);

            _model = new mat4(1.0f);
            _model = glm.translate(_model, new vec3(0.0f, 0.0f, 0.0f));
            _model = glm.rotate(_model, glm.radians(-90.0f), new vec3(1.0f, 0.0f, 0.0f));
            SetMatrices();
            _teapot.Render();

            _shader.SetVector3("Material.Kd", new Vector3(0.4f, 0.4f, 0.4f));
            _shader.SetVector3("Material.Ks", new Vector3(0.0f, 0.0f, 0.0f));
            _shader.SetVector3("Material.Ka", new Vector3(0.1f, 0.1f, 0.1f));
            _shader.SetFloat("Material.Shininess", 1.0f);
            _model = new mat4(1.0f);
            _model = glm.translate(_model, new vec3(0.0f, -0.75f, 0.0f));
            SetMatrices();
            _plane.Render();

            _shader.SetVector4("Light.Position", new Vector4(0.0f, 0.0f, 0.0f, 1.0f));
            _shader.SetVector3("Material.Kd", new Vector3(0.9f, 0.5f, 0.2f));
            _shader.SetVector3("Material.Ks", new Vector3(0.95f, 0.95f, 0.95f));
            _shader.SetVector3("Material.Ka", new Vector3(0.1f, 0.1f, 0.1f));
            _shader.SetFloat("Material.Shininess", 100.0f);
            _model = new mat4(1.0f);
            _model = glm.translate(_model, new vec3(1.0f, 1.0f, 3.0f));
            _model = glm.rotate(_model, glm.radians(90.0f), new vec3(1.0f, 0.0f, 0.0f));
            SetMatrices();
            _torus.Render();


            _shader.SetVector4("Light.Position", new Vector4(3.0f, 3.0f, 3.0f, 1.0f));
            _shader.SetVector3("Material.Kd", new Vector3(0.5f, 0.5f, 0.2f));
            _shader.SetVector3("Material.Ks", new Vector3(0.95f, 0.0f, 0.95f));
            _shader.SetVector3("Material.Ka", new Vector3(0.1f, 0.2f, 0.1f));
            _shader.SetFloat("Material.Shininess", 10.0f);
            _model = new mat4(1.0f);
            _model = glm.translate(_model, new vec3(-2.0f, 1.0f, -2.0f));
            _model = glm.rotate(_model, glm.radians(90.0f), new vec3(1.0f, 0.0f, 0.0f));
            SetMatrices();
            _sphere.Render();



        }

        private void Pass2()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            _passSub2.Use();
            _model = new mat4(1.0f);
            _view = new mat4(1.0f);
            _projection = new mat4(1.0f);
            SetMatrices();

            // Render the full-screen quad
            //_fsQuadVAO.Bind();
            //GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
            _fsQuadVAO.Draw(PrimitiveType.Triangles, 0, 6);
        }

        private void SetMatrices()
        {
            mat4 mv = _view * _model;
            _shader.SetMatrix4("ModelViewMatrix", mv.ConvertToMatrix4());
            _shader.SetMatrix3("NormalMatrix", (new mat3(new vec3(mv[0]), new vec3(mv[1]), new vec3(mv[2]))).ConvertToMatrix3());
            _shader.SetMatrix4("MVP", (_projection * mv).ConvertToMatrix4());
        }


        protected override void OnUnload(EventArgs e)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);
            _plane.DeleteBuffers();
            _shader.Handle.Delete();
            base.OnUnload(e);
        }
    }
}
