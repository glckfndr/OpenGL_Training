namespace OpenGLHelper
{
    public class Plane : TriangleMesh
    {
        public Plane(float xsize, float zsize, int xdivs, int zdivs, float smax = 1.0f, float tmax = 1.0f)
        {
            int nPoints = (xdivs + 1) * (zdivs + 1);
            float[] points = new float[3 * nPoints];
            float[] normals = new float[3 * nPoints];
            float[] textureCoords = new float[2 * nPoints];
            float[] tang = new float[4 * nPoints];
            int[] elements = new int[6 * xdivs * zdivs];

            float x2 = xsize / 2.0f;
            float z2 = zsize / 2.0f;
            float iFactor = (float)zsize / zdivs;
            float jFactor = (float)xsize / xdivs;
            float texi = smax / xdivs;
            float texj = tmax / zdivs;
            float x, z;
            int vidx = 0, tidx = 0;
            for (int i = 0; i <= zdivs; i++)
            {
                z = iFactor * i - z2;
                for (int j = 0; j <= xdivs; j++)
                {
                    x = jFactor * j - x2;
                    points[vidx] = x;
                    points[vidx + 1] = 0.0f;
                    points[vidx + 2] = z;
                    normals[vidx] = 0.0f;
                    normals[vidx + 1] = 1.0f;
                    normals[vidx + 2] = 0.0f;

                    textureCoords[tidx] = j * texi;
                    textureCoords[tidx + 1] = (zdivs - i) * texj;

                    vidx += 3;
                    tidx += 2;
                }
            }

            for (int i = 0; i < nPoints; i++)
            {
                tang[i * 4 + 0] = 1.0f;
                tang[i * 4 + 1] = 0.0f;
                tang[i * 4 + 2] = 0.0f;
                tang[i * 4 + 3] = 1.0f;
            }

            int idx = 0;
            for (int i = 0; i < zdivs; i++)
            {
                var rowStart = (int)(i * (xdivs + 1));
                var nextRowStart = (int)((i + 1) * (xdivs + 1));
                for (int j = 0; j < xdivs; j++)
                {
                    elements[idx] = rowStart + j;
                    elements[idx + 1] = nextRowStart + j;
                    elements[idx + 2] = nextRowStart + j + 1;
                    elements[idx + 3] = rowStart + j;
                    elements[idx + 4] = nextRowStart + j + 1;
                    elements[idx + 5] = rowStart + j + 1;
                    idx += 6;
                }
            }

            InitializeBuffers(elements, points, normals, textureCoords, tang);
        }

    }
}
