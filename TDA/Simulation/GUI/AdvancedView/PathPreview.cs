using Microsoft.Xna.Framework;
namespace TDA
{
    class PathPreview
    {
        public enum PathState
        {
            None,
            ObjectAdded,
            ObjectRemoved
        }

        private Chemin Path;
        private CorpsCeleste CelestialObject;
        private PathState State;


        public PathPreview(Chemin path)
        {
            Path = path;
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
        }


        public void RemoveCelestialObject(CorpsCeleste obj)
        {
            if (State == PathState.ObjectRemoved)
                return;

            CelestialObject = obj;
            CelestialObject.ContientTourelleGravitationnelleByPass = false;
            Path.enleverCorpsCeleste(obj);
            State = PathState.ObjectRemoved;
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
