namespace EphemereGames.Commander
{
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class WorldAnnunciationScene : Scene
    {
        private WorldDescriptor Descriptor;
        private Text WorldId;
        private Text WorldName;

        private double Length;
        private bool TransitionInProgress;


        public WorldAnnunciationScene(WorldDescriptor descriptor) :
            base(1280, 720)
        {
            Name = "World" + descriptor.Id + "Annunciation";

            Descriptor = descriptor;

            WorldId = new Text("World " + Descriptor.Id, "Pixelite")
            {
                SizeX = 4,
                Color = Color.Black
            }.CenterIt();


            WorldName = new Text(Descriptor.Name, "Pixelite")
            {
                Position = new Vector3(0, 50, 0),
                SizeX = 3,
                Color = Color.Black
            }.CenterIt();
        }


        public void Initialize()
        {
            Length = 2000;
            TransitionInProgress = false;
        }


        protected override void UpdateLogic(GameTime gameTime)
        {
            if (TransitionInProgress)
                return;

            Length -= Preferences.TargetElapsedTimeMs;

            if (Length <= 0)
            {
                TransiteTo("World" + Descriptor.Id);
                TransitionInProgress = true;
            }
        }


        protected override void UpdateVisual()
        {
            Add(WorldId);
            Add(WorldName);
        }


        public override void OnFocus()
        {
            Initialize();
        }


        public override void OnFocusLost()
        {

        }
    }
}
