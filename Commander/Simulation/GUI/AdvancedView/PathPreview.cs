namespace EphemereGames.Commander.Simulation
{
    using System;
    using EphemereGames.Core.Utilities;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class PathPreview
    {
        public enum PathState
        {
            None,
            ObjectAdded,
            ObjectRemoved,
            RollbackingObjectAdded,
            RollbackingObjectRemoved
        }

        private Path Path;
        private Path ActualPath;
        private CelestialBody CelestialObject;
        private PathState State;
        private EffectsController<IVisual> EffectsController;


        public PathPreview(Path path, Path actualPath)
        {
            Path = path;
            ActualPath = actualPath;
            State = PathState.None;
            EffectsController = new EffectsController<IVisual>();
        }


        public void AddCelestialObject(CelestialBody obj)
        {
            if (State == PathState.ObjectAdded || Path.ContainsCelestialBody(obj))
                return;

            if (State == PathState.RollbackingObjectAdded || State == PathState.RollbackingObjectRemoved)
                EffectsController.Clear();

            CelestialObject = obj;
            CelestialObject.HasGravitationalTurretBypass = true;
            Path.AddCelestialBody(obj);
            State = PathState.ObjectAdded;

            EffectsController.Clear();

            ActualPath.Fade(Math.Min((int) ActualPath.Alpha, 100), 25, EffectsController, null);
            Path.Fade(Math.Max((int) Path.Alpha, 0), 200, EffectsController, null);
        }


        public void RemoveCelestialObject(CelestialBody obj)
        {
            if (State == PathState.ObjectRemoved)
                return;

            CelestialObject = obj;
            Path.RemoveCelestialBody(obj);
            State = PathState.ObjectRemoved;

            EffectsController.Clear();

            ActualPath.Fade(Math.Min((int) ActualPath.Alpha, 100), 25, EffectsController, null);
            Path.Fade(Math.Max((int) Path.Alpha, 0), 200, EffectsController, null);
        }


        public void RollBack()
        {
            if (State == PathState.ObjectAdded || State == PathState.ObjectRemoved)
            {
                EffectsController.Clear();

                ActualPath.Fade(Math.Max((int) ActualPath.Alpha, 25), 100, EffectsController, FadeCompleted);
                Path.Fade(Math.Min((int) Path.Alpha, 200), 0, EffectsController, FadeCompleted);

                State = State == PathState.ObjectAdded ? PathState.RollbackingObjectAdded : PathState.RollbackingObjectRemoved;
            }
        }


        public void Commit()
        {
            if (CelestialObject != null)
                CelestialObject.HasGravitationalTurretBypass = false;

            CelestialObject = null;
            State = PathState.None;

            EffectsController.Clear();

            ActualPath.Fade(Math.Max((int) ActualPath.Alpha, 25), 100, EffectsController, null);
            Path.Fade(Math.Min((int) Path.Alpha, 200), 0, EffectsController, null);
        }


        public void Update(GameTime gameTime)
        {
            EffectsController.Update(gameTime);
        }


        public void Draw()
        {
            if (State == PathState.None && EffectsController.ActiveEffectsCount == 0)
                return;

            Path.Draw();
        }


        private void FadeCompleted()
        {
            switch (State)
            {
                case PathState.RollbackingObjectAdded:
                    Path.RemoveCelestialBody(CelestialObject);
                    CelestialObject.HasGravitationalTurretBypass = false;
                    break;
                case PathState.RollbackingObjectRemoved:
                    Path.AddCelestialBody(CelestialObject);
                    break;
            }
        }
    }
}
