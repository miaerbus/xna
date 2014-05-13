using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Artificial.XNATutorial
{
    public class ABTLinkedList : LinkedList<IABTItem>
    {
        private ABTNode parent;
        public ABTNode Parent
        {
            get
            {
                return parent;
            }
        }

        public ABTLinkedList(ABTNode parent)
        {
            this.parent = parent;
        }
    }
}
