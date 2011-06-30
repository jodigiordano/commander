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

        private ContextualMenusCollisions ContextualMenusCollisions;


        public EditorGUIController(Simulator simulator)
        {
            Simulator = simulator;

            GeneralMenu = new EditorGeneralMenu(simulator, new Vector3(400, 300, 0), Preferences.PrioriteSimulationCorpsCeleste);

            Players = new Dictionary<EditorPlayer, EditorGUIPlayer>();

            Panels = new Dictionary<EditorPanel, Panel>(EditorPanelComparer.Default);

            var panel = new Panel(Simulator.Scene, Vector3.Zero, new Vector2(500, 500), Preferences.PrioriteGUIPanneauGeneral, Color.White) { Visible = false };
            panel.SetTitle("Player");
            Panels.Add(EditorPanel.Player, panel);

            panel = new Panel(Simulator.Scene, Vector3.Zero, new Vector2(500, 500), Preferences.PrioriteGUIPanneauGeneral, Color.White) { Visible = false };
            panel.SetTitle("Load");
            Panels.Add(EditorPanel.Load, panel);

            panel = new Panel(Simulator.Scene, Vector3.Zero, new Vector2(500, 500), Preferences.PrioriteGUIPanneauGeneral, Color.White) { Visible = false };
            panel.SetTitle("Save As...");
            Panels.Add(EditorPanel.Save, panel);

            panel = new Panel(Simulator.Scene, Vector3.Zero, new Vector2(500, 500), Preferences.PrioriteGUIPanneauGeneral, Color.White) { Visible = false };
            panel.SetTitle("Delete");
            Panels.Add(EditorPanel.Delete, panel);

            ContextualMenusCollisions = new ContextualMenusCollisions();
        }


        public void Initialize()
        {
            Players.Clear();

            PlayerBrowsingGeneralMenu = null;

            foreach (var panel in Panels.Values)
                panel.Visible = false;

            GeneralMenu.Initialize();
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
        }


        public void DoEditorCommandExecuted(EditorPlayer p, EditorCommand command)
        {
            if (command.Name == "PlaytestState" || command.Name == "EditState")
                ((EditorToggleContextualMenuChoice) GeneralMenu.SubMenus[EditorGeneralMenuChoice.File].GetChoice(3)).Next();
            else if (command.Name == "PauseSimulation" || command.Name == "ResumeSimulation")
                ((EditorToggleContextualMenuChoice) GeneralMenu.SubMenus[EditorGeneralMenuChoice.File].GetChoice(5)).Next();
        }


        public void DoEditorPanelCommandExecuted(EditorPlayer p, EditorPanelCommand command)
        {
            Panels[command.Panel].Fade(command.Show ? 0 : 255, command.Show ? 255 : 0, 500);
        }


        public void DoEditorCelestialBodyCommandExecuted(EditorPlayer p, EditorCelestialBodyCommand command)
        {
            var player = Players[p];

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
