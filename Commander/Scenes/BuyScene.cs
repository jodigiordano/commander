namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Input;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.GamerServices;
    using Microsoft.Xna.Framework.Input;


    class BuyScene : Scene
    {
        private Image PleaseBuyBackground;
        private Image BoughtBackground;
        private double TimeBeforeQuitting;
        private SandGlass SandGlass;
        private bool BuyingMode;


        public BuyScene()
            : base(Preferences.BackBuffer)
        {
            Name = "Acheter";

            PleaseBuyBackground = new Image("buy");
            PleaseBuyBackground.VisualPriority = Preferences.PrioriteGUIMenuPrincipal + 0.03f;

            BoughtBackground = new Image("buy2");
            BoughtBackground.VisualPriority = Preferences.PrioriteGUIMenuPrincipal + 0.03f;

            TimeBeforeQuitting = 20000;

            SandGlass = new SandGlass(this, 20000, new Vector3(520, 250, 0), 0);
            SandGlass.RemainingTime = TimeBeforeQuitting;

            BuyingMode = false;
        }


        protected override void UpdateLogic(GameTime gameTime)
        {
            if (!Guide.IsVisible && Main.TrialMode.Active)
                TimeBeforeQuitting -= gameTime.ElapsedGameTime.TotalMilliseconds;

            SandGlass.RemainingTime = TimeBeforeQuitting;
            SandGlass.Update();

            if (TimeBeforeQuitting <= 0)
                Main.Instance.Exit();
        }


        protected override void UpdateVisual()
        {
            Add((Main.TrialMode.Active) ? PleaseBuyBackground : BoughtBackground);

            if (Main.TrialMode.Active)
                SandGlass.Draw();
        }


        private void AsyncExit(IAsyncResult result)
        {
            Main.Instance.Exit();
        }


        public override void DoMouseButtonPressedOnce(Core.Input.Player p, MouseButton button)
        {
        }


        public override void DoGamePadButtonPressedOnce(Core.Input.Player p, Buttons button)
        {
            if (Main.TrialMode.Active && button == Buttons.A)
            {
                if (Preferences.Target == Core.Utilities.Setting.Xbox360)
                {
                    try
                    {
                        BuyingMode = true;
                        Guide.ShowMarketplace(p.Index);
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
                    BuyingMode = true;
            }


            else
            {
                Visuals.Transite("Acheter", "Menu");
            }
        }
    }
}
