using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Artificial.XNATutorial;
using Artificial.XNATutorial.Physics;

namespace Artificial.XNATutorial.Breakout
{
    class Magnet : Part, IPowerup
    {
        int power;
        public int Power
        {
            get
            {
                return power;
            }
        }

        struct ballInfo
        {
            public float ballOffset;
            public Vector3 velocity;
        }
        Dictionary<Ball, ballInfo> holdedBalls = new Dictionary<Ball, ballInfo>();
        IEnumerator<KeyValuePair<Ball, ballInfo>> enumerator;

        public Magnet(Vector3 position, int power)
        {
            enumerator = holdedBalls.GetEnumerator();

            Require<Powerup>();

            // Position
            As<PPosition>().Position = position;
            
            this.power = power;
        }

        // INSTALL INTO PADDLE
        public void Activate(Paddle paddle)
        {
            Magnet magnet = paddle.As<Magnet>();
            if (magnet != null)
            {
                magnet.power += power;
            }
            else
            {
                paddle.Aggregate(this);
                parent.As<CustomCollider>().CollisionMethod += MagnetCollision;
                parent.As<ItemProcess>().Process += UpdateBallPosition;
            }
        }

        // UNINSTALL FROM PADDLE
        public void Deactivate()
        {
            parent.As<CustomCollider>().CollisionMethod -= MagnetCollision;
            parent.As<ItemProcess>().Process -= UpdateBallPosition;
            parent.Remove(this);
        }

        // CUSTOM PADDLE COLLISION
        void MagnetCollision(float elapsedSeconds, Item collidingItem, Vector3 impactPoint)
        {
            Ball b = collidingItem.As<Ball>();
            if (b != null)
            {
                Particle p = b.As<Particle>();
                Vector3 paddlePosition = parent.As<PPosition>().Position;
                if (!holdedBalls.ContainsKey(b))
                {
                    ballInfo info;
                    info.ballOffset = paddlePosition.X - p.Position.X;
                    info.velocity = p.Velocity;
                    holdedBalls.Add(b, info);
                    enumerator = holdedBalls.GetEnumerator();
                    p.Velocity = Vector3.Zero;
                }
            }
        }

        // CUSTOM PADDLE UPDATE
        void UpdateBallPosition(float dt)
        {
            bool release = Input.mouseState.LeftButton == ButtonState.Pressed;
            if (Input.keyState.IsKeyDown(Keys.Space)) release=true;
            if (Input.gamePadState[0].Buttons.B == ButtonState.Pressed) release = true;

            Vector3 paddlePosition = parent.As<PPosition>().Position;
            enumerator.Reset();
            enumerator.MoveNext();
            while (enumerator.Current.Key != null)
            {
                Ball b = enumerator.Current.Key;
                ballInfo info = enumerator.Current.Value;
                Particle p = b.As<Particle>();
                if (release)
                {
                    p.Velocity = info.velocity;
                }
                else
                {
                    p.Position = new Vector3(paddlePosition.X - info.ballOffset, p.Position.Y - 10, 0);
                }
                enumerator.MoveNext();
            }
            if (release && holdedBalls.Count>0)
            {
                holdedBalls.Clear();
                enumerator = holdedBalls.GetEnumerator();
                power--;
                if (power <= 0)
                {
                    Deactivate();
                }
            }
        }
    }
}
