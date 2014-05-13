using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Artificial.XNATutorial;

namespace Artificial.XNATutorial.Physics
{
    public class CollisionDetector : Part
    {
        int updateOrder;
        List<ABTLinkedList> filteredItems = new List<ABTLinkedList>();
        AdaptiveBinaryTree sceneABT;
        IEnumerator<KeyValuePair<int, UpdateItemPairMethod>> enumerator;

        // CONSTRUCTOR
        public CollisionDetector(int updateOrder)
        {
            this.updateOrder = updateOrder;
            enumerator = collisionMethods.GetEnumerator();
        }

        // INSTALL
        public override void Install(Item parent)
        {
            base.Install(parent);
            parent.As<ISceneIterator>().RegisterUpdateItemMethod(updateOrder, DetectCollisions);
            sceneABT = parent.As<SceneABT>().ABT;
        }

        // COLLISION METHODS
        SortedList<int, UpdateItemPairMethod> collisionMethods = new SortedList<int, UpdateItemPairMethod>();
        public void RegisterCollisionMethod(int updateOrder, UpdateItemPairMethod collisionMethod)
        {
            collisionMethods.Add(updateOrder, collisionMethod);
            enumerator = collisionMethods.GetEnumerator();
        }       

        // DETECT COLLISIONS
        void DetectCollisions(float dt, Item item)
        {
            PBoundingSphere sphere = item.As<PBoundingSphere>();
            Vector3 offset = Vector3.Zero;
            if (item.Is<PPosition>()) offset = item.Part<PPosition>().Position;

            BoundingSphere boundingSphere = new BoundingSphere();
            if (sphere != null)
            {
                boundingSphere = sphere.BoundingSphere;
                boundingSphere.Center += offset;
            }
            else
            {
                boundingSphere.Radius = float.PositiveInfinity;
            }

            // Use ABT to get potential colliders
            LinkedListNode<IABTItem> node;

            sceneABT.GetItems(boundingSphere, filteredItems);
            // Iterate through all filtered ABT leaves
            for (int j = 0; j < filteredItems.Count; j++)
            {
                // Iterate through items in current ABT leaf
                node = filteredItems[j].First;
                while (node != null)
                {
                    // Get affected item
                    Item item2 = (node.Value as CustomBoundingBoxABTItem).PositionPart;
                    if (item2 != item)
                    {
                        enumerator.Reset();
                        enumerator.MoveNext();
                        while (enumerator.Current.Value != null)
                        {
                            // Call collision method
                            enumerator.Current.Value(dt, item, item2);
                            // iterate
                            enumerator.MoveNext();
                        }

                    }
                    // Go to next item in this leaf
                    node = node.Next;
                }
            }

        }
    }
}
