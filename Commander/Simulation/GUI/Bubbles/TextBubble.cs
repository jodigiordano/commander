namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class TextBubble : Bubble
    {
        public Text Text;
        public double ShowTime;
        public double FadeTime;
        public bool Visible;


        public TextBubble(Simulator simulator, Text text, Vector3 position, double showTime, double visualPriorirty)
            : base(simulator, new PhysicalRectangle(), visualPriorirty)
        {
            Text = text;
            Text.VisualPriority = visualPriorirty - 0.01f;
            Position = position;
            ShowTime = showTime;
            FadeTime = double.MaxValue;

            ComputeSize();
            ComputePosition();

            Visible = false;
        }


        public Vector3 Position
        {
            set
            {
                Dimension.X = (int) value.X;
                Dimension.Y = (int) value.Y;
            }
        }


        public bool Finished
        {
            get { return ShowTime <= 0; }
        }


        public void Update()
        {
            ShowTime -= Preferences.TargetElapsedTimeMs;

            if (ShowTime <= FadeTime)
            {
                FadeTime = double.NaN;
                FadeOut(ShowTime);
            }

            Visible = Text.Color.A != 0;

            ComputeSize();
            ComputePosition();
        }


        public override void Draw()
        {
            base.Draw();

            Text.Position = new Vector3(Dimension.X, Dimension.Y, 0);
            Simulator.Scene.Add(Text);
        }


        public override void FadeIn(double temps)
        {
            base.FadeIn(temps);

            Text.Alpha = 0;
            Simulator.Scene.VisualEffects.Add(Text, VisualEffects.FadeInFrom0(255, 0, temps));
        }


        public override void FadeOut(double temps)
        {
            base.FadeOut(temps);

            Simulator.Scene.VisualEffects.Add(Text, VisualEffects.FadeOutTo0(255, 0, temps));
        }


        private void ComputeSize()
        {
            Vector2 size = Text.TextSize;

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
