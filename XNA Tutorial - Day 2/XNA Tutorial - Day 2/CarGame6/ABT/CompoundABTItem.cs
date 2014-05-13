using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Artificial.XNATutorial.CarGame
{
    public class CompoundABTItem : IABTItem
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

        private IBoundingBox sizePart;
        public IBoundingBox SizePart
        {
            get
            {
                return sizePart;
            }
            set
            {
                sizePart = value;
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
            EventHandler h = OnPositionChanged;
            if (h != null) h(this, null);
        }

        // Bounding box
        public BoundingBox BoundingBox
        {
            get
            {
                return sizePart.BoundingBox;
            }
            set
            {
            }
        }
    }
}
