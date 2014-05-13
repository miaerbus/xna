using System;

namespace Artificial.XNATutorial.CarGame
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (CarGame game = new CarGame())
            {
                game.Run();
            }
        }
    }
}

