namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class UpgradeTurretContextualMenuChoice : ContextualMenuChoice
    {
        private Text Price;
        private Text Level;
        private Image Logo;


        public UpgradeTurretContextualMenuChoice(string name, Text price, Text level, Image logo)
            : base(name)
        {
            Price = price;
            Level = level;
            Logo = logo;
        }


        public void SetLevel(string text)
        {
            Level.Data = text;
        }


        public void SetPrice(string text)
        {
            bool dataChanged = Price.Data != text;

            Price.Data = text;

            if (dataChanged)
                NotifyDataChanged();
        }


        public void SetColor(Color color)
        {
            Price.Color = color;
        }


        public override Vector3 Position
        {
            set
            {
                Logo.Position = value + new Vector3(0, 3, 0);
                Level.Position = value + new Vector3(20, 0, 0);
                Price.Position = value + new Vector3(60, 0, 0);
            }
        }


        public override Vector2 Size
        {
            get { return Logo.AbsoluteSize + new Vector2(Price.TextSize.X + 60, 0); }
        }


        public override double VisualPriority
        {
            set
            {
                Price.VisualPriority = value;
                Level.VisualPriority = value;
                Logo.VisualPriority = value;
            }
        }


        public override void Draw()
        {
            Scene.Add(Price);
            Scene.Add(Level);
            Scene.Add(Logo);
        }


        public override void Fade(FadeColorEffect effect)
        {
            Scene.VisualEffects.Add(Price, effect);
            Scene.VisualEffects.Add(Level, effect);
            Scene.VisualEffects.Add(Logo, effect);
        }
    }
}
