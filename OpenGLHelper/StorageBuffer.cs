using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace OpenGLHelper
{
    public class StorageBuffer
    {
        private int _id;
        private int _arraySize;
        private BufferUsageHint _usage;
        private int _layoutShaderIndex;
        public int LayoutShaderIndex => _layoutShaderIndex;

        public StorageBuffer(BufferUsageHint usage)
        {
            _id = GL.GenBuffer();
            _usage = usage;
        }

        public void BindLayout(int layoutShaderIndex)
        {
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, layoutShaderIndex, _id);
        }

        public void Bind()
        {
            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, _id);
        }

        public void SetAttribPointer(int layoutShaderIndex, int numberOfComponent, int stride = 0,
                                     int offset = 0, bool normalized = false,
                                     VertexAttribPointerType type = VertexAttribPointerType.Float)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, _id);
            GL.VertexAttribPointer(layoutShaderIndex, numberOfComponent, type, normalized, stride, offset);
            GL.EnableVertexAttribArray(layoutShaderIndex);
        }

        public void SetData<T>(T[] data, int layoutShaderIndex) where T : struct
        {
            _arraySize = data.Length;
            _layoutShaderIndex = layoutShaderIndex;
            BindLayout(_layoutShaderIndex);
            //GL.BufferData(BufferTarget.ShaderStorageBuffer, Buffer.ByteLength(data), data, _usage);
            GL.BufferData(BufferTarget.ShaderStorageBuffer, _arraySize * Marshal.SizeOf(data[0]), data, _usage);
        }

        public void SubData<T>(T[] data, int layoutShaderIndex) where T : struct
        {
            _arraySize = data.Length;
            BindLayout(layoutShaderIndex);
            //GL.BufferData(BufferTarget.ShaderStorageBuffer, Buffer.ByteLength(data), data, _usage);
            GL.BufferSubData(BufferTarget.ShaderStorageBuffer, IntPtr.Zero, _arraySize * Marshal.SizeOf(data[0]), data);
        }

        public void SubData<T>(T[] data) where T : struct
        {
            _arraySize = data.Length;
            BindLayout(_layoutShaderIndex);
            //GL.BufferData(BufferTarget.ShaderStorageBuffer, Buffer.ByteLength(data), data, _usage);
            GL.BufferSubData(BufferTarget.ShaderStorageBuffer, IntPtr.Zero, _arraySize * Marshal.SizeOf(data[0]), data);
        }

        public void Allocate<T>(T[] data, int layoutShaderIndex)
        {
            _arraySize = data.Length;
            _layoutShaderIndex = layoutShaderIndex;
            BindLayout(_layoutShaderIndex);
            //GL.BufferData(BufferTarget.ShaderStorageBuffer, Buffer.ByteLength(data), IntPtr.Zero, _usage);
            var sizeOfElement = Marshal.SizeOf(data[0]);
            GL.BufferData(BufferTarget.ShaderStorageBuffer, _arraySize * sizeOfElement, IntPtr.Zero, _usage);
        }

        public void Allocate(int sizeInByte, int layoutShaderIndex)
        {
            BindLayout(layoutShaderIndex);
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

        public Vector4[] GetVector4Data()
        {
            // Read back the output buffer to check the results
            // Copy the data to our CPU located memory buffer
            //memcpy(&O[0], data, sizeof(float) * arraySize);
            // Release the GPU pointer
            // From here, write results to a file, screen or send them back to
            Bind();
            Vector4[] array = new Vector4[_arraySize];

            unsafe
            {
                var data = (Vector4*)GL.MapBuffer(BufferTarget.ShaderStorageBuffer, BufferAccess.ReadOnly);
                fixed (Vector4* pointer = array) // Obtain a pointer to the output buffer data
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

        public Vector2[] GetVector2Data()
        {
            Bind();
            Vector2[] array = new Vector2[_arraySize];

            unsafe
            {
                var data = (Vector2*)GL.MapBuffer(BufferTarget.ShaderStorageBuffer, BufferAccess.ReadOnly);
                fixed (Vector2* pointer = array) // Obtain a pointer to the output buffer data
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

        public T[] GetData<T>() where T : unmanaged
        {
            Bind();
            T[] array = new T[_arraySize];

            unsafe
            {
                var data = (T*)GL.MapBuffer(BufferTarget.ShaderStorageBuffer, BufferAccess.ReadOnly);
                fixed (T* pointer = array) // Obtain a pointer to the output buffer data
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


        public Vector[] GetVectorData()
        {
            Bind();
            Vector[] array = new Vector[_arraySize];

            unsafe
            {
                var data = (Vector*)GL.MapBuffer(BufferTarget.ShaderStorageBuffer, BufferAccess.ReadOnly);
                fixed (Vector* pointer = array) // Obtain a pointer to the output buffer data
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

        public static StorageBuffer SetBufferData<T>(T[] data, int layoutShaderIndex) where T : struct
        {
            var buffer = new StorageBuffer(BufferUsageHint.DynamicDraw);
            buffer.SetData(data, layoutShaderIndex); // copy data on GPU
            return buffer;
        }

        public static StorageBuffer AllocateBufferData<T>(T[] data, int layoutShaderIndex) where T : struct
        {
            var buffer = new StorageBuffer(BufferUsageHint.DynamicDraw);
            buffer.Allocate(data, layoutShaderIndex); // allocate  data  on GPU
            return buffer;
        }

    }
}
