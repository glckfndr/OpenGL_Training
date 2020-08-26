using OpenGLHelper;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ComputeShaderSLAE
{
    


    public struct Matrix
    {
        public float[] _a;
        private int _rowLength;
        private int _numberOfRows;
        public Matrix(int row, int col)
        {
            _rowLength = col;
            _numberOfRows = row;
            _a = new float[_rowLength * _numberOfRows];

        }

        public Matrix(float[,] m)
        {
            _rowLength = m.GetLength(1);
            _numberOfRows = m.GetLength(0);
            _a = new float[_rowLength * _numberOfRows];
            for (int i = 0; i < _numberOfRows; i++)
            {
                for (int j = 0; j < _rowLength; j++)
                {
                    _a[i * _rowLength + j] = m[i, j];
                }
            }

        }
        public void Set(float val, int i, int j)
        {
            _a[_rowLength * i + j] = val;
        }

        public float Get( int i, int j)
        {
            return _a[_rowLength * i + j];
        }

        public float[] GetData()
        {
            return _a;
        }

    }

    internal class Game : GameWindow
    {
        public Game(int width, int height, string title) : base(width, height, GraphicsMode.Default, title)
        {

        }

        private const int elementNumber = 4;
        //private Column[] A = new Column[elementNumber]; // Input array A
        private float[] A = new float[]
        {
             1, 2, 3, 4, 
             11,12,13,14,
             21,22,23,24,
             31,32,33,34,
             41,42,43,44,
        } ;

        private Matrix2[] AA = new Matrix2[elementNumber * elementNumber];
        
        private float[] B = new float[] { 5, 4, 0, 0};
        private Vector2[] BB = new Vector2[] {new Vector2(1,1), new Vector2(2, 1),
                                            new Vector2(3, 1), new Vector2(4, 1) };
        private float[] C = new float[]{-1,-1,-1,-1,-1};// Output array
        private Vector2 [] CC = new Vector2[elementNumber];// Output array
        //private Matrix2[] CC = new Matrix2[elementNumber];// Output array


        protected override void OnLoad(EventArgs e)
        {
            var shader = new Shader("../../slae.comp");
            shader.SetInt("RowLength", elementNumber);
            for (int i = 0; i < elementNumber * elementNumber; i++)
            {
                AA[i] = new Matrix2(new Vector2(i, i+1), new Vector2((i + 1)*10, (i +1)*10 + 1));
            }

            // fill with some sample values
            Random rnd = new Random();
            

            Stopwatch stp = new Stopwatch();
            stp.Restart();
            var bufferA = new StorageBuffer(BufferUsageHint.StaticDraw);
            //bufferA.SetData(A, 0);
            bufferA.SetData(AA, 0);

            var bufferB = new StorageBuffer(BufferUsageHint.StaticDraw);
            //bufferB.SetData(B, 1);
            bufferB.SetData(BB, 1);

            //var bufferA = new UniformBuffer(BufferUsageHint.StaticDraw);
            //bufferA.SetMatrix(A, 0);

            var bufferC = new StorageBuffer(BufferUsageHint.DynamicDraw);
            //bufferC.SetData(C, 2);
            bufferC.SetData(CC, 2);

            shader.Compute(MemoryBarrierFlags.ShaderStorageBarrierBit,
                elementNumber + 1 , 1, 1);
            //var arrayFromShader = bufferC.GetFloatData();
            //var result = bufferC.GetFloatData();
            var rrr = AA[0].Mult(BB[0]) + AA[1].Mult(BB[1]) + AA[2].Mult(BB[2]) + AA[3].Mult(BB[3]);
            
            var result = bufferC.GetVector2Data();
            stp.Stop();
            Console.WriteLine("Compute Time GPU and Get Data(ms): " + stp.ElapsedMilliseconds);


            base.OnLoad(e);
        }

        private void CompareArray(Vector4[] a, Vector4[] b)
        {
            for (int i = 0; i < a.Length; i++)
            {
                if ((a[i].Xy - b[i].Xy).Length > b[i].Length * 0.4e-6)
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



        private float Step(float a, float x)
        {
            return x >= a ? 1 : 0;
        }
    }

}

