namespace EphemereGames.Commander
{
    public enum Direction
    {
        None = -1,
        Left = 0,
        Right = 1,
        Up = 2,
        Down = 3
    };


    public enum Distance
    {
        Joined = 0,
        Near = 10,
        Normal = 60,
        Far = 110
    };


    public enum Size
    {
        Small = 28,
        Normal = 50,
        Big = 68
    };


    enum MusicContext
    {
        Won = 1,
        Lost = 2,
        Other = 3,
        Cutscene = 4
    };


    public enum GameState
    {
        Running = 1,
        Paused = 2,
        Won = 3,
        Lost = 4,
        Restart = 5,
        PausedToWorld = 6
    };


    public enum PanelType
    {
        None = -1,
        Pause = 0,
        DLC = 1,
        Options = 2,
        Credits = 3,
        Help = 4,
        Controls = 5,
        GeneralNews = 6,
        UpdatesNews = 7,
        Login = 8,
        Register = 9,
        VirtualKeyboard = 10,
        JumpToWorld = 11
    };


    public enum MultiverseMessageType
    {
        None = -1,
        Ok = 0,
        Error = 1,
        LoadWorld = 2,
        HighScores = 3,
        NewPlayer = 4,
        Login = 5
    }


    public enum NewsType
    {
        None = -1,
        General = 0,
        Updates = 1,
        DLC = 2
    };


    public enum HelpBarMessageType
    {
        Select = 1,
        Cancel = 2,
        Toggle = 3,
        QuickToggle = 4,
        Move = 5,
        Retry = 6,
        Fire = 7
    };
}


namespace EphemereGames.Commander.Simulation
{
    enum BulletType
    {
        None = -1,
        Base = 0,
        Missile = 1,
        Missile2 = 2,
        Mine = 3,
        LaserMultiple = 4,
        LaserSimple = 5,
        SlowMotion = 6,
        Gunner = 7,
        Nanobots = 8,
        RailGun = 9,
        Shield = 10,
        Pulse = 11
    };


    public enum EnemyType
    {
        Asteroid = 1,
        Comet = 2,
        Plutoid = 3,
        Centaur = 4,
        Trojan = 5,
        Meteoroid = 6,
        Damacloid = 7,
        Vulcanoid = 8,
        Swarm = 9,
        Alien1 = 10,
        Alien2 = 11,
        Alien3 = 12,
        Alien4 = 13
    };


    public enum TurretType
    {
        Basic = 1,
        Missile = 2,
        Gravitational = 3,
        MultipleLasers = 4,
        Laser = 5,
        None = 6,
        SlowMotion = 8,
        Booster = 9,
        Gunner = 10,
        Nanobots = 11,
        RailGun = 12
    };


    public enum PowerUpType
    {
        None = -1,
        Collector = 0,
        FinalSolution = 1,
        Spaceship = 2,
        DeadlyShootingStars = 3,
        Pulse = 4,
        Miner = 5,
        Shield = 6,
        AutomaticCollector = 7,
        DarkSide = 8,
        RailGun = 9
    };


    enum PowerUpCategory
    {
        Spaceship = 1,
        Turret = 2,
        Other = 3
    };


    enum MineralType
    {
        Cash10 = 1,
        Cash25 = 2,
        Cash150 = 3,
        Life1 = 4
    };


    enum TurretChoice
    {
        None = -1,
        Sell = 0,
        Update = 1
    };


    public enum PausedGameChoice
    {
        None = -1,
        Resume = 0,
        New = 1
    };


    public enum EditorWorldChoice
    {
        None = -1,
        Edit = 0,
        Playtest = 1,
        Save = 2,
        Reset = 3
    };


    enum EditorGeneralMenuChoice
    {
        None = -1,
        Gameplay = 0,
        Battlefield = 1
    };


    enum EditorPanel
    {
        None = -1,
        Player = 0,
        PowerUps = 1,
        Turrets = 2,
        Background = 3,
        Waves = 4,
        CelestialBodyAssets = 5,
        Enemies = 6
    };


    enum EditorState
    {
        Editing = 1,
        Playtest = 2
    };


    enum EditorCommandType
    {
        Basic = 1,
        Panel = 2,
        CelestialBody = 3,
        Player = 4
    };


    enum EditorEditingState
    {
        None = -1,
        MovingCB = 0,
        RotatingCB = 1,
        ShrinkingCB = 2,
        StartPosCB = 3
    };


    enum HelpBarMessage
    {
        None = -1,
        Select = 0,
        Cancel = 1,
        ToggleChoices = 2,
        WorldToggleNewGame = 3,
        CelestialBodyMenu = 4,
        TurretMenu = 5,
        InstallTurret = 6,
        PowerUpMenu = 7,
        CallNextWave = 8,
        MoveYourSpaceship = 9,
        GameLost = 10,
        GameWon = 11,
        HoldToSkip = 12,
        WorldToggleResume = 13,
        WorldNewGame = 14,
        BuyTurret = 15,
        ToggleChoicesSelect = 16,
        StartNewCampaign = 17,
        GameBar = 18,
        WorldWarp = 19,
        QuickToggle = 20
    };
}

