using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Artificial.XNATutorial
{
    // Velocity
    public class PVelocity { public Vector3 Velocity; }

    // Acceleration
    public class PAcceleration { public Vector3 Acceleration; }

    // Mass
    public class PMass { public float Mass; }

    // Accumulated force
    public class PAccumulatedForce { public Vector3 AccumulatedForce; }


    // Angular position
    public class PAngularPosition : DataCell
    {
        Quaternion angularPosition = Quaternion.Identity;
        public Quaternion AngularPosition
        {
            get
            {
                return angularPosition;
            }
            set
            {
                angularPosition = value;
                angularPosition.Normalize();
                if (parent.As<PRotationMatrix>() != null)
                {
                    parent.As<PRotationMatrix>().Rotation = Matrix.CreateFromQuaternion(angularPosition);
                }
            }
        }
    }


    // Rotation matrix = Matrix.CreateFromQuaternion( Quaternion.Normalize( AngularPosition ) )
    public class PRotationMatrix { public Matrix Rotation = Matrix.Identity; }

    // Angular velocity
    public class PAngularVelocity { public Vector3 AngularVelocity; }

    // Angular mass = moment of inertia tensor
    public class PAngularMass { public Matrix AngularMass; }

    // Accumulated torque
    public class PAccumulatedTorque { public Vector3 AccumulatedTorque; }

    // Angular momentum
    public class PAngularMomentum { public Vector3 AngularMomentum; }


    // Particle radius
    public class PParticleRadius { public float ParticleRadius; }

    // Coefficient of restitution
    public class PCoefficientOfRestitution { public float CoefficientOfRestitution; }
}
