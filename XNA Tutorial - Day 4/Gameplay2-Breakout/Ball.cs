using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Artificial.XNATutorial;
using Artificial.XNATutorial.Physics;

namespace Artificial.XNATutorial.Breakout
{
    public class Ball : Item
    {
        Random rnd = new Random();
        float speed = 500;
        Particle particle;

        public Ball()
        {
            this.Require<PPositionWithEvents>();
            particle = this.Require<Particle>();
            PMass m = this.Require<PMass>();
            m.Mass = 1;
            particle.ParticleRadius = 10;
            Reset();
        }

        public void Reset()
        {
            particle.Position = Breakout.Paddle.As<PPosition>().Position + Vector3.Up * 300;
            particle.Velocity = Vector3.Down * speed + Vector3.Right * ((float)rnd.NextDouble() * 2 - 1) * speed * 0.2f;
        }
    }
}
