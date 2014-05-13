using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Artificial.XNATutorial;

namespace Artificial.XNATutorial.Physics
{
    public class ParticleConvexCollisions : Part
    {
        int updateOrder;
        List<ABTLinkedList> filteredItems = new List<ABTLinkedList>();
        AdaptiveBinaryTree sceneABT;

        // CONSTRUCTOR
        public ParticleConvexCollisions(int updateOrder)
        {
            this.updateOrder = updateOrder;
        }

        // INSTALL
        public override void Install(Item parent)
        {
            base.Install(parent);
            parent.As<CollisionDetector>().RegisterCollisionMethod(updateOrder, DetectCollision);
        }

        // DETECT COLLISION
        void DetectCollision(float dt, Item item1, Item item2)
        {
            ParticleCollider p;
            ConvexCollider c;
            Item pItem;
            Item cItem;

            p = item1.As<ParticleCollider>();
            if (p == null)
            {                
                p = item2.As<ParticleCollider>();
                if (p == null) return;
                c = item1.As<ConvexCollider>();
                if (c == null) return;
                pItem = item2;
                cItem = item1;
            }
            else
            {
                c = item2.As<ConvexCollider>();
                if (c == null) return;
                pItem = item1;
                cItem = item2;
            }

            // We have particle and convex items
            Vector3 direction;
            float distance, r;
            direction = p.Position - (c.Position + c.BoundingSphere.Center);
            distance = direction.Length();
            r = p.ParticleRadius + c.BoundingSphere.Radius;
            if (distance < r)
            {
                // Move particle in relative space of rigid body
                Vector3 position1 = p.Position - c.Position;
                Matrix m = Matrix.Invert(c.Rotation);
                position1 = Vector3.Transform(position1, m);

                // Check all planes
                bool collides = true;
                Plane plane;
                int minP = -1;
                float minX = float.NegativeInfinity;
                for (int k = 0; k < c.Convex.Planes.Count; k++)
                {
                    // Project particle onto normal
                    plane = c.Convex.Planes[k];
                    float x = Vector3.Dot(position1, plane.Normal);
                    float dx = (x - p.ParticleRadius) - plane.D;
                    if (dx > minX)
                    {
                        minP = k;
                        minX = dx;
                    }
                    if (x - p.ParticleRadius > plane.D)
                    {
                        collides = false;
                        k = int.MaxValue-1;
                    }
                }

                if (collides)
                {
                    // project along normal of plane with closest collision
                    plane = c.Convex.Planes[minP];

                    Vector3 normal = Vector3.TransformNormal(plane.Normal, c.Rotation);
                    Vector3 pImpact = p.Position - p.ParticleRadius * normal;

                    // Only apply collision if it isn't overriden
                    bool apply = true;

                    if (pItem.As<CustomCollider>() != null)
                    {
                        apply &= !pItem.As<CustomCollider>().OverrideCollision.Contains(cItem.GetType());
                    }
                    if (cItem.As<CustomCollider>() != null)
                    {
                        apply &= !cItem.As<CustomCollider>().OverrideCollision.Contains(pItem.GetType());
                    }

                    if (apply)
                    {
                        // Simple relaxation
                        float m1 = 0;
                        float m2 = 0;
                        float p1 = 0.5f;
                        float p2 = 0.5f;
                        if (p.ForceUser != null) m1 = p.ForceUser.Mass;
                        if (c.ForceUser != null) m2 = p.ForceUser.Mass;
                        if (m1 != 0)
                        {
                            if (m2 != 0)
                            {
                                p1 = m2 / (m1 + m2);
                                p2 = m1 / (m1 + m2);
                                if (p1 == float.NaN) p1 = 1;
                                if (p2 == float.NaN) p1 = 1;
                            }
                            else
                            {
                                p1 = 1;
                                p2 = 0;
                            }
                        }
                        else
                        {
                            if (m2 != 0)
                            {
                                p1 = 0;
                                p2 = 1;
                            }
                        }
                        float dx = minX;
                        dx *= 1.1f;
                        p.Position -= normal * dx * p1;
                        c.Position += normal * dx * p2;

                        // Calculate impact impulse
                        Vector3 tArm = pImpact - c.Position;


                        Vector3 v1 = Vector3.Zero;
                        Vector3 v2 = Vector3.Zero;
                        if (p.ForceUser != null) v1 = p.ForceUser.Velocity;
                        if (c.ForceUser != null) v2 = c.ForceUser.Velocity;
                        if (c.TorqueUser != null) v2 += Vector3.Cross(c.TorqueUser.AngularVelocity, tArm);
                        float cor = 1;
                        PCoefficientOfRestitution icor = item1.As<PCoefficientOfRestitution>();
                        if (icor != null) cor = icor.CoefficientOfRestitution;
                        icor = item2.As<PCoefficientOfRestitution>();
                        if (icor != null) cor *= icor.CoefficientOfRestitution;
                        float v12 = Vector3.Dot(v1 - v2, normal);
                        float m1inverse = 1 / m1;
                        float m2inverse = 1 / m2;
                        if (m1 == 0) m1inverse = 0;
                        if (m2 == 0) m2inverse = 0;
                        float r2 = 0;
                        if (c.TorqueUser != null)
                        {
                            Matrix inertia2Inverse = c.Rotation * Matrix.Invert(c.TorqueUser.AngularMass) * Matrix.Transpose(c.Rotation);
                            r2 = Vector3.Dot(normal, Vector3.Cross(Vector3.Transform(Vector3.Cross(tArm, normal), inertia2Inverse), tArm));
                        }
                        float f = -(1 + cor) * v12 / (m1inverse + m2inverse + r2);

                        // Calculate new velocities
                        Vector3 dv1 = f * normal / m1;
                        Vector3 dv2 = f * -normal / m2;

                        if (p.ForceUser != null) p.ForceUser.Velocity += dv1;
                        if (c.ForceUser != null) c.ForceUser.Velocity += dv2;

                        // Calculate change of angular momentum
                        if (c.TorqueUser != null) c.TorqueUser.AngularMomentum += Vector3.Cross(tArm, -normal * f);
                    }

                    // Call custom collision
                    if (item1.As<CustomCollider>() != null && item1.As<CustomCollider>().CollisionMethod != null) item1.As<CustomCollider>().CollisionMethod(dt, item2, pImpact);
                    if (item2.As<CustomCollider>() != null && item2.As<CustomCollider>().CollisionMethod != null) item2.As<CustomCollider>().CollisionMethod(dt, item1, pImpact);                   
                }
            }
        }
    }
}
