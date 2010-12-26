//=====================================================================
//
// Gestionnaire de particules
// Doit être utilisé comme un Pool. Donc pour chaque "récupérer" doit
// correspondre un "retourner"
//
//=====================================================================

namespace EphemereGames.Core.Visuel
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using EphemereGames.Core.Utilities;

    public class GestionnaireParticules
    {

        //=====================================================================
        // Attributs
        //=====================================================================

        private Dictionary<string, Pool<ParticuleEffectWrapper>> listePoolParticules;            // Liste des pools de particules
        private ProjectMercury.Renderers.SpriteBatchRenderer renderer;                           // S'occupe de dessiner les particules à l'écran
        private List<KeyValuePair<string, ParticuleEffectWrapper>> listeParticulesActives;       // Liste des particules actives à mettre à jour
        private List<KeyValuePair<string, ParticuleEffectWrapper>> listeParticulesMourantes;     // Liste des particules orphelines qui ne s'updatent plus
        private List<KeyValuePair<string, ParticuleEffectWrapper>> listeParticulesActivesASupprimer;
        private Scene Scene;


        //=====================================================================
        // Constructeur
        //=====================================================================

        public GestionnaireParticules(Scene scene)
        {
            listePoolParticules = new Dictionary<string, Pool<ParticuleEffectWrapper>>();
            listeParticulesActives = new List<KeyValuePair<string, ParticuleEffectWrapper>>();
            listeParticulesActivesASupprimer = new List<KeyValuePair<string, ParticuleEffectWrapper>>();
            listeParticulesMourantes = new List<KeyValuePair<string, ParticuleEffectWrapper>>();


            renderer = new ProjectMercury.Renderers.SpriteBatchRenderer
            {
                GraphicsDeviceService = Preferences.GraphicsDeviceManager
            };

            renderer.LoadContent(Preferences.Content);

            this.Scene = scene;
        }
        

        //=====================================================================
        // Services
        //=====================================================================

        /// <summary>
        /// Les systèmes de particules présentement gérés par le gestionnaire
        /// </summary>
        private List<IScenable> composants = new List<IScenable>();
        public List<IScenable> Composants
        {
            get
            {
                composants.Clear();

                for (int i = 0; i < listeParticulesActives.Count; i++)
                    composants.Add(listeParticulesActives[i].Value);

                for (int i = 0; i < listeParticulesMourantes.Count; i++)
                    composants.Add(listeParticulesMourantes[i].Value);

                return composants;
            }
        }


        /// <summary>
        /// Ajoute un pool de systèmes de particules dans le gestionnaire
        /// </summary>
        /// <param name="nom">Nom du système de particules</param>
        /// <param name="pool">Un pool de systèmes de particules</param>
        /// <remarks>
        /// Si le pool de systèmes de particules est déjà dans le gestionnaire, il n'est pas ajouté de nouveau
        /// </remarks>
        public void ajouter(string nom)
        {
            if (!listePoolParticules.ContainsKey(nom))
                listePoolParticules.Add(nom, new Pool<ParticuleEffectWrapper>());
        }


        /// <summary>
        /// Fourni un système de particules à l'appelant
        /// </summary>
        /// <param name="nom">Nom du système de particules</param>
        /// <returns>Le système de particules demandé</returns>
        public ParticuleEffectWrapper recuperer(string nom)
        {
            ParticuleEffectWrapper particule = listePoolParticules[nom].recuperer();

            //tmp. ARK!
            if (particule == null)
            {
                ParticuleEffectWrapper particule2 = new ParticuleEffectWrapper();
                particule2.Scene = Scene;
                particule2.Nom = nom;
                particule2.Renderer = this.renderer;
                particule2.Initialize();

                return particule2;
            }

            particule.Scene = Scene;
            particule.Nom = nom;
            particule.Renderer = this.renderer;
            particule.Initialize();

            listeParticulesActives.Add(new KeyValuePair<string, ParticuleEffectWrapper>(nom, particule));

            return particule;
        }


        /// <summary>
        /// L'appelant retourne un système de particules
        /// </summary>
        /// <param name="particule">Le système de particules retourné</param>
        public void retourner(ParticuleEffectWrapper particule)
        {
            listeParticulesActivesASupprimer.Add(new KeyValuePair<string, ParticuleEffectWrapper>(particule.Nom, particule));

            if (particule.ParticleEffect.ActiveParticlesCount != 0)
                listeParticulesMourantes.Add(new KeyValuePair<string, ParticuleEffectWrapper>(particule.Nom, particule));
            else
                listePoolParticules[particule.Nom].retourner(particule);
        }


        /// <summary>
        /// Vide le gestionnaire
        /// </summary>
        public void vider()
        {
            //this = new GestionnaireParticules(Scene);

            foreach (var paire in listeParticulesActives)
            {
                //paire.Value.ParticleEffect.Update(float.MaxValue);
                listePoolParticules[paire.Key].retourner(paire.Value);
            }

            foreach (var paire in listeParticulesMourantes)
            {
                //paire.Value.ParticleEffect.Update(float.MaxValue);
                listePoolParticules[paire.Key].retourner(paire.Value);
            }

            listePoolParticules.Clear();
            listeParticulesActives.Clear();
            listeParticulesMourantes.Clear();
        }


        private GameTime _gameTime;

        /// <summary>
        /// Mise-à-jour du gestionnaire
        /// </summary>
        /// <param name="gameTime">Le temps</param>
        public void Update(GameTime gameTime)
        {
            _gameTime = gameTime;

//#if XBOX
//            // Mise à jour des particules actives
//            Preferences.ThreadParticules.AddTask(new ThreadTask(doUpdatePremiereMoitiee));

//            doUpdateDeuxiemeMoitiee();
//            doEnleverParticulesMourantes();
//            doEnleverParticulesActives();
//#else
            doTmpVerifierNulls();
            doUpdatePremiereMoitiee();
            doUpdateDeuxiemeMoitiee();
            doEnleverParticulesMourantes();
            doEnleverParticulesActives();
//#endif
        }

        private void doEnleverParticulesActives()
        {
            lock (listeParticulesActivesASupprimer)
            {
                for (int i = 0; i < listeParticulesActivesASupprimer.Count; i++)
                    listeParticulesActives.Remove(listeParticulesActivesASupprimer[i]);

                listeParticulesActivesASupprimer.Clear();
            }
        }

        private void doEnleverParticulesMourantes()
        {
            for (int i = listeParticulesMourantes.Count - 1; i > -1; i--)
            {
                if (listeParticulesMourantes[i].Value.ParticleEffect.ActiveParticlesCount == 0)
                {
                    listePoolParticules[listeParticulesMourantes[i].Key].retourner(listeParticulesMourantes[i].Value);
                    listeParticulesMourantes.RemoveAt(i);
                }
            }
        }

        private void doUpdatePremiereMoitiee()
        {
            doUpdateAsynchrone(listeParticulesActives, 0, listeParticulesActives.Count / 2, _gameTime);
            doUpdateAsynchrone(listeParticulesMourantes, 0, listeParticulesMourantes.Count / 2, _gameTime);
        }

        private void doUpdateDeuxiemeMoitiee()
        {
            doUpdateAsynchrone(listeParticulesActives, listeParticulesActives.Count / 2, listeParticulesActives.Count, _gameTime);
            doUpdateAsynchrone(listeParticulesMourantes, listeParticulesMourantes.Count / 2, listeParticulesMourantes.Count, _gameTime);
        }

        private void doUpdateAsynchrone(List<KeyValuePair<string, ParticuleEffectWrapper>> particules, int i, int j, GameTime gameTime)
        {
            for (int k = i; k < j; k++)
                particules[k].Value.ParticleEffect.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
        }

        private void doTmpVerifierNulls()
        {
            for (int i = listeParticulesActives.Count - 1; i > -1; i--)
                if (listeParticulesActives[i].Key == null || listeParticulesActives[i].Value == null)
                    listeParticulesActives.RemoveAt(i);

            for (int i = listeParticulesMourantes.Count - 1; i > -1; i--)
                if (listeParticulesMourantes[i].Key == null || listeParticulesMourantes[i].Value == null)
                    listeParticulesMourantes.RemoveAt(i);
        }
    }
}
