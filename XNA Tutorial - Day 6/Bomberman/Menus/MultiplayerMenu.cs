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
    public class MultiplayerMenu : Part
    {
        // Gameplay component
        int updateOrder;

        Menu menu;
        State state;

        List<TextButton> otherComputers = new List<TextButton>();
        float updateTimer;

        // CONSTRUCTOR
        public MultiplayerMenu(int updateOrder)
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
            menu.Scene.Add(new TextButton(new Vector3(400, 0, 50), "MULTIPLAYER", menu.Font, null, null));
            menu.Scene.Add(new TextButton(new Vector3(400, 0, 100), "MY IP: " + Network.NetworkInfo.ThisIP().ToString(), 0.5f, menu.Font, null, null));

            menu.Scene.Add(new TextButton(new Vector3(400, 0, 200), "ADD CONNECTION", 0.5f, menu.Font, AddConnection, HoverSound));
            menu.Scene.Add(new TextButton(new Vector3(400, 0, 550), "BACK", menu.Font, Back, HoverSound));

            // Start multiplayer
            Bomberman.Multiplayer = new Multiplayer(!GlobalInput.keyState.IsKeyDown(Keys.LeftShift));

            // List other computers
            int y = 250;
            for (int i=0;i<Bomberman.Multiplayer.TheOthers.Count;i++)
            {
                TextButton text = new TextButton(new Vector3(400, 0, y), Bomberman.Multiplayer.TheOthers[i].Name, 0.5f, menu.Font, Connect, null);
                text.Tag = Bomberman.Multiplayer.TheOthers[i];
                otherComputers.Add(text);
                menu.Scene.Add(text);
                y += 25;
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

            updateTimer-=dt;
            if (updateTimer < 0)
            {
                updateTimer = 0.2f;
                for (int i = 0; i < Bomberman.Multiplayer.TheOthers.Count; i++)
                {
                    otherComputers[i].Text = Bomberman.Multiplayer.TheOthers[i].Name + (Bomberman.Multiplayer.TheOthers[i].Connected ? ": CONNECTED" : ": DEAD");
                }
            }

        }

        void AddConnection(Item sender)
        {
            state.NextState = Bomberman.States.EditConnection;
            state.Finished = true;
            Bomberman.SoundBank.PlayCue("OnMouseClick");
        }

        void Back(Item sender)
        {
            Bomberman.Multiplayer.Disconnect();
            state.NextState = Bomberman.States.MainMenu;
            state.Finished = true;
            Bomberman.SoundBank.PlayCue("OnMouseClick");
        }

        void Connect(Item sender)
        {
            NetworkComputer computer = sender.As<TextButton>().Tag as NetworkComputer;
            if (Bomberman.Multiplayer.ConnectToGame(computer))
            {
                state.NextState = Bomberman.States.Lobby;
                state.Finished = true;
            }
        }

        void HoverSound(Item sender)
        {
            Bomberman.SoundBank.PlayCue("OnMouseOver");
        }
    }
}
