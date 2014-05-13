using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Artificial.XNATutorial;
using Artificial.XNATutorial.Physics;

namespace Artificial.XNATutorial.Breakout
{
    public class Powerup : Part
    {
        public override void Install(Item parent)
        {
            base.Install(parent);
            parent.Require<PPositionWithEvents>();
            parent.Require<Movable>();
            parent.Require<ParticleCollider>();

            parent.As<ParticleCollider>().ParticleRadius = 20;

            // Velocity
            parent.As<PVelocity>().Velocity = Vector3.Down * Breakout.PowerupFallSpeed;

            // Custom collision
            CustomCollider collider = parent.Require<CustomCollider>();
            collider.CollisionMethod += PowerupCollision;
            collider.OverrideCollision.Add(typeof(Brick));
            collider.OverrideCollision.Add(typeof(Ball));
        }

        // POWERUP COLLISION
        void PowerupCollision(float elapsedSeconds, Item collidingItem, Vector3 impactPoint)
        {
            // I've been hit!
            Paddle paddle = collidingItem.As<Paddle>();
            if (paddle != null)
            {
                // Install powerup into paddle
                parent.As<IPowerup>().Activate(paddle);
                Breakout.Scene.Remove(parent);
            }
        }        
    }

    public interface IPowerup
    {
        // ACTIVATE
        void Activate(Paddle paddle);
    }
}
