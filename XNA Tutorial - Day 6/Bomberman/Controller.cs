using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Artificial.XNATutorial;
using Artificial.XNATutorial.Physics;
using Artificial.XNATutorial.Input;

namespace Artificial.XNATutorial.Bomberman
{
    public enum ControlType : int
    {
        None = -1,
        KeyboardLeft = 0,
        KeyboardRight = 1,
        GamePad1 = 2,
        GamePad2 = 3,
        GamePad3 = 4,
        GamePad4 = 5,
    }

    public class Controller : Item
    {
        PPosition position;

        ControlType controlType;
        public ControlType ControlType
        {
            get
            {
                return controlType;
            }
        }

        int playerIndex = -1;
        public int PlayerIndex
        {
            set
            {
                playerIndex = value;
                SetPlayer();
            }
        }

        Player player;
        public Player Player
        {
            get
            {
                return player;
            }
        }

        PlayerIndex gamePadIndex;
        InputMap inputMap;

        public Controller(Vector3 position, ControlType controlType)
        {
            this.controlType = controlType;
            inputMap = new InputMap();

            Require<PPositionWithEvents>().Position = position;
            Require<PBoundingSphere>().BoundingSphere = new BoundingSphere(Vector3.Zero, 50);

            // Position
            this.position = Part<PPosition>();

            // Update
            Require<ItemProcess>().Process = Update;
        }

        public void ActivatePlayerSelectControls()
        {
            inputMap.Controls.Clear();
            if ((int)controlType > 1)
            {
                gamePadIndex = (PlayerIndex)((int)controlType - 2);
                inputMap.Controls.Add(new ControlMap(GamePadControls.LeftThumbstick, ChangePlayer));
                inputMap.Controls.Add(new ControlMap(GamePadControls.DPad, ChangePlayer));
                inputMap.Controls.Add(new ControlMap(GamePadControls.A, SelectPlayer, Action0DMode.OnPress));
            }
            else if (controlType == ControlType.KeyboardLeft)
            {
                inputMap.Controls.Add(new ControlMap(Keys.W, DecreasePlayer, Action0DMode.OnPress));
                inputMap.Controls.Add(new ControlMap(Keys.S, IncreasePlayer, Action0DMode.OnPress));
                inputMap.Controls.Add(new ControlMap(Keys.A, PreviousCharacter, Action0DMode.OnPress));
                inputMap.Controls.Add(new ControlMap(Keys.D, NextCharacter, Action0DMode.OnPress));
                inputMap.Controls.Add(new ControlMap(Keys.LeftControl, SelectPlayer, Action0DMode.OnPress));
            }
            else
            {
                inputMap.Controls.Add(new ControlMap(Keys.Up, DecreasePlayer, Action0DMode.OnPress));
                inputMap.Controls.Add(new ControlMap(Keys.Down, IncreasePlayer, Action0DMode.OnPress));
                inputMap.Controls.Add(new ControlMap(Keys.Left, PreviousCharacter, Action0DMode.OnPress));
                inputMap.Controls.Add(new ControlMap(Keys.Right, NextCharacter, Action0DMode.OnPress));
                inputMap.Controls.Add(new ControlMap(Keys.RightControl, SelectPlayer, Action0DMode.OnPress));
            }
        }

        public void ActivateCharacterSelectControls()
        {
            inputMap.Controls.Clear();
            if ((int)controlType > 1)
            {
                gamePadIndex = (PlayerIndex)((int)controlType - 2);
                inputMap.Controls.Add(new ControlMap(GamePadControls.LeftThumbstick, ChangeCharacter));
                inputMap.Controls.Add(new ControlMap(GamePadControls.DPad, ChangeCharacter));
            }
            else if (controlType == ControlType.KeyboardLeft)
            {
                inputMap.Controls.Add(new ControlMap(Keys.A, PreviousCharacter, Action0DMode.OnPress));
                inputMap.Controls.Add(new ControlMap(Keys.D, NextCharacter, Action0DMode.OnPress));
            }
            else
            {
                inputMap.Controls.Add(new ControlMap(Keys.Left, PreviousCharacter, Action0DMode.OnPress));
                inputMap.Controls.Add(new ControlMap(Keys.Right, NextCharacter, Action0DMode.OnPress));
            }
        }


        float digitalToAnalogSpeed = 5;
        public void ActivateGameControls()
        {
            inputMap.Controls.Clear();

            if ((int)controlType > 1)
            {
                inputMap.Controls.Add(new ControlMap(GamePadControls.LeftThumbstick, player.Character.Move));
                inputMap.Controls.Add(new ControlMap(GamePadControls.DPad, player.Character.Move));
                inputMap.Controls.Add(new ControlMap(GamePadControls.A, player.Character.Fire, Action0DMode.OnPress));
            }
            else if (controlType == ControlType.KeyboardLeft)
            {
                inputMap.Controls.Add(new ControlMap(Keys.W, player.Character.Move, digitalToAnalogSpeed, Vector2.UnitY));
                inputMap.Controls.Add(new ControlMap(Keys.A, player.Character.Move, digitalToAnalogSpeed, -Vector2.UnitX));
                inputMap.Controls.Add(new ControlMap(Keys.S, player.Character.Move, digitalToAnalogSpeed, -Vector2.UnitY));
                inputMap.Controls.Add(new ControlMap(Keys.D, player.Character.Move, digitalToAnalogSpeed, Vector2.UnitX));
                inputMap.Controls.Add(new ControlMap(Keys.LeftControl, player.Character.Fire, Action0DMode.OnPress));
            }
            else
            {
                inputMap.Controls.Add(new ControlMap(Keys.Up, player.Character.Move, digitalToAnalogSpeed, Vector2.UnitY));
                inputMap.Controls.Add(new ControlMap(Keys.Left, player.Character.Move, digitalToAnalogSpeed, -Vector2.UnitX));
                inputMap.Controls.Add(new ControlMap(Keys.Down, player.Character.Move, digitalToAnalogSpeed, -Vector2.UnitY));
                inputMap.Controls.Add(new ControlMap(Keys.Right, player.Character.Move, digitalToAnalogSpeed, Vector2.UnitX));
                inputMap.Controls.Add(new ControlMap(Keys.RightControl, player.Character.Fire, Action0DMode.OnPress));
            }
        }

        public void ClearControls()
        {
            inputMap.Controls.Clear();
        }

        public void ResetPlayerIndex()
        {
            playerIndex = -1;
            player = null;
        }

        public void Update(float dt)
        {
            inputMap.Process(dt, gamePadIndex);
            if (changeTime > 0) changeTime -= dt;
        }

        float changeTime = 0;
        void ChangePlayer(Vector2 direction)
        {
            if (changeTime <= 0)
            {
                if (direction.Y < -0.1f)
                {
                    changeTime = 0.2f;
                    IncreasePlayer();
                }
                if (direction.Y > 0.1f)
                {
                    changeTime = 0.2f;
                    DecreasePlayer();
                }
            }
        }

        void ChangeCharacter(Vector2 direction)
        {
            if (changeTime <= 0)
            {
                if (direction.X < -0.1f)
                {
                    changeTime = 0.2f;
                    PreviousCharacter();
                }
                if (direction.X > 0.1f)
                {
                    changeTime = 0.2f;
                    NextCharacter();
                }
            }
        }


        void SelectPlayer()
        {
            if (player != null)
            {
                Bomberman.Players[playerIndex].Controller = null;
                player.ControllerType = ControlType.None;
                player = null;
            }
            else
            {
                SetPlayer();
            }
        }

        void SetPlayer()
        {
            if (playerIndex > -1 && Bomberman.Players[playerIndex].Controller == null)
            {
                player = Bomberman.Players[playerIndex];
                player.Controller = this;
                player.ControllerType = controlType;
            }
        }

        void IncreasePlayer()
        {
            if (player != null) return;

            int oldPlayer = playerIndex;
            do
            {
                playerIndex++;
            } while (playerIndex < Bomberman.Players.Count && Bomberman.Players[playerIndex].Controller != null);
            if (playerIndex >= Bomberman.Players.Count)
            {
                playerIndex = oldPlayer;
            }
            UpdatePlayerSelectionPosition();
        }

        void DecreasePlayer()
        {
            if (player != null) return;
            
            int oldPlayer = playerIndex;
            do
            {
                playerIndex--;
            } while (playerIndex > -1 && Bomberman.Players[playerIndex].Controller != null);
            if (playerIndex < -1)
            {
                playerIndex = oldPlayer;
            }
            UpdatePlayerSelectionPosition();
        }

        public void UpdatePlayerSelectionPosition()
        {
            position.Position = new Vector3(position.Position.X, 0, 225 + playerIndex * 25);
        }

        void NextCharacter()
        {
            if (player == null) return;
            int t = (int)player.CharacterType;
            t++;
            if (t > 5) t = 0;
            player.CharacterType = (CharacterType)t;
        }

        void PreviousCharacter()
        {
            if (player == null) return;
            int t = (int)player.CharacterType;
            t--;
            if (t < 0) t = 5;
            player.CharacterType = (CharacterType)t;
        }

    }
}
