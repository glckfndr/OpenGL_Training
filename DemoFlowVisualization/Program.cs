using System.Drawing;

namespace DemoFlowVisualization
{
    internal class Program
    {
        private static void Main(string[] args)
        {

            //using (Game game = new Game(1024, 768, "Рух хмари точкових вихорів"))
            using (Game game = new Game(1024, 768, "Обтікання тіл, що погано обтікаються, та рух вихорового шару"))
            {
                game.Location = new Point(600, 10);
                game.Run(20.0);
            }

        }
    }
}
