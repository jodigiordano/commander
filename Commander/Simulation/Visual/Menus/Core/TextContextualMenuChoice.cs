namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class TextContextualMenuChoice : ContextualMenuChoice
    {
        private Text Label;


        public TextContextualMenuChoice(string name, Text label)
            : base(name)
        {
            Label = label;
        }


        public void SetData(string data)
        {
            bool dataChanged = Label.Data != data;

            Label.Data = data;

            if (dataChanged)
                NotifyDataChanged();
        }


        public override Vector3 Position
        {
            set { Label.Position = value; }
        }


        public override Vector2 Size
        {
            get { return Label.AbsoluteSize; }
        }


        public override double VisualPriority
        {
            set { Label.VisualPriority = value; }
        }


        public void SetColor(Color color)
        {
            Label.Color = color;
        }


        public override void Draw()
        {
            Scene.Add(Label);
        }


        public override void Fade(FadeColorEffect effect)
        {
            Scene.VisualEffects.Add(Label, effect);
        }
    }
}
