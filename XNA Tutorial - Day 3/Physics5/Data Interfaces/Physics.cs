using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Artificial.XNATutorial
{
    // Velocity
    public interface IVelocity
    {
        Vector3 Velocity { get; set;}
    }

    // Acceleration
    public interface IAcceleration
    {
        Vector3 Acceleration { get; set;}
    }
   
    // Mass
    public interface IMass
    {
        float Mass { get; set;}
    }

    // Accumulated force
    public interface IAccumulatedForce
    {
        Vector3 AccumulatedForce { get; set;}        
    }


    // Angular position
    public interface IAngularPosition
    {
        Quaternion AngularPosition { get; set;}
    }

    // Rotation matrix = Matrix.CreateFromQuaternion( Quaternion.Normalize( AngularPosition ) )
    public interface IRotationMatrix
    {
        Matrix Rotation { get; set;}
    }

    // Angular velocity
    public interface IAngularVelocity
    {
        Vector3 AngularVelocity { get; set;}
    }

    // Angular acceleration
    public interface IAngularAcceleration
    {
        Vector3 AngularAcceleration { get; set;}
    }

    // Angular mass = moment of inertia tensor
    public interface IAngularMass
    {
        Matrix AngularMass { get; set;}
    }

    // Accumulated torque
    public interface IAccumulatedTorque
    {
        Vector3 AccumulatedTorque { get; set;}
    }

    // Angular momentum
    public interface IAngularMomentum
    {
        Vector3 AngularMomentum { get; set;}
    }

 
    // Particle radius
    public interface IParticleRadius
    {
        float ParticleRadius { get; set;}
    }

    // Coefficient of restitution
    public interface ICoefficientOfRestitution
    {
        float CoefficientOfRestitution { get; set;}
    }
}
