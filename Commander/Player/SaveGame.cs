namespace EphemereGames.Commander
{
    using EphemereGames.Core.Persistence;
    using EphemereGames.Core.Utilities;


    class SaveGame : PlayerData
    {
        public SerializableDictionaryProxy<int, int> Progress;
        public SerializableDictionaryProxy<int, int> Tutorials;


        public SaveGame(Player player)
            : base(player)
        {
            Name = "Save";
            Folder = "Commander";
            File = "Save.xml";
        }


        protected override void DoInitialize(object donnee)
        {
            SaveGame d = donnee as SaveGame;

            Progress = d.Progress;
            Tutorials = d.Tutorials;

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

            Persistence.SaveData(this.Name);
            Loaded = true;
        }
    }
}
