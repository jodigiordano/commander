namespace EphemereGames.Core.Audio
{
    using EphemereGames.Core.SimplePersistence;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Audio;
    

    public static class Audio
    {
        internal static AudioController AudioController = new AudioController();


        public static void Initialize(float musicVolume, float sfxVolume)
        {
            Preferences.MusicVolume = musicVolume;
            Preferences.SfxVolume = sfxVolume;

            AudioController.MusicVolume = musicVolume;
            AudioController.SfxVolume = sfxVolume;

            Persistence.AddAssetType(new SoundEffect());
            Persistence.AddAssetType(new Music());
        }


        public static void SetActiveSfxBank(string bankName)
        {
            AudioController.ActiveBank = bankName;
        }


        public static int PlaySfx(string sfxName, bool loop)
        {
            return AudioController.PlaySfx(sfxName, loop);
        }


        //public static void PlaySfx(string bankName, string sfxName)
        //{
        //    SoundEffectsController.PlaySfx(bankName, sfxName);
        //}


        public static void PlaySfx(string sfxName)
        {
            AudioController.PlaySfx(sfxName);
        }


        public static void StopSfx(string sfxName, int id)
        {
            AudioController.StopSfx(sfxName, id);
        }


        public static void StopSfx(string sfxName)
        {
            AudioController.StopSfx(sfxName);
        }


        //public static void StopSfx(string bankName, string sfxName)
        //{
        //    AudioController.StopSfx(bankName, sfxName);
        //}

        public static bool IsSfxBankInUse(string bankName)
        {
            return AudioController.GetSfxBank(bankName).InUse;
        }


        public static void PlayMusic(string musicName, bool progressive, int fadeTime, NoneHandler callback, double timeBeforeEnd)
        {
            AudioController.GetMusic(musicName).Play(progressive, fadeTime, callback, timeBeforeEnd);
        }


        public static void PlayMusic(string musicName, bool progressive, int fadeTime, bool loop)
        {
            AudioController.GetMusic(musicName).Play(progressive, fadeTime, loop);
        }


        public static void Update(GameTime gameTime)
        {
            AudioController.Update(gameTime);
        }


        public static void SetMaxSimultaneousSfx(int max)
        {
            AudioController.MaxSimultaneousSfx = max;
        }


        public static void SetMaxInstancesSfx(string sfxName, int maxInstances)
        {
            AudioController.SetMaxActivesSfxInstances(sfxName, maxInstances);
        }


        public static void StopMusic(string musicName, bool progressive, int fadeTime)
        {
            AudioController.GetMusic(musicName).Stop(progressive, fadeTime);
        }


        public static void PauseMusic(string musicName, bool progressive, int fadeTime)
        {
            AudioController.GetMusic(musicName).Pause(progressive, fadeTime);
        }


        public static float MusicVolume
        {
            set
            {
                Preferences.MusicVolume = value;
                AudioController.MusicVolume = value;
            }
        }


        public static float SfxVolume
        {
            set
            {
                Preferences.SfxVolume = value;
                AudioController.SfxVolume = value;
            }
        }


        public static bool IsMusicPlaying(string musicName)
        {
            return AudioController.GetMusic(musicName).State == SoundState.Playing;
        }


        public static void ResumeMusic(string musicName, bool progressive, int fadeTime)
        {
            AudioController.GetMusic(musicName).Resume(progressive, fadeTime);
        }
    }
}
