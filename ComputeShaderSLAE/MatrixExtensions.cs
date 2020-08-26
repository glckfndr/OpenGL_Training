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
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    res[i] += m[i, j] * v[j];
                }
            }

            return res;
        }
    }
}
