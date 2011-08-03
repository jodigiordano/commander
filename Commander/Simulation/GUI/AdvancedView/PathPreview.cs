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
            RollbackingObjectAdded,
            RollbackingObjectRemoved
        }

        private Path Path;
        private Path ActualPath;
        private Dictionary<GUIPlayer, CelestialBody> CelestialObject;
        private Dictionary<GUIPlayer, PathState> State;
        private EffectsController<IVisual> EffectsController;
        private GUIPlayer FadePlayer;
        private int FadeCount;


        public PathPreview(Path path, Path actualPath)
        {
            Path = path;
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
            if (State[player] == PathState.ObjectAdded || Path.ContainsCelestialBody(obj))
                return;

            CelestialObject[player] = obj;
            obj.HasGravitationalTurretBypass = true;
            Path.AddCelestialBody(obj, true);
            State[player] = PathState.ObjectAdded;

            //EffectsController.Clear();

            if (FadeCount == 0)
            {
                ActualPath.Fade(Math.Min((int) ActualPath.Alpha, 100), 25, EffectsController, null);
                Path.Fade(Math.Max((int) Path.Alpha, 0), 200, EffectsController, null);
            }

            FadeCount++;
        }


        public void RemoveCelestialObject(GUIPlayer player, CelestialBody obj)
        {
            if (State[player] == PathState.ObjectRemoved)
                return;

            CelestialObject[player] = obj;
            Path.RemoveCelestialBody(obj);
            State[player] = PathState.ObjectRemoved;

            //EffectsController.Clear();

            if (FadeCount == 0)
            {
                ActualPath.Fade(Math.Min((int) ActualPath.Alpha, 100), 25, EffectsController, null);
                Path.Fade(Math.Max((int) Path.Alpha, 0), 200, EffectsController, null);
            }

            FadeCount++;
        }


        public void RollBack(GUIPlayer player)
        {
            if (State[player] == PathState.ObjectAdded || State[player] == PathState.ObjectRemoved)
            {
                //EffectsController.Clear();

                FadeCount--;

                if (FadeCount == 0)
                {
                    ActualPath.Fade(Math.Max((int) ActualPath.Alpha, 25), 100, EffectsController, FadeCompleted);
                    Path.Fade(Math.Min((int) Path.Alpha, 200), 0, EffectsController, FadeCompleted);
                    FadePlayer = player;

                    State[player] = State[player] == PathState.ObjectAdded ? PathState.RollbackingObjectAdded : PathState.RollbackingObjectRemoved;
                }

                else
                {
                    switch (State[player])
                    {
                        case PathState.ObjectAdded:
                            Path.RemoveCelestialBody(CelestialObject[player]);
                            CelestialObject[player].HasGravitationalTurretBypass = false;
                            State[player] = PathState.None;
                            break;
                        case PathState.ObjectRemoved:
                            Path.AddCelestialBody(CelestialObject[player], false);
                            State[player] = PathState.None;
                            break;
                    }

                    State[player] = PathState.None;
                }
            }
        }


        public void Commit(GUIPlayer player)
        {
            if (CelestialObject[player] != null)
                CelestialObject[player].HasGravitationalTurretBypass = false;

            CelestialObject[player] = null;
            State[player] = PathState.None;

            EffectsController.Clear();

            FadeCount--;

            if (FadeCount == 0)
            {
                ActualPath.Fade(Math.Max((int) ActualPath.Alpha, 25), 100, EffectsController, null);
                Path.Fade(Math.Min((int) Path.Alpha, 200), 0, EffectsController, null);
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

            Path.Draw();
        }


        private void FadeCompleted(int id)
        {
            switch (State[FadePlayer])
            {
                case PathState.RollbackingObjectAdded:
                    Path.RemoveCelestialBody(CelestialObject[FadePlayer]);
                    CelestialObject[FadePlayer].HasGravitationalTurretBypass = false;
                    State[FadePlayer] = PathState.None;
                    break;
                case PathState.RollbackingObjectRemoved:
                    Path.AddCelestialBody(CelestialObject[FadePlayer], false);
                    State[FadePlayer] = PathState.None;
                    break;
            }
        }
    }
}
