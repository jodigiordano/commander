namespace EphemereGames.Core.Audio
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Audio;
    using System.Linq;

    class GestionnaireSons
    {

        //=============================================================================
        // Attributs
        //=============================================================================

        private static GestionnaireSons instance = new GestionnaireSons();

        private Dictionary<String, BanqueEffetsSonores> effetsSonores = new Dictionary<string, BanqueEffetsSonores>();
        private Dictionary<String, Musique> musiques = new Dictionary<string, Musique>();

        public int MaxEffetsSonoresEnMemeTemps;
        private int EffetsSonoresInstancesActives;
        private Dictionary<String, int> EffetSonoreMaxInstances;


        //=============================================================================
        // Services
        //=============================================================================

        public static GestionnaireSons Instance
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

        public Musique getMusique(String nomMusique)
        {
            return musiques[nomMusique];
        }

        public void jouerEffetSonore(String nomBanque, String nomEffetSonore)
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

        public BanqueEffetsSonores getEffetsSonores(String nomBanque)
        {
            return effetsSonores[nomBanque];
        }


        //=============================================================================
        // Ajouter des musiques et des effets sonores
        //=============================================================================

        public void setEffetSonoreMaxInstances(String effetSonore, int maxInstances)
        {

        }

        public void setMusique(String nomMusique, Musique musique)
        {
            musiques[nomMusique] = musique;

            musique.Son.Name = nomMusique;
        }

        public void setMusique(String musiqueAffectee, String musiqueExistante)
        {
            musiques[musiqueAffectee].arreter(false, 0);

            musiques[musiqueExistante].Son.Name = musiqueAffectee;
            
            SoundEffect se = musiques[musiqueAffectee].Son;
            
            musiques[musiqueAffectee] = new Musique();
            musiques[musiqueAffectee].Son = se;
        }


        public void setEffetSonore(String banque, EffetSonore effetSonore)
        {
            if (!effetsSonores.ContainsKey(banque))
                effetsSonores[banque] = new BanqueEffetsSonores();

            effetsSonores[banque].ajouter(effetSonore.Son.Name, effetSonore);
        }


        //=============================================================================
        // Initialisation
        //=============================================================================

        private GestionnaireSons()
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
            String[] wCles;
            
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
