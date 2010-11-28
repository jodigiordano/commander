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

            FondEcranAchat = new IVisible(Core.Persistance.Facade.recuperer<Texture2D>("buy"), Vector3.Zero, this);
            FondEcranAchat.Origine = FondEcranAchat.Centre;
            FondEcranAchat.PrioriteAffichage = Preferences.PrioriteGUIMenuPrincipal + 0.03f;

            FondEcranAchatEffectue = new IVisible(Core.Persistance.Facade.recuperer<Texture2D>("buy2"), Vector3.Zero, this);
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

            else if (!Main.ModeTrial.Actif &&
                    (Core.Input.Facade.estPeseeUneSeuleFois(Preferences.toucheSelection, Main.JoueursConnectes[0].Manette, this.Nom) ||
                    Core.Input.Facade.estPeseeUneSeuleFois(Preferences.toucheChangerMusique, Main.JoueursConnectes[0].Manette, this.Nom) ||
                    Core.Input.Facade.estPeseeUneSeuleFois(Preferences.toucheDebug, Main.JoueursConnectes[0].Manette, this.Nom) ||
                    Core.Input.Facade.estPeseeUneSeuleFois(Preferences.toucheProchaineVague, Main.JoueursConnectes[0].Manette, this.Nom) ||
                    Core.Input.Facade.estPeseeUneSeuleFois(Preferences.toucheRetour, Main.JoueursConnectes[0].Manette, this.Nom) ||
                    Core.Input.Facade.estPeseeUneSeuleFois(Preferences.toucheRetourMenu, Main.JoueursConnectes[0].Manette, this.Nom) ||
                    Core.Input.Facade.estPeseeUneSeuleFois(Preferences.toucheSelectionPrecedent, Main.JoueursConnectes[0].Manette, this.Nom) ||
                    Core.Input.Facade.estPeseeUneSeuleFois(Preferences.toucheSelectionSuivant, Main.JoueursConnectes[0].Manette, this.Nom) ||
                    Core.Input.Facade.estPeseeUneSeuleFois(Preferences.toucheVueAvancee, Main.JoueursConnectes[0].Manette, this.Nom)))
            {
                effectuerTransition = true;
                AnimationTransition.In = false;
                AnimationTransition.Initialize();
            }

            else if (Main.ModeTrial.Actif && Core.Input.Facade.estPeseeUneSeuleFois(Preferences.toucheSelection, Main.JoueursConnectes[0].Manette, this.Nom))
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
                if (!Guide.IsVisible && Main.ModeTrial.Actif)
                    TempsAvantQuitter -= gameTime.ElapsedGameTime.TotalMilliseconds;

                Sablier.TempsRestant = TempsAvantQuitter;
                Sablier.Update(gameTime);

                if (TempsAvantQuitter <= 0)
                    Main.Exit();
            }
        }


        protected override void UpdateVisuel()
        {
            ajouterScenable((Main.ModeTrial.Actif) ? FondEcranAchat : FondEcranAchatEffectue);

            if (Main.ModeTrial.Actif)
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
    }
}
