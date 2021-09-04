using System;
using System.Threading.Tasks;

namespace ComputeShaderTwoVortexRing
{
    internal class VortexCurve
    {
        // координати точок кривої
        private Vector3D[] _points;
        // координати точок кривої у напів кроці
        private Vector3D[] _pointsHalfStep;
        // швидкість точок кривої
        private Vector3D[] _velocity;
        private double _gamma;
        // крок по часу
        private double _dt = 0.001;

        public VortexCurve(double gamma, int n, double radius, double y)
        {
            _gamma = gamma;
            Ring(radius, n, y);

        }

        private void Ring(double radius, int n, double yCoord)
        {
            _points = new Vector3D[n + 1];
            _pointsHalfStep = new Vector3D[n + 1];
            _velocity = new Vector3D[n + 1];


            var dfi = 2 * Math.PI / n;
            for (int i = 0; i < n; i++)
            {
                var x = radius * Math.Cos(i * dfi);
                var y = yCoord;
                var z = radius * Math.Sin(i * dfi);
                _points[i] = new Vector3D(x, y, z);
            }
            _points[n] = _points[0];

        }

        public Vector3D Velocity(Vector3D r)
        {
            var velocity = new Vector3D(0);

            for (int i = 0; i < _points.Length - 1; i++)
            {
                velocity += VortexSegment.Velocity(r, _points[i],
                                                       _points[i + 1] - _points[i], _gamma);
            }

            return velocity;
        }

        //public Vector3D VelocitySubStep(Vector3D r)
        //{
        //    var velocity = new Vector3D(0);

        //    for (int i = 0; i < _points.Length - 1; i++)
        //    {
        //        velocity += VortexSegment.Velocity(r, _points[i],
        //            _points[i + 1] - _points[i], _gamma);
        //    }

        //    return velocity;
        //}


        public Vector3D VelocitySubStep(Vector3D r)
        {
            var velocity = new Vector3D(0);

            for (int i = 0; i < _pointsHalfStep.Length - 1; i++)
            {
                velocity += VortexSegment.Velocity(r, _pointsHalfStep[i],
                    _pointsHalfStep[i + 1] - _pointsHalfStep[i], _gamma);
            }

            return velocity;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vortexCurve"></param>
        public void InVelocity(params VortexCurve[] vortexCurve)
        {
            //for (int i = 0; i < _points.Length; i++)
            Parallel.For(0, _points.Length, i =>
            {

                _velocity[i] = new Vector3D(0);
                _velocity[i] += Velocity(_points[i]);

                foreach (var curve in vortexCurve)
                {
                    _velocity[i] += curve.Velocity(_points[i]);
                }


            });


        }


        public void InVelocitySubStep(params VortexCurve[] vortexCurve)
        {
            //for (int i = 0; i < _pointsHalfStep.Length; i++)
            Parallel.For(0, _points.Length, i =>
            {

                _velocity[i] = new Vector3D(0);
                _velocity[i] += VelocitySubStep(_pointsHalfStep[i]); // швидкість від свого кільця

                foreach (var curve in vortexCurve) // швидкість від інших кілець
                {
                    _velocity[i] += curve.VelocitySubStep(_pointsHalfStep[i]);
                }


            });

        }


        public void SetSubStep()
        {
            for (int i = 0; i < _points.Length; i++)
            {
                _pointsHalfStep[i] = _points[i] + _velocity[i] * 0.5 * _dt;
            }

        }

        public void MoveStep()
        {
            for (int i = 0; i < _points.Length; i++)
            {
                _points[i] = _points[i] + _velocity[i] * _dt;
            }

        }


        public VortexPoint[] ToVortexPointArray()
        {
            var outArray = new VortexPoint[_points.Length];
            for (int i = 0; i < _points.Length; i++)
            {
                outArray[i] = new VortexPoint(_points[i], _gamma);

            }

            return outArray;

        }

        public float GetCenter()
        {
            //var center = new Vector3D(0);
            //int n = 0;
            //for (var i = 0; i < _points.Length - 1; i++)
            //{

            //    center += _points[i];
            //}

            //return center / (_points.Length - 1);
            return (float)_points[0].Y;
        }

        public float GetRadius()
        {
            return (float)Math.Sqrt(_points[0].X * _points[0].X + _points[0].Z * _points[0].Z);
        }
    }
}
