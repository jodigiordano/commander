namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.Physics;
    using Microsoft.Xna.Framework;


    class EditorPlayer
    {
        public SimPlayer SimPlayer;
        public EditorGeneralMenu GeneralMenu;

        public EditorPlayerSelection ActualSelection;
        public Circle Circle { get { return SimPlayer.Circle; } }
        public Color Color { get { return SimPlayer.Color; } }

        private Simulator Simulator;


        public EditorPlayer(Simulator simulator)
        {
            Simulator = simulator;

            ActualSelection = new EditorPlayerSelection();
        }


        public void Initialize()
        {

        }


        public void Update()
        {
            CheckGeneralMenu();

            // main menu
            if (ActualSelection.GeneralMenuChoice == EditorGeneralMenuChoice.None)
                ActualSelection.GeneralMenuSubMenuIndex = -1;

            if (ActualSelection.GeneralMenuChoice != EditorGeneralMenuChoice.None && ActualSelection.GeneralMenuSubMenuIndex == -1)
                ActualSelection.GeneralMenuSubMenuIndex = 0;

            // celestial body
            if (SimPlayer.ActualSelection.CelestialBody == null)
                ActualSelection.CelestialBodyChoice = -1;

            if (SimPlayer.ActualSelection.CelestialBody != null && ActualSelection.CelestialBodyChoice == -1)
                ActualSelection.CelestialBodyChoice = 0;
        }


        public void DoSelectAction()
        {
            if (SimPlayer.ActualSelection.CelestialBody == null)
                return;

            if (ActualSelection.CelestialBodyChoice == 8)
                SimPlayer.ActualSelection.EditingState = EditorEditingState.MovingCB;
            else if (ActualSelection.CelestialBodyChoice == 9)
                SimPlayer.ActualSelection.EditingState = EditorEditingState.RotatingCB;
            else if (ActualSelection.CelestialBodyChoice == 10)
                SimPlayer.ActualSelection.EditingState = EditorEditingState.ShrinkingCB;
            else if (ActualSelection.CelestialBodyChoice == 11)
                SimPlayer.ActualSelection.EditingState = EditorEditingState.StartPosCB;

            if (SimPlayer.ActualSelection.EditingState != EditorEditingState.None)
                ActualSelection.CelestialBody = SimPlayer.ActualSelection.CelestialBody;
        }


        public void DoCancelAction()
        {
            if (SimPlayer.ActualSelection.EditingState == EditorEditingState.MovingCB)
                SimPlayer.NinjaPosition = ActualSelection.CelestialBody.Position;

            ActualSelection.CelestialBody = null;

            SimPlayer.ActualSelection.EditingState = EditorEditingState.None;
        }


        public void NextGeneralMenuChoice()
        {
            if (ActualSelection.GeneralMenuChoice == EditorGeneralMenuChoice.None)
                return;

            int nbChoices = GeneralMenu.SubMenus[ActualSelection.GeneralMenuChoice].ChoicesCount;

            ActualSelection.GeneralMenuSubMenuIndex++;

            if (ActualSelection.GeneralMenuSubMenuIndex >= nbChoices)
                ActualSelection.GeneralMenuSubMenuIndex = 0;
        }


        public void PreviousGeneralMenuChoice()
        {
            if (ActualSelection.GeneralMenuChoice == EditorGeneralMenuChoice.None)
                return;

            int nbChoices = GeneralMenu.SubMenus[ActualSelection.GeneralMenuChoice].ChoicesCount;

            ActualSelection.GeneralMenuSubMenuIndex--;

            if (ActualSelection.GeneralMenuSubMenuIndex < 0)
                ActualSelection.GeneralMenuSubMenuIndex = nbChoices - 1;
        }


        public void NextCelestialBodyChoice()
        {
            if (ActualSelection.CelestialBodyChoice == -1)
                return;

            int actual = (int) ActualSelection.CelestialBodyChoice;
            int nbChoices = 15;

            actual += 1;

            if (actual >= nbChoices)
                actual = 0;

            ActualSelection.CelestialBodyChoice = actual;
        }


        public void PreviousCelestialBodyChoice()
        {
            if (ActualSelection.CelestialBodyChoice == -1)
                return;

            int actual = (int) ActualSelection.CelestialBodyChoice;
            int nbChoices = 15;

            actual -= 1;

            if (actual < 0)
                actual = nbChoices - 1;

            ActualSelection.CelestialBodyChoice = actual;
        }


        private void CheckGeneralMenu()
        {
            ActualSelection.GeneralMenuChoice = GeneralMenu.GetSelection(Circle);
        }
    }
}