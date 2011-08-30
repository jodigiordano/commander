namespace EphemereGames.Commander
{
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class PausePanel : VerticalPanel
    {
        private PushButton Options;
        private PushButton Help;
        private PushButton Controls;
        private PushButton Restart;
        private PushButton Resume;
        private PushButton GoBackToWorld;


        public PausePanel(Scene scene, Vector3 position, Vector2 size, double visualPriority, Color color)
            : base(scene, position, size, visualPriority, color)
        {
            SetTitle("Game Paused");

            Resume = new PushButton(new Text("Resume", @"Pixelite") { SizeX = 2 });
            Restart = new PushButton(new Text("Restart", @"Pixelite") { SizeX = 2 });
            GoBackToWorld = new PushButton(new Text("Go to galaxy", @"Pixelite") { SizeX = 2 });
            Options = new PushButton(new Text("Options", @"Pixelite") { SizeX = 2 });
            Help = new PushButton(new Text("How to play", @"Pixelite") { SizeX = 2 });
            Controls = new PushButton(new Text("Controls", @"Pixelite") { SizeX = 2 });

            AddWidget("Resume", Resume);
            AddWidget("Options", Options);
            AddWidget("Help", Help);
            AddWidget("Controls", Controls);
            AddWidget("Restart", Restart);
            AddWidget("GoBackToWorld", GoBackToWorld);

            Alpha = 0;
        }
    }
}
