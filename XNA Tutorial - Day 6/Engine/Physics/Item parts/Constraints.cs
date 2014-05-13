using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Artificial.XNATutorial;

namespace Artificial.XNATutorial.Physics
{
    // PositionConstraint
    public class PositionConstraint : Item
    {
        public IPosition Item1;
        public IPosition Item2;
        public float MinimumDistance;
        public float MaximumDistance;
    }

    // Spring
    public class Spring : Item
    {
        public IPosition Item1;
        public IPosition Item2;
        public float RelaxedDistance;
        public float ForceConstant;
    }

    // Constant Force
    public class ConstantForce : Item
    {
        public Vector3 ForcePerSecond;
        public ConstantForce(Vector3 ForcePerSecond)
        {
            this.ForcePerSecond = ForcePerSecond;
        }
    }

    // Constant Acceleration
    public class ConstantAcceleration : Item
    {
        public Vector3 Acceleration;
        public ConstantAcceleration(Vector3 Acceleration)
        {
            this.Acceleration = Acceleration;
        }
    }

    // Position point 
    public class PositionPoint : IPosition
    {
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
            }
        }
        public PositionPoint(Vector3 Position)
        {
            position = Position;
        }
    }
}
