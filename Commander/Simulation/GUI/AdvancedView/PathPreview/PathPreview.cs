namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;


    class PathPreview
    {
        private enum PathState
        {
            None,
            ObjectAdded,
            ObjectRemoved,
            ObjectUpgraded
        }


        private Path NewPath;
        private Path ActualPath;
        private Dictionary<GUIPlayer, PlayerState> PlayersStates;
        private PathPreviewVisual PPVisual;
        private PlayerState VisualState;


        public PathPreview(Path newPath, Path actualPath)
        {
            NewPath = newPath;
            ActualPath = actualPath;
            PPVisual = new PathPreviewVisual(newPath, actualPath, DoHideCompleted);
            PlayersStates = new Dictionary<GUIPlayer, PlayerState>();
        }


        public void DoPlayerConnected(GUIPlayer player)
        {
            PlayersStates.Add(player, new PlayerState(player));
        }


        public void DoPlayerDisconnected(GUIPlayer player)
        {
            if (IsFirstOrLast)
            {
                VisualState = PlayersStates[player].Clone();
                PPVisual.Hide();
            }

            PlayersStates.Remove(player);
        }


        public void AddCelestialObject(GUIPlayer player, CelestialBody obj)
        {
            var state = PlayersStates[player];

            if (state.PathState == PathState.ObjectAdded || ActualPath.ContainsCelestialBody(obj))
                return;

            if (IsFirstOrLast)
                PPVisual.Cancel();

            state.ChangeState(PathState.ObjectAdded, NewPath, obj);

            if (IsFirstOrLast)
            {
                VisualState = state.Clone();
                PPVisual.Show();
            }
        }


        public void RemoveCelestialObject(GUIPlayer player, CelestialBody obj)
        {
            var state = PlayersStates[player];

            if (state.PathState == PathState.ObjectRemoved)
                return;

            if (IsFirstOrLast)
                PPVisual.Cancel();

            state.ChangeState(PathState.ObjectRemoved, NewPath, obj);

            if (IsFirstOrLast)
            {
                VisualState = state.Clone();
                PPVisual.Show();
            }
        }


        public void UpgradeCelestialObject(GUIPlayer player, CelestialBody obj)
        {
            var state = PlayersStates[player];

            if (state.PathState == PathState.ObjectUpgraded)
                return;

            if (IsFirstOrLast)
                PPVisual.Cancel();

            state.ChangeState(PathState.ObjectUpgraded, NewPath, obj);

            if (IsFirstOrLast)
            {
                VisualState = state.Clone();
                PPVisual.Show();
            }
        }


        public void RollBack(GUIPlayer player)
        {
            var state = PlayersStates[player];

            if (state.PathState == PathState.None)
                return;

            if (IsFirstOrLast)
            {
                PPVisual.Cancel();
                VisualState = state.Clone();
                PPVisual.Hide();
                state.DeferredRollBack();
            }

            else
            {
                state.RollBack(NewPath);
            }
        }


        public void Commit(GUIPlayer player)
        {
            var state = PlayersStates[player];

            if (state.PathState == PathState.None)
                return;

            if (IsFirstOrLast)
            {
                VisualState = null;
                PPVisual.Hide();
            }

            state.Commit(NewPath);
        }


        public void Update()
        {
            NewPath.Active = NewPath.Alpha != 0;
            PPVisual.Update();
        }


        private bool IsFirstOrLast
        {
            get
            {
                int count = 0;

                foreach (var p in PlayersStates.Values)
                    if (p.PathState != PathState.None)
                        count++;

                return count <= 1;
            }
        }


        private void DoHideCompleted()
        {
            if (VisualState == null)
                return;

            VisualState.RollBack(NewPath);
            VisualState = null;
        }


        private class PlayerState
        {
            public GUIPlayer Player             { get; private set; }
            public CelestialBody CelestialBody  { get; private set; }
            public PathState PathState          { get; private set; }


            public PlayerState(GUIPlayer player)
            {
                Player = player;
                CelestialBody = null;
                PathState = PathState.None;
            }


            public PlayerState Clone()
            {
                return new PlayerState(Player)
                {
                    CelestialBody = CelestialBody,
                    PathState = PathState
                };
            }


            public void ChangeState(PathState state, Path newPath, CelestialBody cb)
            {
                RollBack(newPath);

                CelestialBody = cb;
                PathState = state;

                switch (state)
                {
                    case PathState.ObjectAdded:
                        cb.FakeHasGravitationalTurret = true;
                        newPath.AddCelestialBody(cb, true);
                        break;
                    case PathState.ObjectRemoved:
                        newPath.RemoveCelestialBody(cb);
                        break;
                    case PathState.ObjectUpgraded:
                        cb.FakeHasGravitationalTurretLv2 = true;
                        break;
                }
            }


            public void DeferredRollBack()
            {
                PathState = PathState.None;
            }


            public void RollBack(Path newPath)
            {
                if (PathState == PathState.None)
                    return;

                switch (PathState)
                {
                    case PathState.ObjectAdded:
                        newPath.RemoveCelestialBody(CelestialBody);
                        CelestialBody.FakeHasGravitationalTurret = false;
                        break;
                    case PathState.ObjectRemoved:
                        newPath.AddCelestialBody(CelestialBody, false);
                        break;
                    case PathState.ObjectUpgraded:
                        CelestialBody.FakeHasGravitationalTurretLv2 = false;
                        break;
                }


                CelestialBody = null;
                PathState = PathState.None;
            }


            public void Commit(Path newPath)
            {
                CelestialBody.FakeHasGravitationalTurret = false;
                CelestialBody.FakeHasGravitationalTurretLv2 = false;
                CelestialBody = null;
                PathState = PathState.None;
            }
        }
    }
}
