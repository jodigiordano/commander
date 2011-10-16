namespace EphemereGames.Commander
{
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class ImageCheckBox : CheckBox
    {
        private Image Label;

        private Vector3 position;


        public ImageCheckBox(Image label)
        {
            Label = label;
            Label.Origin = Vector2.Zero;
        }


        public override double VisualPriority
        {
            set
            {
                Label.VisualPriority = value;

                base.VisualPriority = value;
            }
        }


        public override byte Alpha
        {
            get { return base.Alpha; }
            set
            {
                Label.Alpha = value;

                base.Alpha = value;
            }
        }


        public override Vector3 Position
        {
            get
            {
                return position;
            }

            set
            {
                position = value;

                Label.Position = value;

                base.Position = Label.Position + new Vector3(Label.AbsoluteSize.X + 30, 0, 0);

                Label.Position += new Vector3(0, (Box.AbsoluteSize.Y - Label.AbsoluteSize.Y) / 2, 0);
            }
        }


        public override Vector3 Dimension
        {
            get { return Box.Position + base.Dimension - Label.Position; }
        }


        public override void Draw()
        {
            Scene.Add(Label);

            base.Draw();
        }


        public override void Fade(int from, int to, double length)
        {
            var effect = VisualEffects.Fade(from, to, 0, length);

            Label.Alpha = (byte) from;

            Scene.VisualEffects.Add(Label, effect);

            base.Fade(from, to, length);
        }
    }
}
