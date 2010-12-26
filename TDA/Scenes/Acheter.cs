namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using EphemereGames.Core.Input;
    using EphemereGames.Core.Visuel;
    using Microsoft.Xna.Framework.GamerServices;
    using Microsoft.Xna.Framework.Input;

    class Acheter : Scene
    {
        private Main Main;
        private IVisible FondEcranAchat;
        private IVisible FondEcranAchatEffectue;
        private double TempsAvantQuitter;
        private Sablier Sablier;

        private AnimationTransition AnimationTransition;
        private bool enAchat;

        public Acheter(Main main)
            : base(Vector2.Zero, 720, 1280)
        {
            Main = main;

            Nom = "Acheter";

            FondEcranAchat = new IVisible(EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>("buy"), Vector3.Zero);
            FondEcranAchat.Origine = FondEcranAchat.Centre;
            FondEcranAchat.VisualPriority = Preferences.PrioriteGUIMenuPrincipal + 0.03f;

            FondEcranAchatEffectue = new IVisible(EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>("buy2"), Vector3.Zero);
            FondEcranAchatEffectue.Origine = FondEcranAchatEffectue.Centre;
            FondEcranAchatEffectue.VisualPriority = Preferences.PrioriteGUIMenuPrincipal + 0.03f;

            TempsAvantQuitter = 20000;

            Sablier = new Sablier(Main, this, 20000, new Vector3(520, 250, 0), 0);
            Sablier.TempsRestant = TempsAvantQuitter;

            AnimationTransition = new AnimationTransition(this, 500, Preferences.PrioriteTransitionScene);

            enAchat = false;
        }


        protected override void UpdateLogique(GameTime gameTime)
        {
            if (Transition != TransitionType.None)
                return;

            if (!Guide.IsVisible && Main.TrialMode.Active)
                TempsAvantQuitter -= gameTime.ElapsedGameTime.TotalMilliseconds;

            Sablier.TempsRestant = TempsAvantQuitter;
            Sablier.Update(gameTime);

            if (TempsAvantQuitter <= 0)
                Main.Exit();
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
                    EphemereGames.Core.Visuel.Facade.Transite("AcheterToMenu");

                Transition = TransitionType.None;
            }
        }


        protected override void UpdateVisuel()
        {
            ajouterScenable((Main.TrialMode.Active) ? FondEcranAchat : FondEcranAchatEffectue);

            if (Main.TrialMode.Active)
                Sablier.Draw(null);

            if (Transition != TransitionType.None)
                AnimationTransition.Draw(null);
        }


        public override void onFocus()
        {
            base.onFocus();

            Transition = TransitionType.In;
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
            if (Main.TrialMode.Active && button == Buttons.A)
            {
                if (Preferences.Target == Setting.Xbox360)
                {
                    try
                    {
                        enAchat = true;
                        Guide.ShowMarketplace(inputIndex);
                    }

                    catch (GamerPrivilegeException)
                    {
                        Guide.BeginShowMessageBox
                        (
                            "Oh no!",
                            "You must be signed in with an Xbox Live enabled profile to buy the game. You can either:\n\n1. Go buy it directly on the marketplace (suggested).\n\n2. Restart the game and sign in with an Xbox Live profile.\n\n\nThank you! The game will now close.",
                            new List<string> { "Ok" },
                            0,
                            MessageBoxIcon.Warning,
                            this.AsyncExit,
                            null);
                    }
                }

                else
                    enAchat = true;
            }


            else
            {
                Transition = TransitionType.Out;
            }
        }
    }
}
