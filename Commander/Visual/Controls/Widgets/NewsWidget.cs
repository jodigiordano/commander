namespace EphemereGames.Commander
{
    using System;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class NewsWidget : PanelWidget
    {
        private Text Title;
        private Text Description;


        public NewsWidget(News news, int maxWidth)
        {
            Title = new Text(news.Title, @"Pixelite") { SizeX = 3, Color = Colors.Default.AlienBright };
            Description = new Text(news.Description, @"Pixelite") { SizeX = 2 }.CompartmentalizeIt(maxWidth);
        }


        public override double VisualPriority
        {
            get
            {
                return Title.VisualPriority;
            }

            set
            {
                Title.VisualPriority = value;
                Description.VisualPriority = value;
            }
        }


        public override Vector3 Position
        {
            get
            {
                return Title.Position;
            }

            set
            {
                Title.Position = value;
                Description.Position = value + new Vector3(0, Title.AbsoluteSize.Y + 10, 0);
            }
        }


        public override byte Alpha
        {
            get { return Title.Alpha; }
            set
            {
                Title.Alpha = value;
                Description.Alpha = value;
            }
        }


        public override Vector3 Dimension
        {
            get
            {
                return new Vector3(Math.Max(Title.AbsoluteSize.X, Description.AbsoluteSize.X), Title.AbsoluteSize.Y + Description.AbsoluteSize.Y + 10, 0);
            }

            set { }
        }


        protected override bool Click(Commander.Player player)
        {
            return false;
        }


        protected override bool Hover(Commander.Player player)
        {
            return false;
        }


        public override void Draw()
        {
            Scene.Add(Title);
            Scene.Add(Description);
        }


        public override void Fade(int from, int to, double length)
        {
            var effect = VisualEffects.Fade(from, to, 0, length);

            Title.Alpha = (byte) from;
            Description.Alpha = Title.Alpha;

            Scene.VisualEffects.Add(Title, effect);
            Scene.VisualEffects.Add(Description, effect);
        }
    }
}
