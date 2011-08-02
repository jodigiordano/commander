namespace EphemereGames.Commander
{
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;
    using ProjectMercury.Emitters;


    class PushButton : PanelWidget
    {
        protected Image Box;
        private Text Label;
        private Circle BoxCircle;
        private Particle Selection;

        public int MinSpaceForValue;
        public int MaxSpaceForValue;

        private Vector3 position;


        public PushButton(Text label)
            : this()
        {
            Label = label;
            MinSpaceForValue = 300;
            MaxSpaceForValue = 400;
        }


        public PushButton()
        {
            Label = new Text("Pixelite");
            Box = new Image("WidgetPush") { SizeX = 4, Origin = Vector2.Zero };
            BoxCircle = new Circle(Vector3.Zero, Box.AbsoluteSize.X / 2);
        }


        public override void Initialize()
        {
            Selection = Scene.Particles.Get(@"selectionCorpsCeleste");

            ((CircleEmitter) Selection.ParticleEffect[0]).Radius = BoxCircle.Radius + 5;
        }


        public override double VisualPriority
        {
            get
            {
                return Box.VisualPriority;
            }

            set
            {
                Box.VisualPriority = value;
                Label.VisualPriority = value;
                Selection.VisualPriority = value + 0.0000001;
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
                Box.Position = Label.Data.Length == 0 ? value : Label.Position + new Vector3(MathHelper.Clamp(Label.AbsoluteSize.X, MinSpaceForValue, MaxSpaceForValue), 0, 0);

                // Sync circles
                BoxCircle.Position = Box.Position + new Vector3(Box.AbsoluteSize / 2f, 0);

                // Center label
                Label.Position += new Vector3(0, (Box.AbsoluteSize.Y - Label.AbsoluteSize.Y) / 2, 0);
            }
        }


        public override byte Alpha
        {
            get { return Box.Alpha; }
            set { Box.Alpha = value; Label.Alpha = value; }
        }


        public override Vector3 Dimension
        {
            get { return new Vector3(Box.AbsoluteSize.X + Label.Data.Length == 0 ? 0 : MathHelper.Clamp(Label.AbsoluteSize.X, MinSpaceForValue, MaxSpaceForValue), Box.AbsoluteSize.Y, 0); }
            set { }
        }


        protected override bool Click(Circle circle)
        {
            if (Physics.CircleCicleCollision(circle, BoxCircle))
                return true;

            return false;
        }


        protected override bool Hover(Circle circle)
        {
            if (Physics.CircleCicleCollision(circle, BoxCircle))
            {
                Selection.Trigger(ref BoxCircle.Position);

                Sticky = true;
                return true;
            }


            Sticky = false;
            return false;
        }


        public override void Draw()
        {
            Scene.Add(Box);
            Scene.Add(Label);
        }


        public override void Fade(int from, int to, double length)
        {
            var effect = VisualEffects.Fade(from, to, 0, length);

            Box.Alpha = (byte) from;

            Scene.VisualEffects.Add(Box, effect);
        }
    }
}
