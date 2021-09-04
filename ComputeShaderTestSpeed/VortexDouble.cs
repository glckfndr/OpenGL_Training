using System.Windows;

namespace ComputeShaderTestSpeed
{
    public struct VortexDouble
    {
        public Vector r;
        public double gamma;
        public double radius;

        public VortexDouble(Vector r, double gamma, double radius) : this()
        {
            this.r = r;
            this.gamma = gamma;
            this.radius = radius;
        }
    }
}
