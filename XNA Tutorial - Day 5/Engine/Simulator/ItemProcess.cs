using System;
using System.Collections.Generic;
using System.Text;

namespace Artificial.XNATutorial
{
    public class ItemProcess : Part
    {
        UpdateMethod process;
        public UpdateMethod Process
        {
            get
            {
                return process;
            }
            set
            {
                process = value;
            }
        }
    }
}
