using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Artificial.XNATutorial;
using Artificial.XNATutorial.Physics;

namespace Artificial.XNATutorial.Bomberman
{
    public delegate void TriggerAction();

    public class Trigger : Item
    {
        TriggerAction target;
        public object Tag;
        public bool AutoActivate;

        public Trigger(Vector3 position, float radious, TriggerAction target, bool autoActivate)
        {
            this.target = target;
            this.AutoActivate = autoActivate;

            Require<PPositionWithEvents>().Position = position;
            Require<PBoundingSphere>().BoundingSphere = new BoundingSphere(Vector3.Zero, radious);
            Require<ParticleCollider>().ParticleRadius = radious;

            CustomCollider collider = Require<CustomCollider>();
            collider.OverrideCollisionWithType.Add(typeof(Building));
            collider.OverrideCollisionWithType.Add(typeof(Character));
            collider.OverrideCollisionWithType.Add(typeof(Door));
            collider.OverrideCollisionWithType.Add(typeof(Bomb));
            collider.OverrideCollisionWithType.Add(typeof(Explosion));
            collider.OverrideCollisionWithType.Add(typeof(CollisionPlane));
        }

        public void Activate()
        {
            if (target != null) target();
        }
    }
}
