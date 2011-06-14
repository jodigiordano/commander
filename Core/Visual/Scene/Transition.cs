namespace EphemereGames.Core.Visual
{
    public enum TransitionType
    {
        In,
        Out,
        None
    }


    public class Transition
    {
        public float Length;
        public string NameSceneFrom;
        public string NameSceneTo;

        public Transition(string nameSceneFrom, string nameSceneTo)
        {
            NameSceneFrom = nameSceneFrom;
            NameSceneTo = nameSceneTo;
            Length = 0;
        }


        public string Name
        {
            get { return NameSceneFrom + "To" + NameSceneTo; }
        }
    }
}
