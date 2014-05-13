using System;

namespace Artificial.XNATutorial.Breakout
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Breakout game = new Breakout())
            {
                game.Run();
            }
        }
    }
}

