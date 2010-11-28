namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Core.Visuel;
    using Core.Utilities;
    using Core.Physique;
    using ProjectMercury.Emitters;
    using ProjectMercury.Modifiers;

    class AnimationLieutenant : Animation
    {
        private Scene Scene;
        private Main Main;
        private IVisible Bulle;
        private IVisible Lieutenant;
        private TextTypeWriter TypeWriter;

        private GestionnaireEffets GE;

        public AnimationLieutenant(Main main, Scene scene, String texte, double temps)
            : base(temps)
        {
            Scene = scene;
            Main = main;

            Lieutenant = new IVisible(Core.Persistance.Facade.recuperer<Texture2D>("lieutenant"), new Vector3(-300, 500, 0), Scene);
            Lieutenant.Taille = 6;
            Lieutenant.Origine = Lieutenant.Centre;
            Lieutenant.PrioriteAffichage = Preferences.PrioriteGUIMenuPrincipal;

            Bulle = new IVisible(Core.Persistance.Facade.recuperer<Texture2D>("bulle"), new Vector3(-100, 300, 0), Scene);
            Bulle.Taille = 8;
            Bulle.PrioriteAffichage = Preferences.PrioriteGUIMenuPrincipal + 0.02f;

            TypeWriter = new TextTypeWriter
            (
                Main,
                texte,
                Color.Black,
                new Vector3(20, 280, 0),
                Core.Persistance.Facade.recuperer<SpriteFont>("Pixelite"),
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
            TypeWriter.Texte.PrioriteAffichage = Preferences.PrioriteGUIMenuPrincipal;

            GE = new GestionnaireEffets();

            EffetDeplacementTrajet edt = new EffetDeplacementTrajet();
            edt.Delai = 0;
            edt.Duree = this.Duree;
            edt.Progression = AbstractEffet.TypeProgression.Lineaire;
            edt.Trajet = new Trajet2D
            (new Vector2[] { new Vector2(-300, 500), new Vector2(-300, 275), new Vector2(-300, 275), new Vector2(-300, 700) },
             new double[] { 0, 1000, this.Duree - 1000, this.Duree });

            GE.ajouter(Lieutenant, edt);

            edt = new EffetDeplacementTrajet();
            edt.Delai = 0;
            edt.Duree = this.Duree;
            edt.Progression = AbstractEffet.TypeProgression.Lineaire;
            edt.Trajet = new Trajet2D
            (new Vector2[] { new Vector2(-100, 300), new Vector2(-100, -100), new Vector2(-100, -100), new Vector2(-100, 500) },
             new double[] { 0, 1000, this.Duree - 1000, this.Duree });

            GE.ajouter(Bulle, edt);

            edt = new EffetDeplacementTrajet();
            edt.Delai = 0;
            edt.Duree = this.Duree;
            edt.Progression = AbstractEffet.TypeProgression.Lineaire;
            edt.Trajet = new Trajet2D
            (new Vector2[] { new Vector2(-75, 325), new Vector2(-75, -75), new Vector2(-75, -75), new Vector2(-75, 500) },
             new double[] { 0, 1000, this.Duree - 1000, this.Duree });

            GE.ajouter(TypeWriter.Texte, edt);
        }

        public override void suivant(GameTime gameTime)
        {
            base.suivant(gameTime);

            TypeWriter.Update(gameTime);

            GE.Update(gameTime);
        }


        public void doShow()
        {
            GE.ajouter(Bulle, EffetsPredefinis.fadeInFrom0(255, 0, 250));
            GE.ajouter(Lieutenant, EffetsPredefinis.fadeInFrom0(255, 0, 250));
            GE.ajouter(TypeWriter.Texte, EffetsPredefinis.fadeInFrom0(255, 0, 250));
        }


        public void doHide()
        {
            GE.ajouter(Bulle, EffetsPredefinis.fadeOutTo0(255, 0, 250));
            GE.ajouter(Lieutenant, EffetsPredefinis.fadeOutTo0(255, 0, 250));
            GE.ajouter(TypeWriter.Texte, EffetsPredefinis.fadeOutTo0(255, 0, 250));
        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            Lieutenant.Draw(spriteBatch);
            Bulle.Draw(spriteBatch);
            TypeWriter.Texte.Draw(spriteBatch);
        }
    }
}
