namespace ComputeShaderSLAE
{
    public struct SquareMatrix
    {
        public float[] _a;
        private int _rowLength;
        private int _numberOfRows;
        public SquareMatrix(int row, int col)
        {
            _rowLength = col;
            _numberOfRows = row;
            _a = new float[_rowLength * _numberOfRows];

        }

        public SquareMatrix(float[,] m)
        {
            _rowLength = m.GetLength(1);
            _numberOfRows = m.GetLength(0);
            _a = new float[_rowLength * _numberOfRows];
            for (int i = 0; i < _numberOfRows; i++)
            {
                for (int j = 0; j < _rowLength; j++)
                {
                    _a[i * _rowLength + j] = m[i, j];
                }
            }

        }
        public void Set(float val, int i, int j)
        {
            _a[_rowLength * i + j] = val;
        }

        public float Get(int i, int j)
        {
            return _a[_rowLength * i + j];
        }

        public float[] GetData()
        {
            return _a;
        }

    }

}

