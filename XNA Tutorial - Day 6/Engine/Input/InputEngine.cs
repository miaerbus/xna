using System;
using System.Collections.Generic;
using System.Text;
using Artificial.XNATutorial;

namespace Artificial.XNATutorial.Input
{
    public class InputEngine : Part
    {
        int updateOrder;

        public InputEngine(int updateOrder)
        {
            this.updateOrder = updateOrder;
        }

        public override void Install(Item parent)
        {
            base.Install(parent);
            parent.As<ISceneIterator>().RegisterUpdateMethod(updateOrder, ProcessInput);
        }

        void ProcessInput(float dt, Item item)
        {
            if (item.Has<InputMap>())
            {
                item.Part<InputMap>().Process(dt);
            }
        }
    }
}
