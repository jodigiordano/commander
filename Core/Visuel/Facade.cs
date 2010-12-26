namespace EphemereGames.Core.Visuel
{
    using System;
    using EphemereGames.Core.Utilities;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;

    public delegate void NoneHandler();


    public static class Facade
    {
        internal static ScenesController ScenesController;
        private static TransitionsController TransitionsController;


        public static void Initialize(
            GraphicsDeviceManager graphicsDeviceManager,
            int windowWidth,
            int windowHeight,
            ContentManager content,
            String[] scenesNames,
            ManagedThread thread1,
            ManagedThread thread2)
        {
            Preferences.GraphicsDeviceManager = graphicsDeviceManager;
            Preferences.WindowWidth = windowWidth;
            Preferences.WindowHeight = windowHeight;
            Preferences.Content = content;
            Preferences.ThreadParticules = thread1;
            Preferences.ThreadLogic = thread2;

            ScenesController = new ScenesController();
            TransitionsController = new TransitionsController();

            graphicsDeviceManager.GraphicsDevice.PresentationParameters.PresentationInterval = PresentInterval.Default;

            foreach (var sceneName in scenesNames)
                ScenesController.AddScene(sceneName, null);

            for (int i = 0; i < scenesNames.Length; i++)
                for (int j = 0; j < scenesNames.Length; j++)
                    if (i != j)
                        TransitionsController.AddTransition(new Transition(scenesNames[i], scenesNames[j]));


            Primitives.Initialize(Preferences.Content.Load<Texture2D>("pixelBlanc"));

            EphemereGames.Core.Persistance.Facade.AddAsset(new ParticuleEffectWrapper());
            EphemereGames.Core.Persistance.Facade.AddAsset(new Sprite());
        }


        public static void AddTransition(Transition transition)
        {
            TransitionsController.AddTransition(transition);
        }


        public static void Transite(string transitionName)
        {
            TransitionsController.Transite(transitionName);
        }


        public static void GetNotifiedTransition(NoneHandler handlerStarted, NoneHandler handlerStopped)
        {
            TransitionsController.TransitionStarted += handlerStarted;
            TransitionsController.TransitionTerminated += handlerStopped;
        }


        public static bool InTransition
        {
            get { return TransitionsController.InTransition; }
        }


        public static Scene GetScene(String nomScene)
        {
            return ScenesController.GetScene(nomScene);
        }


        public static void Update(GameTime gameTime)
        {
            ScenesController.Update(gameTime);

            if (TransitionsController.InTransition)
                TransitionsController.Update(gameTime);
        }


        public static void UpdateScene(string sceneName, Scene scene)
        {
            ScenesController.UpdateScene(sceneName, scene);
        }


        public static void Draw()
        {
            ScenesController.Draw();
        }
    }
}
