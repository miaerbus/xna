using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Artificial.XNATutorial
{
    // Position
    public interface IPosition { Vector3 Position { get; set;} }
    public class PPosition : DataCell, IPosition
    {
        public override void Install(Item parent)
        {
            base.Install(parent);
            onPositionChanged = parent.As<POnPositionChanged>();            
        }

        Vector3 position;
        public Vector3 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
                if (onPositionChanged != null && onPositionChanged.OnPositionChanged != null)
                {
                    onPositionChanged.OnPositionChanged(parent, null);
                }
            }
        }

        POnPositionChanged onPositionChanged;
    }


    public interface IOnPositionChanged { EventHandler OnPositionChanged { get; set;} }
    public class POnPositionChanged { public EventHandler OnPositionChanged; }

    // Position with events
    public interface IPositionWithEvents : IPosition, IOnPositionChanged { }
    public class PPositionWithEvents : Part, IPositionWithEvents
    {
        public override void Install(Item parent)
        {
            base.Install(parent);
            onPositionChanged = parent.Require<POnPositionChanged>();
            position = parent.Require<PPosition>();
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

        #endregion
    }


    // World matrix
    public interface IWorldMatrix { Matrix WorldMatrix { get; set;} }
    public class PWorldMatrix { public Matrix WorldMatrix; }


    // Bounding box
    public interface IBoundingBox { BoundingBox BoundingBox { get; set;} }
    public class PBoundingBox { public BoundingBox BoundingBox; }


    // Bounding sphere
    public interface IBoundingSphere { BoundingSphere BoundingSphere { get; set;} }
    public class PBoundingSphere { public BoundingSphere BoundingSphere; }
}
