using OpenTK.Graphics.OpenGL4;
using System.Collections.Generic;

namespace OpenGLHelper
{
    public class Cube: TriagleMesh
    {
        
        public Cube(float side)
        {
            _buffers = new List<int>();
            float side2 = side / 2.0f;

            List<float> points = new List<float>()
            {
                // Front
               -side2, -side2, side2, side2, -side2, side2, side2,  side2, side2,  -side2,  side2, side2,
               // Right
                side2, -side2, side2, side2, -side2, -side2, side2,  side2, -side2, side2,  side2, side2,
               // Back
               -side2, -side2, -side2, -side2,  side2, -side2, side2,  side2, -side2, side2, -side2, -side2,
               // Left
               -side2, -side2, side2, -side2,  side2, side2, -side2,  side2, -side2, -side2, -side2, -side2,
               // Bottom
               -side2, -side2, side2, -side2, -side2, -side2, side2, -side2, -side2, side2, -side2, side2,
               // Top
               -side2,  side2, side2, side2,  side2, side2, side2,  side2, -side2, -side2,  side2, -side2
            };

            List<float> normals = new List<float>()
            {
                // Front
                0.0f, 0.0f, 1.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, 1.0f,
                // Right
                1.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f,
                // Back
                0.0f, 0.0f, -1.0f, 0.0f, 0.0f, -1.0f, 0.0f, 0.0f, -1.0f, 0.0f, 0.0f, -1.0f,
                // Left
                -1.0f, 0.0f, 0.0f, -1.0f, 0.0f, 0.0f, -1.0f, 0.0f, 0.0f, -1.0f, 0.0f, 0.0f,
                // Bottom
                0.0f, -1.0f, 0.0f, 0.0f, -1.0f, 0.0f, 0.0f, -1.0f, 0.0f, 0.0f, -1.0f, 0.0f,
                // Top
                0.0f, 1.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, 1.0f, 0.0f
            };

            List<float> textureCoords = new List<float>()
            {
                // Front
                0.0f, 0.0f, 1.0f, 0.0f, 1.0f, 1.0f, 0.0f, 1.0f,
                // Right
                0.0f, 0.0f, 1.0f, 0.0f, 1.0f, 1.0f, 0.0f, 1.0f,
                // Back
                0.0f, 0.0f, 1.0f, 0.0f, 1.0f, 1.0f, 0.0f, 1.0f,
                // Left
                0.0f, 0.0f, 1.0f, 0.0f, 1.0f, 1.0f, 0.0f, 1.0f,
                // Bottom
                0.0f, 0.0f, 1.0f, 0.0f, 1.0f, 1.0f, 0.0f, 1.0f,
                // Top
                0.0f, 0.0f, 1.0f, 0.0f, 1.0f, 1.0f, 0.0f, 1.0f
            };

            List<int> indices = new List<int>()
            {
                0,1,2,0,2,3,
                4,5,6,4,6,7,
                8,9,10,8,10,11,
                12,13,14,12,14,15,
                16,17,18,16,18,19,
                20,21,22,20,22,23
            };

            InitializeBuffers(indices.ToArray(), points.ToArray(), normals.ToArray(), textureCoords.ToArray());
        }

    }
}
