namespace EphemereGames.Core.Visual
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;


    public static class Visuals
    {
        internal static ScenesController ScenesController;
        private static TransitionsController TransitionsController;

        private static int nextHashCode = 0;
        public static int NextHashCode { get { return nextHashCode++; } }


        public static void Initialize(
            GraphicsDeviceManager graphicsDeviceManager,
            int windowWidth,
            int windowHeight,
            ContentManager content,
            string[] scenesNames)
        {
            Preferences.GraphicsDeviceManager = graphicsDeviceManager;
            Preferences.WindowWidth = windowWidth;
            Preferences.WindowHeight = windowHeight;
            Preferences.Content = content;

            ScenesController = new ScenesController();
            TransitionsController = new TransitionsController();

            graphicsDeviceManager.PreferMultiSampling = true;

            foreach (var sceneName in scenesNames)
                ScenesController.AddScene(sceneName, null);

            for (int i = 0; i < scenesNames.Length; i++)
                for (int j = 0; j < scenesNames.Length; j++)
                    if (i != j)
                        TransitionsController.AddTransition(new Transition(scenesNames[i], scenesNames[j]));


            Primitives.Initialize(Preferences.Content.Load<Texture2D>("pixelBlanc"));

            EphemereGames.Core.Persistence.Persistence.AddAssetType(new Particle());
            EphemereGames.Core.Persistence.Persistence.AddAssetType(new Sprite());
        }


        //public static void AddTransition(Transition transition)
        //{
        //    TransitionsController.AddTransition(transition);
        //}


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


        public static Scene GetScene(string nomScene)
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
            TransitionsController.Draw();
            ScenesController.Draw();
        }


        public static Vector3 ClampToXbox360SafeZone(Scene s, Vector3 v)
        {
            Rectangle r = ScreenSafeZone.GetXbox360();

            v.X = MathHelper.Clamp(v.X, -r.Width / 2, r.Width / 2);
            v.Y = MathHelper.Clamp(v.Y, -r.Height / 2, r.Height / 2);

            return v;
        }


        public static ITransitionAnimation TransitionAnimation
        {
            set
            {
                TransitionsController.TransitionAnimation = value;
            }
        }
    }
}
