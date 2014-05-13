using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Artificial.XNATutorial.CarGame
{
    class Scene<T>
    {
        List<T> items = new List<T>();

        public void Add(T item)
        {
            items.Add(item);
            RaiseOnItemAdd(item);
        }

        public class SceneManipulationEventArgs<U> : EventArgs
        {
            public U Item;
            public SceneManipulationEventArgs(U item)
            {
                Item = item;
            }

        }

        public event EventHandler<SceneManipulationEventArgs<T>> OnItemAdd;
        private void RaiseOnItemAdd(T item)
        {
            EventHandler<SceneManipulationEventArgs<T>> e = OnItemAdd;
            if (e != null) e(this, new SceneManipulationEventArgs<T>(item));
        }
    }
}
