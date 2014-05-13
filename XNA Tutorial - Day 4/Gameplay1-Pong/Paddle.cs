using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Artificial.XNATutorial;
using Artificial.XNATutorial.Physics;

namespace Artificial.XNATutorial.Pong
{
    class Paddle : Item
    {
        int player;
        PPosition position;
        float speed = 500;
        bool mouseControl;
        float mouseZero;

        public Paddle(PlayerIndex player)
        {
            this.player = (int)player;

            Require<PPositionWithEvents>();
            Require<PConvex>();
            Require<PBoundingSphere>().BoundingSphere = new BoundingSphere(Vector3.Zero, 60);
            Require<ConvexCollider>();
            
            // Mass
            PMass m = this.Require<PMass>();
            m.Mass = float.PositiveInfinity;

            // Paddle shape
            ConvexPolygon convex = new ConvexPolygon();
            convex.Points.Add(new Vector3(-10, -50, 0));
            convex.Points.Add(new Vector3(-10, 50, 0));
            convex.Points.Add(new Vector3(10, 50, 0));
            convex.Points.Add(new Vector3(10, -50, 0));
            convex.CalculatePlanes();
            As<PConvex>().Convex = convex;

            // Position
            position = As<PPosition>();
            if (player == PlayerIndex.One)
            {
                position.Position = new Vector3(-350, 0, 0);
            }
            else
            {
                position.Position = new Vector3(350, 0, 0);
            }

            // Update
            ItemProcess process = Require<ItemProcess>();
            process.Process = update;

            // Custom collision
            CustomCollider collider = Require<CustomCollider>();
            collider.CollisionMethod += Collision;
        }

        void update(float dt)
        {
            // Gamepad                
            position.Position += Input.gamePadState[player].ThumbSticks.Left.Y * Vector3.Up * dt * speed;

            if (player == 0)
            {
                // Keyboard
                if (Input.keyState.IsKeyDown(Keys.Q))
                {
                    position.Position += Vector3.Up * dt * speed;
                }
                if (Input.keyState.IsKeyDown(Keys.A))
                {
                    position.Position += Vector3.Down * dt * speed;
                }
            }
            else
            {
                // Mouse
                if (!mouseControl && Input.mouseState.Y != Input.oldMouseState.Y)
                {
                    mouseControl = true;
                    mouseZero = position.Position.Y / 1.5f + Input.mouseState.Y;
                }

                if (mouseControl)
                {
                    float y = (mouseZero - Input.mouseState.Y) * 1.5f;
                    if (Math.Abs(y - position.Position.Y) > dt * speed)
                    {
                        y = position.Position.Y + Math.Sign(y - position.Position.Y) * dt * speed;
                    }
                    position.Position = new Vector3(position.Position.X, y, 0);
                }

                // Keyboard
                if (Input.keyState.IsKeyDown(Keys.Up))
                {
                    mouseControl = false;
                    position.Position += Vector3.Up * dt * speed;
                }
                if (Input.keyState.IsKeyDown(Keys.Down))
                {
                    mouseControl = false;
                    position.Position += Vector3.Down * dt * speed;
                }
            }


            // Limit movement
            if (position.Position.Y < -200)
            {
                position.Position = new Vector3(position.Position.X, -200, 0);
            }
            if (position.Position.Y > 200)
            {
                position.Position = new Vector3(position.Position.X, 200, 0);
            }
        }

        // CUSTOM COLLISION
        void Collision(float elapsedSeconds, Item collidingItem, Vector3 impactPoint)
        {
            Ball b = collidingItem.As<Ball>();
            if (b != null)
            {
                Particle p = b.As<Particle>();
                // Speed up
                float speed = p.Velocity.Length() * Pong.SpeedupOnHit;
                // Rotate
                Vector3 direction = Vector3.Normalize(p.Velocity);
                float offCenter = (impactPoint.Y - position.Position.Y) * 0.01f;
                direction.Y += offCenter * Pong.RotateFactorOnHit;
                if (Math.Abs(direction.X) < 0.3f) direction.X = Math.Sign(direction.X) * 0.3f;
                direction.Normalize();
                // Apply new velocity
                p.Velocity = direction * speed;
            }
        }
    }
}
