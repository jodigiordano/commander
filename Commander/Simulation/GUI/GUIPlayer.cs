namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;


    class GUIPlayer
    {
        public Cursor Cursor;
        public Cursor Crosshair;
        public SelectedCelestialBodyAnimation SelectedCelestialBodyAnimation;
        public CelestialBodyMenu MenuCelestialBody;
        public TurretMenu MenuTurret;
        public FinalSolutionPreview FinalSolutionPreview;
        public bool PowerUpInputMode;
        public bool PowerUpFinalSolution;
        public WorldMenu WorldMenu;

        private Simulator Simulator;

        
        public GUIPlayer(Simulator simulator, Dictionary<TurretType, bool> availableTurrets, Dictionary<string, LevelDescriptor> availableLevelsDemoMenu)
        {
            Simulator = simulator;

            SelectedCelestialBodyAnimation = new SelectedCelestialBodyAnimation(Simulator);

            Cursor = new Cursor(Simulator.Scene, Vector3.Zero, 2, Preferences.PrioriteGUIPanneauGeneral);
            Crosshair = new Cursor(Simulator.Scene, Vector3.Zero, 2, Preferences.PrioriteGUIPanneauGeneral, "crosshairRailGun", false);
            MenuTurret = new TurretMenu(Simulator, Preferences.PrioriteGUIPanneauGeneral + 0.03f);
            MenuCelestialBody = new CelestialBodyMenu(Simulator, Preferences.PrioriteGUIPanneauGeneral + 0.03f);

            FinalSolutionPreview = new FinalSolutionPreview(Simulator);

            MenuCelestialBody.AvailableTurrets = availableTurrets;
            MenuCelestialBody.Initialize();

            WorldMenu = new WorldMenu(Simulator, Preferences.PrioriteGUIPanneauCorpsCeleste - 0.01f, availableLevelsDemoMenu);

            PowerUpInputMode = false;
            PowerUpFinalSolution = false;
        }


        public void Draw()
        {
            Cursor.Draw();
            Crosshair.Draw();
            SelectedCelestialBodyAnimation.Draw();

            if (Simulator.WorldMode)
                WorldMenu.Draw();

            if (Simulator.DemoMode)
                return;

            FinalSolutionPreview.Draw();

            if (PowerUpInputMode)
                return;

            MenuCelestialBody.Draw();
            MenuTurret.Draw();
        }
    }
}
