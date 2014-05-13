using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Artificial.XNATutorial.Input
{
    public enum ActionType
    {
        Action0D,
        Action1D,
        Action2D
    }

    public enum Action0DMode
    {
        OnPress,
        OnHold,
        OnRelease,
    }

    public enum ControlType
    {
        Keyboard,
        GamePad,
        Mouse
    }

    public enum GamePadControls
    {
        A,
        B,
        X,
        Y,
        LeftShoulder,
        RightShoulder,
        LeftStick,
        RightStick,
        Back,
        Start,
        LeftTrigger,
        RightTrigger,
        LeftThumbstick,
        RightThumbstick,
        DPad,
    }

    public enum MouseControls
    {
        Position,
        LeftButton,
        RightButton,
        MiddleButton,
        Wheel
    }

    // input delegates
    public delegate void Action0D();
    public delegate void Action1D(float degree);
    public delegate void Action2D(Vector2 degree);

}
