using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Artificial.XNATutorial
{
    // UPDATE METHODS
    public delegate void UpdateMethod(float elapsedSeconds);
    public delegate void UpdateItemMethod(float elapsedSeconds, Item item);
    public delegate void UpdateItemPairMethod(float elapsedSeconds, Item item1, Item item2);


    // PHYSICS SIMULATOR
    public class Simulator : Item
    {
        // SCENE
        int abtItemsPerLeaf = 8;

        public Scene Scene
        {
            get
            {
                return Part<Scene>();
            }
            set
            {
                Scene scene = As<Scene>();
                if (scene != null) Remove(scene);
                Aggregate(value);
                Part<SceneABT>().Create(abtItemsPerLeaf);
            }
        }

        // CONSTRUCTOR
        public Simulator()
        {
            Require<SceneABT>();
        }

        public Simulator(int abtItemsPerLeaf)
        {
            this.abtItemsPerLeaf = abtItemsPerLeaf;
            Require<SceneABT>();
        }
        
        // MAIN SIMULATION METHODS
        List<int> updateMethodOrders = new List<int>();
        List<UpdateMethod> updateMethods = new List<UpdateMethod>();
        public void RegisterUpdateMethod(int updateOrder, UpdateMethod updateMethod)
        {
            int index = 0;
            while (index < updateMethodOrders.Count && updateMethodOrders[index] < updateOrder) index++;
            updateMethodOrders.Insert(index, updateOrder);
            updateMethods.Insert(index, updateMethod);
        }

        public void UnregisterUpdateMethod(UpdateMethod updateMethod)
        {
            int index = updateMethods.IndexOf(updateMethod);
            updateMethods.RemoveAt(index);
            updateMethodOrders.RemoveAt(index);
        }


        // SIMULATE
        public void Simulate(float elapsedSeconds)
        {
            float dt = elapsedSeconds;
            if (dt == 0) return;

            for (int i = 0; i < updateMethods.Count; i++)
            {
                updateMethods[i](dt);
            }
        }
    }
}
