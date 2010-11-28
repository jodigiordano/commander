//=====================================================================
//
// Gestionnaire de Scenes
// Helper class du main qui gère les scènes
//
//=====================================================================

namespace Core.Visuel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Core.Utilities;
#if DEBUG
    using System.Diagnostics;
#endif

    class GestionnaireScenes
    {

        //=====================================================================
        // Attributs
        //=====================================================================

        private static GestionnaireScenes instance;

        // Liste des scènes
        private Dictionary<string, Scene> scenes = new Dictionary<string, Scene>();

        // Liste des sous-scènes
        private Dictionary<string, string> sousScenes = new Dictionary<string, string>();

        // Liste des scènes à afficher par tick
        private List<Scene> scenesAafficher = new List<Scene>();

        // Enumerateur
        private Dictionary<String, Scene>.Enumerator iter;

        // Camera dans une scène bidon.
        // Pour rester dans un cas général au niveau de IVisible.
        // Donc pour l'instant, n'utiliser que la Caméra.
        private class ScenePrincipale : Scene
        {
            public ScenePrincipale() : base(Vector2.Zero, 1, 1)
            {
                Camera.Origine = new Vector2(Preferences.FenetreLargeur / 2.0f, Preferences.FenetreHauteur / 2.0f);
            }

            protected override void UpdateLogique(GameTime gameTime) {}
            protected override void UpdateVisuel() {}
        }

        ScenePrincipale scenePrincipale;


        //=====================================================================
        // Getters / Setters
        //=====================================================================

        public SpriteBatch SpriteBatch { get; set; }


        private GestionnaireScenes()
        {
            scenePrincipale = new ScenePrincipale();
            scenes.Clear();
            sousScenes.Clear();

            SpriteBatch = new SpriteBatch(Preferences.GraphicsDeviceManager.GraphicsDevice);
        }

        //=====================================================================
        // Gestion des scènes
        //=====================================================================

        /// <summary>
        /// Met à jour une scène.
        /// </summary>
        /// <param name="nomScene">Nom de la scène à updater.</param>
        /// <param name="scene">La nouvelle scène.</param>
        /// <returns>Une référence sur la scène updatée.</returns>

        public Scene mettreAJour(String nomScene, Scene scene)
        {
            Scene sceneActuelle = scenes[nomScene];

            // Remettre le tampon de la scène dans le pool de tampons disponibles
            if (sceneActuelle != null && sceneActuelle.EstProprietaireDeSonTampon)
                GestionnaireTampons.Instance.remettre(sceneActuelle.Tampon);

            scenes[nomScene] = scene;

            return scene;
        }


        //
        // Met a jour le focus d'une scène
        //

        public void majFocus(String nomScene, bool focus)
        {
            if (scenes[nomScene] != null)
                scenes[nomScene].EnFocus = focus;
        }


        //
        // Ajouter une scène
        // Lorsque la scène ne se trouve pas dans le gestionnaire.
        // Cette opération n'est effectuée qu'une seule fois pour chaque type de scène
        //

        public void ajouter(String nomScene, Scene scene)
        {
#if DEBUG
            if (scenes.ContainsKey(nomScene))
                throw new Exception("Les scènes ne doivent être ajoutées qu'une seule fois au gestionnaire.");
#endif

            scenes[nomScene] = scene;
            sousScenes[nomScene] = "";
        }


        //
        // Récupérer une scène
        // Un getter bien normal :)
        //

        public Scene recuperer(String nomScene)
        {
            if (!scenes.ContainsKey(nomScene))
                return null;

            return scenes[nomScene];
        }

        //
        // Détermine si une scène est en focus
        //

        public bool EnFocus(String nomScene)
        {
            if (scenes[nomScene] == null)
                return false;

            return scenes.ContainsKey(nomScene) && scenes[nomScene].EnFocus;
        }


        //=====================================================================
        // Dessiner les scènes (ce qui sera afficher à l'écran)
        //=====================================================================

        public void Draw() {

            // Vider la liste des scènes à afficher
            scenesAafficher.Clear();

            // Toutes les scènes actives se font tagger pour un update visuel
            iter = scenes.GetEnumerator();

            while (iter.MoveNext())
            {
                if (iter.Current.Value != null)
                    iter.Current.Value.EstUpdateCeTick = false;
            }
            
            // Toutes les sous-scènes actives doivent se mettre à jour
            mettreAJourScenes();

            // Trier les scènes actives en ordre de priorité d'affiche parce que le mode
            // Immediate ne le fait pas

            scenesAafficher.Sort(delegate(Scene s1, Scene s2)
            {
                // tri sur la position en Z
                if (s1.Position.Z > s2.Position.Z)
                    return 1;

                if (s1.Position.Z < s2.Position.Z)
                    return -1;

                // tri sur la priorité d'affichage pour un même Z
                if (s1.PrioriteAffichage < s2.PrioriteAffichage)
                    return 1;

                if (s1.PrioriteAffichage > s2.PrioriteAffichage)
                    return -1;

                // tri final sur le hash code
                if (s1.GetHashCode() > s2.GetHashCode())
                    return 1;

                if (s1.GetHashCode() < s2.GetHashCode())
                    return -1;

                // impossible de se rendre ici :)
                // mais pas d'exceptions levés parce que sa pourrait être deux fois le même objet
                return 0;
            });


            // Effacer l'écran
            Preferences.GraphicsDeviceManager.GraphicsDevice.SetRenderTarget(0, null);
            Preferences.GraphicsDeviceManager.GraphicsDevice.Clear(Color.Black);

            // Affichage des scènes dans le back buffer (à l'écran)
            SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None, scenePrincipale.Camera.Transformee);

            Preferences.GraphicsDeviceManager.GraphicsDevice.RenderState.SourceBlend = Blend.One;
            Preferences.GraphicsDeviceManager.GraphicsDevice.RenderState.DestinationBlend = Blend.InverseSourceAlpha;

            for (int i = 0; i < scenesAafficher.Count; i++)
            {
                ((IVisible)scenesAafficher[i]).Scene = scenePrincipale;
                scenesAafficher[i].Draw(SpriteBatch);
            }

            SpriteBatch.End();

            // Post-processing
            if (Preferences.Luminosite != 0 || Preferences.Contraste != 0)
            {
                PostProcessing.appliquerLuminositeContraste(
                    Preferences.Luminosite,
                    Preferences.Contraste);
            }
        }


        //=====================================================================
        // Questionner le gestionnaire
        //=====================================================================

        //
        // Est-ce que la scène peut mettre à jour sa logique
        //

        public bool peutMettreAJour(Scene scene)
        {
            return scene != null && !scene.EnPause;
        }


        //=====================================================================
        // Mettre à jour le gestionnaire
        //=====================================================================

        public void Update(GameTime gameTime)
        {
            // coute pas cher
            //restructurerSousScenes();

            String[] wCles = scenes.Keys.ToArray();

            for (int i = 0; i < wCles.Length; i++)
            {
                Scene scene = scenes[wCles[i]];

                if (peutMettreAJour(scene))
                    scene.Update(gameTime);
            }
        }


        //=====================================================================
        // Arrêter toutes les scènes 
        //=====================================================================

        public void ToutesArretees()
        {
            foreach (var kvp in scenes)
            {
                if (kvp.Value != null)
                {
                    kvp.Value.EnPause = true;
                    kvp.Value.EstVisible = false;
                }
            }
        }


        public static GestionnaireScenes Instance
        {
            get
            {
                if (instance == null)
                    instance = new GestionnaireScenes();

                return instance;
            }
        }


        //=====================================================================
        // Helpers
        //=====================================================================

        private void mettreAJourScenes()
        {
            int niveau = -1;
            int dernierNiveauVisible = -1;

            // Trouver le niveau le plus haut
            iter = scenes.GetEnumerator();

            while (iter.MoveNext())
                if (iter.Current.Value != null)
                    niveau = (int) MathHelper.Max(niveau, iter.Current.Value.Niveau);

            // Remonter les niveaux en demandant aux scènes de celui-ci de se mettre à jour
            for (int i = niveau; i >= 0; i--)
            {
                iter = scenes.GetEnumerator();

                while (iter.MoveNext())
                {
                    var sousScene = iter.Current.Value;

                    if (sousScene != null && sousScene.EstVisible && sousScene.Niveau == i)
                    {
                        sousScene.Draw();
                        dernierNiveauVisible = i;
                    }
                }
            }

            iter = scenes.GetEnumerator();

            while (iter.MoveNext())
            {
                var sousScene = iter.Current.Value;

                if (sousScene != null && sousScene.EstVisible && sousScene.Niveau == dernierNiveauVisible)
                    scenesAafficher.Add(sousScene);
            }
        }
    }
}
