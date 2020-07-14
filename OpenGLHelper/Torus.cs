using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlmNet;

namespace OpenGLHelper
{
    public class Torus: TriagleMesh
    {
        public Torus(float outerRadius, float innerRadius, int nsides, int nrings)
        {
            int faces = nsides * nrings;
            int nVerts = nsides * (nrings + 1);   // One extra ring to duplicate first ring

            // Points
            float[] p = new float[nVerts * 3];
            float[] n = new float[nVerts * 3];
            float[] tex = new float[nVerts * 2];
            int[] el = new int[faces * 6];

            

            // Generate the vertex data
            float ringFactor = (float)(Math.PI*2) / nrings;
            float sideFactor = (float)(Math.PI * 2) / nsides;
            int idx = 0, tidx = 0;
            for (int ring = 0; ring <= nrings; ring++)
            {
                float u = ring * ringFactor;
                float cu = glm.cos(u);
                float su = glm.sin(u);
                for (int side = 0; side < nsides; side++)
                {
                    float v = side * sideFactor;
                    float cv = glm.cos(v);
                    float sv = glm.sin(v);
                    float r = (outerRadius + innerRadius * cv);
                    p[idx] = r * cu;
                    p[idx + 1] = r * su;
                    p[idx + 2] = innerRadius * sv;
                    n[idx] = cv * cu * r;
                    n[idx + 1] = cv * su * r;
                    n[idx + 2] = sv * r;
                    tex[tidx] = u / (float)(Math.PI * 2);
                    tex[tidx + 1] = v / (float)(Math.PI * 2);
                    tidx += 2;
                    // Normalize
                    float len = (float)Math.Sqrt(n[idx] * n[idx] + n[idx + 1] * n[idx + 1] + n[idx + 2] * n[idx + 2]);
                    n[idx] /= len;
                    n[idx + 1] /= len;
                    n[idx + 2] /= len;
                    idx += 3;
                }
            }

            idx = 0;
            for (int ring = 0; ring < nrings; ring++)
            {
                int ringStart = ring * nsides;
                int nextRingStart = (ring + 1) * nsides;
                for (int side = 0; side < nsides; side++)
                {
                    int nextSide = (side + 1) % nsides;
                    // The quad
                    el[idx] = (ringStart + side);
                    el[idx + 1] = (nextRingStart + side);
                    el[idx + 2] = (nextRingStart + nextSide);
                    el[idx + 3] = ringStart + side;
                    el[idx + 4] = nextRingStart + nextSide;
                    el[idx + 5] = (ringStart + nextSide);
                    idx += 6;
                }
            }

            InitializeBuffers(el, p, n, tex);
        }
    }
}
