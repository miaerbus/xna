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
        IEnumerator<KeyValuePair<int, UpdateMethod>> enumerator;

        // CONSTRUCTOR
        public Simulator(Scene scene)
        {
            Aggregate(scene);
            Require<SceneABT>();
            enumerator = updateMethods.GetEnumerator();
        }
        
        // MAIN SIMULATION METHODS
        SortedList<int, UpdateMethod> updateMethods = new SortedList<int, UpdateMethod>();
        public void RegisterUpdateMethod(int updateOrder, UpdateMethod updateMethod)
        {
            updateMethods.Add(updateOrder, updateMethod);
            enumerator = updateMethods.GetEnumerator();
        }

        // SIMULATE
        public void Simulate(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (dt == 0) return;

            enumerator.Reset();
            enumerator.MoveNext();
            while (enumerator.Current.Value != null)
            {
                // Call simulation method
                enumerator.Current.Value(dt);
                // iterate
                enumerator.MoveNext();
            }
        }
    }
}
