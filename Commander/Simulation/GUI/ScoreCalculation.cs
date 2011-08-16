namespace EphemereGames.Commander.Simulation
{
    using System;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class ScoreCalculation : IVisual
    {
        private Text RemainingCash;
        private Text RemainingLifes;
        private Text TimeTaken;
        private Text FinalScore;

        private float IndividualShowLength;
        private float Elapsed;

        private Scene Scene;

        private byte RemainingCashCurrentAlpha;
        private byte RemainingLifesCurrentAlpha;
        private byte TimeTakenCurrentAlpha;
        private byte FinalScoreCurrentAlpha;


        public ScoreCalculation(Scene scene, int remainingCash, int remainingLifes, int timeTaken, double visualPriority)
        {
            Scene = scene;

            RemainingCash = new Text(remainingCash.ToString(), "Pixelite", Vector3.Zero)
            {
                SizeX = 2,
                Alpha = 0
            }.CenterIt();


            RemainingLifes = new Text(remainingLifes.ToString(), "Pixelite", Vector3.Zero)
            {
                SizeX = 2,
                Alpha = 0
            }.CenterIt();


            TimeTaken = new Text(timeTaken.ToString(), "Pixelite", Vector3.Zero)
            {
                SizeX = 2,
                Alpha = 0
            }.CenterIt();


            FinalScore = new Text((remainingCash + remainingLifes + timeTaken).ToString(), "Pixelite", Vector3.Zero)
            {
                SizeX = 2,
                Alpha = 0
            }.CenterIt();


            Elapsed = 0;
            IndividualShowLength = 500;

            RemainingCashCurrentAlpha = 255;
            RemainingLifesCurrentAlpha = 255;
            TimeTakenCurrentAlpha = 255;
            FinalScoreCurrentAlpha = 255;
        }


        public Vector2 AbsoluteSize
        {
            get
            {
                return FinalScore.AbsoluteSize;
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
                RemainingCash.Position = value;
                RemainingLifes.Position = value;
                TimeTaken.Position = value;
                FinalScore.Position = value;
            }
        }


        public byte Alpha
        {
            get
            {
                return RemainingCash.Alpha;
            }
            set
            {
                RemainingCash.Alpha = Math.Min(value, RemainingCashCurrentAlpha);
                RemainingLifes.Alpha = Math.Min(value, RemainingLifesCurrentAlpha);
                TimeTaken.Alpha = Math.Min(value, TimeTakenCurrentAlpha);
                FinalScore.Alpha = Math.Min(value, FinalScoreCurrentAlpha);
            }
        }


        public void Update()
        {
            Elapsed += Preferences.TargetElapsedTimeMs;
        }


        public void Draw()
        {
            if (Elapsed < IndividualShowLength)
            {
                RemainingCash.Alpha = RemainingCashCurrentAlpha = (byte) MathHelper.Clamp(255 * Elapsed / (IndividualShowLength / 2), 0, 255);
                Scene.Add(RemainingCash);
            }

            else if (Elapsed < IndividualShowLength * 2)
            {
                RemainingLifes.Alpha = RemainingLifesCurrentAlpha = (byte) MathHelper.Clamp(255 * Elapsed / (IndividualShowLength), 0, 255);
                Scene.Add(RemainingLifes);
            }

            else if (Elapsed < IndividualShowLength * 3)
            {
                TimeTaken.Alpha = TimeTakenCurrentAlpha = (byte) MathHelper.Clamp(255 * Elapsed / (IndividualShowLength * 3 / 2), 0, 255);
                Scene.Add(TimeTaken);
            }

            else if (Elapsed < IndividualShowLength * 4)
            {
                FinalScore.Alpha = FinalScoreCurrentAlpha = (byte) MathHelper.Clamp(255 * Elapsed / (IndividualShowLength * 2), 0, 255);
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
