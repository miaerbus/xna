using System;

namespace Artificial.XNATutorial.Physics
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (PhysicsDemo game = new PhysicsDemo())
            {
                game.Run();
            }
        }
    }
}

