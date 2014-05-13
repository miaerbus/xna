using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Artificial.XNATutorial;

namespace Artificial.XNATutorial.Physics
{
    public class SimpleSceneIterator : Part, ISceneIterator
    {
        Scene scene;
        int updateOrder;

        public SimpleSceneIterator(int updateOrder)
        {
            this.updateOrder = updateOrder;
        }

        public override void Install(Item parent)
        {
            base.Install(parent);
            parent.As<Simulator>().RegisterUpdateMethod(updateOrder, IterateScene);
        }

        // ITEM SIMULATION PARTS
        List<int> simulateItemMethodOrders = new List<int>();
        List<UpdateItemMethod> simulateItemMethods = new List<UpdateItemMethod>();
        public void RegisterUpdateItemMethod(int updateOrder, UpdateItemMethod updateItemMethod)
        {
            int index = 0;
            while (index < simulateItemMethodOrders.Count && simulateItemMethodOrders[index] < updateOrder) index++;
            simulateItemMethodOrders.Insert(index, updateOrder);
            simulateItemMethods.Insert(index, updateItemMethod);
        }

        // ITERATE SCENE
        public void IterateScene(float dt)
        {
            scene = parent.Part<Scene>();

            List<int> indices = scene.GetItemIndices();
            int lastIndex = -1;
            int currentIndex = -1;

            for (int i = 0; i < indices.Count; i++)
            {
                currentIndex = indices[i];
                Item sceneItem = scene[currentIndex];

                for (int j = 0; j < simulateItemMethods.Count; j++)
                {
                    simulateItemMethods[j](dt, sceneItem);
                    if (!scene.HasIndex(currentIndex)) j = simulateItemMethods.Count;
                }

                if (!scene.HasIndex(currentIndex))
                {
                    if (scene.HasIndex(lastIndex))
                    {
                        if (i > indices.Count) i = indices.Count;

                        do
                        {
                            i--;
                            currentIndex = indices[i];
                        } while (lastIndex > -1 && indices[i] != lastIndex);
                    }
                    else
                    {
                        i = -1;
                        currentIndex = -1;
                    }
                }

                lastIndex = currentIndex;

            }
        }
    }
}
