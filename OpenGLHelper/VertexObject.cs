using OpenTK.Graphics.OpenGL;
using BufferTarget = OpenTK.Graphics.OpenGL4.BufferTarget;
using BufferUsageHint = OpenTK.Graphics.OpenGL4.BufferUsageHint;

namespace OpenGLHelper
{
    public class VertexObject
    {
        private readonly ArrayBuffer _elementBufferObject;
        private readonly VertexArray _VAO;
        private readonly ArrayBuffer _colorBufferObject;
        private readonly ArrayBuffer _vertexBufferObject;
        private readonly Shader _shader;
        private readonly (float[] vertices, uint[] indices) _shape;
        private readonly Texture2D _texture;
        private readonly int _numberOfPoints;

        public VertexObject(Shader shader, (float[] vertices, uint[] indices) shape, float[] colorData)
        {
            _shape = shape;
            _shader = shader;
            _VAO = new VertexArray();
            _VAO.Bind();
            _vertexBufferObject = new ArrayBuffer(BufferUsageHint.StaticDraw);
            _vertexBufferObject.SetData(_shape.vertices);
            _vertexBufferObject.SetAttribPointer(_shader, "VertexPosition", 3);

            _colorBufferObject = new ArrayBuffer(BufferUsageHint.StaticDraw);
            _colorBufferObject.SetData(colorData);
            _colorBufferObject.SetAttribPointer(_shader, "VertexColor", 3);

            _elementBufferObject = new ArrayBuffer(BufferUsageHint.StaticDraw, BufferTarget.ElementArrayBuffer);
            _elementBufferObject.SetData(_shape.indices);
            GL.BindVertexArray(0);
        }


        public VertexObject(Shader shader, Texture2D texture, string[] varName, int[] stride, int[] offset, params float[][] data)
        {
            _shader = shader;
            _texture = texture;
            _VAO = new VertexArray();
            _VAO.Bind();
            _numberOfPoints = data[0].Length / stride[0];

            for (uint i = 0; i < data.Length; i++)
            {
                _vertexBufferObject = new ArrayBuffer(BufferUsageHint.StaticDraw);
                _vertexBufferObject.SetData(data[i]);
                _vertexBufferObject.SetAttribPointer(_shader, varName[i], stride[i], offset[i]);

            }

            GL.BindVertexArray(0);
        }


        public VertexObject(Shader shader, (float[] vertices, uint[] indices) shape, Texture2D texture)
        {
            _texture = texture;
            _shape = shape;
            _shader = shader;
            _VAO = new VertexArray();
            _VAO.Bind();

            _vertexBufferObject = new ArrayBuffer(BufferUsageHint.DynamicDraw);
            _vertexBufferObject.SetData(_shape.vertices);
            _vertexBufferObject.SetAttribPointer(_shader, new string[] { "VertexPosition", "TexturePosition" }, new int[] { 3, 2 });

            _elementBufferObject = new ArrayBuffer(BufferUsageHint.StaticDraw, BufferTarget.ElementArrayBuffer);
            _elementBufferObject.SetData(_shape.indices);
            GL.BindVertexArray(0);

        }

        public void Draw(PrimitiveType mode = PrimitiveType.Triangles)
        {
            _texture?.Use();
            _shader.Use();
            _VAO.Bind();

            if (mode == PrimitiveType.Triangles)
            {
                GL.DrawElements(PrimitiveType.Triangles, _shape.indices.Length,
                    DrawElementsType.UnsignedInt, 0);
            }
            else
            {
                GL.DrawArrays(PrimitiveType.Points, 0, _numberOfPoints);
            }

            GL.BindVertexArray(0);
        }

        public void DeleteBuffers()
        {
            _vertexBufferObject.Destroy();
            _elementBufferObject?.Destroy();
            _colorBufferObject?.Destroy();
            _VAO.Destroy();
        }
    }
}
