using OpenGLHelper;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ComputeShaderTestUniform
{
    // розмір структури має бути кратим 8 байт, інакше неправильне вирівнювання на графічному процесорі
    struct Vortex
    {
        public Vector2 r;
        public float gamma;
        public float radius;
        public Vector3 dummy0;
        public float dummy1;


        public Vortex(Vector2 r, float gamma, float radius) : this()
        {
            this.r = r;
            this.gamma = gamma;
            this.radius = radius;
            dummy0 = Vector3.Zero;
            dummy1 = 0.5f;

        }

        
    }

    struct Column
    {
        public float[] col;
        public Column(float[] A)
        {
            col = A;

        }
    }

    internal class Game : GameWindow
    {
        public Game(int width, int height, string title) : base(width, height, GraphicsMode.Default, title)
        {

        }

        private const int particleNumber = 4096;
       // private Vector4[] A = new Vector4[particleNumber]; // Input array A
        private Vortex[] A = new Vortex[particleNumber]; // Input array A
        private Vector2[] C = new Vector2[particleNumber];// Output array


        protected override void OnLoad(EventArgs e)
        {
            var shader = new Shader("../../shaderUniform.comp");
            shader.SetInt("BufferSize", particleNumber);


            // fill with some sample values
            Random rnd = new Random();
            for (int i = 0; i < particleNumber; i++)
            {
                var x = (float)rnd.NextDouble();
                var y = (float)rnd.NextDouble();
                
                A[i] = new Vortex( new Vector2(x, y), 0.1f, 0.01f );
                C[i] = new Vector2(0);
            }

            Stopwatch stp = new Stopwatch();
            stp.Restart();
            
            var bufferA = new UniformBuffer(BufferUsageHint.StaticDraw);
            bufferA.SetData(A, 0);

            var bufferC = new StorageBuffer(BufferUsageHint.DynamicDraw);
            bufferC.SetData(C, 1);

            shader.Compute(particleNumber / 32, 1, 1, 
                MemoryBarrierFlags.ShaderStorageBarrierBit);
            var arrayFromShader = bufferC.GetVector2Data();
            stp.Stop();
            Console.WriteLine("Compute Time GPU and Get Data(ms): " + stp.ElapsedMilliseconds);
            
            stp.Restart();
            Parallel.For(0, particleNumber, j =>
            {
                for (int i = 0; i < particleNumber; ++i)
                {
                    C[j] += Velocity(A[j].r, A[i]);
                }
            }

            );
            
            stp.Stop();
            Console.WriteLine("Parallel Compute Time CPU(ms): " + stp.ElapsedMilliseconds);

            stp.Restart();
            for (int j = 0; j < particleNumber; j++)
            {
                C[j] = Vector2.Zero;
                for (int i = 0; i < particleNumber; ++i)
                {
                    C[j] += Velocity(A[j].r, A[i]);
                }
            }
            stp.Stop();
            Console.WriteLine("Compute Time CPU(ms): " + stp.ElapsedMilliseconds);
            ArrayComparer.Compare(C, arrayFromShader);
            base.OnLoad(e);
        }

        


        private Vector2 Velocity(Vector2 r, Vector4 vortex)
        {
            Vector2 dr = r - vortex.Xy;
            float dist = dr.Length;
            float selector = Step(vortex.Z, dist);
            dist = selector * dist + (1 - selector) * vortex.Z;
            return vortex.W * new Vector2(-vortex.Y, vortex.X) / (dist * dist);
        }

        private Vector2 Velocity(Vector2 r, Vortex vortex)
        {
            Vector2 dr = r - vortex.r;
            float dist = dr.Length;
            float selector = Step(vortex.radius, dist);
            dist = selector * dist + (1 - selector) * vortex.radius;
            return vortex.gamma * new Vector2(-vortex.r.Y, vortex.r.X) / (dist * dist);
        }

        private float Step(float a, float x)
        {
            return x >= a ? 1 : 0;
        }
    }

}

