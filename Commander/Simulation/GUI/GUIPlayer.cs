namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Input;
    using EphemereGames.Core.Visual;
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
        private Text Name;

        
        public GUIPlayer(
            Simulator simulator,
            Dictionary<TurretType, bool> availableTurrets,
            Color color,
            string representation,
            Commander.Player p)
        {
            Simulator = simulator;

            SelectedCelestialBodyAnimation = new SelectedCelestialBodyAnimation(Simulator);

            Cursor = new SpaceshipCursor(Simulator.Scene, Vector3.Zero, 2, VisualPriorities.Default.PlayerCursor, color, representation, true);
            Crosshair = new Cursor(Simulator.Scene, Vector3.Zero, 2, VisualPriorities.Default.PlayerCursor, "crosshairRailGun", false);
            TurretMenu = new TurretMenu(Simulator, VisualPriorities.Default.TurretMenu, color, p);
            CelestialBodyMenu = new CelestialBodyMenu(Simulator, VisualPriorities.Default.CelestialBodyMenu, color, p);

            FinalSolutionPreview = new FinalSolutionPreview(Simulator);

            CelestialBodyMenu.AvailableTurrets = availableTurrets;
            CelestialBodyMenu.Initialize();

            WorldMenu = new WorldMenu(Simulator, VisualPriorities.Default.CelestialBodyMenu, color);
            NewGameMenu = new NewGameMenu(Simulator, VisualPriorities.Default.CelestialBodyMenu, color);

            PowerUpInputMode = false;
            PowerUpFinalSolution = false;

            Name = new Text(p.Index.ToString(), "Pixelite", color, p.Position)
            {
                SizeX = 2,
                VisualPriority = VisualPriorities.Default.PlayerName
            }.CenterIt();
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
            CelestialBodyMenu.Position = Cursor.Position;
            TurretMenu.Position = Cursor.Position;
            WorldMenu.Position = Cursor.Position;
            NewGameMenu.Position = Cursor.Position;

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

            if (Inputs.ConnectedPlayers.Count > 1)
            {
                Name.Position = Simulator.CameraController.ClampToCamera(Cursor.FrontImage.Position, new Vector3(Cursor.FrontImage.AbsoluteSize, 0));
                Name.Origin = new Vector2(Name.AbsoluteSize.X / 4, -Cursor.FrontImage.Size.Y - 5);
                Name.Rotation = Cursor.FrontImage.Rotation;
                Simulator.Scene.Add(Name);
            }

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
