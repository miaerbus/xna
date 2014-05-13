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
        IEnumerator<KeyValuePair<int, UpdateItemMethod>> enumerator;

        public SimpleSceneIterator(int updateOrder)
        {
            this.updateOrder = updateOrder;
            enumerator = simulateItemMethods.GetEnumerator();
        }

        public override void Install(Item parent)
        {
            base.Install(parent);
            parent.As<Simulator>().RegisterUpdateMethod(updateOrder, IterateScene);

            scene = parent.As<Scene>();
        }

        // ITEM SIMULATION PARTS
        SortedList<int, UpdateItemMethod> simulateItemMethods = new SortedList<int, UpdateItemMethod>();
        public void RegisterUpdateItemMethod(int updateOrder, UpdateItemMethod updateItemMethod)
        {
            simulateItemMethods.Add(updateOrder, updateItemMethod);
            enumerator = simulateItemMethods.GetEnumerator();
        }

        // ITERATE SCENE
        public void IterateScene(float dt)
        {
            List<int> indices = scene.GetItemIndices();

            for (int i = 0; i < indices.Count; i++)
            {
                Item sceneItem = scene[indices[i]];

                enumerator.Reset();
                enumerator.MoveNext();
                while (enumerator.Current.Value != null)
                {
                    // Call simulation method
                    enumerator.Current.Value(dt,sceneItem);
                    // iterate
                    enumerator.MoveNext();
                }
            }
        }
    }
}
