namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework.Input;


    class KeyboardInputConfiguration
    {
        public Keys AdvancedView;
        public Keys Next;
        public Keys Previous;
        public Keys Back;
        public Keys ChangeMusic;
        public Keys Debug;
        public Keys Editor;
        public Keys Disconnect;
        public Keys Tweaking;
        public Keys RetryLevel;
        public Keys MoveUp;
        public Keys MoveLeft;
        public Keys MoveDown;
        public Keys MoveRight;
        public Keys QuickToggle;
        public Keys SelectionNext;
        public Keys SelectionPrevious;
        public Keys Cheat1;
        public Keys AlternateSelect;
        public Keys Cancel;
        public Keys MoveCursor;
        public Keys Select;
        public Keys RotateLeft;
        public Keys RotateRight;
        public Keys Fire;
        public Keys Home;
        public Keys LeftCoin;
        public Keys RightCoin;

        public Dictionary<Keys, string> ToImage;


        public KeyboardInputConfiguration()
        {
            AdvancedView = Keys.None;
            Next = Keys.None;
            Previous = Keys.None;
            Back = Keys.None;
            ChangeMusic = Keys.None;
            Debug = Keys.None;
            Editor = Keys.None;
            Disconnect = Keys.None;
            Tweaking = Keys.None;
            RetryLevel = Keys.None;
            MoveUp = Keys.None;
            MoveLeft = Keys.None;
            MoveDown = Keys.None;
            MoveRight = Keys.None;
            QuickToggle = Keys.None;
            SelectionNext = Keys.None;
            SelectionPrevious = Keys.None;
            Cheat1 = Keys.None;
            AlternateSelect = Keys.None;
            Cancel = Keys.None;
            MoveCursor = Keys.None;
            Select = Keys.None;
            RotateLeft = Keys.None;
            RotateRight = Keys.None;
            Fire = Keys.None;
            Home = Keys.None;
            LeftCoin = Keys.None;
            RightCoin = Keys.None;
        }


        public List<Keys> ToList
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
                    Cheat1,
                    AlternateSelect,
                    Cancel,
                    MoveCursor,
                    Select,
                    Home,
                    LeftCoin,
                    RightCoin,
                    RotateLeft,
                    RotateRight
                };
            }
        }


        public string GetImageName(Keys key)
        {
            return ToImage[key];
        }
    }
}
