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

            Simulator.Data.Panels[PanelType.EditorCelestialBodyAttributes].SetClickHandler("HasMoons", DoHasMoons);
            Simulator.Data.Panels[PanelType.EditorCelestialBodyAttributes].SetClickHandler("FollowPath", DoFollowPath);
            Simulator.Data.Panels[PanelType.EditorCelestialBodyAttributes].SetClickHandler("CanSelect", DoCanSelect);
            Simulator.Data.Panels[PanelType.EditorCelestialBodyAttributes].SetClickHandler("StraightLine", DoStraightLine);
            Simulator.Data.Panels[PanelType.EditorCelestialBodyAttributes].SetClickHandler("Invincible", DoInvincible);

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

            var cb = ((EditorCelestialBodyCommand) player.ActualSelection.EditorCommand).CelestialBody;

            if (player.ActualSelection.EditingState == EditorEditingState.MovingCB)
            {
                cb.BasePosition += player.DeltaPosition;
                player.NinjaPosition = cb.Position;
            }

            else if (player.ActualSelection.EditingState == EditorEditingState.TrajectoryCB)
            {
                player.NinjaPosition = cb.Position;
                cb.Position = player.Position;
            }

            else if (player.ActualSelection.EditingState == EditorEditingState.RotatingCB)
            {
                player.NinjaPosition = cb.Position;
            }
        }


        public void DoPlayerMovedDelta(SimPlayer player, ref Vector3 delta)
        {
            if (!Simulator.EditorMode)
                return;

            if (player.ActualSelection.EditingState == EditorEditingState.None)
                return;

            var cb = ((EditorCelestialBodyCommand) player.ActualSelection.EditorCommand).CelestialBody;

            if (player.ActualSelection.EditingState == EditorEditingState.RotatingCB)
            {
                var current = cb.GetRotation();

                float to = 0;

                if (delta.X > 0 || delta.Y > 0)
                    to = current += 0.05f;
                else
                    to = current -= 0.05f;

                if (to < -MathHelper.TwoPi)
                    to += MathHelper.TwoPi;

                if (to > MathHelper.TwoPi)
                    to -= MathHelper.TwoPi;

                cb.SetRotation(to);
            }

            else if (player.ActualSelection.EditingState == EditorEditingState.TrajectoryCB)
            {
                cb.Path.X += delta.X;
                cb.Path.Y -= delta.Y;
            }
        }


        public void DoSelectAction(SimPlayer player)
        {
            if (!Simulator.EditorMode)
                return;

            if (Simulator.WorldMode && Simulator.EditorPlaytestingMode)
            {
                // the world level menu is open => do the action
                if (player.ActualSelection.OpenedMenu is EditorWorldLevelMenu)
                {
                    ExecuteCommand(player.ActualSelection.EditorCommand);
                    player.ActualSelection.OpenedMenu.DoCommandExecuted();
                }

                // open the world menu
                else if (!(player.ActualSelection.OpenedMenu is EditorWorldMenu))
                {
                    player.VisualPlayer.SetMenuVisibility("EditorWorld", true);
                }

                // the world menu is open => do the action
                else if (player.ActualSelection.OpenedMenu is EditorWorldMenu)
                {
                    ExecuteCommand(player.ActualSelection.EditorCommand);
                    player.ActualSelection.OpenedMenu.DoCommandExecuted();
                    player.VisualPlayer.SetMenuVisibility("EditorWorld", false);
                }
            }

            else if (Simulator.WorldMode && Simulator.EditorEditingMode)
            {
                // the celestial body menu is open => do the action
                if (player.ActualSelection.OpenedMenu is EditorCelestialBodyMenu)
                {
                    ExecuteCommand(player.ActualSelection.EditorCommand);
                    player.ActualSelection.OpenedMenu.DoCommandExecuted();
                    return;
                }

                if (player.ActualSelection.EditingState != EditorEditingState.None)
                    return;

                // open the build menu
                else if (!(player.ActualSelection.OpenedMenu is EditorWorldBuildMenu))
                {
                    player.VisualPlayer.SetMenuVisibility("EditorBuildWorld", true);
                }

                // the build menu is open => do the action
                else if (player.ActualSelection.OpenedMenu is EditorWorldBuildMenu)
                {
                    ExecuteCommand(player.ActualSelection.EditorCommand);
                    player.ActualSelection.OpenedMenu.DoCommandExecuted();
                    player.VisualPlayer.SetMenuVisibility("EditorBuildWorld", false);
                }
            }

            else if (Simulator.GameMode && Simulator.EditorEditingMode)
            {
                // the celestial body menu is open => do the action
                if (player.ActualSelection.OpenedMenu is EditorCelestialBodyMenu)
                {
                    ExecuteCommand(player.ActualSelection.EditorCommand);
                    player.ActualSelection.OpenedMenu.DoCommandExecuted();
                }

                if (player.ActualSelection.EditingState != EditorEditingState.None)
                    return;

                // open the build menu
                if (!(player.ActualSelection.OpenedMenu is EditorLevelBuildMenu))
                {
                    player.VisualPlayer.SetMenuVisibility("EditorBuildLevel", true);
                }

                // the build menu is open => do the action
                else if (player.ActualSelection.OpenedMenu is EditorLevelBuildMenu)
                {
                    ExecuteCommand(player.ActualSelection.EditorCommand);
                    player.ActualSelection.OpenedMenu.DoCommandExecuted();
                    player.VisualPlayer.SetMenuVisibility("EditorBuildLevel", false);
                }
            }
        }


        public void DoCancelAction(SimPlayer player)
        {
            if (!Simulator.EditorMode)
                return;

            if (player.ActualSelection.EditingState != EditorEditingState.None)
            {
                player.ActualSelection.EditingState = EditorEditingState.None;
                player.VisualPlayer.SetMenuVisibility("EditorCB", true);
            }

            else if (player.ActualSelection.OpenedMenu is EditorWorldMenu ||
                     player.ActualSelection.OpenedMenu is EditorLevelBuildMenu ||
                     player.ActualSelection.OpenedMenu is EditorWorldBuildMenu)
            {
                player.ActualSelection.OpenedMenu.Visible = false;
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

            else if (command.Name == "EditLevel")
            {
                Main.CurrentWorld.World.EditorMode = true;
                Main.CurrentWorld.World.Editing = true;
                Main.CurrentWorld.DoSelectActionEditor(command.Owner.InnerPlayer);
            }

            else if (command.Name == "PlaytestLevel")
            {
                Main.CurrentWorld.World.EditorMode = true;
                Main.CurrentWorld.World.Editing = false;
                Main.CurrentWorld.DoSelectActionEditor(command.Owner.InnerPlayer);
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
                command.CelestialBody = EditorLevelGenerator.GeneratePlanetCB(Simulator, VisualPriorities.Default.CelestialBody);

                if (Simulator.WorldMode)
                    Main.CurrentWorld.AddLevel(command.CelestialBody);
            }

            else if (command.Name == "AddPinkHole")
            {
                command.CelestialBody = EditorLevelGenerator.GeneratePinkHoleCB(Simulator, VisualPriorities.Default.CelestialBody);
            }


            else if (command.Name == "Remove")
            {
                command.CelestialBody = command.Owner.ActualSelection.CelestialBody;

                if (Simulator.WorldMode)
                {
                    if (command.CelestialBody is PinkHole)
                    {

                    }

                    else
                    {
                        Main.CurrentWorld.RemoveLevel(command.CelestialBody);
                    }
                }
            }

            else if (command.Name == "Move")
            {
                command.CelestialBody = command.Owner.ActualSelection.CelestialBody;
                command.Owner.ActualSelection.EditingState = EditorEditingState.MovingCB;
                command.Owner.VisualPlayer.SetMenuVisibility("EditorCB", false);
            }

            else if (command.Name == "Rotate")
            {
                command.CelestialBody = command.Owner.ActualSelection.CelestialBody;
                command.Owner.ActualSelection.EditingState = EditorEditingState.RotatingCB;
                command.Owner.VisualPlayer.SetMenuVisibility("EditorCB", false);
            }

            else if (command.Name == "Trajectory")
            {
                command.CelestialBody = command.Owner.ActualSelection.CelestialBody;
                command.Owner.ActualSelection.EditingState = EditorEditingState.TrajectoryCB;
                command.Owner.VisualPlayer.SetMenuVisibility("EditorCB", false);
            }

            else
            {
                command.CelestialBody = command.Owner.ActualSelection.CelestialBody;
            }

            NotifyEditorCommandExecuted(command);
        }


        private void DoHasMoons(PanelWidget widget)
        {
            var panel = (CelestialBodyAttributesPanel) Simulator.Data.Panels[PanelType.EditorCelestialBodyAttributes];

            NotifyEditorCommandExecuted(new EditorCelestialBodyCommand("HasMoons")
            {
                HasMoons = ((CheckBox) widget).Value,
                CelestialBody = panel.CelestialBody
            });
        }


        private void DoFollowPath(PanelWidget widget)
        {
            var panel = (CelestialBodyAttributesPanel) Simulator.Data.Panels[PanelType.EditorCelestialBodyAttributes];

            NotifyEditorCommandExecuted(new EditorCelestialBodyCommand("FollowPath")
            {
                FollowPath = ((CheckBox) widget).Value,
                CelestialBody = panel.CelestialBody
            });
        }


        private void DoCanSelect(PanelWidget widget)
        {
            var panel = (CelestialBodyAttributesPanel) Simulator.Data.Panels[PanelType.EditorCelestialBodyAttributes];

            NotifyEditorCommandExecuted(new EditorCelestialBodyCommand("CanSelect")
            {
                CanSelect = ((CheckBox) widget).Value,
                CelestialBody = panel.CelestialBody
            });
        }


        private void DoStraightLine(PanelWidget widget)
        {
            var panel = (CelestialBodyAttributesPanel) Simulator.Data.Panels[PanelType.EditorCelestialBodyAttributes];

            NotifyEditorCommandExecuted(new EditorCelestialBodyCommand("StraightLine")
            {
                StraightLine = ((CheckBox) widget).Value,
                CelestialBody = panel.CelestialBody
            });
        }


        private void DoInvincible(PanelWidget widget)
        {
            var panel = (CelestialBodyAttributesPanel) Simulator.Data.Panels[PanelType.EditorCelestialBodyAttributes];

            NotifyEditorCommandExecuted(new EditorCelestialBodyCommand("Invincible")
            {
                Invincible = ((CheckBox) widget).Value,
                CelestialBody = panel.CelestialBody
            });
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
                DoExecuteEditorPanelCommand(new EditorShowPanelCommand(PanelType.EditorEnemies));
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
