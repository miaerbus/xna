using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Artificial.XNATutorial;
using Artificial.XNATutorial.Physics;

namespace Artificial.XNATutorial.Pong
{
    public class PongRules : Part
    {
        int itemUpdateOrder;
        int globalUpdateOrder;

        public PongRules(int itemUpdateOrder, int globalUpdateOrder)
        {
            this.itemUpdateOrder = itemUpdateOrder;
            this.globalUpdateOrder = globalUpdateOrder;
        }

        public override void Install(Item parent)
        {
            base.Install(parent);
            parent.As<ISceneIterator>().RegisterUpdateItemMethod(itemUpdateOrder, ItemRules);
            parent.As<Simulator>().RegisterUpdateMethod(globalUpdateOrder, GlobalRules);
        }

        // ITEM RULES
        void ItemRules(float dt, Item item)
        {
            // Ball
            Ball b = item.As<Ball>();
            if (b != null)
            {
                Particle p = b.As<Particle>();
                if (Math.Abs(p.Position.X) > 450)
                {
                    int winner = (int)(-Math.Sign(p.Position.X) * 0.5f + 0.5f);
                    Pong.Score[winner]++;
                    b.Reset();
                }
            }
        }

        // GLOBAL RULES
        void GlobalRules(float dt)
        {
            if (Pong.Score[0] > 9 || Pong.Score[1] > 9)
            {
                Pong.Score[0] = 0;
                Pong.Score[1] = 0;               
            }
        }        
    }
}
