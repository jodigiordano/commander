namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class ScoreCalculation : IVisual
    {
        private Text Score;
        private Text RemainingCash;
        private Text RemainingLives;
        private Text TimeTaken;
        private Text Percentage;
        private Text FinalScore;
        private Image RemainingCashLogo;
        private Image TimeTakenLogo;
        private Image RemainingLivesLogo;

        private float IndividualShowLength;
        private float Elapsed;

        private Scene Scene;


        public ScoreCalculation(Scene scene, int remainingCash, int remainingLifes, int timeTaken, int percentage, double visualPriority)
        {
            Scene = scene;

            Score = new Text("Score: ", @"Pixelite", Vector3.Zero)
            {
                SizeX = 2,
                Alpha = 0
            };

            RemainingCash = new Text(remainingCash.ToString(), @"Pixelite", Vector3.Zero)
            {
                SizeX = 2,
                Alpha = 0
            };


            RemainingLives = new Text(remainingLifes.ToString(), @"Pixelite", Vector3.Zero)
            {
                SizeX = 2,
                Alpha = 0
            };


            TimeTaken = new Text(timeTaken.ToString(), @"Pixelite", Vector3.Zero)
            {
                SizeX = 2,
                Alpha = 0
            };


            Percentage = new Text(percentage + "%", @"Pixelite", Vector3.Zero)
            {
                SizeX = 2,
                Alpha = 0
            };


            FinalScore = new Text((remainingCash + remainingLifes + timeTaken).ToString(), @"Pixelite", Vector3.Zero)
            {
                SizeX = 2,
                Alpha = 0
            };


            RemainingCashLogo = new Image("ScoreMoney")
            {
                SizeX = 3,
                Alpha = 0,
                Origin = Vector2.Zero
            };


            TimeTakenLogo = new Image("ScoreTime")
            {
                SizeX = 3,
                Alpha = 0,
                Origin = Vector2.Zero
            };


            RemainingLivesLogo = new Image("ScoreLives")
            {
                SizeX = 3,
                Alpha = 0,
                Origin = Vector2.Zero
            };


            Elapsed = 0;
            IndividualShowLength = 750;
        }


        public Vector2 AbsoluteSize
        {
            get
            {
                return Score.AbsoluteSize + new Vector2(RemainingCashLogo.AbsoluteSize.X + FinalScore.AbsoluteSize.X + 10);
            }
        }


        public Vector3 Position
        {
            get
            {
                return RemainingCash.Position;
            }

            set
            {
                Score.Position = value;

                RemainingCashLogo.Position = value + new Vector3(Score.AbsoluteSize.X, 0, 0);
                RemainingLivesLogo.Position = RemainingCashLogo.Position;
                TimeTakenLogo.Position = RemainingCashLogo.Position;


                RemainingCash.Position = RemainingCashLogo.Position + new Vector3(RemainingCashLogo.AbsoluteSize.X + 10, 0, 0);
                RemainingLives.Position = RemainingCash.Position;
                TimeTaken.Position = RemainingCash.Position;
                Percentage.Position = RemainingCashLogo.Position;
                FinalScore.Position = RemainingCashLogo.Position;
            }
        }


        public byte Alpha
        {
            get
            {
                return Score.Alpha;
            }
            set
            {
                Score.Alpha = value;
                RemainingCash.Alpha = value;
                RemainingLives.Alpha = value;
                TimeTaken.Alpha = value;
                Percentage.Alpha = value;
                FinalScore.Alpha = value;
                RemainingCashLogo.Alpha = value;
                RemainingLivesLogo.Alpha = value;
                TimeTaken.Alpha = value;
            }
        }


        public void Update()
        {
            if (Score.Alpha <= 100)
                return;

            Elapsed += Preferences.TargetElapsedTimeMs;
        }


        public void Draw()
        {
            Scene.Add(Score);

            if (Score.Alpha <= 100)
                return;

            if (Elapsed < IndividualShowLength)
            {

                RemainingCash.Alpha = (byte) MathHelper.Clamp(255 * ((IndividualShowLength - Elapsed) / (IndividualShowLength / 2)), 0, 255);
                RemainingCashLogo.Alpha = RemainingCash.Alpha;
                Scene.Add(RemainingCash);
                Scene.Add(RemainingCashLogo);
            }

            else if (Elapsed < IndividualShowLength * 2)
            {
                RemainingLives.Alpha = (byte) MathHelper.Clamp(255 * ((IndividualShowLength * 2 - Elapsed) / (IndividualShowLength / 2)), 0, 255);
                RemainingLivesLogo.Alpha = RemainingLives.Alpha;
                Scene.Add(RemainingLives);
                Scene.Add(RemainingLivesLogo);
            }

            else if (Elapsed < IndividualShowLength * 3)
            {
                TimeTaken.Alpha = (byte) MathHelper.Clamp(255 * ((IndividualShowLength * 3 - Elapsed) / (IndividualShowLength / 2)), 0, 255);
                TimeTakenLogo.Alpha = TimeTaken.Alpha;
                Scene.Add(TimeTaken);
                Scene.Add(TimeTakenLogo);
            }


            else if (Elapsed < IndividualShowLength * 4)
            {
                Percentage.Alpha = (byte) MathHelper.Clamp(255 * ((IndividualShowLength * 4 - Elapsed) / (IndividualShowLength / 2)), 0, 255);
                Scene.Add(Percentage);
            }


            else
            {
                Scene.Add(FinalScore);
            }
        }


        public Rectangle VisiblePart
        {
            set { throw new System.NotImplementedException(); }
        }


        public Vector2 Origin
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }


        public Vector2 Size
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }


        public Color Color
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }
    }
}
