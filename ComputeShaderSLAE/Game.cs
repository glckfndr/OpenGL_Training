using OpenGLHelper;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using BufferUsageHint = OpenTK.Graphics.OpenGL4.BufferUsageHint;
using MemoryBarrierFlags = OpenTK.Graphics.OpenGL4.MemoryBarrierFlags;

namespace ComputeShaderSLAE
{


    internal class Game : GameWindow
    {
        private int _local_size_x = 32;
        private int _local_size_y = 32;

        public Game(int width, int height, string title) : base(width, height, GraphicsMode.Default, title)
        {

        }


        private Column[] aCol = new Column[2];
        //{
        //    new Column(), new Column()
        //}; // Input array A




        private float[] A = new float[]
        {
             1, 2, 3, 4,
             11,12,13,14,
             21,22,23,24,
             31,32,33,34,
             41,42,43,44,
        };

        private float[] B = new float[] { 5, 4, 0, 0 };

        private float[] C = { -1, -1, -1, -1, -1 };// Output array
        private int _numberOfCycle = 16;

        protected override void OnLoad(EventArgs e)
        {
            //  Tester.CopyImageThenGetMatrix();
            // (float[] a, float[] b, float[] c) = MultipleFloatMatrixOnFloatVector();
            //MultipleMatrix2MatrixOnVector(a, b, c);
            //MultipleImageMatrix(a, b, c);
            //MultipleMatrix4MatrixOnVector(a, b, c);
            // MultipleImageMatrixOnImageMatrix();
            MultipleImage3DMatrixOnImageMatrix();
            base.OnLoad(e);
        }

        internal struct Column
        {
            public float[] col;
        }

        //private void SetMatrixAsArrayOfStruct()
        //{
        //    aCol[0].col = new float[] { 1, 2, 3 };
        //    aCol[1].col = new float[] { 4, 5, 6 };
        //    var bufferB = new StorageBuffer(BufferUsageHint.DynamicRead);
        //    bufferB.SetData(aCol, 1);
        //    var bufferC = new StorageBuffer(BufferUsageHint.DynamicDraw);
        //    bufferC.Allocate(aCol, 2);
        //    var shader = new Shader("../../slae6.comp");
        //    shader.Compute(1, 1, 1,
        //        MemoryBarrierFlags.ShaderStorageBarrierBit);

        //    var gpuResult = bufferC.GetData<Column>();

        //}

        private void MultipleImageMatrix(float[] a, float[] b, float[] c)
        {
            var rowLength = b.Length;
            var texture = new Texture2D32(rowLength, rowLength, a, 0);

            var bufferB = new StorageBuffer(BufferUsageHint.DynamicRead);
            bufferB.SetData(b, 1);

            var bufferC = new StorageBuffer(BufferUsageHint.DynamicDraw);
            bufferC.Allocate(c, 2);

            var shader = new Shader("../../slae3.comp");

            shader.SetInt("RowLength", rowLength);
            shader.Use();
            Stopwatch stp = new Stopwatch();
            stp.Restart();
            float[] gpuResult = null;
            for (int i = 0; i < _numberOfCycle; i++)
            {
                shader.Compute(1, rowLength / _local_size_y, 1,
                    MemoryBarrierFlags.ShaderStorageBarrierBit);

            }
            gpuResult = bufferC.GetFloatData();

            stp.Stop();
            Console.WriteLine("Image Compute Time GPU and Get Data(ms): " + stp.ElapsedMilliseconds);
            var cpuResult = MultipleFloatMatrixOnFloatVector(a, b);
            CompareArray(cpuResult, gpuResult);

        }

        private void MultipleImageMatrixOnImageMatrix()
        {
            var rowLength = 1024 * 2;
            var A = CreateFloatMatrix(rowLength);
            var B = CreateFloatMatrix(rowLength);
            var C = CreateFloatMatrix(rowLength);
            //  CompareArray(A, B);

            var texture = new Texture2D32(rowLength, rowLength, A, 0, TextureUnit.Texture0);
            var texture1 = new Texture2D32(rowLength, rowLength, B, 1, TextureUnit.Texture1);
            var texture2 = new Texture2D32(rowLength, rowLength, C, 2, TextureUnit.Texture2);

            var shader = new Shader("../../slae5.comp");

            shader.SetInt("RowLength", rowLength);
            shader.Use();
            Stopwatch stp = new Stopwatch();
            stp.Restart();
            //float[] gpuResult = null;
            for (int i = 0; i < 1; i++)
            {
                shader.Compute(rowLength / _local_size_x, rowLength / _local_size_y, 1,
                    MemoryBarrierFlags.ShaderStorageBarrierBit);

            }
            var gpuResult = texture2.GetData();
            Console.WriteLine("FloatMatrixOnFloatMatrix Compute Time GPU(ms): " + stp.ElapsedMilliseconds);
            stp.Stop();
            stp.Restart();
            var cpuResult = MultipleFloatMatrixOnFloatMatrix(A, B, rowLength);
            CompareArray(cpuResult, gpuResult);
            stp.Stop();
            Console.WriteLine("FloatMatrixOnFloatMatrix Compute Time CPU(ms): " + stp.ElapsedMilliseconds);

            Console.WriteLine("MultipleImageMatrixOnImageMatrix is finished!");

        }


        private void MultipleImage3DMatrixOnImageMatrix()
        {
            int width = 1024;
            int height = width;
            int depth = 4;
            var rowLength = width;
            var A = CreateFloatMatrix(width, height, depth);
            var B = CreateFloatMatrix(rowLength);
            var C = CreateFloatMatrix(rowLength);
            //  CompareArray(A, B);

            var texture = new Texture3D32(width, height, depth, A, 0, TextureUnit.Texture0);
            var texture1 = new Texture2D32(rowLength, rowLength, B, 1, TextureUnit.Texture1);
            var texture2 = new Texture2D32(rowLength, rowLength, C, 2, TextureUnit.Texture2);

            var shader = new Shader("../../slae6.comp");

            shader.SetInt("RowLength", rowLength);
            shader.Use();
            Stopwatch stp = new Stopwatch();
            stp.Restart();
            //float[] gpuResult = null;
            for (int i = 0; i < 1; i++)
            {
                shader.Compute(rowLength / _local_size_x, rowLength / _local_size_y, 1,
                    MemoryBarrierFlags.ShaderStorageBarrierBit);

            }
            var gpuResult = texture2.GetData();
            Console.WriteLine("FloatMatrixOnFloatMatrix Compute Time GPU(ms): " + stp.ElapsedMilliseconds);
            stp.Stop();
            stp.Restart();
             var cpuResult = MultipleFloatMatrix3DOnFloatMatrix(A, B, rowLength,3);
             CompareArray(cpuResult, gpuResult);
           // CompareArray(A, gpuResult, 3);

            stp.Stop();
            Console.WriteLine("FloatMatrixOnFloatMatrix Compute Time CPU(ms): " + stp.ElapsedMilliseconds);

            Console.WriteLine("MultipleImageMatrixOnImageMatrix is finished!");

        }





        private (float[], float[], float[]) MultipleFloatMatrixOnFloatVector()
        {
            var rowLength = 32;
            var A = CreateFloatMatrix(rowLength);
            var B = CreateVector(rowLength);
            var C = CreateVector(rowLength);

            Stopwatch stp = new Stopwatch();


            var shader = new Shader("../../slae.comp");

            shader.SetInt("RowLength", rowLength);
            shader.Use();

            var bufferA = new StorageBuffer(BufferUsageHint.StaticRead);
            bufferA.SetData(A, 0);

            //  var bufferA = new UniformBuffer(BufferUsageHint.StaticDraw);
            //  bufferA.SetData(A, 0);



            var bufferB = new StorageBuffer(BufferUsageHint.StaticRead);
            bufferB.SetData(B, 1);

            var bufferC = new StorageBuffer(BufferUsageHint.DynamicDraw);
            bufferC.Allocate(C, 2);


            stp.Restart();
            for (int i = 0; i < _numberOfCycle; i++)
            {
                // shader.Compute(rowLength / _local_size_x, 1, 1,
                //   MemoryBarrierFlags.ShaderStorageBarrierBit);
                shader.Compute(rowLength / _local_size_x, rowLength / _local_size_y, 1,
                   MemoryBarrierFlags.ShaderStorageBarrierBit);
            }

            var gpuResult = bufferC.GetFloatData();
            stp.Stop();
            Console.WriteLine("FloatMatrixOnFloatVector Compute Time GPU and Get Data(ms): " + stp.ElapsedMilliseconds);

            stp.Restart();
            float[] cpuResult = null;
            for (int i = 0; i < _numberOfCycle; i++)
            {
                cpuResult = MultipleFloatMatrixOnFloatVector(A, B);
            }

            stp.Stop();
            Console.WriteLine("FloatMatrixOnFloatVector Compute Time CPU(ms): " + stp.ElapsedMilliseconds);

            CompareArray(cpuResult, gpuResult);
            return (A, B, C);

        }

        private void MultipleMatrix2MatrixOnVector(float[] a, float[] b, float[] c)
        {
            int rowLength = b.Length;
            var shader = new Shader("../../slae2.comp");

            shader.SetInt("RowLength", rowLength / 2);

            var A = GetMatrixAsRowMatrix2(a);
            var B = GetVectorAsVector2(b);
            var C = GetVectorAsVector2(c);

            Stopwatch stp = new Stopwatch();

            shader.Use();

            var bufferA = new StorageBuffer(BufferUsageHint.DynamicRead);
            bufferA.SetData(A, 0);

            var bufferB = new StorageBuffer(BufferUsageHint.DynamicRead);
            bufferB.SetData(B, 1);

            var bufferC = new StorageBuffer(BufferUsageHint.DynamicDraw);
            bufferC.Allocate(C, 2);
            stp.Restart();
            for (int i = 0; i < _numberOfCycle; i++)
                shader.Compute(rowLength / 2 / _local_size_x, 1, 1, MemoryBarrierFlags.ShaderStorageBarrierBit);
            var gpuResult = bufferC.GetVector2Data();
            // var gpuResult1 = bufferCC.GetData<Vector2>();
            stp.Stop();
            Console.WriteLine("MultipleMatrix2MatrixOnVector Compute Matrix2 Time GPU and Get Data(ms): " + stp.ElapsedMilliseconds);

            //  stp.Restart();
            //  Vector2[] cpuResult1 = null;
            //  for (int i = 0; i < _numberOfCycle; i++)
            var cpuResult = Multiple(A, B);
            // stp.Stop();
            // Console.WriteLine("MultipleMatrix2Matrix2OnVector Compute Time CPU(ms): " + stp.ElapsedMilliseconds);

            CompareArray(cpuResult, gpuResult);
        }

        private void MultipleMatrix4MatrixOnVector(float[] a, float[] b, float[] c)
        {
            int rowLength = b.Length;
            var shader = new Shader("../../slae4.comp");

            shader.SetInt("RowLength", rowLength / 4);

            var A = GetMatrixAsRowMatrix4(a);
            var B = GetVectorAsVector4(b);
            var C = GetVectorAsVector4(c);

            Stopwatch stp = new Stopwatch();

            shader.Use();

            var bufferA = new StorageBuffer(BufferUsageHint.DynamicRead);
            bufferA.SetData(A, 0);

            var bufferB = new StorageBuffer(BufferUsageHint.DynamicRead);
            bufferB.SetData(B, 1);

            var bufferC = new StorageBuffer(BufferUsageHint.DynamicDraw);
            bufferC.Allocate(C, 2);
            stp.Restart();
            for (int i = 0; i < _numberOfCycle; i++)
                shader.Compute(rowLength / 4 / _local_size_x, 1, 1,
                    MemoryBarrierFlags.ShaderStorageBarrierBit);
            var gpuResult = bufferC.GetVector4Data();
            // var gpuResult1 = bufferCC.GetData<Vector2>();
            stp.Stop();
            Console.WriteLine("MultipleMatrix4MatrixOnVector Compute Matrix2 Time GPU and Get Data(ms): " + stp.ElapsedMilliseconds);

            var cpuResult = Multiple(A, B);


            CompareArray(cpuResult, gpuResult);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"> одновимірний масив, що представляє квадратну матрицю</param>
        /// <returns>одновимірний масив Matrix2, що представляє квадратну матрицю</returns>
        private Matrix2[] GetMatrixAsRowMatrix2(float[] a)
        {
            int n = (int)Math.Sqrt(a.Length);
            Matrix2[] b = new Matrix2[a.Length / 4];
            var k = 0;
            //for (int i = 0; i < n - 1; i += 2)
            //{
            //    for (int j = 0; j < n; j += 2)
            //    {
            //        int row = i * n;
            //        int row1 = (i + 1) * n;
            //        b[k].Column0 = new Vector2(a[row + j], a[row1 + j]);
            //        b[k].Column1 = new Vector2(a[row + j + 1], a[row1 + j + 1]);
            //        k++;

            //    }
            //}

            for (int j = 0; j < n - 1; j += 2)
            {
                for (int i = 0; i < n; i += 2)
                {
                    int row = j * n;
                    int row1 = (j + 1) * n;
                    b[k].Row0 = new Vector2(a[row + i], a[row1 + i]);
                    b[k].Row1 = new Vector2(a[row + i + 1], a[row1 + i + 1]);
                    k++;

                }
            }

            return b;
        }


        private Matrix4[] GetMatrixAsRowMatrix4(float[] a)
        {
            int n = (int)Math.Sqrt(a.Length);
            Matrix4[] b = new Matrix4[a.Length / 16];
            var k = 0;

            for (int j = 0; j < n - 1; j += 4)
            {
                for (int i = 0; i < n; i += 4)
                {
                    int row = j * n;
                    int row1 = (j + 1) * n;
                    int row2 = (j + 2) * n;
                    int row3 = (j + 3) * n;
                    b[k].Row0 = new Vector4(a[row + i], a[row1 + i], a[row2 + i], a[row3 + i]);
                    b[k].Row1 = new Vector4(a[row + i + 1], a[row1 + i + 1], a[row2 + i + 1], a[row3 + i + 1]);
                    b[k].Row2 = new Vector4(a[row + i + 2], a[row1 + i + 2], a[row2 + i + 2], a[row3 + i + 2]);
                    b[k].Row3 = new Vector4(a[row + i + 3], a[row1 + i + 3], a[row2 + i + 3], a[row3 + i + 3]);
                    k++;

                }
            }

            return b;
        }

        private Vector2[] GetVectorAsVector2(float[] a)
        {
            int n = a.Length;
            Vector2[] b = new Vector2[n / 2];
            var k = 0;
            for (int i = 0; i < a.Length; i += 2)
            {
                b[k] = new Vector2(a[i], a[i + 1]);

                k++;
            }

            return b;
        }


        private Vector4[] GetVectorAsVector4(float[] a)
        {
            int n = a.Length;
            Vector4[] b = new Vector4[n / 4];
            var k = 0;
            for (int i = 0; i < a.Length; i += 4)
            {
                b[k] = new Vector4(a[i], a[i + 1], a[i + 2], a[i + 3]);

                k++;
            }

            return b;
        }


        private float[] CreateVector(int rowLength)
        {
            float[] a = new float[rowLength];
            Random rnd = new Random();
            for (int i = 0; i < rowLength; i++)
            {
                a[i] = (float)rnd.NextDouble();
            }
            return a;
        }

        private float[] CreateFloatMatrix(int rowLength)
        {
            float[] a = new float[rowLength * rowLength];
            Random rnd = new Random(DateTime.Now.Millisecond);
            for (int i = 0; i < rowLength * rowLength; i++)
            {
                a[i] = (float)rnd.NextDouble();
            }

            return a;
        }
        
        private float[] CreateFloatMatrix(int width, int height, int depth)
        {
            float[] a = new float[width * height * depth];
            Random rnd = new Random(DateTime.Now.Millisecond);
            for (int k = 0; k < depth; k++)
            {
                for (int j = 0; j < height; j++)
                {
                    for (int i = 0; i < width; i++)
                    {
                        int n = i + j * width + k * (width * height);
                        a[n] = (float)rnd.NextDouble() + k;
                    }
                }
                
            }

            return a;
        }



        private void CompareArray(Vector2[] a, Vector2[] b)
        {
            for (int i = 0; i < a.Length; i++)
            {
                if ((a[i].Yx - b[i].Yx).Length > b[i].Length * 5.0e-6)
                {
                    Console.WriteLine("i: " + i + " => " + a[i] + " != " + b[i]);
                }
            }
        }


        private void CompareArray(Vector4[] a, Vector4[] b)
        {
            for (int i = 0; i < a.Length; i++)
            {
                if ((a[i] - b[i]).Length > b[i].Length * 5.0e-6)
                {
                    Console.WriteLine("i: " + i + " => " + a[i] + " != " + b[i]);
                }
            }
        }


        private void CompareArray(float[] a, float[] b)
        {
            for (int i = 0; i < a.Length; i++)
            {
                if (Math.Abs(a[i] - b[i]) > Math.Abs(b[i]) * 5.0e-6)
                {
                    Console.WriteLine("i: " + i + " => " + a[i] + " != " + b[i]);
                }
            }
        }

        private void CompareArray(float[] a, float[] b, int z)
        {
            for (int i = 0; i < b.Length; i++)
            {
                if (Math.Abs(a[i + b.Length * z] - b[i]) > Math.Abs(b[i]) * 5.0e-6)
                {
                    Console.WriteLine("i: " + i + " => " + a[i + b.Length * z] + " != " + b[i]);
                }
            }
        }





        private Vector2[] Multiple(Matrix2[] a, Vector2[] b)
        {
            var rowLength = b.Length;
            Vector2[] x = new Vector2[rowLength];
            for (int j = 0; j < rowLength; j++)
            {
                Vector2 sum = new Vector2(0, 0);
                for (int i = 0; i < rowLength; i++)
                {
                    sum += a[i + rowLength * j].Mult(b[i]);
                }

                x[j] = sum;
            }

            return x;

        }

        private Vector4[] Multiple(Matrix4[] a, Vector4[] b)
        {
            var rowLength = b.Length;
            Vector4[] x = new Vector4[rowLength];
            for (int j = 0; j < rowLength; j++)
            {
                Vector4 sum = new Vector4(0, 0, 0, 0);
                for (int i = 0; i < rowLength; i++)
                {
                    sum += a[i + rowLength * j].Mult(b[i]);
                }

                x[j] = sum;
            }

            return x;

        }

        private float[] MultipleFloatMatrixOnFloatVector(float[] a, float[] b)
        {
            // a квадратна матриця як одновимірний масив
            var rowLength = b.Length;
            float[] x = new float[rowLength];
            Parallel.For(0, rowLength, (i) =>
            {
                float sum = 0;
                for (int j = 0; j < rowLength; j++)
                {
                    sum += a[i*rowLength + j] * b[j];
                }

                x[i] = sum;
            });

            return x;

        }


        private float[] MultipleFloatMatrixOnFloatMatrix(float[] a, float[] b, int rowLength)
        {
            // a квадратна матриця як одновимірний масив

            float[] x = new float[a.Length];
            Parallel.For(0, rowLength, (i) =>
            {
                for (int j = 0; j < rowLength; j++)
                {
                    float sum = 0;
                    for (int k = 0; k < rowLength; k++)
                    {
                        sum += a[k + rowLength * j] * b[i + rowLength * k];
                    }

                    x[i + rowLength * j] = sum;
                }
            });

            return x;

        }

        private float[] MultipleFloatMatrix3DOnFloatMatrix(float[] a, float[] b, int rowLength, int depth)
        {
            // a квадратна матриця як одновимірний масив

            float[] x = new float[b.Length];
            Parallel.For(0, rowLength, (i) =>
            {
                for (int j = 0; j < rowLength; j++)
                {
                    float sum = 0;
                    for (int k = 0; k < rowLength; k++)
                    {
                        sum += a[k + rowLength * j + depth * rowLength * rowLength] * b[i + rowLength * k];
                    }

                    x[i + rowLength * j] = sum;
                }
            });

            return x;

        }





        private float Step(float a, float x)
        {
            return x >= a ? 1 : 0;
        }
    }


}

