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
            for (int i = 0; i < Bomberman.Level.Characters.Count; i++)
            {
                if (Bomberman.Level.Characters[i].IsDead) numDead++;
            }
            if (numDead >= Bomberman.Level.Characters.Count - 1 || Gameplay.TimeLeft < 0)
            {
                for (int i = 0; i < Bomberman.Level.Characters.Count; i++)
                {
                    Bomberman.Level.Characters[i].BombCount = 0;
                    if (isSafe) Bomberman.Level.Characters[i].Speed = 0;
                    if (isSafe && !Bomberman.Level.Characters[i].IsDead && numDead >= Bomberman.Level.Characters.Count - 1)
                    {
                        Bomberman.Level.Characters[i].HasWon = true;
                    }
                }
            }
            if ((numDead >= Bomberman.Level.Characters.Count - 1 || Gameplay.TimeLeft < 0) && isSafe)
            {
                Gameplay.GameEnded = true;
            }

            isSafe = true;
        }
    }
}
