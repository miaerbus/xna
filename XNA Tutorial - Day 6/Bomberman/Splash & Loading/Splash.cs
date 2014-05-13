using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Artificial.XNATutorial;
using Artificial.XNATutorial.Physics;

namespace Artificial.XNATutorial.Bomberman
{
    public partial class Splash : Part
    {
        // Gameplay component
        int updateOrder;

        // Renderer
        SplashRenderer renderer;

        // CONSTRUCTOR
        public Splash(int updateOrder)
        {
            this.updateOrder = updateOrder;
            Require<State>();
        }

        // INSTALL
        public override void Install(Item parent)
        {
            base.Install(parent);
            parent.As<Simulator>().RegisterUpdateMethod(updateOrder, Update);

            // Create main renderer
            renderer = new SplashRenderer(Bomberman.Game);
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
            if (renderer.Finished || GlobalInput.WasKeyPressed(Keys.Enter) || GlobalInput.WasKeyPressed(Keys.Space) || GlobalInput.WasKeyPressed(Keys.Escape) || GlobalInput.gamePadState[0].Buttons.Start == ButtonState.Pressed || GlobalInput.gamePadState[0].Buttons.A == ButtonState.Pressed)
            {
                Part<State>().Finished = true;
            }
#if WINDOWS
            if (GlobalInput.WasLeftMouseButtonPressed())
            {
                Part<State>().Finished = true;
            }
#endif
        }
    }
}
