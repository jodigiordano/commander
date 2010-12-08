namespace Core.Visuel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    class GestionnaireScenes
    {
        private static GestionnaireScenes instance;
        private Dictionary<string, Scene> scenes = new Dictionary<string, Scene>();
        private Dictionary<string, string> sousScenes = new Dictionary<string, string>();
        private List<Scene> scenesAafficher = new List<Scene>();
        private Dictionary<String, Scene>.Enumerator iter;
        private Camera Camera = new Camera();

        public SpriteBatch SpriteBatch { get; set; }
        public Tampon Tampon;


        private GestionnaireScenes()
        {
            Camera.Origine = new Vector2(Preferences.FenetreLargeur / 2.0f, Preferences.FenetreHauteur / 2.0f);
            scenes.Clear();
            sousScenes.Clear();

            SpriteBatch = new SpriteBatch(Preferences.GraphicsDeviceManager.GraphicsDevice);
        }


        public Scene mettreAJour(String nomScene, Scene scene)
        {
            Scene sceneActuelle = scenes[nomScene];

            if (sceneActuelle != null)
                sceneActuelle.Dispose();

            scenes[nomScene] = scene;

            return scene;
        }


        public void majFocus(String nomScene, bool focus)
        {
            if (scenes[nomScene] != null)
                scenes[nomScene].EnFocus = focus;
        }


        public void ajouter(String nomScene, Scene scene)
        {
#if DEBUG
            if (scenes.ContainsKey(nomScene))
                throw new Exception("Les scènes ne doivent être ajoutées qu'une seule fois au gestionnaire.");
#endif

            scenes[nomScene] = scene;
            sousScenes[nomScene] = "";
        }


        public Scene recuperer(String nomScene)
        {
            if (!scenes.ContainsKey(nomScene))
                return null;

            return scenes[nomScene];
        }


        public bool EnFocus(String nomScene)
        {
            if (scenes[nomScene] == null)
                return false;

            return scenes.ContainsKey(nomScene) && scenes[nomScene].EnFocus;
        }


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
            SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None, Camera.Transformee);

            Preferences.GraphicsDeviceManager.GraphicsDevice.RenderState.SourceBlend = Blend.One;
            Preferences.GraphicsDeviceManager.GraphicsDevice.RenderState.DestinationBlend = Blend.InverseSourceAlpha;

            for (int i = 0; i < scenesAafficher.Count; i++)
            {
                scenesAafficher[i].Draw(SpriteBatch);
            }

            SpriteBatch.End();
        }


        public bool peutMettreAJour(Scene scene)
        {
            return scene != null && !scene.EnPause;
        }


        public void Update(GameTime gameTime)
        {
            String[] wCles = scenes.Keys.ToArray();

            for (int i = 0; i < wCles.Length; i++)
            {
                Scene scene = scenes[wCles[i]];

                if (peutMettreAJour(scene))
                    scene.Update(gameTime);
            }
        }


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
