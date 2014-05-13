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
            scene = parent.Part<Scene>();

            List<int> indices = scene.GetItemIndices();
            int lastIndex = -1;
            int currentIndex = -1;

            for (int i = 0; i < indices.Count; i++)
            {
                currentIndex = indices[i];
                Item sceneItem = scene[currentIndex];

                enumerator.Reset();
                enumerator.MoveNext();
                while (enumerator.Current.Value != null && scene.HasIndex(currentIndex))
                {                    
                    // Call simulation method
                    enumerator.Current.Value(dt,sceneItem);
                    // iterate
                    enumerator.MoveNext();                    
                }

                if (!scene.HasIndex(currentIndex))
                {
                    if (i>indices.Count) i = indices.Count;

                    do
                    {
                        i--;
                        currentIndex = indices[i];
                    } while (lastIndex > -1 && indices[i] != lastIndex);
                }

                lastIndex = currentIndex;

            }
        }
    }
}
