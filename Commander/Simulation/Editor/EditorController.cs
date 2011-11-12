namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
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
            Simulator.Data.Panels["EditorWaves"].SetClickHandler(DoWaves);
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
                cb.SteeringBehavior.BasePosition += player.DeltaPosition;
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
                var current = cb.SteeringBehavior.PathRotation;

                float to = 0;

                if (delta.X > 0 || delta.Y > 0)
                    to = current += 0.05f;
                else
                    to = current -= 0.05f;

                if (to < -MathHelper.TwoPi)
                    to += MathHelper.TwoPi;

                if (to > MathHelper.TwoPi)
                    to -= MathHelper.TwoPi;

                cb.SteeringBehavior.PathRotation = to;
            }

            else if (player.ActualSelection.EditingState == EditorEditingState.TrajectoryCB)
            {
                cb.SteeringBehavior.Path = new Vector3(cb.SteeringBehavior.Path.X + delta.X, cb.SteeringBehavior.Path.Y - delta.Y, 0);
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
            else if (command is EditorShowCBPanelCommand)
                DoExecuteEditorCBPanelCommand((EditorShowCBPanelCommand) command);
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

            NotifyEditorCommandExecuted(command);
        }


        private void DoExecuteEditorCBPanelCommand(EditorShowCBPanelCommand command)
        {
            var panel = (IEditorCBPanel) Simulator.Data.Panels[command.Panel];

            panel.CelestialBody = command.Owner.ActualSelection.CelestialBody;

            NotifyEditorCommandExecuted(command);
        }


        private void DoExecuteEditorPanelCommand(EditorShowPanelCommand command)
        {
            if (command.Panel == "EditorWaves")
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


        private void DoWaves(PanelWidget widget)
        {
            var clickedWidget = ((Panel) ((Panel) widget).LastClickedWidget).LastClickedWidget;
            var waveId = ((WaveSubPanel) widget).Id;

            if (clickedWidget.Name == "Enemies")
            {
                var enemiesAssets = (EnemiesAssetsPanel) Simulator.Data.Panels["EditorEnemies"];

                if (waveId < Simulator.Data.Level.Descriptor.Waves.Count)
                    enemiesAssets.Enemies = Simulator.Data.Level.Descriptor.Waves[waveId].Enemies;
                else
                    enemiesAssets.Enemies = new List<EnemyType>();

                enemiesAssets.Sync();
                DoExecuteEditorPanelCommand(new EditorShowPanelCommand("EditorEnemies"));
            }
        }


        private void SyncWaves()
        {
            List<WaveDescriptor> descriptors = new List<WaveDescriptor>();

            var panel = Simulator.Data.Panels["EditorWaves"];

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


        public void DoPanelClosed(string type)
        {
            if (type == "EditorEnemies")
            {
                var enemiesAssets = (EnemiesAssetsPanel) Simulator.Data.Panels["EditorEnemies"];
                ((WavesPanel) Simulator.Data.Panels["EditorWaves"]).SyncEnemiesCurrentWave(enemiesAssets.Enemies);
            }
        }


        public void NotifyEditorCommandExecuted(EditorCommand command)
        {
            if (EditorCommandExecuted != null)
                EditorCommandExecuted(command);
        }
    }
}
