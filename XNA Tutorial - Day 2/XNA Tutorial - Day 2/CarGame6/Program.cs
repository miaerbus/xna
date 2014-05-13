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
            bool runEditor = true;

            if (runEditor)
            {
                using (Editor editor = new Editor())
                {
                    editor.Run();
                }
            }
            else
            {
                using (CarGame game = new CarGame())
                {
                    game.Run();
                }
            }
        }
    }
}

