using OpenTK;

namespace ComputeShaderTestSpeed
{
    internal struct Vortex
    {
        public Vector2 r;
        public float gamma;
        public float radius;
       // private Vector3 _dummy0;
       // private float _dummy1;

        public Vortex(Vector2 r, float gamma, float radius) : this()
        {
            this.r = r;
            this.gamma = gamma;
            this.radius = radius;
           // _dummy0 = Vector3.Zero;
           // _dummy1 = 0.5f;
        }

    }
}
