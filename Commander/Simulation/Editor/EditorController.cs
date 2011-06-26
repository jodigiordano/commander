namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;


    class EditorController
    {
        public EditorGeneralMenu GeneralMenu;
        public Dictionary<EditorGeneralMenuChoice, ContextualMenu> GeneralMenuSubMenus;
        public Dictionary<EditorPanel, Panel> Panels;
        public Dictionary<EditorPlayer, EditorGUIPlayer> EditorGUIPlayers;

        public event EditorPlayerHandler PlayerConnected;
        public event EditorPlayerHandler PlayerDisconnected;
        public event EditorPlayerHandler PlayerChanged;
        public event EditorPlayerEditorCommandHandler EditorCommandExecuted;
        public event EditorPlayerCelestialBodyEditorCommandHandler EditorCelestialBodyCommandExecuted;
        public event EditorPlayerPanelEditorCommandHandler EditorPanelCommandExecuted;

        private Dictionary<SimPlayer, EditorPlayer> Players;
        private Simulator Simulator;

        private EditorPanel OpenedPanel;
        private Dictionary<EditorPanel, EditorPanelCommand[]> PanelsOpenCloseCommands;


        public EditorController(Simulator simulator)
        {
            Simulator = simulator;

            Players = new Dictionary<SimPlayer, EditorPlayer>();
            PanelsOpenCloseCommands = new Dictionary<EditorPanel, EditorPanelCommand[]>(EditorPanelComparer.Default);

            Simulator.EditorCommands = new Dictionary<string, EditorCommand>()
            {
                { "None", new EditorCommand("None") },
                { "ShowPlayerPanel", new EditorPanelCommand("ShowPlayerPanel", EditorPanel.Player, true)  },
                { "HidePlayerPanel", new EditorPanelCommand("HidePlayerPanel", EditorPanel.Player, false)  },
                { "ShowLoadPanel", new EditorPanelCommand("ShowLoadPanel", EditorPanel.Load, true)   },
                { "HideLoadPanel", new EditorPanelCommand("HideLoadPanel", EditorPanel.Load, false)   },
                { "ShowWavesPanel", new EditorPanelCommand("ShowWavesPanel", EditorPanel.Waves, true)   },
                { "HideWavesPanel", new EditorPanelCommand("HideWavesPanel", EditorPanel.Waves, true)   },
                { "ShowGeneratePlanetarySystemPanel", new EditorPanelCommand("ShowGeneratePlanetarySystemPanel", EditorPanel.GeneratePlanetarySystem, true)   },
                { "HideGeneratePlanetarySystemPanel", new EditorPanelCommand("HideGeneratePlanetarySystemPanel", EditorPanel.GeneratePlanetarySystem, false)   },
                { "ShowDeletePanel", new EditorPanelCommand("ShowDeletePanel", EditorPanel.Delete, true)   },
                { "HideDeletePanel", new EditorPanelCommand("HideDeletePanel", EditorPanel.Delete, false)   },
                { "ShowPowerUpsPanel", new EditorPanelCommand("ShowPowerUpsPanel", EditorPanel.PowerUps, true)   },
                { "HidePowerUpsPanel", new EditorPanelCommand("HidePowerUpsPanel", EditorPanel.PowerUps, false)   },
                { "ShowTurretsPanel", new EditorPanelCommand("ShowTurretsPanel", EditorPanel.Turrets, true)   },
                { "HideTurretsPanel", new EditorPanelCommand("HideTurretsPanel", EditorPanel.Turrets, false)   },
                { "ShowGeneralPanel", new EditorPanelCommand("ShowGeneralPanel", EditorPanel.General, true)   },
                { "HideGeneralPanel", new EditorPanelCommand("HideGeneralPanel", EditorPanel.General, false)   },
                { "ShowBackgroundPanel", new EditorPanelCommand("ShowBackgroundPanel", EditorPanel.Background, true)   },
                { "HideBackgroundPanel", new EditorPanelCommand("HideBackgroundPanel", EditorPanel.Background, false)   },
                { "ShowSavePanel", new EditorPanelCommand("ShowSavePanel", EditorPanel.Save, true)   },
                { "HideSavePanel", new EditorPanelCommand("HideSavePanel", EditorPanel.Save, false)   },
                { "SaveLevel", new EditorCommand("SaveLevel")  },
                { "RestartSimulation", new EditorCommand("RestartSimulation")  },
                { "PauseSimulation", new EditorCommand("PauseSimulation")  },
                { "QuickGeneratePlanetarySystem", new EditorCommand("QuickGeneratePlanetarySystem")  },
                { "AddPlanet", new EditorCelestialBodyCommand("Add")  },
                { "ValidatePlanetarySystem", new EditorCommand("ValidatePlanetarySystem")  },
                { "PlaytestState", new EditorCommand("PlaytestState")  },
                { "EditState", new EditorCommand("EditState")  }
            };


            foreach (var command in Simulator.EditorCommands.Values)
            {
                var panelCommand = command as EditorPanelCommand;

                if (panelCommand == null)
                    continue;

                if (!PanelsOpenCloseCommands.ContainsKey(panelCommand.Panel))
                    PanelsOpenCloseCommands.Add(panelCommand.Panel, new EditorPanelCommand[2]);

                PanelsOpenCloseCommands[panelCommand.Panel][panelCommand.Show ? 0 : 1] = panelCommand;
            }
        }


        public void Initialize()
        {
            Players.Clear();

            OpenedPanel = EditorPanel.None;
        }


        public void DoPlayerConnected(SimPlayer player)
        {
            var editorPlayer = new EditorPlayer(Simulator)
            {
                SimPlayer = player,
                GeneralMenu = GeneralMenu,
                GeneralMenuSubMenus = GeneralMenuSubMenus
            };


            editorPlayer.Initialize();

            Players.Add(player, editorPlayer);

            NotifyPlayerConnected(editorPlayer);
        }


        public void DoPlayerDisconnected(SimPlayer player)
        {
            var editorPlayer = Players[player];

            Players.Remove(player);

            NotifyPlayerDisconnected(editorPlayer);
        }


        public void Update()
        {
            foreach (var player in Players.Values)
            {
                player.Update();

                NotifyPlayerChanged(player);
            }
        }


        public void Draw()
        {

        }


        public void DoPlayerMoved(SimPlayer p)
        {
            var player = Players[p];

            player.Circle.Position = p.Position;
        }


        public void DoNextOrPreviousAction(SimPlayer p, int delta)
        {
            var player = Players[p];

            if (player.ActualSelection.GeneralMenuChoice != EditorGeneralMenuChoice.None)
            {
                if (delta > 0)
                    player.NextGeneralMenuChoice();
                else
                    player.PreviousGeneralMenuChoice();
            }

            else if (player.SimPlayer.ActualSelection.CelestialBody != null)
            {
                if (delta > 0)
                    player.NextCelestialBodyChoice();
                else
                    player.PreviousCelestialBodyChoice();
            }
        }


        public void DoSelectAction(SimPlayer p)
        {
            var player = Players[p];

            if (player.ActualSelection.GeneralMenuChoice != EditorGeneralMenuChoice.None)
            {
                var menu = GeneralMenuSubMenus[player.ActualSelection.GeneralMenuChoice];
                var choice = menu.GetChoice(player.ActualSelection.GeneralMenuSubMenuIndex);

                EditorCommand command = GetCommand(choice);

                ExecuteCommand(player, command);

                return;
            }


            if (OpenedPanel != EditorPanel.None)
            {
                var panel = Panels[OpenedPanel];

                if (panel.CloseButton.DoClick(player.Circle)) // close a panel
                {
                    var closeCommand = PanelsOpenCloseCommands[OpenedPanel][1];

                    OpenedPanel = EditorPanel.None;

                    NotifyEditorPanelCommandExecuted(player, closeCommand);
                }

                return;
            }


            if (player.SimPlayer.ActualSelection.CelestialBody != null)
            {
                var choice = EditorGUIPlayers[player].CelestialBodyMenu.Menu.GetCurrentChoice();
                var command = GetCommand(choice);

                ExecuteCommand(player, command);

                return;
            }
        }


        private void ExecuteCommand(EditorPlayer player, EditorCommand command)
        {
            if (command is EditorCelestialBodyCommand)
                DoExecuteEditorCelestialBodyCommand(player, (EditorCelestialBodyCommand) command);
            else if (command is EditorPanelCommand)
                DoExecuteEditorPanelCommand(player, (EditorPanelCommand) command);
            else
                DoExecuteEditorCommand(player, command);
        }


        private EditorCommand GetCommand(ContextualMenuChoice choice)
        {
            var attempt1 = choice as EditorTextContextualMenuChoice;
            var attempt2 = choice as EditorToggleContextualMenuChoice;

            if (attempt1 != null)
                return attempt1.Command;
            else
                return attempt2.Command;
        }


        private void DoExecuteEditorCommand(EditorPlayer player, EditorCommand command)
        {
            // toggle editor mode command
            if (command.Name == "PlaytestState")
            {
                Simulator.EditorState = EditorState.Playtest;
                Simulator.Initialize();
                Simulator.SyncPlayers();
            }

            else if (command.Name == "EditState")
            {
                Simulator.EditorState = EditorState.Editing;
                Simulator.Initialize();
                Simulator.SyncPlayers();
            }

            NotifyEditorCommandExecuted(player, command);
        }


        private void DoExecuteEditorPanelCommand(EditorPlayer player, EditorPanelCommand command)
        {
            if (OpenedPanel == EditorPanel.None) // open a panel
            {
                OpenedPanel = command.Panel;
            }

            else if (OpenedPanel != command.Panel && command.Show) // open a panel while another is opened
            {
                var closeCommand = PanelsOpenCloseCommands[OpenedPanel][command.Show ? 1 : 0];

                NotifyEditorCommandExecuted(player, closeCommand);

                OpenedPanel = command.Panel;
            }

            NotifyEditorPanelCommandExecuted(player, command);
        }


        private void DoExecuteEditorCelestialBodyCommand(EditorPlayer player, EditorCelestialBodyCommand command)
        {
            command.CelestialBody = player.SimPlayer.ActualSelection.CelestialBody;

            NotifyEditorCelestialBodyCommandExecuted(player, command);
        }


        private void NotifyPlayerConnected(EditorPlayer player)
        {
            if (PlayerConnected != null)
                PlayerConnected(player);
        }


        private void NotifyPlayerDisconnected(EditorPlayer player)
        {
            if (PlayerDisconnected != null)
                PlayerDisconnected(player);
        }


        private void NotifyPlayerChanged(EditorPlayer player)
        {
            if (PlayerChanged != null)
                PlayerChanged(player);
        }


        private void NotifyEditorCommandExecuted(EditorPlayer player, EditorCommand command)
        {
            if (EditorCommandExecuted != null)
                EditorCommandExecuted(player, command);
        }


        private void NotifyEditorCelestialBodyCommandExecuted(EditorPlayer player, EditorCelestialBodyCommand command)
        {
            if (EditorCelestialBodyCommandExecuted != null)
                EditorCelestialBodyCommandExecuted(player, command);
        }


        private void NotifyEditorPanelCommandExecuted(EditorPlayer player, EditorPanelCommand command)
        {
            if (EditorPanelCommandExecuted != null)
                EditorPanelCommandExecuted(player, command);
        }
    }
}
