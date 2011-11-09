namespace EphemereGames.Commander.Simulation.Player
{
    abstract class WorldMenu : ContextualMenu
    {
        private bool AlternateSelectedText;

        protected SimPlayer Owner;


        public WorldMenu(Simulator simulator, double visualPriority, SimPlayer owner)
            : base(simulator, visualPriority, owner.Color, 15)
        {
            Owner = owner;

            AlternateSelectedText = owner.Color == Colors.Spaceship.Yellow;
        }


        public override void NextChoice()
        {
            ((TextContextualMenuChoice) Choices[SelectedIndex]).SetColor(Owner.InnerPlayer.GetCMColor(true, false));
            base.NextChoice();
            ((TextContextualMenuChoice) Choices[SelectedIndex]).SetColor(Owner.InnerPlayer.GetCMColor(true, true));
        }


        public override void PreviousChoice()
        {
            ((TextContextualMenuChoice) Choices[SelectedIndex]).SetColor(Owner.InnerPlayer.GetCMColor(true, false));
            base.PreviousChoice();
            ((TextContextualMenuChoice) Choices[SelectedIndex]).SetColor(Owner.InnerPlayer.GetCMColor(true, true));
        }


        //public override void Draw()
        //{
        //    if (!Visible)
        //        return;

        //    base.Draw();

            //if (EditorMenuVisible)
            //{
            //    int newIndex = (int) EditorChoice;

            //    //if (AlternateSelectedText && EditorMenu.Choices.Count > 0 && newIndex != EditorMenu.SelectedIndex)
            //    //{
            //    //    if (EditorMenu.SelectedIndex >= 0)
            //    //        ((TextContextualMenuChoice) PausedGameMenu.Choices[PausedGameMenu.SelectedIndex]).SetColor(Color.White);

            //    //    if (newIndex >= 0)
            //    //        ((TextContextualMenuChoice) PausedGameMenu.Choices[newIndex]).SetColor(Colors.Spaceship.Selected);
            //    //}

            //    EditorMenu.SelectedIndex = newIndex;
            //    EditorMenu.Draw();
            //}

            //else if (MenuCheckedIn && PausedGameMenuVisible)
            //{
            //    int newIndex = (int) PauseChoice;

            //    if (AlternateSelectedText && PausedGameMenu.Choices.Count > 0 && newIndex != PausedGameMenu.SelectedIndex)
            //    {
            //        if (PausedGameMenu.SelectedIndex >= 0)
            //            ((TextContextualMenuChoice) PausedGameMenu.Choices[PausedGameMenu.SelectedIndex]).SetColor(Color.White);

            //        if (newIndex >= 0)
            //            ((TextContextualMenuChoice) PausedGameMenu.Choices[newIndex]).SetColor(Colors.Spaceship.Selected);
            //    }

            //    PausedGameMenu.SelectedIndex = newIndex;
            //    PausedGameMenu.Draw();
            //}
        //}
    }
}
