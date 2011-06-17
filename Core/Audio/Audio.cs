namespace EphemereGames.Core.Audio
{
    using EphemereGames.Core.Persistence;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Audio;
    

    public static class Audio
    {
        internal static SoundEffectsController SoundEffectsController = new SoundEffectsController();


        public static void Initialize(float volumeMusique, float volumeEffetsSonores)
        {
            Preferences.VolumeMusique = volumeMusique;
            Preferences.VolumeEffetsSonores = volumeEffetsSonores;

            SoundEffectsController.MusicVolume = volumeMusique;
            SoundEffectsController.SfxVolume = volumeEffetsSonores;

            Persistence.AddAssetType(new SoundEffect());
            Persistence.AddAssetType(new Music());
        }


        public static void PlaySfx(string bankName, string sfxName)
        {
            SoundEffectsController.PlaySfx(bankName, sfxName);
        }


        public static void StopSfx(string bankName, string sfxName)
        {
            SoundEffectsController.StopSfx(bankName, sfxName);
        }

        public static bool IsSfxBankInUse(string bankName)
        {
            return SoundEffectsController.GetSfxBank(bankName).InUse;
        }


        public static void PlayMusic(string musicName, bool progressive, int fadeTime, bool loop)
        {
            SoundEffectsController.GetMusic(musicName).jouer(progressive, fadeTime, loop);
        }


        public static void Update(GameTime gameTime)
        {
            SoundEffectsController.Update(gameTime);
        }


        public static void SetMaxSimultaneousSfx(int max)
        {
            SoundEffectsController.MaxSimultaneousSfx = max;
        }


        public static void SetMaxInstancesSfx(string sfxName, int maxInstances)
        {
            SoundEffectsController.SetMaxActivesSfxInstances(sfxName, maxInstances);
        }


        public static void StopMusic(string musicName, bool progressive, int fadeTime)
        {
            SoundEffectsController.GetMusic(musicName).Stop(progressive, fadeTime);
        }


        public static void PauseMusic(string musicName, bool progressive, int fadeTime)
        {
            SoundEffectsController.GetMusic(musicName).Pause(progressive, fadeTime);
        }


        public static float MusicVolume
        {
            set
            {
                Preferences.VolumeMusique = value;
                SoundEffectsController.MusicVolume = value;
            }
        }


        public static float SfxVolume
        {
            set
            {
                Preferences.VolumeEffetsSonores = value;
                SoundEffectsController.SfxVolume = value;
            }
        }


        public static bool IsMusicPlaying(string musicName)
        {
            return SoundEffectsController.GetMusic(musicName).State == SoundState.Playing;
        }


        public static void ResumeMusic(string musicName, bool progressive, int fadeTime)
        {
            SoundEffectsController.GetMusic(musicName).Unpause(progressive, fadeTime);
        }
    }
}
