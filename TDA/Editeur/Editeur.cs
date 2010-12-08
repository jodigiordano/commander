﻿namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Core.Visuel;
    using Core.Utilities;
    using Core.Physique;
    using Core.Input;
    using Microsoft.Xna.Framework.Input;

    class Editeur : Scene
    {
        private Main Main;

        private Objets.AnimationTransition AnimationTransition;
        private bool effectuerTransition;
        private String ChoixTransition;
        private Simulation Simulation;
        private GenerateurGUI GenerateurGUI;
        private Cursor Curseur;

        public Editeur(Main main)
            : base(Vector2.Zero, 720, 1280)
        {
            Main = main;

            Nom = "Editeur";
            EnPause = true;
            EstVisible = false;
            EnFocus = false;

            Simulation = new Simulation(main, this, FactoryScenarios.getDescripteurBidon());
            Simulation.Players = Main.Players;
            Simulation.Initialize();
            Simulation.ModeEditeur = true;
            Simulation.EnPause = true;

            Curseur = new Cursor(Main, this, Vector3.Zero, 10, Preferences.PrioriteGUIConsoleEditeur);
            GenerateurGUI = new GenerateurGUI(Simulation, Curseur, new Vector3(-300, 80, 0));
            GenerateurGUI.Visible = true;

            AnimationTransition = new TDA.Objets.AnimationTransition();
            AnimationTransition.Duree = 500;
            AnimationTransition.Scene = this;
            AnimationTransition.PrioriteAffichage = Preferences.PrioriteTransitionScene;

            effectuerTransition = false;

            Main.PlayersController.PlayerDisconnected += new NoneHandler(doJoueurPrincipalDeconnecte);
        }


        private void doJoueurPrincipalDeconnecte()
        {
            AnimationTransition.In = false;
            AnimationTransition.Initialize();
            ChoixTransition = "chargement";
            effectuerTransition = true;
        }


        protected override void UpdateLogique(GameTime gameTime)
        {
            if (effectuerTransition)
            {
                AnimationTransition.suivant(gameTime);

                effectuerTransition = !AnimationTransition.estTerminee(gameTime);

                if (!effectuerTransition && !AnimationTransition.In)
                {
                    switch (ChoixTransition)
                    {
                        case "menu": Core.Visuel.Facade.effectuerTransition("EditeurVersMenu"); break;
                        case "chargement": Core.Visuel.Facade.effectuerTransition("EditeurVersChargement"); break;
                    }
                }
            }


            else
            {
                //Curseur.Update(gameTime); //todo
                Simulation.EnPause = GenerateurGUI.Visible;
                GenerateurGUI.Update(gameTime);
                Simulation.Update(gameTime);
            }

        }


        protected override void UpdateVisuel()
        {
            Curseur.Draw();
            GenerateurGUI.Draw(null);
            Simulation.Draw(null);

            if (effectuerTransition)
                AnimationTransition.Draw(null);
        }


        public override void onFocus()
        {
            base.onFocus();

            effectuerTransition = true;
            AnimationTransition.In = true;
            AnimationTransition.Initialize();
        }


        public override void onFocusLost()
        {
            base.onFocusLost();

            Core.Persistance.Facade.sauvegarderDonnee("savePlayer");
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
            if (effectuerTransition)
                return;

            effectuerTransition = true;
            ChoixTransition = "menu";
            AnimationTransition.In = false;
            AnimationTransition.Initialize();
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
