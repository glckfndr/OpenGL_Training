using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlmNet;
using OpenTK.Graphics.OpenGL4;

namespace ParticlesFeedBack
{
    class ParticleUtils
    {
        /**
     * Return a rotation matrix that rotates the y-axis to be parallel to dir.
     *
     * @param dir
     * @return
     */
        public static mat3 MakeArbitraryBasis(vec3  dir )
        {
            mat3 basis = new mat3(1);
            vec3 u, v, n;
            v = dir;
            n = glm.cross(new vec3(1,0,0), v);
            if((float)Math.Sqrt(glm.dot(n,n)) < 0.00001f )
            {
                n = glm.cross(new vec3(0,1,0), v);
            }
            u = glm.cross(v, n);
            basis[0] = glm.normalize(u);
            basis[1] = glm.normalize(v);
            basis[2] = glm.normalize(n);
            return basis;
        }

        /**
 * Creates a 1D texture of random floating point values in the range [0, 1)
 * @param size number of values
 * @return the texture id
 */
        public static int CreateRandomTex1D(int size)
        {
            Random rand = new Random();

            float[] randData= new float[size];
            for (int i = 0; i < randData.Length; i++)
            {
                randData[i] = (float)rand.NextDouble();
            }

            var sss = GL.GetInteger(GetPName.MaxTextureSize);

            var texId = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture1D, texId);
            GL.TexStorage1D(TextureTarget1d.Texture1D, 1,SizedInternalFormat.R32f, size);
            GL.TexSubImage1D(TextureTarget.Texture1D, 0, 0, size, PixelFormat.Red , PixelType.Float, randData);

            GL.TexParameter(TextureTarget.Texture1D,TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture1D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);

            return texId;
        }
    }
}
