using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Artificial.XNATutorial;
using Artificial.XNATutorial.Physics;

namespace Artificial.XNATutorial.Pong
{
    class Ball : Item
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
            do
            {
                particle.Position = Vector3.Zero;
                particle.Velocity = Vector3.Transform(Vector3.Up, Matrix.CreateRotationZ((float)(rnd.NextDouble() * Math.PI * 2))) * speed;
            } while (Math.Abs(particle.Velocity.X) < 100);
        }
    }
}
