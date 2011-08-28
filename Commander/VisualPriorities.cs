namespace EphemereGames.Commander.VisualPriorities
{
    class Background
    {
    }

    class Foreground
    {
        public const double Transition = 0.5;
    }

    class Cutscenes
    {
        public const double HelpBar = 0.01;

        public const double IntroSimulatorBackground = 0.1;
        public const double IntroLogo = 0.09;
        public const double IntroDedication = 0.09;
        public const double IntroLocation = 0.09;
        public const double IntroWife = 0.08;
        public const double IntroProtagonist = 0.07;
        public const double IntroHumanBattleships = 0.07;
        public const double IntroMothership = 0.06;

        public const double IntroCommanderBackground = 0.1;
        public const double IntroCommanderText = 0.09;
    }

    class Default
    {
        //
        // Higher in the list => Higher priority
        //

        public const double Tutorial = 0.08;
        public const double GameEndedAnimation = 0.1;
        public const double EndOfWorldAnimation = 0.1;

        public const double Title = 0.2;

        public const double PlayerPanelCursor = 0.3;

        public const double PausePanel = 0.35;
        public const double CreditsPanel = 0.35;
        public const double OptionsPanel = 0.36;
        public const double HelpPanel = 0.36;
        public const double ControlsPanel = 0.36;
        public const double NewsPanel = 0.36;
        public const double DLCPanel = 0.36;

        public const double TurretMenu = 0.38;
        public const double CelestialBodyMenu = 0.39;
        public const double StartingPathMenu = 0.39;

        public const double Teleport = 0.39;

        public const double PlayerCursor = 0.4; //Back == 0.4 - 0.00001; Trail == 0.4 - 0.00002, Trail2 == 0.4 - 0.00003

        public const double DefaultSpaceship = 0.41;

        public const double PowerUpsMenu = 0.42;

        public const double GameBar = 0.42;
        public const double PlayerCash = 0.42;

        public const double EditorGeneralMenu = 0.45;
        public const double EditorPanel = 0.46;

        public const double DefaultBullet = 0.5;

        public const double TurretMessage = 0.59;

        public const double TurretUpgradedAnimation = 0.595;

        public const double Turret = 0.6;

        public const double Moon = 0.65;

        public const double EnemyNearCelestialBodyToProtect = 0.65;

        public const double PowerUpShield = 0.69;
        public const double CelestialBodyShield = 0.69;

        public const double CelestialBody = 0.7;
        public const double CelestialBodySelection = 0.71;

        public const double PlayerLives = 0.72;
        public const double NextWavePreview = 0.72;

        public const double NextWaveStarted = 0.73;

        public const double MineBullet = 0.8;
        public const double PulseBullet = 0.8;

        public const double Enemy = 0.89; // between 0.79 and 0.89
        public const double Path = 0.9; // between 0.8 and 0.9

        public const double EnemyCashAnimation = 0.92;

        public const double TurretRange = 0.94;

        public const double CelestialBodiePath = 0.945;

        public const double LevelNumber = 0.946;
        public const double LevelHighScore = 0.946;

        public const double LevelLocked = 0.947;
        public const double LevelPaused = 0.947;

        public const double HelpBar = 0.95;

        public const double MenuChoices = 0.99;
    }
}
