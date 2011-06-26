namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class LogoTextContextualMenuChoice : ContextualMenuChoice
    {
        public Vector2 DistanceBetweenNameAndLogo;
        public Vector3 LogoOffet;

        private Text Name;
        private Image Logo;


        public LogoTextContextualMenuChoice(Text name, Image logo)
        {
            Name = name;
            Logo = logo;

            DistanceBetweenNameAndLogo = new Vector2(Logo.AbsoluteSize.X + 10, 4);
            LogoOffet = Vector3.Zero;
        }


        public void SetText(string text)
        {
            bool dataChanged = Name.Data != text;

            Name.Data = text;

            if (dataChanged)
                NotifyDataChanged();
        }


        public void SetColor(Color color)
        {
            Name.Color = color;
        }


        public override Vector3 Position
        {
            set
            {
                Logo.Position = value + LogoOffet;
                Name.Position = value + new Vector3(DistanceBetweenNameAndLogo, 0);
            }
        }


        public override Vector2 Size
        {
            get { return Name.TextSize + new Vector2(DistanceBetweenNameAndLogo.X, 0); }
        }


        public override double VisualPriority
        {
            set
            {
                Name.VisualPriority = value;
                Logo.VisualPriority = value;
            }
        }


        public override void Draw()
        {
            Scene.Add(Name);
            Scene.Add(Logo);
        }


        public override void Fade(FadeColorEffect effect)
        {
            Scene.VisualEffects.Add(Name, effect);
            Scene.VisualEffects.Add(Logo, effect);
        }
    }
}
