namespace EphemereGames.Commander
{
    using EphemereGames.Commander.Simulation;
    using EphemereGames.Core.Input;
    using EphemereGames.Core.XACTAudio;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;


    class EditorScene : CommanderScene
    {
        private Simulator Simulator;


        public EditorScene()
            : base("Editeur")
        {
            Simulator = new Simulator(this, Main.LevelsFactory.GetEmptyDescriptor())
            {
                EditorMode = true
            };

            Simulator.Initialize();
            Inputs.AddListener(Simulator);
            Simulator.EnableInputs = false;
        }


        protected override void UpdateLogic(GameTime gameTime)
        {
            Simulator.Update();
        }


        protected override void UpdateVisual()
        {
            Simulator.Draw();
        }


        public override void OnFocus()
        {
            base.OnFocus();

            Simulator.OnFocus();
            Simulator.TeleportPlayers(false);
            XACTAudio.PlayCue("ScreenChange", "Sound Bank");
        }


        public override void OnFocusLost()
        {
            base.OnFocusLost();

            Simulator.OnFocusLost();
            Simulator.TeleportPlayers(true);
            XACTAudio.PlayCue("ScreenChange", "Sound Bank");
        }


        #region Input Handling


        public override void DoKeyPressedOnce(Core.Input.Player p, Keys key)
        {
            var player = (Commander.Player) p;

            if (key == player.KeyboardConfiguration.Back)
                TransiteTo("Menu");
        }


        public override void DoMouseButtonPressedOnce(Core.Input.Player p, MouseButton button)
        {

        }


        public override void DoGamePadButtonPressedOnce(Core.Input.Player p, Buttons button)
        {
            var player = (Commander.Player) p;

            if (button == player.GamepadConfiguration.Back)
                TransiteTo("Menu");
        }


        public override void DoPlayerDisconnected(Core.Input.Player player)
        {
            if (Inputs.ConnectedPlayers.Count == 0)
                TransiteTo("Menu");
        }


        #endregion
    }
}
