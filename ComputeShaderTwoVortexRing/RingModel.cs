namespace ComputeShaderTwoVortexRing
{
    internal class RingModel
    {
        private readonly VortexCurve[] _rings;
        public VortexCurve GetRing(int i) => _rings[i - 1];

        private int _ringPointsNumber = 180;
        internal int GetPointNumber() => _ringPointsNumber;

        
        public float TimeStep { get; private set; }

        private double[] _ringRadius;
        public float GetRadius(int i) => (float)_rings[i - 1].GetRadius();

        public RingModel(double dt)
        {
            TimeStep = (float) dt;
            _rings = new VortexCurve[2];
            _ringRadius = new double[] { 0.6, 0.5 };
            _rings[0] = new VortexCurve(dt, -0.4, _ringPointsNumber, _ringRadius[0], -0.1);
            _rings[1] = new VortexCurve(dt, 0.4, _ringPointsNumber, _ringRadius[1], 0.0);
        }

        internal void SetSelfVelocity()
        {
            foreach (var ring in _rings)
            {
                ring.InVelocity(_rings);
            }
        }

        internal void SetSubStep()
        {
            foreach (var ring in _rings)
            {
                ring.SetSubStep();
            }            
        }

        internal void InnerVelocitySubStep()
        {
            foreach (var ring in _rings)
            {
                ring.InVelocitySubStep(_rings);
            }            
        }

        internal void MoveStep()
        {
            foreach (var ring in _rings)
            {
                ring.MoveStep();
            }            
        }

        internal VortexPoint[] GetRingVortexPoints(int i)
        {
            return _rings[i - 1].ToVortexPointArray();
        }        
        
    }
}
