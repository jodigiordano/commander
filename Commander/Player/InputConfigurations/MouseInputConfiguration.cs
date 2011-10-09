namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using EphemereGames.Core.Input;


    class MouseInputConfiguration
    {
        public MouseButton Select;
        public MouseButton Cancel;
        public MouseButton AlternateSelect;
        public MouseButton SelectionNext;
        public MouseButton SelectionPrevious;
        public MouseButton Fire;
        public MouseButton Previous;
        public MouseButton Next;

        public Dictionary<MouseButton, string> ToImage;


        public MouseInputConfiguration()
        {
            Select = MouseButton.None;
            Cancel = MouseButton.None;
            AlternateSelect = MouseButton.None;
            SelectionNext = MouseButton.None;
            SelectionPrevious = MouseButton.None;
            Fire = MouseButton.None;
            Previous = MouseButton.None;
            Next = MouseButton.None;
        }


        public List<MouseButton> ToList
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


        public string GetImageName(MouseButton button)
        {
            return ToImage[button];
        }
    }
}
