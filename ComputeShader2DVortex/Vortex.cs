using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlmNet;

namespace ComputeShader2DVortex
{
    internal struct Vortex
    {
        private vec2 _r;
        private float _gamma;
        private float _rankineRadius;
        
        public Vortex(vec2 r, float gamma, float rankineRadius)
        {
            _r = r;
            _gamma = gamma;
            _rankineRadius = rankineRadius;
        }
    };
}
