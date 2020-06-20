using OpenTK.Graphics.OpenGL4;

namespace OpenGLHelper
{
    public static class CopyData
    {
        public static int ToArrayBuffer(float[] data, Shader shader, string attributeName, int stride)
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


        public static int ToArrayBufferForTexture(float[] data, Shader shader, string[] attributeName, int[] dataSize)
        {
            int stride = 0;
            foreach (var len in dataSize)
            {
                stride += len;
            }

            int handle = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, handle);
            GL.BufferData(BufferTarget.ArrayBuffer, data.Length * sizeof(float), data, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, handle);

            //var dataLocation = shader.GetAttribLocation(attributeName[0]);
            //GL.EnableVertexAttribArray(dataLocation);
            //GL.VertexAttribPointer(dataLocation, dataSize[0], VertexAttribPointerType.Float, false, stride * sizeof(float), 0);

            //GL.BindBuffer(BufferTarget.ArrayBuffer, handle);
            //dataLocation = shader.GetAttribLocation(attributeName[1]);
            //GL.EnableVertexAttribArray(dataLocation);
            //GL.VertexAttribPointer(dataLocation, dataSize[1], VertexAttribPointerType.Float, false, stride * sizeof(float), dataSize[0] * sizeof(float));
            int offset = 0;

            for (var i=0; i< dataSize.Length; i++)
            {
                var dataLocation = shader.GetAttribLocation(attributeName[i]);
                GL.EnableVertexAttribArray(dataLocation);
                GL.VertexAttribPointer(dataLocation, dataSize[i], VertexAttribPointerType.Float, false, stride * sizeof(float), offset);
                offset += dataSize[i] * sizeof(float);
            }



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
