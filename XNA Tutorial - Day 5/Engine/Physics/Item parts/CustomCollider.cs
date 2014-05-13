using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Artificial.XNATutorial.Physics
{
    public delegate void CustomCollisionMethod(float elapsedSeconds, Item collidingItem, Vector3 impactPoint);

    public class CustomCollider : Part
    {
        CustomCollisionMethod collisionMethod;
        public CustomCollisionMethod CollisionMethod
        {
            get
            {
                return collisionMethod;
            }
            set
            {
                collisionMethod = value;
            }
        }

        public List<Type> OverrideCollisionWithType = new List<Type>();
        public List<Item> OverrideCollisionWithItem = new List<Item>();
    }
}
