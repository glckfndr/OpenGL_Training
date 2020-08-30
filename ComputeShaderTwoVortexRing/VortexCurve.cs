using System;
using System.Runtime.InteropServices.WindowsRuntime;

namespace ComputeShaderTwoVortexRing
{
    internal class VortexCurve
    {
        private Vector3D[] _points;
        private Vector3D[] _velocity;
        private double _gamma;
        private double _dt = 0.0005;

        public VortexCurve(double gamma, int n, double radius, double y)
        {
            _gamma = gamma;
            Ring(radius, n, y);

        }

        private void Ring(double radius, int n, double yCoord)
        {
            _points = new Vector3D[n + 1];
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

        public void InVelocity(params VortexCurve[] vortexCurve)
        {
            for (int i = 0; i < _points.Length; i++)
            {

                _velocity[i] = new Vector3D(0);
                _velocity[i] += Velocity(_points[i]);

                foreach (var curve in vortexCurve)
                {
                    _velocity[i] += curve.Velocity(_points[i]);
                }


            }

        }

        public void Move()
        {
            for (int i = 0; i < _points.Length; i++)
            {
                _points[i] += _velocity[i] * _dt;
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
