using System;
using Artificial.XNATutorial.Basics;
using Microsoft.Xna.Framework;

namespace Artificial.XNATutorial.Basics
{
    static class Program
    {
        public static int NextGame = 0;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            while (NextGame != -1)
            {
                System.Threading.Thread t1 = new System.Threading.Thread(new System.Threading.ThreadStart(delegate()
                {
                    BasicsMenu menu = new BasicsMenu();
                    menu.Run();
                    menu.Dispose();
                }));
                t1.Start();

                while (t1.IsAlive)
                {
                    System.Threading.Thread.Sleep(100);
                }

                System.Threading.Thread t2 = new System.Threading.Thread(new System.Threading.ThreadStart(delegate()
                {
                    Game game = null;
                    if (NextGame != -1)
                    {
                        switch (NextGame)
                        {
                            case 1:
                                game = new Basics1();
                                break;
                            case 2:
                                game = new Basics2();
                                break;
                            case 3:
                                game = new Basics3();
                                break;
                            case 4:
                                game = new Basics4();
                                break;
                            case 5:
                                game = new Basics5();
                                break;
                            case 6:
                                game = new Basics6();
                                break;
                            case 7:
                                game = new Basics7();
                                break;
                        }
                        game.Run();
                        game.Dispose();
                    }
                }));
                t2.Start();

                while (t2.IsAlive)
                {
                    System.Threading.Thread.Sleep(100);
                }
            }

        }

    }
}

