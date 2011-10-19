namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Serialization;
    using Microsoft.Xna.Framework;


    class LevelsFactory
    {
        private Dictionary<int, LevelDescriptor> Descriptors;
        public Dictionary<int, LevelDescriptor> CutsceneDescriptors;

        public LevelDescriptor Menu;
        public Dictionary<int, WorldDescriptor> WorldsDescriptors;

        private XmlSerializer LevelSerializer;
        private XmlSerializer WorldSerializer;

        private string DescriptorsDirectory;


        public LevelsFactory()
        {
            Descriptors = new Dictionary<int, LevelDescriptor>();
            CutsceneDescriptors = new Dictionary<int, LevelDescriptor>();
            WorldsDescriptors = new Dictionary<int, WorldDescriptor>();

            LevelSerializer = new XmlSerializer(typeof(LevelDescriptor));
            WorldSerializer = new XmlSerializer(typeof(WorldDescriptor));

            DescriptorsDirectory = @".\Content\scenarios";
        }


        public void Initialize()
        {
            Descriptors.Clear();

            LoadLevels(DescriptorsDirectory, "level", Descriptors);
            LoadLevels(DescriptorsDirectory, "layoutworld", Descriptors);
            LoadWorlds();
            LoadCutscenes();
            LoadMenuDescriptor();

            foreach (var w in WorldsDescriptors.Values)
                PrepareWorldLayout(w.Layout);
        }


        public LevelDescriptor GetLevelDescriptor(int id)
        {
            return Descriptors[id];
        }


        public void SetLevelDescriptor(int id, LevelDescriptor descriptor)
        {
            Descriptors[id] = descriptor;
        }


        public string GetWorldStringId(int id)
        {
            return "World " + id;
        }


        public string GetLevelStringId(int id)
        {
            foreach (var w in WorldsDescriptors.Values)
                for (int i = 0; i < w.Levels.Count; i++)
                    if (w.Levels[i] == id)
                        return w.Id + "-" + (i + 1);

            return "";
        }


        public string GetWorldAnnounciationStringId(int id)
        {
            return GetWorldStringId(id) + "Annunciation";
        }


        public int GetUnlockedWorldIdByIndex(int index)
        {
            var current = 0;

            foreach (var w in WorldsDescriptors.Keys)
            {
                if (!IsWorldUnlocked(w))
                    continue;

                if (current == index)
                    return w;

                current++;
            }

            return -1;
        }


        public EndOfWorldAnimation GetEndOfWorldAnimation(WorldScene scene)
        {
            EndOfWorldAnimation result = null;

            switch (scene.Id)
            {
                case 1:
                    result = new EndOfWorld1Animation(scene);
                    break;
            }

            return result;
        }


        public bool IsWorldUnlocked(int worldId)
        {
            if (worldId == -1)
                return true;

            var descriptor = WorldsDescriptors[worldId];

            foreach (var level in descriptor.Levels)
                if (!Main.SaveGameController.IsLevelUnlocked(level))
                    return false;

            return true;
        }


        private void LoadLevels(string root, string startingWith, Dictionary<int, LevelDescriptor> to)
        {
            string[] levelsFiles = Directory.GetFiles(root, startingWith + "*.xml");

            foreach (var f in levelsFiles)
            {
                var descriptor = LoadLevelDescriptor(f);
                to.Add(descriptor.Infos.Id, descriptor);
            }
        }


        private void LoadWorlds()
        {
            WorldsDescriptors.Clear();

            string[] files = Directory.GetFiles(DescriptorsDirectory, "world" + "*.xml");

            foreach (var f in files)
            {
                var descriptor = LoadWorldDescriptor(f);
                WorldsDescriptors.Add(descriptor.Id, descriptor);
            }
        }


        private void LoadCutscenes()
        {
            CutsceneDescriptors.Clear();

            string[] levelsFiles = Directory.GetFiles(DescriptorsDirectory, "cutscene*.xml");

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


        private WorldDescriptor LoadWorldDescriptor(string path)
        {
            using (StreamReader reader = new StreamReader(path))
                return (WorldDescriptor) WorldSerializer.Deserialize(reader.BaseStream);
        }
        

        public void SaveDescriptorOnDisk(int id)
        {
            using (StreamWriter writer = new StreamWriter(DescriptorsDirectory + @"\level" + id + ".xml"))
                LevelSerializer.Serialize(writer.BaseStream, Descriptors[id]);
        }


        //public void DeleteUserDescriptorFromDisk(int id)
        //{
        //    if (File.Exists(UserDescriptorsDirectory + @"\level" + id + ".xml"))
        //        File.Delete(UserDescriptorsDirectory + @"\level" + id + ".xml");
        //}


        public int GetWorldFromLevelId(int id)
        {
            foreach (var w in WorldsDescriptors.Values)
                foreach (var kvp in w.Levels)
                    if (kvp == id)
                        return w.Id;

            return 1;
        }


        public int GetNextLevel(int worldId, int currentLevelId)
        {
            var otherLevels = WorldsDescriptors[worldId].Levels;

            otherLevels.Sort(delegate(int level1, int level2)
            {
                return level1 > level2 ? 1 : level1 < level2 ? -1 : 0;
            });

            foreach (var other in otherLevels)
                if (other > currentLevelId)
                    return other;

            return -1;
        }


        private void LoadMenuDescriptor()
        {
            Menu = LoadLevelDescriptor(DescriptorsDirectory + @"\menu.xml");

            var newGame = Menu.PlanetarySystem[6];

            newGame.Name = "save the world";
            newGame.AddTurret(TurretType.Basic, 5, new Vector3(10, -14, 0), true, false);
            newGame.Invincible = true;

            var options = Menu.PlanetarySystem[1];
            options.Name = "options";
            options.AddTurret(TurretType.Basic, 2, new Vector3(-20, -5, 0), true, false);
            options.AddTurret(TurretType.MultipleLasers, 4, new Vector3(12, 0, 0), true, false);

            var editor = Menu.PlanetarySystem[2];
            editor.Name = "editor";
            editor.AddTurret(TurretType.Laser, 7, new Vector3(3, -7, 0), true, false);
            editor.AddTurret(TurretType.Missile, 3, new Vector3(-8, 0, 0), true, false);

            var quit = Menu.PlanetarySystem[3];
            quit.Name = "quit";

            var help = Menu.PlanetarySystem[4];
            help.Name = "how to play";
            help.AddTurret(TurretType.SlowMotion, 3, new Vector3(-10, -3, 0), true, false);

            var credits = Menu.PlanetarySystem[5];
            credits.Name = "credits";
            credits.AddTurret(TurretType.SlowMotion, 1, new Vector3(-10, -3, 0), true, false);


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

            d.Infos.Background = "background14";

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


        public LevelDescriptor GetEmptyDescriptor(int id)
        {
            var l = new LevelDescriptor();

            l.AddAsteroidBelt();

            l.Infos.Id = id;

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


        private void PrepareWorldLayout(int id)
        {
            LevelDescriptor d = Descriptors[id];

            // set lives so the planet won't explode...!
            d.Player.Lives = 1;
            d.Player.Money = 100;

            // generate the asteroid belt's enemies
            var asteroidBelt = LevelDescriptor.GetAsteroidBelt(d.PlanetarySystem);

            if (asteroidBelt != null)
            {
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
            }

            //var pinkHolesToAdd = new List<CelestialBodyDescriptor>();

            //// switch some planets for pink holes
            //for (int i = d.PlanetarySystem.Count - 1; i > -1; i--)
            //{
            //    var cb = d.PlanetarySystem[i];

            //    if (!cb.Name.StartsWith("World"))
            //        continue;

            //    var pinkHole = d.CreatePinkHole(cb.Position, cb.Name, (int) cb.Speed, cb.PathPriority);
            //    pinkHole.AddTurret(TurretType.Gravitational, 1, new Vector3(1, -2, 0), false, false);

            //    pinkHolesToAdd.Add(pinkHole);

            //    d.PlanetarySystem.RemoveAt(i);
            //}

            //foreach (var pink in pinkHolesToAdd)
            //    d.PlanetarySystem.Add(pink);

        }


        private void CreateDirectory(string directory)
        {
            if (Directory.Exists(directory))
                return;

            Directory.CreateDirectory(directory);
        }
    }
}
