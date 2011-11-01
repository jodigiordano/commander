namespace EphemereGames.Commander
{
    using EphemereGames.Core.SimplePersistence;


    public class OptionsData : SimpleData
    {
        public int MusicVolume;
        public int SfxVolume;
        public bool FullScreen;
        public bool ShowHelpBar;


        public OptionsData()
        {
            Name = "Options";
            Directory = "UserData";
            File = "Options.xml";
        }


        protected override void DoInitialize(object data)
        {
            OptionsData d = data as OptionsData;

            MusicVolume = d.MusicVolume;
            SfxVolume = d.SfxVolume;
            FullScreen = d.FullScreen;
            ShowHelpBar = d.ShowHelpBar;
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
        }


        protected override void DoLoadEnded()
        {
            base.DoLoadEnded();

            Main.Options.MusicVolume = MusicVolume;
            Main.Options.SfxVolume = SfxVolume;
        }


        private void FirstLoad()
        {
            MusicVolume = Main.Options.MusicVolume;
            SfxVolume = Main.Options.SfxVolume;
            FullScreen = Main.Options.FullScreen;
            ShowHelpBar = Main.Options.ShowHelpBar;

            Persistence.SaveData(this);

            Loaded = true;
        }
    }
}
