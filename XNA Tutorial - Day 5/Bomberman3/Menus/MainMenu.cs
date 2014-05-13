using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Artificial.XNATutorial;
using Artificial.XNATutorial.GUI;
using Artificial.XNATutorial.Physics;

namespace Artificial.XNATutorial.Bomberman
{
    public class MainMenu : Part
    {
        // Gameplay component
        int updateOrder;

        Menu menu;
        State state;


        // CONSTRUCTOR
        public MainMenu(int updateOrder)
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
            menu.Scene.Add(new TextButton(new Vector3(400, 0, 50), "MAIN MENU", menu.Font, null, null));
            menu.Scene.Add(new TextButton(new Vector3(400, 0, 250), "NEW GAME", menu.Font, NewGame, HoverSound));
            menu.Scene.Add(new TextButton(new Vector3(400, 0, 300), "OPTIONS", menu.Font, Options, HoverSound));
            menu.Scene.Add(new TextButton(new Vector3(400, 0, 350), "EXIT", menu.Font, Exit, HoverSound));
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

        void NewGame()
        {
            state.NextState = Bomberman.States.Gameplay;
            state.Finished = true;
            Bomberman.soundBank.PlayCue("OnMouseClick");
        }

        void Options()
        {
            state.NextState = Bomberman.States.Options;
            state.Finished = true;
            Bomberman.soundBank.PlayCue("OnMouseClick");
        }

        void Exit()
        {
            state.NextState = Bomberman.States.End;
            state.Finished = true;
            Bomberman.soundBank.PlayCue("OnMouseClick");
        }

        void HoverSound()
        {
            Bomberman.soundBank.PlayCue("OnMouseOver");
        }
    }
}
