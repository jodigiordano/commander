namespace EphemereGames.Commander
{
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class WorldDownloadingScene : CommanderScene
    {
        private Text Downloading;
        private Image Background;

        private bool TransitionInProgress;


        public WorldDownloadingScene() :
            base("WorldDownloading")
        {
            Downloading = new Text("Downloading... please wait.", "Pixelite")
            {
                SizeX = 4,
                Color = Color.White
            }.CenterIt();

            Background = new Image("WhiteBg", Vector3.Zero) { VisualPriority = 1 };
        }


        public override void Initialize()
        {
            TransitionInProgress = false;

            Background.Color = Color.Transparent;

            VisualEffects.Add(Background, Core.Visual.VisualEffects.FadeInFrom0(255, 0, 500));
        }


        protected override void UpdateLogic(GameTime gameTime)
        {
            if (TransitionInProgress)
                return;

            if (Main.MultiverseController.WorldIsReady)
            {
                Main.MultiverseController.JumpToWorldDirectly("WorldDownloading");
                TransitionInProgress = true;
            }
        }


        protected override void UpdateVisual()
        {
            Add(Downloading);
            Add(Background);
        }


        public override void OnFocus()
        {
            Initialize();
        }
    }
}
