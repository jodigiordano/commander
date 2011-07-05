namespace EphemereGames.Commander.Simulation
{
    using Microsoft.Xna.Framework;


    class EditorPlayerSelection
    {
        public EditorGeneralMenuChoice GeneralMenuChoice;
        public int GeneralMenuSubMenuIndex;

        public int CelestialBodyChoice;

        public CelestialBody CelestialBody;

        public EditorPlayerSelection()
        {
            GeneralMenuChoice = EditorGeneralMenuChoice.None;
            GeneralMenuSubMenuIndex = -1;
            CelestialBodyChoice = -1;
            CelestialBody = null;
        }
    }
}
