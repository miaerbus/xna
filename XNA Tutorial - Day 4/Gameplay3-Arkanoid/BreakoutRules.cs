using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Artificial.XNATutorial;
using Artificial.XNATutorial.Physics;

namespace Artificial.XNATutorial.Breakout
{
    public class BreakoutRules : Part
    {
        int itemUpdateOrder;
        int globalUpdateOrder;

        public BreakoutRules(int itemUpdateOrder, int globalUpdateOrder)
        {
            this.itemUpdateOrder = itemUpdateOrder;
            this.globalUpdateOrder = globalUpdateOrder;
        }

        public override void Install(Item parent)
        {
            base.Install(parent);
            parent.As<ISceneIterator>().RegisterUpdateItemMethod(itemUpdateOrder, ItemRules);
            parent.As<Simulator>().RegisterUpdateMethod(globalUpdateOrder, GlobalRules);
        }

        // ITEM RULES
        void ItemRules(float dt, Item item)
        {
            // Ball
            Ball b = item.As<Ball>();
            if (b != null)
            {
                Particle p = b.As<Particle>();
                if (p.Position.Y < -320)
                {
                    Breakout.Scene.Remove(b);
                    Breakout.Balls.Remove(b);
                    if (Breakout.Balls.Count == 0)
                    {
                        LifeLost();
                    }                    
                }
            }
        }

        // GLOBAL RULES
        void GlobalRules(float dt)
        {
            if (Breakout.Lives < 0)
            {
                Breakout.Lives = Breakout.StartLives;
                Breakout.Level = 1;
                Reset();
            }

            if (Breakout.BricksLeft <= 0)
            {
                Breakout.Lives++;
                Breakout.Level++;
                Reset();
            }
        }       

        // LIFE LOST
        void LifeLost()
        {
            Breakout.Lives--;
            // Create new paddle
            Vector3 position = Breakout.Paddle.As<PPosition>().Position;
            Breakout.Scene.Remove(Breakout.Paddle);
            Breakout.Paddle = new Paddle();
            Breakout.Paddle.As<PPosition>().Position = position;
            Breakout.Scene.Add(Breakout.Paddle);
            // Add magnet
            new Magnet(Vector3.Zero, 1).Activate(Breakout.Paddle);
            // Create new ball
            Ball ball = new Ball();
            Breakout.Balls.Add(ball);
            Breakout.Scene.Add(ball);
        }

        // LEVEL RESET
        public void Reset()
        {
            Breakout.Scene.Clear();
            Breakout.BricksLeft = 0;
            Breakout.Balls.Clear();
            // Paddle
            Breakout.Paddle = new Paddle();
            Breakout.Scene.Add(Breakout.Paddle);
            new Magnet(Vector3.Zero, 1).Activate(Breakout.Paddle);
            // Ball
            Ball ball = new Ball();
            Breakout.Balls.Add(ball);
            Breakout.Scene.Add(ball);
            // Bounds
            Breakout.Scene.Add(new CollisionPlane(new Plane(Vector3.Down, -300)));
            Breakout.Scene.Add(new CollisionPlane(new Plane(Vector3.Right, -400)));
            Breakout.Scene.Add(new CollisionPlane(new Plane(Vector3.Left, -400)));     
            // Bricks
            LoadLevel(Breakout.Level);
        }

        void LoadLevel(int level)
        {
            if (level == 1)
            {
                for (int y = 0; y < 6; y++)
                {
                    int numBricks = 16;
                    for (int x = 0; x < numBricks; x++)
                    {
                        float px = -375 + x * 50;
                        float py = 200 - y * 25;
                        Breakout.Scene.Add(new Brick(new Vector3(px, py, 0), y == 0 ? 3 : 1));
                    }
                }
            }
            else
            {
                for (int y = 0; y < 16; y++)
                {
                    int numBricks = y + 1;
                    for (int x = 0; x < numBricks; x++)
                    {
                        float px = -375 + x * 50;
                        float py = 275 - y * 25;
                        Breakout.Scene.Add(new Brick(new Vector3(px, py, 0), y == 15 ? 3 : 1));
                    }
                }
            }
        }

    }
}
