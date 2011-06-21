﻿namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;


    class GUIPlayer
    {
        public SpaceshipCursor Cursor;
        public Cursor Crosshair;
        public SelectedCelestialBodyAnimation SelectedCelestialBodyAnimation;
        public CelestialBodyMenu MenuCelestialBody;
        public TurretMenu MenuTurret;
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
            MenuTurret = new TurretMenu(Simulator, Preferences.PrioriteGUIPanneauCorpsCeleste, color);
            MenuCelestialBody = new CelestialBodyMenu(Simulator, Preferences.PrioriteGUIPanneauCorpsCeleste, color);

            FinalSolutionPreview = new FinalSolutionPreview(Simulator);

            MenuCelestialBody.AvailableTurrets = availableTurrets;
            MenuCelestialBody.Initialize();

            WorldMenu = new WorldMenu(Simulator, Preferences.PrioriteGUIPanneauCorpsCeleste, availableLevelsDemoMenu, color);

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
