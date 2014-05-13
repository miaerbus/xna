using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Artificial.XNATutorial;
using Artificial.XNATutorial.Physics;

namespace Artificial.XNATutorial.GUI
{
    public class MousePointer : Item
    {
        PPosition position;
        float scaleMovement;
        Vector2 offsetMovement;

        private bool active;
        public bool IsActive
        {
            get
            {
                return active;
            }
            set
            {
                active = value;
            }
        }

        public MousePointer(float scaleMovement, Vector2 offsetMovement)
        {
            this.scaleMovement = scaleMovement;
            this.offsetMovement = offsetMovement;

            Require<PPositionWithEvents>();
            Require<PBoundingSphere>().BoundingSphere = new BoundingSphere(Vector3.Zero, 1);
            Require<ParticleCollider>().ParticleRadius = 1;

            // Position
            position = Part<PPosition>();

            // Update
            Require<ItemProcess>().Process = update;
        }

        void update(float dt)
        {
            position.Position = new Vector3((Input.mouseState.X + offsetMovement.X) * scaleMovement, 0, (Input.mouseState.Y + offsetMovement.Y) * scaleMovement );
            active = Input.WasLeftMouseButtonPressed();
        }

    }
}
