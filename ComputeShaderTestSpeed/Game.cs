using OpenGLHelper;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ComputeShaderTestSpeed
{

    internal class Game : GameWindow
    {
        public Game(int width, int height, string title) : base(width, height, GraphicsMode.Default, title)
        {

        }

        private const int particleNumber = 4096;
        private Vector4[] A = new Vector4[particleNumber]; // Input array A
        private Vector4[] C = new Vector4[particleNumber];// Output array


        protected override void OnLoad(EventArgs e)
        {
            var shader = new Shader("../../shader.comp");
            shader.SetInt("BufferSize", particleNumber);


            // fill with some sample values
            Random rnd = new Random();
            for (int i = 0; i < particleNumber; i++)
            {
                A[i] = new Vector4((float)rnd.NextDouble(), (float)rnd.NextDouble(), 0.1f, 0.1f);
                C[i] = new Vector4(0);
            }
            
            var a = new float[particleNumber * 4];
            for (int i = 0; i < particleNumber; i++)
            {
                int k = 4 * i;
                a[k] = A[i].X;
                a[k + 1] = A[i].Y;
                a[k + 2] = A[i].Z;
                a[k + 3] = A[i].W;
            }

            var c = new float[particleNumber * 4];
            for (int i = 0; i < particleNumber; i++)
            {
                int k = 4 * i;
                c[k] = C[i].X;
                c[k + 1] = C[i].Y;
                c[k + 2] = C[i].Z;
                c[k + 3] = C[i].W;
            }
            Stopwatch stp = new Stopwatch();
            stp.Restart();
            // Create buffers
            var bufferA = new StorageBuffer(BufferUsageHint.StaticDraw);
            bufferA.SetData(a, 0);
            
            var bufferC = new StorageBuffer(BufferUsageHint.DynamicDraw);
            bufferC.SetData(c, 1);
            stp.Stop();
            Console.WriteLine("Send Data to GPU(ms): " + stp.ElapsedMilliseconds);

            stp.Restart();
            shader.Compute(MemoryBarrierFlags.ShaderStorageBarrierBit,
                particleNumber/32, 1, 1);
            var arrayFromShader = bufferC.GetFloatData();
            stp.Stop();
            Console.WriteLine("Compute Time GPU(ms): " + stp.ElapsedMilliseconds);

            stp.Restart();
            Parallel.For(0, particleNumber, j => {
                    for (int i = 0; i < particleNumber; ++i)
                    {
                        C[j].Xy += Velocity(A[j].Xy, A[i]);
                    }
                }

            );
            //for (int j = 0; j < particleNumber; j++)
            //{
            //    for (int i = 0; i < particleNumber; ++i)
            //    {
            //        C[j].Xy += Velocity(A[j].Xy, A[i]);
            //    }
            //}
            stp.Stop();
            Console.WriteLine("Compute Time CPU(ms): " + stp.ElapsedMilliseconds);

            for (int i = 0; i < particleNumber; i++)
            {
                int k = 4 * i;
                c[k] = C[i].X;
                c[k + 1] = C[i].Y;
                c[k + 2] = C[i].Z;
                c[k + 3] = C[i].W;
            }


            
            //stp.Restart();
            //var arrayFromShader = bufferC.GetFloatData(C.Length);
            //arrayFromShader = bufferC.GetFloatData();
            //stp.Stop();
            //Console.WriteLine("Get Data from GPU Time (ms): " + stp.ElapsedMilliseconds);
            CompareArray(c, arrayFromShader);
            base.OnLoad(e);
        }

        private void CompareArray(Vector4[] a, Vector4[] b)
        {
            for (int i = 0; i < a.Length; i++)
            {
                if ((a[i].Xy - b[i].Xy).Length > b[i].Length * 5.0e-5)
                {
                    Console.WriteLine("i: " + i + " => " + a[i].Length + " != " + b[i].Length);
                }
            }
        }

        private void CompareArray(float[] a, float[] b)
        {
            for (int i = 0; i < a.Length; i++)
            {
                if (Math.Abs(a[i] - b[i]) > Math.Abs(b[i]) * 0.5e-6)
                {
                    Console.WriteLine("i: " + i + " => " + a[i] + " != " + b[i]);
                }
            }
        }
        

        private Vector2 Velocity(Vector2 r, Vector4 vortex)
        {
            Vector2 dr = r - vortex.Xy;
            float dist = dr.Length;
            float selector = Step(vortex.Z, dist);
            dist = selector * dist + (1 - selector) * vortex.Z;
            return vortex.W * new Vector2(-vortex.Y, vortex.X) / (dist * dist);

        }

        private float Step(float a, float x)
        {
            return x >= a ? 1 : 0;
        }
    }

}

