using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Artificial.XNATutorial.CarGame
{
    class Scene<T>
    {
        Dictionary<int,T> items = new Dictionary<int,T>();
        List<int> indices = new List<int>();
        int lastIndex;

        // INDICES
        public int[] GetItemIndices()
        {
            return indices.ToArray();
        }

        // ITEM ACCESS
        public T this[int index]
        {
            get
            {
                return items[index];
            }
        }

        // ADD
        public int Add(T item)
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
            T item = items[index];
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
            items = (Dictionary<int,T>)formatter.Deserialize(file);
            indices = (List<int>)formatter.Deserialize(file);
            lastIndex = (int)formatter.Deserialize(file);
            file.Close();
            // raise events
            foreach (T item in items.Values)
            {
                RaiseOnItemAdd(item);
            }
        }

        // EVENTS
        public class SceneManipulationEventArgs<U> : EventArgs
        {
            public U Item;
            public SceneManipulationEventArgs(U item)
            {
                Item = item;
            }
        }

        // OnItemAdd
        public event EventHandler<SceneManipulationEventArgs<T>> OnItemAdd;
        private void RaiseOnItemAdd(T item)
        {
            EventHandler<SceneManipulationEventArgs<T>> e = OnItemAdd;
            if (e != null) e(this, new SceneManipulationEventArgs<T>(item));
        }

        // OnItemRemove
        public event EventHandler<SceneManipulationEventArgs<T>> OnItemRemove;
        private void RaiseOnItemRemove(T item)
        {
            EventHandler<SceneManipulationEventArgs<T>> e = OnItemRemove;
            if (e != null) e(this, new SceneManipulationEventArgs<T>(item));
        }
    }
}
