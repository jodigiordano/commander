namespace EphemereGames.Commander.Simulation.Player
{
    using System.Collections.Generic;
    using EphemereGames.Core.Input;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class VisualPlayer
    {
        // General
        public VisualSpaceship CurrentVisual;
        public SelectedCelestialBodyAnimation SelectedCelestialBodyAnimation;
        
        // In game
        public Cursor Crosshair;
        public FinalSolutionPreview FinalSolutionPreview;
        public bool PowerUpInputMode;
        public bool PowerUpFinalSolution;

        // Other
        private SimPlayer Owner;
        private Simulator Simulator;
        private Text Name;
        private VisualSpaceship Visual;
        private VisualSpaceship PanelVisual;
        private LevelInfos LevelInfos;

        // Menus
        public EditorCelestialBodyMenu EditorCelestialBodyMenu;
        public EditorWorldMenu EditorWorldMenu;
        public EditorBuildMenu EditorBuildMenu;
        private CelestialBodyMenu CelestialBodyMenu;
        private NewGameMenu NewGameMenu;
        private List<ContextualMenu> Menus;

        
        public VisualPlayer(Simulator simulator, SimPlayer owner)
        {
            Simulator = simulator;
            Owner = owner;

            SelectedCelestialBodyAnimation = new SelectedCelestialBodyAnimation(Simulator);

            Visual = new VisualSpaceship(Simulator.Scene, Vector3.Zero, 2, VisualPriorities.Default.VisualPlayer, Owner.Color, Owner.ImageName, true);
            PanelVisual = new VisualSpaceship(Simulator.Scene, Vector3.Zero, 2, VisualPriorities.Default.VisualPanelPlayer, Owner.Color, Owner.ImageName, false);
            PanelVisual.Alpha = 0;
            CurrentVisual = Visual;
            
            Crosshair = new Cursor(Simulator.Scene, Vector3.Zero, 2, VisualPriorities.Default.VisualPlayer, "crosshairRailGun", false);

            FinalSolutionPreview = new FinalSolutionPreview(Simulator, Owner);

            PowerUpInputMode = false;
            PowerUpFinalSolution = false;

            Name = new Text(Owner.InnerPlayer.Index.ToString(), "Pixelite", Owner.Color, Owner.Position)
            {
                VisualPriority = VisualPriorities.Default.PlayerName
            }.CenterIt();

            Menus = new List<ContextualMenu>();

            // Highest priority
            EditorCelestialBodyMenu = new EditorCelestialBodyMenu(Simulator, VisualPriorities.Default.CelestialBodyMenu, Owner);
            EditorWorldMenu = new EditorWorldMenu(Simulator, VisualPriorities.Default.CelestialBodyMenu, Owner);
            EditorBuildMenu = new EditorBuildMenu(Simulator, VisualPriorities.Default.CelestialBodyMenu, Owner);
            CelestialBodyMenu = new CelestialBodyMenu(Simulator, VisualPriorities.Default.CelestialBodyMenu, Owner);
            NewGameMenu = new NewGameMenu(Simulator, VisualPriorities.Default.CelestialBodyMenu, Owner);

            Menus.Add(EditorCelestialBodyMenu);
            Menus.Add(new PauseWorldMenu(Simulator, VisualPriorities.Default.CelestialBodyMenu, Owner));
            Menus.Add(NewGameMenu);
            Menus.Add(EditorWorldMenu);
            Menus.Add(new EditorWorldLevelMenu(Simulator, VisualPriorities.Default.CelestialBodyMenu, Owner));
            Menus.Add(EditorBuildMenu);
            Menus.Add(new StartingPathMenu(Simulator, VisualPriorities.Default.CelestialBodyMenu, Owner));
            Menus.Add(new TurretMenu(Simulator, VisualPriorities.Default.TurretMenu, Owner));
            Menus.Add(CelestialBodyMenu);
            // Lowest priority

            LevelInfos = new LevelInfos(Simulator, Owner);
        }


        public bool Visible
        {
            get { return CurrentVisual.Visible; }
        }


        public Dictionary<TurretType, bool> AvailableTurrets
        {
            set
            {
                CelestialBodyMenu.AvailableTurrets = value;
            }
        }


        public void Initialize()
        {
            foreach (var m in Menus)
                m.Initialize();
        }

        public void SyncNewGameMenu()
        {
            NewGameMenu.Initialize();
        }


        public ContextualMenu GetOpenedMenu()
        {
            foreach (var m in Menus)
                if (m.Visible)
                    return m;

            return null;
        }


        public void Update()
        {
            foreach (var m in Menus)
            {
                m.Position = CurrentVisual.Position;
                m.Update();
            }
        }


        public void SwitchToPanelVisual()
        {
            PanelVisual.FadeIn();

            if (Visual.Alpha > 100)
                Visual.FadeOut(100);

            CurrentVisual = PanelVisual;
        }


        public void SwitchToVisual()
        {
            PanelVisual.FadeOut();

            if (Owner.ActualSelection.TurretToPlace == null)
                Visual.FadeIn();

            CurrentVisual = Visual;
        }


        public void TeleportIn()
        {
            CurrentVisual.TeleportIn();
        }


        public void TeleportOut()
        {
            CurrentVisual.TeleportOut();
        }


        public void Draw()
        {
            // draw cursors
            Visual.Draw();
            PanelVisual.Draw();
            Crosshair.Draw();

            SelectedCelestialBodyAnimation.Draw();

            if (Inputs.ConnectedPlayers.Count > 1)
            {
                Name.Position = Simulator.CameraController.ClampToCamera(
                    CurrentVisual.FrontImage.Position + new Vector3(0, CurrentVisual.FrontImage.AbsoluteSize.Y / 2 + 5, 0),
                    new Vector3(CurrentVisual.FrontImage.AbsoluteSize, 0));
                Simulator.Scene.Add(Name);
            }

            var menu = GetOpenedMenu();

            if (menu != null)
                menu.Draw();

            if (Simulator.DemoMode && Simulator.WorldMode && !Simulator.EditorEditingMode)
                LevelInfos.Draw();

            if (Simulator.DemoMode)
                return;

            FinalSolutionPreview.Draw();
        }
    }
}
