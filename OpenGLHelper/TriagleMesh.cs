using OpenTK.Graphics.OpenGL4;
using System.Collections.Generic;

namespace OpenGLHelper
{
    public class TriagleMesh
    {
        private int _numberOfVertices;     // Number of vertices
        private int _VAO;        // The Vertex Array Object

        // Vertex _buffers
        protected List<int> _buffers;
        protected void InitializeBuffers(int[] indices,
                                        float[] points,
                                        float[] normals,
                                        float[] texCoords = null,
                                        float[] tangents = null)
        {

            // Must have data for indices, points, and normals
            if (indices == null || points == null || normals == null)
                return;
            _buffers = new List<int>();
            _numberOfVertices = indices.Length;

            int indexBuf = 0, posBuf = 0, normBuf = 0, textureBuf = 0, tangentBuf = 0;
            indexBuf = GL.GenBuffer();
            _buffers.Add(indexBuf);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuf);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(int), indices, BufferUsageHint.StaticDraw);

            posBuf = GL.GenBuffer();
            _buffers.Add(posBuf);
            GL.BindBuffer(BufferTarget.ArrayBuffer, posBuf);
            GL.BufferData(BufferTarget.ArrayBuffer, points.Length * sizeof(float), points, BufferUsageHint.StaticDraw);

            normBuf = GL.GenBuffer();
            _buffers.Add(normBuf);
            GL.BindBuffer(BufferTarget.ArrayBuffer, normBuf);
            GL.BufferData(BufferTarget.ArrayBuffer, normals.Length * sizeof(float), normals, BufferUsageHint.StaticDraw);

            if (texCoords != null)
            {
                textureBuf = GL.GenBuffer();
                _buffers.Add(textureBuf);
                GL.BindBuffer(BufferTarget.ArrayBuffer, textureBuf);
                GL.BufferData(BufferTarget.ArrayBuffer, texCoords.Length * sizeof(float), texCoords, BufferUsageHint.StaticDraw);
            }

            if (tangents != null)
            {
                tangentBuf = GL.GenBuffer();
                _buffers.Add(tangentBuf);
                GL.BindBuffer(BufferTarget.ArrayBuffer, tangentBuf);
                GL.BufferData(BufferTarget.ArrayBuffer, tangents.Length * sizeof(float), tangents, BufferUsageHint.StaticDraw);
            }

            _VAO = GL.GenVertexArray();
            GL.BindVertexArray(_VAO);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuf);

            // Position
            GL.BindBuffer(BufferTarget.ArrayBuffer, posBuf);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(0);  // Vertex position

            // Normal
            GL.BindBuffer(BufferTarget.ArrayBuffer, normBuf);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(1);  // Normal

            // Tex coords
            if (texCoords != null)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, textureBuf);
                GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 0, 0);
                GL.EnableVertexAttribArray(2);  // Tex coord
            }

            if (tangents != null)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, tangentBuf);
                GL.VertexAttribPointer(3, 4, VertexAttribPointerType.Float, false, 0, 0);
                GL.EnableVertexAttribArray(3);  // Tangents
            }

            GL.BindVertexArray(0);
        }

        public void Render()
        {
            if (_VAO == 0) return;

            GL.BindVertexArray(_VAO);
            GL.DrawElements(PrimitiveType.Triangles, _numberOfVertices, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
        }

        public void DeleteBuffers()
        {
            if (_buffers.Count > 0)
            {
                GL.DeleteBuffers(_buffers.Count, _buffers.ToArray());
                _buffers.Clear();
            }

            if (_VAO != 0)
            {
                GL.DeleteVertexArray(_VAO);
                _VAO = 0;
            }
        }
    }
}
