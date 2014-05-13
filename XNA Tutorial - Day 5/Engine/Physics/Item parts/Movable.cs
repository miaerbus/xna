using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Artificial.XNATutorial;

namespace Artificial.XNATutorial.Physics
{
    public interface IMovable : IPosition, IVelocity { }
    public class Movable : Part, IMovable
    {
        public override void Install(Item parent)
        {
            base.Install(parent);
            parent.Require<PPosition>();
            parent.Require<PVelocity>();

            position = parent.As<PPosition>();
            velocity = parent.As<PVelocity>();
        }

        #region Properties

        PPosition position;
        public Vector3 Position
        {
            get
            {
                return position.Position;
            }
            set
            {
                position.Position = value;
            }
        }

        PVelocity velocity;
        public Vector3 Velocity
        {
            get
            {
                return velocity.Velocity;
            }
            set
            {
                velocity.Velocity = value;
            }
        }

        #endregion
    }
}
