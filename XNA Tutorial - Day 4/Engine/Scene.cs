using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Artificial.XNATutorial
{
    public class Scene
    {
        Dictionary<int, Item> items = new Dictionary<int, Item>();
        Dictionary<Item, int> itemIndices = new Dictionary<Item, int>();
        List<int> indices = new List<int>();
        int lastIndex;

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

        // ADD
        public int Add(Item item)
        {
            lastIndex++;
            items.Add(lastIndex, item);
            itemIndices.Add(item, lastIndex);
            indices.Add(lastIndex);
            RaiseOnItemAdd(item);
            return lastIndex;
        }

        // REMOVE
        public void Remove(int index)
        {
            Item item = items[index];
            items.Remove(index);
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

        // SAVE
        public void Save(string path)
        {
            FileStream file = new FileStream(path, FileMode.Create);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(file, items);
            formatter.Serialize(file, indices);
            formatter.Serialize(file, lastIndex);
            file.Close();
        }

        // LOAD
        public void Load(string path)
        {
            FileStream file = new FileStream(path, FileMode.Open);
            BinaryFormatter formatter = new BinaryFormatter();
            Clear();
            items = (Dictionary<int, Item>)formatter.Deserialize(file);
            indices = (List<int>)formatter.Deserialize(file);
            lastIndex = (int)formatter.Deserialize(file);
            file.Close();
            // raise events
            foreach (Item item in items.Values)
            {
                RaiseOnItemAdd(item);
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
