using GlmNet;
using OpenGLHelper;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using System;

namespace ComputeShader4
{
    public class Game : GameWindow
    {
        private string _type = "c";
        private Shader _shader;
        private Shader _computeShader;
        private ArrayBuffer _elementBufferObject;
        private int _vertexArrayObject;
        //private int _vertexBufferObject;
        private ArrayBuffer _vertexBufferObject;
        private mat4 _view;
        private mat4 _model;
        private mat4 _projection;
        private (float[] vertices, uint[] indices) plain;
        private float time = 0.0f;
        private float _angle = 0.0f;
        private Plane _plane;
        //private int fsQuad;
        private VertexArray fsQuad;
        private ShaderSubroutine pass1sub, pass2sub;
        private FrameBuffer _fbo;
        private Torus _torus;
        private TeaPot _teapot;
        private Sphere _sphere;
        private float angle = 0;
        private float tPrev = 0;
        private float rotSpeed = ((float)Math.PI / 4.0f);


        public Game(int width, int height, string title) :
            base(width, height, GraphicsMode.Default, title)
        {
        }

        protected override void OnLoad(EventArgs e)
        {
            _shader = new Shader("../../Shaders/edge.vert", "../../Shaders/edge.frag");
            _computeShader = new Shader("../../Shaders/edge.comp");
            GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            GL.Enable(EnableCap.DepthTest);

            //  plain = Mesh.PlainXY(new Vector3(0, 0, 0), 12.0f, 0.1f, 200, 200);
            _plane = new Plane(50.0f, 50.0f, 1, 1);
            _teapot = new TeaPot(28, new mat4(1));
            _torus = new Torus(0.7f * 1.5f, 0.3f * 1.5f, 50, 50);
            _sphere = new Sphere(0.75f,32,32);

            _projection = new mat4(1.0f);
            _angle = (float)Math.PI / 2;
            setupFBO();

            // Array for full-screen quad
            float[] verts =
            {
                -1.0f, -1.0f, 0.0f, 1.0f, -1.0f, 0.0f, 1.0f, 1.0f, 0.0f,
                -1.0f, -1.0f, 0.0f, 1.0f, 1.0f, 0.0f, -1.0f, 1.0f, 0.0f
            };

            float[] tc =
            {
                0.0f, 0.0f, 1.0f, 0.0f, 1.0f, 1.0f,
                0.0f, 0.0f, 1.0f, 1.0f, 0.0f, 1.0f
            };

            // Set up the _buffers
            
            //GL.GenBuffers(2, handle);

            //GL.BindBuffer(BufferTarget.ArrayBuffer, handle[0]);
            //GL.BufferData(BufferTarget.ArrayBuffer, 6 * 3 * sizeof(float), verts, BufferUsageHint.StaticDraw);


            //GL.BindBuffer(BufferTarget.ArrayBuffer, handle[1]);
            //GL.BufferData(BufferTarget.ArrayBuffer, 6 * 2 * sizeof(float), tc, BufferUsageHint.StaticDraw);

            // Set up the vertex array object

            fsQuad = new VertexArray();//GL.GenVertexArray();
            //GL.BindVertexArray(fsQuad);
            fsQuad.Bind();
            ArrayBuffer[] handle = new ArrayBuffer[2];
            handle[0] = new ArrayBuffer(BufferUsageHint.StaticDraw);
            handle[0].SetData(verts);
            handle[0].SetAttribPointer(0, 3);
            //GL.BindBuffer(BufferTarget.ArrayBuffer, handle[0]);
            //GL.VertexAttribPointer(0, 3,VertexAttribPointerType.Float, false, 0, 0);
            //GL.EnableVertexAttribArray(0);  // Vertex position
            handle[1] = new ArrayBuffer(BufferUsageHint.StaticDraw);
            handle[1].SetData(tc);
            handle[1].SetAttribPointer(2, 2);
            //GL.BindBuffer(BufferTarget.ArrayBuffer, handle[1]);
            //GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 0, 0);
            //GL.EnableVertexAttribArray(2);  // Texture coordinates

            //GL.BindVertexArray(0);
            fsQuad.Unbind();

            // Set up the subroutine indexes
            //_shader.Use();
            //int programHandle = _shader.Handle.GetHandle();
            //pass1Index = _shader.SubroutineIndex(ShaderType.FragmentShader, "pass1");
            //pass1Index = GL.GetSubroutineIndex(programHandle,ShaderType.FragmentShader, "pass1");
            pass1sub = new ShaderSubroutine(_shader, ShaderType.FragmentShader, "pass1");
            //pass2Index = GL.GetSubroutineIndex(programHandle,ShaderType.FragmentShader, "pass2");
            //pass2Index = _shader.SubroutineIndex(ShaderType.FragmentShader, "pass2");
            pass2sub = new ShaderSubroutine(_shader, ShaderType.FragmentShader, "pass2");
            _shader.SetVector3("Light.Intensity", new Vector3(1.0f, 1.0f, 1.0f));


            base.OnLoad(e);
        }

        private void setupFBO()
        {
            // Generate and bind the framebuffer
            _fbo = new FrameBuffer();

            // Create the texture object
            var renderTex = new Texture(Width, Height, TextureWrapMode.ClampToBorder, 0, TextureUnit.Texture1);
            var edgeTex = new Texture(Width, Height, TextureWrapMode.ClampToBorder, 1);

            // Bind the texture to the FBO
            renderTex.BindToFrameBuffer();

            // Create the depth buffer
            var depthBuf = new RenderBuffer(Width, Height);
            // Bind the depth buffer to the FBO
            depthBuf.BindToFrameBuffer();

            // Set the targets for the fragment output variables
            DrawBuffersEnum[] drawBuffers = new[] { DrawBuffersEnum.ColorAttachment0 };
            GL.DrawBuffers(1, drawBuffers);

            // Unbind the framebuffer, and revert to default framebuffer
            _fbo.UnBind();
        }


        protected override void OnRenderFrame(FrameEventArgs e)
        {
            // GL.Clear(ClearBufferMask.ColorBufferBit);
            time += 0.01f;
            float deltaT = time - tPrev;
            if (tPrev == 0.0f)
                deltaT = 0.0f;
            tPrev = time;

            angle += rotSpeed * deltaT;
            if (angle > (float)(Math.PI * 2))
                angle -= (float)(Math.PI * 2);
            switch (_type)
            {

                case "c":

                    _shader.Use();
                    pass1();
                    break;

                case "f":
                    _shader.Use();
                    _fbo.Bind();
                    pass1();
                    _computeShader.Compute(MemoryBarrierFlags.ShaderImageAccessBarrierBit, Width / 25, Height / 25, 1);
                    _shader.Use();
                    _fbo.UnBind();
                    pass2();

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

        private void pass1()
        {
            //GL.BindFramebuffer(FramebufferTarget.Framebuffer, _fbo);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            //GL.UniformSubroutines(ShaderType.FragmentShader, 1, ref pass1Index);
            pass1sub.Use();

            _view = glm.lookAt(new vec3(7.0f * glm.cos(angle), 4.0f, 7.0f * glm.sin(angle)),
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

        private void pass2()
        {
            //GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // GL.UniformSubroutines(ShaderType.FragmentShader, 1, ref pass2Index);
            pass2sub.Use();
            _model = new mat4(1.0f);
            _view = new mat4(1.0f);
            _projection = new mat4(1.0f);
            SetMatrices();

            // Render the full-screen quad
            // GL.BindVertexArray(fsQuad);
            fsQuad.Bind();

            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
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
