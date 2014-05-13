using System;
using System.Collections.Generic;
using System.Text;

namespace Artificial.XNATutorial.Bomberman
{
    class State : Part
    {
        public bool Finished;
        public Bomberman.States NextState = Bomberman.States.None;

        public Item Parent
        {
            get
            {
                return parent;
            }
        }

        public override void Install(Item parent)
        {
            base.Install(parent);
        }
    }
}
