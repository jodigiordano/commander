namespace EphemereGames.Commander
{
    using EphemereGames.Core.Input;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;


    class WarningScene : CommanderScene
    {
        private Text Warning;
        private Image Background;

        private static string Message =
            "Welcome to the battlefield, Commander! (v. 0.1)\n\n\n\n" +

            "Please keep in mind that it's a pre-alpha demo.\n\n" +
            "In your journey, you will encounter bugs, stumble\n\n" +
            "upon rough edges and experiment weird glitches.\n\n" +
            "If you do, let me know, and we'll laugh about\n\n" +
            "it together.\n\n\n\n" +

            "Also, I'm currently putting all my energy on\n\n" +
            "making a solid gameplay. More worlds/levels and\n\n" +
            "story will follow later. So don't be surprised to\n\n" +
            "finish the game really quickly!\n\n\n\n" +

            "Finally, take a deep breath; relax; and\n\n" +
            "enjoy your stay, my friend.\n\n\n\n";

        private Text Continue;


        public WarningScene() :
            base("Warning")
        {
            Continue = new Text(
                Preferences.Target == Core.Utilities.Setting.Xbox360 ?
                "Press a button to continue." :
                "Click a mouse button to continue.", "Pixelite", new Vector3(0, 270, 0))
                {
                    Color = new Color(255, 7, 106),
                    SizeX = 3
                }.CenterIt();

            Warning = new Text(Message, "Pixelite")
            {
                SizeX = 2,
                Color = Color.Black,
                VisualPriority = 0.5
            }.CenterIt();

            Background = new Image("WhiteBg", Vector3.Zero)
            {
                VisualPriority = 1,
                Color = new Color(Main.Random.Next(220, 255), Main.Random.Next(220, 255), Main.Random.Next(220, 255))
            };
        }


        protected override void UpdateLogic(GameTime gameTime)
        {

        }


        protected override void UpdateVisual()
        {
            Add(Warning);
            Add(Background);
            Add(Continue);
        }


        public override void PlayerGamePadConnectionRequested(Core.Input.Player Player, Buttons button)
        {
            TransiteTo("Menu");
        }


        public override void PlayerMouseConnectionRequested(Core.Input.Player Player, MouseButton button)
        {
            TransiteTo("Menu");
        }


        public override void PlayerKeyboardConnectionRequested(Core.Input.Player Player, Keys key)
        {
            TransiteTo("Menu");
        }
    }
}
