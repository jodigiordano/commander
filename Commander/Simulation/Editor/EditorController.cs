namespace EphemereGames.Commander.Simulation
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Visual;


    class EditorController
    {
        public EditorGeneralMenu GeneralMenu;
        public Dictionary<EditorPanel, Panel> Panels;
        public Dictionary<EditorPlayer, EditorGUIPlayer> EditorGUIPlayers;
        public List<CelestialBody> CelestialBodies;

        public event EditorPlayerHandler PlayerConnected;
        public event EditorPlayerHandler PlayerDisconnected;
        public event EditorPlayerHandler PlayerChanged;
        public event EditorCommandHandler EditorCommandExecuted;

        private Dictionary<SimPlayer, EditorPlayer> Players;
        private Simulator Simulator;

        private EditorPanel OpenedPanel;
        private EditorPlayer CurrentOpenedPanelPlayer;


        public EditorController(Simulator simulator)
        {
            Simulator = simulator;

            Players = new Dictionary<SimPlayer, EditorPlayer>();
        }


        public void Initialize()
        {
            Players.Clear();

            OpenedPanel = EditorPanel.None;

            Panels[EditorPanel.Player].SetHandler("Lives", DoLives);
            Panels[EditorPanel.Player].SetHandler("Cash", DoCash);

            Panels[EditorPanel.General].SetHandler("Difficulty", DoDifficulty);
            Panels[EditorPanel.General].SetHandler("World", DoWorld);
            Panels[EditorPanel.General].SetHandler("Level", DoLevel);

            Panels[EditorPanel.Turrets].SetHandler(DoTurrets);
            Panels[EditorPanel.PowerUps].SetHandler(DoPowerUps);
            Panels[EditorPanel.Background].SetHandler(DoBackgrounds);

            Panels[EditorPanel.Waves].SetHandler(DoWaves);

            foreach (var panel in Panels.Values)
                panel.CloseButtonHandler = DoClosePanel;
        }


        public void DoPlayerConnected(SimPlayer player)
        {
            var editorPlayer = new EditorPlayer(Simulator)
            {
                SimPlayer = player,
                GeneralMenu = GeneralMenu
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
                var menu = GeneralMenu.SubMenus[player.ActualSelection.GeneralMenuChoice];
                var choice = menu.GetChoice(player.ActualSelection.GeneralMenuSubMenuIndex);

                EditorCommand command = GetCommand(choice);

                command.Owner = player;

                ExecuteCommand(command);

                return;
            }


            if (OpenedPanel != EditorPanel.None)
            {
                var panel = Panels[OpenedPanel];

                CurrentOpenedPanelPlayer = player;

                panel.DoClick(player.Circle);

                return;
            }

            if (Simulator.EditorState == EditorState.Playtest)
                return;

            if (player.SimPlayer.ActualSelection.CelestialBody != null)
            {
                var choice = EditorGUIPlayers[player].CelestialBodyMenu.Menu.GetCurrentChoice();
                var command = GetCommand(choice);

                command.Owner = player;

                ExecuteCommand(command);

                return;
            }
        }


        private void ExecuteCommand(EditorCommand command)
        {
            if (command is EditorCelestialBodyCommand)
                DoExecuteEditorCelestialBodyCommand((EditorCelestialBodyCommand) command);
            else if (command is EditorPanelCommand)
                DoExecuteEditorPanelCommand((EditorPanelCommand) command);
            else
                DoExecuteEditorCommand(command);
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


        private void DoExecuteEditorCommand(EditorCommand command)
        {
            // toggle editor mode command
            if (command.Name == "PlaytestState")
            {
                Simulator.EditorState = EditorState.Playtest;
                Simulator.SyncLevel();
                Simulator.Initialize();
                Simulator.SyncPlayers();
            }

            else if (command.Name == "EditState")
            {
                Simulator.EditorState = EditorState.Editing;
                Simulator.Initialize();
                Simulator.SyncPlayers();
            }

            else if (command.Name == "RestartSimulation")
            {
                if (Simulator.EditorState == EditorState.Editing)
                    Simulator.SyncLevel();
                
                Simulator.Initialize();
                Simulator.SyncPlayers();
            }

            else if (command.Name == "PauseSimulation")
            {
                Simulator.State = GameState.Paused;
            }

            else if (command.Name == "ResumeSimulation")
            {
                Simulator.State = GameState.Running;
            }

            NotifyEditorCommandExecuted(command);
        }


        private void DoExecuteEditorPanelCommand(EditorPanelCommand command)
        {
            if (OpenedPanel == EditorPanel.None) // open a panel
            {
                OpenedPanel = command.Panel;
                NotifyEditorCommandExecuted(command);
            }

            else if (OpenedPanel == command.Panel) // open a panel that is already opened => close it
            {
                var closeCommand = new EditorPanelCommand("ClosePanel", OpenedPanel, false);

                OpenedPanel = EditorPanel.None;
                
                NotifyEditorCommandExecuted(closeCommand);
            }

            else if (OpenedPanel != command.Panel && command.Show) // open a panel while another is opened
            {
                var closeCommand = new EditorPanelCommand("ClosePanel", OpenedPanel, false);

                NotifyEditorCommandExecuted(closeCommand);

                OpenedPanel = command.Panel;

                NotifyEditorCommandExecuted(command);
            }
        }


        private void DoExecuteEditorCelestialBodyCommand(EditorCelestialBodyCommand command)
        {
            if (command.Name == "AddPlanet")
                command.CelestialBody = EditorLevelGenerator.GenerateCelestialBody(Simulator, CelestialBodies, Preferences.PrioriteSimulationCorpsCeleste);
            else
                command.CelestialBody = command.Owner.SimPlayer.ActualSelection.CelestialBody;

            NotifyEditorCommandExecuted(command);
        }


        private void DoLives(PanelWidget widget)
        {
            var slider = (NumericHorizontalSlider) widget;

            var command = new EditorPlayerCommand("AddOrRemoveLives")
            {
                LifePoints = slider.Value,
                Owner = CurrentOpenedPanelPlayer
            };

            NotifyEditorCommandExecuted(command);
        }


        private void DoCash(PanelWidget widget)
        {
            var slider = (NumericHorizontalSlider) widget;

            var command = new EditorPlayerCommand("AddOrRemoveCash")
            {
                Cash = slider.Value,
                Owner = CurrentOpenedPanelPlayer
            };

            NotifyEditorCommandExecuted(command);
        }


        private void DoDifficulty(PanelWidget widget)
        {
            var slider = (ChoicesHorizontalSlider) widget;

            Simulator.Level.Difficulty = slider.Value;
        }


        private void DoWorld(PanelWidget widget)
        {
            var slider = (NumericHorizontalSlider) widget;

            Simulator.Level.Mission =
                slider.Value + "-" +
                Simulator.Level.Mission.Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries)[1];
        }


        private void DoLevel(PanelWidget widget)
        {
            var slider = (NumericHorizontalSlider) widget;

            Simulator.Level.Mission =
                Simulator.Level.Mission.Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries)[0] + "-" +
                slider.Value;
        }


        private void DoTurrets(PanelWidget widget)
        {
            var checkbox = (TurretCheckBox) widget;

            if (checkbox.Checked)
                Simulator.TurretsFactory.Availables.Add(checkbox.Turret.Type, checkbox.Turret);
            else
                Simulator.TurretsFactory.Availables.Remove(checkbox.Turret.Type);
        }


        private void DoPowerUps(PanelWidget widget)
        {
            var checkbox = (PowerUpCheckBox) widget;

            if (checkbox.Checked)
                Simulator.PowerUpsFactory.Availables.Add(checkbox.PowerUp, Simulator.PowerUpsFactory.Create(checkbox.PowerUp));
            else
                Simulator.PowerUpsFactory.Availables.Remove(checkbox.PowerUp);

            NotifyEditorCommandExecuted(new EditorCommand("AddOrRemovePowerUp"));
        }


        private void DoBackgrounds(PanelWidget widget)
        {
            var img = (ImageWidget) widget;

            Simulator.Level.Background = new Image(img.Image.TextureName) { VisualPriority = Preferences.PrioriteFondEcran };
        }


        private void DoWaves(PanelWidget widget)
        {
            // Prepare the waves
            List<WaveDescriptor> descriptors = new List<WaveDescriptor>();

            var panel = Panels[EditorPanel.Waves];

            foreach (var w in panel.Widgets.Values)
            {
                var subPanel = (WaveSubPanel) w;

                if (subPanel.EnemiesCount != 0)
                    descriptors.Add(subPanel.GenerateDescriptor());
            }

            Simulator.Level.Waves.Clear();

            foreach (var wd in descriptors)
                Simulator.Level.Waves.AddLast(new Wave(Simulator, wd));
        }


        private void DoClosePanel()
        {
            var closeCommand = new EditorPanelCommand("ClosePanel", OpenedPanel, false) { Owner = CurrentOpenedPanelPlayer };

            OpenedPanel = EditorPanel.None;

            NotifyEditorCommandExecuted(closeCommand);
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


        private void NotifyEditorCommandExecuted(EditorCommand command)
        {
            if (EditorCommandExecuted != null)
                EditorCommandExecuted(command);
        }
    }
}
