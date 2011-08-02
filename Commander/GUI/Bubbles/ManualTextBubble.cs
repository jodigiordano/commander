namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class ManualTextBubble : Bubble
    {
        public Text Text;
        public bool Visible;


        public ManualTextBubble(Scene scene, Text text, Vector3 position, double visualPriority)
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


        public void Update()
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


        public override void FadeIn(double time)
        {
            base.FadeIn(time);

            Text.Alpha = 0;
            Scene.VisualEffects.Add(Text, VisualEffects.FadeInFrom0(255, 0, time));
        }


        public override void FadeOut(double time)
        {
            base.FadeOut(time);

            Scene.VisualEffects.Add(Text, VisualEffects.FadeOutTo0(255, 0, time));
        }


        private void ComputeSize()
        {
            Vector2 size = Text.AbsoluteSize;

            this.Dimension.Width = (int) size.X + 4;
            this.Dimension.Height = (int) size.Y + 4;
        }


        private void ComputePosition()
        {
            bool tropADroite = Dimension.X + Dimension.Width + 50 > 640 - Preferences.Xbox360DeadZoneV2.X;
            bool tropBas = Dimension.Y + Dimension.Height > 370 - Preferences.Xbox360DeadZoneV2.Y;

            if (tropADroite && tropBas)
            {
                Dimension.X += -Dimension.Width - 50;
                Dimension.Y += -Dimension.Height - 10;
                BlaPosition = 2;
            }

            else if (tropADroite)
            {
                Dimension.X += -Dimension.Width - 50;
                BlaPosition = 1;
            }

            else if (tropBas)
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

            Dimension.X = (int) MathHelper.Clamp(Dimension.X, -640 + Preferences.Xbox360DeadZoneV2.X, 640 - Preferences.Xbox360DeadZoneV2.X - Dimension.Width / 2);
            Dimension.Y = (int) MathHelper.Clamp(Dimension.Y, -370 + Preferences.Xbox360DeadZoneV2.Y + Dimension.Height / 2, 370 - Preferences.Xbox360DeadZoneV2.Y - Dimension.Height / 2);
        }
    }
}
