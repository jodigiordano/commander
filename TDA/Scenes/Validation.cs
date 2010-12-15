#if WINDOWS

namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Core.Visuel;
    using Core.Utilities;
    using Microsoft.Xna.Framework.GamerServices;
    using System.Threading;
    using Core.Input;
    using Microsoft.Xna.Framework.Input;

    class Validation : Scene
    {
        public Main Main;

        private IVisible FondEcran;
        private IVisible ThankYou;
        private List<KeyValuePair<IVisible, PushButton>> Clavier;
        private KeyValuePair<IVisible, PushButton> Exit;
        private KeyValuePair<IVisible, PushButton> Continue;
        private IVisible MessageErreur;
        private Cursor Curseur;
        private IVisible Saisie;
        private bool effectuerValidation;
        private bool effectuerTransition;
        private Objets.AnimationTransition AnimationTransition;
        private Sablier Sablier;
        private ValidationServeur ValidationServeur;

        public Validation(Main main)
            : base(Vector2.Zero, 720, 1280)
        {
            Main = main;

            Nom = "Validation";
            EnPause = true;
            EstVisible = false;
            EnFocus = false;

            Sablier = new Sablier(Main, this, 5000, new Vector3(0, 250, 0), 0.3f);
            Sablier.TempsRestant = 5000;
            Sablier.doHide(0);

            AnimationTransition = new TDA.Objets.AnimationTransition();
            AnimationTransition.Duree = 500;
            AnimationTransition.Scene = this;
            AnimationTransition.In = false;
            AnimationTransition.Initialize();
            AnimationTransition.PrioriteAffichage = Preferences.PrioriteTransitionScene;

            FondEcran = new IVisible(Core.Persistance.Facade.recuperer<Texture2D>("PixelBlanc"), Vector3.Zero);
            FondEcran.Couleur = Color.White;
            FondEcran.TailleVecteur = new Vector2(1280, 720);
            FondEcran.Origine = FondEcran.Centre;
            FondEcran.PrioriteAffichage = 1f;

            ThankYou = new IVisible("Thank you for your support, Commander !\n\nTo continue saving the world, please enter\n\nthe 16 digits product key you received by\n\nmail and click continue. You must be\n\nconnected to Internet to complete this\n\nstep. If you have any problems, send me\n\nan e-mail at jodi@ephemeregames.com or\n\nvisit my website. Good luck Commander !", Core.Persistance.Facade.recuperer<SpriteFont>("Pixelite"), Color.White, new Vector3(0, -150, 0));
            ThankYou.Taille = 3;
            ThankYou.Origine = ThankYou.Centre;
            ThankYou.PrioriteAffichage = 0.3f;

            this.Effets.ajouter(ThankYou, EffetsPredefinis.fadeInFrom0(255, 0, 1000));

            Curseur = new Cursor(Main, this, new Vector3(0, 100, 0), 10, 0.2f);

            Clavier = new List<KeyValuePair<IVisible, PushButton>>();
            Clavier.Add
            (
                new KeyValuePair<IVisible, PushButton>(
                new IVisible("0", Core.Persistance.Facade.recuperer<SpriteFont>("Pixelite"), Color.White, new Vector3(-450, 100, 0)),
                new PushButton(Main, this, Curseur, new Vector3(-450, 100, 0), 0.3f)
                )
            );
            Clavier.Add
            (
                new KeyValuePair<IVisible, PushButton>(
                new IVisible("1", Core.Persistance.Facade.recuperer<SpriteFont>("Pixelite"), Color.White, new Vector3(-400, 100, 0)),
                new PushButton(Main, this, Curseur, new Vector3(-400, 100, 0), 0.3f)
                )
            );
            Clavier.Add
            (
                new KeyValuePair<IVisible, PushButton>(
                new IVisible("2", Core.Persistance.Facade.recuperer<SpriteFont>("Pixelite"), Color.White, new Vector3(-350, 100, 0)),
                new PushButton(Main, this, Curseur, new Vector3(-350, 100, 0), 0.3f)
                )
            );
            Clavier.Add
            (
                new KeyValuePair<IVisible, PushButton>(
                new IVisible("3", Core.Persistance.Facade.recuperer<SpriteFont>("Pixelite"), Color.White, new Vector3(-450, 150, 0)),
                new PushButton(Main, this, Curseur, new Vector3(-450, 150, 0), 0.3f)
                )
            );
            Clavier.Add
            (
                new KeyValuePair<IVisible, PushButton>(
                new IVisible("4", Core.Persistance.Facade.recuperer<SpriteFont>("Pixelite"), Color.White, new Vector3(-400, 150, 0)),
                new PushButton(Main, this, Curseur, new Vector3(-400, 150, 0), 0.3f)
                )
            );
            Clavier.Add
            (
                new KeyValuePair<IVisible, PushButton>(
                new IVisible("5", Core.Persistance.Facade.recuperer<SpriteFont>("Pixelite"), Color.White, new Vector3(-350, 150, 0)),
                new PushButton(Main, this, Curseur, new Vector3(-350, 150, 0), 0.3f)
                )
            );
            Clavier.Add
            (
                new KeyValuePair<IVisible, PushButton>(
                new IVisible("6", Core.Persistance.Facade.recuperer<SpriteFont>("Pixelite"), Color.White, new Vector3(-450, 200, 0)),
                new PushButton(Main, this, Curseur, new Vector3(-450, 200, 0), 0.3f)
                )
            );
            Clavier.Add
            (
                new KeyValuePair<IVisible, PushButton>(
                new IVisible("7", Core.Persistance.Facade.recuperer<SpriteFont>("Pixelite"), Color.White, new Vector3(-400, 200, 0)),
                new PushButton(Main, this, Curseur, new Vector3(-400, 200, 0), 0.3f)
                )
            );
            Clavier.Add
            (
                new KeyValuePair<IVisible, PushButton>(
                new IVisible("8", Core.Persistance.Facade.recuperer<SpriteFont>("Pixelite"), Color.White, new Vector3(-350, 200, 0)),
                new PushButton(Main, this, Curseur, new Vector3(-350, 200, 0), 0.3f)
                )
            );
            Clavier.Add
            (
                new KeyValuePair<IVisible, PushButton>(
                new IVisible("9", Core.Persistance.Facade.recuperer<SpriteFont>("Pixelite"), Color.White, new Vector3(-450, 250, 0)),
                new PushButton(Main, this, Curseur, new Vector3(-450, 250, 0), 0.3f)
                )
            );
            Clavier.Add
            (
                new KeyValuePair<IVisible, PushButton>(
                new IVisible("Clear", Core.Persistance.Facade.recuperer<SpriteFont>("Pixelite"), Color.White, new Vector3(-400, 300, 0)),
                new PushButton(Main, this, Curseur, new Vector3(-400, 300, 0), 0.3f)
                )
            );


            foreach (var touche in Clavier)
            {
                touche.Key.Taille = 3;
                touche.Key.Origine = touche.Key.Centre;
                touche.Key.PrioriteAffichage = 0.3f;
                touche.Key.Couleur = new Color(234, 196, 28, 255);

                this.Effets.ajouter(touche.Key, EffetsPredefinis.fadeInFrom0(255, 0, 1000));
                this.Effets.ajouter(touche.Value.Bouton, EffetsPredefinis.fadeInFrom0(255, 0, 1000));
            }

            Saisie = new IVisible("", Core.Persistance.Facade.recuperer<SpriteFont>("Pixelite"), Color.White, new Vector3(0, 100, 0));
            Saisie.Origine = Saisie.Centre;
            Saisie.Taille = 5;
            Saisie.PrioriteAffichage = 0.3f;
            Saisie.Couleur = new Color(234, 196, 28, 255);

            Exit = new KeyValuePair<IVisible,PushButton>
            (
                new IVisible("Exit", Core.Persistance.Facade.recuperer<SpriteFont>("Pixelite"), Color.White, new Vector3(400, 200, 0)),
                new PushButton(Main, this, Curseur, new Vector3(550, 200, 0), 0.3f)
            );
            Exit.Key.Taille = 3;
            Exit.Key.Origine = Exit.Key.Centre;
            Exit.Key.PrioriteAffichage = 0.3f;

            Continue = new KeyValuePair<IVisible, PushButton>
            (
                new IVisible("Continue", Core.Persistance.Facade.recuperer<SpriteFont>("Pixelite"), Color.White, new Vector3(400, 300, 0)),
                new PushButton(Main, this, Curseur, new Vector3(550, 300, 0), 0.3f)
            );
            Continue.Key.Taille = 3;
            Continue.Key.Origine = Continue.Key.Centre;
            Continue.Key.PrioriteAffichage = 0.3f;
            Continue.Key.Couleur = new Color(155, 255, 102);
            Continue.Value.Bouton.Couleur = new Color(155, 255, 102);

            this.Effets.ajouter(Exit.Key, EffetsPredefinis.fadeInFrom0(255, 0, 1000));
            this.Effets.ajouter(Continue.Key, EffetsPredefinis.fadeInFrom0(255, 0, 1000));

            effectuerTransition = false;

            MessageErreur = new IVisible("", Core.Persistance.Facade.recuperer<SpriteFont>("Pixelite"), Color.White, new Vector3(0, 250, 0));
            MessageErreur.Taille = 3;
            MessageErreur.Origine = MessageErreur.Centre;
            MessageErreur.Couleur = Color.Red;
            MessageErreur.PrioriteAffichage = 0.3f;
        }


        protected override void UpdateLogique(GameTime gameTime)
        {
            if (effectuerTransition)
            {
                AnimationTransition.suivant(gameTime);

                effectuerTransition = !AnimationTransition.estTerminee(gameTime);
                
                if (!effectuerTransition && !AnimationTransition.In)
                    Core.Visuel.Facade.effectuerTransition("ValidationVersMenu");
            }

            else if (effectuerValidation)
            {
                if (!Sablier.Tourne)
                    Sablier.TempsRestant -= gameTime.ElapsedGameTime.TotalMilliseconds;

                if (Sablier.TempsRestant <= 0)
                {
                    Sablier.TempsRestant = 5000;
                    Sablier.tourner();
                }

                Sablier.Update(gameTime);
                ValidationServeur.Update(gameTime);

                if (ValidationServeur.DelaiExpire)
                    ValidationServeur.canceler();

                //todo: mettre asynchrone
                if (ValidationServeur.ValidationTerminee && !ValidationServeur.Valide)
                {
                    MessageErreur.Texte = ValidationServeur.Message;
                    effectuerValidation = false;
                    Sablier.doHide(500);
                    Curseur.doShow();
                    this.Effets.ajouter(MessageErreur, EffetsPredefinis.fadeInFrom0(255, 0, 500));
                }
                else if (ValidationServeur.ValidationTerminee && ValidationServeur.Valide)
                {
                    MessageErreur.Texte = ValidationServeur.Message;
                    MessageErreur.Couleur = new Color(155, 255, 102);
                    Main.SaveGame.ProductKey = Saisie.Texte;
                    Core.Persistance.Facade.sauvegarderDonnee("savePlayer");
                    effectuerTransition = true;
                    AnimationTransition.In = false;
                    AnimationTransition.Initialize();
                    effectuerValidation = false;
                    Sablier.doHide(250);
                    this.Effets.ajouter(MessageErreur, EffetsPredefinis.fadeInFrom0(255, 0, 250));
                }
            }

            else
            {
                foreach (var kvp in Clavier)
                {
                    if (kvp.Value.Pressed && kvp.Key.Texte == "Clear")
                        Saisie.Texte = "";
                    else if (kvp.Value.Pressed && Saisie.Texte.Length < 16)
                        Saisie.Texte += kvp.Key.Texte;

                    kvp.Value.Update(gameTime);
                }

                if (Exit.Value.Pressed)
                    Main.Exit();

                if (Continue.Value.Pressed)
                {
                    effectuerValidation = true;
                    Sablier.doShow(500);
                    Curseur.doHide();

                    ValidationServeur = new ValidationServeur(Preferences.ProductName, Saisie.Texte);
                    ValidationServeur.valider();

                    this.Effets.ajouter(MessageErreur, EffetsPredefinis.fadeOutTo0(255, 0, 250));
                }

                Exit.Value.Update(gameTime);
                Continue.Value.Update(gameTime);
            }
        }


        protected override void UpdateVisuel()
        {
            Sablier.Draw(null);

            MessageErreur.Origine = MessageErreur.Centre;
            this.ajouterScenable(MessageErreur);

            this.ajouterScenable(ThankYou);

            foreach (var kvp in Clavier)
            {
                this.ajouterScenable(kvp.Key);
                kvp.Value.Draw(null);
            }

            Saisie.Origine = Saisie.Centre;
            this.ajouterScenable(Saisie);

            this.ajouterScenable(Exit.Key);
            Exit.Value.Draw(null);

            this.ajouterScenable(Continue.Key);
            Continue.Value.Draw(null);

            Curseur.Draw();

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


        public override void doMouseButtonPressedOnce(PlayerIndex inputIndex, MouseButton button)
        {
            Player p = Main.Players[inputIndex];

            if (!p.Master)
                return;

            if (button == p.MouseConfiguration.Select)
            {
                foreach (var bouton in Clavier)
                    bouton.Value.doClick();

                Exit.Value.doClick();
                Continue.Value.doClick();
            }
        }


        public override void doMouseMoved(PlayerIndex inputIndex, Vector3 delta)
        {
            Player p = Main.Players[inputIndex];

            if (!p.Master)
                return;

            p.Move(ref delta, p.MouseConfiguration.Speed);
            Curseur.Position = p.Position;
        }


        public override void doGamePadJoystickMoved(PlayerIndex inputIndex, Buttons button, Vector3 delta)
        {
            Player p = Main.Players[inputIndex];

            if (!p.Master)
                return;

            if (button == p.GamePadConfiguration.MoveCursor)
            {
                p.Move(ref delta, p.GamePadConfiguration.Speed);
                Curseur.Position = p.Position;
            }
        }


        public override void doKeyPressedOnce(PlayerIndex inputIndex, Keys key)
        {
            Player p = Main.Players[inputIndex];

            if (!p.Master)
                return;

            if ((int)key >= (int)Keys.D0 && (int)key <= (int)Keys.D9)
                Clavier[(int)key - (int)Keys.D0].Value.Pressed = true;

            if ((int)key >= (int)Keys.NumPad0 && (int)key <= (int)Keys.NumPad9)
                Clavier[(int)key - (int)Keys.NumPad0].Value.Pressed = true;

            if (key == Keys.Back)
                Clavier[Clavier.Count - 1].Value.Pressed = true;

            if (key == Keys.Enter)
                Continue.Value.doClick();
        }


        public override void doGamePadButtonPressedOnce(PlayerIndex inputIndex, Buttons button)
        {
            Player p = Main.Players[inputIndex];

            if (!p.Master)
                return;

            if (button == p.GamePadConfiguration.Select)
            {
                foreach (var bouton in Clavier)
                    bouton.Value.doClick();

                Exit.Value.doClick();
                Continue.Value.doClick();
            }
        }
    }
}

#endif