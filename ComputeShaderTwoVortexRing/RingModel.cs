using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputeShaderTwoVortexRing
{

    class RingModel
    {
        private readonly VortexCurve[] _rings;
        //private readonly VortexCurve _ring2;
        public VortexCurve GetRing(int i) => _rings[i - 1];
        //public VortexCurve GetRing2() => _ring2;
        private int _ringPointsNumber = 180;
        private double[] _ringRadius;
        public float GetRingRadius(int i) => (float)_ringRadius[i - 1];
        //private double _ring2Radius = 0.5;
        //public float GetRing2Radius() => (float)_ring2Radius;

        public RingModel()
        {
            _rings = new VortexCurve[2];
            _ringRadius = new double[] { 0.6, 0.5 };
            _rings[0] = new VortexCurve(-0.4, _ringPointsNumber, _ringRadius[0], -0.1);
            _rings[1] = new VortexCurve(0.4, _ringPointsNumber, _ringRadius[1], 0.0);
        }

        internal void InnerVelocity()
        {
            foreach (var ring in _rings)
            {
                ring.InVelocity(_rings);
            }
            //_ring1.InVelocity(_ring1,_ring2);
            //_ring2.InVelocity(_ring1, _ring2);
        }

        internal void SetSubStep()
        {

            foreach (var ring in _rings)
            {
                ring.SetSubStep();
            }
           // _ring1.SetSubStep();
           // _ring2.SetSubStep();
        }

        internal void InnerVelocitySubStep()
        {
            foreach (var ring in _rings)
            {
                ring.InVelocitySubStep(_rings);
            }
            //_ring1.InVelocitySubStep(_ring1, _ring2);
            //_ring2.InVelocitySubStep(_ring1, _ring2);
        }

        internal void MoveStep()
        {
            foreach (var ring in _rings)
            {
                ring.MoveStep();
            }
            //_ring1.MoveStep();
            //_ring2.MoveStep();
        }

        internal VortexPoint[] GetRingVortexPoints(int i)
        {
            return _rings[i - 1].ToVortexPointArray();
        }

        //internal VortexPoint[] GetRing1VortexPoints()
        //{
        //   return  _ring1.ToVortexPointArray();
        //}

        //internal VortexPoint[] GetRing2VortexPoints()
        //{
        //    return _ring2.ToVortexPointArray();
        //}

        internal int GetPointNumber() => _ringPointsNumber;
        
    }
}
