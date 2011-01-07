namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Physique;
    using EphemereGames.Core.Utilities;
    using EphemereGames.Core.Visuel;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    class AnimationLieutenant : Animation
    {
        private Main Main;
        private Image Bubble;
        private Image TheLieutenant;
        private TextTypeWriter TypeWriter;
        private EffectsController GE;


        public AnimationLieutenant(Main main, Scene scene, String texte, double temps)
            : base(temps)
        {
            Scene = scene;
            Main = main;

            TheLieutenant = new Image("lieutenant", new Vector3(-300, 500, 0));
            TheLieutenant.SizeX = 6;
            TheLieutenant.VisualPriority = Preferences.PrioriteGUIMenuPrincipal;

            Bubble = new Image("bulle", new Vector3(-100, 300, 0));
            Bubble.SizeX = 8;
            Bubble.VisualPriority = Preferences.PrioriteGUIMenuPrincipal + 0.02f;
            Bubble.Origin = Vector2.Zero;

            TypeWriter = new TextTypeWriter
            (
                Main,
                texte,
                Color.Black,
                new Vector3(20, 280, 0),
                EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"),
                2.0f,
                new Vector2(600, 500),
                50,
                true,
                1000,
                true,
                new List<string>()
                {
                    "sfxLieutenantParle1",
                    "sfxLieutenantParle2",
                    "sfxLieutenantParle3",
                    "sfxLieutenantParle4"
                },
                Scene
            );
            TypeWriter.Texte.VisualPriority = Preferences.PrioriteGUIMenuPrincipal;

            GE = new EffectsController();

            EffetDeplacementTrajet edt = new EffetDeplacementTrajet();
            edt.Delay = 0;
            edt.Length = this.Length;
            edt.Progress = AbstractEffect.ProgressType.Linear;
            edt.Trajet = new Trajet2D
            (new Vector2[] { new Vector2(-300, 500), new Vector2(-300, 275), new Vector2(-300, 275), new Vector2(-300, 700) },
             new double[] { 0, 1000, this.Length - 1000, this.Length });

            GE.Add(TheLieutenant, edt);

            edt = new EffetDeplacementTrajet();
            edt.Delay = 0;
            edt.Length = this.Length;
            edt.Progress = AbstractEffect.ProgressType.Linear;
            edt.Trajet = new Trajet2D
            (new Vector2[] { new Vector2(-100, 300), new Vector2(-100, -100), new Vector2(-100, -100), new Vector2(-100, 500) },
             new double[] { 0, 1000, this.Length - 1000, this.Length });

            GE.Add(Bubble, edt);

            edt = new EffetDeplacementTrajet();
            edt.Delay = 0;
            edt.Length = this.Length;
            edt.Progress = AbstractEffect.ProgressType.Linear;
            edt.Trajet = new Trajet2D
            (new Vector2[] { new Vector2(-75, 325), new Vector2(-75, -75), new Vector2(-75, -75), new Vector2(-75, 500) },
             new double[] { 0, 1000, this.Length - 1000, this.Length });

            GE.Add(TypeWriter.Texte, edt);
        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            TypeWriter.Update(gameTime);

            GE.Update(gameTime);
        }


        public void doShow()
        {
            GE.Add(Bubble, PredefinedEffects.FadeInFrom0(255, 0, 250));
            GE.Add(TheLieutenant, PredefinedEffects.FadeInFrom0(255, 0, 250));
            GE.Add(TypeWriter.Texte, PredefinedEffects.FadeInFrom0(255, 0, 250));
        }


        public void doHide()
        {
            GE.Add(Bubble, PredefinedEffects.FadeOutTo0(255, 0, 250));
            GE.Add(TheLieutenant, PredefinedEffects.FadeOutTo0(255, 0, 250));
            GE.Add(TypeWriter.Texte, PredefinedEffects.FadeOutTo0(255, 0, 250));
        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            TheLieutenant.Draw(spriteBatch);
            Bubble.Draw(spriteBatch);
            TypeWriter.Texte.Draw(spriteBatch);
        }
    }
}
