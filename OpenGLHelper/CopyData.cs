using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace OpenGLHelper
{
    public static class CopyData
    {
        public static int ToArrayBuffer(float[] data, Shader shader , string attributeName, int stride)
        {
            int handle = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, handle);
            GL.BufferData(BufferTarget.ArrayBuffer, data.Length * sizeof(float), data, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, handle);
            var dataLocation = shader.GetAttribLocation(attributeName);
            GL.EnableVertexAttribArray(dataLocation);


            GL.VertexAttribPointer(dataLocation, 3, VertexAttribPointerType.Float, false, stride * sizeof(float), 0);
            return handle;
        }

        public static int ToElementBuffer(uint[] data)
        {
            int handle = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, handle);
            GL.BufferData(BufferTarget.ElementArrayBuffer, data.Length * sizeof(uint), data, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, handle);
            return handle;
        }
    }
}
