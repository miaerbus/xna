using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Artificial.XNATutorial;

namespace Artificial.XNATutorial.Physics
{
    // Convex Polygon
    public class ConvexPolygon : Convex
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
                Vector3.Cross(ref v, ref Normal, out n);
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
                float p = Vector3.Dot(Points[i] - zero, axis);
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
        public override Plane GetExtraPlane(int index, Vector3 point)
        {
            Vector3 p = Points[index];
            Vector3 axis = point - p;
            axis -= Vector3.Dot(axis, Normal) * Normal;
            p -= Vector3.Dot(p, Normal) * Normal;
            return new Plane(Vector3.Normalize(axis), p.Length());
        }
        public override int ExtraPlaneCount
        {
            get
            {
                return Points.Count;
            }
        }

    }
}