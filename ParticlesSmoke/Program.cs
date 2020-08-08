using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParticlesSmoke;

namespace ParticlesSmoke
{
    class Program
    {
        static void Main(string[] args)
        {
            using (Game game = new Game(800, 800, "Fire"))
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
