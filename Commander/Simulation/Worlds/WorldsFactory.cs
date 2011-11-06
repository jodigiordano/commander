namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Serialization;
    using EphemereGames.Commander.Cutscenes;
    using Microsoft.Xna.Framework;


    class WorldsFactory
    {
        public Dictionary<int, World> Worlds;
        public Dictionary<int, World> CampaignWorlds;
        public Dictionary<int, World> MultiverseWorlds;
        public Dictionary<int, LevelDescriptor> CutsceneDescriptors;

        public LevelDescriptor Menu;
        public LevelDescriptor Multiverse;

        private XmlSerializer LevelSerializer;
        private XmlSerializer WorldSerializer;
        private XmlSerializer HighscoresSerializer;

        private string CampaignDirectory;
        private string MenuDirectory;
        private string MultiverseDirectory;
        private string MultiverseHomeWorldDirectory;


        public WorldsFactory()
        {
            Worlds = new Dictionary<int, World>();
            CampaignWorlds = new Dictionary<int, World>();
            MultiverseWorlds = new Dictionary<int, World>();
            CutsceneDescriptors = new Dictionary<int, LevelDescriptor>();

            LevelSerializer = new XmlSerializer(typeof(LevelDescriptor));
            WorldSerializer = new XmlSerializer(typeof(WorldDescriptor));
            HighscoresSerializer = new XmlSerializer(typeof(HighScores));

            CampaignDirectory = @".\Content\scenarios\campaign\";
            MenuDirectory = @".\Content\scenarios\";
            MultiverseHomeWorldDirectory = @".\Content\scenarios\";
            MultiverseDirectory = @".\UserData\Multiverse\";
        }


        public void Initialize()
        {
            CampaignWorlds.Clear();
            MultiverseWorlds.Clear();

            LoadCampaign();
            LoadMenu();
            LoadMultiverse();
        }


        #region Menu

        private void LoadMenu()
        {
            Menu = LoadLevelDescriptor(MenuDirectory + "menu.xml");

            var newGame = Menu.PlanetarySystem[6];

            newGame.Name = "campaign";
            newGame.AddTurret(TurretType.Basic, 3, new Vector3(10, -14, 0), true, false);
            newGame.Invincible = true;

            var options = Menu.PlanetarySystem[1];
            options.Name = "options";
            options.AddTurret(TurretType.Basic, 1, new Vector3(-20, -5, 0), true, false);
            options.AddTurret(TurretType.MultipleLasers, 4, new Vector3(12, 0, 0), true, false);

            var editor = Menu.PlanetarySystem[2];
            editor.Name = "credits";
            editor.AddTurret(TurretType.Laser, 2, new Vector3(3, -7, 0), true, false);
            editor.AddTurret(TurretType.Missile, 1, new Vector3(-8, 0, 0), true, false);

            var quit = Menu.PlanetarySystem[3];
            quit.Name = "quit";

            var help = Menu.PlanetarySystem[4];
            help.Name = "multiverse";
            help.AddTurret(TurretType.SlowMotion, 2, new Vector3(-10, -3, 0), true, false);

            var credits = Menu.PlanetarySystem[5];
            credits.Name = "how to play";
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

        #endregion


        #region Multiverse

        public void AddMultiverseWorld(World w)
        {
            Worlds.Add(w.Id, w);
            MultiverseWorlds.Add(w.Id, w);

            SaveWorldOnDisk(w.Id);
        }


        private void LoadMultiverse()
        {
            Multiverse = LoadLevelDescriptor(MultiverseHomeWorldDirectory + "multiverse.xml");
        }


        private string GetWorldMultiverseLocalDirectory(int id)
        {
            return MultiverseDirectory + @"worlds\world" + id;
        }


        public static string WorldToURLArgument(int id)
        {
            return "world=" + id;
        }


        public static string GetWorldMultiverseRemoteDirectory(int id)
        {
            return
                Preferences.WebsiteURL +
                Preferences.MultiverseWorldsURL +
                @"/" + GetWorldMultiverseRemoteRelativeDirectory(id);
        }


        public static string GetWorldMultiverseRemoteRelativeDirectory(int id)
        {
            return "world" + id;
        }

        #endregion


        #region Campaign

        private void LoadCampaign()
        {
            LoadCutscenes();

            var directories = Directory.GetDirectories(CampaignDirectory, "world*");

            foreach (var d in directories)
            {
                var world = LoadWorld(d);
                world.CampaignMode = true;
                world.LoadHighscores(Main.PlayersController.CampaignData.Directory + @"\world" + world.Id);

                Worlds.Add(world.Descriptor.Id, world);
                CampaignWorlds.Add(world.Descriptor.Id, world);

                Core.Visual.Visuals.AddScene(new StoryScene("Cutscene" + world.Id, world.Id, new IntroCutscene()));
            }
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


        public static bool IsCampaignCB(CelestialBody cb)
        {
            return cb.Name == "campaign";
        }


        public int GetUnlockedWorldIdByIndex(int index)
        {
            var current = 0;

            foreach (var w in CampaignWorlds)
            {
                if (!w.Value.Unlocked)
                    continue;

                if (current == index)
                    return w.Key;

                current++;
            }

            return -1;
        }

        #endregion


        #region Worlds

        public World GetWorld(int id)
        {
            if (Worlds.ContainsKey(id))
                return Worlds[id];

            if (MultiverseWorldExistsOnDisk(id))
            {
                var directory = GetWorldMultiverseLocalDirectory(id);
                var w = LoadWorld(directory);

                Worlds.Add(w.Descriptor.Id, w);
                MultiverseWorlds.Add(w.Descriptor.Id, w);

                return w;
            }

            return null;
        }


        public string GetWorldLastModification(int id)
        {
            return Worlds[id].Descriptor.LastModification;
        }


        public bool MultiverseWorldExistsOnDisk(int id)
        {
            var directory = GetWorldMultiverseLocalDirectory(id);

            return
                File.Exists(directory + @"\world.xml") &&
                File.Exists(directory + @"\layout.xml");
        }


        public void SaveWorldOnDisk(int id)
        {
            World w = Worlds[id];

            var directory = GetWorldMultiverseLocalDirectory(id);

            Main.PlayersController.CreateDirectory(directory);
            Main.PlayersController.ClearDirectory(directory);

            SaveWorldDescriptor(w.Descriptor, directory + @"\world.xml");
            SaveLevelDescriptor(w.Layout, directory + @"\layout.xml");

            foreach (var l in w.LevelsDescriptors)
                SaveLevelDescriptor(l.Value, directory + @"\level" + l.Key + ".xml");
        }


        public static string GetWorldStringId(int id)
        {
            return "World " + id;
        }


        public World GetEmptyWorld(int id)
        {
            return new World()
            {
                Descriptor = GetEmptyWorldDescriptor(id),
                Layout = GetEmptyLevelDescriptor(id)
            };
        }


        public WorldDescriptor GetEmptyWorldDescriptor(int id)
        {
            var w = new WorldDescriptor()
            {
                Id = id,
                Name = "My World"
            };

            return w;
        }


        private WorldDescriptor LoadWorldDescriptor(string path)
        {
            using (StreamReader reader = new StreamReader(path))
                return (WorldDescriptor) WorldSerializer.Deserialize(reader);
        }


        private WorldDescriptor LoadWorldDescriptorFromString(string txt)
        {
            using (StringReader reader = new StringReader(txt))
                return (WorldDescriptor) WorldSerializer.Deserialize(reader);
        }


        private void SaveWorldDescriptor(WorldDescriptor descriptor, string path)
        {
            using (StreamWriter writer = new StreamWriter(path))
                WorldSerializer.Serialize(writer.BaseStream, descriptor);
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

            return w;
        }


        public World LoadWorldFromStrings(List<KeyValuePair<string, string>> strings)
        {
            World w = new World();

            foreach (var f in strings)
            {
                if (f.Key == "world.xml")
                    w.Descriptor = LoadWorldDescriptorFromString(f.Value);
                else if (f.Key == "layout.xml")
                    w.Layout = LoadLevelDescriptorFromString(f.Value);
                else
                {
                    var level = LoadLevelDescriptorFromString(f.Value);
                    w.LevelsDescriptors.Add(level.Infos.Id, level);
                }
            }

            w.Initialize();

            return w;
        }


        #endregion


        #region Levels

        public LevelDescriptor GetNextLevel(int worldId, int currentLevelId)
        {
            return CampaignWorlds[worldId].GetNextLevel(currentLevelId);
        }


        public LevelDescriptor GetEmptyLevelDescriptor(int id)
        {
            var l = new LevelDescriptor();

            l.AddAsteroidBelt();

            l.Infos.Id = id;

            return l;
        }


        private LevelDescriptor LoadLevelDescriptor(string path)
        {
            using (StreamReader reader = new StreamReader(path))
                return (LevelDescriptor) LevelSerializer.Deserialize(reader);
        }


        private LevelDescriptor LoadLevelDescriptorFromString(string txt)
        {
            using (StringReader reader = new StringReader(txt))
                return (LevelDescriptor) LevelSerializer.Deserialize(reader);
        }


        private void SaveLevelDescriptor(LevelDescriptor descriptor, string path)
        {
            using (StreamWriter writer = new StreamWriter(path))
                LevelSerializer.Serialize(writer.BaseStream, descriptor);
        }

        #endregion
    }
}
