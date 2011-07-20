namespace EphemereGames.Commander
{
    using EphemereGames.Core.Persistence;
    using EphemereGames.Core.Utilities;


    public class SaveGame : PlayerData
    {
        public SerializableDictionaryProxy<int, int> Progress;
        public SerializableDictionaryProxy<int, int> Tutorials;
        public int CurrentWorld;


        // for deserialization only
        public SaveGame()
            : base() { }

        
        public SaveGame(Core.Input.Player player)
            : base(player)
        {
            Name = "Save";
            Folder = "Commander";
            File = "Save.xml";
        }


        public int LevelsFinishedCount
        {
            get { return Progress.Count; }
        }


        public int LastUnlockedWorld
        {
            get
            {
                int highestLevelDone = -1;

                foreach (var l in Progress.Keys)
                    if (l > highestLevelDone)
                        highestLevelDone = l;

                return Main.LevelsFactory.GetWorldFromLevelId(highestLevelDone);
            }
        }


        public void ClearAndSave()
        {
            Progress.Clear();
            Tutorials.Clear();
            CurrentWorld = 0;

            Save();
        }


        protected override void DoInitialize(object donnee)
        {
            SaveGame d = donnee as SaveGame;

            Progress = d.Progress;
            Tutorials = d.Tutorials;
            CurrentWorld = d.CurrentWorld;

            Progress.Initialize();
            Tutorials.Initialize();
        }


        public override void DoFileNotFound()
        {
            base.DoFileNotFound();

            FirstLoad();
        }


        protected override void DoLoadFailed()
        {
            base.DoLoadFailed();

            FirstLoad();
        }


        protected override void DoSaveStarted()
        {
            base.DoSaveStarted();

            Progress.InitializeToSave();
            Tutorials.InitializeToSave();
        }


        protected override void DoLoadEnded()
        {
            base.DoLoadEnded();
        }


        private void FirstLoad()
        {
            Progress = new SerializableDictionaryProxy<int, int>();
            Tutorials = new SerializableDictionaryProxy<int, int>();

            CurrentWorld = 0;

            Persistence.SaveData(this.Name);
            Loaded = true;
        }
    }
}
