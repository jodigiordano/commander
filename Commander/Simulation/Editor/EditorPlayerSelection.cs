namespace EphemereGames.Commander.Simulation
{
    enum EditorGeneralMenuChoice
    {
        None,
        File,
        Waves,
        Battlefield,
        Gameplay
    }


    enum EditorPanel
    {
        Player,
        Load,
        Save,
        Delete,
        PowerUps,
        Turrets,
        General,
        Background,
        GeneratePlanetarySystem,
        Waves,
        None
    }


    enum EditorState
    {
        Editing,
        Playtest
    }


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
