using Microsoft.Xna.Framework;
namespace EphemereGames.Commander
{
    class PathPreview
    {
        public enum PathState
        {
            None,
            ObjectAdded,
            ObjectRemoved
        }

        private Path Path;
        private Path ActualPath;
        private CorpsCeleste CelestialObject;
        private PathState State;


        public PathPreview(Path path, Path actualPath)
        {
            Path = path;
            ActualPath = actualPath;
            State = PathState.None;
        }


        public void AddCelestialObject(CorpsCeleste obj)
        {
            if (State == PathState.ObjectAdded)
                return;

            CelestialObject = obj;
            CelestialObject.ContientTourelleGravitationnelleByPass = true;
            Path.ajouterCorpsCeleste(obj);
            State = PathState.ObjectAdded;
            ActualPath.AlphaChannel = 25;
            Path.AlphaChannel = 200;
        }


        public void RemoveCelestialObject(CorpsCeleste obj)
        {
            if (State == PathState.ObjectRemoved)
                return;

            CelestialObject = obj;
            CelestialObject.ContientTourelleGravitationnelleByPass = false;
            Path.enleverCorpsCeleste(obj);
            State = PathState.ObjectRemoved;
            ActualPath.AlphaChannel = 25;
            Path.AlphaChannel = 200;
        }


        public void RollBack()
        {
            switch (State)
            {
                case PathState.ObjectAdded:
                    Path.enleverCorpsCeleste(CelestialObject);
                    CelestialObject = null;
                    State = PathState.None;
                    break;
                case PathState.ObjectRemoved:
                    Path.ajouterCorpsCeleste(CelestialObject);
                    CelestialObject = null;
                    State = PathState.None;
                    break;
            }

            ActualPath.AlphaChannel = 100;
            Path.AlphaChannel = 25;
        }


        public void Commit()
        {
            CelestialObject = null;
            State = PathState.None;
            ActualPath.AlphaChannel = 100;
            Path.AlphaChannel = 25;
        }


        public void Draw()
        {
            if (State == PathState.None)
                return;

            Path.Update(null);
            Path.Draw();
        }
    }
}
