using System;
using OpenGLHelper;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;

namespace OpenGL_Training
{
    public class Game: GameWindow
    {
        private Matrix4 _rotation;
        Matrix4 trans = Matrix4.CreateTranslation(0.1f, 0.1f, 0.0f);
        private ArrayBuffer _vertexBufferObject; 
        private ArrayBuffer _colorBufferObject;
       // private int[] _vertexArrayObject = new int[2];
       private VertexArray _vertexArrayObject;
        private ArrayBuffer _elementBufferObject;
        private Shader _shader;
     //   private int _location;
        private string _type = "r";

        float[] verticesDataTriangle =
        {
            -0.5f, -0.5f, 0.0f, //Bottom-left vertex
            0.5f, -0.5f, 0.0f, //Bottom-right vertex
            0.0f,  0.5f, 0.0f  //Top vertex
        };

        private readonly float[] verticesData =
        {
            0.5f,  0.5f, 0.0f, // top right
            0.5f, -0.5f, 0.0f, // bottom right
            -0.5f, -0.5f, 0.0f, // bottom left
            -0.5f,  0.5f, 0.0f, // top left
        };

        float[] colorData = 
        {
            1.0f, 0.0f, 0.0f,
            0.0f, 1.0f, 0.0f,
            0.0f, 1.0f, 1.0f,
            0.0f, 0.0f, 1.0f
        };

        private readonly uint[] _indices =
        {
            // Note that indices start at 0!
            0, 1, 3, // The first triangle will be the bottom-right half of the triangle
            1, 2, 3  // Then the second will be the top-right half of the triangle
        };

        private float _angle;

        public Game(int width, int height, string title) : base(width, height, GraphicsMode.Default, title)
        {

        }

        protected override void OnLoad(EventArgs e)
        {
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            _shader = new Shader("../../Shaders/shader.vert", "../../Shaders/shader.frag");
            _shader.Use();
            _shader.SetVector4("blob.InnerColor", new Vector4(1.0f, 0.0f, 0.0f, 1));
            _shader.SetVector4("blob.OuterColor", new Vector4(0.0f, 0.0f, 0.0f, 1));
            _shader.SetFloat("blob.RadiusInner", 0.05f);
            _shader.SetFloat("blob.RadiusOuter", 0.15f);

            //_vertexArrayObject[0] = GL.GenVertexArray();
            _vertexArrayObject = new VertexArray();
            //GL.BindVertexArray(_vertexArrayObject[0]);

            ///GL.EnableVertexAttribArray(0);
            //GL.EnableVertexAttribArray(1);
            _vertexBufferObject = new ArrayBuffer(BufferUsageHint.StaticDraw);
            _vertexBufferObject.SetData(verticesData);
            _vertexBufferObject.SetAttribPointer(_shader, "VertexPosition", 3);
            //  _vertexBufferObject = CopyData.ToArrayBuffer(verticesData, _shader, "VertexPosition",3,0);

            _colorBufferObject = new ArrayBuffer(BufferUsageHint.StaticDraw);
            _colorBufferObject.SetData(colorData);
            _colorBufferObject.SetAttribPointer(_shader, "VertexColor", 3);

            //_colorBufferObject = CopyData.ToArrayBuffer(colorData, _shader, "VertexColor", 3,0);
            _elementBufferObject = new ArrayBuffer(BufferUsageHint.StaticDraw, BufferTarget.ElementArrayBuffer);
            _elementBufferObject.SetData(_indices);

            //_elementBufferObject = CopyData.ToElementBuffer(_indices);
            
            
            
            base.OnLoad(e);
        }
        
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            KeyboardState input = Keyboard.GetState();

            if (input.IsKeyDown(Key.Escape))
            {
                Exit();
            }
            else if (input.IsKeyDown(Key.T))
            {
                _type = "t";

            }
            else if (input.IsKeyDown(Key.R))
            {
                _type = "r";

            }

            base.OnUpdateFrame(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            _shader.Use();
            _angle += 0.1f;

            _rotation = Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(_angle));
            _shader.SetMatrix4("RotationMatrix", _rotation);
            
            //GL.BindVertexArray(_vertexArrayObject[0]);
            _vertexArrayObject.Bind();
            switch (_type)
            {
                case "t":
                    GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
                    
                    break;
                case "r":
                    GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
                    break;
            }
            
            

            Context.SwapBuffers();
            base.OnRenderFrame(e);
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            base.OnResize(e);
        }

        protected override void OnUnload(EventArgs e)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            // Delete all the resources.
            //GL.DeleteBuffer(_vertexBufferObject);
            _vertexBufferObject.Destroy();
            //GL.DeleteBuffer(_colorBufferObject);
            _colorBufferObject.Destroy();
            //GL.DeleteBuffer(_elementBufferObject);
            _elementBufferObject.Destroy();
            
            //GL.DeleteVertexArray(_vertexArrayObject[0]);
            _vertexArrayObject.Destroy();

            _shader.Handle.Delete();
            base.OnUnload(e);
        }
    }
}
