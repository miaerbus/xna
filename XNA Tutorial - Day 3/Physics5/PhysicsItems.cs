using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Artificial.XNATutorial;

namespace Artificial.XNATutorial.Physics
{
    // ITEMS SUPPORTED BY PHYSICS SIMULATOR

    // Movable
    public interface IMovable : IPosition, IVelocity
    {
    }

    // Rotatable
    public interface IRotatable : IRotationMatrix, IAngularPosition, IAngularVelocity
    {
    }

    // ForceUser
    public interface IForceUser : IMass, IAccumulatedForce, IAcceleration, IMovable
    {
    }

    // TorqueUser
    public interface ITorqueUser : IAngularMass, IAccumulatedTorque, IAngularAcceleration, IRotatable, IAngularMomentum
    {
    }

    // ParticleCollider
    public interface IParticleCollider : IForceUser, IParticleRadius, ICoefficientOfRestitution
    {
    }

    // RigidCollider
    public interface IRigidCollider : IForceUser, ITorqueUser, IConvex, IBoundingSphere, ICoefficientOfRestitution
    {
    }

    // PositionConstraint
    public class PositionConstraint
    {
        public IPosition Item1;
        public IPosition Item2;
        public float MinimumDistance;
        public float MaximumDistance;
    }

    // Spring
    public class Spring
    {
        public IPosition Item1;
        public IPosition Item2;
        public float RelaxedDistance;
        public float ForceConstant;
    }

    // Constant Force
    public class ConstantForce
    {
        public Vector3 ForcePerSecond;
        public ConstantForce(Vector3 ForcePerSecond)
        {
            this.ForcePerSecond = ForcePerSecond;
        }        
    }

    // Constant Acceleration
    public class ConstantAcceleration
    {
        public Vector3 Acceleration;
        public ConstantAcceleration(Vector3 Acceleration)
        {
            this.Acceleration = Acceleration;
        }
    }

    // IConvex
    public interface IConvex
    {
        ConvexPolyhedron ConvexPolyhedron { get; set;}
    }

    // Convex Polyhedron
    public class ConvexPolyhedron
    {
        public List<Plane> Planes = new List<Plane>();
        public virtual void ProjectToAxis(Vector3 axis, Vector3 zero, out float min, out float max)
        {
            min = 0;
            max = 0;
        }
        public virtual void MinimumOnAxis(Vector3 axis, Vector3 zero, out float min, out Vector3 position)
        {
            min = 0;
            position = Vector3.Zero;
        }
    }

    // Convex Polygon
    public class ConvexPolygon : ConvexPolyhedron
    {
        public List<Vector3> Points = new List<Vector3>();
        public Vector3 Normal = Vector3.Backward;
        public void CalculatePlanes()
        {
            // Create planes between given points and using the specified normal
            Planes.Clear();
            if (Planes.Capacity < Points.Count) Planes.Capacity = Points.Count;
            Vector3 v, n;
            float d;
            for (int i = 0; i < Points.Count; i++)
            {
                int j = (i + 1) % Points.Count;
                v = Points[i] - Points[j];                
                Vector3.Cross(ref v, ref Normal,out n);
                n.Normalize();
                v = Points[i];
                Vector3.Dot(ref n, ref v, out d);
                Planes.Add(new Plane(n, d));
            }
        }
        public override void ProjectToAxis(Vector3 axis, Vector3 zero, out float min, out float max)
        {
            min = float.PositiveInfinity;
            max = float.NegativeInfinity;

            for (int i = 0; i < Points.Count; i++)
            {
                float p = Vector3.Dot(Points[i]-zero, axis);
                if (p < min)
                {
                    min = p;
                }
                if (p > max)
                {
                    max = p;
                }
            }
        }
        public override void MinimumOnAxis(Vector3 axis, Vector3 zero, out float min, out Vector3 position)
        {
            min = float.PositiveInfinity;
            position = Vector3.Zero;

            for (int i = 0; i < Points.Count; i++)
            {
                float p = Vector3.Dot(Points[i] - zero, axis);
                if (p < min)
                {
                    min = p;
                    position = Points[i];
                }
            }
        }
    }

    // SUPPORTING CLASSES

    // Position    
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