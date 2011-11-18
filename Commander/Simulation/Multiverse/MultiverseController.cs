namespace EphemereGames.Commander.Simulation
{
    using Microsoft.Xna.Framework;


    class MultiverseController
    {
        public event EditorCommandHandler EditorCommandExecuted;

        private Simulator Simulator;


        public MultiverseController(Simulator simulator)
        {
            Simulator = simulator;
        }


        public void Initialize()
        {

        }


        public void DoPlayerMoved(SimPlayer player)
        {
            if (!Simulator.EditingMode)
                return;

            if (player.ActualSelection.EditingState == EditorEditingState.None)
                return;

            var cb = player.ActualSelection.CelestialBody;

            if (cb == null)
                return;

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
            if (!Simulator.EditingMode)
                return;

            if (player.ActualSelection.EditingState == EditorEditingState.None)
                return;

            var cb = player.ActualSelection.CelestialBody;

            if (cb == null)
                return;

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
            var menu = player.ActualSelection.OpenedMenu; // here because can become null.

            if (menu != null && menu is MultiverseContextualMenu)
            {
                ((MultiverseContextualMenu) menu).DoClick();
            }

            else if (menu == null && Simulator.WorldMode && Simulator.EditingMode)
            {
                player.VisualPlayer.SetMenuVisibility("EditorBuildWorld", true);
            }

            else if (menu == null && !Simulator.GameMode)
            {
                player.VisualPlayer.SetMenuVisibility("MultiverseWorld", true);
            }

            else if (
                menu == null &&
                Simulator.GameMode &&
                Simulator.EditingMode &&
                player.ActualSelection.EditingState == EditorEditingState.None)
            {
                player.VisualPlayer.SetMenuVisibility("EditorBuildLevel", true);
            }
        }


        public void DoCancelAction(SimPlayer player)
        {
            if (player.ActualSelection.EditingState != EditorEditingState.None)
            {
                player.ActualSelection.EditingState = EditorEditingState.None;
                player.VisualPlayer.SetMenuVisibility("EditorCB", true);
            }

            else if (player.ActualSelection.OpenedMenu is MultiverseLevelBuildMenu ||
                     player.ActualSelection.OpenedMenu is MultiverseWorldBuildMenu ||
                     player.ActualSelection.OpenedMenu is MultiverseWorldMenu)
            {
                player.ActualSelection.OpenedMenu.Visible = false;
            }
        }


        public void ExecuteCommand(EditorCommand command)
        {
            NotifyEditorCommandExecuted(command);
        }


        private void NotifyEditorCommandExecuted(EditorCommand command)
        {
            if (EditorCommandExecuted != null)
                EditorCommandExecuted(command);
        }
    }
}
