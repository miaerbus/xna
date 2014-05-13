using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Artificial.XNATutorial.CarGame
{
    class Car : Object2D, IPosition
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
            }
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
            WorldMatrix = Matrix.CreateRotationZ(rotation) * Matrix.CreateTranslation(position);
            direction = WorldMatrix.Up;
        }

        // Constructor
        public Car(SpriteBatch spriteBatch) : base(spriteBatch)
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
