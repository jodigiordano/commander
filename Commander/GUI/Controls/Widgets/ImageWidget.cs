namespace EphemereGames.Commander
{
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;
    using ProjectMercury.Emitters;


    class ImageWidget : PanelWidget
    {
        public Image Image;

        private Circle ImageCircle;
        private Particle Selection;


        public ImageWidget(string imageName, float size)
        {
            Image = new Image(imageName) { SizeX = size, Origin = Vector2.Zero };

            ImageCircle = new Circle(Vector3.Zero, Image.AbsoluteSize.X / 2);
        }


        public override void Initialize()
        {
            Selection = Scene.Particles.Get(@"hoverRectangle");

            var emitter = (RectEmitter) Selection.Model[0];

            emitter.Width = Image.AbsoluteSize.X + 5;
            emitter.Height = Image.AbsoluteSize.Y + 5;
            emitter.TriggerOffset = Image.AbsoluteSize / 2;
        }


        public override double VisualPriority
        {
            get
            {
                return Image.VisualPriority;
            }

            set
            {
                Image.VisualPriority = value;

                if (Selection != null)
                    Selection.VisualPriority = value + 0.0000001;
            }
        }


        public override Vector3 Position
        {
            get
            {
                return Image.Position;
            }

            set
            {
                Image.Position = value;

                ImageCircle.Position = Image.Position + new Vector3(Image.AbsoluteSize / 2f, 0);
            }
        }


        public override byte Alpha
        {
            get { return Image.Alpha; }
            set { Image.Alpha = value; }
        }


        public override Vector3 Dimension
        {
            get { return new Vector3(Image.AbsoluteSize, 0); }
            set { }
        }


        protected override bool Click(Circle circle)
        {
            if (Physics.CircleCicleCollision(circle, ImageCircle))
                return true;

            return false;
        }


        protected override bool Hover(Circle circle)
        {
            if (Physics.CircleCicleCollision(circle, ImageCircle))
            {
                Selection.Trigger(ref Image.position);
                return true;
            }

            return false;
        }


        public override void Draw()
        {
            Scene.Add(Image);
        }


        public override void Fade(int from, int to, double length)
        {
            var effect = VisualEffects.Fade(from, to, 0, length);

            Image.Alpha = (byte) from;

            Scene.VisualEffects.Add(Image, effect);
        }
    }
}
