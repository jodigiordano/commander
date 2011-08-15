namespace EphemereGames.Commander.Simulation
{
    using Microsoft.Xna.Framework;


    class CommonStash
    {
        public int Score;
        public int Cash;
        public int Lives;
        public int TotalScore;
        public int TimeLeft;

        public Vector3 StartingPosition;


        public CommonStash()
        {
            Score = 0;
            Cash = 0;
            Lives = 0;
            TotalScore = 0;
            TimeLeft = 0;

            StartingPosition = Vector3.Zero;
        }
    }
}
