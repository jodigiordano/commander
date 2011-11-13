namespace EphemereGames.Commander.Simulation
{
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
            //Simulator.Data.Panels["EditorWaves"].SetClickHandler(DoWaves);
        }


        public void DoPlayerMoved(SimPlayer player)
        {
            if (!Simulator.EditorMode)
                return;

            if (player.ActualSelection.EditingState == EditorEditingState.None)
                return;

            var cb = player.ActualSelection.CelestialBody;

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

            var cb = player.ActualSelection.CelestialBody;

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

            var menu = player.ActualSelection.OpenedMenu; // here because can become null.

            if (menu != null && menu is EditorContextualMenu)
            {
                ((EditorContextualMenu) menu).DoClick();
            }

            else if (menu == null && Simulator.WorldMode && Simulator.EditorMode)
            {
                player.VisualPlayer.SetMenuVisibility("EditorBuildWorld", true);
            }

            else if (
                menu == null &&
                Simulator.GameMode &&
                Simulator.EditorEditingMode &&
                player.ActualSelection.EditingState == EditorEditingState.None)
            {
                player.VisualPlayer.SetMenuVisibility("EditorBuildLevel", true);
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

            else if (player.ActualSelection.OpenedMenu is EditorLevelBuildMenu ||
                     player.ActualSelection.OpenedMenu is EditorWorldBuildMenu)
            {
                player.ActualSelection.OpenedMenu.Visible = false;
            }
        }


        public void ExecuteCommand(EditorCommand command)
        {
            NotifyEditorCommandExecuted(command);
        }


        //private void DoExecuteEditorPanelCommand(EditorPanelShowCommand command)
        //{
        //    if (command.Panel == "EditorWaves")
        //        SyncWaves();

        //    NotifyEditorCommandExecuted(command);
        //}


        private void DoWaves(PanelWidget widget, Commander.Player player)
        {
            //var clickedWidget = ((Panel) ((Panel) widget).LastClickedWidget).LastClickedWidget;
            //var waveId = ((WaveSubPanel) widget).Id;

            //if (clickedWidget.Name == "Enemies")
            //{
            //    var enemiesAssets = (EnemiesAssetsPanel) Simulator.Data.Panels["EditorEnemies"];

            //    if (waveId < Simulator.Data.Level.Descriptor.Waves.Count)
            //        enemiesAssets.Enemies = Simulator.Data.Level.Descriptor.Waves[waveId].Enemies;
            //    else
            //        enemiesAssets.Enemies = new List<EnemyType>();

            //    enemiesAssets.Sync();
            //    DoExecuteEditorPanelCommand(new EditorPanelShowCommand("EditorEnemies"));
            //}
        }


        //private void SyncWaves()
        //{
        //    List<WaveDescriptor> descriptors = new List<WaveDescriptor>();

        //    var panel = Simulator.Data.Panels["EditorWaves"];

        //    foreach (var w in panel.Widgets)
        //    {
        //        var subPanel = (WaveSubPanel) w.Value;

        //        if (subPanel.EnemiesCount != 0 && subPanel.Quantity != 0)
        //            descriptors.Add(subPanel.GenerateDescriptor());
        //    }

        //    Simulator.Data.Level.Waves.Clear();

        //    foreach (var wd in descriptors)
        //        Simulator.Data.Level.Waves.AddLast(new Wave(Simulator, wd));
        //}


        //public void DoPanelClosed(string type)
        //{
        //    if (type == "EditorEnemies")
        //    {
        //        var enemiesAssets = (EnemiesAssetsPanel) Simulator.Data.Panels["EditorEnemies"];
        //        ((WavesPanel) Simulator.Data.Panels["EditorWaves"]).SyncEnemiesCurrentWave(enemiesAssets.Enemies);
        //    }
        //}


        private void NotifyEditorCommandExecuted(EditorCommand command)
        {
            if (EditorCommandExecuted != null)
                EditorCommandExecuted(command);
        }
    }
}
