namespace EphemereGames.Commander
{
    class Options
    {
        public event BooleanHandler ShowHelpBarChanged;
        public event BooleanHandler FullScreenChanged;
        public event Integer2Handler VolumeMusicChanged;
        public event Integer2Handler VolumeSfxChanged;


        private bool showHelpBar;
        private bool fullScreen;
        private int volumeMusic;
        private int volumeSfx;
        

        public Options()
        {
            ShowHelpBar = true;

#if DEBUG
            FullScreen = false;
#else
            FullScreen = true;
#endif

            SfxVolume = 10;
            MusicVolume = 10;
        }


        public bool ShowHelpBar
        {
            get { return showHelpBar; }
            set
            {
                bool changed = value != showHelpBar;

                showHelpBar = value;

                if (changed)
                    NotifyShowHelpBarChanged();
            }
        }


        public bool FullScreen
        {
            get { return fullScreen; }
            set
            {
                bool changed = value != fullScreen;

                fullScreen = value;

                if (changed)
                    NotifyFullScreenChanged();
            }
        }


        public int MusicVolume
        {
            get { return volumeMusic; }
            set
            {
                bool changed = value != volumeMusic;

                volumeMusic = value;

                if (changed)
                    NotifyVolumeMusicChanged();
            }
        }


        public int SfxVolume
        {
            get { return volumeSfx; }
            set
            {
                bool changed = value != volumeSfx;

                volumeSfx = value;

                if (changed)
                    NotifyVolumeSfxChanged();
            }
        }


        private void NotifyShowHelpBarChanged()
        {
            if (ShowHelpBarChanged != null)
                ShowHelpBarChanged(showHelpBar);
        }


        private void NotifyFullScreenChanged()
        {
            if (FullScreenChanged != null)
                FullScreenChanged(fullScreen);
        }


        private void NotifyVolumeMusicChanged()
        {
            if (VolumeMusicChanged != null)
                VolumeMusicChanged(volumeMusic);
        }


        private void NotifyVolumeSfxChanged()
        {
            if (VolumeSfxChanged != null)
                VolumeSfxChanged(volumeSfx);
        }
    }
}
