using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Artificial.XNATutorial.CarGame
{
    // An item that can be represented in the ABT needs a position, a bounding box
    // and an event when the position is changed.
    public interface IABTItem : IPosition, IBoundingBox
    {
        event EventHandler OnPositionChanged;
    }
}
