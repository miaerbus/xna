using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Artificial.XNATutorial;

namespace Artificial.XNATutorial.Physics
{
    public interface IForceUser : IMass, IAccumulatedForce, IAcceleration, IMovable { }
    public class ForceUser : Part, IForceUser
    {
        public override void Install(Item parent)
        {
            base.Install(parent);
            parent.Require<PMass>();
            parent.Require<PAccumulatedForce>();
            parent.Require<PAcceleration>();
            parent.Require<Movable>();

            position = parent.As<PPosition>();
            velocity = parent.As<PVelocity>();
            acceleration = parent.As<PAcceleration>();
            mass = parent.As<PMass>();
            accumulatedForce = parent.As<PAccumulatedForce>();
        }

        #region Properties

        PMass mass;
        public float Mass
        {
            get
            {
                return mass.Mass;
            }
            set
            {
                mass.Mass = value;
            }
        }

        PAccumulatedForce accumulatedForce;
        public Vector3 AccumulatedForce
        {
            get
            {
                return accumulatedForce.AccumulatedForce;
            }
            set
            {
                accumulatedForce.AccumulatedForce = value;
            }
        }

        PAcceleration acceleration;
        public Vector3 Acceleration
        {
            get
            {
                return acceleration.Acceleration;
            }
            set
            {
                acceleration.Acceleration = value;
            }
        }

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
