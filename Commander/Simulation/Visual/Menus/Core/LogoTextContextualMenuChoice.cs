namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class LogoTextContextualMenuChoice : ContextualMenuChoice
    {
        public Vector2 DistanceBetweenNameAndLogo;
        public Vector3 LogoOffet;

        private Text Label;
        private Image Logo;


        public LogoTextContextualMenuChoice(string name, Text label, Image logo)
            : base(name)
        {
            Label = label;
            Logo = logo;

            DistanceBetweenNameAndLogo = new Vector2(Logo.AbsoluteSize.X + 10, 4);
            LogoOffet = Vector3.Zero;
        }


        public void SetText(string text)
        {
            bool dataChanged = Label.Data != text;

            Label.Data = text;

            if (dataChanged)
                NotifyDataChanged();
        }


        public void SetColor(Color color)
        {
            Label.Color = color;
        }


        public override Vector3 Position
        {
            set
            {
                Logo.Position = value + LogoOffet;
                Label.Position = value + new Vector3(DistanceBetweenNameAndLogo, 0);
            }
        }


        public override Vector2 Size
        {
            get { return Label.AbsoluteSize + new Vector2(DistanceBetweenNameAndLogo.X, 0); }
        }


        public override double VisualPriority
        {
            set
            {
                Label.VisualPriority = value;
                Logo.VisualPriority = value;
            }
        }


        public override void Draw()
        {
            Scene.Add(Label);
            Scene.Add(Logo);
        }


        public override void Fade(FadeColorEffect effect)
        {
            Scene.VisualEffects.Add(Label, effect);
            Scene.VisualEffects.Add(Logo, effect);
        }
    }
}
