namespace EphemereGames.Core.Audio
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Xna.Framework;


    class SoundEffectsController
    {
        private static SoundEffectsController instance = new SoundEffectsController();

        private Dictionary<string, SoundEffectsBank> effetsSonores = new Dictionary<string, SoundEffectsBank>();
        private Dictionary<string, Music> musiques = new Dictionary<string, Music>();

        public int MaxEffetsSonoresEnMemeTemps;
        private int EffetsSonoresInstancesActives;
        private Dictionary<string, int> EffetSonoreMaxInstances;


        public static SoundEffectsController Instance
        {
            get
            {
                return instance;
            }
        }

        private float volumeMusique;
        public float VolumeMusique
        {
            get { return volumeMusique; }

            set
            {
                volumeMusique = value;

                foreach (var kvp in musiques)
                    kvp.Value.Volume = value;
            }
        }

        private float volumeEffetsSonores;
        public float VolumeEffetsSonores
        {
            get { return volumeEffetsSonores; }

            set
            {
                volumeEffetsSonores = value;

                foreach (var kvp in effetsSonores)
                    kvp.Value.Volume = value;
            }
        }

        public Music getMusique(string nomMusique)
        {
            return musiques[nomMusique];
        }

        public void jouerEffetSonore(string nomBanque, string nomEffetSonore)
        {
            int maxInstancesActives = 0;

            if (EffetsSonoresInstancesActives >= MaxEffetsSonoresEnMemeTemps ||
                (EffetSonoreMaxInstances.TryGetValue(nomEffetSonore, out maxInstancesActives) && effetsSonores[nomBanque].EffetsSonores[nomEffetSonore].InstancesActives >= maxInstancesActives))
                return;

            effetsSonores[nomBanque].jouer(nomEffetSonore);
        }

        public void arreterEffetSonore(string nomBanque, string nomEffetSonore)
        {
            effetsSonores[nomBanque].arreter(nomEffetSonore, true, 100);
        }

        public SoundEffectsBank getEffetsSonores(string nomBanque)
        {
            return effetsSonores[nomBanque];
        }


        //=============================================================================
        // Ajouter des musiques et des effets sonores
        //=============================================================================

        public void setEffetSonoreMaxInstances(string effetSonore, int maxInstances)
        {

        }

        public void setMusique(string nomMusique, Music musique)
        {
            musiques[nomMusique] = musique;

            musique.Son.Name = nomMusique;
        }

        public void setMusique(string musiqueAffectee, string musiqueExistante)
        {
            musiques[musiqueAffectee].arreter(false, 0);

            musiques[musiqueExistante].Son.Name = musiqueAffectee;

            Microsoft.Xna.Framework.Audio.SoundEffect se = musiques[musiqueAffectee].Son;
            
            musiques[musiqueAffectee] = new Music();
            musiques[musiqueAffectee].Son = se;
        }


        public void setEffetSonore(string banque, SoundEffect effetSonore)
        {
            if (!effetsSonores.ContainsKey(banque))
                effetsSonores[banque] = new SoundEffectsBank();

            effetsSonores[banque].ajouter(effetSonore.Son.Name, effetSonore);
        }


        //=============================================================================
        // Initialisation
        //=============================================================================

        private SoundEffectsController()
        {
            // arrêter les musiques
            foreach (var kvp in musiques)
                kvp.Value.arreter(false, 0);

            // arrêter les effets sonores
            // appeler l'update des banques pour disposer des effets sonores
            foreach (var kvp in effetsSonores)
            {
                kvp.Value.arreter(false, 0);
                kvp.Value.Update(null);
            }

            effetsSonores.Clear();
            EffetsSonoresInstancesActives = 0;
            MaxEffetsSonoresEnMemeTemps = int.MaxValue;
            EffetSonoreMaxInstances = new Dictionary<string, int>();

            VolumeMusique = 0.5f;
            VolumeEffetsSonores = 0.5f;
        }


        //=============================================================================
        // Mise-à-jour de la logique
        //=============================================================================
 
        public void Update(GameTime gameTime)
        {
            string[] wCles;
            
            wCles = musiques.Keys.ToArray();

            for (int i = 0; i < wCles.Length; i++)
                musiques[wCles[i]].Update(gameTime);


            // gestion des effets sonores

            wCles = effetsSonores.Keys.ToArray();

            EffetsSonoresInstancesActives = 0;

            for (int i = 0; i < wCles.Length; i++)
            {
                effetsSonores[wCles[i]].Update(gameTime);
                EffetsSonoresInstancesActives += effetsSonores[wCles[i]].InstancesActives;
            }
        }


        public void setMaxInstancesActivesEffetSonore(string effetSonore, int maxInstances)
        {
            if (!EffetSonoreMaxInstances.ContainsKey(effetSonore))
                EffetSonoreMaxInstances.Add(effetSonore, maxInstances);
            else
                EffetSonoreMaxInstances[effetSonore] = maxInstances;
        }
    }
}
