namespace EphemereGames.Commander.Simulation.Player
{
    //class EditorPlayer
    //{
    //    public EditorGUIPlayer GUIPlayer;


    //    public EditorPlayer(Simulator simulator, SimPlayer innerPlayer)
    //    {
    //        GUIPlayer = new EditorGUIPlayer(Simulator, this);
    //    }


        //public void Update()
        //{
        //    LastSelection.Sync(ActualSelection);

        //    CheckGeneralMenu();

        //    // main menu
        //    if (ActualSelection.GeneralMenuChoice == EditorGeneralMenuChoice.None)
        //        ActualSelection.GeneralMenuSubMenuIndex = -1;

        //    if ((ActualSelection.GeneralMenuChoice != EditorGeneralMenuChoice.None && ActualSelection.GeneralMenuSubMenuIndex == -1) ||
        //        ActualSelection.GeneralMenuChoiceChanged)
        //        ActualSelection.GeneralMenuSubMenuIndex = 0;


        //    // celestial body
        //    if (InnerPlayer.ActualSelection.CelestialBody == null)
        //        ActualSelection.CelestialBodyChoice = -1;

        //    if (InnerPlayer.ActualSelection.CelestialBody != null && ActualSelection.CelestialBodyChoice == -1)
        //        ActualSelection.CelestialBodyChoice = 0;
        //}


        //public void DoSelectAction()
        //{
        //    if (InnerPlayer.ActualSelection.CelestialBody == null)
        //        return;

        //    if (Simulator.EditorState == EditorState.Editing)
        //    {
        //        if (ActualSelection.CelestialBodyChoice == 0)
        //            InnerPlayer.ActualSelection.EditingState = EditorEditingState.MovingCB;
        //        else if (ActualSelection.CelestialBodyChoice == 1)
        //            InnerPlayer.ActualSelection.EditingState = EditorEditingState.RotatingCB;
        //        else if (ActualSelection.CelestialBodyChoice == 2)
        //            InnerPlayer.ActualSelection.EditingState = EditorEditingState.TrajectoryCB;
        //        else if (ActualSelection.CelestialBodyChoice == 10)
        //            InnerPlayer.ActualSelection.EditingState = EditorEditingState.StartPosCB;
        //    }

        //    if (InnerPlayer.ActualSelection.EditingState != EditorEditingState.None)
        //        ActualSelection.CelestialBody = InnerPlayer.ActualSelection.CelestialBody;
        //}


        //public void DoCancelAction()
        //{
        //    if (InnerPlayer.ActualSelection.EditingState == EditorEditingState.MovingCB)
        //        InnerPlayer.NinjaPosition = ActualSelection.CelestialBody.Position;

        //    ActualSelection.CelestialBody = null;

        //    InnerPlayer.ActualSelection.EditingState = EditorEditingState.None;
        //}
    //}
}