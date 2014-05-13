using System;
using System.Collections.Generic;
using System.Text;

namespace Artificial.XNATutorial
{
    public abstract class Part : Item, IInstallable
    {
        // Parent that this item is a part of
        protected Item parent;

        // Install this part into an item 
        public virtual void Install(Item parent)
        {
            this.parent = parent;
        }
    }
}
