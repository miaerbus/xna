using System;

namespace Artificial.XNATutorial.Bomberman
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Bomberman game = new Bomberman())
            {
                game.Run();
            }
        }
    }
}

