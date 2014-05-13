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
    class Scene
    {
        Dictionary<int, object> items = new Dictionary<int, object>();
        List<int> indices = new List<int>();
        int lastIndex;

        // INDICES
        public List<int> GetItemIndices()
        {
            return indices;
        }

        // ITEM ACCESS
        public object this[int index]
        {
            get
            {
                return items[index];
            }
        }

        // ADD
        public int Add(object item)
        {
            lastIndex++;
            items.Add(lastIndex, item);
            indices.Add(lastIndex);
            RaiseOnItemAdd(item);
            return lastIndex;
        }

        // REMOVE
        public void Remove(int index)
        {
            object item = items[index];
            items.Remove(index);
            indices.Remove(index);
            RaiseOnItemRemove(item);
        }

        // CLEAR
        public void Clear()
        {
            for (int i = 0; i < indices.Count; i++)
            {
                Remove(indices[i]);
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
            items = (Dictionary<int, object>)formatter.Deserialize(file);
            indices = (List<int>)formatter.Deserialize(file);
            lastIndex = (int)formatter.Deserialize(file);
            file.Close();
            // raise events
            foreach (object item in items.Values)
            {
                RaiseOnItemAdd(item);
            }
        }

        // EVENTS
        public class SceneManipulationEventArgs : EventArgs
        {
            public object Item;
        }
        SceneManipulationEventArgs SceneManipulationArgs = new SceneManipulationEventArgs();

        // OnItemAdd
        public event EventHandler<SceneManipulationEventArgs> OnItemAdd;
        private void RaiseOnItemAdd(object item)
        {
            if (OnItemAdd != null)
            {
                SceneManipulationArgs.Item = item;
                OnItemAdd(this, SceneManipulationArgs);
            }
        }

        // OnItemRemove
        public event EventHandler<SceneManipulationEventArgs> OnItemRemove;
        private void RaiseOnItemRemove(object item)
        {
            if (OnItemRemove != null)
            {
                SceneManipulationArgs.Item = item;
                OnItemRemove(this, SceneManipulationArgs);
            }
        }
    }
}
