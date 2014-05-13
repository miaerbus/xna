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
    class Door : Item
    {
        float turnSpeed = 2f;
        Quaternion open, closed;
        Quaternion targetRotation;
        PAngularPosition angularPosition;
        bool isOpen;

        public Door(Vector3 position, Quaternion open, Quaternion closed)
        {
            this.open = open;
            this.closed = closed;

            Require<PRotationMatrix>();
            Require<PPositionWithEvents>().Position = position;
            angularPosition =Require<PAngularPosition>();
            angularPosition.AngularPosition = closed;
            targetRotation = closed;
            Require<PWorldMatrix>();
            Require<PBoundingSphere>();

            // Collider
            Require<ConvexCollider>();
            ConvexPolygon convex = new ConvexPolygon();
            float h = Bomberman.GridSize * 0.5f;
            float f = -Bomberman.GridSize * 0.1f;
            float g = Bomberman.GridSize;
            convex.Points.Add(new Vector3(0, h, 0));
            convex.Points.Add(new Vector3(0, h, f));
            convex.Points.Add(new Vector3(g, h, f));
            convex.Points.Add(new Vector3(g, h, 0));
            convex.Normal = Vector3.Up;
            convex.CalculatePlanes();
            Part<PConvex>().Convex = convex;
            Part<PBoundingSphere>().BoundingSphere = new BoundingSphere(new Vector3(0, h * 0.7f, 0), g * 1.1f);

            // Update
            Require<ItemProcess>().Process = update;

            // Collider
            CustomCollider collider = Require<CustomCollider>();
            collider.CollisionMethod += Collide;
            
        }

        void update(float dt)
        {
            angularPosition.AngularPosition = Quaternion.Lerp(angularPosition.AngularPosition, targetRotation, turnSpeed * dt);
        }

        void Collide(float elapsedSeconds, Item collidingItem, Vector3 impactPoint)
        {
            if (collidingItem.Is<Character>())
            {
                //Open();
            }
        }
        public void OpenOrClose()
        {
            if (isOpen)
            {
                Close();
            }
            else
            {
                Open();
            }
        }

        public void Open()
        {
            isOpen = true;
            targetRotation = open;
        }

        public void Close()
        {
            isOpen = false;
            targetRotation = closed;
        }

    }
}
