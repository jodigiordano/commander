namespace EphemereGames.Commander
{
    using System;
    using EphemereGames.Core.Input;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;


    class Editor : Scene
    {
        private Main Main;
        private string ChoixTransition;
        private Simulation Simulation;
        private GenerateurGUI GenerateurGUI;
        private Cursor Cursor;


        public Editor(Main main)
            : base(Vector2.Zero, 720, 1280)
        {
            Main = main;

            Name = "Editeur";

            Simulation = new Simulation(main, this, ScenariosFactory.getDescripteurBidon());
            Simulation.Players = Main.Players;
            Simulation.Initialize();
            Simulation.EditorMode = true;
            Simulation.Etat = GameState.Paused;

            Cursor = new Cursor(Main, this, Vector3.Zero, 10, Preferences.PrioriteGUIConsoleEditeur);
            GenerateurGUI = new GenerateurGUI(Simulation, Cursor, new Vector3(-300, 80, 0));
            GenerateurGUI.Visible = true;

            Main.PlayersController.PlayerDisconnected += new NoneHandler(DoPlayerDisconnected);
        }


        private void DoPlayerDisconnected()
        {
            Visuals.Transite("EditeurToChargement");
        }


        protected override void UpdateLogic(GameTime gameTime)
        {
            if (GenerateurGUI.Visible)
                Simulation.Etat = GameState.Paused;
            else
                Simulation.Etat = GameState.Running;

            GenerateurGUI.Update(gameTime);
            Simulation.Update(gameTime);
        }


        protected override void UpdateVisual()
        {
            Cursor.Draw();
            GenerateurGUI.Draw();
            Simulation.Draw();
        }


        public override void OnFocus()
        {
            base.OnFocus();

            Input.AddListener(Simulation);
        }


        public override void OnFocusLost()
        {
            base.OnFocusLost();

            //EphemereGames.Core.Persistance.Facade.SaveData("savePlayer");
            Input.RemoveListener(Simulation);
        }


        #region Input Handling

        public override void doMouseMoved(PlayerIndex inputIndex, Vector3 delta)
        {
            Player p = Main.Players[inputIndex];

            if (!p.Master)
                return;

            if (!GenerateurGUI.Visible)
                return;

            p.Move(ref delta, p.MouseConfiguration.Speed);
            Cursor.Position = p.Position;
        }


        public override void doGamePadJoystickMoved(PlayerIndex inputIndex, Buttons button, Vector3 delta)
        {
            Player p = Main.Players[inputIndex];

            if (!p.Master)
                return;

            if (!GenerateurGUI.Visible)
                return;

            if (button == p.GamePadConfiguration.MoveCursor)
            {
                p.Move(ref delta, p.GamePadConfiguration.Speed);
                Cursor.Position = p.Position;
            }
        }


        public override void doMouseButtonPressedOnce(PlayerIndex inputIndex, MouseButton button)
        {
            Player p = Main.Players[inputIndex];

            if (!p.Master)
                return;

            if (!GenerateurGUI.Visible)
                return;

            if (button == p.MouseConfiguration.Cancel)
                BeginTransition();

            if (button == p.MouseConfiguration.Select)
                GenerateurGUI.DoClick();
        }


        public override void doGamePadButtonPressedOnce(PlayerIndex inputIndex, Buttons button)
        {
            Player p = Main.Players[inputIndex];

            if (!p.Master)
                return;

            if (button == p.GamePadConfiguration.Cancel)
                BeginTransition();

            if (button == p.GamePadConfiguration.Editor)
                DoToggleEditor();

            if (button == p.GamePadConfiguration.Select)
                GenerateurGUI.DoClick();
        }


        public override void doKeyPressedOnce(PlayerIndex inputIndex, Keys key)
        {
            Player p = Main.Players[inputIndex];

            if (!p.Master)
                return;

            if (key == p.KeyboardConfiguration.Editor)
                DoToggleEditor();
        }

        #endregion


        private void BeginTransition()
        {
            Visuals.Transite("EditeurToMenu");
        }


        private void DoToggleEditor()
        {
            GenerateurGUI.Visible = !GenerateurGUI.Visible;

            if (GenerateurGUI.Visible)
                Cursor.FadeIn();
            else
                Cursor.FadeOut();
        }
    }
}
