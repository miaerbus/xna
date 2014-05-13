using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Artificial.XNATutorial.CarGame
{
    public class ABTItem<T> : IABTItem
    {
        // This is a default implementation of the IABTItem interface with a tag that this
        // item is representing.

        // Position
        private Vector3 position;
        public Vector3 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
                RaiseOnPositionChanged();
            }
        }
        // Position changed event
        public event EventHandler OnPositionChanged;
        private void RaiseOnPositionChanged()
        {
            EventHandler e = OnPositionChanged;
            if (e != null) e(this, null);
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

        // Tag
        private T tag;
        public T Tag
        {
            get
            {
                return tag;
            }
            set
            {
                tag = value;
            }
        }
    }
}
