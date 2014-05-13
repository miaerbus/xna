using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Artificial.XNATutorial.CarGame
{
    class Car : IPositionWithEvents, IWorldMatrix
    {
        // Direction
        private Vector3 direction;
        public Vector3 Direction
        {
            get
            {
                return direction;
            }
        }    

        // Position
        private Vector3 position;
        public Vector3 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
                updateWorldMatrix();
                RaiseOnPositionChanged();
            }
        }

        // OnPositionChanged
        public event EventHandler OnPositionChanged;
        private void RaiseOnPositionChanged()
        {
            EventHandler e = OnPositionChanged;
            if (e != null) e(this, null);
        }

        // Rotation
        private float rotation;
        public float Rotation
        {
            get
            {
                return rotation;
            }
            set
            {
                rotation = value;                
                updateWorldMatrix();
            }
        }

        // World matrix
        private Matrix worldMatrix = Matrix.Identity;
        public Matrix WorldMatrix
        {
            get
            {
                return worldMatrix;
            }
            set
            {
                worldMatrix = value;
            }
        }

        // Speed
        private float speed;
        public float Speed
        {
            get
            {
                return speed;
            }
            set
            {
                speed = value;
            }
        }

        // Update world matrix
        private void updateWorldMatrix()
        {
            worldMatrix = Matrix.CreateRotationZ(rotation) * Matrix.CreateTranslation(position);
            direction = WorldMatrix.Up;
        }

        // Constructor
        public Car()
        {
            updateWorldMatrix();
        }

        // Update
        public void Update(GameTime gameTime)
        {
            // Move car in its direction regarding its speed
            Position += direction * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}
