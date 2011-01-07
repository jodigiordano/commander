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
        private Cursor Curseur;

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

            Curseur = new Cursor(Main, this, Vector3.Zero, 10, Preferences.PrioriteGUIConsoleEditeur);
            GenerateurGUI = new GenerateurGUI(Simulation, Curseur, new Vector3(-300, 80, 0));
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
            Curseur.Draw();
            GenerateurGUI.Draw(null);
            Simulation.Draw();

            if (Transition != TransitionType.None)
                AnimationTransition.Draw(null);
        }


        public override void onFocus()
        {
            base.onFocus();

            Transition = TransitionType.In;
        }


        public override void onFocusLost()
        {
            base.onFocusLost();

            EphemereGames.Core.Persistance.Facade.SaveData("savePlayer");
        }


        public override void doMouseButtonPressedOnce(PlayerIndex inputIndex, MouseButton button)
        {
            if (button == MouseButton.Right)
                beginTransition();
        }


        public override void doKeyPressedOnce(PlayerIndex inputIndex, Keys key)
        {
            if (key == Keys.F2)
                doHideEditor();
        }


        public override void doGamePadButtonPressedOnce(PlayerIndex inputIndex, Buttons button)
        {
            if (button == Buttons.B)
                beginTransition();

            if (button == Buttons.LeftShoulder)
                doHideEditor();
        }


        private void beginTransition()
        {
            if (Transition != TransitionType.None)
                return;

            Transition = TransitionType.Out;
            ChoixTransition = "menu";
        }


        private void doHideEditor()
        {
            GenerateurGUI.Visible = !GenerateurGUI.Visible;

            if (GenerateurGUI.Visible)
                Curseur.doShow();
            else
                Curseur.doHide();
        }
    }
}
