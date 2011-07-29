namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Input;
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
        public NewGameMenu NewGameMenu;

        private Simulator Simulator;

        
        public GUIPlayer(Simulator simulator, Dictionary<TurretType, bool> availableTurrets, Dictionary<string, LevelDescriptor> levelsDescriptors, Color color, string representation, InputType inputType)
        {
            Simulator = simulator;

            SelectedCelestialBodyAnimation = new SelectedCelestialBodyAnimation(Simulator);

            Cursor = new SpaceshipCursor(Simulator.Scene, Vector3.Zero, 2, VisualPriorities.Default.PlayerCursor, color, representation);
            Crosshair = new Cursor(Simulator.Scene, Vector3.Zero, 2, Preferences.PrioriteGUIPanneauGeneral, "crosshairRailGun", false);
            TurretMenu = new TurretMenu(Simulator, Preferences.PrioriteGUIPanneauCorpsCeleste, color, inputType);
            CelestialBodyMenu = new CelestialBodyMenu(Simulator, Preferences.PrioriteGUIPanneauCorpsCeleste, color, inputType);

            FinalSolutionPreview = new FinalSolutionPreview(Simulator);

            CelestialBodyMenu.AvailableTurrets = availableTurrets;
            CelestialBodyMenu.Initialize();

            WorldMenu = new WorldMenu(Simulator, Preferences.PrioriteGUIPanneauCorpsCeleste, levelsDescriptors, color);
            NewGameMenu = new NewGameMenu(Simulator, Preferences.PrioriteGUIPanneauCorpsCeleste, color);

            PowerUpInputMode = false;
            PowerUpFinalSolution = false;
        }


        public ContextualMenu OpenedMenu
        {
            get
            {
                // highest priority

                if (WorldMenu.PausedGameMenuVisible)
                    return WorldMenu.PausedGameMenu;

                if (NewGameMenu.Visible)
                    return NewGameMenu;

                if (TurretMenu.Visible)
                    return TurretMenu.Menu;

                if (CelestialBodyMenu.Visible)
                    return CelestialBodyMenu.Menu;

                // lowest priority

                return null;
            }
        }


        public void Update()
        {
            TurretMenu.Update();
            CelestialBodyMenu.Update();
            WorldMenu.Update();
            NewGameMenu.Update();
        }


        public void Draw()
        {
            Cursor.Draw();
            Crosshair.Draw();
            SelectedCelestialBodyAnimation.Draw();

            if (Simulator.WorldMode)
                WorldMenu.Draw();

            if (Simulator.DemoMode)
            {
                NewGameMenu.Draw();
                return;
            }

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
