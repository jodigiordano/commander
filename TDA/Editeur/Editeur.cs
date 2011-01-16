namespace EphemereGames.Commander
{
    using System;
    using EphemereGames.Core.Input;
    using EphemereGames.Core.Visuel;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;


    class Editeur : Scene
    {
        private Main Main;
        private AnimationTransition AnimationTransition;
        private String ChoixTransition;
        private Simulation Simulation;
        private GenerateurGUI GenerateurGUI;
        private Cursor Cursor;


        public Editeur(Main main)
            : base(Vector2.Zero, 720, 1280)
        {
            Main = main;

            Nom = "Editeur";

            Simulation = new Simulation(main, this, FactoryScenarios.getDescripteurBidon());
            Simulation.Players = Main.Players;
            Simulation.Initialize();
            Simulation.ModeEditeur = true;
            Simulation.Etat = GameState.Paused;

            Cursor = new Cursor(Main, this, Vector3.Zero, 10, Preferences.PrioriteGUIConsoleEditeur);
            GenerateurGUI = new GenerateurGUI(Simulation, Cursor, new Vector3(-300, 80, 0));
            GenerateurGUI.Visible = true;

            AnimationTransition = new AnimationTransition(this, 500, Preferences.PrioriteTransitionScene);

            Main.PlayersController.PlayerDisconnected += new NoneHandler(doJoueurPrincipalDeconnecte);
        }


        private void doJoueurPrincipalDeconnecte()
        {
            Transition = TransitionType.Out;
            ChoixTransition = "chargement";
        }


        protected override void UpdateLogique(GameTime gameTime)
        {
            if (Transition != TransitionType.None)
                return;

            if (GenerateurGUI.Visible)
                Simulation.Etat = GameState.Paused;
            else
                Simulation.Etat = GameState.Running;

            GenerateurGUI.Update(gameTime);
            Simulation.Update(gameTime);
        }


        protected override void InitializeTransition(TransitionType type)
        {
            AnimationTransition.In = (type == TransitionType.In) ? true : false;
            AnimationTransition.Initialize();
        }


        protected override void UpdateTransition(GameTime gameTime)
        {
            AnimationTransition.Update(gameTime);

            if (AnimationTransition.Finished(gameTime))
            {
                if (Transition == TransitionType.Out)
                    switch (ChoixTransition)
                    {
                        case "menu": EphemereGames.Core.Visuel.Facade.Transite("EditeurToMenu"); break;
                        case "chargement": EphemereGames.Core.Visuel.Facade.Transite("EditeurToChargement"); break;
                    }

                Transition = TransitionType.None;
            }
        }


        protected override void UpdateVisuel()
        {
            Cursor.Draw();
            GenerateurGUI.Draw();
            Simulation.Draw();

            if (Transition != TransitionType.None)
                AnimationTransition.Draw(null);
        }


        public override void onFocus()
        {
            base.onFocus();

            Transition = TransitionType.In;

            EphemereGames.Core.Input.Facade.AddListener(Simulation);
        }


        public override void onFocusLost()
        {
            base.onFocusLost();

            //EphemereGames.Core.Persistance.Facade.SaveData("savePlayer");
            EphemereGames.Core.Input.Facade.RemoveListener(Simulation);
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
                beginTransition();

            if (button == p.MouseConfiguration.Select)
                GenerateurGUI.DoClick();
        }


        public override void doGamePadButtonPressedOnce(PlayerIndex inputIndex, Buttons button)
        {
            Player p = Main.Players[inputIndex];

            if (!p.Master)
                return;

            if (button == p.GamePadConfiguration.Cancel)
                beginTransition();

            if (button == p.GamePadConfiguration.Editor)
                doToggleEditor();

            if (button == p.GamePadConfiguration.Select)
                GenerateurGUI.DoClick();
        }


        public override void doKeyPressedOnce(PlayerIndex inputIndex, Keys key)
        {
            Player p = Main.Players[inputIndex];

            if (!p.Master)
                return;

            if (key == p.KeyboardConfiguration.Editor)
                doToggleEditor();
        }

        #endregion


        private void beginTransition()
        {
            if (Transition != TransitionType.None)
                return;

            Transition = TransitionType.Out;
            ChoixTransition = "menu";
        }


        private void doToggleEditor()
        {
            GenerateurGUI.Visible = !GenerateurGUI.Visible;

            if (GenerateurGUI.Visible)
                Cursor.doShow();
            else
                Cursor.doHide();
        }
    }
}
