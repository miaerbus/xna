using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Artificial.XNATutorial;

namespace Artificial.XNATutorial.Physics
{
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

            #region "Accumulate constants"

            // Accumulate constants
            Vector3 constantForce = Vector3.Zero;
            Vector3 constantAcceleration = Vector3.Zero;
            for (int i = 0; i < indices.Count; i++)
            {
                object item = scene[indices[i]];
                ConstantForce force = item as ConstantForce;
                ConstantAcceleration acceleration = item as ConstantAcceleration;

                if (force != null)
                {
                    constantForce += force.ForcePerSecond;
                }

                if (acceleration != null)
                {
                    constantAcceleration += acceleration.Acceleration;
                }
            }
            constantForce *= dt;

            #endregion

            // Simulate on scene items
            for (int i = 0; i < indices.Count; i++)
            {
                object item = scene[indices[i]];
                IMovable itemMovable = item as IMovable;
                IForceUser itemForceUser = item as IForceUser;
                IRotatable itemRotatable = item as IRotatable;
                ITorqueUser itemTorqueUser = item as ITorqueUser;
                IParticleCollider itemParticleCollider = item as IParticleCollider;
                Spring itemSpring = item as Spring;
                PositionConstraint itemConstraint = item as PositionConstraint;

                // Constant forces
                if (itemForceUser != null)
                {
                    itemForceUser.AccumulatedForce += constantForce;
                    itemForceUser.AccumulatedForce += constantAcceleration * itemForceUser.Mass;
                }

                #region "Universal law of gravity"

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

                #endregion

                #region "Mechanics"

                // TRANSLATION

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


                // ROTATION

                // Transform accumulated torque into angular momentum
                if (itemTorqueUser != null)
                {
                    // Calculate position
                    //
                    //  L = L0 + T * dt
                    //
                    itemTorqueUser.AngularMomentum += itemTorqueUser.AccumulatedTorque * dt;

                    // Zero out torque for next turn
                    itemTorqueUser.AccumulatedTorque = Vector3.Zero;

                    // Calculate rotation matrix
                    itemTorqueUser.Rotation = Matrix.CreateFromQuaternion(Quaternion.Normalize(itemTorqueUser.AngularPosition));

                    // Calculate moment of inertia tensor in world space
                    //
                    //   -1        -1      T
                    //  I   = R * I     * R
                    //             body
                    //
                    Matrix inertiaInverse = itemTorqueUser.Rotation * Matrix.Invert(itemTorqueUser.AngularMass) * Matrix.Transpose(itemTorqueUser.Rotation);

                    // Calculate angular velocity
                    //
                    //       -1
                    //  w = I   * L
                    //
                    itemTorqueUser.AngularVelocity = Vector3.Transform(itemTorqueUser.AngularMomentum, inertiaInverse);
                }                               

                // Rotation
                if (itemRotatable != null)
                {
                    // Calculate orientation
                    //
                    //           w * q
                    //  q = q0 + ----- * dt
                    //             2
                    //
                    itemRotatable.AngularPosition += new Quaternion(itemRotatable.AngularVelocity, 0) * itemRotatable.AngularPosition * 0.5f * dt;
                }

                #endregion

                #region "Collisions"

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
                                    //
                                    // PARTICLE - PARTICLE COLLISION
                                    //
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

                                // See if this item supports rigid collisions
                                IRigidCollider itemRigidCollider2 = item2 as IRigidCollider;
                                if (itemRigidCollider2 != null)
                                {
                                    //
                                    // PARTICLE - RIGID BODY COLLISION
                                    //
                                    Vector3 direction;
                                    float distance, r;
                                    IBoundingSphere itemBoundingSphere2 = item2 as IBoundingSphere;
                                    if (itemBoundingSphere2 != null)
                                    {
                                        direction = itemParticleCollider.Position - (itemRigidCollider2.Position + itemBoundingSphere2.BoundingSphere.Center);
                                        distance = direction.Length();
                                        r = itemParticleCollider.ParticleRadius + itemBoundingSphere2.BoundingSphere.Radius;
                                    }
                                    else
                                    {
                                        distance = 0;
                                        r = 1;
                                    }
                                    if (distance < r)
                                    {
                                        // Move particle in relative space of rigid body
                                        Vector3 position1 = itemParticleCollider.Position - itemRigidCollider2.Position;
                                        Vector3 velocity1 = itemParticleCollider.Velocity - itemRigidCollider2.Velocity;
                                        Matrix m  = Matrix.Invert(itemRigidCollider2.Rotation);
                                        position1 = Vector3.Transform(position1, m);
                                        velocity1 = Vector3.TransformNormal(velocity1, m);

                                        // Check all planes
                                        bool collides = true;
                                        Plane p;
                                        int minP = -1;
                                        float minX = float.NegativeInfinity;
                                        for (int k = 0; k < itemRigidCollider2.ConvexPolyhedron.Planes.Count; k++)
                                        {
                                            // Project particle onto normal
                                            p = itemRigidCollider2.ConvexPolyhedron.Planes[k];
                                            float x = Vector3.Dot(position1, p.Normal);
                                            float dx = (x - itemParticleCollider.ParticleRadius) - p.D;
                                            if (dx > minX)
                                            {
                                                minP = k;
                                                minX = dx;
                                            }
                                            if (x-itemParticleCollider.ParticleRadius>p.D)
                                            {
                                                collides = false;
                                                k = 100000;                                                
                                            }
                                        }

                                        if (collides)
                                        {
                                            // project along normal of plane with closest collision
                                            p = itemRigidCollider2.ConvexPolyhedron.Planes[minP];

                                            // Simple relaxation
                                            Vector3 normal = Vector3.TransformNormal(p.Normal, itemRigidCollider2.Rotation);
                                            float m1 = itemParticleCollider.Mass;
                                            float m2 = itemRigidCollider2.Mass;
                                            float p1 = m2 / (m1 + m2);
                                            float p2 = m1 / (m1 + m2);
                                            float dx = minX;
                                            dx *= 1.5f;
                                            itemParticleCollider.Position -= normal * dx * p1;
                                            itemRigidCollider2.Position += normal * dx * p2;

                                            // Calculate new velocities
                                            //
                                            //        v1 * (m1 - cof * m2) * (1 + cof) * m2 * v2
                                            //  nv1 = ------------------------------------------
                                            //                         m1 + m2
                                            //
                                            Vector3 pImpact = itemParticleCollider.Position - itemParticleCollider.ParticleRadius * normal;
                                            Vector3 tArm = pImpact - itemRigidCollider2.Position;
                                            float v1 = Vector3.Dot(itemParticleCollider.Velocity, normal);
                                            float v2 = Vector3.Dot(itemRigidCollider2.Velocity + Vector3.Cross(itemRigidCollider2.AngularVelocity, tArm), normal);
                                            float cof = itemParticleCollider.CoefficientOfRestitution * itemRigidCollider2.CoefficientOfRestitution;
                                            float nv1 = (v1 * (m1 - m2 * cof) + (1 + cof) * m2 * v2) / (m1 + m2);
                                            float nv2 = (v2 * (m2 - m1 * cof) + (1 + cof) * m1 * v1) / (m1 + m2);
                                            itemParticleCollider.Velocity -= normal * v1;
                                            itemRigidCollider2.Velocity -= normal * v2;
                                            itemParticleCollider.Velocity += normal * nv1;
                                            itemRigidCollider2.Velocity += normal * nv2;

                                            // Calculate torque
                                            //
                                            float f1 = (v2 - v1) * m1 * m2 / (m1+m2);
                                            Vector3 torque = Vector3.Cross(tArm, normal * f1);
                                            itemRigidCollider2.AccumulatedTorque -= torque;
                                        }
                                    }
                                }

                            }
                            // Go to next item in this leaf
                            node = node.Next;
                        }
                    }
                }

                #endregion
            }

            #region "Constraints"

            // Satisfy Constraints
            for (int i = 0; i < indices.Count; i++)
            {
                object item = scene[indices[i]];
                Spring itemSpring = item as Spring;
                PositionConstraint itemConstraint = item as PositionConstraint;

                // spring
                if (itemSpring != null)
                {
                    Vector3 direction = itemSpring.Item1.Position - itemSpring.Item2.Position;
                    float distance = direction.Length();

                    // Find out how to distribute the force
                    float p1, p2;
                    IForceUser f1 = itemSpring.Item1 as IForceUser;
                    IForceUser f2 = itemSpring.Item2 as IForceUser;
                    if (f1 != null)
                    {
                        if (f2 != null)
                        {
                            float m1 = f1.Mass;
                            float m2 = f2.Mass;
                            p1 = m2 / (m1 + m2);
                            p2 = m1 / (m1 + m2);
                        }
                        else
                        {
                            p1 = 1;
                            p2 = 0;
                        }
                    }
                    else
                    {
                        if (f2 != null)
                        {
                            p1 = 0;
                            p2 = 1;
                        }
                        else
                        {
                            p1 = 0.5f;
                            p2 = 0.5f;
                        }
                    }

                    // Calculate spring force
                    //
                    //  F = -x * c
                    //
                    float dx = (itemSpring.RelaxedDistance - distance) * itemSpring.ForceConstant;
                    Vector3 normal = Vector3.Normalize(direction);

                    // Apply force
                    if (f1 != null)
                    {
                        f1.AccumulatedForce += normal * dx * p1 / dt;
                    }
                    if (f2 != null)
                    {
                        f2.AccumulatedForce -= normal * dx * p2 / dt;
                    }
                }

                // constraint
                if (itemConstraint != null)
                {
                    Vector3 direction = itemConstraint.Item1.Position - itemConstraint.Item2.Position;
                    float distance = direction.Length();
                    // Check if constraints are met
                    if (distance < itemConstraint.MinimumDistance || distance > itemConstraint.MaximumDistance)
                    {
                        // Calculate target length
                        float target=0;
                        if (distance < itemConstraint.MinimumDistance) target = itemConstraint.MinimumDistance;
                        if (distance > itemConstraint.MaximumDistance) target = itemConstraint.MaximumDistance;

                        // Find out hot to distribute the translation
                        float p1, p2;
                        IForceUser f1 = itemConstraint.Item1 as IForceUser;
                        IForceUser f2 = itemConstraint.Item2 as IForceUser;
                        if (f1 != null)
                        {
                            if (f2 != null)
                            {
                                float m1 = f1.Mass;
                                float m2 = f2.Mass;
                                p1 = m2 / (m1 + m2);
                                p2 = m1 / (m1 + m2);
                            }
                            else
                            {
                                p1 = 1;
                                p2 = 0;
                            }
                        }
                        else
                        {
                            if (f2 != null)
                            {
                                p1 = 0;
                                p2 = 1;
                            }
                            else
                            {
                                p1 = 0.5f;
                                p2 = 0.5f;
                            }
                        }

                        // Calculate translation to target
                        float dx = (target - distance);
                        Vector3 normal = Vector3.Normalize(direction);                        

                        // Apply translation
                        itemConstraint.Item1.Position += normal * dx * p1;
                        itemConstraint.Item2.Position -= normal * dx * p2;

                        // Correct velocities by removed translation
                        if (f1 != null)
                        {
                            f1.Velocity += normal * dx * p1 / dt;
                        }
                        if (f2 != null)
                        {
                            f2.Velocity -= normal * dx * p2 / dt;
                        }
                    }
                }
            }    
    
            #endregion
        }


        // SCENE ITEM ADDED
        void sceneItemAdd(object sender, Scene.SceneManipulationEventArgs e)
        {
            // Update sceneABT
            IPositionWithEvents itemPosition = e.Item as IPositionWithEvents;
            IParticleRadius itemParticleRadius = e.Item as IParticleRadius;
            IBoundingSphere itemBoundingSphere = e.Item as IBoundingSphere;

            if (itemPosition != null)
            {
                CustomBoundingBoxABTItem i = new CustomBoundingBoxABTItem();
                i.PositionPart = itemPosition;
                if (itemParticleRadius != null)
                {
                    i.BoundingBox = new BoundingBox(itemParticleRadius.ParticleRadius * -Vector3.One, itemParticleRadius.ParticleRadius * Vector3.One);
                }
                else if (itemBoundingSphere != null)
                {
                    i.BoundingBox = BoundingBox.CreateFromSphere(itemBoundingSphere.BoundingSphere);                   
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
