using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Artificial.XNATutorial.CarGame
{
    // ABT ITEM INTERFACE
    public interface IABTItem : IPosition, IBoundingBox
    {
        event EventHandler OnPositionChanged;
    }
}
