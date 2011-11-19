namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Xml.Serialization;
    using EphemereGames.Commander.Cutscenes;
    using ICSharpCode.SharpZipLib.Zip;
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

        private static string CampaignDirectory = @".\Content\scenarios\campaign\";
        private static string MenuDirectory = @".\Content\scenarios\";
        private static string MultiverseDirectory = @".\UserData\Multiverse\";
        private static string MultiverseHomeWorldDirectory = @".\Content\scenarios\";


        public WorldsFactory()
        {
            Worlds = new Dictionary<int, World>();
            CampaignWorlds = new Dictionary<int, World>();
            MultiverseWorlds = new Dictionary<int, World>();
            CutsceneDescriptors = new Dictionary<int, LevelDescriptor>();

            LevelSerializer = new XmlSerializer(typeof(LevelDescriptor));
            WorldSerializer = new XmlSerializer(typeof(WorldDescriptor));
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

            var newGame = (PlanetCBDescriptor) Menu.PlanetarySystem[6];

            newGame.Name = "campaign";
            newGame.AddTurret(TurretType.Basic, 3, new Vector3(10, -14, 0), true, false);
            newGame.Invincible = true;

            var options = (PlanetCBDescriptor) Menu.PlanetarySystem[1];
            options.Name = "options";
            options.AddTurret(TurretType.Basic, 1, new Vector3(-20, -5, 0), true, false);
            options.AddTurret(TurretType.MultipleLasers, 4, new Vector3(12, 0, 0), true, false);

            var editor = (PlanetCBDescriptor) Menu.PlanetarySystem[2];
            editor.Name = "credits";
            editor.AddTurret(TurretType.Laser, 2, new Vector3(3, -7, 0), true, false);
            editor.AddTurret(TurretType.Missile, 1, new Vector3(-8, 0, 0), true, false);

            var quit = Menu.PlanetarySystem[3];
            quit.Name = "quit";

            var help = (PlanetCBDescriptor) Menu.PlanetarySystem[4];
            help.Name = "multiverse";
            help.AddTurret(TurretType.SlowMotion, 2, new Vector3(-10, -3, 0), true, false);

            var credits = (PlanetCBDescriptor) Menu.PlanetarySystem[5];
            credits.Name = "how to play";
            credits.AddTurret(TurretType.SlowMotion, 1, new Vector3(-10, -3, 0), true, false);


            InfiniteWavesDescriptor v = new InfiniteWavesDescriptor()
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
            if (Worlds.ContainsKey(w.Id))
            {
                Worlds[w.Id] = w;
                MultiverseWorlds[w.Id] = w;
            }

            else
            {
                Worlds.Add(w.Id, w);
                MultiverseWorlds.Add(w.Id, w);
            }

            SaveWorldOnDisk(w.Id);
        }


        private void LoadMultiverse()
        {
            Multiverse = LoadLevelDescriptor(MultiverseHomeWorldDirectory + "multiverse.xml");
        }


        public static string WorldToURLArgument(int id)
        {
            return "world=" + id;
        }


        public static string WorldUsernameToURLArgument(string username)
        {
            return "world_username=" + username;
        }


        public static string GetWorldZipFileName(int id)
        {
            return "world" + id + ".zip";
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


        public static string GetWorldMultiverseLocalDirectory(int id)
        {
            return MultiverseDirectory + @"worlds\world" + id;
        }


        public static string GetWorldMultiverseRemoteZipFile(int id)
        {
            return GetWorldMultiverseRemoteDirectory(id) + @"/" + GetWorldZipFileName(id);
        }


        public static string GetWorldMultiverseLocalZipFile(int id)
        {
            return GetWorldMultiverseLocalDirectory(id) + @"/" + GetWorldZipFileName(id);
        }


        public static void CreateWorldHighscoresFromString(int id, string data)
        {
            var hs = new HighScores()
            {
                Directory = GetWorldMultiverseLocalDirectory(id)
            };

            Core.SimplePersistence.Persistence.SaveDataFromString(hs, data);
        }


        #endregion


        #region Campaign

        private void LoadCampaign()
        {
            LoadCutscenes();

            var directories = Directory.GetDirectories(CampaignDirectory, "world*");

            foreach (var d in directories)
            {
                var infos = new DirectoryInfo(d);
                var world = LoadWorldFromDisk(d + @"\" + infos.Name + ".zip");
                world.CampaignMode = true;
                world.LoadHighscoresFromDisk(Main.PlayersController.CampaignData.Directory + @"\world" + world.Id);

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

        public void EmptyWorldDirectory(int id)
        {
            var directory = GetWorldMultiverseLocalDirectory(id);

            Main.PlayersController.CreateDirectory(directory);
            Main.PlayersController.ClearDirectory(directory);
        }


        public World GetWorld(int id)
        {
            if (Worlds.ContainsKey(id))
                return Worlds[id];

            if (MultiverseWorldExistsOnDisk(id))
            {
                var w = LoadWorldFromDisk(GetWorldMultiverseLocalZipFile(id));
                
                w.LoadHighscoresFromDisk(GetWorldMultiverseLocalDirectory(id));

                Worlds.Add(w.Descriptor.Id, w);
                MultiverseWorlds.Add(w.Descriptor.Id, w);

                return w;
            }

            return null;
        }


        public string GetWorldLastModification(int id)
        {
            return GetWorld(id).Descriptor.LastModification;
        }


        public bool MultiverseWorldExistsOnDisk(int id)
        {
            return File.Exists(GetWorldMultiverseLocalZipFile(id));
        }


        public bool MultiverseWorldIsValidOnDisk(int id)
        {
            try
            {
                LoadWorldFromDisk(GetWorldMultiverseLocalZipFile(id));

                return true;
            }

            catch
            {
                return false;
            }
        }


        public void SaveWorldOnDisk(int id)
        {
            World w = Worlds[id];

            var directory = GetWorldMultiverseLocalDirectory(id);

            Main.PlayersController.CreateDirectory(directory);
            Main.PlayersController.ClearDirectory(directory);

            FileStream zipFile = File.Create(GetWorldMultiverseLocalZipFile(id));

            using (ZipOutputStream output = new ZipOutputStream(zipFile))
            {
                ZipEntry entry;
                byte[] data;
                var encoding = new UTF8Encoding();

                entry = new ZipEntry("world.xml");
                data = encoding.GetBytes(w.Descriptor.ToXML());
                output.PutNextEntry(entry);
                output.Write(data, 0, data.Length);

                entry = new ZipEntry("layout.xml");
                data = encoding.GetBytes(w.Layout.ToXML());
                output.PutNextEntry(entry);
                output.Write(data, 0, data.Length);

                foreach (var l in w.LevelsDescriptors)
                {
                    entry = new ZipEntry("level" + l.Key + ".xml");
                    data = encoding.GetBytes(l.Value.ToXML());
                    output.PutNextEntry(entry);
                    output.Write(data, 0, data.Length);
                }
            }
        }


        public World LoadWorldFromDisk(string file)
        {
            World w = null;

            FileStream zipFile = new FileStream(file, FileMode.Open, FileAccess.Read);

            var files = new List<KeyValuePair<string, byte[]>>();
            var encoding = new UTF8Encoding();

            using (ZipInputStream input = new ZipInputStream(zipFile))
            {
                ZipEntry entry = input.GetNextEntry();

                while (entry != null)
                {
                    byte[] buffer = new byte[input.Length];
                    input.Read(buffer, 0, (int) input.Length);

                    files.Add(new KeyValuePair<string, byte[]>(entry.Name, buffer));

                    entry = input.GetNextEntry();
                }
            }

            w = LoadWorldFromStream(files);

            return w;
        }


        private World LoadWorldFromStream(List<KeyValuePair<string, byte[]>> bytes)
        {
            World w = new World();

            foreach (var b in bytes)
            {
                if (b.Key == "world.xml")
                    w.Descriptor = LoadWorldDescriptorFromStream(b.Value);
                else if (b.Key == "layout.xml")
                    w.Layout = LoadLevelDescriptorFromStream(b.Value);
                else
                {
                    var level = LoadLevelDescriptorFromStream(b.Value);
                    w.LevelsDescriptors.Add(level.Infos.Id, level);
                }
            }

            w.Initialize();

            return w;
        }


        private WorldDescriptor LoadWorldDescriptorFromStream(byte[] txt)
        {
            MemoryStream ms = new MemoryStream(txt);

            using (StreamReader reader = new StreamReader(ms, true))
                return (WorldDescriptor) WorldSerializer.Deserialize(reader);
        }


        public static string GetWorldStringId(int id)
        {
            return "World " + id;
        }


        public World GetEmptyWorld(int id, string author)
        {
            return new World()
            {
                Descriptor = GetEmptyWorldDescriptor(id, author),
                Layout = GetEmptyLevelDescriptor(id)
            };
        }


        public WorldDescriptor GetEmptyWorldDescriptor(int id, string author)
        {
            var w = new WorldDescriptor()
            {
                Id = id,
                Author = author,
                Name = "My World"
            };

            return w;
        }


        public bool GetWorldUnlocked(int id)
        {
            // Campaign
            if (CampaignWorlds.ContainsKey(id))
                return CampaignWorlds[id].Unlocked;

            // Multiverse
            return id > 0;
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


        private LevelDescriptor LoadLevelDescriptorFromStream(byte[] bytes)
        {
            MemoryStream ms = new MemoryStream(bytes);

            using (StreamReader reader = new StreamReader(ms, true))
                return (LevelDescriptor) LevelSerializer.Deserialize(reader);
        }

        #endregion
    }
}
