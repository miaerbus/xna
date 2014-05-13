using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Artificial.XNATutorial
{
    public class Scene
    {
        List<Item> items = new List<Item>();
        Dictionary<Item, int> itemIndices = new Dictionary<Item, int>();
        List<int> indices = new List<int>();
        int lastIndex = -1;

        // INDICES
        public List<int> GetItemIndices()
        {
            return indices;
        }

        // ITEM ACCESS
        public Item this[int index]
        {
            get
            {
                return items[index];
            }
        }

        public bool HasIndex(int index)
        {
            return items[index] != null;
        }

        // ADD
        public int Add(Item item)
        {
            lastIndex++;
            items.Add(item);
            itemIndices.Add(item, lastIndex);
            indices.Add(lastIndex);
            RaiseOnItemAdd(item);
            return lastIndex;
        }

        // REMOVE
        public void Remove(int index)
        {
            Item item = items[index];
            items[index] = null;
            itemIndices.Remove(item);
            indices.Remove(index);
            RaiseOnItemRemove(item);
        }

        public void Remove(Item item)
        {
            if (itemIndices.ContainsKey(item))
            {
                int index = itemIndices[item];
                Remove(index);
            }
        }

        // CLEAR
        public void Clear()
        {
            while (indices.Count > 0)
            {
                Remove(indices[0]);
            }
        }

        // EVENTS
        public class SceneManipulationEventArgs : EventArgs
        {
            public Item Item;
        }
        SceneManipulationEventArgs SceneManipulationArgs = new SceneManipulationEventArgs();

        // OnItemAdd
        public event EventHandler<SceneManipulationEventArgs> OnItemAdd;
        private void RaiseOnItemAdd(Item item)
        {
            if (OnItemAdd != null)
            {
                SceneManipulationArgs.Item = item;
                OnItemAdd(this, SceneManipulationArgs);
            }
        }

        // OnItemRemove
        public event EventHandler<SceneManipulationEventArgs> OnItemRemove;
        private void RaiseOnItemRemove(Item item)
        {
            if (OnItemRemove != null)
            {
                SceneManipulationArgs.Item = item;
                OnItemRemove(this, SceneManipulationArgs);
            }
        }
    }
}
