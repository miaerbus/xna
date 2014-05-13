using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Artificial.XNATutorial;
using Artificial.XNATutorial.Physics;
using FontRenderer;

namespace Artificial.XNATutorial.GUI
{
    public delegate void GUIAction();

    public class TextButton : Item
    {
        PPosition position;

        Font font;

        String text;
        public String Text
        {
            get
            {
                return text;
            }
        }

        bool mouseHoverLast;
        bool mouseHover;
        public bool MouseHover
        {
            get
            {
                return mouseHover;
            }
        }

        GUIAction onClick;
        GUIAction onHover;

        public TextButton(Vector3 position, string text, Font font)
        {
            Create(position, text, font, null, null);
        }

        public TextButton(Vector3 position, string text, Font font, GUIAction onClick)
        {
            Create(position, text, font, onClick, null);
        }

        public TextButton(Vector3 position, string text, Font font, GUIAction onClick, GUIAction onHover)
        {
            Create(position, text, font, onClick, onHover);
        }

        void Create(Vector3 position, string text, Font font, GUIAction onClick, GUIAction onHover)
        {
            this.text = text;
            this.font = font;
            this.onClick = onClick;
            this.onHover = onHover;

            Require<PPositionWithEvents>();
            Require<PBoundingSphere>();
            Require<PBoundingBox>();

            Require<ConvexCollider>();
            ConvexPolygon convex = new ConvexPolygon();
            float w = font.CalculateWidth(text, 1) * 0.5f;
            float h = font.CalculateHeight(1) *0.4f;

            convex.Points.Add(new Vector3(-w, 0, -h));
            convex.Points.Add(new Vector3(w, 0, -h));
            convex.Points.Add(new Vector3(w, 0, h));
            convex.Points.Add(new Vector3(-w, 0, h));
            convex.Normal = Vector3.Up;
            convex.CalculatePlanes();
            Part<PConvex>().Convex = convex;
            Part<PBoundingBox>().BoundingBox = new BoundingBox(new Vector3(-w, -1, -h), new Vector3(w, 1, h));
            Part<PBoundingSphere>().BoundingSphere = new BoundingSphere(Vector3.Zero, w + h);

            // Position
            this.position = Part<PPosition>();
            this.position.Position = position;

            // Update
            Require<ItemProcess>().Process = update;

            // Collider
            CustomCollider collider = Require<CustomCollider>();
            collider.CollisionMethod += Collide;
            collider.OverrideCollisionWithType.Add(typeof(MousePointer));
        }

        void update(float dt)
        {
            if (mouseHover == false) mouseHoverLast = false;
            mouseHover = false;            
        }

        void Collide(float elapsedSeconds, Item collidingItem, Vector3 impactPoint)
        {
            if (onClick!=null && collidingItem.Is<MousePointer>())
            {
                mouseHover = true;
                if (collidingItem.As<MousePointer>().IsActive) onClick();
                if (mouseHoverLast == false && onHover != null) onHover();
                mouseHoverLast = true;
            }
        }


    }
}
