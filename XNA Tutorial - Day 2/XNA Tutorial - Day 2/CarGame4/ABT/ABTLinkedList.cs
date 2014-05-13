using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Artificial.XNATutorial.CarGame
{
    // This is just an enhanced LinkedList that knows which ABT node is it's parent.
    // We need this information because updating starts as an event of the ABT item, that
    // is inside the linked list and needs to inform it's current node about it.
    class ABTLinkedList : LinkedList<IABTItem>
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
