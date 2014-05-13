using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Artificial.XNATutorial;
using Artificial.XNATutorial.GUI;
using Artificial.XNATutorial.Physics;

namespace Artificial.XNATutorial.Bomberman
{
    public class Loading : Part
    {
        // Gameplay component
        int updateOrder;
        State state;

        // Renderer
        LoadingRenderer renderer;
        bool waitForWorldRenderer;

        // CONSTRUCTOR
        public Loading(int updateOrder, bool waitForWorldRenderer)
        {
            this.updateOrder = updateOrder;
            this.waitForWorldRenderer = waitForWorldRenderer;
            state = Require<State>();
        }

        // INSTALL
        public override void Install(Item parent)
        {
            base.Install(parent);
            parent.As<Simulator>().RegisterUpdateMethod(updateOrder, Update);

            // Create main renderer
            renderer = new LoadingRenderer(Bomberman.Game);
            renderer.DrawOrder = 0;
            Bomberman.Game.Components.Add(renderer);
        }

        public override void Uninstall()
        {
            parent.As<Simulator>().UnregisterUpdateMethod(Update);
            Bomberman.Game.Components.Remove(renderer);
        }

        void Update(float dt)
        {
            if (waitForWorldRenderer)
            {
                if (Bomberman.WorldRendererLoaded)
                {
                    state.Finished = true;
                }
            }
            else
            {
                if (Bomberman.MenuRendererLoaded)
                {
                    state.Finished = true;
                }
            }
        }
    }
}
