namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework.Input;


    class GamepadInputConfiguration
    {
        public Buttons Back;
        public Buttons Cancel;
        public Buttons ChangeMusic;
        public Buttons Debug;
        public Buttons Tweaking;
        public Buttons Select;
        public Buttons AlternateSelect;
        public Buttons AdvancedView;
        public Buttons SelectionNext;
        public Buttons SelectionPrevious;
        public Buttons AlternateSelectionNext;
        public Buttons AlternateSelectionPrevious;
        public Buttons MoveCursor;
        public Buttons DirectionCursor;
        public Buttons Disconnect;
        public Buttons RetryLevel;
        public Buttons Cheat1;
        public Buttons ZoomIn;
        public Buttons ZoomOut;

        public Dictionary<Buttons, string> ToImage;


        public GamepadInputConfiguration()
        {
            Back = Buttons.BigButton;
            Cancel = Buttons.BigButton;
            ChangeMusic = Buttons.BigButton;
            Debug = Buttons.BigButton;
            Tweaking = Buttons.BigButton;
            Select = Buttons.BigButton;
            AlternateSelect = Buttons.BigButton;
            AdvancedView = Buttons.BigButton;
            SelectionNext = Buttons.BigButton;
            SelectionPrevious = Buttons.BigButton;
            AlternateSelectionNext = Buttons.BigButton;
            AlternateSelectionPrevious = Buttons.BigButton;
            MoveCursor = Buttons.BigButton;
            DirectionCursor = Buttons.BigButton;
            Disconnect = Buttons.BigButton;
            RetryLevel = Buttons.BigButton;
            Cheat1 = Buttons.BigButton;
            ZoomIn = Buttons.BigButton;
            ZoomOut = Buttons.BigButton;
        }


        public List<Buttons> ToList
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
                    Cheat1,
                    ZoomIn,
                    ZoomOut
                };
            }
        }


        public string GetImageName(Buttons button)
        {
            return ToImage[button];
        }
    }
}
