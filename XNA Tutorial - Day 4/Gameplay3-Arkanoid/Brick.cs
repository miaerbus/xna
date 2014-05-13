using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Artificial.XNATutorial;
using Artificial.XNATutorial.Physics;

namespace Artificial.XNATutorial.Breakout
{
    public class Brick : Item
    {
        int power;
        public int Power
        {
            get
            {
                return power;
            }
        }

        public Brick(Vector3 position, int power)
        {
            Require<PPositionWithEvents>();
            Require<PConvex>();
            Require<PBoundingSphere>().BoundingSphere = new BoundingSphere(Vector3.Zero, 30);
            Require<ConvexCollider>();
            Require<PowerupSpawner>();
            
            // Mass
            PMass m = this.Require<PMass>();
            m.Mass = float.PositiveInfinity;

            // Brick shape
            ConvexPolygon convex = new ConvexPolygon();
            convex.Points.Add(new Vector3(-25, -12, 0));
            convex.Points.Add(new Vector3(-25, 12, 0));
            convex.Points.Add(new Vector3(25, 12, 0));
            convex.Points.Add(new Vector3(25, -12, 0));
            convex.CalculatePlanes();
            As<PConvex>().Convex = convex;

            // Position
            As<PPosition>().Position = position;

            // Custom collision
            CustomCollider collider = Require<CustomCollider>();
            collider.CollisionMethod += Collision;

            this.power = power;

            Breakout.BricksLeft++;
        }

        // CUSTOM COLLISION
        void Collision(float elapsedSeconds, Item collidingItem, Vector3 impactPoint)
        {
            if (collidingItem.As<Ball>() != null)
            {
                // I've been hit!
                power--;
                if (power <= 0)
                {
                    Breakout.Scene.Remove(this);
                    Breakout.BricksLeft--;
                }
            }
        }
    }
}
