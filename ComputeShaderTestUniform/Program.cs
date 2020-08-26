using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputeShaderTestUniform
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var game = new Game(800, 800, "Test GPU speed"))
            {
                game.Run(60);
            }
        }
    }
}
