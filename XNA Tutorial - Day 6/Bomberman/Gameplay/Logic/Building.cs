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
        private int type;
        public int Type
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
            this.type = (int)type;
            int t = (int)type;
            if (t < 10)
            {
                destructible = false;
                collidable = true;
            }
            else if (t < 20)
            {
                destructible = false;
                collidable = false;
            }
            else if (t < 30)
            {
                destructible = true;
                collidable = true;
            }
            else if (t < 40)
            {
                destructible = false;
                collidable = false;
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
                float h = Bomberman.GridSize * 0.5f;
                convex.Points.Add(new Vector3(-h, h, -h));
                convex.Points.Add(new Vector3(h, h, -h));
                convex.Points.Add(new Vector3(h, h, h));
                convex.Points.Add(new Vector3(-h, h, h));
                convex.Normal = Vector3.Up;
                convex.CalculatePlanes();
                Part<PConvex>().Convex = convex;
                Part<PBoundingBox>().BoundingBox = new BoundingBox(new Vector3(-h, 0, -h), new Vector3(h, h * 2, h));
                Part<PBoundingSphere>().BoundingSphere = new BoundingSphere(new Vector3(0, h*0.7f, 0), 1.5f * h);
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
                Bomberman.Level.Scene.Remove(this);
            }
        }

    }
}
