namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Serialization;
    using Microsoft.Xna.Framework;


    static class LevelsFactory
    {
        public static WorldDescriptor GetWorldDescriptor(string name)
        {
            WorldDescriptor wd;

            switch (name)
            {
                case "World1":
                default:
                    wd = new WorldDescriptor()
                    {
                        Name = name,
                        Levels = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8 },
                        Warps = new List<KeyValuePair<int, string>>() { new KeyValuePair<int, string>(9, "World2") },
                        Layout = 101,
                        UnlockedCondition = new List<int>(),
                        WarpBlockedMessage = "You're not Commander\n\nenough to ascend to\n\na higher level."
                    };
                    break;

                case "World2":
                    wd = new WorldDescriptor()
                    {
                        Name = name,
                        //Levels = new List<int>() { 10, 11, 12, 13, 14, 15, 16, 17, 18 },
                        Levels = new List<int>() { 0 },
                        Warps = new List<KeyValuePair<int, string>>() { new KeyValuePair<int, string>(19, "World3"), new KeyValuePair<int, string>(20, "World1") },
                        Layout = 102,
                        UnlockedCondition = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8 },
                        WarpBlockedMessage = "Only a true Commander\n\nmay enjoy a better world."
                    };
                    break;

                case "World3":
                    wd = new WorldDescriptor()
                    {
                        Name = name,
                        //Levels = new List<int>() { 21, 22, 23, 24, 25, 26, 27, 28, 29, 30 },
                        Levels = new List<int>() { 0 },
                        Warps = new List<KeyValuePair<int, string>>() { new KeyValuePair<int, string>(31, "World2") },
                        Layout = 103,
                        UnlockedCondition = new List<int>() { -1 },
                        WarpBlockedMessage = ""
                    };
                    break;
            }


            return wd;
        }


        public static LevelDescriptor GetDescriptor(int id)
        {
            LevelDescriptor d;

            if (id == 9)
            {
                d = new LevelDescriptor();
                d.Id = 9;
                d.Mission = "World2";
                d.Difficulty = "";

                return d;
            }

            else if (id == 19)
            {
                d = new LevelDescriptor();
                d.Id = 19;
                d.Mission = "World3";
                d.Difficulty = "";

                return d;
            }

            else if (id == 20)
            {
                d = new LevelDescriptor();
                d.Id = 20;
                d.Mission = "World1";
                d.Difficulty = "";

                return d;
            }

            else if (id == 31)
            {
                d = new LevelDescriptor();
                d.Id = id;
                d.Mission = "World2";
                d.Difficulty = "";

                return d;
            }

            else if (id >= 21 && id <= 30)
            {
                d = new LevelDescriptor();
                d.Id = id;
                d.Mission = "3-" + (id - 20);
                d.Difficulty = (id == 10 || id == 17) ? "Easy" : (id == 11 || id == 12 || id == 13 || id == 14) ? "Normal" : "Hard";

                return d;
            }

            else if (id >= 100 && id <= 200)
            {
                switch (id)
                {
                    case 101: return GetWorld1Descriptor(); break;
                    case 102: return GetWorld2Descriptor(); break;
                    case 103: return GetWorld3Descriptor(); break;
                }
            }

            else
            {
                XmlSerializer serializer = new XmlSerializer(typeof(LevelDescriptor));

                using (StreamReader reader = new StreamReader(".\\Content\\scenarios\\scenario" + id + ".xml"))
                    d = (LevelDescriptor) serializer.Deserialize(reader.BaseStream);

                return d;
            }

            return null;
        }


        public static LevelDescriptor GetMenuDescriptor()
        {
            LevelDescriptor d = new LevelDescriptor();

            CelestialBodyDescriptor c;

            d.Player.Lives = 1;
            d.Player.Money = 100;

            d.Background = "fondecran16";

            d.AddCelestialBody(Size.Normal, new Vector3(-300, -150, 0), "save the\nworld", "planete2", 0, 100);
            d.PlanetarySystem[0].AddTurret(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);
            d.PlanetarySystem[0].AddTurret(TurretType.Basic, 5, new Vector3(10, -14, 0), true);

            d.CelestialBodyToProtect = d.PlanetarySystem[0].PathPriority;

            d.AddCelestialBody(Size.Big, new Vector3(300, -220, 0), "options", "planete4", 0, 99);
            d.PlanetarySystem[1].AddTurret(TurretType.Gravitational, 1, new Vector3(3, 2, 0), false);
            d.PlanetarySystem[1].AddTurret(TurretType.Basic, 8, new Vector3(-20, -5, 0), true);
            d.PlanetarySystem[1].AddTurret(TurretType.MultipleLasers, 4, new Vector3(12, 0, 0), true);

            d.AddCelestialBody(Size.Small, new Vector3(-50, 150, 0), "editor", "planete3", 0, 98);
            d.PlanetarySystem[2].AddTurret(TurretType.Gravitational, 1, new Vector3(4, 2, 0), false);
            d.PlanetarySystem[2].AddTurret(TurretType.Laser, 7, new Vector3(3, -7, 0), true);
            d.PlanetarySystem[2].AddTurret(TurretType.Missile, 3, new Vector3(-8, 0, 0), true);


            d.AddCelestialBody(Size.Small, new Vector3(-400, 200, 0), "help", "planete6", 0, 97);
            d.PlanetarySystem[3].AddTurret(TurretType.Gravitational, 1, new Vector3(2, 1, 0), false);

            d.AddCelestialBody(Size.Normal, new Vector3(350, 200, 0), "quit", "planete5", 0, 96);
            d.PlanetarySystem[4].AddTurret(TurretType.Gravitational, 1, new Vector3(-5, 3, 0), false);
            d.PlanetarySystem[4].AddTurret(TurretType.SlowMotion, 6, new Vector3(-10, -3, 0), true);

            c = new CelestialBodyDescriptor();
            c.Name = "whatever";
            c.Position = new Vector3(700, -400, 0);
            c.Speed = 320000;
            c.Size = Size.Small;
            c.Images.Add("Asteroid");
            c.Images.Add("Plutoid");
            c.Images.Add("Comet");
            c.Images.Add("Centaur");
            c.Images.Add("Trojan");
            c.Images.Add("Meteoroid");
            c.PathPriority = 1;
            c.CanSelect = false;
            c.AddTurret(TurretType.Alien, 1, Vector3.Zero, true, false, false);
            d.PlanetarySystem.Add(c);


            DescriptorInfiniteWaves v = new DescriptorInfiniteWaves()
            {
                StartingDifficulty = 12,
                DifficultyIncrement = 0,
                MineralsPerWave = 0,
                MinMaxEnemiesPerWave = new Vector2(10, 30),
                Enemies = new List<EnemyType>() { EnemyType.Asteroid, EnemyType.Comet, EnemyType.Plutoid },
                FirstOneStartNow = true,
                Upfront = true,
                NbWaves = 10
            };

            d.InfiniteWaves = v;

            return d;
        }


        public static LevelDescriptor GetPerformanceTestDescriptor()
        {
            LevelDescriptor d = new LevelDescriptor();

            CelestialBodyDescriptor c;
            WaveDescriptor v;

            d.Player.Lives = 1;
            d.Player.Money = 100;

            d.Background = "fondecran14";

            for (int i = 0; i < 6; i++)
            {
                c = new CelestialBodyDescriptor();
                c.Name = i.ToString();
                c.Position = new Vector3(150, 100, 0);
                c.StartingPosition = (int) (i * 14f);
                c.Size = Size.Small;
                c.Speed = 120000;
                c.Image = "stationSpatiale1";
                c.PathPriority = 30 - i;
                c.Invincible = true;

                c.AddTurret(TurretType.Gravitational, 1, new Vector3(-6, 0, 0));
                c.AddTurret(TurretType.Basic, 10, new Vector3(0, 6, 0));
                c.AddTurret(TurretType.Basic, 10, new Vector3(0, -6, 0));

                d.PlanetarySystem.Add(c);
            }

            d.CelestialBodyToProtect = d.PlanetarySystem[0].PathPriority;

            for (int i = 0; i < 8; i++)
            {
                c = new CelestialBodyDescriptor();
                c.Name = (i+8).ToString();
                c.Position = new Vector3(300, 200, 0);
                c.StartingPosition = (int)(i * 11f);
                c.Size = Size.Small;
                c.Speed = 120000;
                c.Image = "stationSpatiale2";
                c.PathPriority = 24 - i;
                c.Invincible = true;

                c.AddTurret(TurretType.Gravitational, 1, new Vector3(-6, 0, 0));
                c.AddTurret(TurretType.MultipleLasers, 10, new Vector3(0, 6, 0));
                c.AddTurret(TurretType.MultipleLasers, 10, new Vector3(0, -6, 0));

                d.PlanetarySystem.Add(c);
            }

            for (int i = 0; i < 16; i++)
            {
                c = new CelestialBodyDescriptor();
                c.Name = (i+16).ToString();
                c.Position = new Vector3(450, 300, 0);
                c.StartingPosition = (int)(i * 5f);
                c.Size = Size.Small;
                c.Speed = 120000;
                c.Image = "stationSpatiale1";
                c.PathPriority = 16 - i;
                c.Invincible = true;

                c.AddTurret(TurretType.Gravitational, 1, new Vector3(-6, 0, 0));
                c.AddTurret(TurretType.Basic, 10, new Vector3(0, 6, 0));
                c.AddTurret(TurretType.Basic, 10, new Vector3(0, -6, 0));

                d.PlanetarySystem.Add(c);
            }


            c = new CelestialBodyDescriptor();
            c.Name = "1111";
            c.Position = new Vector3(700, -400, 0);
            c.Speed = 320000;
            c.Size = Size.Small;
            c.Images.Add("Asteroid");
            c.Images.Add("Plutoid");
            c.Images.Add("Comet");
            c.Images.Add("Centaur");
            c.Images.Add("Trojan");
            c.Images.Add("Meteoroid");
            c.PathPriority = 0;
            c.CanSelect = true;
            c.AddTurret(TurretType.Alien, 1, Vector3.Zero, true);

            d.PlanetarySystem.Add(c);

            v = new WaveDescriptor();
            v.Enemies = new List<EnemyType>() { EnemyType.Plutoid };
            v.LivesLevel = 150;
            d.Waves.Add(v);

            v = new WaveDescriptor();
            v.Enemies = new List<EnemyType>() { EnemyType.Comet };
            v.LivesLevel = 150;
            v.Quantity = 100;
            d.Waves.Add(v);

            v = new WaveDescriptor();
            v.Enemies = new List<EnemyType>() { EnemyType.Meteoroid };
            v.LivesLevel = 150;
            v.Quantity = 100;
            d.Waves.Add(v);

            return d;
        }


        public static LevelDescriptor GetEmptyDescriptor()
        {
            var l = new LevelDescriptor();

            return l;
        }


        public static LevelDescriptor GetWorld1Descriptor()
        {
            LevelDescriptor d = new LevelDescriptor();

            CelestialBodyDescriptor c;

            d.Player.Lives = 1;
            d.Player.Money = 100;

            d.Background = "fondecran4";

            d.AddCelestialBody(Size.Small, new Vector3(-300, -200, 0), "1-1", "planete6", 0, 1);
            d.PlanetarySystem[0].AddTurret(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);
            d.PlanetarySystem[0].AddTurret(TurretType.Basic, 5, new Vector3(5, -4, 0), true);

            d.AddCelestialBody(Size.Small, new Vector3(-450, -50, 0), "1-2", "planete7", 0, 2);
            d.PlanetarySystem[1].AddTurret(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);
            d.PlanetarySystem[1].AddTurret(TurretType.Basic, 3, new Vector3(0, 8, 0), true);
            d.PlanetarySystem[1].AddTurret(TurretType.Basic, 3, new Vector3(6, 16, 0), true);
            d.PlanetarySystem[1].AddTurret(TurretType.Basic, 3, new Vector3(-6, 24, 0), true);
            
            d.AddCelestialBody(Size.Normal, new Vector3(-400, 150, 0), "1-3", "planete1", 0, 3);
            d.PlanetarySystem[2].AddTurret(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);
            
            d.AddCelestialBody(Size.Normal, new Vector3(-150, 30, 0), "1-4", "planete2", 0, 4);
            d.PlanetarySystem[3].AddTurret(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);
            d.PlanetarySystem[3].AddTurret(TurretType.Laser, 5, new Vector3(-3, 10, 0), true);
            d.PlanetarySystem[3].AddTurret(TurretType.Laser, 5, new Vector3(10, -6, 0), true);
            d.PlanetarySystem[3].AddTurret(TurretType.Laser, 5, new Vector3(0, -10, 0), true);

            d.AddCelestialBody(Size.Small, new Vector3(0, 200, 0), "1-5", "planete3", 0, 5);
            d.PlanetarySystem[4].AddTurret(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);

            d.AddCelestialBody(Size.Small, new Vector3(100, 75, 0), "1-6", "planete4", 0, 6);
            d.PlanetarySystem[5].AddTurret(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);
            d.PlanetarySystem[5].AddTurret(TurretType.MultipleLasers, 1, new Vector3(5, 5, 0), true);

            d.AddCelestialBody(Size.Big, new Vector3(400, 150, 0), "1-7", "planete5", 0, 7);
            d.PlanetarySystem[6].AddTurret(TurretType.Gravitational, 2, new Vector3(1, -2, 0), false);
            d.PlanetarySystem[6].AddTurret(TurretType.Missile, 2, new Vector3(-14, -9, 0), true);
            d.PlanetarySystem[6].AddTurret(TurretType.SlowMotion, 4, new Vector3(0, -12, 0), true);

            d.AddCelestialBody(Size.Big, new Vector3(450, -150, 0), "1-8", "planete6", 0, 8);
            d.PlanetarySystem[7].AddTurret(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);
            
            d.AddCelestialBody(Size.Normal, new Vector3(200, -200, 0), "1-9", "planete7", 0, 9);
            d.PlanetarySystem[8].AddTurret(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);
            d.PlanetarySystem[8].AddTurret(TurretType.SlowMotion, 4, new Vector3(12, 2, 0), true);

            d.AddPinkHole(new Vector3(-50, -220, 0), "World2", 0, 10);
            d.PlanetarySystem[9].AddTurret(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);

            d.CelestialBodyToProtect = d.PlanetarySystem[9].PathPriority;

            c = new CelestialBodyDescriptor();
            c.Name = "Asteroid belt";
            c.Position = new Vector3(700, -400, 0);
            c.Speed = 2560000;
            c.StartingPosition = 40;
            c.Size = Size.Small;
            c.Images.Add("Asteroid");
            c.Images.Add("Plutoid");
            c.Images.Add("Comet");
            c.Images.Add("Centaur");
            c.Images.Add("Trojan");
            c.Images.Add("Meteoroid");
            c.PathPriority = 0;
            c.AddTurret(TurretType.Alien, 1, Vector3.Zero, true, false, false);
            d.PlanetarySystem.Add(c);

            DescriptorInfiniteWaves v = new DescriptorInfiniteWaves()
            {
                StartingDifficulty = 10,
                DifficultyIncrement = 0,
                MineralsPerWave = 0,
                MinMaxEnemiesPerWave = new Vector2(10, 30),
                Enemies = new List<EnemyType>() { EnemyType.Asteroid, EnemyType.Comet, EnemyType.Plutoid },
                FirstOneStartNow = true,
                Upfront = true,
                NbWaves = 10
            };

            d.InfiniteWaves = v;

            return d;
        }


        public static LevelDescriptor GetWorld2Descriptor()
        {
            LevelDescriptor d = new LevelDescriptor();

            CelestialBodyDescriptor c;

            d.Player.Lives = 1;
            d.Player.Money = 100;

            d.Background = "fondecran5";

            d.AddPinkHole(new Vector3(0, 300, 0), "World1", 0, 1);
            d.PlanetarySystem[0].AddTurret(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);

            d.AddCelestialBody(Size.Small, new Vector3(-150, 200, 0), "2-1", "planete6", 0, 2);
            d.PlanetarySystem[1].AddTurret(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);

            d.AddCelestialBody(Size.Normal, new Vector3(250, 250, 0), "2-2", "planete7", 0, 3);
            d.PlanetarySystem[2].AddTurret(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);
            d.PlanetarySystem[2].AddTurret(TurretType.Basic, 6, new Vector3(-10, -5, 0), true);
            d.PlanetarySystem[2].AddTurret(TurretType.Basic, 7, new Vector3(20, 6, 0), true);


            d.AddCelestialBody(Size.Normal, new Vector3(0, 100, 0), "2-3", "planete1", 0, 4);
            d.PlanetarySystem[3].AddTurret(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);

            d.AddCelestialBody(Size.Normal, new Vector3(-300, 175, 0), "2-4", "planete2", 0, 5);
            d.PlanetarySystem[4].AddTurret(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);
            d.PlanetarySystem[4].AddTurret(TurretType.Missile, 5, new Vector3(-18, -20, 0), true);
            d.PlanetarySystem[4].AddTurret(TurretType.MultipleLasers, 6, new Vector3(-18, -8, 0), true);
            d.PlanetarySystem[4].AddTurret(TurretType.SlowMotion, 4, new Vector3(17, -15, 0), true);

            d.AddCelestialBody(Size.Normal, new Vector3(0, -75, 0), "2-5", "planete3", 0, 6);
            d.PlanetarySystem[5].AddTurret(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);

            d.AddCelestialBody(Size.Big, new Vector3(250, 50, 0), "2-6", "planete4", 0, 7);
            d.PlanetarySystem[6].AddTurret(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);
            d.PlanetarySystem[6].AddTurret(TurretType.Missile, 8, new Vector3(-6, -25, 0), true);

            d.AddCelestialBody(Size.Big, new Vector3(400, -150, 0), "2-7", "planete5", 0, 8);
            d.PlanetarySystem[7].AddTurret(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);
            d.PlanetarySystem[7].AddTurret(TurretType.Laser, 5, new Vector3(24, 0, 0), true);
            d.PlanetarySystem[7].AddTurret(TurretType.Laser, 6, new Vector3(20, 8, 0), true);
            d.PlanetarySystem[7].AddTurret(TurretType.Laser, 7, new Vector3(18, 16, 0), true);
            d.PlanetarySystem[7].AddTurret(TurretType.Laser, 8, new Vector3(10, 24, 0), true);
            d.PlanetarySystem[7].AddTurret(TurretType.Laser, 9, new Vector3(0, 32, 0), true);
            d.PlanetarySystem[7].AddTurret(TurretType.Laser, 10, new Vector3(0, 40, 0), true);

            d.AddCelestialBody(Size.Small, new Vector3(100, -220, 0), "2-8", "planete6", 0, 9);
            d.PlanetarySystem[8].AddTurret(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);

            d.AddCelestialBody(Size.Big, new Vector3(-300, -100, 0), "2-9", "planete7", 0, 10);
            d.PlanetarySystem[9].AddTurret(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);

            d.AddPinkHole(new Vector3(-125, -200, 0), "World3", 0, 11);
            d.PlanetarySystem[10].AddTurret(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);

            d.CelestialBodyToProtect = d.PlanetarySystem[10].PathPriority;

            c = new CelestialBodyDescriptor();
            c.Name = "Asteroid belt";
            c.Position = new Vector3(700, -400, 0);
            c.Speed = 2560000;
            c.StartingPosition = 75;
            c.Size = Size.Small;
            c.Images.Add("Plutoid");
            c.PathPriority = 0;
            c.AddTurret(TurretType.Alien, 1, Vector3.Zero, true, false, false);
            d.PlanetarySystem.Add(c);

            DescriptorInfiniteWaves v = new DescriptorInfiniteWaves();
            v.StartingDifficulty = 40;
            v.DifficultyIncrement = 0;
            v.MineralsPerWave = 0;
            v.MinMaxEnemiesPerWave = new Vector2(10, 30);
            v.Enemies = new List<EnemyType>();
            v.Enemies.Add(EnemyType.Asteroid);
            v.Enemies.Add(EnemyType.Comet);
            v.Enemies.Add(EnemyType.Plutoid);
            v.FirstOneStartNow = true;
            d.InfiniteWaves = v;

            return d;
        }


        public static LevelDescriptor GetWorld3Descriptor()
        {
            LevelDescriptor d = new LevelDescriptor();

            CelestialBodyDescriptor c;

            d.Player.Lives = 1;
            d.Player.Money = 100;

            d.Background = "fondecran6";

            d.AddPinkHole(new Vector3(500, 0, 0), "World2", 0, 1);
            d.PlanetarySystem[0].AddTurret(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);

            d.AddCelestialBody(Size.Big, new Vector3(450, -200, 0), "3-1", "stationSpatiale1", 0, 2);
            d.PlanetarySystem[1].AddTurret(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);
            d.PlanetarySystem[1].AddTurret(TurretType.MultipleLasers, 10, new Vector3(-20, -15, 0), true);

            d.AddCelestialBody(Size.Big, new Vector3(350, 200, 0), "3-2", "stationSpatiale1", 0, 3);
            d.PlanetarySystem[2].AddTurret(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);

            d.AddCelestialBody(Size.Big, new Vector3(250, 0, 0), "3-3", "stationSpatiale2", 0, 4);
            d.PlanetarySystem[3].AddTurret(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);

            d.AddCelestialBody(Size.Normal, new Vector3(150, -200, 0), "3-4", "stationSpatiale1", 0, 5);
            d.PlanetarySystem[4].AddTurret(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);
            d.PlanetarySystem[4].AddTurret(TurretType.Basic, 10, new Vector3(-17, -13, 0), true);
            d.PlanetarySystem[4].AddTurret(TurretType.Basic, 10, new Vector3(-19, 0, 0), true);
            d.PlanetarySystem[4].AddTurret(TurretType.Basic, 10, new Vector3(-3, 15, 0), true);

            d.AddCelestialBody(Size.Normal, new Vector3(50, 200, 0), "3-5", "stationSpatiale2", 0, 6);
            d.PlanetarySystem[5].AddTurret(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);

            d.AddCelestialBody(Size.Big, new Vector3(-50, 0, 0), "3-6", "stationSpatiale2", 0, 7);
            d.PlanetarySystem[6].AddTurret(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);

            d.AddCelestialBody(Size.Normal, new Vector3(-150, -200, 0), "3-7", "stationSpatiale1", 0, 8);
            d.PlanetarySystem[7].AddTurret(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);

            d.AddCelestialBody(Size.Big, new Vector3(-200, 225, 0), "3-8", "stationSpatiale1", 0, 9);
            d.PlanetarySystem[8].AddTurret(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);
            d.PlanetarySystem[8].AddTurret(TurretType.Missile, 10, new Vector3(-25, 0, 0), true);
            d.PlanetarySystem[8].AddTurret(TurretType.Missile, 10, new Vector3(-18, -8, 0), true);

            d.AddCelestialBody(Size.Big, new Vector3(-300, 30, 0), "3-9", "stationSpatiale2", 0, 10);
            d.PlanetarySystem[9].AddTurret(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);
            d.PlanetarySystem[9].AddTurret(TurretType.MultipleLasers, 10, new Vector3(-20, 0, 0), true);

            d.AddCelestialBody(Size.Big, new Vector3(-400, -160, 0), "3-10", "planete1", 0, 11);
            d.PlanetarySystem[10].AddTurret(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);


            d.CelestialBodyToProtect = d.PlanetarySystem[10].PathPriority;

            c = new CelestialBodyDescriptor();
            c.Name = "Asteroid belt";
            c.Position = new Vector3(700, -400, 0);
            c.Speed = 2560000;
            c.StartingPosition = 0;
            c.Size = Size.Small;
            c.Images.Add("Asteroid");
            c.PathPriority = 0;
            c.CanSelect = false;
            c.AddTurret(TurretType.Alien, 1, Vector3.Zero, true, false, false);
            d.PlanetarySystem.Add(c);

            DescriptorInfiniteWaves v = new DescriptorInfiniteWaves();
            v.StartingDifficulty = 70;
            v.DifficultyIncrement = 0;
            v.MineralsPerWave = 0;
            v.MinMaxEnemiesPerWave = new Vector2(10, 30);
            v.Enemies = new List<EnemyType>();
            v.Enemies.Add(EnemyType.Asteroid);
            v.Enemies.Add(EnemyType.Comet);
            v.Enemies.Add(EnemyType.Plutoid);
            v.FirstOneStartNow = true;
            d.InfiniteWaves = v;

            return d;
        }
    }
}
