namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;


    class GUIPlayer
    {
        public SpaceshipCursor Cursor;
        public Cursor Crosshair;
        public SelectedCelestialBodyAnimation SelectedCelestialBodyAnimation;
        public CelestialBodyMenu CelestialBodyMenu;
        public TurretMenu TurretMenu;
        public FinalSolutionPreview FinalSolutionPreview;
        public bool PowerUpInputMode;
        public bool PowerUpFinalSolution;
        public WorldMenu WorldMenu;

        private Simulator Simulator;

        
        public GUIPlayer(Simulator simulator, Dictionary<TurretType, bool> availableTurrets, Dictionary<string, LevelDescriptor> availableLevelsDemoMenu, Color color, string representation)
        {
            Simulator = simulator;

            SelectedCelestialBodyAnimation = new SelectedCelestialBodyAnimation(Simulator);

            Cursor = new SpaceshipCursor(Simulator.Scene, Vector3.Zero, 2, Preferences.PrioriteGUIPanneauGeneral, color, representation);
            Crosshair = new Cursor(Simulator.Scene, Vector3.Zero, 2, Preferences.PrioriteGUIPanneauGeneral, "crosshairRailGun", false);
            TurretMenu = new TurretMenu(Simulator, Preferences.PrioriteGUIPanneauCorpsCeleste, color);
            CelestialBodyMenu = new CelestialBodyMenu(Simulator, Preferences.PrioriteGUIPanneauCorpsCeleste, color);

            FinalSolutionPreview = new FinalSolutionPreview(Simulator);

            CelestialBodyMenu.AvailableTurrets = availableTurrets;
            CelestialBodyMenu.Initialize();

            WorldMenu = new WorldMenu(Simulator, Preferences.PrioriteGUIPanneauCorpsCeleste, availableLevelsDemoMenu, color);

            PowerUpInputMode = false;
            PowerUpFinalSolution = false;
        }


        public ContextualMenu OpenedMenu
        {
            get
            {
                if (TurretMenu.Visible)
                    return TurretMenu.Menu;

                if (CelestialBodyMenu.Visible)
                    return CelestialBodyMenu.Menu;

                if (WorldMenu.PausedGameMenuVisible)
                    return WorldMenu.PausedGameMenu;

                return null;
            }
        }


        public void Update()
        {
            TurretMenu.Update();
            CelestialBodyMenu.Update();
            WorldMenu.Update();
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

            if (Simulator.EditorMode && Simulator.EditorState == EditorState.Editing)
                return;

            CelestialBodyMenu.Draw();
            TurretMenu.Draw();
        }
    }
}
