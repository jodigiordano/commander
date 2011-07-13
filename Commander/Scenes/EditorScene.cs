namespace EphemereGames.Commander
{
    using EphemereGames.Commander.Simulation;
    using EphemereGames.Core.Input;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;


    class EditorScene : Scene
    {
        private Simulator Simulator;


        public EditorScene()
            : base(1280, 720)
        {
            Name = "Editeur";

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
            Simulator.Update(gameTime);
        }


        protected override void UpdateVisual()
        {
            Simulator.Draw();
        }


        public override void OnFocus()
        {
            base.OnFocus();

            Simulator.SyncPlayers();

            Simulator.EnableInputs = true;
        }


        public override void OnFocusLost()
        {
            base.OnFocusLost();

            Simulator.EnableInputs = false;
        }


        #region Input Handling


        public override void DoKeyPressedOnce(Core.Input.Player player, Keys key)
        {
            if (key == KeyboardConfiguration.Back)
                TransiteTo("Menu");
        }


        public override void DoMouseButtonPressedOnce(Core.Input.Player p, MouseButton button)
        {

        }


        public override void DoGamePadButtonPressedOnce(Core.Input.Player p, Buttons button)
        {
            if (button == GamePadConfiguration.Back)
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
