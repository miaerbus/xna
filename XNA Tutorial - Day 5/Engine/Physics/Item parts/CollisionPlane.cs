using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Artificial.XNATutorial;

namespace Artificial.XNATutorial.Physics
{
    public class CollisionPlane : Item
    {
        public CollisionPlane(Plane plane)
        {
            PConvex c = Require<PConvex>();
            c.Convex = new Convex();
            c.Convex.Planes.Add(plane);
            Require<ConvexCollider>();
        }
    }



}
