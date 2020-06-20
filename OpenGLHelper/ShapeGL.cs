using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform.Windows;

namespace OpenGLHelper
{
    public class ShapeGL
    {

        private int _elementBufferObject;
        private readonly int _vertexArrayObject;
        private int _colorBufferObject;
        private int _vertexBufferObject;
        private Shader _shader;
        private readonly (float[] vertices, uint[] indices) _shape;
        private Texture _texture;
        

        public ShapeGL(Shader shader, (float[] vertices, uint[] indices) shape, float[] colorData)
        {
            _shape = shape;
            _shader = shader;
            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);
            _vertexBufferObject = CopyData.ToArrayBuffer(_shape.vertices, _shader, "VertexPosition", 3);
            _colorBufferObject = CopyData.ToArrayBuffer(colorData, _shader, "VertexColor", 3);
            _elementBufferObject = CopyData.ToElementBuffer(_shape.indices);
            GL.BindVertexArray(0);
        }


        public ShapeGL(Shader shader, (float[] vertices, uint[] indices) shape, Texture texture)
        {
            _texture = texture;
            _shape = shape;
            _shader = shader;
            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);
            _vertexBufferObject = CopyData.ToArrayBufferForTexture(_shape.vertices, _shader, new string[] {"VertexPosition", "TexturePosition" 
        }, new int[]{3,2});

        _elementBufferObject = CopyData.ToElementBuffer(_shape.indices);
        }

        public void Use()
        {
            _texture?.Use();
            _shader.Use();
            GL.BindVertexArray(_vertexArrayObject);
            GL.DrawElements(PrimitiveType.Triangles, _shape.indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
        }

        public void DeleteBuffers()
        {
            GL.DeleteBuffer(_vertexBufferObject);
            GL.DeleteBuffer(_elementBufferObject);
            GL.DeleteVertexArray(_vertexArrayObject);
        }
    }
}
