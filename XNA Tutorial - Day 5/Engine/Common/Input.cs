using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Artificial.XNATutorial
{
    public static class Input
    {
        // Global input access
        public static KeyboardState keyState, oldKeyState;
        public static MouseState mouseState, oldMouseState;
        public static List<GamePadState> gamePadState, oldGamePadState;

        static Input()
        {
            gamePadState = new List<GamePadState>();
            oldGamePadState = new List<GamePadState>();
            for (int i = 0; i < 4; i++)
            {
                gamePadState.Add(new GamePadState());
                oldGamePadState.Add(new GamePadState());
            }
        }

        public static void Update()
        {
            oldKeyState = keyState;
            keyState = Keyboard.GetState();

            oldMouseState = mouseState;
            mouseState = Mouse.GetState();

            for (int i = 0; i < 4; i++)
            {
                oldGamePadState[i] = gamePadState[i];
                gamePadState[i] = GamePad.GetState((PlayerIndex)i);
            }
        }

        public static bool WasKeyPressed(Keys key)
        {
            return keyState.IsKeyDown(key) && oldKeyState.IsKeyUp(key);
        }

        public static bool WasLeftMouseButtonPressed()
        {
            return mouseState.LeftButton == ButtonState.Pressed && oldMouseState.LeftButton == ButtonState.Released;
        }

    }
}
