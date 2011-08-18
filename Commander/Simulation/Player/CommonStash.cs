namespace EphemereGames.Commander.Simulation
{
    using Microsoft.Xna.Framework;


    class CommonStash
    {
        public int Score;
        public int Cash;
        public int Lives;
        public int Time;

        // for score
        public int TotalScore;
        public int TotalTime;
        public int TotalLives;
        public int TotalCash;
        public int PotentialScore;

        public Vector3 StartingPosition;


        public CommonStash()
        {
            Score = 0;
            Cash = 0;
            Lives = 0;
            TotalScore = 0;
            TotalTime = 0;

            StartingPosition = Vector3.Zero;
        }
    }
}
