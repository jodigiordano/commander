namespace EphemereGames.Commander.VisualPriorities
{
    class Background
    {
    }

    class Foreground
    {
        public const double HelpBar = 0.5;
    }

    class Default
    {
        public const double PlayerCursor = 0.4; //Back == 0.4 - 0.00001; Trail == 0.4 - 0.00002, Trail2 == 0.4 = 0.00003

        public const double GameEndedAnimation = 0.1;
        public const double Tutorial = 0.08;
        public const double Path = 0.9; // between 0.8 and 0.9
        public const double Enemy = 0.89; // between 0.79 and 0.89

        // Bullets
        public const double MineBullet = 0.8;
        public const double PulseBullet = 0.8;


        // Cutscenes

        public const double IntroSimulatorBackground = 0.000010;
        public const double IntroLogo = 0.000009;
        public const double IntroDedication = 0.000009;
        public const double IntroLocation = 0.000009;
        public const double IntroWife = 0.00008;
        public const double IntroProtagonist = 0.00007;
        public const double IntroMothership = 0.00006;

        public const double IntroCommanderBackground = 0.000010;
        public const double IntroCommanderText = 0.000009;
    }
}
