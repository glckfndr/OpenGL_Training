using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace OpenGLHelper
{
    public static class ArrayComparer
    {
        public static void Compare(Vector2[] a, Vector4[] b, float eps = 0.5e-6f)
        {
            for (int i = 0; i < a.Length; i++)
            {
                if ((a[i] - b[i].Xy).Length > b[i].Length * eps)
                {
                    Console.WriteLine("i: " + i + " => " + a[i].Length + " != " + b[i].Length);
                }
            }
        }

        public static void Compare(Vector2[] a, Vector2[] b, float eps = 0.5e-6f)
        {
            for (int i = 0; i < a.Length; i++)
            {
                if ((a[i] - b[i]).Length > b[i].Length * eps)
                {
                    Console.WriteLine("i: " + i + " => " + a[i].Length + " != " + b[i].Length);
                }
            }
        }

        public static void Compare(float[] a, float[] b, float eps = 0.5e-6f)
        {
            for (int i = 0; i < a.Length; i++)
            {
                if (Math.Abs(a[i] - b[i]) > Math.Abs(b[i]) * eps)
                {
                    Console.WriteLine("i: " + i + " => " + a[i] + " != " + b[i]);
                }
            }
        }

        public static void Compare(Vector4[] a, Vector4[] b, float eps = 0.5e-6f)
        {
            for (int i = 0; i < a.Length; i++)
            {
                if ((a[i].Xy - b[i].Xy).Length > b[i].Length * eps)
                {
                    Console.WriteLine("i: " + i + " => " + a[i].Length + " != " + b[i].Length);
                }
            }
        }
    }
}
