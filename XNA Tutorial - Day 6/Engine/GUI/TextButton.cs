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
    public class TextButton : Item
    {
        PPosition position;
        Vector3 positionOrigin;

        Font font;

        protected String text;
        public String Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
                ConvexPolygon convex = Part<PConvex>().Convex as ConvexPolygon;
                float w = font.CalculateWidth(text, 1) * 0.5f * scale;
                float h = font.CalculateHeight(1) * 0.4f * scale;

                convex.Points[0] = new Vector3(-w, 0, -h);
                convex.Points[1] = new Vector3(w, 0, -h);
                convex.Points[2] = new Vector3(w, 0, h);
                convex.Points[3] = new Vector3(-w, 0, h);
                convex.CalculatePlanes();

                if (textPosition == TextPosition.Left)
                    position.Position = positionOrigin + Vector3.Right * w;
                else if (textPosition == TextPosition.Right)
                    position.Position = positionOrigin + Vector3.Left * w;
                else
                    position.Position = positionOrigin;

                Part<PBoundingBox>().BoundingBox = new BoundingBox(new Vector3(-w, -1, -h), new Vector3(w, 1, h));
                Part<PBoundingSphere>().BoundingSphere = new BoundingSphere(Vector3.Zero, w + h);
            }
        }
        TextPosition textPosition;

        float scale;
        public float Scale
        {
            get
            {
                return scale;
            }
        }

        bool mouseHoverLast;
        protected bool mouseHover;
        public bool MouseHover
        {
            get
            {
                return mouseHover;
            }
        }

        GUIAction onClick;
        GUIAction onHover;
        public object Tag;

        public TextButton()
        {
        }

        public TextButton(Vector3 position, string text, Font font)
        {
            Create(position, text, 1, TextPosition.Centered, font, null, null);
        }

        public TextButton(Vector3 position, string text, Font font, GUIAction onClick)
        {
            Create(position, text, 1, TextPosition.Centered, font, onClick, null);
        }

        public TextButton(Vector3 position, string text, Font font, GUIAction onClick, GUIAction onHover)
        {
            Create(position, text, 1, TextPosition.Centered, font, onClick, onHover);
        }

        public TextButton(Vector3 position, string text, float scale, Font font, GUIAction onClick, GUIAction onHover)
        {
            Create(position, text, scale, TextPosition.Centered, font, onClick, onHover);
        }

        public TextButton(Vector3 position, string text, float scale, TextPosition textPosition, Font font, GUIAction onClick, GUIAction onHover)
        {
            Create(position, text, scale, textPosition, font, onClick, onHover);
        }

        protected void Create(Vector3 position, string text, float scale, TextPosition textPosition, Font font, GUIAction onClick, GUIAction onHover)
        {
            this.scale = scale;
            this.textPosition = textPosition;
            this.font = font;
            this.onClick = onClick;
            this.onHover = onHover;            

            Require<PPositionWithEvents>();
            Require<PBoundingSphere>();
            Require<PBoundingBox>();
            this.position = Part<PPosition>();
            positionOrigin = position;

            Require<ConvexCollider>();
            ConvexPolygon convex = new ConvexPolygon();
            float w = font.CalculateWidth(text, 1) * 0.5f * scale;
            float h = font.CalculateHeight(1) *0.4f * scale;

            convex.Points.Add(new Vector3());
            convex.Points.Add(new Vector3());
            convex.Points.Add(new Vector3());
            convex.Points.Add(new Vector3());
            convex.Normal = Vector3.Up;
            convex.CalculatePlanes();
            Part<PConvex>().Convex = convex;
            Text = text;

            // Update
            Require<ItemProcess>().Process = update;

            // Collider
            CustomCollider collider = Require<CustomCollider>();
            collider.CollisionMethod += Collide;
            collider.OverrideCollisionWithType.Add(typeof(MousePointer));
        }

        protected virtual void update(float dt)
        {
            if (mouseHover == false) mouseHoverLast = false;
            mouseHover = false;            
        }

        protected virtual void Collide(float elapsedSeconds, Item collidingItem, Vector3 impactPoint)
        {
            if (onClick!=null && collidingItem.Is<MousePointer>())
            {
                mouseHover = true;
                if (collidingItem.As<MousePointer>().IsActive)
                {
                    collidingItem.As<MousePointer>().IsActive = false;
                    onClick(this);
                }
                if (mouseHoverLast == false && onHover != null) onHover(this);
                mouseHoverLast = true;
            }
        }


    }
}
