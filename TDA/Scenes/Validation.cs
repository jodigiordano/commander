#if WINDOWS

namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using EphemereGames.Core.Input;
    using EphemereGames.Core.Visuel;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
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
        private AnimationTransition AnimationTransition;
        private Sablier Sablier;
        private ValidationServeur ValidationServeur;


        public Validation(Main main)
            : base(Vector2.Zero, 720, 1280)
        {
            Main = main;

            Nom = "Validation";

            Sablier = new Sablier(Main, this, 5000, new Vector3(0, 250, 0), 0.3f);
            Sablier.TempsRestant = 5000;
            Sablier.doHide(0);

            AnimationTransition = new AnimationTransition(this, 500, Preferences.PrioriteTransitionScene);

            FondEcran = new IVisible(EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>("PixelBlanc"), Vector3.Zero);
            FondEcran.Couleur = Color.White;
            FondEcran.TailleVecteur = new Vector2(1280, 720);
            FondEcran.Origine = FondEcran.Centre;
            FondEcran.VisualPriority = 1f;

            ThankYou = new IVisible("Thank you for your support, Commander !\n\nTo continue saving the world, please enter\n\nthe 16 digits product key you received by\n\nmail and click continue. You must be\n\nconnected to Internet to complete this\n\nstep. If you have any problems, send me\n\nan e-mail at jodi@ephemeregames.com or\n\nvisit my website. Good luck Commander !", EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"), Color.White, new Vector3(0, -150, 0));
            ThankYou.Taille = 3;
            ThankYou.Origine = ThankYou.Centre;
            ThankYou.VisualPriority = 0.3f;

            this.Effets.Add(ThankYou, PredefinedEffects.FadeInFrom0(255, 0, 1000));

            Curseur = new Cursor(Main, this, new Vector3(0, 100, 0), 10, 0.2f);

            Clavier = new List<KeyValuePair<IVisible, PushButton>>();
            Clavier.Add
            (
                new KeyValuePair<IVisible, PushButton>(
                new IVisible("0", EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"), Color.White, new Vector3(-450, 100, 0)),
                new PushButton(Main, this, Curseur, new Vector3(-450, 100, 0), 0.3f)
                )
            );
            Clavier.Add
            (
                new KeyValuePair<IVisible, PushButton>(
                new IVisible("1", EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"), Color.White, new Vector3(-400, 100, 0)),
                new PushButton(Main, this, Curseur, new Vector3(-400, 100, 0), 0.3f)
                )
            );
            Clavier.Add
            (
                new KeyValuePair<IVisible, PushButton>(
                new IVisible("2", EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"), Color.White, new Vector3(-350, 100, 0)),
                new PushButton(Main, this, Curseur, new Vector3(-350, 100, 0), 0.3f)
                )
            );
            Clavier.Add
            (
                new KeyValuePair<IVisible, PushButton>(
                new IVisible("3", EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"), Color.White, new Vector3(-450, 150, 0)),
                new PushButton(Main, this, Curseur, new Vector3(-450, 150, 0), 0.3f)
                )
            );
            Clavier.Add
            (
                new KeyValuePair<IVisible, PushButton>(
                new IVisible("4", EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"), Color.White, new Vector3(-400, 150, 0)),
                new PushButton(Main, this, Curseur, new Vector3(-400, 150, 0), 0.3f)
                )
            );
            Clavier.Add
            (
                new KeyValuePair<IVisible, PushButton>(
                new IVisible("5", EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"), Color.White, new Vector3(-350, 150, 0)),
                new PushButton(Main, this, Curseur, new Vector3(-350, 150, 0), 0.3f)
                )
            );
            Clavier.Add
            (
                new KeyValuePair<IVisible, PushButton>(
                new IVisible("6", EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"), Color.White, new Vector3(-450, 200, 0)),
                new PushButton(Main, this, Curseur, new Vector3(-450, 200, 0), 0.3f)
                )
            );
            Clavier.Add
            (
                new KeyValuePair<IVisible, PushButton>(
                new IVisible("7", EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"), Color.White, new Vector3(-400, 200, 0)),
                new PushButton(Main, this, Curseur, new Vector3(-400, 200, 0), 0.3f)
                )
            );
            Clavier.Add
            (
                new KeyValuePair<IVisible, PushButton>(
                new IVisible("8", EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"), Color.White, new Vector3(-350, 200, 0)),
                new PushButton(Main, this, Curseur, new Vector3(-350, 200, 0), 0.3f)
                )
            );
            Clavier.Add
            (
                new KeyValuePair<IVisible, PushButton>(
                new IVisible("9", EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"), Color.White, new Vector3(-450, 250, 0)),
                new PushButton(Main, this, Curseur, new Vector3(-450, 250, 0), 0.3f)
                )
            );
            Clavier.Add
            (
                new KeyValuePair<IVisible, PushButton>(
                new IVisible("Clear", EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"), Color.White, new Vector3(-400, 300, 0)),
                new PushButton(Main, this, Curseur, new Vector3(-400, 300, 0), 0.3f)
                )
            );


            foreach (var touche in Clavier)
            {
                touche.Key.Taille = 3;
                touche.Key.Origine = touche.Key.Centre;
                touche.Key.VisualPriority = 0.3f;
                touche.Key.Couleur = new Color(234, 196, 28, 255);

                this.Effets.Add(touche.Key, PredefinedEffects.FadeInFrom0(255, 0, 1000));
                this.Effets.Add(touche.Value.Bouton, PredefinedEffects.FadeInFrom0(255, 0, 1000));
            }

            Saisie = new IVisible("", EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"), Color.White, new Vector3(0, 100, 0));
            Saisie.Origine = Saisie.Centre;
            Saisie.Taille = 5;
            Saisie.VisualPriority = 0.3f;
            Saisie.Couleur = new Color(234, 196, 28, 255);

            Exit = new KeyValuePair<IVisible,PushButton>
            (
                new IVisible("Exit", EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"), Color.White, new Vector3(400, 200, 0)),
                new PushButton(Main, this, Curseur, new Vector3(550, 200, 0), 0.3f)
            );
            Exit.Key.Taille = 3;
            Exit.Key.Origine = Exit.Key.Centre;
            Exit.Key.VisualPriority = 0.3f;

            Continue = new KeyValuePair<IVisible, PushButton>
            (
                new IVisible("Continue", EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"), Color.White, new Vector3(400, 300, 0)),
                new PushButton(Main, this, Curseur, new Vector3(550, 300, 0), 0.3f)
            );
            Continue.Key.Taille = 3;
            Continue.Key.Origine = Continue.Key.Centre;
            Continue.Key.VisualPriority = 0.3f;
            Continue.Key.Couleur = new Color(155, 255, 102);
            Continue.Value.Bouton.Couleur = new Color(155, 255, 102);

            this.Effets.Add(Exit.Key, PredefinedEffects.FadeInFrom0(255, 0, 1000));
            this.Effets.Add(Continue.Key, PredefinedEffects.FadeInFrom0(255, 0, 1000));

            MessageErreur = new IVisible("", EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"), Color.White, new Vector3(0, 250, 0));
            MessageErreur.Taille = 3;
            MessageErreur.Origine = MessageErreur.Centre;
            MessageErreur.Couleur = Color.Red;
            MessageErreur.VisualPriority = 0.3f;
        }


        protected override void UpdateLogique(GameTime gameTime)
        {
            if (effectuerValidation)
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
                    this.Effets.Add(MessageErreur, PredefinedEffects.FadeInFrom0(255, 0, 500));
                }
                else if (ValidationServeur.ValidationTerminee && ValidationServeur.Valide)
                {
                    MessageErreur.Texte = ValidationServeur.Message;
                    MessageErreur.Couleur = new Color(155, 255, 102);
                    Main.SaveGame.ProductKey = Saisie.Texte;
                    EphemereGames.Core.Persistance.Facade.SaveData("savePlayer");
                    Transition = TransitionType.Out;
                    effectuerValidation = false;
                    Sablier.doHide(250);
                    this.Effets.Add(MessageErreur, PredefinedEffects.FadeInFrom0(255, 0, 250));
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

                    this.Effets.Add(MessageErreur, PredefinedEffects.FadeOutTo0(255, 0, 250));
                }

                Exit.Value.Update(gameTime);
                Continue.Value.Update(gameTime);
            }
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
                    EphemereGames.Core.Visuel.Facade.Transite("ValidationToMenu");

                Transition = TransitionType.None;
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

            if (Transition != TransitionType.None)
                AnimationTransition.Draw(null);
        }

        public override void onFocus()
        {
            base.onFocus();

            Transition = TransitionType.In;
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