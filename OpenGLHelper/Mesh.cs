using OpenTK;
using System.Collections.Generic;

namespace OpenGLHelper
{
    public class Mesh
    {
        private static float xMin = -0.5f;
        private static float yMin = -0.5f;
        private static float xMax = 0.5f;
        private static float yMax = 0.5f;
        private static float stepX;
        private static float stepY;
        private static uint imax = 10;
        private static uint jmax = 10;

        public static (float[] vertices, uint[] indices) PlainXY(Vector3 center, float width, float height, uint nx, uint ny)
        {

            imax = nx;
            jmax = ny;
            xMin = center.X - width / 2;
            xMax = center.X + width / 2;

            yMin = center.Y - height / 2;
            yMax = center.Y + height / 2;

            List<float> vertices = new List<float>(1000);
            stepX = (xMax - xMin) / imax;
            stepY = (yMax - yMin) / jmax;


            for (var i = 0; i <= imax; i++)
            {
                for (var j = 0; j <= jmax; j++)
                {
                    vertices.Add(xMin + i * stepX);
                    vertices.Add(yMin + j * stepY);
                    vertices.Add(center.Z);
                }
            }

            var indices = IndicesForPlain();
            return (vertices.ToArray(), indices);

        }


        public static (float[] vertices, uint[] indices) PlainXYUV(Vector3 center, float width, float height, uint nx, uint ny)
        {

            imax = nx;
            jmax = ny;
            xMin = center.X - width / 2;
            xMax = center.X + width / 2;

            yMin = center.Y - height / 2;
            yMax = center.Y + height / 2;

            List<float> vertices = new List<float>(1000);
            stepX = (xMax - xMin) / imax;
            stepY = (yMax - yMin) / jmax;
            var stepU = 1.0f / nx;
            var stepV = 1.0f / ny;


            for (var i = 0; i <= imax; i++)
            {
                for (var j = 0; j <= jmax; j++)
                {
                    vertices.Add(xMin + i * stepX);
                    vertices.Add(yMin + j * stepY);
                    vertices.Add(center.Z);
                    vertices.Add(i * stepU);
                    vertices.Add(j * stepV);
                }
            }

            var indices = IndicesForPlain();
            return (vertices.ToArray(), indices);

        }

        public static (float[], uint[]) PlainXY()
        {
            xMin = -0.5f;
            yMin = -0.5f;
            xMax = 0.5f;
            yMax = 0.5f;
            imax = 10;
            jmax = 10;

            List<float> vertices = new List<float>(100);
            stepX = (xMax - xMin) / imax;
            stepY = (yMax - yMin) / jmax;
            for (var i = 0; i <= imax; i++)
            {
                for (var j = 0; j <= jmax; j++)
                {
                    vertices.Add(xMin + i * stepX);
                    vertices.Add(yMin + j * stepY);
                    vertices.Add(0);


                }
            }
            var indices = IndicesForPlain();
            return (vertices.ToArray(), indices);


        }

        public static (float[], uint[]) PlainXYUV()
        {
            xMin = -0.5f;
            yMin = -0.5f;
            xMax = 0.5f;
            yMax = 0.5f;
            imax = 32;
            jmax = 32;
            
            List<float> vertices = new List<float>(100);

            stepX = (xMax - xMin) / imax;
            stepY = (yMax - yMin) / jmax;
            var stepU = 1.0f / imax;
            var stepV = 1.0f / jmax;

            for (var i = 0; i <= imax; i++)
            {
                for (var j = 0; j <= jmax; j++)
                {
                    vertices.Add(xMin + i * stepX);
                    vertices.Add(yMin + j * stepY);
                    vertices.Add(0);
                    vertices.Add(i * stepU);
                    vertices.Add(j * stepV);


                }
            }
            var indices = IndicesForPlain();
            return (vertices.ToArray(), indices);


        }

        private static uint[] IndicesForPlain()
        {
            List<uint> indices = new List<uint>(100);

            var jNum = jmax + 1;
            for (uint i = 0; i < imax; i++)
            {
                for (uint j = 0; j < jmax; j++)
                {
                    indices.Add(i * jNum + j);
                    indices.Add(i * jNum + j + 1);
                    indices.Add((i + 1) * jNum + j);

                    indices.Add((i + 1) * jNum + j);
                    indices.Add(i * jNum + j + 1);
                    indices.Add((i + 1) * jNum + j + 1);

                }
            }
            return indices.ToArray();

        }
    }
}
