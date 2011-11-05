namespace EphemereGames.Commander
{
    using EphemereGames.Commander.Simulation;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class WorldAnnunciationScene : CommanderScene
    {
        public World World;
        public bool InitWorld;

        private Text WorldId;
        private Translator WorldName;
        private Image Background;

        private double Length;
        private bool TransitionInProgress;


        public WorldAnnunciationScene() :
            base("WorldAnnunciation")
        {
            WorldId = new Text("Pixelite")
            {
                SizeX = 4,
                Color = Color.Black
            };

            Background = new Image("WhiteBg", Vector3.Zero) { VisualPriority = 1 };

            InitWorld = false;
        }


        public override void Initialize()
        {
            Length = 2500;
            TransitionInProgress = false;

            WorldId.Data = World.StringId;
            WorldId.CenterIt();

            WorldName = new Translator(this, new Vector3(0, 50, 0), "Alien", Colors.Default.AlienBright, @"Pixelite", Colors.Default.NeutralDark, World.Name, 3, true, 1500, 100, Preferences.PrioriteGUIPanneauGeneral, false);
            WorldName.CenterText = true;

            VisualEffects.Add(WorldName.ToTranslate, Core.Visual.VisualEffects.FadeInFrom0(255, 0, 1000));
            VisualEffects.Add(WorldName.Translated, Core.Visual.VisualEffects.FadeInFrom0(255, 0, 1000));

            Background.Color = new Color(Main.Random.Next(235, 255), Main.Random.Next(235, 255), Main.Random.Next(235, 255));
        }


        protected override void UpdateLogic(GameTime gameTime)
        {
            if (TransitionInProgress)
                return;

            Length -= Preferences.TargetElapsedTimeMs;

            if (Length <= 0)
            {
                if (InitWorld)
                {
                    Main.CurrentWorld.World = World;
                    Main.CurrentWorld.Initialize();
                    InitWorld = false;
                }
                TransiteTo("World");
                TransitionInProgress = true;
            }

            WorldName.Update();
        }


        protected override void UpdateVisual()
        {
            Add(WorldId);
            Add(Background);
            WorldName.Draw();
        }


        public override void OnFocus()
        {
            Initialize();

            Main.MusicController.Play("IntroNiveau");
        }


        public override void OnFocusLost()
        {

        }
    }
}
