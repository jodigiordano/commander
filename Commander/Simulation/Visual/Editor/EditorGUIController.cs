namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;


    class EditorGUIController
    {
        public Dictionary<EditorPanel, Panel> Panels;

        private Simulator Simulator;

        private CelestialBodiesPathPreviews CelestialBodiesPathPreviews;

        private ContextualMenusCollisions ContextualMenusCollisions;


        public EditorGUIController(Simulator simulator)
        {
            Simulator = simulator;

            CelestialBodiesPathPreviews = new CelestialBodiesPathPreviews(Simulator);

            Panels = new Dictionary<EditorPanel, Panel>(EditorPanelComparer.Default);

            ContextualMenusCollisions = new ContextualMenusCollisions();
        }


        public void Initialize()
        {
            Panels.Clear();

            // Player'input panel
            var playerPanel = new PlayerPanel(Simulator, Vector3.Zero, new Vector2(700, 500), VisualPriorities.Default.EditorPanel, Color.White) { Visible = false };
            Panels.Add(EditorPanel.Player, playerPanel);

            // Turrets' panel
            var turretsPanel = new TurretsAssetsPanel(Simulator, Vector3.Zero, new Vector2(700, 500), VisualPriorities.Default.EditorPanel, Color.White) { Visible = false };
            turretsPanel.Initialize();
            turretsPanel.Sync();
            Panels.Add(EditorPanel.Turrets, turretsPanel);

            // PowerUps' panel
            var powerUpsPanel = new PowerUpsAssetsPanel(Simulator, Vector3.Zero, new Vector2(700, 500), VisualPriorities.Default.EditorPanel, Color.White) { Visible = false };
            powerUpsPanel.Initialize();
            powerUpsPanel.Sync();
            Panels.Add(EditorPanel.PowerUps, powerUpsPanel);

            // Background panel
            var backgroundPanel = new BackgroundsAssetsPanel(Simulator, Vector3.Zero, new Vector2(500, 500), VisualPriorities.Default.EditorPanel, Color.White) { Visible = false };
            backgroundPanel.Initialize();
            Panels.Add(EditorPanel.Background, backgroundPanel);

            // Waves panel
            var wavesPanel = new WavesPanel(Simulator, Vector3.Zero, new Vector2(1200, 600), VisualPriorities.Default.EditorPanel, Color.White) { Visible = false };
            Panels.Add(EditorPanel.Waves, wavesPanel);

            // Celestial Body Assets panel
            var assetsPanel = new CelestialBodyAssetsPanel(Simulator, Vector3.Zero, new Vector2(700, 500), VisualPriorities.Default.EditorPanel, Color.White);
            assetsPanel.Initialize();
            Panels.Add(EditorPanel.CelestialBodyAssets, assetsPanel);

            // Enemies Assets panel
            var enemiesPanel = new EnemiesAssetsPanel(Simulator, Vector3.Zero, new Vector2(700, 500), VisualPriorities.Default.EditorPanel, Color.White);
            enemiesPanel.Initialize();
            Panels.Add(EditorPanel.Enemies, enemiesPanel);

            foreach (var panel in Panels.Values)
                panel.Visible = false;

            CelestialBodiesPathPreviews.Initialize();
        }


        public void DoPlayerChanged(SimPlayer p)
        {
            //var player = Players[p];

            //// Change general menu
            //if (PlayerBrowsingGeneralMenu == null || (player == PlayerBrowsingGeneralMenu &&
            //    player.SelectedGeneralMenu != p.ActualSelection.GeneralMenuChoice))
            //{
            //    if (PlayerBrowsingGeneralMenu == null)
            //        PlayerBrowsingGeneralMenu = player;

            //    if (player == PlayerBrowsingGeneralMenu)
            //        GeneralMenu.DoMenuChanged(player.SelectedGeneralMenu, p.ActualSelection.GeneralMenuChoice, p.Color);

            //    if (player == PlayerBrowsingGeneralMenu && p.ActualSelection.GeneralMenuChoice == EditorGeneralMenuChoice.None)
            //        PlayerBrowsingGeneralMenu = null;
            //}

            //// update actual selection
            //player.SelectedGeneralMenu = p.ActualSelection.GeneralMenuChoice;
            //player.GeneralMenuSubMenuIndex = p.ActualSelection.GeneralMenuSubMenuIndex;
            //player.CelestialBodyMenu.Menu.SelectedIndex = p.ActualSelection.CelestialBodyChoice;

            //if (p.Owner.ActualSelection.TurretToPlace != null)
            //    return;

            //// synchronize celestial body menu
            //if (p.Owner.ActualSelection.CelestialBody != null && player.CelestialBodyMenu.CelestialBody != p.Owner.ActualSelection.CelestialBody)
            //{
            //    player.CelestialBodyMenu.CelestialBody = p.Owner.ActualSelection.CelestialBody;
            //    player.CelestialBodyMenu.SyncData();
            //    player.CelestialBodyMenu.Visible = true;
            //}

            //else if (p.Owner.ActualSelection.CelestialBody == null)
            //{
            //    player.CelestialBodyMenu.CelestialBody = null;
            //    player.CelestialBodyMenu.Visible = false;
            //}

            //player.CelestialBodyMenu.Visible = p.Owner.ActualSelection.EditingState == EditorEditingState.None;
        }


        public void Update()
        {

        }


        public void Draw()
        {
            

            //OrganizeContextualMenus();

            //foreach (var player in Players.Values)
            //    player.Draw();

            //foreach (var panel in Panels.Values)
            //    panel.Draw();

            //if (PlayerBrowsingGeneralMenu != null)
            //{
            //    var menu = GeneralMenu.SubMenus[PlayerBrowsingGeneralMenu.SelectedGeneralMenu];
            //    menu.SelectedIndex = PlayerBrowsingGeneralMenu.GeneralMenuSubMenuIndex;
            //    menu.Draw();
            //}


            //if (GeneralMenu.SubMenuToFadeOut != EditorGeneralMenuChoice.None)
            //{
            //    var menu = GeneralMenu.SubMenus[GeneralMenu.SubMenuToFadeOut];
            //    menu.Position = GeneralMenu.Menus[GeneralMenu.SubMenuToFadeOut].Position;
            //    menu.Draw();
            //}

            //CelestialBodiesPathPreviews.Draw();
        }


        public void DoEditorCommandExecuted(EditorCommand command)
        {
            //if (command.Name == "ShowCelestialBodiesPaths" || command.Name == "HideCelestialBodiesPaths")
            //{
            //    ((EditorToggleContextualMenuChoice) GeneralMenu.SubMenus[EditorGeneralMenuChoice.Battlefield].GetChoice(2)).Next();
            //    return;
            //}


            //if (command.NType == EditorCommandType.Panel)
            //{
            //    var panelCommand = (EditorPanelCommand) command;

            //    Panels[panelCommand.Panel].Fade(panelCommand.Show ? 0 : 255, panelCommand.Show ? 255 : 0, 500);
            //    return;
            //}


            //if (command.NType == EditorCommandType.CelestialBody)
            //    DoEditorCelestialBodyCommandExecuted((EditorCelestialBodyCommand) command);
        }


        private void DoEditorCelestialBodyCommandExecuted(EditorCelestialBodyCommand command)
        {
            //if (command.Name == "AddPlanet")
            //    CelestialBodiesPathPreviews.Sync();
            //else if (command.Name == "Remove")
            //    CelestialBodiesPathPreviews.Sync();
            //else if (command.Name == "ShowPathPreview")
            //    command.CelestialBody.ShowPath = true;
            
            //else if (command.Name == "HidePathPreview")
            //    command.CelestialBody.ShowPath = false;

            //if (command.Owner != null)
            //    Players[command.Owner].CelestialBodyMenu.SyncData();
        }


        private void OrganizeContextualMenus()
        {
            //ContextualMenusCollisions.Menus.Clear();

            //foreach (var p in Players.Values)
            //{
            //    p.Update();

            //    if (p.OpenedMenu != null)
            //        ContextualMenusCollisions.Menus.Add(p.OpenedMenu);
            //}

            //if (PlayerBrowsingGeneralMenu != null)
            //{
            //    var menu = GeneralMenu.SubMenus[PlayerBrowsingGeneralMenu.SelectedGeneralMenu];

            //    menu.Position = GeneralMenu.Menus[PlayerBrowsingGeneralMenu.SelectedGeneralMenu].Position;

            //    ContextualMenusCollisions.Menus.Add(menu);
            //}

            //ContextualMenusCollisions.Sync();
        }
    }
}
