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
    public class Options : Part
    {
        // Gameplay component
        int updateOrder;

        Menu menu;
        State state;


        // CONSTRUCTOR
        public Options(int updateOrder)
        {
            this.updateOrder = updateOrder;
            state = Require<State>();
        }

        // INSTALL
        public override void Install(Item parent)
        {
            base.Install(parent);
            parent.As<Simulator>().RegisterUpdateMethod(updateOrder, Update);

            menu = Require<Menu>();

            // Prepare scene
            menu.Scene.Add(new TextButton(new Vector3(400, 0, 50), "OPTIONS", menu.Font, null, null));
            menu.Scene.Add(new TextButton(new Vector3(400, 0, 275), Bomberman.Game.Graphics.IsFullScreen ? "WINDOWED" : "FULLSCREEN", menu.Font, Fullscreen, HoverSound));
            menu.Scene.Add(new TextButton(new Vector3(400, 0, 325), "BACK", menu.Font, Back, HoverSound));
        }

        public override void Uninstall()
        {
            parent.As<Simulator>().UnregisterUpdateMethod(Update);
            menu.Uninstall();
        }

        void Update(float dt)
        {
            menu.Update(dt);
        }

        void Fullscreen()
        {
            Bomberman.Game.Graphics.ToggleFullScreen();
            // Deactivate mouse so it doesn't go right back
            menu.Mouse.IsActive = false;
            // Reload state so the correct text is displayed
            // Would be nicer if we could change button text
            state.NextState = Bomberman.States.Options;
            state.Finished = true;
            Bomberman.soundBank.PlayCue("OnMouseClick");
        }

        void Back()
        {
            state.NextState = Bomberman.States.MainMenu;
            state.Finished = true;
            Bomberman.soundBank.PlayCue("OnMouseClick");
        }

        void HoverSound()
        {
            Bomberman.soundBank.PlayCue("OnMouseOver");
        }
    }
}
