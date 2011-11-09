namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.SimplePersistence;
    using Microsoft.Xna.Framework;

    
    class World
    {
        public WorldDescriptor Descriptor;
        public LevelDescriptor Layout;
        public Dictionary<int, LevelDescriptor> LevelsDescriptors;
        public HighScores HighScores;

        public bool EditorMode;
        public bool Editing;
        public bool CampaignMode;
        public bool MultiverseMode;


        public World()
        {
            Descriptor = null;
            Layout = null;
            LevelsDescriptors = new Dictionary<int, LevelDescriptor>();
            HighScores = new HighScores();

            EditorMode = false;
            Editing = false;
            CampaignMode = false;
            MultiverseMode = false;
        }


        public void Initialize()
        {
            // set lives so the planet won't explode...!
            Layout.Player.Lives = 1;
            Layout.Player.Money = 100;

            // generate the asteroid belt'input enemies
            var asteroidBelt = LevelDescriptor.GetAsteroidBelt(Layout.PlanetarySystem);

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

                Layout.InfiniteWaves = v;
            }
        }


        public int Id
        {
            get { return Descriptor.Id; }
        }


        public string Author
        {
            get { return Descriptor.Author; }
        }


        public string Name
        {
            get { return Descriptor.Name; }
        }


        public string StringId
        {
            get { return WorldsFactory.GetWorldStringId(Id); }
        }


        public string AnnounciationStringId
        {
            get { return StringId + "Annunciation"; }
        }


        public bool Unlocked
        {
            get
            {
                if (!CampaignMode)
                    return true;

                if (Preferences.Target == Core.Utilities.Setting.ArcadeRoyale)
                    return false;

                if (Id == 1)
                    return true;

                var other = Main.WorldsFactory.CampaignWorlds[Id - 1];

                foreach (var level in other.Descriptor.Levels)
                    if (!other.IsLevelUnlocked(level))
                        return false;

                return true;
            }
        }


        public int LastLevel
        {
            get { return Descriptor.Levels[Descriptor.Levels.Count - 1]; }
        }


        public LevelDescriptor GetLevelDescriptor(int id)
        {
            return LevelsDescriptors[id];
        }


        public void SetLevelDescriptor(int id, LevelDescriptor descriptor)
        {
            LevelsDescriptors[id] = descriptor;
        }


        public LevelDescriptor GetNextLevel(int currentLevelId)
        {
            var otherLevels = Descriptor.Levels;

            int currentIndex = otherLevels.FindIndex(l => l == currentLevelId);

            if (currentIndex == otherLevels.Count - 1)
                currentIndex = -1;

            if (currentIndex != -1)
                currentIndex = otherLevels[currentIndex + 1];

            return currentIndex == -1 ? null : LevelsDescriptors[currentIndex];
        }


        public string GetLevelStringId(int id)
        {
            for (int i = 0; i < Descriptor.Levels.Count; i++)
                if (Descriptor.Levels[i] == id)
                    return Id + "-" + (i + 1);

            return "";
        }


        public bool IsLevelUnlocked(int id)
        {
            // the highscore is only saved if the game is won.
            return HighScores.ContainsHighScores(id);
        }


        public void UnlockAllLevels()
        {
            foreach (var l in Descriptor.Levels)
                HighScores.Add(l, "cheater", 1);
        }


        public void LoadHighscores(string to)
        {
            HighScores = new HighScores() { Directory = to };

            Persistence.LoadData(HighScores);
        }
    }
}
