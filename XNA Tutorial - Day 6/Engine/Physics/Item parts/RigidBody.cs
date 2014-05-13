using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Artificial.XNATutorial.Physics
{
    public class RigidBody : Part
    {
        public override void Install(Item parent)
        {
            base.Install(parent);
            parent.Require<Movable>();
            parent.Require<Rotatable>();
            parent.Require<ForceUser>();
            parent.Require<TorqueUser>();
            parent.Require<ConvexCollider>();

            // properties for quick access
            position = parent.As<PPosition>();
            velocity = parent.As<PVelocity>();
            acceleration = parent.As<PAcceleration>();
            mass = parent.As<PMass>();
            accumulatedForce = parent.As<PAccumulatedForce>();

            rotation = parent.As<PRotationMatrix>();
            angularPosition = parent.As<PAngularPosition>();
            angularVelocity = parent.As<PAngularVelocity>();
            angularMass = parent.As<PAngularMass>();
            accumulatedTorque = parent.As<PAccumulatedTorque>();
            angularMomentum = parent.As<PAngularMomentum>();

            boundingSphere = parent.As<PBoundingSphere>();
            coefficientOfRestitution = parent.As<PCoefficientOfRestitution>();
            convex = parent.As<PConvex>();
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

        PConvex convex;
        public Convex Convex
        {
            get
            {
                return convex.Convex;
            }
            set
            {
                convex.Convex = value;
            }
        }

        PBoundingSphere boundingSphere;
        public BoundingSphere BoundingSphere
        {
            get
            {
                return boundingSphere.BoundingSphere;
            }
            set
            {
                boundingSphere.BoundingSphere = value;
            }
        }

        PCoefficientOfRestitution coefficientOfRestitution;
        public float CoefficientOfRestitution
        {
            get
            {
                return coefficientOfRestitution.CoefficientOfRestitution;
            }
            set
            {
                coefficientOfRestitution.CoefficientOfRestitution = value;
            }
        }

        #endregion
    }
}
