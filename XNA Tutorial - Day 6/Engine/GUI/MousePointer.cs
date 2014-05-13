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

            Require<PPositionWithEvents>().Position = new Vector3(400, 0, 300);
            Require<PBoundingSphere>().BoundingSphere = new BoundingSphere(Vector3.Zero, 1);
            Require<ParticleCollider>().ParticleRadius = 1;

            // Position
            position = Part<PPosition>();

            // Update
            Require<ItemProcess>().Process = update;
        }

        void update(float dt)
        {
#if WINDOWS
            position.Position = new Vector3((GlobalInput.mouseState.X + offsetMovement.X) * scaleMovement, 0, (GlobalInput.mouseState.Y + offsetMovement.Y) * scaleMovement );
            active = GlobalInput.WasLeftMouseButtonPressed();
#else
            position.Position += new Vector3(GlobalInput.gamePadState[0].ThumbSticks.Left.X, 0, -GlobalInput.gamePadState[0].ThumbSticks.Left.Y) * dt * 300;
            active = GlobalInput.gamePadState[0].Buttons.A == ButtonState.Pressed && GlobalInput.oldGamePadState[0].Buttons.A == ButtonState.Pressed;
#endif
        }

    }
}
