﻿namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using EphemereGames.Core.Input;
    using Microsoft.Xna.Framework.Input;
    

    static class KeyboardConfiguration
    {
        public static Keys AdvancedView = Keys.R;
        public static Keys Next = Keys.D;
        public static Keys Previous = Keys.A;
        public static Keys Back = Keys.Escape;
        public static Keys ChangeMusic = Keys.RightShift;
        public static Keys Debug = Keys.F1;
        public static Keys Editor = Keys.F2;
        public static Keys Disconnect = Keys.Delete;
        public static Keys Tweaking = Keys.T;
        public static Keys RetryLevel = Keys.R;
        public static Keys MoveUp = Keys.W;
        public static Keys MoveLeft = Keys.A;
        public static Keys MoveDown = Keys.S;
        public static Keys MoveRight = Keys.D;
        public static Keys QuickToggle = Keys.Space;
        public static Keys SelectionNext = Keys.E;
        public static Keys SelectionPrevious = Keys.Q;
        public static Keys Cheat1 = Keys.P;

        public static List<Keys> ToList
        {
            get
            {
                return new List<Keys>()
                {
                    AdvancedView,
                    Back,
                    Next,
                    Previous,
                    ChangeMusic,
                    Debug,
                    Editor,
                    Disconnect,
                    Tweaking,
                    RetryLevel,
                    MoveDown,
                    MoveLeft,
                    MoveRight,
                    MoveUp,
                    QuickToggle,
                    SelectionNext,
                    SelectionPrevious,
                    Cheat1
                };
            }
        }


        public static Dictionary<Keys, string> ToImage = new Dictionary<Keys, string>(KeysComparer.Default)
        {
            { Keys.W, "KeyW" },
            { Keys.A, "KeyA" },
            { Keys.S, "KeyS" },
            { Keys.D, "KeyD" },
            { Keys.R, "KeyR" },
            { Keys.F, "KeyF" },
            { Keys.E, "KeyE" },
            { Keys.Q, "KeyQ" },
            { Keys.Delete, "KeyDelete" }
        };
    }


    static class MouseConfiguration
    {
        public static MouseButton Select = MouseButton.Left;
        public static MouseButton Cancel = MouseButton.Right;
        public static MouseButton AlternateSelect = MouseButton.Middle;
        public static MouseButton SelectionNext = MouseButton.MiddleDown;
        public static MouseButton SelectionPrevious = MouseButton.MiddleUp;
        public static MouseButton Fire = MouseButton.Right;
        public static MouseButton Previous = MouseButton.Left;
        public static MouseButton Next = MouseButton.Right;
        public static float MovingSpeed = 1.5f;
        public static float RotatingSpeed = 0.1f;


        public static List<MouseButton> ToList
        {
            get
            {
                return new List<MouseButton>()
                {
                    Select,
                    Cancel,
                    AlternateSelect,
                    SelectionNext,
                    SelectionPrevious,
                    Fire,
                    Previous,
                    Next
                };
            }
        }


        public static Dictionary<MouseButton, string> ToImage = new Dictionary<MouseButton, string>(MouseButtonComparer.Default)
        {
            { MouseButton.Left, "MouseLeft" },
            { MouseButton.Right, "MouseRight" },
            { MouseButton.Middle, "MouseMiddle" },
            { MouseButton.MiddleUp, "MouseMiddleRoll" },
            { MouseButton.MiddleDown, "MouseMiddleRoll" }
        };
    }


    static class GamePadConfiguration
    {
        public static Buttons Back = Buttons.Start;
        public static Buttons Cancel = Buttons.B;
        public static Buttons ChangeMusic = Buttons.DPadLeft;
        public static Buttons Debug = Buttons.LeftShoulder;
        public static Buttons Tweaking = Buttons.RightShoulder;
        public static Buttons Select = Buttons.A;
        public static Buttons AlternateSelect = Buttons.X;
        public static Buttons AdvancedView = Buttons.Y;
        public static Buttons SelectionNext = Buttons.RightTrigger;
        public static Buttons SelectionPrevious = Buttons.LeftTrigger;
        public static Buttons AlternateSelectionNext = Buttons.DPadDown;
        public static Buttons AlternateSelectionPrevious = Buttons.DPadUp;
        public static Buttons MoveCursor = Buttons.LeftStick;
        public static Buttons DirectionCursor = Buttons.RightStick;
        public static Buttons Disconnect = Buttons.Back;
        public static Buttons RetryLevel = Buttons.Y;
        public static Buttons Cheat1 = Buttons.DPadRight;
        public static float Speed = 15;


        public static List<Buttons> ToList
        {
            get
            {
                return new List<Buttons>()
                {
                    Back,
                    Disconnect,
                    Cancel,
                    ChangeMusic,
                    Debug,
                    AlternateSelect,
                    Select,
                    AdvancedView,
                    SelectionNext,
                    SelectionPrevious,
                    Tweaking,
                    RetryLevel,
                    AlternateSelectionNext,
                    AlternateSelectionPrevious,
                    Cheat1
                };
            }
        }


        public static Dictionary<Buttons, string> ToImage = new Dictionary<Buttons, string>(ButtonsComparer.Default)
        {
            { Buttons.Back, "ButtonBack" },
            { Buttons.A, "ButtonA" },
            { Buttons.B, "ButtonB" },
            { Buttons.X, "ButtonX" },
            { Buttons.Y, "ButtonY" },
            { Buttons.LeftShoulder, "BumperLeft" },
            { Buttons.RightShoulder, "BumperRight" },
            { Buttons.LeftStick, "ThumbstickLeft" },
            { Buttons.RightStick, "ThumbstickRight" },
            { Buttons.LeftTrigger, "TriggerLeft" },
            { Buttons.RightTrigger, "TriggerRight" },
            { Buttons.Start, "ButtonStart" }
        };
    }
}
