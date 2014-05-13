using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Artificial.XNATutorial;
using Artificial.XNATutorial.Physics;

namespace Artificial.XNATutorial.Bomberman
{
    public class Rules : Part
    {
        int itemUpdateOrder;
        int globalUpdateOrder;

        public Rules(int itemUpdateOrder, int globalUpdateOrder)
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

        bool isSafe;

        // ITEM RULES
        void ItemRules(float dt, Item item)
        {
            if (item.Is<Explosion>() || item.Is<Bomb>())
            {
                isSafe = false;
            }
        }

        // GLOBAL RULES
        void GlobalRules(float dt)
        {
            int numDead=0;
            for (int i = 0; i < Bomberman.CurrentLevel.Characters.Count;i++)
            {
                if (Bomberman.CurrentLevel.Characters[i].IsDead) numDead++;
            }
            if (numDead >= Bomberman.CurrentLevel.Characters.Count - 1)
            {
                for (int i = 0; i < Bomberman.CurrentLevel.Characters.Count; i++)
                {
                    Bomberman.CurrentLevel.Characters[i].BombCount = 0;
                    if (isSafe && !Bomberman.CurrentLevel.Characters[i].IsDead)
                    {
                        Bomberman.CurrentLevel.Characters[i].HasWon=true;
                        Bomberman.CurrentLevel.Characters[i].Speed = 0;
                    }
                }                
            }
            isSafe = true;
        }
    }
}
