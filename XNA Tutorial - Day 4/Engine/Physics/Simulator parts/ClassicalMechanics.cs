using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Artificial.XNATutorial;

namespace Artificial.XNATutorial.Physics
{
    public class ClassicalMechanics : Part
    {
        int updateOrder;

        public ClassicalMechanics(int updateOrder)
        {
            this.updateOrder = updateOrder;
        }

        public override void Install(Item parent)
        {
            base.Install(parent);
            parent.As<ISceneIterator>().RegisterUpdateItemMethod(updateOrder, SimulateMechanics);
        }

        void SimulateMechanics(float dt, Item item)
        {
            Movable itemMovable = item.As<Movable>();
            ForceUser itemForceUser = item.As<ForceUser>();
            Rotatable itemRotatable = item.As<Rotatable>();
            TorqueUser itemTorqueUser = item.As<TorqueUser>();

            // TRANSLATION

            // Transform accumulated force into acceleration
            if (itemForceUser != null)
            {
                if (itemForceUser.Mass > 0)
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
                else
                {
                    itemForceUser.Acceleration = Vector3.Zero;
                    itemForceUser.AccumulatedForce = Vector3.Zero;
                }
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
                itemRotatable.AngularPosition.Normalize();
            }
        }
    }
}
