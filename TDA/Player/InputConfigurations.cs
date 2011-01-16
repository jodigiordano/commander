namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using EphemereGames.Core.Input;
    using Microsoft.Xna.Framework.Input;
    

    class KeyboardConfiguration
    {
        public Keys Next = Keys.Enter;
        public Keys Back = Keys.Back;
        public Keys Cancel = Keys.Escape;
        public Keys ChangeMusic = Keys.RightShift;
        public Keys NextWave = Keys.RightControl;
        public Keys Debug = Keys.F1;
        public Keys Editor = Keys.F2;

        public List<Keys> ToList
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


    class MouseConfiguration
    {
        public MouseButton Select = MouseButton.Left;
        public MouseButton Back = MouseButton.Right;
        public MouseButton Cancel = MouseButton.Right;
        public MouseButton AdvancedView = MouseButton.Middle;
        public MouseButton SelectionNext = MouseButton.MiddleDown;
        public MouseButton SelectionPrevious = MouseButton.MiddleUp;
        public float Speed = 2;


        public List<MouseButton> ToList
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


    class GamePadConfiguration
    {
        public Buttons Back = Buttons.Start;
        public Buttons Cancel = Buttons.B;
        public Buttons ChangeMusic = Buttons.DPadUp;
        public Buttons NextWave = Buttons.Y;
        public Buttons Debug = Buttons.Back;
        public Buttons Editor = Buttons.LeftShoulder;
        public Buttons Select = Buttons.A;
        public Buttons AdvancedView = Buttons.X;
        public Buttons SelectionNext = Buttons.RightTrigger;
        public Buttons SelectionPrevious = Buttons.LeftTrigger;
        public Buttons PilotSpaceShip = Buttons.LeftStick;
        public Buttons MoveCursor = Buttons.LeftStick;
        public float Speed = 10;


        public List<Buttons> ToList
        {
            get
            {
                return new List<Buttons>()
                {
                    Back,
                    Cancel,
                    ChangeMusic,
                    NextWave,
                    Debug,
                    Editor,
                    Select,
                    AdvancedView,
                    SelectionNext,
                    SelectionPrevious,
                    PilotSpaceShip,
                };
            }
        }
    }
}
