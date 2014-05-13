using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Artificial.XNATutorial;
using Artificial.XNATutorial.Physics;

namespace Artificial.XNATutorial.Bomberman
{
    class Building : Item
    {
        private BuildingType type;
        public BuildingType Type
        {
            get
            {
                return type;
            }
            set
            {
                type = value;
            }
        }

        bool destructible;
        bool collidable;

        public Building(BuildingType type, Vector3 position, Quaternion rotation)
        {
            this.type = type;
            int t = (int)type;
            if (t < 100)
            {
                destructible = false;
                collidable = true;
            }
            else if (t < 200)
            {
                destructible = false;
                collidable = false;
            }
            else if (t < 300)
            {
                destructible = true;
                collidable = true;
            }

            Require<PPositionWithEvents>().Position = position;
            Require<PAngularPosition>().AngularPosition = rotation;
            Require<PWorldMatrix>();
            Require<PBoundingBox>();
            Require<PBoundingSphere>();

            if (collidable)
            {
                Require<ConvexCollider>();
                ConvexPolygon convex = new ConvexPolygon();
                float h = Gameplay.GridSize * 0.5f;
                convex.Points.Add(new Vector3(-h, h, -h));
                convex.Points.Add(new Vector3(h, h, -h));
                convex.Points.Add(new Vector3(h, h, h));
                convex.Points.Add(new Vector3(-h, h, h));
                convex.Normal = Vector3.Up;
                convex.CalculatePlanes();
                Part<PConvex>().Convex = convex;
                Part<PBoundingBox>().BoundingBox = new BoundingBox(new Vector3(-h, 0, -h), new Vector3(h, h * 2, h));
                Part<PBoundingSphere>().BoundingSphere = new BoundingSphere(Vector3.Zero, 3 * h);
            }

            if (destructible)
            {
                CustomCollider collider = Require<CustomCollider>();
                collider.CollisionMethod += Collide;
            }
        }

        void Collide(float elapsedSeconds, Item collidingItem, Vector3 impactPoint)
        {
            if (collidingItem.Is<Explosion>())
            {
                Gameplay.Level.Scene.Remove(this);
            }
        }

    }
}
