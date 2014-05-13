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
    public class PlayerSelection : Part
    {
        // Gameplay component
        int updateOrder;

        Menu menu;
        State state;

        int controllerX = 275;
        int controllerDX = 150;

        // CONSTRUCTOR
        public PlayerSelection(int updateOrder)
        {
            this.updateOrder = updateOrder;
            state = Require<State>();
        }

        List<TextBox> playerBoxes = new List<TextBox>();

        // INSTALL
        public override void Install(Item parent)
        {
            base.Install(parent);
            parent.As<Simulator>().RegisterUpdateMethod(updateOrder, Update);
            menu = Require<Menu>();

            // Prepare scene
            menu.Scene.Add(new TextButton(new Vector3(400, 0, 50), "PLAYER SELECTION", menu.Font, null, null));
            menu.Scene.Add(new TextButton(new Vector3(50, 0, 150), "PLAYERS", 0.5f, TextPosition.Left, menu.Font, null, null));
            menu.Scene.Add(new TextButton(new Vector3(250, 0, 150), "AVAILABLE CONTROLLERS", 0.5f, TextPosition.Left, menu.Font, null, null));
            menu.Scene.Add(new TextButton(new Vector3(50, 0, 450), "ADD PLAYER", 0.5f, TextPosition.Left, menu.Font, AddPlayer, HoverSound));
            menu.Scene.Add(new TextButton(new Vector3(400, 0, 550), "START", menu.Font, Start, HoverSound));

            Refresh();

            // Add controllers
#if WINDOWS
            if (Bomberman.Controllers[0] == null) Bomberman.Controllers[0] = new Controller(new Vector3(300, 0, 200), ControlType.KeyboardLeft);
            if (Bomberman.Controllers[1] == null) Bomberman.Controllers[1] = new Controller(new Vector3(425, 0, 200), ControlType.KeyboardRight);
            menu.Scene.Add(Bomberman.Controllers[0]);
            menu.Scene.Add(Bomberman.Controllers[1]);
            controllerX = 525;
            controllerDX = 65;
#endif
            for (int i = 0; i < 4; i++)
            {
                if (GlobalInput.gamePadState[i].IsConnected)
                {
                    if (Bomberman.Controllers[i + 2] == null) Bomberman.Controllers[i + 2] = new Controller(new Vector3(controllerX + controllerDX * i, 0, 200), (ControlType)(i + 2));
                    menu.Scene.Add(Bomberman.Controllers[i + 2]);
                }
            }

            for (int i = 0; i < 6; i++)
            {
                if (Bomberman.Controllers[i] != null)
                {
                    if (Bomberman.Controllers[i].Player == null) Bomberman.Controllers[i].ResetPlayerIndex();
                    Bomberman.Controllers[i].UpdatePlayerSelectionPosition();
                    Bomberman.Controllers[i].ActivatePlayerSelectControls();
                }
            }

            UpdateControllerPositions();
        }

        void Refresh()
        {
            // Delete existing text boxes
            for (int i = 0; i < playerBoxes.Count; i++)
            {
                menu.Scene.Remove(playerBoxes[i]);
            }
            playerBoxes.Clear();

            for (int i = 0; i < Bomberman.Players.Count; i++)
            {
                TextBox p = new TextBox(new Vector3(50, 0, 225 + i * 25), Bomberman.Players[i].Name, 0.5f, TextPosition.Left, menu.Font, UpdatePlayerName, HoverSound);
                playerBoxes.Add(p);
                menu.Scene.Add(p);
            }
        }

        void UpdatePlayerName(Item sender)
        {
            TextBox t = sender as TextBox;
            int i = playerBoxes.IndexOf(t);
            if (t.Text == "")
            {
                if (Bomberman.Players[i].Controller != null)
                {
                    Bomberman.Players[i].Controller.ResetPlayerIndex();
                    Bomberman.Players[i].Controller.UpdatePlayerSelectionPosition();
                    Bomberman.Players[i].Controller = null;
                }
                Bomberman.Players.RemoveAt(i);
                Refresh();
            }
            else
            {
                Bomberman.Players[i].Name = t.Text;
            }
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
                    if (Bomberman.Players[k].Controller!=null && (int)Bomberman.Players[k].Controller.ControlType - 2 == j)
                    {
                        Bomberman.Controllers[j + 2] = Bomberman.Players[k].Controller;
                    }
                }
                if (Bomberman.Controllers[j + 2] == null) Bomberman.Controllers[j + 2] = new Controller(new Vector3(controllerX + controllerDX * j, 0, 200), (ControlType)(j + 2));
                Bomberman.Controllers[j + 2].ActivatePlayerSelectControls();
                menu.Scene.Add(Bomberman.Controllers[j + 2]);
                UpdateControllerPositions();
            }
            for (int i = 0; i < GlobalInput.disconnectedControllers.Count; i++)
            {
                int j = GlobalInput.disconnectedControllers[i];
                menu.Scene.Remove(Bomberman.Controllers[j + 2]);
                Bomberman.Controllers[j + 2] = null;
            }

            if (GlobalInput.gamePadState[0].Buttons.Start == ButtonState.Pressed)
            {
                Start(null);
            }
        }


        void UpdateControllerPositions()
        {
#if WINDOWS
            Bomberman.Controllers[0].Part<PPosition>().Position = new Vector3(300, 0, Bomberman.Controllers[0].Part<PPosition>().Position.Z);
            Bomberman.Controllers[1].Part<PPosition>().Position = new Vector3(425, 0, Bomberman.Controllers[1].Part<PPosition>().Position.Z);
#endif
            for (int i = 0; i < 4; i++)
            {
                if (Bomberman.Controllers[i+2]!=null)
                {
                    Bomberman.Controllers[i+2].Part<PPosition>().Position = new Vector3(controllerX + controllerDX * i, 0, Bomberman.Controllers[i+2].Part<PPosition>().Position.Z);
                }
            }

        }

        void AddPlayer(Item sender)
        {
            Bomberman.Players.Add(new Player());
            Refresh();
            Bomberman.SoundBank.PlayCue("OnMouseClick");
        }

        void Start(Item sender)
        {
            Bomberman.ActivePlayers.Clear();
            for (int k = 0; k < Bomberman.Players.Count; k++)
            {
                if (Bomberman.Players[k].Controller != null)
                {
                    Bomberman.Players[k].GenerateCharacter();
                    Bomberman.Players[k].Controller.ClearControls();
                    Bomberman.ActivePlayers.Add(Bomberman.Players[k]);
                }
            }

            if (Bomberman.ActivePlayers.Count > 0)
            {
                state.Finished = true;
                Bomberman.SoundBank.PlayCue("OnMouseClick");
            }
        }

        void HoverSound(Item sender)
        {
            Bomberman.SoundBank.PlayCue("OnMouseOver");
        }
    }
}
