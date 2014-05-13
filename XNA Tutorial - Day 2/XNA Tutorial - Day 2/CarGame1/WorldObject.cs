using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Artificial.XNATutorial.CarGame
{
    abstract class WorldObject
    {
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

        // Normal matrix
        private Matrix normalMatrix = Matrix.Identity;
        public Matrix NormalMatrix
        {
            get
            {
                return normalMatrix;
            }
            set
            {
                normalMatrix = value;
            }
        }

        // Every world object must be able to render itself
        abstract public void Render(Camera camera);

    }
}
