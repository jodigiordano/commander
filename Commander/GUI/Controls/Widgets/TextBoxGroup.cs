namespace EphemereGames.Commander
{
    using System.Collections.Generic;

    
    class TextBoxGroup
    {
        public TextBox Focus;

        private List<TextBox> TextBoxes;


        public TextBoxGroup()
            : this(new List<TextBox>())
        {

        }


        public TextBoxGroup(List<TextBox> textBoxes)
        {
            TextBoxes = new List<TextBox>();
            TextBoxes.AddRange(textBoxes);
            Focus = null;
        }


        public void Add(TextBox tb)
        {
            TextBoxes.Add(tb);
        }


        public void SwitchTo(TextBox box)
        {
            foreach (var t in TextBoxes)
                t.Focus = false;

            Focus = null;

            if (box != null)
            {
                Focus = box;
                Focus.Focus = true;
            }
        }


        public void Toggle()
        {
            if (Focus == null)
                return;

            var index = TextBoxes.FindIndex(tb => tb == Focus);

            if (index == -1)
                return;

            index++;
            index %= TextBoxes.Count;

            Focus.Focus = false;
            Focus = TextBoxes[index];
            Focus.Focus = true;
        }
    }
}
