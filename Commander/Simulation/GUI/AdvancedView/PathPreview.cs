namespace EphemereGames.Commander.Simulation
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Utilities;
    using EphemereGames.Core.Visual;


    class PathPreview
    {
        private enum PathState
        {
            None,
            ObjectAdded,
            ObjectRemoved,
            ObjectUpgraded,
            RollbackingObjectAdded,
            RollbackingObjectRemoved,
            RollbackingObjectUpgraded
        }

        private Path NewPath;
        private Path ActualPath;
        private Dictionary<GUIPlayer, CelestialBody> CelestialObject;
        private Dictionary<GUIPlayer, PathState> State;
        private EffectsController<IVisual> EffectsController;
        private GUIPlayer FadePlayer;
        private int FadeCount;


        public PathPreview(Path newPath, Path actualPath)
        {
            NewPath = newPath;
            ActualPath = actualPath;
            State = new Dictionary<GUIPlayer, PathState>();
            CelestialObject = new Dictionary<GUIPlayer, CelestialBody>();
            EffectsController = new EffectsController<IVisual>();
            FadeCount = 0;
            FadePlayer = null;
        }


        public void DoPlayerConnected(GUIPlayer player)
        {
            State.Add(player, PathState.None);
            CelestialObject.Add(player, null);
        }


        public void DoPlayerDisconnected(GUIPlayer player)
        {
            State.Remove(player);
            CelestialObject.Remove(player);
        }


        public void AddCelestialObject(GUIPlayer player, CelestialBody obj)
        {
            if (State[player] == PathState.ObjectAdded || NewPath.ContainsCelestialBody(obj))
                return;

            CelestialObject[player] = obj;
            obj.FakeHasGravitationalTurret = true;
            NewPath.AddCelestialBody(obj, true);
            State[player] = PathState.ObjectAdded;

            SwitchToPreview();
        }


        public void RemoveCelestialObject(GUIPlayer player, CelestialBody obj)
        {
            if (State[player] == PathState.ObjectRemoved)
                return;

            CelestialObject[player] = obj;
            NewPath.RemoveCelestialBody(obj);
            State[player] = PathState.ObjectRemoved;

            SwitchToPreview();
        }


        public void UpgradeCelestialObject(GUIPlayer player, CelestialBody obj)
        {
            if (State[player] == PathState.ObjectUpgraded)
                return;

            CelestialObject[player] = obj;

            obj.FakeHasGravitationalTurretLv2 = true;
            State[player] = PathState.ObjectUpgraded;

            SwitchToPreview();
        }


        public void RollBack(GUIPlayer player)
        {
            if (State[player] != PathState.ObjectAdded &&
                State[player] != PathState.ObjectRemoved &&
                State[player] != PathState.ObjectUpgraded)
                return;

            FadeCount--;

            if (FadeCount == 0)
                DoDelayedRemoval(player);

            else
                DoRemoval(player);
        }


        public void Commit(GUIPlayer player)
        {
            if (CelestialObject[player] != null)
                CelestialObject[player].FakeHasGravitationalTurret = false;

            CelestialObject[player] = null;
            State[player] = PathState.None;

            EffectsController.Clear();

            FadeCount--;

            if (FadeCount == 0)
            {
                ActualPath.Fade(Math.Max((int) ActualPath.Alpha, 25), 100, EffectsController, null);
                NewPath.Fade(Math.Min((int) NewPath.Alpha, 200), 0, EffectsController, null);
            }
        }


        public void Update()
        {
            EffectsController.Update(Preferences.TargetElapsedTimeMs);
        }


        public void Draw()
        {
            if (FadeCount == 0 && EffectsController.ActiveEffectsCount == 0)
                return;

            NewPath.Draw();
        }


        private void SwitchToPreview()
        {
            if (FadeCount == 0)
            {
                ActualPath.Fade(Math.Min((int) ActualPath.Alpha, 100), 25, EffectsController, null);
                NewPath.Fade(Math.Max((int) NewPath.Alpha, 0), 200, EffectsController, null);
            }

            FadeCount++;
        }


        private void DoRemoval(GUIPlayer player)
        {
            switch (State[player])
            {
                case PathState.ObjectAdded:
                case PathState.RollbackingObjectAdded:
                    NewPath.RemoveCelestialBody(CelestialObject[player]);
                    CelestialObject[player].FakeHasGravitationalTurret = false;
                    State[player] = PathState.None;
                    break;
                case PathState.ObjectRemoved:
                case PathState.RollbackingObjectRemoved:
                    NewPath.AddCelestialBody(CelestialObject[player], false);
                    State[player] = PathState.None;
                    break;
                case PathState.ObjectUpgraded:
                case PathState.RollbackingObjectUpgraded:
                    CelestialObject[player].FakeHasGravitationalTurretLv2 = false;
                    State[player] = PathState.None;
                    break;
            }

            State[player] = PathState.None;
        }


        private void DoDelayedRemoval(GUIPlayer player)
        {
            ActualPath.Fade(Math.Max((int) ActualPath.Alpha, 25), 100, EffectsController, FadeCompleted);
            NewPath.Fade(Math.Min((int) NewPath.Alpha, 200), 0, EffectsController, FadeCompleted);
            FadePlayer = player;

            if (State[player] == PathState.ObjectAdded)
                State[player] = PathState.RollbackingObjectAdded;
            else if (State[player] == PathState.ObjectRemoved)
                State[player] = PathState.RollbackingObjectRemoved;
            else
                State[player] = PathState.RollbackingObjectUpgraded;
        }


        private void FadeCompleted(int id)
        {
            DoRemoval(FadePlayer);
        }
    }
}
