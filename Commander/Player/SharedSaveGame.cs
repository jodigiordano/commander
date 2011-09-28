namespace EphemereGames.Commander
{
    using EphemereGames.Core.Persistence;
    using EphemereGames.Core.Utilities;
    using EphemereGames.Core.XACTAudio;
    

    public class SharedSaveGame : SharedData
    {
        public int MusicVolume;
        public int SfxVolume;
        public bool FullScreen;
        public bool ShowHelpBar;

        public SerializableDictionaryProxy<int, HighScores> HighScores;


        public SharedSaveGame()
        {
            Name = "SharedSaveGame";
            Folder = "Commander";
            File = "SharedSaveGame.xml";
        }


        protected override void DoInitialize(object data)
        {
            SharedSaveGame d = data as SharedSaveGame;

            MusicVolume = d.MusicVolume;
            SfxVolume = d.SfxVolume;
            HighScores = d.HighScores;
            FullScreen = d.FullScreen;
            ShowHelpBar = d.ShowHelpBar;
            
            HighScores.Initialize();
        }


        public void UpdateProgress(string name, int level, int score)
        {
            if (!HighScores.ContainsKey(level))
                HighScores.Add(level, new HighScores(level));

            HighScores[level].Add(name, score);
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

            HighScores.InitializeToSave();
        }


        protected override void DoLoadEnded()
        {
            base.DoLoadEnded();

            XACTAudio.ChangeCategoryVolume("Music", MusicVolume);
            XACTAudio.ChangeCategoryVolume("Default", SfxVolume);
        }


        private void FirstLoad()
        {
            MusicVolume = Main.Options.MusicVolume;
            SfxVolume = Main.Options.SfxVolume;
            FullScreen = Main.Options.FullScreen;
            ShowHelpBar = Main.Options.ShowHelpBar;

            HighScores = new SerializableDictionaryProxy<int, HighScores>();

            Save();

            Loaded = true;
        }
    }
}
