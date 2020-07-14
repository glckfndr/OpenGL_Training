using OpenTK.Graphics.OpenGL4;
using System.Collections.Generic;

namespace ParticlesFeedBack
{
    public class Grid
    {
        //private List<int> _buffers;
        private int _VAO;
        private int nVerts;

        public Grid(float size, int nDivisions)
        {
            float size2 = size / 2.0f;
            float divisionSize = size / nDivisions;
            nVerts = 4 * (nDivisions + 1);
            List<float> p = new List<float>();

            float y = 0.0f;
            for (int row = 0; row <= nDivisions; row++)
            {
                float z = (row * divisionSize) - size2;
                p.Add(-size2);
                p.Add(0);
                p.Add(z);
                p.Add(size2);
                p.Add(0);
                p.Add(z);
            }

            for (int col = 0; col <= nDivisions; col++)
            {
                float x = (col * divisionSize) - size2;
                p.Add(x);
                p.Add(0);
                p.Add(-size2);
                p.Add(x);
                p.Add(0);
                p.Add(size2);
            }

            int posBuf = 0;
            posBuf = GL.GenBuffer();
            //_buffers.Add(posBuf);
            GL.BindBuffer(BufferTarget.ArrayBuffer, posBuf);
            GL.BufferData(BufferTarget.ArrayBuffer, p.Count * sizeof(float), p.ToArray(), BufferUsageHint.StaticDraw);

            _VAO = GL.GenVertexArray();
            GL.BindVertexArray(_VAO);
            // Position
                GL.BindBuffer(BufferTarget.ArrayBuffer, posBuf);
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
                GL.EnableVertexAttribArray(0);  // Vertex position
            GL.BindVertexArray(0);
        }

        public void Render()
        {
            GL.BindVertexArray(_VAO);
            GL.DrawArrays(PrimitiveType.Lines, 0, nVerts);
            GL.BindVertexArray(0);
        }

    }
}
