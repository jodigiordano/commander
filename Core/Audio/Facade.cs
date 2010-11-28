//=============================================================================
//
// Point d'entrée dans la librairie
//
//=============================================================================

namespace Core.Audio
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Audio;
    
    public static class Facade
    {
        public static void Initialize(
            float volumeMusique,
            float volumeEffetsSonores)
        {
            Preferences.VolumeMusique = volumeMusique;
            Preferences.VolumeEffetsSonores = volumeEffetsSonores;

            GestionnaireSons.Instance.VolumeMusique = volumeMusique;
            GestionnaireSons.Instance.VolumeEffetsSonores = volumeEffetsSonores;

            Core.Persistance.Facade.ajouterTypeAsset(new EffetSonore());
            Core.Persistance.Facade.ajouterTypeAsset(new Musique());
        }

        public static void jouerEffetSonore(string nomBanque, string nomEffetSonore)
        {
            GestionnaireSons.Instance.jouerEffetSonore(nomBanque, nomEffetSonore);
        }

        public static bool banqueEffetsSonoreUtilisee(string nomBanque)
        {
            return GestionnaireSons.Instance.getEffetsSonores(nomBanque).Utilise;
        }

        public static void jouerMusique(string nomMusique, bool apparitionProgressive, int tempsFade, bool loop)
        {
            GestionnaireSons.Instance.getMusique(nomMusique).jouer(apparitionProgressive, tempsFade, loop);
        }

        public static void Update(GameTime gameTime)
        {
            GestionnaireSons.Instance.Update(gameTime);
        }

        public static void setMaxEffetsSonoresEnMemeTemps(int max)
        {
            GestionnaireSons.Instance.MaxEffetsSonoresEnMemeTemps = max;
        }

        public static void setMaxInstancesActivesEffetSonore(String effetSonore, int maxInstances)
        {
            GestionnaireSons.Instance.setMaxInstancesActivesEffetSonore(effetSonore, maxInstances);
        }

        //todo: pas le bon comportement
        public static void reprendreEffetsSonores(string nomScene, bool apparitionProgressive, int tempsFade)
        {
            GestionnaireSons.Instance.getEffetsSonores(nomScene).reprendre(apparitionProgressive, tempsFade);
        }

        //todo: pas le bon comportement
        public static void pauserEffetsSonores(string nomScene, bool disparitionProgressive, int tempsFade)
        {
            GestionnaireSons.Instance.getEffetsSonores(nomScene).pauser(disparitionProgressive, tempsFade);
        }

        public static void arreterMusique(string nomMusique, bool disparitionProgressive, int tempsFade)
        {
            GestionnaireSons.Instance.getMusique(nomMusique).arreter(disparitionProgressive, tempsFade);
        }

        public static void pauserMusique(string nomMusique, bool disparitionProgessive, int tempsFade)
        {
            GestionnaireSons.Instance.getMusique(nomMusique).pauser(disparitionProgessive, tempsFade);
        }

        public static float VolumeMusique
        {
            set
            {
                Preferences.VolumeMusique = value;
                GestionnaireSons.Instance.VolumeMusique = value;
            }
        }

        public static float VolumeEffetsSonores
        {
            set
            {
                Preferences.VolumeEffetsSonores = value;
                GestionnaireSons.Instance.VolumeEffetsSonores = value;
            }
        }

        public static bool musiqueJoue(string nomMusique)
        {
            return GestionnaireSons.Instance.getMusique(nomMusique).Etat == SoundState.Playing;
        }

        public static void reprendreMusique(string nomMusique, bool apparitionProgressive, int tempsFade)
        {
            GestionnaireSons.Instance.getMusique(nomMusique).reprendre(apparitionProgressive, tempsFade);
        }
    }
}
