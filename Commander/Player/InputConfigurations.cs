namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using EphemereGames.Core.Input;
    using Microsoft.Xna.Framework.Input;
    

    static class KeyboardConfiguration
    {
        public static Keys Next = Keys.Enter;
        public static Keys Back = Keys.Back;
        public static Keys Cancel = Keys.Escape;
        public static Keys ChangeMusic = Keys.RightShift;
        public static Keys NextWave = Keys.RightControl;
        public static Keys Debug = Keys.F1;
        public static Keys Editor = Keys.F2;

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
                    Keys.D0, //tmp
                    Keys.D1,
                    Keys.D2,
                    Keys.D3,
                    Keys.D4,
                    Keys.D5,
                    Keys.D6,
                    Keys.D7,
                    Keys.D8,
                    Keys.D9,
                    Keys.NumPad0,
                    Keys.NumPad1,
                    Keys.NumPad2,
                    Keys.NumPad3,
                    Keys.NumPad4,
                    Keys.NumPad5,
                    Keys.NumPad6,
                    Keys.NumPad7,
                    Keys.NumPad8,
                    Keys.NumPad9,
                    Keys.Back
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
        public static float Speed = 2;


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
        public static Buttons Back2 = Buttons.Back;
        public static Buttons Cancel = Buttons.B;
        public static Buttons ChangeMusic = Buttons.DPadUp;
        public static Buttons NextWave = Buttons.Y;
        public static Buttons Debug = Buttons.Back;
        public static Buttons Editor = Buttons.LeftShoulder;
        public static Buttons Select = Buttons.A;
        public static Buttons AdvancedView = Buttons.X;
        public static Buttons SelectionNext = Buttons.RightTrigger;
        public static Buttons SelectionPrevious = Buttons.LeftTrigger;
        public static Buttons MoveCursor = Buttons.LeftStick;
        public static float Speed = 15;


        public static List<Buttons> ToList
        {
            get
            {
                return new List<Buttons>()
                {
                    Back,
                    Back2,
                    Cancel,
                    ChangeMusic,
                    NextWave,
                    Debug,
                    Editor,
                    Select,
                    AdvancedView,
                    SelectionNext,
                    SelectionPrevious
                };
            }
        }
    }
}
