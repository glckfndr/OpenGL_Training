using System;

namespace ComputeShaderTwoVortexRing
{
    internal struct Vector3D
    {
        public bool Equals(Vector3D other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);
        }

        public override bool Equals(object obj)
        {
            return obj is Vector3D other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                hashCode = (hashCode * 397) ^ Z.GetHashCode();
                return hashCode;
            }
        }

        public double X;
        public double Y;
        public double Z;


        public Vector3D(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vector3D(double val)
        {
            X = val;
            Y = val;
            Z = val;
        }

        public Vector3D(Vector3D r) : this(r.X, r.Y, r.Z)
        {

        }

        public Vector3D Cross(Vector3D b)
        {
            return new Vector3D(Y * b.Z - Z * b.Y, Z * b.X - X * b.Z, X * b.Y - Y * b.X);
        }

        public double Abs()
        {
            return Math.Sqrt(X * X + Y * Y + Z * Z);
        }

        public static Vector3D operator +(Vector3D a, Vector3D b)
        {
            return new Vector3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Vector3D operator -(Vector3D a, Vector3D b)
        {
            return new Vector3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static double operator *(Vector3D a, Vector3D b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }

        public static Vector3D operator *(Vector3D a, double b)
        {
            return new Vector3D(a.X * b, a.Y * b, a.Z * b);
        }

        public static Vector3D operator *(double b, Vector3D a)
        {
            return new Vector3D(a.X * b, a.Y * b, a.Z * b);
        }

        public static Vector3D operator /(Vector3D a, double b)
        {
            return new Vector3D(a.X / b, a.Y / b, a.Z / b);
        }
        
        public static bool operator ==(Vector3D a, double b)
        {
            return a.X == b && a.Y == b && a.Z == b;
        }

        public static bool operator !=(Vector3D a, double b)
        {
            return !(a == b);
        }


    }
}
