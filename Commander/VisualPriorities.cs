namespace EphemereGames.Commander.VisualPriorities
{
    class Background
    {
    }

    class Foreground
    {
        
    }

    class Default
    {
        //
        // Higher in the list => Higher priority
        //

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


        public const double Tutorial = 0.08;
        public const double GameEndedAnimation = 0.1;

        public const double Title = 0.2;

        public const double PlayerPanelCursor = 0.3;

        public const double PausePanel = 0.35;
        public const double OptionsPanel = 0.36;

        public const double TurretMenu = 0.38;
        public const double CelestialBodyMenu = 0.39;

        public const double PlayerCursor = 0.4; //Back == 0.4 - 0.00001; Trail == 0.4 - 0.00002, Trail2 == 0.4 - 0.00003

        public const double DefaultSpaceship = 0.41;

        public const double EditorGeneralMenu = 0.45;
        public const double EditorPanel = 0.46;

        public const double DefaultBullet = 0.5;

        public const double TurretMessage = 0.59;

        public const double Turret = 0.6;

        public const double Moon = 0.65;

        public const double EnemyNearCelestialBodyToProtect = 0.65;

        public const double PowerUpShield = 0.69;

        public const double CelestialBody = 0.7;
        public const double CelestialBodySelection = 0.71;

        public const double MineBullet = 0.8;
        public const double PulseBullet = 0.8;

        public const double Enemy = 0.89; // between 0.79 and 0.89
        public const double Path = 0.9; // between 0.8 and 0.9

        public const double HelpBar = 0.95;

        public const double MenuChoices = 0.99;
    }
}
