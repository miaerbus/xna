using System;
using System.Collections.Generic;
using System.Text;

namespace Artificial.XNATutorial
{
    interface IInstallable
    {
        void Install(Item parent);
        void Uninstall();
    }
}
