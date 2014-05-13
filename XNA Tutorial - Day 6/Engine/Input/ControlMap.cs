using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Artificial.XNATutorial.Input
{
    public class ControlMap
    {
        ControlType controlType;
        ActionType actionType;
        Action0DMode action0DMode;

        GamePadControls gamePadControl;
        MouseControls mouseControl;
        Keys keyControl;

        float digitalToAnalogSpeed;
        float digitalToAnalogValue;
        Vector2 digitalToAnalogDirection;

        Action0D action0D;
        Action1D action1D;
        Action2D action2D;


        // KEYBOARD
        public ControlMap(Keys key, Action0D action, Action0DMode mode)
        {
            controlType = ControlType.Keyboard;
            actionType = ActionType.Action0D;
            keyControl = key;
            action0D = action;
            action0DMode = mode;
        }

        public ControlMap(Keys key, Action1D action, float speed)
        {
            controlType = ControlType.Keyboard;
            actionType = ActionType.Action1D;
            keyControl = key;
            action1D = action;
            digitalToAnalogSpeed = speed;
        }

        public ControlMap(Keys key, Action2D action, float speed, Vector2 direction)
        {
            controlType = ControlType.Keyboard;
            actionType = ActionType.Action2D;
            keyControl = key;
            action2D = action;
            digitalToAnalogSpeed = speed;
            digitalToAnalogDirection = direction;
        }


        // MOUSE
        public ControlMap(MouseControls mouseButton, Action0D action, Action0DMode mode)
        {
            if (mouseButton == MouseControls.Position || mouseButton == MouseControls.Wheel) throw new ArgumentException("Only mouse buttons can be bound to an Action0D");
            controlType = ControlType.Mouse;
            actionType = ActionType.Action0D;
            mouseControl = mouseButton;
            action0D = action;
            action0DMode = mode;
        }

        public ControlMap(MouseControls mouseWheel, Action1D action)
        {
            if (mouseWheel != MouseControls.Wheel) throw new ArgumentException("Only mouse wheel can be bound to an Action1D");
            controlType = ControlType.Mouse;
            actionType = ActionType.Action1D;
            mouseControl = mouseWheel;
            action1D = action;
        }

        public ControlMap(MouseControls mousePosition, Action2D action)
        {
            if (mousePosition != MouseControls.Position) throw new ArgumentException("Only mouse position can be bound to an Action2D");
            controlType = ControlType.Mouse;
            actionType = ActionType.Action2D;
            mouseControl = mousePosition;
            action2D = action;
        }


        // GAMEPAD
        public ControlMap(GamePadControls gamePadButton, Action0D action, Action0DMode mode)
        {
            if (gamePadButton == GamePadControls.DPad || 
                gamePadButton == GamePadControls.LeftThumbstick ||
                gamePadButton == GamePadControls.LeftTrigger ||
                gamePadButton == GamePadControls.RightThumbstick ||
                gamePadButton == GamePadControls.RightTrigger) throw new ArgumentException("Only gamepad buttons can be bound to an Action0D");
            controlType = ControlType.GamePad;
            actionType = ActionType.Action0D;
            gamePadControl = gamePadButton;
            action0D = action;
            action0DMode = mode;
        }

        public ControlMap(GamePadControls gamePadTrigger, Action1D action)
        {
            if (!(gamePadTrigger == GamePadControls.LeftTrigger || gamePadTrigger == GamePadControls.RightTrigger)) throw new ArgumentException("Only gamepad triggers can be bound to an Action1D");
            controlType = ControlType.GamePad;
            actionType = ActionType.Action1D;
            gamePadControl = gamePadTrigger;
            action1D = action;
        }

        public ControlMap(GamePadControls gamePadStick, Action2D action)
        {
            if (!(gamePadStick == GamePadControls.LeftThumbstick ||
                gamePadStick == GamePadControls.RightThumbstick ||
                gamePadStick == GamePadControls.DPad)) throw new ArgumentException("Only gamepad thumbsticks and the D-pad can be bound to an Action2D");
            controlType = ControlType.GamePad;
            actionType = ActionType.Action2D;
            gamePadControl = gamePadStick;
            action2D = action;
        }


        // PROCESS
        public void Process(float dt, PlayerIndex gamePadIndex)
        {
            switch (controlType)
            {
                // Keyboard
                case ControlType.Keyboard:
                    switch (actionType)
                    {
                        case ActionType.Action0D:
                            switch (action0DMode)
                            {
                                case Action0DMode.OnHold:
                                    if (GlobalInput.keyState.IsKeyDown(keyControl)) action0D();
                                    break;

                                case Action0DMode.OnPress:
                                    if (GlobalInput.WasKeyPressed(keyControl)) action0D();
                                    break;

                                case Action0DMode.OnRelease:
                                    if (GlobalInput.WasKeyReleased(keyControl)) action0D();
                                    break;
                            }
                            break;

                        case ActionType.Action1D:
                        case ActionType.Action2D:
                            if (GlobalInput.keyState.IsKeyDown(keyControl))
                            {
                                digitalToAnalogValue += digitalToAnalogSpeed * dt;
                            }
                            else
                            {
                                digitalToAnalogValue -= digitalToAnalogSpeed * dt;
                            }
                            if (digitalToAnalogValue > 1) digitalToAnalogValue = 1;
                            if (digitalToAnalogValue < 0) digitalToAnalogValue = 0;
                            if (actionType == ActionType.Action1D)
                            {
                                action1D(digitalToAnalogValue);
                            }
                            else
                            {
                                action2D(digitalToAnalogValue * digitalToAnalogDirection);
                            }
                            break;
                    }
                    break;

#if WINDOWS
                // Mouse
                case ControlType.Mouse:
                    switch (actionType)
                    {
                        case ActionType.Action0D:
                            ButtonState oldState = ButtonState.Released;
                            ButtonState newState = ButtonState.Released;
                            switch (mouseControl)
                            {
                                case MouseControls.LeftButton:
                                    oldState = GlobalInput.oldMouseState.LeftButton;
                                    newState = GlobalInput.mouseState.LeftButton;
                                    break;
                                case MouseControls.RightButton:
                                    oldState = GlobalInput.oldMouseState.RightButton;
                                    newState = GlobalInput.mouseState.RightButton;
                                    break;
                                case MouseControls.MiddleButton:
                                    oldState = GlobalInput.oldMouseState.MiddleButton;
                                    newState = GlobalInput.mouseState.MiddleButton;
                                    break;
                            }
                            switch (action0DMode)
                            {
                                case Action0DMode.OnHold:
                                    if (newState == ButtonState.Pressed) action0D();
                                    break;

                                case Action0DMode.OnPress:
                                    if (newState == ButtonState.Pressed && oldState == ButtonState.Released) action0D();
                                    break;

                                case Action0DMode.OnRelease:
                                    if (newState == ButtonState.Released && oldState == ButtonState.Pressed) action0D();
                                    break;
                            }
                            break;

                        case ActionType.Action1D:
                            action1D(GlobalInput.mouseState.ScrollWheelValue);
                            break;

                        case ActionType.Action2D:
                            action2D(new Vector2(GlobalInput.mouseState.X, GlobalInput.mouseState.Y));
                            break;
                    }
                    break;
#endif

                // GamePad
                case ControlType.GamePad:
                    int index = (int)gamePadIndex;
                    switch (actionType)
                    {
                        case ActionType.Action0D:
                            ButtonState oldState = ButtonState.Released;
                            ButtonState newState = ButtonState.Released;
                            switch (gamePadControl)
                            {
                                case GamePadControls.A:
                                    oldState = GlobalInput.oldGamePadState[index].Buttons.A;
                                    newState = GlobalInput.gamePadState[index].Buttons.A;
                                    break;
                                case GamePadControls.B:
                                    oldState = GlobalInput.oldGamePadState[index].Buttons.B;
                                    newState = GlobalInput.gamePadState[index].Buttons.B;
                                    break;
                                case GamePadControls.X:
                                    oldState = GlobalInput.oldGamePadState[index].Buttons.X;
                                    newState = GlobalInput.gamePadState[index].Buttons.X;
                                    break;
                                case GamePadControls.Y:
                                    oldState = GlobalInput.oldGamePadState[index].Buttons.Y;
                                    newState = GlobalInput.gamePadState[index].Buttons.Y;
                                    break;
                                case GamePadControls.LeftShoulder:
                                    oldState = GlobalInput.oldGamePadState[index].Buttons.LeftShoulder;
                                    newState = GlobalInput.gamePadState[index].Buttons.LeftShoulder;
                                    break;
                                case GamePadControls.RightShoulder:
                                    oldState = GlobalInput.oldGamePadState[index].Buttons.RightShoulder;
                                    newState = GlobalInput.gamePadState[index].Buttons.RightShoulder;
                                    break;
                                case GamePadControls.LeftStick:
                                    oldState = GlobalInput.oldGamePadState[index].Buttons.LeftStick;
                                    newState = GlobalInput.gamePadState[index].Buttons.LeftStick;
                                    break;
                                case GamePadControls.RightStick:
                                    oldState = GlobalInput.oldGamePadState[index].Buttons.RightStick;
                                    newState = GlobalInput.gamePadState[index].Buttons.RightStick;
                                    break;
                                case GamePadControls.Back:
                                    oldState = GlobalInput.oldGamePadState[index].Buttons.Back;
                                    newState = GlobalInput.gamePadState[index].Buttons.Back;
                                    break;
                                case GamePadControls.Start:
                                    oldState = GlobalInput.oldGamePadState[index].Buttons.Start;
                                    newState = GlobalInput.gamePadState[index].Buttons.Start;
                                    break;

                            }
                            switch (action0DMode)
                            {
                                case Action0DMode.OnHold:
                                    if (newState == ButtonState.Pressed) action0D();
                                    break;

                                case Action0DMode.OnPress:
                                    if (newState == ButtonState.Pressed && oldState == ButtonState.Released) action0D();
                                    break;

                                case Action0DMode.OnRelease:
                                    if (newState == ButtonState.Released && oldState == ButtonState.Pressed) action0D();
                                    break;
                            }
                            break;

                        case ActionType.Action1D:
                            float value = 0;
                            switch (gamePadControl)
                            {
                                case GamePadControls.LeftTrigger:
                                    value = GlobalInput.gamePadState[index].Triggers.Left;
                                    break;
                                case GamePadControls.RightTrigger:
                                    value = GlobalInput.gamePadState[index].Triggers.Right;
                                    break;
                            }
                            action1D(value);
                            break;

                        case ActionType.Action2D:
                            Vector2 direction = Vector2.Zero;
                            switch (gamePadControl)
                            {
                                case GamePadControls.LeftThumbstick:
                                    direction = GlobalInput.gamePadState[index].ThumbSticks.Left;
                                    break;
                                case GamePadControls.RightThumbstick:
                                    direction = GlobalInput.gamePadState[index].ThumbSticks.Right;
                                    break;
                                case GamePadControls.DPad:
                                    if (GlobalInput.gamePadState[index].DPad.Down == ButtonState.Pressed) direction -= Vector2.UnitY;
                                    if (GlobalInput.gamePadState[index].DPad.Up == ButtonState.Pressed) direction += Vector2.UnitY;
                                    if (GlobalInput.gamePadState[index].DPad.Left == ButtonState.Pressed) direction -= Vector2.UnitX;
                                    if (GlobalInput.gamePadState[index].DPad.Right == ButtonState.Pressed) direction += Vector2.UnitX;
                                    break;
                            }
                            action2D(direction);
                            break;
                    }
                    break;                    
            }
        }
    }
}
