using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Artificial.XNATutorial;
using Artificial.XNATutorial.Physics;

namespace Artificial.XNATutorial.Bomberman
{
    public class Explosion : Item
    {
        Vector3 direction;
        public Vector3 Direction
        {
            get
            {
                return direction;
            }
        }

        int power;

        int frame = 0;
        public int Frame
        {
            get
            {
                return frame;
            }
        }

        public Explosion(Vector3 position, Vector3 direction, int power)
        {
            this.direction = direction;
            this.power = power;

            Require<PPositionWithEvents>().Position = position;
            Require<PBoundingSphere>().BoundingSphere = new BoundingSphere(Vector3.Zero, Bomberman.GridSize * 0.25f);
            Require<ParticleCollider>().ParticleRadius = Bomberman.GridSize * 0.24f;

            // Update
            Require<ItemProcess>().Process = update;

            CustomCollider collider = Require<CustomCollider>();
            collider.OverrideCollisionWithType.Add(typeof(Explosion));
            collider.OverrideCollisionWithType.Add(typeof(Bomb));
            collider.OverrideCollisionWithType.Add(typeof(Building));
            collider.OverrideCollisionWithType.Add(typeof(Character));
            collider.CollisionMethod += Collide;
        }

        void update(float dt)
        {
            frame++;
            if (frame == 2)
            {
                if (power > 0)
                {
                    Vector3 p = Part<PPosition>().Position;
                    if (direction == Vector3.Zero)
                    {
                        Bomberman.Level.Scene.Add(new Explosion(p + Vector3.Left * Bomberman.GridSize, Vector3.Left, power - 1));
                        Bomberman.Level.Scene.Add(new Explosion(p + Vector3.Right * Bomberman.GridSize, Vector3.Right, power - 1));
                        Bomberman.Level.Scene.Add(new Explosion(p + Vector3.Forward * Bomberman.GridSize, Vector3.Forward, power - 1));
                        Bomberman.Level.Scene.Add(new Explosion(p + Vector3.Backward * Bomberman.GridSize, Vector3.Backward, power - 1));
                    }
                    else
                    {
                        Bomberman.Level.Scene.Add(new Explosion(p + direction * Bomberman.GridSize, direction, power - 1));
                    }
                }
            }
            else if (frame > 10)
            {
                Bomberman.Level.Scene.Remove(this);
            }
        }

        void Collide(float elapsedSeconds, Item collidingItem, Vector3 impactPoint)
        {
            if (collidingItem.Is<Building>() || collidingItem.Is<Bomb>())
            {
                Bomberman.Level.Scene.Remove(this);
            }
        }
    }
}
