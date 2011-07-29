﻿namespace EphemereGames.Commander
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
        Running = 1,
        Paused = 2,
        Won = 3,
        Lost = 4
    }


    enum MusicContext
    {
        Won = 1,
        Lost = 2,
        Other = 3
    }
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
        Swarm = 9
    };


    public enum TurretType
    {
        Basic = 1,
        Missile = 2,
        Gravitational = 3,
        MultipleLasers = 4,
        Laser = 5,
        None = 6,
        Alien = 7,
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
        TheResistance = 3,
        DeadlyShootingStars = 4,
        Pulse = 5,
        Miner = 6,
        Shield = 7,
        AutomaticCollector = 8,
        DarkSide = 9,
        RailGun = 10
    };


    enum PowerUpCategory
    {
        Spaceship = 1,
        Turret = 2,
        Other = 3
    }


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
    }


    public enum PausedGameChoice
    {
        None = -1,
        Resume = 0,
        New = 1
    }


    public enum NewGameChoice
    {
        None = -1,
        Continue = 0,
        WrapToWorld1 = 1,
        WrapToWorld2 = 2,
        WrapToWorld3 = 3,
        WrapToWorld4 = 4,
        WrapToWorld5 = 5,
        WrapToWorld6 = 6,
        WrapToWorld7 = 7,
        WrapToWorld8 = 8,
        NewGame = 9,
    }


    enum EditorGeneralMenuChoice
    {
        None = -1,
        File = 0,
        Waves = 1,
        Battlefield = 2,
        Gameplay = 3
    }


    enum EditorPanel
    {
        None = -1,
        Player = 0,
        Load = 1,
        Save = 2,
        Delete = 3,
        PowerUps = 4,
        Turrets = 5,
        General = 6,
        Background = 7,
        GeneratePlanetarySystem = 8,
        Waves = 9
    }


    enum EditorState
    {
        Editing = 1,
        Playtest = 2
    }


    enum EditorCommandType
    {
        Basic = 1,
        Panel = 2,
        CelestialBody = 3,
        Player = 4
    }


    enum EditorEditingState
    {
        None = -1,
        MovingCB = 0,
        RotatingCB = 1,
        ShrinkingCB = 2,
        StartPosCB = 3
    }


    enum HelpBarMessage
    {
        None = -1,
        Select = 0,
        Cancel = 1,
        ToggleChoices = 2,
        WorldMenu = 3,
        CelestialBodyMenu = 4,
        TurretMenu = 5,
        InstallTurret = 6,
        PowerUpMenu = 7,
        CallNextWave = 8,
        MoveYourSpaceship = 9,
        GameLost = 10,
        GameWon = 11,
        HoldToSkip = 12
    }


    enum PanelType
    {
        None = -1,
        Options = 0,
        Help = 1,
        Pause = 2
    }
}
