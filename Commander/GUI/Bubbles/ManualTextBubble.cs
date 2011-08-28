namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class ManualTextBubble : Bubble
    {
        public Text Text;
        public bool Visible;


        public ManualTextBubble(CommanderScene scene, Text text, Vector3 position, double visualPriority)
            : base(scene, new PhysicalRectangle(), visualPriority)
        {
            Text = text;
            Text.VisualPriority = visualPriority - 0.00001;
            Position = position;

            ComputeSize();
            ComputePosition();

            Visible = false;
        }


        public override Color Color
        {
            get { return base.Color; }

            set
            {
                base.Color = value;

                if (Text != null)
                    Text.Color = value;
            }
        }


        public override byte Alpha
        {
            get { return base.Alpha; }

            set
            {
                base.Alpha = value;

                Text.Alpha = value;
            }
        }


        public virtual void Update()
        {
            Visible = Text.Color.A != 0;

            ComputeSize();
            ComputePosition();
        }


        public override void Draw()
        {
            base.Draw();

            Text.Position = new Vector3(Dimension.X, Dimension.Y, 0);
            Scene.Add(Text);
        }


        public override void FadeIn(double duration)
        {
            base.FadeIn(duration);

            Text.Alpha = 0;
            Scene.VisualEffects.Add(Text, VisualEffects.FadeInFrom0(255, 0, duration));
        }


        public override void FadeOut(double duration)
        {
            base.FadeOut(duration);

            Scene.VisualEffects.Add(Text, VisualEffects.FadeOutTo0(255, 0, duration));
        }


        private void ComputeSize()
        {
            Vector2 size = Text.AbsoluteSize;

            Dimension.Width = (int) size.X + 4;
            Dimension.Height = (int) size.Y + 4;
        }


        private void ComputePosition()
        {
            bool tooMuchRight = Dimension.X + Dimension.Width + 50 > Scene.CameraView.Right;
            bool tooMuchBottom = Dimension.Y + Dimension.Height > Scene.CameraView.Bottom;

            if (tooMuchRight && tooMuchBottom)
            {
                Dimension.X += -Dimension.Width - 50;
                Dimension.Y += -Dimension.Height - 10;
                BlaPosition = 2;
            }

            else if (tooMuchRight)
            {
                Dimension.X += -Dimension.Width - 50;
                BlaPosition = 1;
            }

            else if (tooMuchBottom)
            {
                Dimension.Y += -Dimension.Height - 50;
                BlaPosition = 3;
            }

            else
            {
                Dimension.X += 50;
                Dimension.Y += -10;
                BlaPosition = 0;
            }

            ClampPositionInView();
        }
    }
}
