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
    public class Character : Item
    {
        PPosition position;
        PVelocity velocity;
        PAngularPosition angularPosition;
        CustomCollider collider;

        Player player;
        public Player Player
        {
            get
            {
                return player;
            }
        }
        public float Speed;

        private int type;
        public int Type
        {
            get
            {
                return type;
            }
        }

        public int BombCount = 3;
        public int ExplosionPower = 3;

        bool dead = false;
        public bool IsDead
        {
            get
            {
                return dead;
            }
        }

        public bool HasWon = false;

        Vector2 move;

        List<Bomb> bombsInContact = new List<Bomb>();

        public Character(Player player)
        {
            this.player = player;
            this.type = (int)player.CharacterType;

            Speed = Bomberman.StartSpeed;

            Require<PPositionWithEvents>();
            Require<PAngularPosition>();
            Require<PBoundingSphere>().BoundingSphere = new BoundingSphere(Vector3.Zero, Bomberman.GridSize * 0.45f);
            Require<ForceUser>().Mass = 1000;
            Require<ParticleCollider>().ParticleRadius = Bomberman.GridSize * 0.45f;
            Require<PCoefficientOfRestitution>().CoefficientOfRestitution = 0;

            // Position
            position = Part<PPosition>();
            angularPosition = Part<PAngularPosition>();
            velocity = Part<PVelocity>();

            // Update
            Require<ItemProcess>().Process = update;

            // Custom collider
            collider = Require<CustomCollider>();
            collider.CollisionMethod += Collide;
            collider.OverrideCollisionWithType.Add(typeof(Character));
        }

        public void Reset()
        {
            dead = false;
            HasWon = false;
            Speed = Bomberman.StartSpeed;
        }

        public void OverrideNewBomb(Bomb bomb)
        {
            collider.OverrideCollisionWithItem.Add(bomb);
            bombsInContact.Add(bomb);
        }

        void update(float dt)
        {
            if (dead) return;

            // Remove bombs in override
            for (int i = 0; i < collider.OverrideCollisionWithItem.Count; i++)
            {
                if (!bombsInContact.Contains(collider.OverrideCollisionWithItem[i].As<Bomb>()))
                {
                    collider.OverrideCollisionWithItem.RemoveAt(i);
                    i--;
                }
            }
            bombsInContact.Clear();

            // Movement
            move.Y = -move.Y;

            if (move != Vector2.Zero)
            {
                float p = move.Length();
                move.Normalize();
                if (p > 1) p = 1;
                move *= p;
                //move.X = Math.Sign(move.X) * p;
                //move.Y = Math.Sign(move.Y) * p;

                //position.Position += new Vector3(m.X, 0, m.Y) * dt * Speed;
                velocity.Velocity = new Vector3(move.X, 0, move.Y) * Speed;

                float angle = (float)Math.Atan2(move.Y, move.X);
                angularPosition.AngularPosition = Quaternion.CreateFromAxisAngle(Vector3.Down, angle);
                move = Vector2.Zero;
            }

            if (active != activeLast)
            {
                activeLast = active;
            }
            else
            {
                if (active)
                {
                    active = false;
                }
            }
        }

        bool active;
        bool activeLast;

        public void Fire()
        {
            if (BombCount > 0 && collider.OverrideCollisionWithItem.Count == 0)
            {
                Vector3 r = position.Position;
                r.X = ((int)(r.X / Bomberman.GridSize) + 0.5f) * Bomberman.GridSize;
                r.Z = ((int)(r.Z / Bomberman.GridSize) + 0.5f) * Bomberman.GridSize;
                r.Y = Bomberman.GridSize;
                Bomb newBomb = new Bomb(r, this, Bomberman.BombFuseTime, ExplosionPower);
                Bomberman.Level.AddBomb(newBomb);
                BombCount--;
            }
            active = true;
        }

        public void Move(Vector2 m)
        {
            move += m;
        }

        void Collide(float elapsedSeconds, Item collidingItem, Vector3 impactPoint)
        {
            if (collidingItem.Is<Explosion>())
            {
                dead = true;
            }
            else if (collidingItem.Is<Bomb>())
            {
                Bomb bomb = collidingItem.As<Bomb>();
                if (!bombsInContact.Contains(bomb))
                {
                    bombsInContact.Add(bomb);
                }
            }
            else if (collidingItem.Is<Trigger>())
            {
                Trigger trigger = collidingItem.As<Trigger>();
                if (active || trigger.AutoActivate)
                {
                    trigger.Activate();
                }
            }
        }

    }
}
