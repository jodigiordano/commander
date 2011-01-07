﻿namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using EphemereGames.Core.Visuel;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;


    class Help
    {
        private Image Lieutenant;
        private Image Bubble;
        private TextTypeWriter Directive;
        private Simulation Simulation;
        private List<string> Texts;
        private int ActiveText;
        private bool ActiveOverride;
        private bool HiddingOverride;
        private double EffectTimeRemaining;


        public Help(Simulation simulation, List<string> texts)
        {
            Simulation = simulation;
            Texts = texts;

            Lieutenant = new Image("lieutenant", new Vector3(-500, 370, 0));
            Lieutenant.Origin = new Vector2(0, Lieutenant.TextureSize.Y);
            Lieutenant.SizeX = 5;
            Lieutenant.VisualPriority = Preferences.PrioriteGUIMenuPrincipal;

            Bubble = new Image("bulle", new Vector3(-250, 200, 0));
            Bubble.Origin = new Vector2(0, Bubble.TextureSize.Y);
            Bubble.SizeX = 8;
            //Bubble.Color.A = 150;
            Bubble.VisualPriority = Preferences.PrioriteGUIMenuPrincipal;

            ActiveText = 0;
            ActiveOverride = Texts.Count > 0;
            HiddingOverride = false;
            EffectTimeRemaining = 1000;

            InitDirective(Texts.Count == 0 ? "" : Texts[ActiveText]);

            Simulation.Scene.Effets.Add(Lieutenant, Core.Visuel.PredefinedEffects.FadeInFrom0(255, 0, EffectTimeRemaining));
            Simulation.Scene.Effets.Add(Bubble, Core.Visuel.PredefinedEffects.FadeInFrom0(150, 0, EffectTimeRemaining));
            Simulation.Scene.Effets.Add(Directive.Texte, Core.Visuel.PredefinedEffects.FadeInFrom0(255, 0, EffectTimeRemaining));
        }


        public bool Active
        {
            get { return HiddingOverride || (ActiveOverride && (ActiveText != Texts.Count || !Directive.Termine)); }
        }


        public void Update(GameTime gameTime)
        {
            Directive.Update(gameTime);

            if (!Active)
                Hide();
            
            if (HiddingOverride)
            {
                EffectTimeRemaining -= gameTime.ElapsedGameTime.TotalMilliseconds;
                HiddingOverride = EffectTimeRemaining > 0;
            }
        }


        public void NextDirective()
        {
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
            ActiveText--;

            if (ActiveText < 0)
                ActiveText = 0;
            else
                InitDirective(Texts[ActiveText]);
        }


        public void Skip()
        {
            ActiveOverride = false;

            Hide();
        }


        public void Draw()
        {
            Simulation.Scene.ajouterScenable(Lieutenant);
            Simulation.Scene.ajouterScenable(Bubble);
            Simulation.Scene.ajouterScenable(Directive.Texte);
        }


        private void Hide()
        {
            HiddingOverride = true;
            Simulation.Scene.Effets.Add(Lieutenant, Core.Visuel.PredefinedEffects.FadeOutTo0(255, 0, EffectTimeRemaining / 2));
            Simulation.Scene.Effets.Add(Bubble, Core.Visuel.PredefinedEffects.FadeOutTo0(150, 0, EffectTimeRemaining / 2));
            Simulation.Scene.Effets.Add(Directive.Texte, Core.Visuel.PredefinedEffects.FadeOutTo0(255, 0, EffectTimeRemaining / 2));
        }


        private void InitDirective(string text)
        {
            text = text.Replace("[Cancel]", Preferences.Target == Setting.Xbox360 ? "[B]" : "[Esc]");
            text = text.Replace("[Continue]", Preferences.Target == Setting.Xbox360 ? "[Press A to continue]" : "[Click to continue]");
            text = text.Replace("[Action]", Preferences.Target == Setting.Xbox360 ? "[Press A]" : "[Click]");
            text = text.Replace("[AdvancedView]", Preferences.Target == Setting.Xbox360 ? "[X]" : "[Middle mouse]");

            Directive = new TextTypeWriter(
                Simulation.Main,
                text,
                Color.Black,
                new Vector3(-230, -175, 0),
                Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"),
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
                Simulation.Scene
            );
            Directive.Texte.VisualPriority = Preferences.PrioriteGUIMenuPrincipal - 0.01f;
        }
    }
}