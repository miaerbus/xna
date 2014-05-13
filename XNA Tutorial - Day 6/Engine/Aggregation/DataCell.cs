using System;
using System.Collections.Generic;
using System.Text;

namespace Artificial.XNATutorial
{
    public abstract class DataCell : IInstallable
    {
        // Parent that this item is a part of
        protected Item parent;

        // Install this data cell into an item 
        public virtual void Install(Item parent)
        {
            this.parent = parent;
        }

        public virtual void Uninstall()
        {
        }
    }
}
