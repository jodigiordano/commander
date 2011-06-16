namespace EphemereGames.Core.Visual
{
    public enum TransitionType
    {
        In,
        Out,
        None
    }


    class Transition
    {
        public Scene From { get { return Visuals.ScenesController.GetScene(FromName); } }
        public Scene To { get { return Visuals.ScenesController.GetScene(ToName); } }
        public Scene ActiveTransition;
        public TransitionType CurrentType;

        private string FromName;
        private string ToName;


        public Transition(string from, string to)
        {
            FromName = from;
            ToName = to;
        }


        public string Name
        {
            get { return FromName + "To" + ToName; }
        }
    }
}
