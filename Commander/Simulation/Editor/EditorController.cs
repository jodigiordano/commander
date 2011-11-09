namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Commander.Simulation.Player;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class EditorController
    {
        public event EditorCommandHandler EditorCommandExecuted;

        private Simulator Simulator;


        public EditorController(Simulator simulator)
        {
            Simulator = simulator;
        }


        public void Initialize()
        {
            Simulator.Data.Panels[PanelType.EditorPlayer].SetClickHandler("Lives", DoLives);
            Simulator.Data.Panels[PanelType.EditorPlayer].SetClickHandler("Cash", DoCash);
            Simulator.Data.Panels[PanelType.EditorPlayer].SetClickHandler("Minerals", DoMinerals);
            Simulator.Data.Panels[PanelType.EditorPlayer].SetClickHandler("BulletDamage", DoBulletDamage);
            Simulator.Data.Panels[PanelType.EditorPlayer].SetClickHandler("LifePacks", DoLifePacks);
            Simulator.Data.Panels[PanelType.EditorTurrets].SetClickHandler(DoTurrets);
            Simulator.Data.Panels[PanelType.EditorPowerUps].SetClickHandler(DoPowerUps);
            Simulator.Data.Panels[PanelType.EditorBackground].SetClickHandler(DoBackgrounds);
            Simulator.Data.Panels[PanelType.EditorWaves].SetClickHandler(DoWaves);
            Simulator.Data.Panels[PanelType.EditorCelestialBodyAssets].SetClickHandler(DoCelestialBodyAssets);
        }


        public void DoPlayerMoved(SimPlayer player)
        {
            if (!Simulator.EditorMode)
                return;

            if (player.ActualSelection.EditingState == EditorEditingState.None)
                return;

            if (player.ActualSelection.EditingState == EditorEditingState.MovingCB)
            {
                player.ActualSelection.CelestialBody.BasePosition += player.DeltaPosition;
                player.NinjaPosition = player.ActualSelection.CelestialBody.Position;
            }

            else if (player.ActualSelection.EditingState == EditorEditingState.TrajectoryCB)
            {
                player.NinjaPosition = player.ActualSelection.CelestialBody.Position;
                player.ActualSelection.CelestialBody.Position = player.Position;
            }

            else if (player.ActualSelection.EditingState == EditorEditingState.RotatingCB)
            {
                player.NinjaPosition = player.ActualSelection.CelestialBody.Position;
            }
        }


        public void DoPlayerMovedDelta(SimPlayer player, ref Vector3 delta)
        {
            if (!Simulator.EditorMode)
                return;

            if (player.ActualSelection.EditingState == EditorEditingState.None)
                return;

            if (player.ActualSelection.EditingState == EditorEditingState.RotatingCB)
            {
                float rotation = Core.Physics.Utilities.VectorToAngle(ref delta);
                player.ActualSelection.CelestialBody.SetRotation(rotation);
            }

            else if (player.ActualSelection.EditingState == EditorEditingState.TrajectoryCB)
            {
                player.ActualSelection.CelestialBody.Path.X += delta.X;
                player.ActualSelection.CelestialBody.Path.Y -= delta.Y;
            }
        }


        public void DoSelectAction(SimPlayer player)
        {
            if (!Simulator.EditorMode)
                return;

            if (Simulator.WorldMode && Simulator.EditorPlaytestingMode)
            {
                // open the world menu
                if (player.ActualSelection.EditorWorldLevelCommand == null && !(player.ActualSelection.OpenedMenu is EditorWorldMenu))
                {
                    player.VisualPlayer.EditorWorldMenu.Visible = true;
                }

                // the world menu is open => do the action
                else if (player.ActualSelection.OpenedMenu is EditorWorldMenu)
                {
                    ExecuteCommand(player.ActualSelection.EditorWorldCommand);
                    player.VisualPlayer.EditorWorldMenu.Visible = false;
                }

                // the world level menu is open => do the action
                else if (player.ActualSelection.OpenedMenu is EditorWorldLevelMenu)
                {
                    ExecuteCommand(player.ActualSelection.EditorWorldLevelCommand);
                }
            }

            else if (Simulator.EditorEditingMode)
            {
                // the celestial body menu is open => do the action
                if (player.ActualSelection.OpenedMenu is EditorCelestialBodyMenu)
                {
                    ExecuteCommand(player.ActualSelection.EditorCelestialBodyMenuCommand);
                }

                if (player.ActualSelection.EditingState != EditorEditingState.None)
                    return;

                // open the build menu
                else if (player.ActualSelection.EditorCelestialBodyMenuCommand == null && !(player.ActualSelection.OpenedMenu is EditorBuildMenu))
                {
                    player.VisualPlayer.EditorBuildMenu.Visible = true;
                }

                // the build menu is open => do the action
                else if (player.ActualSelection.OpenedMenu is EditorBuildMenu)
                {
                    ExecuteCommand(player.ActualSelection.EditorBuildMenuCommand);
                    player.VisualPlayer.EditorBuildMenu.Visible = false;
                }
            }

            //player.DoSelectAction();

            //if (player.ActualSelection.GeneralMenuChoice != EditorGeneralMenuChoice.None)
            //{
            //    var menu = player.GeneralMenu.SubMenus[player.ActualSelection.GeneralMenuChoice];
            //    var choice = menu.GetChoice(player.ActualSelection.GeneralMenuSubMenuIndex);

            //    EditorCommand command = GetCommand(choice);

            //    command.Owner = player;

            //    ExecuteCommand(command);

            //    return;
            //}


            //if (OpenedPanel != EditorPanel.None)
            //{
            //    var panel = Panels[OpenedPanel];

            //    panel.DoClick(p.Circle);

            //    return;
            //}

            //if (Simulator.EditorState == EditorState.Playtest)
            //    return;

            //if (p.ActualSelection.CelestialBody != null)
            //{
            //    var choice = player.GUIPlayer.CelestialBodyMenu.Menu.GetCurrentChoice();
            //    var command = GetCommand(choice);

            //    command.Owner = player;

            //    if (p.ActualSelection.EditingState != EditorEditingState.None)
            //        ExecuteCommand(new EditorCelestialBodyCommand("ShowPathPreview") { Owner = player });

            //    ExecuteCommand(command);

            //    return;
            //}
        }


        public void DoCancelAction(SimPlayer player)
        {
            if (!Simulator.EditorMode)
                return;

            if (player.ActualSelection.EditingState != EditorEditingState.None)
            {
                player.ActualSelection.EditingState = EditorEditingState.None;
                player.ActualSelection.CelestialBody.ShowPath = false;
                player.VisualPlayer.EditorCelestialBodyMenu.Visible = true;
            }

            else if (player.ActualSelection.OpenedMenu is EditorWorldMenu)
            {
                player.VisualPlayer.EditorWorldMenu.Visible = false;
            }

            else if (player.ActualSelection.OpenedMenu is EditorBuildMenu)
            {
                player.VisualPlayer.EditorBuildMenu.Visible = false;
            }
        }


        private void ExecuteCommand(EditorCommand command)
        {
            if (command is EditorCelestialBodyCommand)
                DoExecuteEditorCelestialBodyCommand((EditorCelestialBodyCommand) command);
            else if (command is EditorShowPanelCommand)
                DoExecuteEditorPanelCommand((EditorShowPanelCommand) command);
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
            if (command.Name == "Edit")
            {
                Simulator.EditMode = true;
                Simulator.Initialize();
                Simulator.SyncPlayers();
            }

            else if (command.Name == "Playtest")
            {
                if (Simulator.WorldMode)
                {
                    Main.CurrentWorld.World.Editing = false;
                    Simulator.Data.Level.SyncDescriptor();
                    Main.CurrentWorld.Initialize();
                }

                else
                {
                    Simulator.EditMode = false;
                    Simulator.Data.Level.SyncDescriptor();
                    Simulator.Initialize();
                    Simulator.SyncPlayers();
                }
            }

            //// toggle editor mode command
            //else if (command.Name == "PlaytestState")
            //{
            //    Simulator.EditMode = false;
            //    Simulator.Data.Level.SyncDescriptor();
            //    Simulator.Initialize();
            //    Simulator.SyncPlayers();
            //}

            //else if (command.Name == "EditState")
            //{
            //    Simulator.EditMode = true;
            //    Simulator.Initialize();
            //    Simulator.SyncPlayers();
            //}

            //else if (command.Name == "RestartSimulation")
            //{
            //    //if (Simulator.EditorState == EditorState.Editing)
            //    //    Simulator.SyncLevel();

            //    //Simulator.Initialize();
            //    //Simulator.SyncPlayers();
            //}

            //else if (command.Name == "PauseSimulation")
            //{
            //    //Simulator.State = GameState.Paused;
            //}

            //else if (command.Name == "ResumeSimulation")
            //{
            //    //Simulator.State = GameState.Running;
            //}

            //else if (command.Name == "NewLevel")
            //{
            //    //Simulator.LevelDescriptor = Main.WorldsFactory.GetEmptyLevelDescriptor();
            //    //Simulator.Initialize();
            //    //Simulator.SyncPlayers();
            //}

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


        private void DoExecuteEditorPanelCommand(EditorShowPanelCommand command)
        {
            if (command.Panel == PanelType.EditorCelestialBodyAssets)
                ((CelestialBodyAssetsPanel) Simulator.Data.Panels[command.Panel]).CelestialBody = command.Owner.ActualSelection.CelestialBody;
            
            if (command.Panel == PanelType.EditorCelestialBodyAttributes)
                ((CelestialBodyAttributesPanel) Simulator.Data.Panels[command.Panel]).CelestialBody = command.Owner.ActualSelection.CelestialBody;

            if (command.Panel == PanelType.EditorWaves)
                SyncWaves();

            NotifyEditorCommandExecuted(command);
        }


        private void DoExecuteEditorCelestialBodyCommand(EditorCelestialBodyCommand command)
        {
            if (command.Name == "AddPlanet")
            {
                command.CelestialBody = EditorLevelGenerator.GenerateCelestialBody(Simulator, VisualPriorities.Default.CelestialBody);

                if (Simulator.WorldMode)
                    Main.CurrentWorld.World.AddLevel();
            }

            else if (command.Name == "AddPinkHole")
            {
                command.CelestialBody = EditorLevelGenerator.GeneratePinkHole(Simulator, VisualPriorities.Default.CelestialBody);
            }

            else if (command.Name == "Move")
            {
                command.Owner.ActualSelection.CelestialBody.ShowPath = true;
                command.Owner.ActualSelection.EditingState = EditorEditingState.MovingCB;
                command.Owner.VisualPlayer.EditorCelestialBodyMenu.Visible = false;
            }

            else if(command.Name == "Rotate")
            {
                command.Owner.ActualSelection.CelestialBody.ShowPath = true;
                command.Owner.ActualSelection.EditingState = EditorEditingState.RotatingCB;
                command.Owner.VisualPlayer.EditorCelestialBodyMenu.Visible = false;
            }

            else if(command.Name == "Trajectory")
            {
                command.Owner.ActualSelection.CelestialBody.ShowPath = true;
                command.Owner.ActualSelection.EditingState = EditorEditingState.TrajectoryCB;
                command.Owner.VisualPlayer.EditorCelestialBodyMenu.Visible = false;
            }

            else
            {
                command.CelestialBody = command.Owner.ActualSelection.CelestialBody;
            }

            NotifyEditorCommandExecuted(command);
        }


        private void DoLives(PanelWidget widget)
        {
            var slider = (NumericHorizontalSlider) widget;

            var command = new EditorPlayerCommand("AddOrRemoveLives")
            {
                LifePoints = slider.Value
            };

            NotifyEditorCommandExecuted(command);
        }


        private void DoCash(PanelWidget widget)
        {
            var slider = (NumericHorizontalSlider) widget;

            var command = new EditorPlayerCommand("AddOrRemoveCash")
            {
                Cash = slider.Value
            };

            NotifyEditorCommandExecuted(command);
        }


        private void DoMinerals(PanelWidget widget)
        {
            var slider = (NumericHorizontalSlider) widget;

            var command = new EditorPlayerCommand("AddOrRemoveMinerals")
            {
                Minerals = slider.Value
            };

            NotifyEditorCommandExecuted(command);
        }


        private void DoBulletDamage(PanelWidget widget)
        {
            var slider = (NumericHorizontalSlider) widget;

            var command = new EditorPlayerCommand("AddOrRemoveBulletDamage")
            {
                BulletDamage = slider.Value
            };

            NotifyEditorCommandExecuted(command);
        }


        private void DoLifePacks(PanelWidget widget)
        {
            var slider = (NumericHorizontalSlider) widget;

            var command = new EditorPlayerCommand("AddOrRemoveLifePacks")
            {
                LifePacks = slider.Value
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

            NotifyEditorCommandExecuted(new EditorSimpleCommand("AddOrRemovePowerUp"));
        }


        private void DoBackgrounds(PanelWidget widget)
        {
            var img = (ImageWidget) ((GridPanel) widget).LastClickedWidget;

            Simulator.Data.Level.Background = new Image(img.Image.TextureName) { VisualPriority = Preferences.PrioriteFondEcran };
        }


        private void DoCelestialBodyAssets(PanelWidget widget)
        {
            var panel = (CelestialBodyAssetsPanel) Simulator.Data.Panels[PanelType.EditorCelestialBodyAssets];
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
                var enemiesAssets = (EnemiesAssetsPanel) Simulator.Data.Panels[PanelType.EditorEnemies];

                if (waveId < Simulator.Data.Level.Descriptor.Waves.Count)
                    enemiesAssets.Enemies = Simulator.Data.Level.Descriptor.Waves[waveId].Enemies;
                else
                    enemiesAssets.Enemies = new List<EnemyType>();

                enemiesAssets.Sync();
            }
        }


        private void SyncWaves()
        {
            List<WaveDescriptor> descriptors = new List<WaveDescriptor>();

            var panel = Simulator.Data.Panels[PanelType.EditorWaves];

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


        public void DoPanelClosed(PanelType type)
        {
            if (type == PanelType.EditorEnemies)
            {
                var enemiesAssets = (EnemiesAssetsPanel) Simulator.Data.Panels[PanelType.EditorEnemies];
                ((WavesPanel) Simulator.Data.Panels[PanelType.EditorWaves]).SyncEnemiesCurrentWave(enemiesAssets.Enemies);
            }
        }


        private void NotifyEditorCommandExecuted(EditorCommand command)
        {
            if (EditorCommandExecuted != null)
                EditorCommandExecuted(command);
        }
    }
}
