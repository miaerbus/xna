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

        TextButton warning;
        String warningMessage = "YOU NEED TO CONNECT AT LEAST TWO CONTROLLERS BEFORE YOU CAN START THE GAME";

        // INSTALL
        public override void Install(Item parent)
        {
            base.Install(parent);
            parent.As<Simulator>().RegisterUpdateMethod(updateOrder, Update);

            menu = Require<Menu>();

            // Prepare scene
            menu.Scene.Add(new TextButton(new Vector3(400, 0, 50), "MAIN MENU", menu.Font, null, null));

#if WINDOWS
            if (Bomberman.ActivePlayers.Count > 1)
            {
                menu.Scene.Add(new TextButton(new Vector3(400, 0, 200), "NEW GAME", menu.Font, StartOffline, HoverSound));
                menu.Scene.Add(new TextButton(new Vector3(400, 0, 250), "ONLINE MULTIPLAYER", menu.Font, StartOnline, HoverSound));
                menu.Scene.Add(new TextButton(new Vector3(400, 0, 300), "PLAYER SELECTION", menu.Font, PlayerSelection, HoverSound));
                menu.Scene.Add(new TextButton(new Vector3(400, 0, 350), "OPTIONS", menu.Font, Options, HoverSound));
                menu.Scene.Add(new TextButton(new Vector3(400, 0, 400), "EXIT", menu.Font, Exit, HoverSound));
            }
            else
            {
                menu.Scene.Add(new TextButton(new Vector3(400, 0, 225), "ONLINE MULTIPLAYER", menu.Font, StartOnline, HoverSound));
                menu.Scene.Add(new TextButton(new Vector3(400, 0, 275), "PLAYER SELECTION", menu.Font, PlayerSelection, HoverSound));
                menu.Scene.Add(new TextButton(new Vector3(400, 0, 325), "OPTIONS", menu.Font, Options, HoverSound));
                menu.Scene.Add(new TextButton(new Vector3(400, 0, 375), "EXIT", menu.Font, Exit, HoverSound));
            }
#else
            warning = new TextButton(new Vector3(400, 0, 500), warningMessage, 0.25f, menu.Font, null, null);
            menu.Scene.Add(new TextButton(new Vector3(400, 0, 275), "START", menu.Font, StartOffline, HoverSound));
            menu.Scene.Add(new TextButton(new Vector3(400, 0, 325), "EXIT", menu.Font, Exit, HoverSound));
#endif

            // Add players
            for (int i = 0; i < Bomberman.ActivePlayers.Count; i++)
            {
                int c = (int)Bomberman.ActivePlayers[i].ControllerType;
                if (c < 2 || GlobalInput.gamePadState[c - 2].IsConnected) menu.Scene.Add(Bomberman.ActivePlayers[i].Controller);
            }
            RefreshActiveControllers();
        }

        public void RefreshActiveControllers()
        {
            int numPlayers = Bomberman.ActivePlayers.Count;
            float width = 0;
            float space = 30;
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
                Bomberman.ActivePlayers[i].Controller.Part<PPosition>().Position = new Vector3(startX + offsetX[i], 0, 550);
            }
#if XBOX
            menu.Scene.Remove(warning);
            if (Bomberman.ActivePlayers.Count < 2) menu.Scene.Add(warning);
#endif
        }

        public override void Uninstall()
        {
            parent.As<Simulator>().UnregisterUpdateMethod(Update);
            menu.Uninstall();
        }

        void Update(float dt)
        {
            menu.Update(dt);

            for (int i = 0; i < GlobalInput.connectedControllers.Count; i++)
            {
                int j = GlobalInput.connectedControllers[i];
                for (int k = 0; k < Bomberman.Players.Count; k++)
                {
                    if (Bomberman.Players[k].Controller != null && (int)Bomberman.Players[k].Controller.ControlType - 2 == j)
                    {
                        Bomberman.Controllers[j + 2] = Bomberman.Players[k].Controller;
                    }
                }
                if (Bomberman.Controllers[j + 2] == null) Bomberman.Controllers[j + 2] = new Controller(new Vector3(), (ControlType)(j + 2));
                Bomberman.Players[j].Controller = null;
                Bomberman.Controllers[j + 2].PlayerIndex = j;
                if (!Bomberman.ActivePlayers.Contains(Bomberman.Players[j])) Bomberman.ActivePlayers.Add(Bomberman.Players[j]);
                menu.Scene.Add(Bomberman.Controllers[j + 2]);
                RefreshActiveControllers();
            }
            for (int i = 0; i < GlobalInput.disconnectedControllers.Count; i++)
            {
                int j = GlobalInput.disconnectedControllers[i];
                menu.Scene.Remove(Bomberman.Controllers[j + 2]);
                Bomberman.Controllers[j + 2] = null;
                RefreshActiveControllers();
            }

            if (GlobalInput.gamePadState[0].Buttons.Start == ButtonState.Pressed)
            {
                if (Bomberman.ActivePlayers.Count > 1)
                {
                    StartOffline(null);
                }
            }

            if (GlobalInput.gamePadState[0].Buttons.Back == ButtonState.Pressed)
            {
                Exit(null);
            }

        }

        void StartOffline(Item sender)
        {
            state.NextState = Bomberman.States.OfflinePlay;
            state.Finished = true;
            Bomberman.SoundBank.PlayCue("OnMouseClick");
        }

#if WINDOWS
        void StartOnline(Item sender)
        {
            state.NextState = Bomberman.States.OnlinePlay;
            state.Finished = true;
            Bomberman.SoundBank.PlayCue("OnMouseClick");
        }
#endif

        void PlayerSelection(Item sender)
        {
            state.NextState = Bomberman.States.PlayerSelection;
            state.Finished = true;
            Bomberman.SoundBank.PlayCue("OnMouseClick");
        }

        void Options(Item sender)
        {
            state.NextState = Bomberman.States.Options;
            state.Finished = true;
            Bomberman.SoundBank.PlayCue("OnMouseClick");
        }

        void Exit(Item sender)
        {
            state.NextState = Bomberman.States.End;
            state.Finished = true;
            Bomberman.SoundBank.PlayCue("OnMouseClick");
        }

        void HoverSound(Item sender)
        {
            Bomberman.SoundBank.PlayCue("OnMouseOver");
        }
    }
}
