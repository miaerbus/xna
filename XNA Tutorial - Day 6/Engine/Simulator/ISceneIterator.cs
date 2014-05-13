using System;
using System.Collections.Generic;
using System.Text;

namespace Artificial.XNATutorial
{
    public interface ISceneIterator
    {
        void RegisterUpdateItemMethod(int updateOrder, UpdateItemMethod updateItemMethod);
    }
}
