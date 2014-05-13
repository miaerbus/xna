using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Artificial.XNATutorial;

namespace Artificial.XNATutorial.Physics
{
    public class TorqueUser : Part
    {
        public override void Install(Item parent)
        {
            base.Install(parent);
            parent.Require<PAngularMass>();
            parent.Require<PAccumulatedTorque>();
            parent.Require<PAngularMomentum>();
            parent.Require<Rotatable>();

            rotation = parent.As<PRotationMatrix>();
            angularPosition = parent.As<PAngularPosition>();
            angularVelocity = parent.As<PAngularVelocity>();
            angularMass = parent.As<PAngularMass>();
            accumulatedTorque = parent.As<PAccumulatedTorque>();
            angularMomentum = parent.As<PAngularMomentum>();
        }

        #region Properties

        PAngularMass angularMass;
        public Matrix AngularMass
        {
            get
            {
                return angularMass.AngularMass;
            }
            set
            {
                angularMass.AngularMass = value;
            }
        }

        PAccumulatedTorque accumulatedTorque;
        public Vector3 AccumulatedTorque
        {
            get
            {
                return accumulatedTorque.AccumulatedTorque;
            }
            set
            {
                accumulatedTorque.AccumulatedTorque = value;
            }
        }

        PRotationMatrix rotation;
        public Matrix Rotation
        {
            get
            {
                return rotation.Rotation;
            }
            set
            {
                rotation.Rotation = value;
            }
        }

        PAngularPosition angularPosition;
        public Quaternion AngularPosition
        {
            get
            {
                return angularPosition.AngularPosition;
            }
            set
            {
                angularPosition.AngularPosition = value;
            }
        }

        PAngularVelocity angularVelocity;
        public Vector3 AngularVelocity
        {
            get
            {
                return angularVelocity.AngularVelocity;
            }
            set
            {
                angularVelocity.AngularVelocity = value;
            }
        }

        PAngularMomentum angularMomentum;
        public Vector3 AngularMomentum
        {
            get
            {
                return angularMomentum.AngularMomentum;
            }
            set
            {
                angularMomentum.AngularMomentum = value;
            }
        }

        #endregion
    }

}
