using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;

namespace OpenGLHelper
{
    public class TriangleMesh
    {
        private int _numberOfVertices;     // Number of vertices
        protected VertexArray _VAO;  // The Vertex Array Object
        protected const float _twoPi = (float)(Math.PI * 2.0);
        // Vertex _buffers
        protected List<int> _buffers;
        protected void InitializeBuffers(int[] indices, float[] points, float[] normals,
                                        float[] texCoords = null,
                                        float[] tangents = null)
        {
            // Must have data for indices, points, and normals
            if (indices == null || points == null || normals == null)
                throw new Exception("TriangleMesh must have indices, points, and normals!");
            _buffers = new List<int>();
            _numberOfVertices = indices.Length;

            _VAO = new VertexArray();
            
            var indexBuf = new ArrayBuffer(BufferUsageHint.StaticDraw, BufferTarget.ElementArrayBuffer);
            indexBuf.SetData(indices);
            indexBuf.Bind();

            _buffers.Add(indexBuf.GetId());

            // Position
            var positionBuffer = new ArrayBuffer(BufferUsageHint.StaticDraw);
            positionBuffer.SetData(points);
            positionBuffer.SetAttribPointer(0, 3);

            _buffers.Add(positionBuffer.GetId());

            // Normal
            var normalBuffer = new ArrayBuffer(BufferUsageHint.StaticDraw);
            normalBuffer.SetData(normals);
            normalBuffer.SetAttribPointer(1, 3);

            _buffers.Add(normalBuffer.GetId());
            
            // Tex coords
            if (texCoords != null)
            {
                var textureBuf = new ArrayBuffer(BufferUsageHint.StaticDraw);//GL.GenBuffer();
                textureBuf.SetData(texCoords);
                textureBuf.SetAttribPointer(2, 2);

                _buffers.Add(textureBuf.GetId());
            }

            //ArrayBuffer tangentBuf;
            if (tangents != null)
            {
                var tangentBuf = new ArrayBuffer(BufferUsageHint.StaticDraw);
                tangentBuf.SetData(tangents);
                tangentBuf.SetAttribPointer(3, 4);
                _buffers.Add(tangentBuf.GetId());
            }

            _VAO.Unbind();
        }

        public void Render()
        {
            if (_VAO.GetId() == 0) return;
            _VAO.Bind();
            GL.DrawElements(PrimitiveType.Triangles, _numberOfVertices, DrawElementsType.UnsignedInt, 0);
            _VAO.Unbind();
        }

        public void DeleteBuffers()
        {
            if (_buffers.Count > 0)
            {
                GL.DeleteBuffers(_buffers.Count, _buffers.ToArray());
                _buffers.Clear();
            }

            if (_VAO.GetId() != 0)
            {
                _VAO.Destroy();
            }
        }
    }
}
