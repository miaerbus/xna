using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Artificial.XNATutorial.Input
{
    public class InputMap
    {
        public List<ControlMap> Controls = new List<ControlMap>();

        public void Process(float dt, PlayerIndex gamePadIndex)
        {
            for (int i = 0; i < Controls.Count; i++)
            {
                Controls[i].Process(dt, gamePadIndex);
            }
        }
    }
}
