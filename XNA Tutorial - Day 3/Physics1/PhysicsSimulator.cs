using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Artificial.XNATutorial;

namespace Artificial.XNATutorial.Physics
{
    // SUPPORTED ITEMS

    // Movable
    public interface IMovable : IPosition, IVelocity
    {
    }

    // ForceUser
    public interface IForceUser : IMass, IAccumulatedForce, IAcceleration, IMovable
    {
    }

    // ParticleCollider
    public interface IParticleCollider : IForceUser, IParticleRadius, ICoefficientOfRestitution
    {
    }  


    // PHYSICS SIMULATOR    
    class PhysicsSimulator
    {
        // Scene and ABTs
        Scene scene;
        AdaptiveBinaryTree sceneABT;
        public AdaptiveBinaryTree SceneABT
        {
            get
            {
                return sceneABT;
            }
        }
        Dictionary<object, IABTItem> ABTMap = new Dictionary<object, IABTItem>();
        List<ABTLinkedList> filteredItems = new List<ABTLinkedList>();

        // Physical properties
        float gravitationalConstant = 6.67E-11f; // Constant used in universal law of gravity
        public float GravitationalConstant
        {
            get
            {
                return gravitationalConstant;
            }
        }

        float minAcceleration = 10E-3f; // Minimum acceleration that is still considered worth calculating for
        public float MinAcceleration
        {
            get
            {
                return minAcceleration;
            }
        }

        float minMass = 10E6f; // Minimum mass worth generating gravity force for
        public float MinMass
        {
            get
            {
                return minMass;
            }
        }
     
        // CONSTRUCTOR
        public PhysicsSimulator(Scene scene)
        {
            // Add scene handlers
            this.scene = scene;
            scene.OnItemAdd += sceneItemAdd;
            scene.OnItemRemove += sceneItemRemoved;

            // Create ABTs
            sceneABT = new AdaptiveBinaryTree(8);
        }


        // SIMULATE
        public void Simulate(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (dt == 0) return;

            List<int> indices = scene.GetItemIndices();
            LinkedListNode<IABTItem> node;
            BoundingSphere boundingSphere;

            for (int i = 0; i < indices.Count; i++)
            {
                object item = scene[indices[i]];
                IMovable itemMovable = item as IMovable;
                IForceUser itemForceUser = item as IForceUser;
                IParticleCollider itemParticleCollider = item as IParticleCollider;

                // Universal law of gravity
                if (itemForceUser != null)
                {
                    // Calculate gravity only for objects of sufficient mass
                    if (itemForceUser.Mass > minMass)
                    {
                        // Affect only object close enough
                        boundingSphere.Center = itemForceUser.Position;
                        boundingSphere.Radius = (float)Math.Sqrt(gravitationalConstant * itemForceUser.Mass / minAcceleration);
                        // Use ABT to get items inside calculated radius
                        sceneABT.GetItems(boundingSphere, filteredItems);
                        // Iterate through all filtered ABT leaves
                        for (int j = 0; j < filteredItems.Count; j++)
                        {
                            // Iterate through items in current ABT leaf
                            node = filteredItems[j].First;
                            while (node != null)
                            {
                                // Get affected item
                                object item2 = (node.Value as CustomBoundingBoxABTItem).PositionPart;
                                if (item2 != item)
                                {
                                    // See if this item supports forces
                                    IForceUser itemForceUser2 = item2 as IForceUser;
                                    if (itemForceUser2 != null)
                                    {
                                        // Calculate force vector
                                        //
                                        //          m1 * m2
                                        //  F = G * -------
                                        //            r^2
                                        //
                                        Vector3 r = itemForceUser.Position - itemForceUser2.Position;
                                        float d = r.Length();
                                        // Again filter exactly those particles withing the radious
                                        if (d < boundingSphere.Radius)
                                        {
                                            if (itemParticleCollider != null)
                                            {
                                                if (d < itemParticleCollider.ParticleRadius)
                                                {
                                                    d = itemParticleCollider.ParticleRadius;
                                                }
                                            }
                                            float f = gravitationalConstant * itemForceUser.Mass * itemForceUser2.Mass / (d * d);
                                            r.Normalize();
                                            itemForceUser2.AccumulatedForce += r * f;
                                        }
                                    }
                                }
                                // Go to next item in this leaf
                                node = node.Next;
                            }
                        }
                    }
                }

                // Transform accumulated force into acceleration
                if (itemForceUser != null)
                {
                    // Calculate acceleration
                    //
                    //                      F
                    //  F = m * a  =>  a = ---
                    //                      m
                    //
                    itemForceUser.Acceleration = itemForceUser.AccumulatedForce / itemForceUser.Mass;

                    // Zero out force for next turn
                    itemForceUser.AccumulatedForce = Vector3.Zero;

                    // Calculate velocity
                    //
                    //  v = v0 + a * dt
                    //
                    itemForceUser.Velocity += itemForceUser.Acceleration * dt;
                }

                // Movement
                if (itemMovable != null)
                {
                    // Calculate position
                    //
                    //  x = x0 + v * dt
                    //
                    itemMovable.Position += itemMovable.Velocity * dt;
                }

                // Particle Collisions
                if (itemParticleCollider != null)
                {
                    // Use ABT to get potential colliders
                    boundingSphere.Center = itemParticleCollider.Position;
                    boundingSphere.Radius = itemParticleCollider.ParticleRadius;
                    sceneABT.GetItems(boundingSphere, filteredItems);
                    // Iterate through all filtered ABT leaves
                    for (int j = 0; j < filteredItems.Count; j++)
                    {
                        // Iterate through items in current ABT leaf
                        node = filteredItems[j].First;
                        while (node != null)
                        {
                            // Get affected item
                            object item2 = (node.Value as CustomBoundingBoxABTItem).PositionPart;
                            if (item2 != item)
                            {
                                // See if this item supports particle collisions
                                IParticleCollider itemParticleCollider2 = item2 as IParticleCollider;
                                if (itemParticleCollider2 != null)
                                {
                                    Vector3 direction = itemParticleCollider.Position - itemParticleCollider2.Position;
                                    float distance = direction.Length();
                                    float r = itemParticleCollider.ParticleRadius + itemParticleCollider2.ParticleRadius;
                                    if (distance < r)
                                    {
                                        // Simple relaxation
                                        Vector3 normal = Vector3.Normalize(direction);
                                        float m1 = itemParticleCollider.Mass;
                                        float m2 = itemParticleCollider2.Mass;
                                        float p1 = m2 / (m1 + m2);
                                        float p2 = m1 / (m1 + m2);
                                        float dx = (r - distance);
                                        dx *= 1.5f;
                                        itemParticleCollider.Position += normal * dx * p1;
                                        itemParticleCollider2.Position -= normal * dx * p2;

                                        // Calculate new velocities
                                        //
                                        //        v1 * (m1 - cof * m2) * (1 + cof) * m2 * v2
                                        //  nv1 = ------------------------------------------
                                        //                         m1 + m2
                                        //
                                        normal = Vector3.Normalize(itemParticleCollider.Position - itemParticleCollider2.Position);
                                        float v1 = Vector3.Dot(itemParticleCollider.Velocity, normal);
                                        float v2 = Vector3.Dot(itemParticleCollider2.Velocity, normal);
                                        float cof = itemParticleCollider.CoefficientOfRestitution * itemParticleCollider2.CoefficientOfRestitution;
                                        float nv1 = (v1 * (m1 - m2 * cof) + (1 + cof) * m2 * v2) / (m1 + m2);
                                        float nv2 = (v2 * (m2 - m1 * cof) + (1 + cof) * m1 * v1) / (m1 + m2);
                                        itemParticleCollider.Velocity -= normal * v1;
                                        itemParticleCollider2.Velocity -= normal * v2;
                                        itemParticleCollider.Velocity += normal * nv1;
                                        itemParticleCollider2.Velocity += normal * nv2;
                                    }
                                }
                            }
                            // Go to next item in this leaf
                            node = node.Next;
                        }
                    }
                }
            }
        }


        // SCENE ITEM ADDED
        void sceneItemAdd(object sender, Scene.SceneManipulationEventArgs e)
        {
            // Update sceneABT
            IPositionWithEvents itemPosition = e.Item as IPositionWithEvents;
            IParticleRadius itemParticleRadius = e.Item as IParticleRadius;
            if (itemPosition != null)
            {
                CustomBoundingBoxABTItem i = new CustomBoundingBoxABTItem();
                i.PositionPart = itemPosition;
                if (itemParticleRadius != null)
                {
                    i.BoundingBox = new BoundingBox(itemParticleRadius.ParticleRadius * -Vector3.One, itemParticleRadius.ParticleRadius * Vector3.One);
                }
                else
                {
                    i.BoundingBox = new BoundingBox(-Vector3.One, Vector3.One);
                }
                sceneABT.Add(i);
                ABTMap.Add(itemPosition, i);
            }
        }

        // SCENE ITEM REMOVED
        void sceneItemRemoved(object sender, Scene.SceneManipulationEventArgs e)
        {
            // Update sceneABT
            IABTItem item = ABTMap[e.Item];
            if (item != null)
            {
                sceneABT.Remove(item);
            }
        }
    }
}
