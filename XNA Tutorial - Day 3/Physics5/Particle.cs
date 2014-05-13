using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Artificial.XNATutorial.Physics
{
    class Particle : IPositionWithEvents, IVelocity, IAcceleration, IMass, IAccumulatedForce, IParticleRadius,
        ICoefficientOfRestitution,

        IMovable, 
        IForceUser,
        IParticleCollider
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

        // Radius
        private float particleRadius;
        public float ParticleRadius
        {
            get
            {
                return particleRadius;
            }
            set
            {
                particleRadius = value;
            }
        }

        // Coefficient of restitution
        private float coefficientOfRestitution;
        public float CoefficientOfRestitution
        {
            get
            {
                return coefficientOfRestitution;
            }
            set
            {
                coefficientOfRestitution = value;
            }
        }        

    }
}
