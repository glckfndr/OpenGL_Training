using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace OpenGLHelper
{
    public class SkyBox
    {
        private int vaoHandle;

        public SkyBox()
        {
            float side = 50.0f;
            float side2 = side / 2.0f;
            float[] v = new float[24 * 3]{
                // Front
                -side2, -side2, side2,
                side2, -side2, side2,
                side2,  side2, side2,
                -side2,  side2, side2,
                // Right
                side2, -side2, side2,
                side2, -side2, -side2,
                side2,  side2, -side2,
                side2,  side2, side2,
                // Back
                -side2, -side2, -side2,
                -side2,  side2, -side2,
                side2,  side2, -side2,
                side2, -side2, -side2,
                // Left
                -side2, -side2, side2,
                -side2,  side2, side2,
                -side2,  side2, -side2,
                -side2, -side2, -side2,
                // Bottom
                -side2, -side2, side2,
                -side2, -side2, -side2,
                side2, -side2, -side2,
                side2, -side2, side2,
                // Top
                -side2,  side2, side2,
                side2,  side2, side2,
                side2,  side2, -side2,
                -side2,  side2, -side2
            };

            int[] el = new []{
                0,2,1,0,3,2,
                4,6,5,4,7,6,
                8,10,9,8,11,10,
                12,14,13,12,15,14,
                16,18,17,16,19,18,
                20,22,21,20,23,22
            };

            int[] handle = new int[2];
            GL.GenBuffers(2, handle);

            GL.BindBuffer(BufferTarget.ArrayBuffer, handle[0]);
            GL.BufferData(BufferTarget.ArrayBuffer, 24 * 3 * sizeof(float), v, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, handle[1]);
            GL.BufferData(BufferTarget.ElementArrayBuffer, 36 * sizeof(int), el, BufferUsageHint.StaticDraw);

            vaoHandle = GL.GenVertexArray();
            GL.BindVertexArray(vaoHandle);

            GL.VertexAttribPointer((int)0, 3, VertexAttribPointerType.Float, false, 0, 0 );
            GL.EnableVertexAttribArray(0);  // Vertex position

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, handle[1]);

            GL.BindVertexArray(0);
        }

        public void Render()
        {
            GL.BindVertexArray(vaoHandle);
            GL.DrawElements(PrimitiveType.Triangles, 36,DrawElementsType.UnsignedInt, 0);
        }

    }
}
