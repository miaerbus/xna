using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Artificial.XNATutorial;
using Artificial.XNATutorial.Physics;

namespace Artificial.XNATutorial.Breakout
{
    class Breakthrough : Part, IPowerup
    {
        float timeLeft;

        public Breakthrough(Vector3 position, float timeLeft)
        {
            Require<Powerup>();

            // Position
            As<PPosition>().Position = position;

            this.timeLeft = timeLeft;
        }

        // INSTALL INTO BALLS
        public void Activate(Paddle paddle)
        {
            for (int i = 0; i < Breakout.Balls.Count; i++)
            {
                Ball b = Breakout.Balls[i];
                Breakthrough breakthrough = b.As<Breakthrough>();
                if (breakthrough != null)
                {
                    breakthrough.timeLeft += timeLeft;
                }
                else
                {
                    breakthrough = new Breakthrough(Vector3.Zero, timeLeft);
                    b.Aggregate(breakthrough);                    
                    breakthrough.parent.Require<CustomCollider>().OverrideCollision.Add(typeof(Brick));
                    breakthrough.parent.Require<ItemProcess>().Process += breakthrough.UpdateTime;
                }
            }            
        }

        // CUSTOM PADDLE UPDATE
        void UpdateTime(float dt)
        {
            timeLeft -= dt;
            if (timeLeft < 0)
            {
                parent.As<CustomCollider>().OverrideCollision.Remove(typeof(Brick));
                parent.As<ItemProcess>().Process -= UpdateTime;
                parent.Remove(this);
            }
        }
    }
}
