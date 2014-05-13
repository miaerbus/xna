using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Artificial.XNATutorial;
using System.Threading;

namespace Artificial.XNATutorial.Physics
{
    public class MultithreadedSceneIterator : Part, ISceneIterator
    {
        Scene scene;
        int updateOrder;
        IEnumerator<KeyValuePair<int, UpdateItemMethod>> enumerator;

        int numThreads;
        Thread[] threads;
        int[] firstIndex;
        int[] lastIndex;
        int[] indices;
        float dt;


        public MultithreadedSceneIterator(int updateOrder, int numThreads)
        {
            this.numThreads = numThreads;
            threads = new Thread[numThreads];
            firstIndex = new int[numThreads];
            lastIndex = new int[numThreads];
            this.updateOrder = updateOrder;
            enumerator = simulateItemMethods.GetEnumerator();
        }

        int HardwareThread(int id)
        {
            int result = id % 3 + 3;
            if (result == 2) result = 1;
            return result;
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
            this.dt = dt;
            scene = parent.Part<Scene>();
            indices = scene.GetItemIndices().ToArray();
            int n = indices.Length;
            for (int i = 0; i < numThreads; i++)
            {
                threads[i] = new Thread(IteratePartOfScene);
                threads[i].Name = i.ToString();
                firstIndex[i] = n / numThreads * i;
                lastIndex[i] = (n / numThreads * (i + 1)) - 1;
                if (i == numThreads - 1) lastIndex[i] = n - 1;
                threads[i].Start();               
            }
            for (int i = 0; i < numThreads; i++)
            {
                threads[i].Join();
            }
          
        }

        void IteratePartOfScene()
        {
            int id = int.Parse(Thread.CurrentThread.Name);
#if XBOX
            Thread.CurrentThread.SetProcessorAffinity(new int[] { HardwareThread(id) });
#endif
            IEnumerator<KeyValuePair<int, UpdateItemMethod>> enumerator = simulateItemMethods.GetEnumerator();
            int last = lastIndex[id];
            int currentIndex;

            for (int i = firstIndex[id]; i <= lastIndex[id]; i++)
            {
                currentIndex = indices[i];
                if (scene.HasIndex(currentIndex))
                {
                    Item sceneItem = scene[currentIndex];

                    enumerator.Reset();
                    enumerator.MoveNext();
                    while (enumerator.Current.Value != null && scene.HasIndex(currentIndex))
                    {
                        // Call simulation method
                        enumerator.Current.Value(dt, sceneItem);
                        // iterate
                        enumerator.MoveNext();
                    }
                }
            }
        }
    }
}
