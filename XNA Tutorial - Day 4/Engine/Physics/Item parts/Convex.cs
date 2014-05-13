using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Artificial.XNATutorial;

namespace Artificial.XNATutorial.Physics
{
    // IConvex
    public interface IConvex
    {
        Convex Convex { get; set; }
    }

    public class PConvex
    {
        public Convex Convex;
    }

    // Convex Polyhedron
    public class Convex
    {
        public List<Plane> Planes = new List<Plane>();
        public List<Vector3> Points = new List<Vector3>();

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
}