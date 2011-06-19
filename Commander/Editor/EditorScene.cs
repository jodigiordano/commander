namespace EphemereGames.Commander
{
    using EphemereGames.Commander.Simulation;
    using EphemereGames.Core.Input;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;


    class EditorScene : Scene
    {
        private string ChoixTransition;
        private Simulator Simulator;
        private Cursor Cursor;


        public EditorScene()
            : base(Vector2.Zero, 1280, 720)
        {
            Name = "Editeur";

            Simulator = new Simulator(this, LevelsFactory.GetEmptyDescriptor());
            Simulator.Initialize();
            Simulator.EditorMode = true;
            Simulator.State = GameState.Paused;

            Cursor = new Cursor(this, Vector3.Zero, 10, Preferences.PrioriteGUIConsoleEditeur);
        }


        protected override void UpdateLogic(GameTime gameTime)
        {
            Simulator.Update(gameTime);
        }


        protected override void UpdateVisual()
        {
            Cursor.Draw();
            Simulator.Draw();
        }


        public override void OnFocus()
        {
            base.OnFocus();

            Inputs.AddListener(Simulator);
        }


        public override void OnFocusLost()
        {
            base.OnFocusLost();

            Inputs.RemoveListener(Simulator);
        }


        #region Input Handling

        public override void DoMouseMoved(Core.Input.Player p, Vector3 delta)
        {
            Player player = (Player) p;

            player.Move(ref delta, MouseConfiguration.Speed);
            Cursor.Position = player.Position;
        }


        public override void DoGamePadJoystickMoved(Core.Input.Player p, Buttons button, Vector3 delta)
        {
            Player player = (Player) p;

            if (button == GamePadConfiguration.MoveCursor)
            {
                player.Move(ref delta, GamePadConfiguration.Speed);
                Cursor.Position = player.Position;
            }
        }


        public override void DoMouseButtonPressedOnce(Core.Input.Player p, MouseButton button)
        {
            if (button == MouseConfiguration.Cancel)
                BeginTransition();
        }


        public override void DoGamePadButtonPressedOnce(Core.Input.Player p, Buttons button)
        {
            if (button == GamePadConfiguration.Cancel)
                BeginTransition();
        }


        public override void DoKeyPressedOnce(Core.Input.Player p, Keys key)
        {

        }


        public override void DoPlayerDisconnected(Core.Input.Player player)
        {
            if (Inputs.ConnectedPlayers.Count == 0)
                TransiteTo("Chargement");
        }


        #endregion


        private void BeginTransition()
        {
            Visuals.Transite("Editeur", "Menu");
        }
    }
}
