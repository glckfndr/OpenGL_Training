namespace ComputeShaderGetData
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            using (var game = new Game(800, 800, "Test get from GPU"))
            {
                game.Run(60);
            }
        }
    }
}
