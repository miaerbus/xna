using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Artificial.XNATutorial.Physics
{
    class RigidBody : IPositionWithEvents, IVelocity, IAcceleration, IMass, IAccumulatedForce,
        IAngularPosition, IAngularVelocity, IAngularAcceleration, IAngularMass,
        IAccumulatedTorque, IAngularMomentum, IConvex, IBoundingSphere,

        IMovable, 
        IRotatable,
        IForceUser,
        ITorqueUser
        
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
                RaiseOnPositionChanged();
            }
        }

        // OnPositionChanged
        public event EventHandler OnPositionChanged;
        private void RaiseOnPositionChanged()
        {
            if (OnPositionChanged != null)
            {
                OnPositionChanged(this, null);
            }
        }

        // Velocity
        private Vector3 velocity;
        public Vector3 Velocity
        {
            get
            {
                return velocity;
            }
            set
            {
                velocity = value;
            }
        }

        // Acceleration
        private Vector3 acceleration;
        public Vector3 Acceleration
        {
            get
            {
                return acceleration;
            }
            set
            {
                acceleration = value;
            }
        }

        // Mass
        private float mass;
        public float Mass
        {
            get
            {
                return mass;
            }
            set
            {
                mass = value;
            }
        }

        // Accumulated force
        private Vector3 accumulatedForce;
        public Vector3 AccumulatedForce
        {
            get
            {
                return accumulatedForce;
            }
            set
            {
                accumulatedForce = value;
            }
        }


        // Rotation matrix
        private Matrix rotation;
        public Matrix Rotation
        {
            get
            {
                return rotation;
            }
            set
            {
                rotation = value;
            }
        }
        
        // Angular position
        private Quaternion angularPosition;
        public Quaternion AngularPosition
        {
            get
            {
                return angularPosition;
            }
            set
            {
                angularPosition = value;
            }
        }

        // Angular velocity
        private Vector3 angularVelocity;
        public Vector3 AngularVelocity
        {
            get
            {
                return angularVelocity;
            }
            set
            {
                angularVelocity = value;
            }
        }

        // Angular acceleration
        private Vector3 angularAcceleration;
        public Vector3 AngularAcceleration
        {
            get
            {
                return angularAcceleration;
            }
            set
            {
                angularAcceleration = value;
            }
        }

        // Angular mass
        private Matrix angularMass;
        public Matrix AngularMass
        {
            get
            {
                return angularMass;
            }
            set
            {
                angularMass = value;
            }
        }

        // Accumulated torque
        private Vector3 accumulatedTorque;
        public Vector3 AccumulatedTorque
        {
            get
            {
                return accumulatedTorque;
            }
            set
            {
                accumulatedTorque = value;
            }
        }

        // Angular momentum
        private Vector3 angularMomentum;
        public Vector3 AngularMomentum
        {
            get
            {
                return angularMomentum;
            }
            set
            {
                angularMomentum = value;
            }
        }


        // Convex polyhedron
        private ConvexPolyhedron convexPolyhedron;
        public ConvexPolyhedron ConvexPolyhedron
        {
            get
            {
                return convexPolyhedron;
            }
            set
            {
                convexPolyhedron = value;
            }
        }

        // Bounding sphere
        private BoundingSphere boundingSphere;
        public BoundingSphere BoundingSphere
        {
            get
            {
                return boundingSphere;
            }
            set
            {
                boundingSphere = value;
            }
        }

    }
}
