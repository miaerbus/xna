using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Artificial.XNATutorial
{
    public class ABTNode
    {
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
            node.bounds.Min = Vector3.One * float.NegativeInfinity;
            node.bounds.Max = Vector3.One * float.PositiveInfinity;
            return node;
        }

        // ADD ITEM
        public void Add(LinkedListNode<IABTItem> item)
        {
            // adapt volume
            if (isLeaf && items.Count == 0)
            {
                volume.Min = item.Value.Position + item.Value.BoundingBox.Min;
                volume.Max = item.Value.Position + item.Value.BoundingBox.Max;
            }
            else
            {
                updateVolume(item);
            }

            // are we already in a leaf?
            if (isLeaf)
            {
                // insert item in this node
                items.AddLast(item);

                // if maximum capacity is reached, subdivide into two child leaves
                if (items.Count > maxItemsInNode)
                {
                    subdivide();
                }
            }
            else
            {
                // add into the appropriate child

                // project position onto partition normal
                float d = Vector3.Dot(item.Value.Position, partition.Normal);
                if (d > partition.D || // object is on the positive side of partition
                    (d == partition.D && // object is on the partition
                    ++equaldistribution % 2 == 0)) // first child has less items then the second
                {
                    child[0].Add(item);
                }
                else
                {
                    child[1].Add(item);
                }
            }
        }

        public void AddWithoutSubdivide(LinkedListNode<IABTItem> item)
        {
            // adapt volume
            if (isLeaf && items.Count == 0)
            {
                volume.Min = item.Value.Position + item.Value.BoundingBox.Min;
                volume.Max = item.Value.Position + item.Value.BoundingBox.Max;
            }
            else
            {
                updateVolume(item);
            }

            // are we already in a leaf?
            if (isLeaf)
            {
                // insert item in this node
                items.AddLast(item);
            }
            else
            {
                // add into the appropriate child

                // project position onto partition normal
                float d = Vector3.Dot(item.Value.Position, partition.Normal);
                if (d > partition.D || // object is on the positive side of partition
                    (d == partition.D && // object is on the partition
                    ++equaldistribution % 2 == 0)) // first child has less items then the second
                {
                    child[0].AddWithoutSubdivide(item);
                }
                else
                {
                    child[1].AddWithoutSubdivide(item);
                }
            }
        }

        public void ForceSubdivide()
        {
            // are we already in a leaf?
            if (isLeaf)
            {
                // if maximum capacity is reached, subdivide into two child leaves
                if (items.Count > maxItemsInNode)
                {
                    subdivide();
                }
            }
            else
            {
                child[0].ForceSubdivide();
                child[1].ForceSubdivide();
            }
        }


        // REMOVE ITEM
        public void Remove(LinkedListNode<IABTItem> item)
        {
            items.Remove(item);
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


        // GET ITEMS
        public void GetItems(ref BoundingFrustum frustum, List<ABTLinkedList> itemlist)
        {
            if (isLeaf)
            {
                // this node is inside the bounds so add all items
                itemlist.Add(items);
            }
            else
            {
                // check if first child's volume is inside the frustum
                bool result;
                frustum.Intersects(ref child[0].volume, out result);
                if (result)
                {
                    child[0].GetItems(ref frustum, itemlist);
                }
                // check if second child's volume is inside the frustum
                frustum.Intersects(ref child[1].volume, out result);
                if (result)
                {
                    child[1].GetItems(ref frustum, itemlist);
                }
            }
        }

        public void GetItems(ref BoundingSphere sphere, List<ABTLinkedList> itemlist)
        {
            if (isLeaf)
            {
                // this node is inside the bounds so add all items
                itemlist.Add(items);
            }
            else
            {
                // check if first child's volume is inside the frustum
                bool result;
                sphere.Intersects(ref child[0].volume, out result);
                if (result)
                {
                    child[0].GetItems(ref sphere, itemlist);
                }
                // check if second child's volume is inside the frustum
                sphere.Intersects(ref child[1].volume, out result);
                if (result)
                {
                    child[1].GetItems(ref sphere, itemlist);
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

        // GET A TRAVERSABLE COPY
        public ABTNode GetCopy()
        {
            ABTNode node = new ABTNode(maxItemsInNode);
            node.volume = volume;
            node.bounds = bounds;
            node.isLeaf = isLeaf;
            node.items = items;
            node.partition = partition;
            if (!isLeaf)
            {
                // Create childs and initialize them into leaves
                node.child[0] = child[0].GetCopy();
                node.child[1] = child[1].GetCopy();
            }
            return node;
        }

    }
}
