using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Artificial.XNATutorial.CarGame
{
    class ABTNode
    {
        // This is a node of our ABT and where most of the code is. Some of the fields are exposed
        // as public for investigating the construction of the ABT. Not having readonly properties
        // in this case is OK because the outside user of the ABT can only get a copy of the entire
        // tree and not the actual objects.

        // Max items in node
        private int maxItemsInNode;

        // Volume containing all the items
        public BoundingBox volume;

        // Bounds of this node
        public BoundingBox bounds;

        // Partition plane
        public Plane partition;

        // Parent
        private ABTNode parent;

        // Two children
        public ABTNode[] child = new ABTNode[2];

        // Is leaf
        public bool isLeaf = true;

        // Items in the leaf
        public ABTLinkedList items;

        private int equaldistribution = 0;

        // CONSTRUCTOR
        public ABTNode(int maxItemsInNode)
        {
            this.maxItemsInNode = maxItemsInNode;
            items = new ABTLinkedList(this);
        }

        // ROOT CONSTRUCTOR
        public static ABTNode CreateRoot(int maxItemsInNode)
        {
            ABTNode node = new ABTNode(maxItemsInNode);
            // The root fills the whole 3D space
            node.bounds.Min = Vector3.One * float.NegativeInfinity;
            node.bounds.Max = Vector3.One * float.PositiveInfinity;
            return node;
        }

        // ADD ITEM
        public void Add(LinkedListNode<IABTItem> item)
        {
            // Adapt volume
            if (isLeaf && items.Count == 0)
            {
                // If this is the first item in this node, just use it's volume
                volume.Min = item.Value.Position + item.Value.BoundingBox.Min;
                volume.Max = item.Value.Position + item.Value.BoundingBox.Max;
            }
            else
            {
                // Do a union of the new item with the current volume
                updateVolume(item);
            }

            // Are we already in a leaf?
            if (isLeaf)
            {
                // Yes, insert item in this node
                items.AddLast(item);

                // If maximum capacity is reached, subdivide into two child leaves
                if (items.Count == maxItemsInNode)
                {
                    subdivide();
                }
            }
            else
            {
                // No, we need to go deeper into the appropriate child
                
                // Calcualte on which side of the partition is this item
                float d = Vector3.Dot(item.Value.Position, partition.Normal); // Project position onto
                                                                              // partition normal
                // Insert into appropriate child
                if (d > partition.D || // object is on the positive side of partition
                    (d == partition.D && // if object is on the partition
                    ++equaldistribution % 2 == 0)) // distribute evenly between both children
                {
                    child[0].Add(item);
                }
                else
                {
                    child[1].Add(item);
                }
            }
        }

        // UPDATE VOLUME
        private void updateVolume(LinkedListNode<IABTItem> item)
        {
            volume.Min = Vector3.Min(volume.Min, item.Value.Position + item.Value.BoundingBox.Min);
            volume.Max = Vector3.Max(volume.Max, item.Value.Position + item.Value.BoundingBox.Max);
        }

        // UPDATE VOLUME RECURSIVELY TO THE ROOT
        private void UpdateVolumeRecursively(LinkedListNode<IABTItem> item)
        {
            // Update myself
            updateVolume(item);            
            if (parent != null)
            {
                // Update my parents
                parent.UpdateVolumeRecursively(item);
            }
        }


        // GET ITEMS INSIDE THE FRUSTUM
        public void GetItems(BoundingFrustum frustum, List<IABTItem> itemlist)
        {
            if (isLeaf)
            {
                // this node is inside the bounds so add all items
                itemlist.AddRange(items);
            }
            else
            {
                // check if first child's volume is inside the frustum
                if (child[0].volume.Intersects(frustum))
                {
                    child[0].GetItems(frustum, itemlist);
                }
                // check if second child's volume is inside the frustum
                if (child[1].volume.Intersects(frustum))
                {
                    child[1].GetItems(frustum, itemlist);
                }
            }
        }

        // SUBDIVIDE LEAF INTO TWO CHILDREN
        private void subdivide()
        {
            // Create childs and initialize them into leaves
            child[0] = new ABTNode(maxItemsInNode);
            child[1] = new ABTNode(maxItemsInNode);

            child[0].bounds = bounds;
            child[1].bounds = bounds;

            child[0].parent = this;
            child[1].parent = this;

            // Determine partition plane
            // We try to divide along the longest dimension
            int direction = 0;
            float maxdimension = volume.Max.X - volume.Min.X;
            float dimensionY = volume.Max.Y - volume.Min.Y;
            float dimensionZ = volume.Max.Z - volume.Min.Z;
            if (dimensionY > maxdimension)
            {
                direction = 1;
                maxdimension = dimensionY;
            }
            if (dimensionZ > maxdimension)
            {
                direction = 2;
            }
            
            // Calculate the arithemtic middle along the longest dimension
            float middle = 0f;
            LinkedListNode<IABTItem> node = items.First;
            float fraction = 1f / items.Count;
            switch (direction)
            {
                case 0:
                    partition.Normal = Vector3.UnitX;
                    do
                    {
                        middle += node.Value.Position.X * fraction;
                        node = node.Next;
                    } while (node != null);
                    child[0].bounds.Min.X = middle;
                    child[1].bounds.Max.X = middle;
                    break;
                case 1:
                    partition.Normal = Vector3.UnitY;
                    do
                    {
                        middle += node.Value.Position.Y * fraction;
                        node = node.Next;
                    } while (node != null);
                    child[0].bounds.Min.Y = middle;
                    child[1].bounds.Max.Y = middle;
                    break;
                case 2:
                    partition.Normal = Vector3.UnitZ;
                    do
                    {
                        middle += node.Value.Position.Z * fraction;
                        node = node.Next;
                    } while (node != null);
                    child[0].bounds.Min.Z = middle;
                    child[1].bounds.Max.Z = middle;
                    break;
            }

            // Set this middle as the new partition
            partition.D = middle;
            isLeaf = false;

            // Reinsert items
            node = items.First;
            do
            {
                LinkedListNode<IABTItem> next = node.Next;
                items.Remove(node);
                Add(node);
                node = next;
            } while (node != null);

            // Delete this linked list
            items = null;
        }

        // UPDATE NODE IN RELEVANCE TO AN UPDATED ITEM
        public void Update(LinkedListNode<IABTItem> item)
        {
            if (bounds.Contains(item.Value.Position) == ContainmentType.Contains)
            {
                // We are still inside this node, just update the volume
                UpdateVolumeRecursively(item);
            }
            else
            {
                // Reposition to another ABT node
#if DEBUG
                Globals.ABTNodesRepositioned++;
#endif
                items.Remove(item);
                parent.Reposition(item);
            }
        }

        // REPOSITION ITEM INTO CORRECT NODE
        private void Reposition(LinkedListNode<IABTItem> item)
        {
            if (bounds.Contains(item.Value.Position) == ContainmentType.Contains)
            {
                // We are in the right branch, so we can readd this node
                Add(item);
            }
            else
            {
                // Climb the tree until we get to the right branch
                parent.Reposition(item);
            }
        }
    }
}
