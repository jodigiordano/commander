namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.SimplePersistence;


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
            set { Descriptor.Name = value; }
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
            if (!CampaignMode)
                return true;

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


        public int AddLevel()
        {
            var id = Descriptor.GetNextLevelId();
            var level = Main.WorldsFactory.GetEmptyLevelDescriptor(id);

            Descriptor.Levels.Add(id);
            LevelsDescriptors.Add(id, level);

            return id;
        }


        public void RemoveLevel(int id)
        {
            Descriptor.Levels.Remove(id);
            LevelsDescriptors.Remove(id);
        }


        public int AddWarp()
        {
            var id = Descriptor.GetNextNoWarpId();

            Descriptor.Warps.Add(id);

            return id;
        }


        public void ModifyWarp(int oldId, int newId)
        {
            var index = Descriptor.Warps.IndexOf(oldId);

            Descriptor.Warps.Insert(index, newId);
            Descriptor.Warps.Remove(oldId);
        }


        public void RemoveWarp(int id)
        {
            Descriptor.Warps.Remove(id);
        }
    }
}
