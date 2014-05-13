using System;

namespace Artificial.XNATutorial.Basics
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (BasicGame game = new BasicGame())
            {
                game.Run();
            }
        }
    }
}

