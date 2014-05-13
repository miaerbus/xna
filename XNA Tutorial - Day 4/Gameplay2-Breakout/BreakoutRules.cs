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
                if (Math.Abs(p.Position.Y) > 350)
                {
                    Breakout.Lives--;
                    b.Reset();
                }
            }
        }

        // GLOBAL RULES
        void GlobalRules(float dt)
        {
            if (Breakout.Lives < 0)
            {
                Breakout.Lives = Breakout.StartLives;
                Reset();
            }

            if (Breakout.BricksLeft <= 0)
            {
                Reset();
                Breakout.Lives++;
            }
        }       

        // LEVEL RESET
        public void Reset()
        {
            Breakout.Scene.Clear();
            Breakout.BricksLeft = 0;
            // Paddle
            Breakout.Paddle = new Paddle();
            Breakout.Scene.Add(Breakout.Paddle);
            // Ball
            Breakout.Scene.Add(new Ball());
            // Bounds
            Breakout.Scene.Add(new CollisionPlane(new Plane(Vector3.Down, -300)));
            Breakout.Scene.Add(new CollisionPlane(new Plane(Vector3.Right, -400)));
            Breakout.Scene.Add(new CollisionPlane(new Plane(Vector3.Left, -400)));     
            // Bricks
            for (int y = 0; y < 5; y++)
            {
                int numBricks = 16 + y % 2;
                for (int x = 0; x < numBricks; x++)
                {
                    float px = -400 + (x + ((float)((y + 1) % 2) * 0.5f)) * 50;
                    float py = 200 - y * 25;
                    Breakout.Scene.Add(new Brick(new Vector3(px, py, 0)));
                }
            }

        }

    }
}
