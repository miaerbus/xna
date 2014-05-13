using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Artificial.XNATutorial;

namespace Artificial.XNATutorial.Physics
{
    public class ParticleParticleCollisions : Part
    {
        int updateOrder;
        List<ABTLinkedList> filteredItems = new List<ABTLinkedList>();
        AdaptiveBinaryTree sceneABT;

        // CONSTRUCTOR
        public ParticleParticleCollisions(int updateOrder)
        {
            this.updateOrder = updateOrder;
        }

        // INSTALL
        public override void Install(Item parent)
        {
            base.Install(parent);
            parent.As<CollisionDetector>().RegisterCollisionMethod(updateOrder, DetectCollision);
        }

        // DETECT COLLISION
        void DetectCollision(float dt, Item item1, Item item2)
        {

        }
    }
}
