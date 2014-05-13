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
