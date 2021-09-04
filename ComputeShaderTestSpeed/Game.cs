using OpenGLHelper;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;

namespace ComputeShaderTestSpeed
{
    // розмір структури має бути кратим 8 байт, інакше неправильне вирівнювання на графічному процесорі

    internal class Game : GameWindow
    {
        public Game(int width, int height, string title) : base(width, height, GraphicsMode.Default, title)
        {

        }

        private const int particleNumber = 16 * 16 * 16 * 4;
        private Vector4[] A = new Vector4[particleNumber]; // Input array A
        private Vortex[] AStruct = new Vortex[particleNumber]; // Input array A
        private VortexDouble[] AStructDouble = new VortexDouble[particleNumber]; // Input array A
        private Vector2[] Cout = new Vector2[particleNumber];// Output array
        private Vector[] CoutDouble = new Vector[particleNumber];// Output array


        protected override void OnLoad(EventArgs e)
        {
            var shaderVec4 = CreateShader("../../shaderVec4.comp");
            var shaderStruct = CreateShader("../../shaderStruct.comp");
            var shaderStructDouble = CreateShader("../../shaderStructDouble.comp");
            var shaderStructChain = CreateShader("../../vortexVelocity2D.comp");

            SetInitialData();

            Stopwatch stp = new Stopwatch();

            stp.Restart();
            ComputeParallel();
            stp.Stop();
            Console.WriteLine($"********** CPU(ms):  {stp.ElapsedMilliseconds}");

            stp.Restart();
            var arrayFromShader = ComputeOnGPU(shaderVec4, A, Cout);
            stp.Stop();
            Console.WriteLine($"********** GPU(ms) and Get Data: {stp.ElapsedMilliseconds}");

            ArrayComparer.Compare(Cout, arrayFromShader);

            //stp.Restart();
            //ComputeSequential();
            //stp.Stop();
            //Console.WriteLine($"********** CPU(ms) Sequential:  {stp.ElapsedMilliseconds}");

            //ArrayComparer.Compare(Cout, arrayFromShader);

            stp.Restart();
            ComputeParallelStruct();
            stp.Stop();
            Console.WriteLine($"********** CPU(ms) Parallel Struct:  {stp.ElapsedMilliseconds}");

            stp.Restart();
            arrayFromShader = ComputeOnGPU(shaderStruct, AStruct, Cout);
            stp.Stop();

            Console.WriteLine($"********** GPU(ms) Struct Time and Get Data: {stp.ElapsedMilliseconds}");
            ArrayComparer.Compare(Cout, arrayFromShader);
            
            //var outVel = ComputeSequentialStructDouble();
            //ArrayComparer.Compare(CoutDouble, outVel);

            stp.Restart();
            //var arrayFromShaderDouble = ComputeOnGPUDouble(shaderStructDouble, AStructDouble, CoutDouble);
            var arrayFromShaderDouble = ComputeOnGPU(shaderStructDouble, AStructDouble, CoutDouble);
            stp.Stop();

            Console.WriteLine($"********** GPU(ms) Double Struct  and Get Data: {stp.ElapsedMilliseconds}");

            stp.Restart();
            ComputeParallelStructDouble();

            stp.Stop();
            Console.WriteLine($"********** CPU(ms) Parallel Double Struct :  {stp.ElapsedMilliseconds}");

            ArrayComparer.Compare(CoutDouble, arrayFromShaderDouble);
            //CompareTest(arrayFromShaderDouble, AStructDouble);

            stp.Restart();
            ComputeParallelStructChain();
            stp.Stop();
            Console.WriteLine($"********** CPU(ms) Chain Parallel Struct:  {stp.ElapsedMilliseconds}");

            stp.Restart();
            arrayFromShader = ComputeOnGPU(shaderStructChain, AStruct, Cout);
            stp.Stop();

            Console.WriteLine($"********** GPU(ms) Chain Struct Time and Get Data: {stp.ElapsedMilliseconds}");
            //ArrayComparer.Compare(Cout, arrayFromShader);

            Console.WriteLine($"********** Calculation is finished ********* ");

            shaderStructDouble.Handle.Delete();
            shaderStruct.Handle.Delete();
            shaderVec4.Handle.Delete();
            shaderStructChain.Handle.Delete();

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, 0);

            base.OnLoad(e);
        }

        private void CompareTest(Vector[] a, VortexDouble[] b, float eps = 1.0e-14f)
        {
            Console.WriteLine($"Relative error eps =  {eps}");
            //var sum = 0.0;
            //for (int i = 0; i < a.Length; i++)
            //{
            //    sum += 1.0/3.0;
            //}

            for (int i = 0; i < a.Length; i++)
            {
                if ((a[i] - b[i].r).Length > a[i].Length * eps)
                {
                    Console.WriteLine("i: " + i + " => " + a[i].Length + " != " + b[i].r.Length);
                }
                //if (Math.Abs((a[i]).Length - sum) > sum * eps)
                //{
                //    Console.WriteLine("i: " + i + " => " + a[i].Length + " != " + sum);
                //}


            }
        }



        private void SetInitialData()
        {
            Console.WriteLine($"****** Number of Vortices: {particleNumber} ******");
            // Set vortex sytem in A and velocity in Cout
            Random rnd = new Random();
            float gamma = 0.0008f;
            for (int i = 0; i < particleNumber; i++)
            {
                var x = rnd.NextDouble();
                var y = rnd.NextDouble();

                A[i] = new Vector4((float)x, (float)y, 0.001f, gamma);
                AStruct[i] = new Vortex(new Vector2((float)x, (float)y), gamma, 0.001f);
                Cout[i] = new Vector2(0);
                AStructDouble[i] = new VortexDouble(new Vector(x, y), gamma, 0.001);
                CoutDouble[i] = new Vector(0, 0);
            }
        }

        private static Shader CreateShader(string path)
        {
            var shader = new Shader(path);
            shader.SetInt("BufferSize", particleNumber);
            return shader;
        }

        private void ComputeSequential()
        {
            for (int j = 0; j < particleNumber; j++)
            {
                Cout[j] = Vector2.Zero;
                for (int i = 0; i < particleNumber; ++i)
                {
                    Cout[j] += Velocity(A[j].Xy, A[i]);
                }
            }
        }

        private void ComputeParallel()
        {
            for (var k = 0; k < 3; k++)
                Parallel.For(0, particleNumber, j =>
            {
                Cout[j] = Vector2.Zero;
                for (int i = 0; i < particleNumber; ++i)
                {
                    Cout[j] += Velocity(A[j].Xy, A[i]);
                }
            });
        }

        private void ComputeSequentialStruct()
        {
            for (int j = 0; j < particleNumber; j++)
            {
                Cout[j] = Vector2.Zero;
                for (int i = 0; i < particleNumber; ++i)
                {
                    Cout[j] += Velocity(AStruct[j].r, AStruct[i]);
                }
            }
        }

        private void ComputeParallelStruct()
        {
            for (var k = 0; k < 3; k++)
                Parallel.For(0, particleNumber, j =>
            {
                Cout[j] = Vector2.Zero;
                for (int i = 0; i < particleNumber; ++i)
                {
                    Cout[j] += Velocity(AStruct[j].r, AStruct[i]);
                }
            });
        }


        private void ComputeParallelStructChain()
        {
            for (var k = 0; k < 3; k++)
                Parallel.For(0, particleNumber, j =>
            {
                Cout[j] = Vector2.Zero;
                for (int i = 0; i < particleNumber; ++i)
                {
                    Cout[j] += VelocityChain(AStruct[j].r, AStruct[i]);
                }
            });
        }

        private void ComputeParallelStructDouble()
        {
            for (var k = 0; k < 3; k++)
                Parallel.For(0, particleNumber, j =>
                {
                    CoutDouble[j] = new Vector(0, 0);
                    for (int i = 0; i < particleNumber; i++)
                    {
                        CoutDouble[j] += Velocity(AStructDouble[j].r, AStructDouble[i]);
                    }

                });
        }

        private Vector[] ComputeSequentialStructDouble()
        {
            Vector[] velocity = new Vector[particleNumber];
            for (int j = 0; j < particleNumber; j++)
            {
                velocity[j] = new Vector(0, 0);
                for (int i = 0; i < particleNumber; i++)
                {
                    velocity[j] += Velocity(AStructDouble[j].r, AStructDouble[i]);
                }

            };
            return velocity;
        }

        private T2[] ComputeOnGPU<T1, T2>(Shader shader, T1[] array, T2[] outArray)
            where T1 : struct
            where T2 : unmanaged
        {
            // create buffer on GPU
            var bufferA = new StorageBuffer(BufferUsageHint.StaticDraw);
            // send data to GPU
            bufferA.SetData(array, 0);
            // create buffer on GPU
            var bufferC = new StorageBuffer(BufferUsageHint.DynamicDraw);
            // allocate data on GPU
            bufferC.Allocate(outArray, 1);
            // make calculation on GPU
            for (var k = 0; k < 3; k++)
                shader.Compute(particleNumber / 128, 1, 1, 
                    MemoryBarrierFlags.ShaderStorageBarrierBit | MemoryBarrierFlags.BufferUpdateBarrierBit);
            // get data from GPU

            var arrayFromShader = bufferC.GetData<T2>();

            return arrayFromShader;
        }

        //private Vector[] ComputeOnGPUDouble<T>(Shader shader, T[] inData, Vector[] outData) where T : struct
        //{
        //    // create buffer on GPU
        //    var bufferinData = new StorageBuffer(BufferUsageHint.StaticDraw);
        //    // send data to GPU
        //    bufferinData.SetData(inData, 3);
        //    // create buffer on GPU
        //    var bufferOutData = new StorageBuffer(BufferUsageHint.DynamicDraw);
        //    // allocate data on GPU
        //    bufferOutData.Allocate(outData, 4);
        //    // make calculation on GPU
        //    shader.Compute(MemoryBarrierFlags.ShaderStorageBarrierBit | MemoryBarrierFlags.BufferUpdateBarrierBit,
        //        particleNumber / 128, 1, 1);
        //    // get data from GPU
        //    var arrayFromShader = bufferOutData.GetVectorData();

        //    return arrayFromShader;
        //}


        private Vector2 Velocity(Vector2 r, Vector4 vortex)
        {
            Vector2 dr = r - vortex.Xy;
            float dist = dr.Length;
            var selector = Step(vortex.Z, dist);
            dist = selector * dist + (1 - selector) * vortex.Z;
            return vortex.W * new Vector2(-dr.Y, dr.X) / (dist * dist);

        }

        private Vector2 Velocity(Vector2 r, Vortex vortex)
        {
            Vector2 dr = r - vortex.r;
            float dist = dr.Length;
            var selector = Step(vortex.radius, dist);
            dist = selector * dist + (1 - selector) * vortex.radius;
            return vortex.gamma * new Vector2(-dr.Y, dr.X) / (dist * dist);
        }

        private Vector2 VelocityChain(Vector2 r, Vortex vortex)
        {
            const float a = 2.0f;
            const float pi = (float)Math.PI;
            //const float koef = (float)(1.0 / 2.0 / Math.PI);
            const float k2 = 2.0f * pi / a;
            const float a2 = 2 * a;

            Vector2 dr = r - vortex.r;
            float dist = dr.Length;
            float self = Step(dist, vortex.radius);

            float denom = (1.0f - self) * a2 * ((float)Math.Cosh(k2 * dr.Y) - (float)Math.Cos(k2 * dr.X)) +
                                                    self * a2 * k2 * k2 * 0.5f * vortex.radius * vortex.radius;
            float u = -vortex.gamma * (float)Math.Sinh(k2 * dr.Y) / denom;
            float v = vortex.gamma * (float)Math.Sin(k2 * dr.X) / denom;
            //float vx = - koef * vortex.gamma * dr.y / (dist * dist + self);
            //float vy =  koef * vortex.gamma * dr.x / (dist * dist + self);
            //u = u;// - vx;
            //v = v;// - vy;

            //	float selector = step(vortex.rankinRadius, dist);
            //	dist = selector * dist + (1 - selector)*vortex.rankinRadius;
            //	vx = - koef * vortex.gamma * dr.y / (dist * dist);
            //	vy =  koef * vortex.gamma * dr.x / (dist * dist);
            //	u =  u + vx;
            //	v =  v + vy;
            //if(u > 10) u =1;
            //if(v > 10) v =1;

            //return koef * vortex.gamma * vec2( -dr.y, dr.x)/(dist*dist);
            return new Vector2(u, v);
        }

        private Vector Velocity(Vector r, VortexDouble vortex)
        {
            Vector dr = r - vortex.r;
            double dist = dr.Length;
            var selector = Step(vortex.radius, dist);
            dist = selector * dist + (1 - selector) * vortex.radius;
            return vortex.gamma * new Vector(-dr.Y, dr.X) / (dist * dist);
        }

        private int Step(float a, float x)
        {
            return x >= a ? 1 : 0;
        }

        private double Step(double a, double x)
        {
            return x >= a ? 1 : 0;
        }
    }

}

