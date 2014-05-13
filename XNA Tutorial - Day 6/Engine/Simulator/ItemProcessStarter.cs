using System;
using System.Collections.Generic;
using System.Text;

namespace Artificial.XNATutorial
{
    public class ItemProcessStarter : Part
    {
        int updateOrder;

        public ItemProcessStarter(int updateOrder)
        {
            this.updateOrder = updateOrder;
        }

        public override void Install(Item parent)
        {
            base.Install(parent);
            parent.As<ISceneIterator>().RegisterUpdateItemMethod(updateOrder, StartItemProcess);
        }

        void StartItemProcess(float dt, Item item)
        {
            ItemProcess i = item.As<ItemProcess>();
            if (i != null && i.Process != null)
            {
                i.Process(dt);
            }
        }
    }
}
