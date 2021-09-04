using System.Drawing;

namespace ComputeShaderGravityGalaxies
{
    class Program
    {
        static void Main(string[] args)
        {
            // This line creates a new instance, and wraps the instance in a using statement so it's automatically disposed once we've exited the block.
            using (Game game = new Game(1368, 768, "Рух галактик"))
            {
                //Run takes a double, which is how many frames per second it should strive to reach.
                //You can leave that out and it'll just update as fast as the hardware will allow it.
                //game.ClientSize = new Size(512,512);
                game.Location = new Point(10, 10);
                game.Run(30.0);
            }
        }
    }
}
