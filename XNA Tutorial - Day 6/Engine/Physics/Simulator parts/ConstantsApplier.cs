using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Artificial.XNATutorial;

namespace Artificial.XNATutorial.Physics

{
    public class ConstantsApplier : Part
    {
        int constantsApplicationOrder;
        int constantsAccumulationOrder;

        Vector3 constantForce;
        Vector3 constantAcceleration;

        public ConstantsApplier(int constantsAccumulationOrder, int constantsApplicationOrder)
        {
            this.constantsApplicationOrder = constantsApplicationOrder;
            this.constantsAccumulationOrder = constantsAccumulationOrder;
        }

        public override void Install(Item parent)
        {
            base.Install(parent);
            parent.As<ISceneIterator>().RegisterUpdateItemMethod(constantsApplicationOrder, Application);
            parent.As<Simulator>().RegisterUpdateMethod(constantsAccumulationOrder, Accumulation);
        }

        // ACCUMULATION
        void Accumulation(float dt)
        {
            Scene scene = parent.Part<Scene>();
            List<int> indices = scene.GetItemIndices();
            constantForce = Vector3.Zero;
            constantAcceleration = Vector3.Zero;
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
        }

        // APPLICATION
        void Application(float dt, Item item)
        {
            ForceUser itemForceUser = item.As<ForceUser>();
            if (itemForceUser != null)
            {
                itemForceUser.AccumulatedForce += constantForce;
                itemForceUser.AccumulatedForce += constantAcceleration * itemForceUser.Mass;
            }
        }
    }
}
