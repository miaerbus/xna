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
            if (renderer.Finished || Input.mouseState.LeftButton==ButtonState.Pressed || Input.WasKeyPressed(Keys.Enter) || Input.WasKeyPressed(Keys.Space) || Input.WasKeyPressed(Keys.Escape))
            {
                Part<State>().Finished = true;
            }
        }
    }
}
