﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGL_Training
{
    class Program
    {
        static void Main(string[] args)
        {
            using (Game game = new Game(600, 600, "LearnOpenTK. Press  r - rectangle, t - triangle"))
            {
                //Run takes a double, which is how many frames per second it should strive to reach.
                //You can leave that out and it'll just update as fast as the hardware will allow it.
                game.Run(60.0);
            }
        }
    }
}
