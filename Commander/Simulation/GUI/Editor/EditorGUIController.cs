namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;


    class EditorGUIController
    {
        public List<CelestialBody> CelestialBodies;

        public EditorGeneralMenu GeneralMenu;
        public Dictionary<EditorPanel, Panel> Panels;
        public Dictionary<EditorGeneralMenuChoice, ContextualMenu> GeneralMenuSubMenus;
        public Dictionary<EditorPlayer, EditorGUIPlayer> Players;

        private Simulator Simulator;


        public EditorGUIController(Simulator simulator)
        {
            Simulator = simulator;

            GeneralMenu = new EditorGeneralMenu(simulator, new Vector3(400, 300, 0), Preferences.PrioriteSimulationCorpsCeleste);
            GeneralMenuSubMenus = new Dictionary<EditorGeneralMenuChoice, ContextualMenu>(EditorGeneralMenuChoiceComparer.Default);

            Players = new Dictionary<EditorPlayer, EditorGUIPlayer>();

            Panels = new Dictionary<EditorPanel, Panel>(EditorPanelComparer.Default);

            var panel = new Panel(Simulator.Scene, Vector3.Zero, new Vector2(500, 500), Preferences.PrioriteGUIPanneauGeneral, Color.White) { Visible = false };
            panel.SetTitle("Player");
            Panels.Add(EditorPanel.Player, panel);

            panel = new Panel(Simulator.Scene, Vector3.Zero, new Vector2(500, 500), Preferences.PrioriteGUIPanneauGeneral, Color.White) { Visible = false };
            panel.SetTitle("Load");
            Panels.Add(EditorPanel.Load, panel);
        }


        public void Initialize()
        {
            foreach (var subMenu in GeneralMenuSubMenus.Values)
                subMenu.Visible = false;

            foreach (var panel in Panels.Values)
                panel.Visible = false;
        }


        public void DoPlayerConnected(EditorPlayer p)
        {
            EditorGUIPlayer player = new EditorGUIPlayer(Simulator, p.Color);

            player.GeneralMenu = GeneralMenu;

            Players.Add(p, player);

            //ark...
            if (GeneralMenuSubMenus.Count == 0)
            {
                foreach (var subMenu in player.GeneralMenuChoices)
                    GeneralMenuSubMenus.Add(subMenu.Key, subMenu.Value);
            }
        }


        public void DoPlayerDisconnected(EditorPlayer p)
        {
            Players.Remove(p);
        }


        public void DoPlayerChanged(EditorPlayer p)
        {
            var player = Players[p];

            // Change general menu
            if (player.SelectedGeneralMenu != p.ActualSelection.GeneralMenuChoice)
            {
                GeneralMenu.DoMenuChanged(player.SelectedGeneralMenu, p.ActualSelection.GeneralMenuChoice);
                player.DoGeneralMenuChanged(player.SelectedGeneralMenu, p.ActualSelection.GeneralMenuChoice);
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

            foreach (var player in Players.Values)
                player.Draw();

            foreach (var panel in Panels.Values)
                panel.Draw();
        }


        public void DoEditorCommandExecuted(EditorPlayer p, EditorCommand command)
        {
            if (command.Name == "PlaytestState" || command.Name == "EditState")
            {
                ((EditorToggleContextualMenuChoice) GeneralMenuSubMenus[EditorGeneralMenuChoice.File].GetChoice(3)).Next();
            }
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
    }
}
