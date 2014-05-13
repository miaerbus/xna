using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Artificial.XNATutorial;

namespace Artificial.XNATutorial.Physics
{
    public class ParticleParticleCollisions : Part
    {
        int updateOrder;
        List<ABTLinkedList> filteredItems = new List<ABTLinkedList>();

        // CONSTRUCTOR
        public ParticleParticleCollisions(int updateOrder)
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
            ParticleCollider p = item1.As<ParticleCollider>();
            ParticleCollider c = item2.As<ParticleCollider>();

            if (p != null && c != null)
            {
                //
                // PARTICLE - PARTICLE COLLISION
                //
                Vector3 direction = p.Position - c.Position;
                float distance = direction.Length();
                float r = p.ParticleRadius + c.ParticleRadius;
                if (distance < r)
                {
                    Vector3 normal = Vector3.Normalize(direction);
                    Vector3 pImpact = p.Position + p.ParticleRadius * normal;

                    // Only apply collision if it isn't overriden
                    bool apply = true;

                    if (item1.As<CustomCollider>() != null)
                    {
                        apply &= !item1.As<CustomCollider>().OverrideCollisionWithType.Contains(item2.GetType());
                        apply &= !item1.As<CustomCollider>().OverrideCollisionWithItem.Contains(item2);
                    }
                    if (item2.As<CustomCollider>() != null)
                    {
                        apply &= !item2.As<CustomCollider>().OverrideCollisionWithType.Contains(item1.GetType());
                        apply &= !item2.As<CustomCollider>().OverrideCollisionWithItem.Contains(item1);
                    }

                    if (apply)
                    {
                        // Simple relaxation
                        float m1 = 0;
                        float m2 = 0;
                        float p1 = 0.5f;
                        float p2 = 0.5f;
                        if (p.ForceUser != null) m1 = p.ForceUser.Mass;
                        if (c.ForceUser != null) m2 = c.ForceUser.Mass;
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
                        float dx = (r - distance);
                        dx *= 1.1f;
                        p.Position += normal * dx * p1;
                        c.Position -= normal * dx * p2;

                        // Calculate impact impulse
                        Vector3 v1 = Vector3.Zero;
                        Vector3 v2 = Vector3.Zero;
                        if (p.ForceUser != null) v1 = p.ForceUser.Velocity;
                        if (c.ForceUser != null) v2 = c.ForceUser.Velocity;
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
                        float f = -(1 + cor) * v12 / (m1inverse + m2inverse);

                        // Calculate new velocities
                        Vector3 dv1 = f * normal / m1;
                        Vector3 dv2 = f * -normal / m2;

                        if (p.ForceUser != null) p.ForceUser.Velocity += dv1;
                        if (c.ForceUser != null) c.ForceUser.Velocity += dv2;
                    }

                    // Call custom collision
                    if (item1.As<CustomCollider>() != null && item1.As<CustomCollider>().CollisionMethod != null) item1.As<CustomCollider>().CollisionMethod(dt, item2, pImpact);
                    if (item2.As<CustomCollider>() != null && item2.As<CustomCollider>().CollisionMethod != null) item2.As<CustomCollider>().CollisionMethod(dt, item1, pImpact);
                }
            }
        }
    }
}
