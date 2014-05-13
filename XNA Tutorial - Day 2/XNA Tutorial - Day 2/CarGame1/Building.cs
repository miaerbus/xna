using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Artificial.XNATutorial.CarGame
{
    class Building : Object3D, IPosition
    {
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

        // Update world matrix
        private void updateWorldMatrix()
        {
            WorldMatrix = Matrix.CreateRotationZ(rotation) * Matrix.CreateTranslation(position);
        }   

    }
}
