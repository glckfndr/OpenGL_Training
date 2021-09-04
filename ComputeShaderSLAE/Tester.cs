using OpenGLHelper;
using OpenTK.Graphics.OpenGL4;
using System;

namespace ComputeShaderSLAE
{
    public static class Tester
    {
        public static void CopyImageThenGetMatrix()
        {
            // copy matrix into GPU then read from GPU
            var rowLength = 12;
            var colLength = 4;
            var shader3 = new Shader("../../getmatrix.comp");
            shader3.SetInt("RowLength", rowLength);
            var data_txt = GetColMatrix(colLength, rowLength);

            var texture = new Texture2D32(rowLength, colLength, data_txt, 0);
            var outMatrixBuffer = new StorageBuffer(BufferUsageHint.DynamicDraw);
            outMatrixBuffer.Allocate(data_txt, 1);
            shader3.Compute(rowLength / 4, colLength / 4, 1,
                MemoryBarrierFlags.ShaderStorageBarrierBit);
            var resmatr = outMatrixBuffer.GetFloatData();
            //CompareArray(resmatr, data_txt);
            PrintArray(data_txt, rowLength);
            PrintArray(resmatr, rowLength);
        }

        private static float[] GetColMatrix(int colLength, int rowLength)
        {
            float[] data_txt = new float[colLength * rowLength];
            int k = 0;
            float dx = 0.01f;
            float dy = 10.0f;
            // на відеокарті у файлі зображення перший індекс це стовпчик, другий стрічка
            for (int j = 0; j < colLength; j++)
            {
                for (int i = 0; i < rowLength; i++)
                {
                    k = j * rowLength + i;
                    data_txt[k] = i * dx + j * dy;
                }
            }
            return data_txt;
        }


        public static float[,] GetImageMatrix(float[] a, int rowLength, int colLength)
        {
            float[,] data_txt = new float[rowLength, rowLength];
            int k = 0;

            // на відеокарті у файлі зображення перший індекс це стовпчик, другий стрічка
            for (int j = 0; j < colLength; j++)
            {
                for (int i = 0; i < rowLength; i++)
                {
                    k = j * rowLength + i;
                    data_txt[i, j] = a[k];
                }
            }

            return data_txt;
        }

        private static void PrintArray(float[] a, int rowlan)
        {
            for (int i = 0; i < a.Length / rowlan; i++)
            {

                Console.Write($" {i}---- >  ");
                for (int j = 0; j < rowlan; j++)
                {
                    var ind = i * rowlan + j;
                    Console.Write($" {a[ind]}, ");
                    if (j == rowlan - 1)
                        Console.WriteLine();

                }
            }

            Console.WriteLine("**********************************************");
        }
    }
}
