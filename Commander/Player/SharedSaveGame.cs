namespace EphemereGames.Commander
{
    using EphemereGames.Core.Audio;
    using EphemereGames.Core.Persistence;
    using EphemereGames.Core.Utilities;
    

    public class SharedSaveGame : SharedData
    {
        public int VolumeMusic;
        public int VolumeSfx;

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

            VolumeMusic = d.VolumeMusic;
            VolumeSfx = d.VolumeSfx;
            HighScores = d.HighScores;
            
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

            Audio.MusicVolume = VolumeMusic / 10f;
            Audio.SfxVolume = VolumeSfx / 10f;
        }


        public void ApplyChanges()
        {
            Audio.MusicVolume = VolumeMusic / 10f;
            Audio.SfxVolume = VolumeSfx / 10f;
        }


        private void FirstLoad()
        {
            VolumeMusic = 5;
            VolumeSfx = 5;

            HighScores = new SerializableDictionaryProxy<int, HighScores>();

            ApplyChanges();
            Save();

            Loaded = true;
        }
    }
}
