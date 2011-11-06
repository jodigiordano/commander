﻿namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class EditorController
    {
        public EditorGeneralMenu GeneralMenu;
        public Dictionary<EditorPanel, Panel> Panels;
        public Dictionary<EditorPlayer, EditorGUIPlayer> EditorGUIPlayers;

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

            Panels[EditorPanel.Player].Initialize();
            Panels[EditorPanel.Waves].Initialize();

            Panels[EditorPanel.Player].SetClickHandler("Lives", DoLives);
            Panels[EditorPanel.Player].SetClickHandler("Cash", DoCash);
            Panels[EditorPanel.Player].SetClickHandler("Minerals", DoMinerals);
            Panels[EditorPanel.Player].SetClickHandler("BulletDamage", DoBulletDamage);
            Panels[EditorPanel.Player].SetClickHandler("LifePacks", DoLifePacks);
            Panels[EditorPanel.Turrets].SetClickHandler(DoTurrets);
            Panels[EditorPanel.PowerUps].SetClickHandler(DoPowerUps);
            Panels[EditorPanel.Background].SetClickHandler(DoBackgrounds);
            Panels[EditorPanel.Waves].SetClickHandler(DoWaves);
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
                if (hover && player.SimPlayer.SpaceshipMove.SteeringBehavior.LastNextMovement == Vector3.Zero)
                {
                    var panel = Panels[OpenedPanel];

                    if (panel.LastHoverWidget.Sticky)
                    {
                        player.SimPlayer.SpaceshipMove.SteeringBehavior.Friction = 0.1f;
                    }

                    else if (OpenedPanel == EditorPanel.Waves && ((Panel) panel.LastHoverWidget).LastHoverWidget.Sticky)
                    {
                        player.SimPlayer.SpaceshipMove.SteeringBehavior.Friction = 0.1f;
                    }
                }
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
                Simulator.Data.Level.SyncDescriptor();
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
                //Simulator.LevelDescriptor = Main.WorldsFactory.GetEmptyLevelDescriptor();
                //Simulator.Initialize();
                //Simulator.SyncPlayers();
            }

            //else if (command.Name == "SaveLevel")
            //{
            //    Simulator.SyncLevel();

            //    var descriptor = Simulator.LevelDescriptor;

            //    if (!Main.WorldsFactory.UserDescriptors.ContainsKey(descriptor.Infos.Id))
            //    {
            //        Main.WorldsFactory.UserDescriptors.Add(descriptor.Infos.Id, descriptor);

            //        ((LevelsPanel) Panels[EditorPanel.Load]).Initialize();
            //        ((LevelsPanel) Panels[EditorPanel.Delete]).Initialize();
            //    }

            //    Main.WorldsFactory.SaveUserDescriptorOnDisk(descriptor.Infos.Id);
            //}


            NotifyEditorCommandExecuted(command);
        }


        private void DoExecuteEditorPanelCommand(EditorPanelCommand command)
        {
            if (command.Panel == EditorPanel.CelestialBodyAssets)
                ((CelestialBodyAssetsPanel) Panels[command.Panel]).CelestialBody = command.Owner.SimPlayer.ActualSelection.CelestialBody;

            if (command.Panel == EditorPanel.Waves && !command.Show)
                SyncWaves();

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
                command.CelestialBody = EditorLevelGenerator.GenerateCelestialBody(Simulator, VisualPriorities.Default.CelestialBody);
            else if (command.Name == "AddPinkHole")
                command.CelestialBody = EditorLevelGenerator.GeneratePinkHole(Simulator, VisualPriorities.Default.CelestialBody);
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
                BulletDamage = slider.Value,
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


        private void DoTurrets(PanelWidget widget)
        {
            var checkbox = (TurretCheckBox) ((GridPanel) widget).LastClickedWidget;

            if (checkbox.Value)
                Simulator.Data.Level.AvailableTurrets.Add(checkbox.Turret.Type, checkbox.Turret);
            else
                Simulator.Data.Level.AvailableTurrets.Remove(checkbox.Turret.Type);
        }


        private void DoPowerUps(PanelWidget widget)
        {
            var checkbox = (PowerUpCheckBox) widget;

            if (checkbox.Value)
                Simulator.Data.Level.AvailablePowerUps.Add(checkbox.PowerUp, Simulator.PowerUpsFactory.Create(checkbox.PowerUp));
            else
                Simulator.Data.Level.AvailablePowerUps.Remove(checkbox.PowerUp);

            NotifyEditorCommandExecuted(new EditorCommand("AddOrRemovePowerUp"));
        }


        private void DoBackgrounds(PanelWidget widget)
        {
            var img = (ImageWidget) ((GridPanel) widget).LastClickedWidget;

            Simulator.Data.Level.Background = new Image(img.Image.TextureName) { VisualPriority = Preferences.PrioriteFondEcran };
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
            var clickedWidget = ((Panel) ((Panel) widget).LastClickedWidget).LastClickedWidget;
            var waveId = ((WaveSubPanel) widget).Id;

            if (clickedWidget.Name == "Enemies")
            {
                var enemiesAssets = (EnemiesAssetsPanel) Panels[EditorPanel.Enemies];

                if (waveId < Simulator.Data.Level.Descriptor.Waves.Count)
                    enemiesAssets.Enemies = Simulator.Data.Level.Descriptor.Waves[waveId].Enemies;
                else
                    enemiesAssets.Enemies = new List<EnemyType>();

                enemiesAssets.Sync();

                ExecuteCommand(new EditorPanelCommand("ShowPanel", EditorPanel.Enemies, true));
                return;
            }
        }


        private void SyncWaves()
        {
            List<WaveDescriptor> descriptors = new List<WaveDescriptor>();

            var panel = Panels[EditorPanel.Waves];

            foreach (var w in panel.Widgets)
            {
                var subPanel = (WaveSubPanel) w.Value;

                if (subPanel.EnemiesCount != 0 && subPanel.Quantity != 0)
                    descriptors.Add(subPanel.GenerateDescriptor());
            }

            Simulator.Data.Level.Waves.Clear();

            foreach (var wd in descriptors)
                Simulator.Data.Level.Waves.AddLast(new Wave(Simulator, wd));
        }


        private void DoClosePanel(PanelWidget widget)
        {
            EditorPanelCommand closeCommand = new EditorPanelCommand("ClosePanel", OpenedPanel, false) { Owner = CurrentOpenedPanelPlayer };

            if (OpenedPanel == EditorPanel.Enemies)
            {
                var enemiesAssets = (EnemiesAssetsPanel) Panels[EditorPanel.Enemies];
                ((WavesPanel) Panels[EditorPanel.Waves]).SyncEnemiesCurrentWave(enemiesAssets.Enemies);

                closeCommand = new EditorPanelCommand("OpenPanel", EditorPanel.Waves, true) { Owner = CurrentOpenedPanelPlayer };
            }

            ExecuteCommand(closeCommand);
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
