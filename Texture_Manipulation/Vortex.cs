using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Texture_Manipulation
{
    
    public class Vortex
    {
        float _rankine2 = 0.01f;
        Vector3 _center;
        Vector3 _gamma;

        public Vortex(Vector3 center, Vector3 gamma)
        {
            _gamma = gamma;
            _center = center;

        }

        public Vector3 velocity(Vector3 r)
        {
            var delta = r - _center;
            float r2 = Vector3.Dot(delta, delta);
            if (r2 < _rankine2) r2 = _rankine2;
            return Vector3.Cross(delta, _gamma) / r2;
        }

        public Vector3 GetCenter(float dt, Vector3 velocity)
        {
            _center += velocity * dt;
            return _center;
        }
}

   
}
