using OpenTK.Graphics.OpenGL4;
using System;
using Buffer = System.Buffer;

namespace OpenGLHelper
{
    public class ArrayBuffer
    {
        private int _id;
        private BufferTarget _target;
        private BufferUsageHint _usage;
        private BufferRangeTarget _rangeTarget;
        private bool _ok;

        public ArrayBuffer(BufferUsageHint usage, BufferTarget target = BufferTarget.ArrayBuffer)
        {
            _usage = usage;
            _target = target;
            _ok = false;

            Create();
            if (!_ok)
            {
                throw new Exception("Cant create ArrayBuffer!");
            }
        }

        //ArrayBuffer:: ~ArrayBuffer()
        //{
        //    destroy();
        //}

        public void Destroy()
        {
            if (_id != 0)
                GL.DeleteBuffer(_id);

            _id = 0;
            _ok = false;
            _target = 0;
        }

        private bool Create()
        {
            _id = GL.GenBuffer();
            Bind();
            return _ok = GL.IsBuffer(_id);
        }

        public void Bind()
        {
            GL.BindBuffer(_target, _id);
            // var res = GL.IsBuffer(_id);
        }

        public void Unbind()
        {
            GL.BindBuffer(_target, 0);
        }

        public void Allocate(int sizeInByte)
        {
            Bind();
            GL.BufferData(_target, sizeInByte, IntPtr.Zero, _usage);
        }

        public void SetData<T>(T[] data) where T : struct
        {
            Bind();
            GL.BufferData(_target, Buffer.ByteLength(data), data, _usage);
        }

        public void SetSubData(IntPtr offs, int size, float[] data)
        {
            Bind();
            GL.BufferSubData(_target, offs, size, data);
        }

        public void GetSubData(IntPtr offs, int size, IntPtr ptr)
        {
            GL.GetBufferSubData(_target, offs, size, ptr);
        }

        public IntPtr Map(BufferAccess access)
        {
            return GL.MapBuffer(_target, access);
        }

        public bool Unmap()
        {
            return GL.UnmapBuffer(_target) == true;
        }

        public void SetAttribPointer(int index, int numberOfComponent, int stride = 0, int offset = 0, bool normalized = false, VertexAttribPointerType type = VertexAttribPointerType.Float)
        {

            Bind();
            GL.VertexAttribPointer(index,                    // index
                                numberOfComponent,          // number of values per vertex
                                    type,                   // type (GL_FLOAT)
                                    normalized,
                                    stride * sizeof(float),               // stride (offset to next vertex data)
                                offset * sizeof(float));

            GL.EnableVertexAttribArray(index);
        }

        public void SetAttribPointer(Shader shader, string attributeName, int numberOfComponent, int stride = 0, int offset = 0,
                                        bool normalized = false, VertexAttribPointerType type = VertexAttribPointerType.Float)
        {
            Bind();
            var dataLocation = shader.GetAttribLocation(attributeName);
            GL.EnableVertexAttribArray(dataLocation);
            GL.VertexAttribPointer(dataLocation,
                                    numberOfComponent,
                                        type,
                                        normalized,
                                    stride * sizeof(float),
                                offset * sizeof(float));
            GL.EnableVertexAttribArray(dataLocation);

        }

        public void SetAttribPointer(Shader shader, string[] attributeName, int[] dataSize)
        {
            Bind();
            int stride = 0;
            foreach (var len in dataSize)
            {
                stride += len;
            }
            int offset = 0;

            for (var i = 0; i < dataSize.Length; i++)
            {
                SetAttribPointer(shader, attributeName[i], dataSize[i], stride, offset);
                offset += dataSize[i];
            }
        }


        //shader, string attributeName, int stride, int offset

        public void BindBase(BufferRangeTarget theTarget, int index)
        {
            _rangeTarget = theTarget;
            GL.BindBufferBase(theTarget, index, _id);
        }

        public void BindRange(BufferRangeTarget theTarget, int index, IntPtr offset, int size)
        {
            _rangeTarget = theTarget;
            GL.BindBufferRange(theTarget, index, _id, offset, size);
        }

        public void Clear()
        {
            GL.BufferData(_target, 0, IntPtr.Zero, 0 /*usage*/ );
        }

        public int GetId()
        {
            return _id;
        }
        //bool IsSupported()
        //{
        //    return  GLEW_ARB_vertex_buffer_object != 0;
        //}
    }
}
