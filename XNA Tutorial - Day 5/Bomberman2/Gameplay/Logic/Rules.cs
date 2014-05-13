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
            for (int i = 0; i < Gameplay.Level.Characters.Count; i++)
            {
                if (Gameplay.Level.Characters[i].IsDead) numDead++;
            }
            if (numDead >= Gameplay.Level.Characters.Count - 1 || Gameplay.timeLeft < 0)
            {
                for (int i = 0; i < Gameplay.Level.Characters.Count; i++)
                {
                    Gameplay.Level.Characters[i].BombCount = 0;
                    if (isSafe) Gameplay.Level.Characters[i].Speed = 0;
                    if (isSafe && !Gameplay.Level.Characters[i].IsDead && numDead >= Gameplay.Level.Characters.Count - 1)
                    {
                        Gameplay.Level.Characters[i].HasWon = true;
                    }
                }
            }
            if ((numDead >= Gameplay.Level.Characters.Count - 1 || Gameplay.timeLeft < 0) && isSafe)
            {
                Gameplay.GameEnded = true;
            }

            isSafe = true;
        }
    }
}
