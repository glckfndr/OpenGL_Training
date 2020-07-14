using OpenTK.Graphics.OpenGL4;
using System;
using Buffer = System.Buffer;

namespace OpenGLHelper
{
    public class StorageBuffer
    {
        private int _id;
        private BufferUsageHint _usage;

        public StorageBuffer(BufferUsageHint usage)
        {
            _id = GL.GenBuffer();
            _usage = usage;
        }

        public void Bind(int index)
        {
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, index, _id);

        }

        public void SetAttribPointer(int index, int numberOfComponent, int stride = 0, int offset = 0, bool normalized = false, VertexAttribPointerType type = VertexAttribPointerType.Float)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, _id);
            GL.VertexAttribPointer(index, numberOfComponent, type, normalized, stride, offset);
            GL.EnableVertexAttribArray(index);
        }


        public void SetData<T>(T[] data, int index) where T : struct
        {
            Bind(index);
            GL.BufferData(BufferTarget.ShaderStorageBuffer, Buffer.ByteLength(data), data, _usage);
        }

        public void Allocate<T>(T[] data, int index)
        {
            Bind(index);
            GL.BufferData(BufferTarget.ShaderStorageBuffer, Buffer.ByteLength(data), IntPtr.Zero, _usage);
        }

        public void Allocate(int sizeInByte, int index)
        {
            Bind(index);
            GL.BufferData(BufferTarget.ShaderStorageBuffer, sizeInByte, IntPtr.Zero, _usage);
        }




    }
}
