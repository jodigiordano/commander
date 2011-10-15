namespace EphemereGames.Commander.Simulation
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


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

            ((PlayerPanel) Panels[EditorPanel.Player]).Initialize();
            ((TurretsPanel) Panels[EditorPanel.Turrets]).Initialize();
            ((PowerUpsPanel) Panels[EditorPanel.PowerUps]).Initialize();
            ((GeneralPanel) Panels[EditorPanel.General]).Initialize();
            ((WavesPanel) Panels[EditorPanel.Waves]).Initialize();

            Panels[EditorPanel.Player].SetClickHandler("Lives", DoLives);
            Panels[EditorPanel.Player].SetClickHandler("Cash", DoCash);
            Panels[EditorPanel.Player].SetClickHandler("Minerals", DoMinerals);
            Panels[EditorPanel.Player].SetClickHandler("BulletDamage", DoBulletDamage);
            Panels[EditorPanel.Player].SetClickHandler("LifePacks", DoLifePacks);
            Panels[EditorPanel.General].SetClickHandler("Difficulty", DoDifficulty);
            Panels[EditorPanel.General].SetClickHandler("World", DoWorld);
            Panels[EditorPanel.General].SetClickHandler("Level", DoLevel);
            Panels[EditorPanel.Turrets].SetClickHandler(DoTurrets);
            Panels[EditorPanel.PowerUps].SetClickHandler(DoPowerUps);
            Panels[EditorPanel.Background].SetClickHandler(DoBackgrounds);
            Panels[EditorPanel.Waves].SetClickHandler(DoWaves);
            Panels[EditorPanel.Load].SetClickHandler(DoLoad);
            Panels[EditorPanel.Delete].SetClickHandler(DoDelete);
            Panels[EditorPanel.CelestialBodyAssets].SetClickHandler(DoCelestialBodyAssets);

            foreach (var panel in Panels.Values)
                panel.CloseButtonHandler = DoClosePanel;
        }


        public void DoPlayerConnected(SimPlayer player)
        {
            if (!Simulator.EditorMode)
                return; 
            
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
            if (!Simulator.EditorMode)
                return;

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
            if (!Simulator.EditorMode)
                return;

            var player = Players[p];

            if (player.SimPlayer.ActualSelection.EditingState == EditorEditingState.MovingCB)
            {
                player.ActualSelection.CelestialBody.BasePosition += player.SimPlayer.DeltaPosition;
                player.SimPlayer.NinjaPosition = player.ActualSelection.CelestialBody.Position;
            }

            else if (player.SimPlayer.ActualSelection.EditingState == EditorEditingState.ShrinkingCB)
            {
                player.SimPlayer.NinjaPosition = player.ActualSelection.CelestialBody.Position;
                player.ActualSelection.CelestialBody.Position = player.SimPlayer.Position;
            }

            else if (player.SimPlayer.ActualSelection.EditingState == EditorEditingState.RotatingCB)
            {
                player.SimPlayer.NinjaPosition = player.ActualSelection.CelestialBody.Position;
            }

            else if (player.SimPlayer.ActualSelection.EditingState == EditorEditingState.StartPosCB)
            {

            }

            player.Circle.Position = p.Position;

            if (OpenedPanel != EditorPanel.None)
            {
                var hover = Panels[OpenedPanel].DoHover(player.Circle);

                // More friction on a celestial body and a turret
                if (hover &&
                    player.SimPlayer.SpaceshipMove.SteeringBehavior.LastNextMovement == Vector3.Zero &&
                    Panels[OpenedPanel].LastHoverWidget.Sticky)
                        player.SimPlayer.SpaceshipMove.SteeringBehavior.Friction = 0.1f;
            }
        }


        public void DoPlayerMovedDelta(SimPlayer p, ref Vector3 delta)
        {
            if (!Simulator.EditorMode)
                return;

            var player = Players[p];

            if (player.SimPlayer.ActualSelection.EditingState == EditorEditingState.RotatingCB)
            {
                float rotation = Core.Physics.Utilities.VectorToAngle(ref delta);
                player.ActualSelection.CelestialBody.SetRotation(rotation);
            }

            else if (player.SimPlayer.ActualSelection.EditingState == EditorEditingState.ShrinkingCB)
            {
                player.ActualSelection.CelestialBody.Path.X += delta.X;
                player.ActualSelection.CelestialBody.Path.Y -= delta.Y;
            }
        }


        public void DoNextOrPreviousAction(SimPlayer p, int delta)
        {
            if (!Simulator.EditorMode)
                return;

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
            if (!Simulator.EditorMode)
                return;

            var player = Players[p];

            player.DoSelectAction();

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

                if (player.SimPlayer.ActualSelection.EditingState != EditorEditingState.None)
                    ExecuteCommand(new EditorCelestialBodyCommand("ShowPathPreview") { Owner = player });

                ExecuteCommand(command);

                return;
            }
        }


        public void DoCancelAction(SimPlayer p)
        {
            if (!Simulator.EditorMode)
                return;

            var player = Players[p];

            //tmp
            if (player.SimPlayer.ActualSelection.CelestialBody != null && player.SimPlayer.ActualSelection.EditingState != EditorEditingState.None)
                ExecuteCommand(new EditorCelestialBodyCommand("HidePathPreview") { Owner = player });

            player.DoCancelAction();
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
                //if (Simulator.EditorState == EditorState.Editing)
                //    Simulator.SyncLevel();
                
                //Simulator.Initialize();
                //Simulator.SyncPlayers();
            }

            else if (command.Name == "PauseSimulation")
            {
                //Simulator.State = GameState.Paused;
            }

            else if (command.Name == "ResumeSimulation")
            {
                //Simulator.State = GameState.Running;
            }

            else if (command.Name == "NewLevel")
            {
                //Simulator.LevelDescriptor = Main.LevelsFactory.GetEmptyDescriptor();
                //Simulator.Initialize();
                //Simulator.SyncPlayers();
            }

            //else if (command.Name == "SaveLevel")
            //{
            //    Simulator.SyncLevel();

            //    var descriptor = Simulator.LevelDescriptor;

            //    if (!Main.LevelsFactory.UserDescriptors.ContainsKey(descriptor.Infos.Id))
            //    {
            //        Main.LevelsFactory.UserDescriptors.Add(descriptor.Infos.Id, descriptor);

            //        ((LevelsPanel) Panels[EditorPanel.Load]).Initialize();
            //        ((LevelsPanel) Panels[EditorPanel.Delete]).Initialize();
            //    }

            //    Main.LevelsFactory.SaveUserDescriptorOnDisk(descriptor.Infos.Id);
            //}


            NotifyEditorCommandExecuted(command);
        }


        private void DoExecuteEditorPanelCommand(EditorPanelCommand command)
        {
            if (command.Panel == EditorPanel.CelestialBodyAssets)
                ((CelestialBodyAssetsPanel) Panels[command.Panel]).CelestialBody = command.Owner.SimPlayer.ActualSelection.CelestialBody;

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
                command.CelestialBody = EditorLevelGenerator.GenerateCelestialBody(Simulator, CelestialBodies, VisualPriorities.Default.CelestialBody);
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


        private void DoMinerals(PanelWidget widget)
        {
            var slider = (NumericHorizontalSlider) widget;

            var command = new EditorPlayerCommand("AddOrRemoveMinerals")
            {
                Minerals = slider.Value,
                Owner = CurrentOpenedPanelPlayer
            };

            NotifyEditorCommandExecuted(command);
        }


        private void DoBulletDamage(PanelWidget widget)
        {
            var slider = (NumericHorizontalSlider) widget;

            var command = new EditorPlayerCommand("AddOrRemoveBulletDamage")
            {
                BulletDamage = slider.Value / 10f,
                Owner = CurrentOpenedPanelPlayer
            };

            NotifyEditorCommandExecuted(command);
        }


        private void DoLifePacks(PanelWidget widget)
        {
            var slider = (NumericHorizontalSlider) widget;

            var command = new EditorPlayerCommand("AddOrRemoveLifePacks")
            {
                LifePacks = slider.Value,
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

            if (checkbox.Value)
                Simulator.TurretsFactory.Availables.Add(checkbox.Turret.Type, checkbox.Turret);
            else
                Simulator.TurretsFactory.Availables.Remove(checkbox.Turret.Type);
        }


        private void DoPowerUps(PanelWidget widget)
        {
            var checkbox = (PowerUpCheckBox) widget;

            if (checkbox.Value)
                Simulator.PowerUpsFactory.Availables.Add(checkbox.PowerUp, Simulator.PowerUpsFactory.Create(checkbox.PowerUp));
            else
                Simulator.PowerUpsFactory.Availables.Remove(checkbox.PowerUp);

            NotifyEditorCommandExecuted(new EditorCommand("AddOrRemovePowerUp"));
        }


        private void DoBackgrounds(PanelWidget widget)
        {
            var img = (ImageWidget) ((GridPanel) widget).LastClickedWidget;

            Simulator.Level.Background = new Image(img.Image.TextureName) { VisualPriority = Preferences.PrioriteFondEcran };
        }


        private void DoCelestialBodyAssets(PanelWidget widget)
        {
            var panel = (CelestialBodyAssetsPanel) Panels[EditorPanel.CelestialBodyAssets];
            var img = (ImageWidget) ((GridPanel) widget).LastClickedWidget;

            NotifyEditorCommandExecuted(
                new EditorCelestialBodyCommand("ChangeAsset")
                {
                    CelestialBody = panel.CelestialBody,
                    AssetName = img.Image.TextureName.Substring(0, img.Image.TextureName.Length - 1)
                });
        }


        private void DoWaves(PanelWidget widget)
        {
            // Prepare the waves
            List<WaveDescriptor> descriptors = new List<WaveDescriptor>();

            var panel = Panels[EditorPanel.Waves];

            foreach (var w in panel.Widgets)
            {
                var subPanel = (WaveSubPanel) w.Value;

                if (subPanel.EnemiesCount != 0 && subPanel.Quantity != 0)
                    descriptors.Add(subPanel.GenerateDescriptor());
            }

            Simulator.Level.Waves.Clear();

            foreach (var wd in descriptors)
                Simulator.Level.Waves.AddLast(new Wave(Simulator, wd));
        }


        private void DoClosePanel(PanelWidget widget)
        {
            var closeCommand = new EditorPanelCommand("ClosePanel", OpenedPanel, false) { Owner = CurrentOpenedPanelPlayer };

            OpenedPanel = EditorPanel.None;

            NotifyEditorCommandExecuted(closeCommand);
        }


        private void DoLoad(PanelWidget widget)
        {
            var panel = (LevelsPanel) widget;

            if (panel.ClickedLevel != null)
            {
                Simulator.LevelDescriptor = panel.ClickedLevel;
                Simulator.Initialize();
                Simulator.SyncPlayers();
            }
        }


        private void DoDelete(PanelWidget widget)
        {
            //var panel = (LevelsPanel) widget;

            //if (panel.ClickedLevel != null)
            //{
            //    var descriptor = Simulator.LevelDescriptor;

            //    Main.LevelsFactory.DeleteUserDescriptorFromDisk(descriptor.Infos.Id);
            //    Main.LevelsFactory.Descriptors.Remove(descriptor.Infos.Id);

            //    ((LevelsPanel) Panels[EditorPanel.Load]).Initialize();
            //    ((LevelsPanel) Panels[EditorPanel.Delete]).Initialize();
            //}
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
