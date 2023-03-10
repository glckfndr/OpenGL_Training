using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace ComputeShaderTwoVortexRing
{
    struct VortexPoint
    {
        public Vector3 _r;
        public float _gamma;
        //public float _dummy;
        
        public VortexPoint(Vector3D r, double gamma) : this()
        {
            _r.X = (float)r.X;
            _r.Y = (float)r.Y;
            _r.Z = (float)r.Z;
            //_r.W = 1.0f;
            _gamma = (float)gamma;
            //_dummy = 0.0f;
        }
    }
}
