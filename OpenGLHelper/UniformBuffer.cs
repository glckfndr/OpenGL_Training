using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using Buffer = System.Buffer;

namespace OpenGLHelper
{
    public class UniformBuffer
    {
        private int _id;
        private int _arraySize;
        private BufferUsageHint _usage;

        public UniformBuffer(BufferUsageHint usage)
        {
            _id = GL.GenBuffer();
            _usage = usage;
        }

        public void Bind(int index)
        {
            GL.BindBufferBase(BufferRangeTarget.UniformBuffer, index, _id);

        }

        public void Bind()
        {
            GL.BindBuffer(BufferTarget.UniformBuffer, _id);
        }


        public void SetAttribPointer(int index, int numberOfComponent, int stride = 0, int offset = 0, bool normalized = false, VertexAttribPointerType type = VertexAttribPointerType.Float)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, _id);
            GL.VertexAttribPointer(index, numberOfComponent, type, normalized, stride, offset);
            GL.EnableVertexAttribArray(index);
        }

        public void SetData<T>(T[] data, int index) where T : struct
        {
            _arraySize = data.Length;
            Bind(index);
            GL.BufferData(BufferTarget.UniformBuffer, _arraySize * Marshal.SizeOf(data[0]), data, _usage);
            //GL.BufferData(BufferTarget.UniformBuffer, Buffer.ByteLength(data), data, _usage);
        }
    }
}
