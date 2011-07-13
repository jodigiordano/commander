namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using EphemereGames.Core.Input;
    using Microsoft.Xna.Framework.Input;
    

    static class KeyboardConfiguration
    {
        public static Keys Next = Keys.Enter;
        public static Keys Back = Keys.Escape;
        public static Keys Cancel = Keys.Escape;
        public static Keys ChangeMusic = Keys.RightShift;
        public static Keys NextWave = Keys.RightControl;
        public static Keys Debug = Keys.F1;
        public static Keys Editor = Keys.F2;
        public static Keys Disconnect = Keys.Q;
        public static Keys Tweaking = Keys.T;
        public static Keys RetryLevel = Keys.R;

        public static List<Keys> ToList
        {
            get
            {
                return new List<Keys>()
                {
                    Back,
                    Next,
                    Cancel,
                    ChangeMusic,
                    NextWave,
                    Debug,
                    Editor,
                    Disconnect,
                    Tweaking,
                    RetryLevel
                };
            }
        }
    }


    static class MouseConfiguration
    {
        public static MouseButton Select = MouseButton.Left;
        public static MouseButton Back = MouseButton.Right;
        public static MouseButton Cancel = MouseButton.Right;
        public static MouseButton AdvancedView = MouseButton.Middle;
        public static MouseButton SelectionNext = MouseButton.MiddleDown;
        public static MouseButton SelectionPrevious = MouseButton.MiddleUp;
        public static float Speed = 1.5f;


        public static List<MouseButton> ToList
        {
            get
            {
                return new List<MouseButton>()
                {
                    Select,
                    Back,
                    Cancel,
                    AdvancedView,
                    SelectionNext,
                    SelectionPrevious
                };
            }
        }
    }


    static class GamePadConfiguration
    {
        public static Buttons Back = Buttons.Start;
        public static Buttons Cancel = Buttons.B;
        public static Buttons ChangeMusic = Buttons.DPadUp;
        public static Buttons NextWave = Buttons.Y;
        public static Buttons Debug = Buttons.DPadDown;
        public static Buttons Editor = Buttons.LeftShoulder;
        public static Buttons Select = Buttons.A;
        public static Buttons AdvancedView = Buttons.X;
        public static Buttons SelectionNext = Buttons.RightTrigger;
        public static Buttons SelectionPrevious = Buttons.LeftTrigger;
        public static Buttons MoveCursor = Buttons.LeftStick;
        public static Buttons Disconnect = Buttons.Back;
        public static Buttons Tweaking = Buttons.DPadLeft;
        public static Buttons RetryLevel = Buttons.Y;
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
                    NextWave,
                    Debug,
                    Editor,
                    Select,
                    AdvancedView,
                    SelectionNext,
                    SelectionPrevious,
                    Tweaking,
                    RetryLevel
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
