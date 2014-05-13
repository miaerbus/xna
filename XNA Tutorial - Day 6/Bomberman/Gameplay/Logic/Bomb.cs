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
    public class Bomb : Item
    {
        Character owner;
        float timeLeft;
        int power;
        Vector3 lastPosition;

        public Bomb(Vector3 position, Character owner, float timeLeft, int power)
        {
            this.owner = owner;
            this.timeLeft = timeLeft;
            this.power = power;

            Require<PPositionWithEvents>().Position = position;
            Require<PAngularPosition>();
            Require<PBoundingSphere>().BoundingSphere = new BoundingSphere(Vector3.Zero, Bomberman.GridSize * 0.45f);
            Require<ForceUser>().Mass = 100000;
            Require<ParticleCollider>().ParticleRadius = Bomberman.GridSize * 0.45f;
            Require<PCoefficientOfRestitution>().CoefficientOfRestitution = 0.2f;

            // Update
            Require<ItemProcess>().Process = update;
            lastPosition = position;

            // Collider
            CustomCollider collider = Require<CustomCollider>();
            collider.CollisionMethod += Collide;
        }


        void update(float dt)
        {
            timeLeft -= dt;
            if (timeLeft < 0)
            {
                Bomberman.Level.Scene.Remove(this);
                owner.BombCount++;
                Bomberman.Level.Scene.Add(new Explosion(Part<PPosition>().Position, Vector3.Zero, power));
                Bomberman.SoundBank.PlayCue("Explosion");
            }
            if (Part<PPosition>().Position.Y < Bomberman.GridSize * 0.5f && lastPosition.Y > Bomberman.GridSize * 0.5f)
            {
                Bomberman.SoundBank.PlayCue("BombDrop");
            }
            lastPosition = Part<PPosition>().Position;
        }

        void Collide(float elapsedSeconds, Item collidingItem, Vector3 impactPoint)
        {
            if (collidingItem.Is<Explosion>())
            {
                timeLeft = 0;
            }
        }
    }
}
