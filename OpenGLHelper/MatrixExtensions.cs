using GlmNet;
using OpenTK;

namespace OpenGLHelper
{
    public static class MatrixExtensions
    {
        public static Matrix4 ConvertToMatrix4(this mat4 mat)
        {
            Matrix4 Mat = new Matrix4();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    Mat[i, j] = mat[i, j];
                }
            }

            return Mat;
        }

        public static Matrix3 ConvertToMatrix3(this mat3 mat)
        {
            Matrix3 Mat = new Matrix3();
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Mat[i, j] = mat[i, j];
                }
            }

            return Mat;
        }

        public static Vector3 ConvertToVector3(this vec3 v)
        {
            Vector3 vec = new Vector3();
            for (int i = 0; i < 3; i++)
            {
                
                    vec[i] = v[i];
                
            }

            return vec;
        }
    }
}
