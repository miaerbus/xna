using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Artificial.XNATutorial
{
    // An item that can be represented in the ABT needs a position, a bounding box
    // and an event when the position is changed.
    public interface IABTItem : IPositionWithEvents, IBoundingBox
    {
    }

    public class PABTItem : Part, IABTItem
    {
        public override void Install(Item parent)
        {
            base.Install(parent);
            parent.Require<PPositionWithEvents>();
            parent.Require<PBoundingBox>();

            position = parent.As<PPosition>();
            onPositionChanged = parent.As<POnPositionChanged>();
            boundingBox = parent.As<PBoundingBox>();
        }

        #region Properties

        PPosition position;
        public Vector3 Position
        {
            get
            {
                return position.Position;
            }
            set
            {
                position.Position = value;
            }
        }

        POnPositionChanged onPositionChanged;
        public EventHandler OnPositionChanged
        {
            get
            {
                return onPositionChanged.OnPositionChanged;
            }
            set
            {
                onPositionChanged.OnPositionChanged = value;
            }
        }

        PBoundingBox boundingBox;
        public BoundingBox BoundingBox
        {
            get
            {
                return boundingBox.BoundingBox;
            }
            set
            {
                boundingBox.BoundingBox = value;
            }
        }


        #endregion
    }
}
