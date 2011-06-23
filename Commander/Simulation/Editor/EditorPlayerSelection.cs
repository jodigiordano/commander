namespace EphemereGames.Commander.Simulation
{
    enum EditorGeneralMenuAction
    {
        File,
        Waves,
        Battlefield,
        Gameplay,
        None
    }


    class EditorPlayerSelection
    {
        public EditorGeneralMenuAction GeneralMenu;
        public int GeneralMenuSubMenuIndex;


        public EditorPlayerSelection()
        {
            GeneralMenu = EditorGeneralMenuAction.None;
            GeneralMenuSubMenuIndex = -1;
        }
    }
}
