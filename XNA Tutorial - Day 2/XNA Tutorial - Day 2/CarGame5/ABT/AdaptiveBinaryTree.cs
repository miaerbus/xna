using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Artificial.XNATutorial.CarGame
{
    class AdaptiveBinaryTree
    {
        // Root
        ABTNode root;

        // Map from items to linked list node containers
        Dictionary<IABTItem, LinkedListNode<IABTItem>> itemMap = new Dictionary<IABTItem, LinkedListNode<IABTItem>>();

        // Cache last size for item count prediction
        int lastsize = 0;

        // CONSTRUCTOR
        public AdaptiveBinaryTree(int maxItemsInNode)
        {
            root = ABTNode.CreateRoot(maxItemsInNode);
        }

        // ADD ITEM
        public void Add(IABTItem item)
        {
            // Handle moving items
            item.OnPositionChanged += ItemPositionChanged;
            // Create item's container
            LinkedListNode<IABTItem> container = new LinkedListNode<IABTItem>(item);
            itemMap.Add(item, container);
            // Add into ABT
            root.Add(container);
        }

        // GET ITEMS INSIDE THE FRUSTUM
        public List<IABTItem> GetItems(BoundingFrustum frustum)
        {
            List<IABTItem> items = new List<IABTItem>((int)(lastsize*1.1f));
            root.GetItems(frustum, items);
            lastsize = items.Count;
            return items;
        }

        // UPDATE ITEM POSITION
        private void ItemPositionChanged(object sender, EventArgs e)
        {
            IABTItem i = sender as IABTItem;
            if (i != null)
            {
                ABTNode parent = (itemMap[i].List as ABTLinkedList).Parent;
                parent.Update(itemMap[i]);
            }
        }

        // GET TRAVERSABLE COPY
        public ABTNode GetCopy()
        {
            return root.GetCopy();
        }

    }
}
