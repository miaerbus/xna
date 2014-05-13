using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Artificial.XNATutorial.Input
{
    public interface IInputReceiver
    {
        InputMap currentInputMap();
        PlayerIndex gamePadIndex();
    }
}
