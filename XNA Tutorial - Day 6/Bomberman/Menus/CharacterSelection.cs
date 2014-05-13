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
    public class CharacterSelection : Part
    {
        // Gameplay component
        int updateOrder;

        Menu menu;
        State state;


        // CONSTRUCTOR
        public CharacterSelection(int updateOrder)
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
            menu.renderer.DrawCharacters = true;
            menu.renderer.BackgroundIndex = 0;

            // Prepare scene
            menu.Scene.Add(new TextButton(new Vector3(400, 0, 50), "CHARACTER SELECTION", menu.Font, null, null));
            menu.Scene.Add(new TextButton(new Vector3(400, 0, 500), "START", menu.Font, Start, HoverSound));
            menu.Scene.Add(new TextButton(new Vector3(400, 0, 550), "BACK", menu.Font, Back, HoverSound));

            // Add players
            int numPlayers = Bomberman.ActivePlayers.Count;
            float width = 0;
            float space = 80;
            float[] offsetX = new float[numPlayers];
            for (int i = 0; i < numPlayers; i++)
            {
                width += (int)Bomberman.ActivePlayers[i].Controller.ControlType < 2 ? 100 : 50;
                if (i > 0) offsetX[i] = offsetX[i - 1] + ((int)Bomberman.ActivePlayers[i - 1].Controller.ControlType < 2 ? 50 : 25) + space + ((int)Bomberman.ActivePlayers[i].Controller.ControlType < 2 ? 50 : 25);
            }
            width += space * (numPlayers - 1);
            float startX = 400 - width / 2 + ((int)Bomberman.ActivePlayers[0].Controller.ControlType < 2 ? 50 : 25);
            for (int i = 0; i < numPlayers; i++)
            {
                Bomberman.ActivePlayers[i].Controller.ActivateCharacterSelectControls();
                Bomberman.ActivePlayers[i].Controller.Part<PPosition>().Position = new Vector3(startX + offsetX[i], 0, 400);
                menu.Scene.Add(Bomberman.ActivePlayers[i].Controller);
            }
        }

        public override void Uninstall()
        {
            parent.As<Simulator>().UnregisterUpdateMethod(Update);
            menu.renderer.DrawCharacters = false;
            menu.Uninstall();
        }

        void Update(float dt)
        {
            menu.Update(dt);

            if (GlobalInput.gamePadState[0].Buttons.Start == ButtonState.Pressed)
            {
                    Start(null);
            }

            if (GlobalInput.gamePadState[0].Buttons.Back == ButtonState.Pressed)
            {
                Back(null);
            }

        }

        void Start(Item sender)
        {
            if (Bomberman.ActivePlayers.Count > 1)
            {
                for (int i = 0; i < Bomberman.ActivePlayers.Count; i++)
                {
                    Bomberman.ActivePlayers[i].GenerateCharacter();
                }

                state.Finished = true;
                Bomberman.SoundBank.PlayCue("OnMouseClick");
            }
        }

        void Back(Item sender)
        {
            state.NextState = Bomberman.States.MainMenu;
            state.Finished = true;
            Bomberman.SoundBank.PlayCue("OnMouseClick");
        }

        void HoverSound(Item sender)
        {
            Bomberman.SoundBank.PlayCue("OnMouseOver");
        }
    }
}
