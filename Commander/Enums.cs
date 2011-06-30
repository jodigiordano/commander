namespace EphemereGames.Commander
{
    public enum Distance
    {
        Joined = 0,
        Near = 10,
        Normal = 60,
        Far = 110
    }


    public enum Size
    {
        Small = 28,
        Normal = 50,
        Big = 68
    }


    enum GameState
    {
        Running,
        Paused,
        Won,
        Lost
    }
}


namespace EphemereGames.Commander.Simulation
{
    enum BulletType
    {
        Base,
        Missile,
        Missile2,
        LaserMultiple,
        LaserSimple,
        Aucun,
        SlowMotion,
        Gunner,
        Nanobots,
        RailGun,
        Shield,
        Pulse,
        Mine
    };


    public enum EnemyType
    {
        Asteroid,
        Comet,
        Plutoid,
        Centaur,
        Trojan,
        Meteoroid,
        Apohele,
        Damacloid,
        Vulcanoid,
        Swarm
    };


    public enum TurretType
    {
        Basic,
        Missile,
        Gravitational,
        MultipleLasers,
        Laser,
        None,
        Alien,
        SlowMotion,
        Booster,
        Gunner,
        Nanobots,
        RailGun,
        Sniper
    };


    public enum PowerUpType
    {
        None = -1,
        Collector = 0,
        FinalSolution = 1,
        Spaceship = 2,
        TheResistance = 3,
        DeadlyShootingStars = 4,
        Pulse = 5,
        Miner = 6,
        Shield = 7,
        Sniper = 8,
        AutomaticCollector = 9,
        DarkSide = 10,
        RailGun = 11
    };


    enum PowerUpCategory
    {
        Spaceship,
        Turret,
        Other
    }


    enum MineralType
    {
        Cash10,
        Cash25,
        Cash150,
        Life1
    };


    enum TurretChoice
    {
        None = -1,
        Sell = 0,
        Update = 1
    }


    public enum PausedGameChoice
    {
        None = -1,
        Resume = 0,
        New = 1
    }


    enum EditorGeneralMenuChoice
    {
        None,
        File,
        Waves,
        Battlefield,
        Gameplay
    }


    enum EditorPanel
    {
        Player,
        Load,
        Save,
        Delete,
        PowerUps,
        Turrets,
        General,
        Background,
        GeneratePlanetarySystem,
        Waves,
        None
    }


    enum EditorState
    {
        Editing,
        Playtest
    }
}
