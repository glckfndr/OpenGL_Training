using OpenTK.Graphics.OpenGL4;
using System;
using OpenTK;
using Buffer = System.Buffer;

namespace OpenGLHelper
{
    public class StorageBuffer
    {
        private int _id;
        private int _arraySize;
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

        public void Bind()
        {
            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, _id);
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
            GL.BufferData(BufferTarget.ShaderStorageBuffer, Buffer.ByteLength(data), data, _usage);
        }

        public void Allocate<T>(T[] data, int index)
        {
            _arraySize = data.Length;
            Bind(index);
            GL.BufferData(BufferTarget.ShaderStorageBuffer, Buffer.ByteLength(data), IntPtr.Zero, _usage);
        }

        public void Allocate(int sizeInByte, int index)
        {
            Bind(index);
            GL.BufferData(BufferTarget.ShaderStorageBuffer, sizeInByte, IntPtr.Zero, _usage);
        }

        public float[] GetFloatData(int size)
        {
            // Read back the output buffer to check the results
            // Copy the data to our CPU located memory buffer
            //memcpy(&O[0], data, sizeof(float) * arraySize);
            // Release the GPU pointer
            // From here, write results to a file, screen or send them back to
            Bind();
            float[] array = new float[size];

            unsafe
            {
                var data = (float*)GL.MapBuffer(BufferTarget.ShaderStorageBuffer, BufferAccess.ReadOnly);
                fixed (float* pointer = array) // Obtain a pointer to the output buffer data
                {
                    for (int i = 0; i < size; i++)
                    {
                        pointer[i] = data[i];
                    }
                }
                GL.UnmapBuffer(BufferTarget.ShaderStorageBuffer);
            }
            return array;
        }


        public float[] GetFloatData()
        {
            // Read back the output buffer to check the results
            // Copy the data to our CPU located memory buffer
            //memcpy(&O[0], data, sizeof(float) * arraySize);
            // Release the GPU pointer
            // From here, write results to a file, screen or send them back to
            Bind();
            float[] array = new float[_arraySize];

            unsafe
            {
                var data = (float*)GL.MapBuffer(BufferTarget.ShaderStorageBuffer, BufferAccess.ReadOnly);
                fixed (float* pointer = array) // Obtain a pointer to the output buffer data
                {
                    for (int i = 0; i < _arraySize; i++)
                    {
                        pointer[i] = data[i];
                    }
                }
                GL.UnmapBuffer(BufferTarget.ShaderStorageBuffer);
            }
            return array;
        }

        //public Vector4[] GetVectorData()
        //{
        //    // Read back the output buffer to check the results
        //    // Copy the data to our CPU located memory buffer
        //    //memcpy(&O[0], data, sizeof(float) * arraySize);
        //    // Release the GPU pointer
        //    // From here, write results to a file, screen or send them back to
        //    Bind();
        //    Vector4[] array = new Vector4[_arraySize];

        //    unsafe
        //    {
        //        var data = (Vector4*)GL.MapBuffer(BufferTarget.ShaderStorageBuffer, BufferAccess.ReadOnly);
        //        fixed (Vector4* pointer = array) // Obtain a pointer to the output buffer data
        //        {
        //            for (int i = 0; i < _arraySize; i++)
        //            {
        //                pointer[i] = data[i];
        //            }
        //        }
        //        GL.UnmapBuffer(BufferTarget.ShaderStorageBuffer);
        //    }
        //    return array;
        //}

    }
}
