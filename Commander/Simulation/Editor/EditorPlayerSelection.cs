namespace EphemereGames.Commander.Simulation
{
    class EditorPlayerSelection
    {
        public EditorGeneralMenuChoice GeneralMenuChoice;
        public int GeneralMenuSubMenuIndex;

        public int CelestialBodyChoice;


        public EditorPlayerSelection()
        {
            GeneralMenuChoice = EditorGeneralMenuChoice.None;
            GeneralMenuSubMenuIndex = -1;
            CelestialBodyChoice = -1;
        }
    }
}
