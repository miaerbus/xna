using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Artificial.XNATutorial;
using Artificial.XNATutorial.Physics;

namespace Artificial.XNATutorial.Breakout
{
    class PowerupSpawner : Part
    {
        public override void Install(Item parent)
        {
            base.Install(parent);

            // Custom collision
            CustomCollider collider = parent.Require<CustomCollider>();
            collider.CollisionMethod += Spawn;
        }

        // SPAWN
        void Spawn(float elapsedSeconds, Item collidingItem, Vector3 impactPoint)
        {
            if (collidingItem.As<Ball>() != null)
            {
                if (parent.As<Brick>().Power <= 1)
                {
                    // Brick was destroyed
                    if (Breakout.Random.NextDouble() < Breakout.PowerupChance)
                    {
                        // Spawn goodies!
                        Item powerup;
                        float choice = (float)Breakout.Random.NextDouble();
                        if (choice < 0.5f)
                        {
                            powerup = new Magnet(parent.As<PPosition>().Position, 5);
                        }
                        else
                        {
                            powerup = new Breakthrough(parent.As<PPosition>().Position, 10);
                        }
                        Breakout.Scene.Add(powerup);
                    }
                }
            }
        }
    }
}
