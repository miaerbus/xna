using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Artificial.XNATutorial
{
    public class CustomBoundingBoxABTItem : IABTItem
    {
        // Parts
        private IPositionWithEvents positionPart;
        public IPositionWithEvents PositionPart
        {
            get
            {
                return positionPart;
            }
            set
            {
                if (positionPart != null)
                {
                    positionPart.OnPositionChanged -= RaiseOnPositionChanged;
                }
                positionPart = value;
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
        public event EventHandler OnPositionChanged;
        private void RaiseOnPositionChanged(object sender, EventArgs e)
        {
            if (OnPositionChanged != null)
            {
                OnPositionChanged(this, null);
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
