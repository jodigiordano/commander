namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class TextContextualMenuChoice : ContextualMenuChoice
    {
        private Text Name;


        public TextContextualMenuChoice(Text name)
        {
            Name = name;
        }


        public void SetData(string data)
        {
            bool dataChanged = Name.Data != data;

            Name.Data = data;

            if (dataChanged)
                NotifyDataChanged();
        }


        public override Vector3 Position
        {
            set { Name.Position = value; }
        }


        public override Vector2 Size
        {
            get { return Name.TextSize; }
        }


        public override double VisualPriority
        {
            set { Name.VisualPriority = value; }
        }


        public override void Draw()
        {
            Scene.Add(Name);
        }


        public override void Fade(FadeColorEffect effect)
        {
            Scene.VisualEffects.Add(Name, effect);
        }
    }
}
