namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    abstract class ContextualMenuChoice
    {
        public Scene Scene;
        public string Name { get; private set; }
        public NoneHandler DoClick;

        public event NoneHandler DataChanged;
        public event NoneHandler AvailabilityChanged;

        private bool active;


        public ContextualMenuChoice(string name)
        {
            active = true;
            Name = name;
            DoClick = null;
        }


        public bool Active
        {
            get { return active; }
            set
            {
                bool changed = active != value;

                active = value;

                if (changed && AvailabilityChanged != null)
                    AvailabilityChanged();
            }
        }


        public abstract Vector3 Position            { set; }
        public abstract Vector2 Size                { get; }
        public abstract double VisualPriority       { set; }

        public abstract void Draw();
        public abstract void Fade(FadeColorEffect effect);


        protected void NotifyDataChanged()
        {
            if (DataChanged != null)
                DataChanged();
        }
    }
}
