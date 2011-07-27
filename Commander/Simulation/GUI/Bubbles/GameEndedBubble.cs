namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class GameEndedBubble : Bubble
    {
        private Text Quote;
        private ScoreStars Stars;
        private Text Score;
        private int DistanceY;


        public GameEndedBubble(Scene scene, double visualPriority, string quote, Color quoteColor, int score, int nbStars)
            : base(scene, new PhysicalRectangle(), visualPriority)
        {
            DistanceY = 15;

            Quote = new Text(quote, "Pixelite") { SizeX = 2, Color = quoteColor, VisualPriority = visualPriority - 0.00001 };
            Stars = new ScoreStars(Scene, nbStars, visualPriority - 0.00001);
            Score = new Text("Score: " + score, "Pixelite") { SizeX = 2, VisualPriority = visualPriority - 0.00001 };

            ComputeSize();
        }


        public override Color Color
        {
            get { return base.Color; }

            set
            {
                base.Color = value;

                if (Quote != null)
                {
                    Quote.Color = value;
                    Stars.Color = value;
                    Score.Color = value;
                }
            }
        }


        public override byte Alpha
        {
            get { return base.Alpha; }

            set
            {
                base.Alpha = value;

                if (Quote != null)
                {
                    Quote.Alpha = value;
                    Stars.Alpha = value;
                    Score.Alpha = value;
                }
            }
        }


        public override Vector3 Position
        {
            get
            {
                return base.Position;
            }
            set
            {
                base.Position = value;

                bool tooFarRight = Dimension.X + Dimension.Width + 50 > 640 - Preferences.Xbox360DeadZoneV2.X;
                bool tooFarBottom = Dimension.Y + Dimension.Height > 370 - Preferences.Xbox360DeadZoneV2.Y;

                if (tooFarRight && tooFarBottom)
                {
                    Dimension.X += -Dimension.Width - 50;
                    Dimension.Y += -Dimension.Height - 10;
                    BlaPosition = 2;
                }

                else if (tooFarRight)
                {
                    Dimension.X += -Dimension.Width - 50;
                    BlaPosition = 1;
                }

                else if (tooFarBottom)
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


        public override void Draw()
        {
            base.Draw();

            Quote.Position = new Vector3(Dimension.X, Dimension.Y, 0);
            Score.Position = new Vector3(Dimension.X, Quote.Position.Y + Quote.TextSize.Y + DistanceY, 0);
            Stars.Position = new Vector3(Dimension.X, Score.Position.Y + Score.TextSize.Y + DistanceY, 0);

            Scene.Add(Quote);
            Scene.Add(Score);
            Stars.Draw();
        }


        private void ComputeSize()
        {
            // find the max X
            float sizeX = Quote.TextSize.X;

            if (Stars.Size.X > sizeX)
                sizeX = Stars.Size.X;

            if (Score.TextSize.X > sizeX)
                sizeX = Score.TextSize.X;

            float sizeY = Quote.TextSize.Y + Stars.Size.Y + Score.TextSize.Y + 2 * DistanceY;

            Dimension.Width = (int) sizeX + 4;
            Dimension.Height = (int) sizeY + 4;
        }
    }
}
