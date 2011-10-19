namespace EphemereGames.Commander
{
    using EphemereGames.Core.Persistence;
    using EphemereGames.Core.Utilities;


    public class SaveGame : PlayerData
    {
        public SerializableDictionaryProxy<int, int> Progress;
        public SerializableDictionaryProxy<int, int> Scores;
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


        public void ClearAndSave()
        {
            Progress.Clear();
            Scores.Clear();
            Tutorials.Clear();
            CurrentWorld = 0;

            Save();
        }


        public void UpdateProgress(GameState state, int level, int score)
        {
            if (state == GameState.Won && !Progress.ContainsKey(level))
                Progress.Add(level, 1);

            if (!Scores.ContainsKey(level))
                Scores.Add(level, score);
            else if (score > Scores[level])
                Scores[level] = score;
        }


        protected override void DoInitialize(object donnee)
        {
            SaveGame d = donnee as SaveGame;

            Progress = d.Progress;
            Scores = d.Scores;
            Tutorials = d.Tutorials;
            CurrentWorld = d.CurrentWorld;

            Progress.Initialize();
            Scores.Initialize();
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
            Scores.InitializeToSave();
            Tutorials.InitializeToSave();
        }


        protected override void DoLoadEnded()
        {
            base.DoLoadEnded();
        }


        private void FirstLoad()
        {
            Progress = new SerializableDictionaryProxy<int, int>();
            Scores = new SerializableDictionaryProxy<int, int>();
            Tutorials = new SerializableDictionaryProxy<int, int>();

            CurrentWorld = 0;

            Persistence.SaveData(this.Name);
            Loaded = true;
        }
    }
}
