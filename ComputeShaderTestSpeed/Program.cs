using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputeShaderTestSpeed
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var game = new Game(800, 800, "Test GPU speed on 2D Vortex pair to pair interaction"))
            {
                game.Run(60);
            }
        }
    }
}
