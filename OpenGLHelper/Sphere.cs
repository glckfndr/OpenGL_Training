using System;
using GlmNet;

namespace OpenGLHelper
{
    public class Sphere: TriangleMesh
    {
        public Sphere(float radius, int nSlices, int nStacks)
        {

            int nVerts = (nSlices + 1) * (nStacks + 1);
            int elements = (nSlices * 2 * (nStacks - 1)) * 3;
            

            // Verts
            float[] p = new float[nVerts * 3];
            // Normals
            float[] n = new float[nVerts * 3];
            // Tex coords
            float[] tex = new float[nVerts * 2];
            // Elements
            int[] el = new int[elements];


            // Generate positions and normals
            float theta, phi;
            float thetaFac = _twoPi / nSlices;
            float phiFac = 0.5f *_twoPi / nStacks;
            float nx, ny, nz, s, t;
            int idx = 0, tIdx = 0;
            for (int i = 0; i <= nSlices; i++)
            {
                theta = i * thetaFac;
                s = (float)i / nSlices;
                for (int j = 0; j <= nStacks; j++)
                {
                    phi = j * phiFac;
                    t = (float)j / nStacks;
                    nx = glm.sin(phi) * glm.cos(theta);
                    ny = glm.sin(phi) * glm.sin(theta);
                    nz = glm.cos(phi);
                    p[idx] = radius * nx; p[idx + 1] = radius * ny; p[idx + 2] = radius * nz;
                    n[idx] = nx; n[idx + 1] = ny; n[idx + 2] = nz;
                    idx += 3;

                    tex[tIdx] = s;
                    tex[tIdx + 1] = t;
                    tIdx += 2;
                }
            }

            // Generate the element list
            idx = 0;
            for (int i = 0; i < nSlices; i++)
            {
                int stackStart = i * (nStacks + 1);
                int nextStackStart = (i + 1) * (nStacks + 1);
                for (int j = 0; j < nStacks; j++)
                {
                    if (j == 0)
                    {
                        el[idx] = stackStart;
                        el[idx + 1] = stackStart + 1;
                        el[idx + 2] = nextStackStart + 1;
                        idx += 3;
                    }
                    else if (j == nStacks - 1)
                    {
                        el[idx] = stackStart + j;
                        el[idx + 1] = stackStart + j + 1;
                        el[idx + 2] = nextStackStart + j;
                        idx += 3;
                    }
                    else
                    {
                        el[idx] = stackStart + j;
                        el[idx + 1] = stackStart + j + 1;
                        el[idx + 2] = nextStackStart + j + 1;
                        el[idx + 3] = nextStackStart + j;
                        el[idx + 4] = stackStart + j;
                        el[idx + 5] = nextStackStart + j + 1;
                        idx += 6;
                    }
                }
            }

            InitializeBuffers(el, p, n, tex);
        }
    }
}

