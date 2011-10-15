namespace EphemereGames.Commander
{
    using EphemereGames.Core.Input;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;


    class EndOfDemoScene : CommanderScene
    {
        private Text EndOfDemo;
        private Image Background;

        private static string Message =
            "This is the end of the demo, Commander!\n\n\n\n" +

            "I hope you enjoyed it. The final release will\n\n" +
            "include more worlds, turrets, enemies, power-ups,\n\n" +
            "environments, game modes, challenges, and support\n\n" +
            "4 players local multiplayer and leaderboards. \n\n\n\n" +

            "In the meanwhile, you can try to get the 3 stars\n\n" +
            "on each levels and stay in touch with me by visiting\n\n" +
            "www.ephemeregames.com. I love to receive feedback, so\n\n" +
            "don't hesitate! Great ideas will get implemented.\n\n\n\n" +

            "Thank you very much to have tried the game. You rock!\n\n\n\n";

        private Text Continue;


        public EndOfDemoScene() :
            base("EndOfDemo")
        {
            Continue = new Text(
                Preferences.Target == Core.Utilities.Setting.Xbox360 ?
                "Press a button to continue." :
                "Click a mouse button to continue.", "Pixelite", new Vector3(0, 270, 0))
                {
                    Color = new Color(255, 7, 106),
                    SizeX = 3
                }.CenterIt();

            EndOfDemo = new Text(Message, "Pixelite")
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
            Add(EndOfDemo);
            Add(Background);
            Add(Continue);
        }


        public override void DoMouseButtonPressedOnce(Core.Input.Player player, MouseButton button)
        {
            TransiteTo("Menu");
        }


        public override void DoGamePadButtonPressedOnce(Core.Input.Player player, Buttons button)
        {
            TransiteTo("Menu");
        }


        public override void DoPlayerDisconnected(Core.Input.Player player)
        {
            if (Inputs.ConnectedPlayers.Count == 0)
                TransiteTo("Menu");
        }
    }
}
