using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Artificial.XNATutorial.Physics
{
    public class Particle : Part
    {
        public override void Install(Item parent)
        {
            base.Install(parent);
            parent.Require<Movable>();
            parent.Require<ForceUser>();
            parent.Require<ParticleCollider>();

            // properties for quick access
            position = parent.As<PPosition>();
            velocity = parent.As<PVelocity>();
            acceleration = parent.As<PAcceleration>();
            mass = parent.As<PMass>();
            accumulatedForce = parent.As<PAccumulatedForce>();

            particleRadius = parent.As<PParticleRadius>();
            coefficientOfRestitution = parent.As<PCoefficientOfRestitution>();
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

        PParticleRadius particleRadius;
        public float ParticleRadius
        {
            get
            {
                return particleRadius.ParticleRadius;
            }
            set
            {
                particleRadius.ParticleRadius = value;
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
