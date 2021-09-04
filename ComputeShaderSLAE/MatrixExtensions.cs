using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace ComputeShaderSLAE
{
    public static class MatrixExtensions
    {
        public static Vector2 Mult(this Matrix2 m, Vector2 v)
        {
            var res = new Vector2(0);
            for (int j = 0; j < 2; j++)
            {
                for (int i = 0; i < 2; i++)
                {
                    // j - номер стрічки, i - номер стовпчика
                    res[j] += m[i, j] * v[i];
                }
            }

            return res;
        }

        public static Vector4 Mult(this Matrix4 m, Vector4 v)
        {
            var res = new Vector4(0);
            for (int j = 0; j < 4; j++)
            {
                for (int i = 0; i < 4; i++)
                {
                    // j - номер стрічки, i - номер стовпчика
                    res[j] += m[i, j] * v[i];
                }
            }

            return res;
        }

    }
}
