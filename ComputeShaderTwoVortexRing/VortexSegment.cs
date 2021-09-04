using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputeShaderTwoVortexRing
{
    class VortexSegment
    {
        public Vector3D Start;
        public Vector3D End;
        private Vector3D _dl;
        private static double koef = 1.0 / (4 * Math.PI);
        public double Gamma;
        public Vector3D InVelocity;

        public VortexSegment(Vector3D start, Vector3D end, double gamma)
        {
            Start = start;
            End = end;
            _dl = End - Start;
            Gamma = gamma;
            InVelocity = new Vector3D(0);
        }

        public Vector3D Velocity(Vector3D r)
        {
            return Velocity(r, Start, _dl, Gamma);
        }

        /// <summary>
        /// Швидкість від вихрового відрізку
        /// </summary>
        /// <param name="r">координати точки де визначається швидкість </param>
        /// <param name="start">початок відрізку</param>
        /// <param name="dl"></param>
        /// <param name="gamma">циркуляція</param>
        /// <returns></returns>
        public static Vector3D Velocity(Vector3D r, Vector3D start, Vector3D dl, double gamma)
        {
            var a = r - start;
            var b = a - dl;
            var cross = dl.Cross(a);
            if (cross == 0.0)
                return new Vector3D();
            //var distance = r_start.Abs();
            var c = cross.Abs();
            return gamma * cross * koef / (c * c) * (dl * a / a.Abs() - dl * b / b.Abs());
        }
    }
}
