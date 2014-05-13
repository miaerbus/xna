using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Artificial.XNATutorial.CarGame
{
    // Position
    public interface IPosition
    {
        Vector3 Position {get; set;}
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
