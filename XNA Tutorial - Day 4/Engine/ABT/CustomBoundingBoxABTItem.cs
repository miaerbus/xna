using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Artificial.XNATutorial
{
    public class CustomBoundingBoxABTItem : IABTItem
    {
        // Position part
        PPositionWithEvents positionPart;
        Item positionItem;
        public Item PositionPart
        {
            get
            {
                return positionItem;
            }
            set
            {
                if (positionPart != null)
                {
                    positionPart.OnPositionChanged -= RaiseOnPositionChanged;
                }
                positionItem = value;
                positionPart = positionItem.As<PPositionWithEvents>();
                positionPart.OnPositionChanged += RaiseOnPositionChanged;
            }
        }

        // Position
        public Vector3 Position
        {
            get
            {
                return positionPart.Position;
            }
            set
            {
            }
        }

        // On position changed
        EventHandler onPositionChanged;
        private void RaiseOnPositionChanged(object sender, EventArgs e)
        {
            if (onPositionChanged != null)
            {
                onPositionChanged(this, null);
            }
        }
        public EventHandler OnPositionChanged
        {
            get
            {
                return onPositionChanged;
            }
            set
            {
                onPositionChanged += value;
            }
        }

        // Bounding box
        private BoundingBox boundingBox;
        public BoundingBox BoundingBox
        {
            get
            {
                return boundingBox;
            }
            set
            {
                boundingBox = value;
            }
        }
    }
}
