namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Core.Input;
    using Core.Visuel;
    using Core.Utilities;
    using Microsoft.Xna.Framework.GamerServices;
    using Microsoft.Xna.Framework.Input;

    class Acheter : Scene
    {
        private Main Main;
        private IVisible FondEcranAchat;
        private IVisible FondEcranAchatEffectue;
        private double TempsAvantQuitter;
        private Sablier Sablier;

        private Objets.AnimationTransition AnimationTransition;
        private bool effectuerTransition;
        private bool enAchat;

        public Acheter(Main main)
            : base(Vector2.Zero, 720, 1280)
        {
            Main = main;

            Nom = "Acheter";
            EnPause = true;
            EstVisible = false;
            EnFocus = false;

            FondEcranAchat = new IVisible(Core.Persistance.Facade.recuperer<Texture2D>("buy"), Vector3.Zero);
            FondEcranAchat.Origine = FondEcranAchat.Centre;
            FondEcranAchat.PrioriteAffichage = Preferences.PrioriteGUIMenuPrincipal + 0.03f;

            FondEcranAchatEffectue = new IVisible(Core.Persistance.Facade.recuperer<Texture2D>("buy2"), Vector3.Zero);
            FondEcranAchatEffectue.Origine = FondEcranAchatEffectue.Centre;
            FondEcranAchatEffectue.PrioriteAffichage = Preferences.PrioriteGUIMenuPrincipal + 0.03f;

            TempsAvantQuitter = 20000;

            Sablier = new Sablier(Main, this, 20000, new Vector3(520, 250, 0), 0);
            Sablier.TempsRestant = TempsAvantQuitter;

            AnimationTransition = new TDA.Objets.AnimationTransition();
            AnimationTransition.Duree = 500;
            AnimationTransition.Scene = this;
            AnimationTransition.PrioriteAffichage = Preferences.PrioriteTransitionScene;

            effectuerTransition = false;
            enAchat = false;
        }


        protected override void UpdateLogique(GameTime gameTime)
        {
            if (effectuerTransition)
            {
                AnimationTransition.suivant(gameTime);

                effectuerTransition = !AnimationTransition.estTerminee(gameTime);

                if (!effectuerTransition && !AnimationTransition.In)
                    Core.Visuel.Facade.effectuerTransition("AcheterVersMenu");
            }

            else
            {
                if (!Guide.IsVisible && Main.TrialMode.Active)
                    TempsAvantQuitter -= gameTime.ElapsedGameTime.TotalMilliseconds;

                Sablier.TempsRestant = TempsAvantQuitter;
                Sablier.Update(gameTime);

                if (TempsAvantQuitter <= 0)
                    Main.Exit();
            }
        }


        protected override void UpdateVisuel()
        {
            ajouterScenable((Main.TrialMode.Active) ? FondEcranAchat : FondEcranAchatEffectue);

            if (Main.TrialMode.Active)
                Sablier.Draw(null);

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

        private void AsyncExit(IAsyncResult result)
        {
            Main.Exit();
        }


        public override void doMouseButtonPressedOnce(PlayerIndex inputIndex, MouseButton button)
        {
        }


        public override void doGamePadButtonPressedOnce(PlayerIndex inputIndex, Buttons button)
        {
            if (effectuerTransition == true)
                return;

            if (Main.TrialMode.Active && button == Buttons.A)
            {
                try
                {
#if XBOX
                    Guide.ShowMarketplace(Main.JoueursConnectes[0].Manette);
#endif
                    enAchat = true;
                }

                catch (GamerPrivilegeException)
                {
#if XBOX
                    Guide.BeginShowMessageBox
                    (
                        "Oh no!",
                        "You must be signed in with an Xbox Live enabled profile to buy the game. You can either:\n\n1. Go buy it directly on the marketplace (suggested).\n\n2. Restart the game and sign in with an Xbox Live profile.\n\n\nThank you! The game will now close.",
                        new List<string> { "Ok" },
                        0,
                        MessageBoxIcon.Warning,
                        this.AsyncExit,
                        null);
#endif
                }
            }


            else
            {
                effectuerTransition = true;
                AnimationTransition.In = false;
                AnimationTransition.Initialize();
            }
        }
    }
}
