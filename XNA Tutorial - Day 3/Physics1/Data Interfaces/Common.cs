using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Artificial.XNATutorial
{
    // Position
    public interface IPosition
    {
        Vector3 Position {get; set;}
    }

    public interface IOnPositionChanged
    {
        event EventHandler OnPositionChanged;
    }

    public interface IPositionWithEvents : IPosition, IOnPositionChanged
    {
    }

    // World matrix
    public interface IWorldMatrix
    {
        Matrix WorldMatrix { get; set;}
    }

    // Bounding box
    public interface IBoundingBox
    {
        BoundingBox BoundingBox { get; set;}
    }


}
