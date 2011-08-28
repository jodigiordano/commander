namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class TextBubble : ManualTextBubble
    {
        public double ShowTime;
        public double FadeTime;


        public TextBubble(CommanderScene scene, Text text, Vector3 position, double showTime, double visualPriority)
            : base(scene, text, position, visualPriority)
        {
            ShowTime = showTime;
            FadeTime = double.MaxValue;
        }


        public bool Finished
        {
            get { return ShowTime <= 0; }
        }


        public override void Update()
        {
            ShowTime -= Preferences.TargetElapsedTimeMs;

            if (ShowTime <= FadeTime)
            {
                FadeTime = double.NaN;
                FadeOut(ShowTime);
            }

            base.Update();
        }
    }
}
