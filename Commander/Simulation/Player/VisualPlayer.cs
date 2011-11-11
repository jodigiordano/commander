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
        private Dictionary<string, ContextualMenu> Menus;
        private Dictionary<string, ContextualMenu> CBMenus;

        
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

            Menus = new Dictionary<string, ContextualMenu>();
            CBMenus = new Dictionary<string, ContextualMenu>();

            // Highest priority
            CBMenus.Add("EditorPlanetCB", new EditorPlanetCBMenu(Simulator, VisualPriorities.Default.CelestialBodyMenu, Owner));
            CBMenus.Add("EditorPinkHoleCB", new EditorPinkHoleCBMenu(Simulator, VisualPriorities.Default.EditorPanel, Owner));

            foreach (var m in CBMenus)
                Menus.Add(m.Key, m.Value);

            Menus.Add("WorldPause", new PauseWorldMenu(Simulator, VisualPriorities.Default.CelestialBodyMenu, Owner));
            Menus.Add("MainMenuCampaign", new CampaignMenu(Simulator, VisualPriorities.Default.CelestialBodyMenu, Owner));
            Menus.Add("EditorWorld", new EditorWorldMenu(Simulator, VisualPriorities.Default.CelestialBodyMenu, Owner));
            Menus.Add("EditorWorldLevel", new EditorWorldLevelMenu(Simulator, VisualPriorities.Default.CelestialBodyMenu, Owner));
            Menus.Add("EditorBuildLevel", new EditorLevelBuildMenu(Simulator, VisualPriorities.Default.CelestialBodyMenu, Owner));
            Menus.Add("EditorBuildWorld", new EditorWorldBuildMenu(Simulator, VisualPriorities.Default.CelestialBodyMenu, Owner));
            Menus.Add("StartingPath", new StartingPathMenu(Simulator, VisualPriorities.Default.CelestialBodyMenu, Owner));
            Menus.Add("Turret", new TurretMenu(Simulator, VisualPriorities.Default.TurretMenu, Owner));
            Menus.Add("CB", new CelestialBodyMenu(Simulator, VisualPriorities.Default.CelestialBodyMenu, Owner));
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
                ((CelestialBodyMenu) Menus["CB"]).AvailableTurrets = value;
            }
        }


        public void Initialize()
        {
            foreach (var m in Menus.Values)
                m.Initialize();
        }


        public void SetMenuVisibility(string menu, bool visible)
        {
            if (menu == "EditorCB")
            {
                foreach (var m in CBMenus.Values)
                    m.Visible = visible;

                return;
            }

            Menus[menu].Visible = visible;
        }


        public void SyncCampaignMenu()
        {
            Menus["MainMenuCampaign"].Initialize();
        }


        public ContextualMenu GetOpenedMenu()
        {
            foreach (var m in Menus.Values)
                if (m.Visible)
                    return m;

            return null;
        }


        public void Update()
        {
            foreach (var m in Menus.Values)
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
