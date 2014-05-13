using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Artificial.XNATutorial
{
    public static class GlobalInput
    {
        // Global input access
        public static KeyboardState keyState, oldKeyState;
        public static List<GamePadState> gamePadState, oldGamePadState;
        public static List<int> connectedControllers = new List<int>();
        public static List<int> disconnectedControllers = new List<int>();
#if WINDOWS
        public static MouseState mouseState, oldMouseState;
#endif

        static GlobalInput()
        {
            gamePadState = new List<GamePadState>();
            oldGamePadState = new List<GamePadState>();
            for (int i = 0; i < 4; i++)
            {
                gamePadState.Add(new GamePadState());
                oldGamePadState.Add(new GamePadState());
            }
            Update();
        }


        public static void Update()
        {
            oldKeyState = keyState;
            keyState = Keyboard.GetState();
#if WINDOWS
            oldMouseState = mouseState;
            mouseState = Mouse.GetState();
#endif
            connectedControllers.Clear();
            disconnectedControllers.Clear();

            for (int i = 0; i < 4; i++)
            {
                oldGamePadState[i] = gamePadState[i];
                gamePadState[i] = GamePad.GetState((PlayerIndex)i);
                if (gamePadState[i].IsConnected != oldGamePadState[i].IsConnected)
                {
                    if (gamePadState[i].IsConnected)
                    {
                        connectedControllers.Add(i);
                    }
                    else
                    {
                        disconnectedControllers.Add(i);
                    }
                }
            }
        }

        public static bool WasKeyPressed(Keys key)
        {
           return keyState.IsKeyDown(key) && oldKeyState.IsKeyUp(key);
        }

        public static bool WasKeyReleased(Keys key)
        {
            return keyState.IsKeyUp(key) && oldKeyState.IsKeyDown(key);
        }


#if WINDOWS
        public static bool WasLeftMouseButtonPressed()
        {
            return mouseState.LeftButton == ButtonState.Pressed && oldMouseState.LeftButton == ButtonState.Released;
        }
#endif
    }
}
