namespace EphemereGames.Core.Visuel
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using EphemereGames.Core.Utilities;

    public class GestionnaireParticules
    {
        private Dictionary<string, Pool<ParticuleEffectWrapper>> listePoolParticules;
        private ProjectMercury.Renderers.SpriteBatchRenderer renderer;
        private List<KeyValuePair<string, ParticuleEffectWrapper>> listeParticulesActives;
        private List<KeyValuePair<string, ParticuleEffectWrapper>> listeParticulesMourantes;
        private List<KeyValuePair<string, ParticuleEffectWrapper>> listeParticulesActivesASupprimer;
        private Scene Scene;
        private List<IScenable> composants = new List<IScenable>();
        private GameTime _gameTime;


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


        public void ajouter(string nom)
        {
            if (!listePoolParticules.ContainsKey(nom))
                listePoolParticules.Add(nom, new Pool<ParticuleEffectWrapper>());
        }


        public void Add(List<string> names, bool hard)
        {
            if (hard)
                vider();

            foreach (var name in names)
                ajouter(name);
        }


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


        public void retourner(ParticuleEffectWrapper particule)
        {
            listeParticulesActivesASupprimer.Add(new KeyValuePair<string, ParticuleEffectWrapper>(particule.Nom, particule));

            if (particule.ParticleEffect.ActiveParticlesCount != 0)
                listeParticulesMourantes.Add(new KeyValuePair<string, ParticuleEffectWrapper>(particule.Nom, particule));
            else
                listePoolParticules[particule.Nom].retourner(particule);
        }


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
