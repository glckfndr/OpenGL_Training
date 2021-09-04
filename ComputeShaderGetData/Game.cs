using OpenGLHelper;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Diagnostics;

namespace ComputeShaderGetData
{

    internal class Game : GameWindow
    {

        public Game(int width, int height, string title) : base(width, height, GraphicsMode.Default, title)
        {

        }

        private const int arraySize = 2000;
        private float[] A = new float[arraySize]; // Input array A
        private float[] B = new float[arraySize]; // Input array B
        private float[] C = new float[arraySize];// Output array


        protected override void OnLoad(EventArgs e)
        {
            var shader = new Shader("../../shader.comp");
            shader.SetInt("BufferSize", arraySize);


            // fill with some sample values
            for (int i = 0; i < arraySize; i++)
            {
                A[i] = 0.02341f * i;
                B[i] = 0.0001f + 0.011f * i;///arraySize - i - 1.0f;
                C[i] = -10000;
            }
            // Create buffers
            var bufferA = new StorageBuffer(BufferUsageHint.StaticDraw);
            bufferA.SetData(A, 0);

            var bufferB = new StorageBuffer(BufferUsageHint.StaticDraw);
            bufferB.SetData(B, 1);

            var bufferC = new StorageBuffer(BufferUsageHint.DynamicDraw);
            bufferC.SetData(C, 2);

            shader.Compute(64, 1, 1,
                MemoryBarrierFlags.ShaderImageAccessBarrierBit |
                MemoryBarrierFlags.ShaderStorageBarrierBit |
                MemoryBarrierFlags.BufferUpdateBarrierBit);

            for (int i = 0; i < arraySize; ++i)
            {
                C[i] = (float)(Math.Sin(Math.Pow(A[i], 2)) * Math.Pow(B[i], 3) + Math.Sqrt(B[i]));
            }

            Stopwatch stp = new Stopwatch();
            stp.Restart();
            //var arrayFromShader = bufferC.GetFloatData(C.Length);
            var arrayFromShader = bufferC.GetFloatData();
            stp.Stop();
            Console.WriteLine("Get Time (ms): " + stp.ElapsedMilliseconds);
            CompareArray(C, arrayFromShader);
            base.OnLoad(e);
        }

        

        private void CompareArray(float[] a, float[] b)
        {
            for (int i = 0; i < a.Length; i++)
            {
                if (Math.Abs(a[i] - b[i]) > Math.Abs(b[i]) * 0.2e-6)
                {
                    Console.WriteLine("i: " + i + " => " + a[i] + " != " + b[i]);
                }
            }
        }
    }

}

