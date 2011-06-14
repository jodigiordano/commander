namespace EphemereGames.Core.Audio
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Audio;
    using EphemereGames.Core.Persistence;
    

    public static class Audio
    {
        public static void Initialize(float volumeMusique, float volumeEffetsSonores)
        {
            Preferences.VolumeMusique = volumeMusique;
            Preferences.VolumeEffetsSonores = volumeEffetsSonores;

            SoundEffectsController.Instance.VolumeMusique = volumeMusique;
            SoundEffectsController.Instance.VolumeEffetsSonores = volumeEffetsSonores;

            Persistence.AddAssetType(new SoundEffect());
            Persistence.AddAssetType(new Music());
        }


        public static void PlaySfx(string bankName, string sfxName)
        {
            SoundEffectsController.Instance.jouerEffetSonore(bankName, sfxName);
        }


        public static void StopSfx(string bankName, string sfxName)
        {
            SoundEffectsController.Instance.arreterEffetSonore(bankName, sfxName);
        }

        public static bool IsSfxBankInUse(string bankName)
        {
            return SoundEffectsController.Instance.getEffetsSonores(bankName).Utilise;
        }


        public static void PlayMusic(string musicName, bool progressive, int fadeTime, bool loop)
        {
            SoundEffectsController.Instance.getMusique(musicName).jouer(progressive, fadeTime, loop);
        }


        public static void Update(GameTime gameTime)
        {
            SoundEffectsController.Instance.Update(gameTime);
        }


        public static void SetMaxSimultaneousSfx(int max)
        {
            SoundEffectsController.Instance.MaxEffetsSonoresEnMemeTemps = max;
        }


        public static void SetMaxInstancesSfx(string sfxName, int maxInstances)
        {
            SoundEffectsController.Instance.setMaxInstancesActivesEffetSonore(sfxName, maxInstances);
        }


        //todo: pas le bon comportement
        public static void UnpauseSfx(string sceneName, bool progressive, int fadeTime)
        {
            SoundEffectsController.Instance.getEffetsSonores(sceneName).reprendre(progressive, fadeTime);
        }


        //todo: pas le bon comportement
        public static void PauseSfx(string sceneName, bool progressive, int fadeTime)
        {
            SoundEffectsController.Instance.getEffetsSonores(sceneName).pauser(progressive, fadeTime);
        }


        public static void StopMusic(string musicName, bool progressive, int fadeTime)
        {
            SoundEffectsController.Instance.getMusique(musicName).arreter(progressive, fadeTime);
        }


        public static void PauseMusic(string musicName, bool progressive, int fadeTime)
        {
            SoundEffectsController.Instance.getMusique(musicName).pauser(progressive, fadeTime);
        }


        public static float MusicVolume
        {
            set
            {
                Preferences.VolumeMusique = value;
                SoundEffectsController.Instance.VolumeMusique = value;
            }
        }


        public static float SfxVolume
        {
            set
            {
                Preferences.VolumeEffetsSonores = value;
                SoundEffectsController.Instance.VolumeEffetsSonores = value;
            }
        }


        public static bool IsMusicPlaying(string musicName)
        {
            return SoundEffectsController.Instance.getMusique(musicName).Etat == SoundState.Playing;
        }


        public static void UnpauseMusic(string musicName, bool progressive, int fadeTime)
        {
            SoundEffectsController.Instance.getMusique(musicName).reprendre(progressive, fadeTime);
        }
    }
}
