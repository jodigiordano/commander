namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class Help
    {
        private Image Lieutenant;
        private Image Bubble;
        private TextTypeWriter Directive;
        private Simulator Simulator;
        private List<string> Texts;
        private int ActiveText;
        private bool ActiveOverride;
        private bool HiddingOverride;
        private double EffectTimeRemaining;


        public Help(Simulator simulator, List<string> texts)
        {
            Simulator = simulator;
            Texts = texts;

            Lieutenant = new Image("lieutenant", new Vector3(-500, 370, 0));
            Lieutenant.Origin = new Vector2(0, Lieutenant.TextureSize.Y);
            Lieutenant.SizeX = 5;
            Lieutenant.VisualPriority = Preferences.PrioriteGUIVictoireDefaite - 0.0001;

            Bubble = new Image("bulle", new Vector3(-250, 200, 0));
            Bubble.Origin = new Vector2(0, Bubble.TextureSize.Y);
            Bubble.SizeX = 8;
            //Bubble.Color.A = 150;
            Bubble.VisualPriority = Preferences.PrioriteGUIVictoireDefaite - 0.0001;

            ActiveText = 0;
            ActiveOverride = Texts.Count > 0;
            HiddingOverride = false;
            EffectTimeRemaining = 1000;

            InitDirective(Texts.Count == 0 ? "" : Texts[ActiveText]);

            Simulator.Scene.VisualEffects.Add(Lieutenant, Core.Visual.VisualEffects.FadeInFrom0(255, 0, EffectTimeRemaining));
            Simulator.Scene.VisualEffects.Add(Bubble, Core.Visual.VisualEffects.FadeInFrom0(150, 0, EffectTimeRemaining));
            Simulator.Scene.VisualEffects.Add(Directive.Text, Core.Visual.VisualEffects.FadeInFrom0(255, 0, EffectTimeRemaining));
        }


        public bool Active
        {
            get { return HiddingOverride || (ActiveOverride && (ActiveText != Texts.Count || !Directive.Finished)); }
        }


        public void Update(GameTime gameTime)
        {
            Directive.Update(gameTime);

            if (!Active)
                FadeOut();
            
            if (HiddingOverride)
            {
                EffectTimeRemaining -= gameTime.ElapsedGameTime.TotalMilliseconds;
                HiddingOverride = EffectTimeRemaining > 0;
            }
        }


        public void NextDirective()
        {
            if (HiddingOverride)
                return;

            if (!Directive.Finished)
            {
                Directive.Finished = true;
                return;
            }
            
            ActiveText++;

            if (ActiveText > Texts.Count)
                ActiveText = Texts.Count;

            if (ActiveText == Texts.Count)
                Skip();

            if (ActiveText < Texts.Count)
                InitDirective(Texts[ActiveText]);
        }


        public void PreviousDirective()
        {
            if (HiddingOverride)
                return;

            if (!Directive.Finished)
            {
                Directive.Finished = true;
                return;
            }

            ActiveText--;

            if (ActiveText < 0)
                ActiveText = 0;
            else
                InitDirective(Texts[ActiveText]);
        }


        public void Skip()
        {
            if (HiddingOverride)
                return;

            ActiveOverride = false;

            FadeOut();
        }


        //public void Show()
        //{
        //    Simulation.Scene.Add(Lieutenant);
        //    Simulation.Scene.Add(Bubble);

        //    Directive.Show();
        //}


        //public void Hide()
        //{
        //    Simulation.Scene.Remove(Lieutenant);
        //    Simulation.Scene.Remove(Bubble);

        //    Directive.Hide();
        //}


        public void Draw()
        {
            Simulator.Scene.Add(Lieutenant);
            Simulator.Scene.Add(Bubble);

            Directive.Draw();
        }


        private void FadeOut()
        {
            HiddingOverride = true;
            Simulator.Scene.VisualEffects.Add(Lieutenant, Core.Visual.VisualEffects.FadeOutTo0(255, 0, EffectTimeRemaining / 2));
            Simulator.Scene.VisualEffects.Add(Bubble, Core.Visual.VisualEffects.FadeOutTo0(150, 0, EffectTimeRemaining / 2));
            Simulator.Scene.VisualEffects.Add(Directive.Text, Core.Visual.VisualEffects.FadeOutTo0(255, 0, EffectTimeRemaining / 2));
        }


        private void InitDirective(string text)
        {
            text = text.Replace("[Cancel]", Preferences.Target == Setting.Xbox360 ? "[B]" : "[Esc]");
            text = text.Replace("[Continue]", Preferences.Target == Setting.Xbox360 ? "[Press A to continue]" : "[Click to continue]");
            text = text.Replace("[Action]", Preferences.Target == Setting.Xbox360 ? "[Press A]" : "[Click]");
            text = text.Replace("[AdvancedView]", Preferences.Target == Setting.Xbox360 ? "[X]" : "[Middle mouse]");
            text = text.Replace("[Cycle]", Preferences.Target == Setting.Xbox360 ? "[LT/RT]" : "[Middle mouse scroll]");


            Directive = new TextTypeWriter(
                text,
                Color.Black,
                new Vector3(-230, -175, 0),
                "Pixelite",
                2,
                new Vector2(Bubble.TextureSize.X * Bubble.Size.X - 40, Bubble.TextureSize.Y * Bubble.Size.Y - 40),
                20,
                true,
                200,
                true,
                new List<string>()
                {
                    "sfxLieutenantParle1",
                    "sfxLieutenantParle2",
                    "sfxLieutenantParle3",
                    "sfxLieutenantParle4"
                },
                Simulator.Scene
            );
            Directive.Text.VisualPriority = Preferences.PrioriteGUIMenuPrincipal - 0.01f;
        }
    }
}
