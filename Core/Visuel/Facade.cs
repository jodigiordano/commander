//=============================================================================
//
// Point d'entrée dans la librairie visuelle
//
//=============================================================================



namespace Core.Visuel
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Content;
    using Core.Utilities;
    using System.Threading;

    public static class Facade
    {
       
        public static void Initialize(
            GraphicsDeviceManager graphicsDeviceManager,
            int fenetreLargeur,
            int fenetreHauteur,
            ContentManager content,
            float luminosite,
            float contraste,
            String[] nomsScenes,
            Vector2[] tampons,
            ManagedThread thread1,
            ManagedThread thread2)
        {
            Preferences.GraphicsDeviceManager = graphicsDeviceManager;
            Preferences.FenetreLargeur = fenetreLargeur;
            Preferences.FenetreHauteur = fenetreHauteur;
            Preferences.Content = content;
            Preferences.Luminosite = luminosite;
            Preferences.Contraste = contraste;
            Preferences.ThreadParticules = thread1;
            Preferences.ThreadLogique = thread2;

            graphicsDeviceManager.GraphicsDevice.DepthStencilBuffer = null;

            // Anti-aliasing
            graphicsDeviceManager.GraphicsDevice.PresentationParameters.MultiSampleQuality = 0;
            graphicsDeviceManager.GraphicsDevice.PresentationParameters.MultiSampleType = MultiSampleType.None;
            graphicsDeviceManager.GraphicsDevice.RenderState.MultiSampleAntiAlias = false;

            graphicsDeviceManager.GraphicsDevice.PresentationParameters.SwapEffect = SwapEffect.Discard;
            graphicsDeviceManager.GraphicsDevice.PresentationParameters.BackBufferFormat = SurfaceFormat.Color;
            graphicsDeviceManager.GraphicsDevice.RenderState.DepthBufferEnable = false;
            graphicsDeviceManager.GraphicsDevice.RenderState.DepthBufferWriteEnable = false;
            graphicsDeviceManager.GraphicsDevice.RenderState.StencilEnable = false;
            graphicsDeviceManager.GraphicsDevice.RenderState.TwoSidedStencilMode = false;

            graphicsDeviceManager.GraphicsDevice.PresentationParameters.PresentationInterval = PresentInterval.Default;

            Scene.Contenu = Preferences.Content;

            for (int i = 0; i < nomsScenes.Length; i++)
                GestionnaireScenes.Instance.ajouter(nomsScenes[i], null);

            Primitives.init(Preferences.Content.Load<Texture2D>("pixelBlanc"));
            PostProcessing.init();

            Core.Persistance.Facade.ajouterTypeAsset(new ParticuleEffectWrapper());
            Core.Persistance.Facade.ajouterTypeAsset(new Sprite());
            Core.Persistance.Facade.ajouterTypeAsset(new Transition());

            foreach (var tampon in tampons)
                GestionnaireTampons.Instance.ajouter((int) tampon.X, (int) tampon.Y);
        }

        public static void ajouterTransition(String nomTransition, Transition transition)
        {
            GestionnaireTransitions.Instance.ajouter(nomTransition, transition);
        }

        /// <summary>
        /// Effectuer une transition entre deux ou plusieurs scènes
        /// </summary>
        /// <param name="nomTransition">Nom de la transition</param>
        public static void effectuerTransition(String nomTransition)
        {
            GestionnaireTransitions.Instance.transition(nomTransition);
        }


        /// <summary>
        /// Etre notifier lorsqu'une transition est démarrée et se termine
        /// </summary>
        /// <param name="handlerDemarree">Méthode qui recoit la notification que la transition est démarrée</param>
        /// <param name="handlerTerminee">Méthode qui recoit la notification que la transition est terminée</param>
        public static void etreNotifierTransition(EventHandler handlerDemarree, EventHandler handlerTerminee)
        {
            GestionnaireTransitions.Instance.TransitionDemarree += handlerDemarree;
            GestionnaireTransitions.Instance.TransitionTerminee += handlerTerminee;
        }

        public static bool TransitionEnCours
        {
            get { return GestionnaireTransitions.Instance.EnTransition; }
        }

        public static Scene recupererScene(String nomScene)
        {
            return GestionnaireScenes.Instance.recuperer(nomScene);
        }

        public static void Update(GameTime gameTime)
        {
            GestionnaireScenes.Instance.Update(gameTime);

            if (GestionnaireTransitions.Instance.EnTransition)
                GestionnaireTransitions.Instance.Update(gameTime);
        }

        public static void mettreAJourScene(string nomScene, Scene scene)
        {
            GestionnaireScenes.Instance.mettreAJour(nomScene, scene);
        }

        //public static void mettreAJourSousScene(string nomSousScene, string nomSceneParent)
        //{
        //    GestionnaireScenes.Instance.mettreAJourSousScene(nomSousScene, nomSceneParent);
        //}

        public static void Draw()
        {
            GestionnaireTransitions.Instance.Draw();
            GestionnaireScenes.Instance.Draw();
        }

        public static bool sceneEnFocus(string nomScene)
        {
            return GestionnaireScenes.Instance.EnFocus(nomScene);
        }

        public static void arreterScenes()
        {
            GestionnaireScenes.Instance.ToutesArretees();
        }
    }
}
