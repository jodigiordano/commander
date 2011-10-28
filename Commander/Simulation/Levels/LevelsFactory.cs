namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Serialization;
    using EphemereGames.Commander.Cutscenes;
    using Microsoft.Xna.Framework;


    class LevelsFactory
    {
        public Dictionary<int, World> Worlds;
        public Dictionary<int, LevelDescriptor> CutsceneDescriptors;

        public LevelDescriptor Menu;

        private XmlSerializer LevelSerializer;
        private XmlSerializer WorldSerializer;

        private string CampaignDirectory;
        private string MenuDirectory;
        private string EditorDirectory;


        public LevelsFactory()
        {
            Worlds = new Dictionary<int, World>();
            CutsceneDescriptors = new Dictionary<int, LevelDescriptor>();

            LevelSerializer = new XmlSerializer(typeof(LevelDescriptor));
            WorldSerializer = new XmlSerializer(typeof(WorldDescriptor));

            CampaignDirectory = @".\Content\scenarios\campaign\";
            MenuDirectory = @".\Content\scenarios\";
            EditorDirectory = @".\Content\scenarios\universe\";
        }


        public void Initialize()
        {
            Worlds.Clear();

            LoadCampaign();
            LoadMenu();
            LoadEditor();
        }


        public static string GetWorldStringId(int id)
        {
            return "World " + id;
        }


        public static bool IsCampaignCB(CelestialBody cb)
        {
            return cb.Name == "campaign";
        }


        //public static string GetWorldAnnounciationStringId(int id)
        //{
        //    return GetWorldStringId(id) + "Annunciation";
        //}


        public int GetUnlockedWorldIdByIndex(int index)
        {
            var current = 0;

            foreach (var w in Worlds)
            {
                if (!w.Value.Unlocked)
                    continue;

                if (current == index)
                    return w.Key;

                current++;
            }

            return -1;
        }


        public void SaveWorldOnDisk(int id)
        {
            World w = Worlds[id];

            //using (StreamWriter writer = new StreamWriter(DescriptorsDirectory + @"\level" + id + ".xml"))
            //    LevelSerializer.Serialize(writer.BaseStream, Descriptors[id]);
        }


        public EndOfWorldAnimation GetEndOfWorldAnimation(int id, WorldScene scene)
        {
            EndOfWorldAnimation result = null;

            switch (id)
            {
                case 1:
                    result = new EndOfWorld1Animation(scene);
                    break;
            }

            return result;
        }


        private void LoadCampaign()
        {
            LoadCutscenes();
            
            var worlds = Directory.GetDirectories(CampaignDirectory, "world*");

            foreach (var w in worlds)
            {
                LoadWorld(w);
            }
        }


        private void LoadMenu()
        {
            Menu = LoadLevelDescriptor(MenuDirectory + "menu.xml");

            var newGame = Menu.PlanetarySystem[6];

            newGame.Name = "campaign";
            newGame.AddTurret(TurretType.Basic, 5, new Vector3(10, -14, 0), true, false);
            newGame.Invincible = true;

            var options = Menu.PlanetarySystem[1];
            options.Name = "options";
            options.AddTurret(TurretType.Basic, 2, new Vector3(-20, -5, 0), true, false);
            options.AddTurret(TurretType.MultipleLasers, 4, new Vector3(12, 0, 0), true, false);

            var editor = Menu.PlanetarySystem[2];
            editor.Name = "universe";
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


        private void LoadEditor()
        {
            var w = LoadWorld(EditorDirectory);

            w.EditorMode = true;
        }


        private World LoadWorld(string directory)
        {
            World w = new World();

            w.Descriptor = LoadWorldDescriptor(directory + @"\world.xml");
            w.Layout = LoadLevelDescriptor(directory + @"\layout.xml");

            var levels = Directory.GetFiles(directory, @"level*.xml");

            foreach (var l in levels)
            {
                var descriptor = LoadLevelDescriptor(l);
                w.LevelsDescriptors.Add(descriptor.Infos.Id, descriptor);
            }

            w.Initialize();

            Core.Visual.Visuals.AddScene(new StoryScene("Cutscene" + w.Id, w.Id, new IntroCutscene()));

            Worlds.Add(w.Descriptor.Id, w);

            return w;
        }


        private void LoadCutscenes()
        {
            CutsceneDescriptors.Clear();

            var files = Directory.GetFiles(CampaignDirectory + @"cutscenes\", "cutscene*.xml");

            foreach (var f in files)
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


        //public void DeleteUserDescriptorFromDisk(int id)
        //{
        //    if (File.Exists(UserDescriptorsDirectory + @"\level" + id + ".xml"))
        //        File.Delete(UserDescriptorsDirectory + @"\level" + id + ".xml");
        //}


        public LevelDescriptor GetNextLevel(int worldId, int currentLevelId)
        {
            return Worlds[worldId].GetNextLevel(currentLevelId);
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


        public LevelDescriptor GetEmptyLevelDescriptor(int id)
        {
            var l = new LevelDescriptor();

            l.AddAsteroidBelt();

            l.Infos.Id = id;

            return l;
        }


        private void CreateDirectory(string directory)
        {
            if (Directory.Exists(directory))
                return;

            Directory.CreateDirectory(directory);
        }
    }
}
