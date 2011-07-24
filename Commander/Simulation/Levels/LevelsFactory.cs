namespace EphemereGames.Commander.Simulation
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Serialization;
    using Microsoft.Xna.Framework;


    class LevelsFactory
    {
        public Dictionary<int, LevelDescriptor> Descriptors;
        public Dictionary<int, LevelDescriptor> CutsceneDescriptors;

        public LevelDescriptor Menu;
        public Dictionary<int, WorldDescriptor> WorldsDescriptors;

        private XmlSerializer LevelSerializer;
        private XmlSerializer WorldSerializer;


        public LevelsFactory()
        {
            Descriptors = new Dictionary<int, LevelDescriptor>();
            CutsceneDescriptors = new Dictionary<int, LevelDescriptor>();
            WorldsDescriptors = new Dictionary<int, WorldDescriptor>();

            LevelSerializer = new XmlSerializer(typeof(LevelDescriptor));
            WorldSerializer = new XmlSerializer(typeof(WorldDescriptor));
        }


        public void Initialize()
        {
            Descriptors.Clear();

            LoadLevels("level");
            LoadLevels("worldlayout");
            LoadWorlds();
            LoadCutscenes();
            LoadMenuDescriptor();

            PrepareWorldLayout(WorldsDescriptors[1].Layout); //todo: do others
        }


        public LevelDescriptor GetLevelDescriptor(int id)
        {
            LevelDescriptor d = null;

            if (id >= 2000 && id <= 3000)
            {
                d = new LevelDescriptor();
                d.Infos.Id = id;
                d.Infos.Difficulty = "";

                if (id == 2001)
                    d.Infos.Mission = "World2";
                else if (id == 2002)
                    d.Infos.Mission = "World3";
                else if (id == 2003)
                    d.Infos.Mission = "World1";
                else if (id == 2004)
                    d.Infos.Mission = "World2";
            }

            else
            {
                d = Descriptors[id];
            }

            return d;
        }


        private void LoadLevels(string startingWith)
        {
            string[] levelsFiles = Directory.GetFiles(".\\Content\\scenarios", startingWith + "*.xml");

            foreach (var f in levelsFiles)
            {
                var descriptor = LoadLevelDescriptor(f);
                Descriptors.Add(descriptor.Infos.Id, descriptor);
            }
        }


        private void LoadWorlds()
        {
            WorldsDescriptors.Clear();

            WorldDescriptor wd;

            wd = new WorldDescriptor()
            {
                Id = 1,
                Name = "The colonies",
                Levels = new List<KeyValuePair<int, List<int>>>()
                {
                    new KeyValuePair<int, List<int>>(1, new List<int>() {}),
                    new KeyValuePair<int, List<int>>(2, new List<int>() { 1 }),
                    new KeyValuePair<int, List<int>>(3, new List<int>() { 2 }),
                    new KeyValuePair<int, List<int>>(4, new List<int>() { 3 }),
                    new KeyValuePair<int, List<int>>(5, new List<int>() { 4 }),
                    new KeyValuePair<int, List<int>>(6, new List<int>() { 5 }),
                    new KeyValuePair<int, List<int>>(7, new List<int>() { 6 }),
                    new KeyValuePair<int, List<int>>(8, new List<int>() { 7 }),
                    new KeyValuePair<int, List<int>>(9, new List<int>() { 8 }),
                    new KeyValuePair<int, List<int>>(10, new List<int>() { 9 })
                },
                Warps = new List<KeyValuePair<int, string>>() { new KeyValuePair<int, string>(2001, "World2") },
                Layout = 1001,
                UnlockedCondition = new List<int>(),
                WarpBlockedMessage = "You're not Commander\n\nenough to ascend to\n\na higher level."
            };
            WorldsDescriptors.Add(wd.Id, wd);

            wd = new WorldDescriptor()
            {
                Id = 2,
                Name = "Battle for Earth",
                Levels = new List<KeyValuePair<int, List<int>>>()
                {
                    new KeyValuePair<int, List<int>>(1, new List<int>() {})
                },
                //Levels = new List<int>() { 10, 11, 12, 13, 14, 15, 16, 17, 18 },
                Warps = new List<KeyValuePair<int, string>>() { new KeyValuePair<int, string>(2002, "World3"), new KeyValuePair<int, string>(2003, "World1") },
                Layout = 1001,
                UnlockedCondition = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8 },
                WarpBlockedMessage = "Only a true Commander\n\nmay enjoy a better world."
            };
            WorldsDescriptors.Add(wd.Id, wd);

            wd = new WorldDescriptor()
            {
                Id = 3,
                Name = "The invasion",
                //Levels = new List<int>() { 21, 22, 23, 24, 25, 26, 27, 28, 29, 30 },
                Levels = new List<KeyValuePair<int, List<int>>>()
                {
                    new KeyValuePair<int, List<int>>(1, new List<int>() {})
                },
                Warps = new List<KeyValuePair<int, string>>() { new KeyValuePair<int, string>(2004, "World2") },
                Layout = 1001,
                UnlockedCondition = new List<int>() { -1 },
                WarpBlockedMessage = ""
            };
            WorldsDescriptors.Add(wd.Id, wd);
        }


        private void LoadCutscenes()
        {
            CutsceneDescriptors.Clear();

            string[] levelsFiles = Directory.GetFiles(".\\Content\\scenarios", "cutscene*.xml");

            foreach (var f in levelsFiles)
            {
                var descriptor = LoadLevelDescriptor(f);
                CutsceneDescriptors.Add(descriptor.Infos.Id, descriptor);
            }
        }


        private LevelDescriptor LoadLevelDescriptor(string path)
        {
            using (StreamReader reader = new StreamReader(path))
                return (LevelDescriptor) LevelSerializer.Deserialize(reader.BaseStream);
        }


        public void SaveLevelDescriptorOnDisk(int id)
        {
            using(StreamWriter writer = new StreamWriter(".\\Content\\scenarios\\level" + id + ".xml"))
                LevelSerializer.Serialize(writer.BaseStream, Descriptors[id]);
        }


        public void DeleteDescriptorFromDisk(int id)
        {
            if (File.Exists(".\\Content\\scenarios\\level" + id + ".xml"))
                File.Delete(".\\Content\\scenarios\\level" + id + ".xml");
        }


        public int GetWorldFromLevelId(int id)
        {
            foreach (var w in WorldsDescriptors.Values)
                foreach (var kvp in w.Levels)
                    if (kvp.Key == id)
                        return w.Id;

            return 1;
        }


        public int GetNextLevel(int worldId, int currentLevelId)
        {
            var otherLevels = WorldsDescriptors[worldId].Levels;

            otherLevels.Sort(delegate(KeyValuePair<int, List<int>> level1, KeyValuePair<int, List<int>> level2)
            {
                return level1.Key > level2.Key ? 1 : level1.Key < level2.Key ? -1 : 0;
            });

            foreach (var other in otherLevels)
                if (other.Key > currentLevelId)
                    return other.Key;

            return -1;
        }


        private void LoadMenuDescriptor()
        {
            Menu = LoadLevelDescriptor(".\\Content\\scenarios\\menu.xml");

            var newGame = Menu.PlanetarySystem[6];

            newGame.Name = "save the world";
            newGame.AddTurret(TurretType.Basic, 5, new Vector3(10, -14, 0), true);
            newGame.Invincible = true;

            var options = Menu.PlanetarySystem[1];
            options.Name = "options";
            options.AddTurret(TurretType.Basic, 2, new Vector3(-20, -5, 0), true);
            options.AddTurret(TurretType.MultipleLasers, 4, new Vector3(12, 0, 0), true);

            var editor = Menu.PlanetarySystem[2];
            editor.Name = "editor";
            editor.AddTurret(TurretType.Laser, 7, new Vector3(3, -7, 0), true);
            editor.AddTurret(TurretType.Missile, 3, new Vector3(-8, 0, 0), true);

            var quit = Menu.PlanetarySystem[3];
            quit.Name = "quit";

            var help = Menu.PlanetarySystem[4];
            help.Name = "how to play";
            help.AddTurret(TurretType.SlowMotion, 6, new Vector3(-10, -3, 0), true);

            var credits = Menu.PlanetarySystem[5];
            credits.Name = "credits";
            credits.AddTurret(TurretType.SlowMotion, 6, new Vector3(-10, -3, 0), true);


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

            var asteroidBelt = LevelDescriptor.GetAsteroidBelt(Menu.PlanetarySystem);

            asteroidBelt.Images = new List<string>() { "Asteroid", "Plutoid", "Comet", "Centaur", "Trojan", "Meteoroid" };


            Menu.InfiniteWaves = v;
        }


        public static LevelDescriptor GetPerformanceTestDescriptor()
        {
            LevelDescriptor d = new LevelDescriptor();

            CelestialBodyDescriptor c;
            WaveDescriptor v;

            d.Player.Lives = 1;
            d.Player.Money = 100;

            d.Infos.Background = "fondecran14";

            for (int i = 0; i < 6; i++)
            {
                c = new CelestialBodyDescriptor();
                c.Name = i.ToString();
                c.Path = new Vector3(150, 100, 0);
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

            d.Objective.CelestialBodyToProtect = d.PlanetarySystem[0].PathPriority;

            for (int i = 0; i < 8; i++)
            {
                c = new CelestialBodyDescriptor();
                c.Name = (i+8).ToString();
                c.Path = new Vector3(300, 200, 0);
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
                c.Path = new Vector3(450, 300, 0);
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
            c.Path = new Vector3(700, -400, 0);
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


        public LevelDescriptor GetEmptyDescriptor()
        {
            var l = new LevelDescriptor();

            l.AddAsteroidBelt();

            l.Infos.Id = GetHighestId() + 1;
            l.Infos.Mission = "1-" + (GetHighestWorld1Level() + 1).ToString();

            return l;
        }


        private int GetHighestId()
        {
            int highest = -1;

            foreach (var d in Descriptors.Values)
            {
                if (d.Infos.Id > highest)
                    highest = d.Infos.Id;
            }

            return highest;
        }


        private int GetHighestWorld1Level()
        {
            int highest = -1;

            foreach (var d in Descriptors.Values)
            {
                string[] worldLevel = d.Infos.Mission.Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
                int level = int.Parse(worldLevel[1]);

                if (worldLevel[0] == "1" && level > highest)
                    highest = level;
            }

            return highest;
        }


        private void PrepareWorldLayout(int id)
        {
            LevelDescriptor d = Descriptors[id];

            // set lives so the planet won't explode...!
            d.Player.Lives = 1;
            d.Player.Money = 100;

            // generate the asteroid belt's enemies
            var asteroidBelt = LevelDescriptor.GetAsteroidBelt(d.PlanetarySystem);

            var enemies = EnemiesFactory.ToEnemyTypeList(asteroidBelt.Images);

            DescriptorInfiniteWaves v = new DescriptorInfiniteWaves()
            {
                StartingDifficulty = 10,
                DifficultyIncrement = 0,
                MineralsPerWave = 0,
                MinMaxEnemiesPerWave = new Vector2(10, 30),
                Enemies = enemies,
                FirstOneStartNow = true,
                Upfront = true,
                NbWaves = 10
            };

            d.InfiniteWaves = v;

            var pinkHolesToAdd = new List<CelestialBodyDescriptor>();

            // switch some planets for pink holes
            for (int i = d.PlanetarySystem.Count - 1; i > -1; i--)
            {
                var cb = d.PlanetarySystem[i];

                if (!cb.Name.StartsWith("World"))
                    continue;

                var pinkHole = d.CreatePinkHole(cb.Position, cb.Name, (int) cb.Speed, cb.PathPriority);
                pinkHole.AddTurret(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);

                pinkHolesToAdd.Add(pinkHole);

                d.PlanetarySystem.RemoveAt(i);
            }

            foreach (var pink in pinkHolesToAdd)
                d.PlanetarySystem.Add(pink);

        }
        //private static LevelDescriptor GetWorld1Descriptor()
        //{

        //    d.AddPinkHole(new Vector3(-50, -220, 0), "World2", 0, 10);
        //    d.PlanetarySystem[9].AddTurret(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);

        //    d.Objective.CelestialBodyToProtect = d.PlanetarySystem[9].PathPriority;

        //    d.AddAsteroidBelt();

        //    DescriptorInfiniteWaves v = new DescriptorInfiniteWaves()
        //    {
        //        StartingDifficulty = 10,
        //        DifficultyIncrement = 0,
        //        MineralsPerWave = 0,
        //        MinMaxEnemiesPerWave = new Vector2(10, 30),
        //        Enemies = new List<EnemyType>() { EnemyType.Asteroid, EnemyType.Comet, EnemyType.Plutoid },
        //        FirstOneStartNow = true,
        //        Upfront = true,
        //        NbWaves = 10
        //    };

        //    d.InfiniteWaves = v;

        //    return d;
        //}


        //private static LevelDescriptor GetWorld2Descriptor()
        //{
        //    LevelDescriptor d = new LevelDescriptor();

        //    CelestialBodyDescriptor c;

        //    d.Player.Lives = 1;
        //    d.Player.Money = 100;

        //    d.Infos.Background = "fondecran5";

        //    d.AddPinkHole(new Vector3(0, 300, 0), "World1", 0, 1);
        //    d.PlanetarySystem[0].AddTurret(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);

        //    d.AddCelestialBody(Size.Small, new Vector3(-150, 200, 0), "2-1", "planete6", 0, 2);
        //    d.PlanetarySystem[1].AddTurret(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);

        //    d.AddCelestialBody(Size.Normal, new Vector3(250, 250, 0), "2-2", "planete7", 0, 3);
        //    d.PlanetarySystem[2].AddTurret(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);
        //    d.PlanetarySystem[2].AddTurret(TurretType.Basic, 6, new Vector3(-10, -5, 0), true);
        //    d.PlanetarySystem[2].AddTurret(TurretType.Basic, 7, new Vector3(20, 6, 0), true);


        //    d.AddCelestialBody(Size.Normal, new Vector3(0, 100, 0), "2-3", "planete1", 0, 4);
        //    d.PlanetarySystem[3].AddTurret(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);

        //    d.AddCelestialBody(Size.Normal, new Vector3(-300, 175, 0), "2-4", "planete2", 0, 5);
        //    d.PlanetarySystem[4].AddTurret(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);
        //    d.PlanetarySystem[4].AddTurret(TurretType.Missile, 5, new Vector3(-18, -20, 0), true);
        //    d.PlanetarySystem[4].AddTurret(TurretType.MultipleLasers, 6, new Vector3(-18, -8, 0), true);
        //    d.PlanetarySystem[4].AddTurret(TurretType.SlowMotion, 4, new Vector3(17, -15, 0), true);

        //    d.AddCelestialBody(Size.Normal, new Vector3(0, -75, 0), "2-5", "planete3", 0, 6);
        //    d.PlanetarySystem[5].AddTurret(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);

        //    d.AddCelestialBody(Size.Big, new Vector3(250, 50, 0), "2-6", "planete4", 0, 7);
        //    d.PlanetarySystem[6].AddTurret(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);
        //    d.PlanetarySystem[6].AddTurret(TurretType.Missile, 8, new Vector3(-6, -25, 0), true);

        //    d.AddCelestialBody(Size.Big, new Vector3(400, -150, 0), "2-7", "planete5", 0, 8);
        //    d.PlanetarySystem[7].AddTurret(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);
        //    d.PlanetarySystem[7].AddTurret(TurretType.Laser, 5, new Vector3(24, 0, 0), true);
        //    d.PlanetarySystem[7].AddTurret(TurretType.Laser, 6, new Vector3(20, 8, 0), true);
        //    d.PlanetarySystem[7].AddTurret(TurretType.Laser, 7, new Vector3(18, 16, 0), true);
        //    d.PlanetarySystem[7].AddTurret(TurretType.Laser, 8, new Vector3(10, 24, 0), true);
        //    d.PlanetarySystem[7].AddTurret(TurretType.Laser, 9, new Vector3(0, 32, 0), true);
        //    d.PlanetarySystem[7].AddTurret(TurretType.Laser, 10, new Vector3(0, 40, 0), true);

        //    d.AddCelestialBody(Size.Small, new Vector3(100, -220, 0), "2-8", "planete6", 0, 9);
        //    d.PlanetarySystem[8].AddTurret(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);

        //    d.AddCelestialBody(Size.Big, new Vector3(-300, -100, 0), "2-9", "planete7", 0, 10);
        //    d.PlanetarySystem[9].AddTurret(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);

        //    d.AddPinkHole(new Vector3(-125, -200, 0), "World3", 0, 11);
        //    d.PlanetarySystem[10].AddTurret(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);

        //    d.Objective.CelestialBodyToProtect = d.PlanetarySystem[10].PathPriority;

        //    c = new CelestialBodyDescriptor();
        //    c.Name = "Asteroid belt";
        //    c.Path = new Vector3(700, -400, 0);
        //    c.Speed = 2560000;
        //    c.StartingPosition = 75;
        //    c.Size = Size.Small;
        //    c.Images.Add("Plutoid");
        //    c.PathPriority = 0;
        //    c.AddTurret(TurretType.Alien, 1, Vector3.Zero, true, false, false);
        //    d.PlanetarySystem.Add(c);

        //    DescriptorInfiniteWaves v = new DescriptorInfiniteWaves();
        //    v.StartingDifficulty = 40;
        //    v.DifficultyIncrement = 0;
        //    v.MineralsPerWave = 0;
        //    v.MinMaxEnemiesPerWave = new Vector2(10, 30);
        //    v.Enemies = new List<EnemyType>();
        //    v.Enemies.Add(EnemyType.Asteroid);
        //    v.Enemies.Add(EnemyType.Comet);
        //    v.Enemies.Add(EnemyType.Plutoid);
        //    v.FirstOneStartNow = true;
        //    d.InfiniteWaves = v;

        //    return d;
        //}


        //private static LevelDescriptor GetWorld3Descriptor()
        //{
        //    LevelDescriptor d = new LevelDescriptor();

        //    CelestialBodyDescriptor c;

        //    d.Player.Lives = 1;
        //    d.Player.Money = 100;

        //    d.Infos.Background = "fondecran6";

        //    d.AddPinkHole(new Vector3(500, 0, 0), "World2", 0, 1);
        //    d.PlanetarySystem[0].AddTurret(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);

        //    d.AddCelestialBody(Size.Big, new Vector3(450, -200, 0), "3-1", "stationSpatiale1", 0, 2);
        //    d.PlanetarySystem[1].AddTurret(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);
        //    d.PlanetarySystem[1].AddTurret(TurretType.MultipleLasers, 10, new Vector3(-20, -15, 0), true);

        //    d.AddCelestialBody(Size.Big, new Vector3(350, 200, 0), "3-2", "stationSpatiale1", 0, 3);
        //    d.PlanetarySystem[2].AddTurret(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);

        //    d.AddCelestialBody(Size.Big, new Vector3(250, 0, 0), "3-3", "stationSpatiale2", 0, 4);
        //    d.PlanetarySystem[3].AddTurret(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);

        //    d.AddCelestialBody(Size.Normal, new Vector3(150, -200, 0), "3-4", "stationSpatiale1", 0, 5);
        //    d.PlanetarySystem[4].AddTurret(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);
        //    d.PlanetarySystem[4].AddTurret(TurretType.Basic, 10, new Vector3(-17, -13, 0), true);
        //    d.PlanetarySystem[4].AddTurret(TurretType.Basic, 10, new Vector3(-19, 0, 0), true);
        //    d.PlanetarySystem[4].AddTurret(TurretType.Basic, 10, new Vector3(-3, 15, 0), true);

        //    d.AddCelestialBody(Size.Normal, new Vector3(50, 200, 0), "3-5", "stationSpatiale2", 0, 6);
        //    d.PlanetarySystem[5].AddTurret(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);

        //    d.AddCelestialBody(Size.Big, new Vector3(-50, 0, 0), "3-6", "stationSpatiale2", 0, 7);
        //    d.PlanetarySystem[6].AddTurret(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);

        //    d.AddCelestialBody(Size.Normal, new Vector3(-150, -200, 0), "3-7", "stationSpatiale1", 0, 8);
        //    d.PlanetarySystem[7].AddTurret(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);

        //    d.AddCelestialBody(Size.Big, new Vector3(-200, 225, 0), "3-8", "stationSpatiale1", 0, 9);
        //    d.PlanetarySystem[8].AddTurret(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);
        //    d.PlanetarySystem[8].AddTurret(TurretType.Missile, 10, new Vector3(-25, 0, 0), true);
        //    d.PlanetarySystem[8].AddTurret(TurretType.Missile, 10, new Vector3(-18, -8, 0), true);

        //    d.AddCelestialBody(Size.Big, new Vector3(-300, 30, 0), "3-9", "stationSpatiale2", 0, 10);
        //    d.PlanetarySystem[9].AddTurret(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);
        //    d.PlanetarySystem[9].AddTurret(TurretType.MultipleLasers, 10, new Vector3(-20, 0, 0), true);

        //    d.AddCelestialBody(Size.Big, new Vector3(-400, -160, 0), "3-10", "planete1", 0, 11);
        //    d.PlanetarySystem[10].AddTurret(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false);


        //    d.Objective.CelestialBodyToProtect = d.PlanetarySystem[10].PathPriority;

        //    c = new CelestialBodyDescriptor();
        //    c.Name = "Asteroid belt";
        //    c.Path = new Vector3(700, -400, 0);
        //    c.Speed = 2560000;
        //    c.StartingPosition = 0;
        //    c.Size = Size.Small;
        //    c.Images.Add("Asteroid");
        //    c.PathPriority = 0;
        //    c.CanSelect = false;
        //    c.AddTurret(TurretType.Alien, 1, Vector3.Zero, true, false, false);
        //    d.PlanetarySystem.Add(c);

        //    DescriptorInfiniteWaves v = new DescriptorInfiniteWaves();
        //    v.StartingDifficulty = 70;
        //    v.DifficultyIncrement = 0;
        //    v.MineralsPerWave = 0;
        //    v.MinMaxEnemiesPerWave = new Vector2(10, 30);
        //    v.Enemies = new List<EnemyType>();
        //    v.Enemies.Add(EnemyType.Asteroid);
        //    v.Enemies.Add(EnemyType.Comet);
        //    v.Enemies.Add(EnemyType.Plutoid);
        //    v.FirstOneStartNow = true;
        //    d.InfiniteWaves = v;

        //    return d;
        //}
    }
}
