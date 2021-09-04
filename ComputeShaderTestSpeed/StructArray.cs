namespace ComputeShaderTestSpeed
{
    internal class StructArray<T1, T2>
    {
        private T1 _vortexData;
        private T2 _outData;
        public StructArray(T1 vortexData, T2 outData)
        {
            _vortexData = vortexData;
            _outData = outData;
        }

        public void Compute()
        {

        }
    }
}
