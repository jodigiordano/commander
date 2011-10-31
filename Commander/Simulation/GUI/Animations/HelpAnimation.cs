namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class HelpAnimation : Animation
    {
        private Translator Translator;
        private string Message;


        public HelpAnimation(string message, double visualPriority)
            : base(5000, visualPriority)
        {
            Message = message;
        }


        public override void Initialize()
        {
            base.Initialize();

            var position = Preferences.Target == Core.Utilities.Setting.ArcadeRoyale ? new Vector3(0, -150, 0) : new Vector3(0, -250, 0);
            var size = Preferences.Target == Core.Utilities.Setting.ArcadeRoyale ? 2 : 3;

            Translator = new Translator(
                Scene,
                position,
                "Alien",
                Colors.Default.AlienBright,
                "Pixelite",
                Colors.Default.NeutralBright,
                Message,
                size,
                true,
                3000,
                100,
                VisualPriorities.Default.HelpMessage,
                true);
            Translator.CenterText = true;

            Scene.VisualEffects.Add(Translator, VisualEffects.FadeOutTo0(255, 4500, 500));
        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Translator.Update();
        }


        public override void Draw()
        {
            //Scene.BeginForeground();

            Translator.Position = new Vector3(((CommanderScene) Scene).CameraView.Center.X, ((CommanderScene) Scene).CameraView.Top + 200, 0); 
            Translator.Draw();

            //Scene.EndForeground();
        }
    }
}
