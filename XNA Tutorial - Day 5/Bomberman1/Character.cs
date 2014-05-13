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

        PlayerIndex player;
        public PlayerIndex Player
        {
            get
            {
                return player;
            }
        }
        public float Speed;

        private CharacterType type;
        public CharacterType Type
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

        List<Bomb> bombsInContact = new List<Bomb>();

        public Character(PlayerIndex player, CharacterType type)
        {
            this.player = player;
            this.type = type;

            Speed = Bomberman.StartSpeed;

            Require<PPositionWithEvents>();
            Require<PAngularPosition>();
            Require<PBoundingSphere>().BoundingSphere = new BoundingSphere(Vector3.Zero, Bomberman.GridSize * 0.5f);
            Require<ForceUser>().Mass = 1000;
            Require<ParticleCollider>().ParticleRadius = Bomberman.GridSize * 0.4f;
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
            Vector2 m = Input.gamePadState[(int)player].ThumbSticks.Left;
            m.Y = -m.Y;
            float p = m.Length();
            m.X = Math.Sign(m.X) * p;
            m.Y = Math.Sign(m.Y) * p;


            if (player == PlayerIndex.One)
            {
                if (Input.keyState.IsKeyDown(Keys.Right)) m += Vector2.UnitX;
                if (Input.keyState.IsKeyDown(Keys.Left)) m -= Vector2.UnitX;
                if (Input.keyState.IsKeyDown(Keys.Up)) m -= Vector2.UnitY;
                if (Input.keyState.IsKeyDown(Keys.Down)) m += Vector2.UnitY;
            }

            if (player == PlayerIndex.Two)
            {
                if (Input.keyState.IsKeyDown(Keys.D)) m += Vector2.UnitX;
                if (Input.keyState.IsKeyDown(Keys.A)) m -= Vector2.UnitX;
                if (Input.keyState.IsKeyDown(Keys.W)) m -= Vector2.UnitY;
                if (Input.keyState.IsKeyDown(Keys.S)) m += Vector2.UnitY;
            }

            if (m != Vector2.Zero)
            {
                //position.Position += new Vector3(m.X, 0, m.Y) * dt * Speed;
                velocity.Velocity = new Vector3(m.X, 0, m.Y) * Speed;

                float angle = (float)Math.Atan2(m.Y, m.X);
                angularPosition.AngularPosition = Quaternion.CreateFromAxisAngle(Vector3.Down, angle);
            }

            // Fire
            if (Input.WasKeyPressed(Keys.RightControl) && player == PlayerIndex.One ||
                Input.WasKeyPressed(Keys.LeftControl) && player == PlayerIndex.Two ||
                Input.gamePadState[(int)player].Buttons.A == ButtonState.Pressed)
            {
                if (BombCount > 0 && collider.OverrideCollisionWithItem.Count==0)
                {
                    Vector3 r = position.Position;
                    r.X = ((int)(r.X / Bomberman.GridSize) + 0.5f) * Bomberman.GridSize;
                    r.Z = ((int)(r.Z / Bomberman.GridSize) + 0.5f) * Bomberman.GridSize;
                    r.Y = Bomberman.GridSize;
                    Bomb newBomb = new Bomb(r, this, Bomberman.BombFuseTime, ExplosionPower);
                    Bomberman.CurrentLevel.AddBomb(newBomb);
                    BombCount--;
                }
            }
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
        }

    }
}
