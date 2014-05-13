using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Artificial.XNATutorial
{
    // Velocity
    public interface IVelocity { Vector3 Velocity { get; set;} }
    public class PVelocity { public Vector3 Velocity; }

    // Acceleration
    public interface IAcceleration { Vector3 Acceleration { get; set;} }
    public class PAcceleration { public Vector3 Acceleration; }

    // Mass
    public interface IMass { float Mass { get; set;} }
    public class PMass { public float Mass; }

    // Accumulated force
    public interface IAccumulatedForce { Vector3 AccumulatedForce { get; set;} }
    public class PAccumulatedForce { public Vector3 AccumulatedForce; }


    // Angular position
    public interface IAngularPosition { Quaternion AngularPosition { get; set;} }
    public class PAngularPosition : DataCell, IAngularPosition
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
    public interface IRotationMatrix { Matrix Rotation { get; set;} }
    public class PRotationMatrix { public Matrix Rotation = Matrix.Identity; }

    // Angular velocity
    public interface IAngularVelocity { Vector3 AngularVelocity { get; set;} }
    public class PAngularVelocity { public Vector3 AngularVelocity; }

    // Angular mass = moment of inertia tensor
    public interface IAngularMass { Matrix AngularMass { get; set;} }
    public class PAngularMass { public Matrix AngularMass; }

    // Accumulated torque
    public interface IAccumulatedTorque { Vector3 AccumulatedTorque { get; set;} }
    public class PAccumulatedTorque { public Vector3 AccumulatedTorque; }

    // Angular momentum
    public interface IAngularMomentum { Vector3 AngularMomentum { get; set;} }
    public class PAngularMomentum { public Vector3 AngularMomentum; }


    // Particle radius
    public interface IParticleRadius { float ParticleRadius { get; set;} }
    public class PParticleRadius { public float ParticleRadius; }

    // Coefficient of restitution
    public interface ICoefficientOfRestitution { float CoefficientOfRestitution { get; set;} }
    public class PCoefficientOfRestitution { public float CoefficientOfRestitution; }
}
