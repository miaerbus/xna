using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Artificial.XNATutorial;
using Artificial.XNATutorial.Physics;

namespace Artificial.XNATutorial.Breakout
{
    public class Paddle : Item
    {
        PPosition position;
        float speed = 500;
        bool mouseControl;
        float mouseZero;

        ConvexPolygon convex;
        float width;
        public float Width
        {
            get
            {
                return width;
            }
            set
            {
                width = value;
                convex.Points[0] = new Vector3(-width * 0.5f, convex.Points[0].Y, 0);
                convex.Points[1] = new Vector3(-width * 0.5f, convex.Points[1].Y, 0);
                convex.Points[2] = new Vector3(width * 0.5f, convex.Points[2].Y, 0);
                convex.Points[3] = new Vector3(width * 0.5f, convex.Points[3].Y, 0);
                convex.CalculatePlanes();
            }
        }

        public Paddle()
        {
            Require<PPositionWithEvents>();
            Require<PConvex>();
            Require<PBoundingSphere>().BoundingSphere = new BoundingSphere(Vector3.Zero, 65);
            Require<ConvexCollider>();
            
            // Mass
            PMass m = this.Require<PMass>();
            m.Mass = float.PositiveInfinity;

            // Paddle shape
            convex = new ConvexPolygon();
            convex.Points.Add(new Vector3(0, -12, 0));
            convex.Points.Add(new Vector3(0, 12, 0));
            convex.Points.Add(new Vector3(0, 12, 0));
            convex.Points.Add(new Vector3(0, -12, 0));
            As<PConvex>().Convex = convex;
            Width = 100;

            // Position
            position = As<PPosition>();
            position.Position = new Vector3(0, -250, 0);

            // Update
            ItemProcess process = Require<ItemProcess>();
            process.Process = update;

            // Custom collision
            CustomCollider collider = Require<CustomCollider>();
            collider.CollisionMethod += Collision;
        }

        void update(float dt)
        {
            // Mouse
            if (!mouseControl && Input.mouseState.X != Input.oldMouseState.X)
            {
                mouseControl = true;
                mouseZero = position.Position.X / 1.5f - Input.mouseState.X;
            }

            if (mouseControl)
            {
                float x = (mouseZero + Input.mouseState.X) * 1.5f;
                position.Position = new Vector3(x, position.Position.Y, 0);
            }

            // Gamepad                
            float speedup = 1;
            if (Input.gamePadState[0].Buttons.A == ButtonState.Pressed) speedup = 2;
            if (Input.gamePadState[0].ThumbSticks.Left.X != 0)
            {
                mouseControl = false;
            }
            position.Position += Input.gamePadState[0].ThumbSticks.Left.X * Vector3.Right * dt * speed * speedup;


            // Keyboard
            if (Input.keyState.IsKeyDown(Keys.LeftShift) || Input.keyState.IsKeyDown(Keys.LeftShift))
            {
                mouseControl = false;
                speedup = 2;
            }
            if (Input.keyState.IsKeyDown(Keys.Right))
            {
                mouseControl = false;
                position.Position += Vector3.Right * dt * speed * speedup;
            }
            if (Input.keyState.IsKeyDown(Keys.Left))
            {
                mouseControl = false;
                position.Position += Vector3.Left * dt * speed * speedup;
            }

            // Limit movement
            if (position.Position.X < -350)
            {
                position.Position = new Vector3(-350, position.Position.Y, 0);
            }
            if (position.Position.X > 350)
            {
                position.Position = new Vector3(350, position.Position.Y, 0);
            }
        }


        // CUSTOM COLLISION
        void Collision(float elapsedSeconds, Item collidingItem, Vector3 impactPoint)
        {
            Ball b = collidingItem.As<Ball>();
            if (b != null)
            {
                Particle p = b.As<Particle>();
                float speed = p.Velocity.Length() * Breakout.SpeedupOnHit;
                Vector3 direction = Vector3.Zero;
                if (speed > 0)
                {
                    // Rotate
                    direction = Vector3.Normalize(p.Velocity);
                    float offCenter = (impactPoint.X - position.Position.X) / width;
                    direction.X = offCenter * Breakout.MaxAngleOnHit;
                    direction.Normalize();
                }
                // Apply new velocity
                p.Velocity = direction * speed;
            }
        }
    }
}
