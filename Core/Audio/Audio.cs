namespace EphemereGames.Core.Audio
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Audio;
    
    public static class Audio
    {
        public static void Initialize(
            float volumeMusique,
            float volumeEffetsSonores)
        {
            Preferences.VolumeMusique = volumeMusique;
            Preferences.VolumeEffetsSonores = volumeEffetsSonores;

            SoundEffectsController.Instance.VolumeMusique = volumeMusique;
            SoundEffectsController.Instance.VolumeEffetsSonores = volumeEffetsSonores;

            EphemereGames.Core.Persistence.Persistence.AddAssetType(new SoundEffect());
            EphemereGames.Core.Persistence.Persistence.AddAssetType(new Music());
        }

        public static void jouerEffetSonore(string nomBanque, string nomEffetSonore)
        {
            SoundEffectsController.Instance.jouerEffetSonore(nomBanque, nomEffetSonore);
        }

        public static void arreterEffetSonore(string nomBanque, string nomEffetSonore)
        {
            SoundEffectsController.Instance.arreterEffetSonore(nomBanque, nomEffetSonore);
        }

        public static bool banqueEffetsSonoreUtilisee(string nomBanque)
        {
            return SoundEffectsController.Instance.getEffetsSonores(nomBanque).Utilise;
        }

        public static void jouerMusique(string nomMusique, bool apparitionProgressive, int tempsFade, bool loop)
        {
            SoundEffectsController.Instance.getMusique(nomMusique).jouer(apparitionProgressive, tempsFade, loop);
        }

        public static void Update(GameTime gameTime)
        {
            SoundEffectsController.Instance.Update(gameTime);
        }

        public static void setMaxEffetsSonoresEnMemeTemps(int max)
        {
            SoundEffectsController.Instance.MaxEffetsSonoresEnMemeTemps = max;
        }

        public static void setMaxInstancesActivesEffetSonore(string effetSonore, int maxInstances)
        {
            SoundEffectsController.Instance.setMaxInstancesActivesEffetSonore(effetSonore, maxInstances);
        }

        //todo: pas le bon comportement
        public static void reprendreEffetsSonores(string nomScene, bool apparitionProgressive, int tempsFade)
        {
            SoundEffectsController.Instance.getEffetsSonores(nomScene).reprendre(apparitionProgressive, tempsFade);
        }

        //todo: pas le bon comportement
        public static void pauserEffetsSonores(string nomScene, bool disparitionProgressive, int tempsFade)
        {
            SoundEffectsController.Instance.getEffetsSonores(nomScene).pauser(disparitionProgressive, tempsFade);
        }

        public static void arreterMusique(string nomMusique, bool disparitionProgressive, int tempsFade)
        {
            SoundEffectsController.Instance.getMusique(nomMusique).arreter(disparitionProgressive, tempsFade);
        }

        public static void pauserMusique(string nomMusique, bool disparitionProgessive, int tempsFade)
        {
            SoundEffectsController.Instance.getMusique(nomMusique).pauser(disparitionProgessive, tempsFade);
        }

        public static float VolumeMusique
        {
            set
            {
                Preferences.VolumeMusique = value;
                SoundEffectsController.Instance.VolumeMusique = value;
            }
        }

        public static float VolumeEffetsSonores
        {
            set
            {
                Preferences.VolumeEffetsSonores = value;
                SoundEffectsController.Instance.VolumeEffetsSonores = value;
            }
        }

        public static bool musiqueJoue(string nomMusique)
        {
            return SoundEffectsController.Instance.getMusique(nomMusique).Etat == SoundState.Playing;
        }

        public static void reprendreMusique(string nomMusique, bool apparitionProgressive, int tempsFade)
        {
            SoundEffectsController.Instance.getMusique(nomMusique).reprendre(apparitionProgressive, tempsFade);
        }
    }
}
