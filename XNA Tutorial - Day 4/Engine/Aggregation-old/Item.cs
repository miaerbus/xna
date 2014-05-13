using System;
using System.Collections.Generic;
using System.Text;

namespace Artificial.XNATutorial
{
    public class Item
    {
        // Dictionary of parts that this item is created from, referenced by type
        Dictionary<Type, object> parts = new Dictionary<Type, object>();

        // Aggregate a new part into this item
        public void Aggregate(object part)
        {
            IInstallable n = part as IInstallable;
            if (n != null) n.Install(this);
            Type t = part.GetType();
            parts.Add(t, part);
            Type[] interfaces = t.GetInterfaces();
            for (int i = 0; i < interfaces.Length; i++)
            {
                if (!parts.ContainsKey(interfaces[i]))
                {
                    parts.Add(interfaces[i], part);
                }
            }            
        }

        // Remove a part from this item
        public void Remove(object part)
        {
            Type t = part.GetType();
            parts.Remove(t);            
            Type[] interfaces = t.GetInterfaces();
            for (int i = 0; i < interfaces.Length; i++)
            {
                if (!parts.ContainsKey(interfaces[i]))
                {
                    parts.Remove(interfaces[i]);
                }
            }
        }

        // Cast this item as one of its parts
        public T As<T>() where T : class
        {
            Type t = typeof(T);
            if (parts.ContainsKey(t))
            {
                return parts[t] as T;
            }
            else
            {
                T self = this as T;
                return self;
            }
        }

        // Get one of the parts without safe checking
        public T Part<T>() where T : class
        {
            return parts[typeof(T)] as T;
        }

        // Create a new part of required class if it doesn't exist and return the part that meets the requirements
        public T Require<T>() where T : class, new()
        {
            T part = As<T>();
            if (part == null)
            {
                part = new T();
                Aggregate(part);
            }
            return part;
        }
    }
}
