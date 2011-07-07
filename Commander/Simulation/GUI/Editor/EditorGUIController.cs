namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;


    class EditorGUIController
    {
        public List<CelestialBody> CelestialBodies;

        public EditorGeneralMenu GeneralMenu;
        public Dictionary<EditorPanel, Panel> Panels;
        public Dictionary<EditorPlayer, EditorGUIPlayer> Players;

        private Simulator Simulator;
        private EditorGUIPlayer PlayerBrowsingGeneralMenu;

        private CelestialBodiesPathPreviews CelestialBodiesPathPreviews;

        private ContextualMenusCollisions ContextualMenusCollisions;


        public EditorGUIController(Simulator simulator)
        {
            Simulator = simulator;

            GeneralMenu = new EditorGeneralMenu(simulator, new Vector3(400, 300, 0), Preferences.PrioriteSimulationCorpsCeleste);

            Players = new Dictionary<EditorPlayer, EditorGUIPlayer>();

            CelestialBodiesPathPreviews = new CelestialBodiesPathPreviews(Simulator);

            Panels = new Dictionary<EditorPanel, Panel>(EditorPanelComparer.Default);

            // Player's panel
            PlayerPanel playerPanel = new PlayerPanel(Simulator, Vector3.Zero, new Vector2(500, 500), Preferences.PrioriteGUIPanneauGeneral, Color.White) { Visible = false };
            Panels.Add(EditorPanel.Player, playerPanel);

            // Turrets' panel
            TurretsPanel turretsPanel = new TurretsPanel(Simulator, Vector3.Zero, new Vector2(500, 500), Preferences.PrioriteGUIPanneauGeneral, Color.White) { Visible = false };
            Panels.Add(EditorPanel.Turrets, turretsPanel);

            // PowerUps' panel
            PowerUpsPanel powerUpsPanel = new PowerUpsPanel(Simulator, Vector3.Zero, new Vector2(500, 500), Preferences.PrioriteGUIPanneauGeneral, Color.White) { Visible = false };
            Panels.Add(EditorPanel.PowerUps, powerUpsPanel);

            // General panel
            GeneralPanel generalPanel = new GeneralPanel(Simulator, Vector3.Zero, new Vector2(500, 500), Preferences.PrioriteGUIPanneauGeneral, Color.White) { Visible = false };
            Panels.Add(EditorPanel.General, generalPanel);

            // Background panel
            GridPanel backgroundPanel = new GridPanel(Simulator.Scene, Vector3.Zero, new Vector2(600, 500), Preferences.PrioriteGUIPanneauGeneral, Color.White) { Visible = false, NbColumns = 4 };
            backgroundPanel.SetTitle("Background");

            for (int i = 1; i <= 16; i++)
                backgroundPanel.AddWidget("fondecran" + i, new ImageWidget("fondecran" + i, 0.1f));
            Panels.Add(EditorPanel.Background, backgroundPanel);

            // Waves panel
            WavesPanel wavesPanel = new WavesPanel(Simulator, Vector3.Zero, new Vector2(500, 500), Preferences.PrioriteGUIPanneauGeneral, Color.White) { Visible = false };
            Panels.Add(EditorPanel.Waves, wavesPanel);

            // Load panel
            LevelsPanel loadPanel = new LevelsPanel(Simulator.Scene, Vector3.Zero, new Vector2(800, 500), Preferences.PrioriteGUIPanneauGeneral, Color.White);
            loadPanel.SetTitle("Load");
            loadPanel.Initialize();
            Panels.Add(EditorPanel.Load, loadPanel);

            // Save panel
            LevelsPanel deletePanel = new LevelsPanel(Simulator.Scene, Vector3.Zero, new Vector2(800, 500), Preferences.PrioriteGUIPanneauGeneral, Color.White);
            deletePanel.SetTitle("Delete - No confirmation!!!");
            deletePanel.Initialize();
            Panels.Add(EditorPanel.Delete, deletePanel);

            ContextualMenusCollisions = new ContextualMenusCollisions();
        }


        public void Initialize()
        {
            Players.Clear();

            PlayerBrowsingGeneralMenu = null;

            foreach (var panel in Panels.Values)
                panel.Visible = false;

            GeneralMenu.Initialize();

            CelestialBodiesPathPreviews.CelestialBodies = CelestialBodies;
        }


        public void DoPlayerConnected(EditorPlayer p)
        {
            EditorGUIPlayer player = new EditorGUIPlayer(Simulator, p.Color);

            player.GeneralMenu = GeneralMenu;

            Players.Add(p, player);
        }


        public void DoPlayerDisconnected(EditorPlayer p)
        {
            Players.Remove(p);
        }


        public void DoPlayerChanged(EditorPlayer p)
        {
            var player = Players[p];

            // Change general menu
            if (PlayerBrowsingGeneralMenu == null || (player == PlayerBrowsingGeneralMenu &&
                player.SelectedGeneralMenu != p.ActualSelection.GeneralMenuChoice))
            {
                if (PlayerBrowsingGeneralMenu == null)
                    PlayerBrowsingGeneralMenu = player;

                if (player == PlayerBrowsingGeneralMenu)
                    GeneralMenu.DoMenuChanged(player.SelectedGeneralMenu, p.ActualSelection.GeneralMenuChoice, p.Color);

                if (player == PlayerBrowsingGeneralMenu && p.ActualSelection.GeneralMenuChoice == EditorGeneralMenuChoice.None)
                    PlayerBrowsingGeneralMenu = null;
            }

            // update actual selection
            player.SelectedGeneralMenu = p.ActualSelection.GeneralMenuChoice;
            player.GeneralMenuSubMenuIndex = p.ActualSelection.GeneralMenuSubMenuIndex;
            player.CelestialBodyMenu.Menu.SelectedIndex = p.ActualSelection.CelestialBodyChoice;

            if (p.SimPlayer.ActualSelection.TurretToPlace != null)
                return;

            // synchronize celestial body menu
            if (p.SimPlayer.ActualSelection.CelestialBody != null && player.CelestialBodyMenu.CelestialBody != p.SimPlayer.ActualSelection.CelestialBody)
            {
                player.CelestialBodyMenu.CelestialBody = p.SimPlayer.ActualSelection.CelestialBody;
                player.CelestialBodyMenu.SyncData();
                player.CelestialBodyMenu.Visible = true;
            }

            else if (p.SimPlayer.ActualSelection.CelestialBody == null)
            {
                player.CelestialBodyMenu.CelestialBody = null;
                player.CelestialBodyMenu.Visible = false;
            }

            player.CelestialBodyMenu.Visible = p.SimPlayer.ActualSelection.EditingState == EditorEditingState.None;
        }


        public void Update()
        {

        }


        public void Draw()
        {
            GeneralMenu.Draw();

            OrganizeContextualMenus();

            foreach (var player in Players.Values)
                player.Draw();

            foreach (var panel in Panels.Values)
                panel.Draw();

            if (PlayerBrowsingGeneralMenu != null)
            {
                var menu = GeneralMenu.SubMenus[PlayerBrowsingGeneralMenu.SelectedGeneralMenu];
                menu.SelectedIndex = PlayerBrowsingGeneralMenu.GeneralMenuSubMenuIndex;
                menu.Draw();
            }


            if (GeneralMenu.SubMenuToFadeOut != EditorGeneralMenuChoice.None)
            {
                var menu = GeneralMenu.SubMenus[GeneralMenu.SubMenuToFadeOut];
                menu.Position = GeneralMenu.Menus[GeneralMenu.SubMenuToFadeOut].Position;
                menu.Draw();
            }

            CelestialBodiesPathPreviews.Draw();
        }


        public void DoEditorCommandExecuted(EditorCommand command)
        {
            if (command.Name == "PlaytestState" || command.Name == "EditState")
            {
                ((EditorToggleContextualMenuChoice) GeneralMenu.SubMenus[EditorGeneralMenuChoice.File].GetChoice(4)).Next();
                return;
            }


            if (command.Name == "PauseSimulation" || command.Name == "ResumeSimulation")
            {
                ((EditorToggleContextualMenuChoice) GeneralMenu.SubMenus[EditorGeneralMenuChoice.File].GetChoice(6)).Next();
                return;
            }


            if (command.Type == EditorCommandType.Panel)
            {
                var panelCommand = (EditorPanelCommand) command;

                Panels[panelCommand.Panel].Fade(panelCommand.Show ? 0 : 255, panelCommand.Show ? 255 : 0, 500);
                return;
            }


            if (command.Type == EditorCommandType.CelestialBody)
                DoEditorCelestialBodyCommandExecuted((EditorCelestialBodyCommand) command);
        }


        private void DoEditorCelestialBodyCommandExecuted(EditorCelestialBodyCommand command)
        {
            var player = Players[command.Owner];

            if (command.Name == "ShowPathPreview")
                command.CelestialBody.ShowPath = true;
            
            else if (command.Name == "HidePathPreview")
                command.CelestialBody.ShowPath = false;

            player.CelestialBodyMenu.SyncData();
        }


        private void OrganizeContextualMenus()
        {
            ContextualMenusCollisions.Menus.Clear();

            foreach (var p in Players.Values)
            {
                p.Update();

                if (p.OpenedMenu != null)
                    ContextualMenusCollisions.Menus.Add(p.OpenedMenu);
            }

            if (PlayerBrowsingGeneralMenu != null)
            {
                var menu = GeneralMenu.SubMenus[PlayerBrowsingGeneralMenu.SelectedGeneralMenu];

                menu.Position = GeneralMenu.Menus[PlayerBrowsingGeneralMenu.SelectedGeneralMenu].Position;

                ContextualMenusCollisions.Menus.Add(menu);
            }

            ContextualMenusCollisions.Sync();
        }
    }
}
