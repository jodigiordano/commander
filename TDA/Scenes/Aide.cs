namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Core.Input;
    using Core.Visuel;
    using Core.Utilities;

    class Aide : Scene
    {
        private Main Main;
        //private IVisible Retour;
        private IVisible FondEcran;
        private IVisible Bulle;
        private IVisible Lieutenant;
        private TextTypeWriter TypeWriter;
        private List<KeyValuePair<IVisible,IVisible>> TitresSlides;
        private HorizontalSlider SlidesSlider;
        private Curseur Curseur;

        private Objets.AnimationTransition AnimationTransition;
        private bool effectuerTransition;
        private String ChoixTransition;
        private double TempsEntreDeuxChangementMusique;

        public Aide(Main main)
            : base(Vector2.Zero, 720, 1280)
        {
            Main = main;

            Nom = "Aide";
            EnPause = true;
            EstVisible = false;
            EnFocus = false;

            //Retour = new IVisible(Core.Persistance.Facade.recuperer<Texture2D>("pressBack"), new Vector3(-585, -335, 0), this);
            //Retour.PrioriteAffichage = Preferences.PrioriteGUIMenuPrincipal;

            Lieutenant = new IVisible(Core.Persistance.Facade.recuperer<Texture2D>("lieutenant"), new Vector3(120, -420, 0), this);
            Lieutenant.Taille = 8;
            Lieutenant.Origine = new Vector2(0, Lieutenant.Texture.Height);
            Lieutenant.Rotation = MathHelper.Pi;
            Lieutenant.PrioriteAffichage = Preferences.PrioriteGUIMenuPrincipal;

            Bulle = new IVisible(Core.Persistance.Facade.recuperer<Texture2D>("bulleRenversee"), new Vector3(80, -150, 0), this);
            Bulle.Taille = 8;
            Bulle.PrioriteAffichage = Preferences.PrioriteGUIMenuPrincipal + 0.02f;

            FondEcran = new IVisible(Core.Persistance.Facade.recuperer<Texture2D>("fondecran7"), Vector3.Zero, this);
            FondEcran.Origine = FondEcran.Centre;
            FondEcran.PrioriteAffichage = Preferences.PrioriteGUIMenuPrincipal + 0.03f;

            TypeWriter = new TextTypeWriter
            (
                Main,
                "",
                Color.Black,
                new Vector3(170, -130, 0),
                Core.Persistance.Facade.recuperer<SpriteFont>("Pixelite"),
                2.0f,
                new Vector2(370, 330),
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
                this
            );
            TypeWriter.Texte.PrioriteAffichage = Preferences.PrioriteGUIMenuPrincipal;

            Curseur = new Curseur(Main, this, Vector3.Zero, 10, Preferences.PrioriteGUIMenuPrincipal);

            TitresSlides = new List<KeyValuePair<IVisible, IVisible>>();

            IVisible titre, slide;
            
            titre = new IVisible("Help:Controls", Core.Persistance.Facade.recuperer<SpriteFont>("Pixelite"), Color.White, new Vector3(0, -300, 0), this);
            titre.Taille = 4;
            titre.Origine = titre.Centre;
            titre.PrioriteAffichage = Preferences.PrioriteGUIMenuPrincipal + 0.01f;

#if WINDOWS && !MANETTE_WINDOWS
            slide = new IVisible(Core.Persistance.Facade.recuperer<Texture2D>("HelpControlsWin"), new Vector3(0, 50, 0), this);
#else
            slide = new IVisible(Core.Persistance.Facade.recuperer<Texture2D>("HelpControls"), new Vector3(0, 50, 0), this);
#endif
            
            slide.Origine = slide.Centre;
            slide.PrioriteAffichage = Preferences.PrioriteGUIMenuPrincipal + 0.01f;

            TitresSlides.Add(new KeyValuePair<IVisible,IVisible>(titre, slide));

            titre = new IVisible("Help:Battlefield", Core.Persistance.Facade.recuperer<SpriteFont>("Pixelite"), Color.White, new Vector3(0, -300, 0), this);
            titre.Taille = 4;
            titre.Origine = titre.Centre;
            titre.PrioriteAffichage = Preferences.PrioriteGUIMenuPrincipal + 0.01f;

            slide = new IVisible(Core.Persistance.Facade.recuperer<Texture2D>("HelpBattlefield"), new Vector3(0, 50, 0), this);
            slide.Origine = slide.Centre;
            slide.PrioriteAffichage = Preferences.PrioriteGUIMenuPrincipal + 0.01f;

            TitresSlides.Add(new KeyValuePair<IVisible, IVisible>(titre, slide));

            titre = new IVisible("Help:Mercenaries", Core.Persistance.Facade.recuperer<SpriteFont>("Pixelite"), Color.White, new Vector3(0, -300, 0), this);
            titre.Taille = 4;
            titre.Origine = titre.Centre;
            titre.PrioriteAffichage = Preferences.PrioriteGUIMenuPrincipal + 0.01f;

            slide = new IVisible(Core.Persistance.Facade.recuperer<Texture2D>("HelpMercenaries"), new Vector3(0, 50, 0), this);
            slide.Origine = slide.Centre;
            slide.PrioriteAffichage = Preferences.PrioriteGUIMenuPrincipal + 0.01f;

            TitresSlides.Add(new KeyValuePair<IVisible, IVisible>(titre, slide));

            titre = new IVisible("Help:The Resistance", Core.Persistance.Facade.recuperer<SpriteFont>("Pixelite"), Color.White, new Vector3(0, -300, 0), this);
            titre.Taille = 4;
            titre.Origine = titre.Centre;
            titre.PrioriteAffichage = Preferences.PrioriteGUIMenuPrincipal + 0.01f;

            slide = new IVisible(Core.Persistance.Facade.recuperer<Texture2D>("HelpTheResistance"), new Vector3(0, 50, 0), this);
            slide.Origine = slide.Centre;
            slide.PrioriteAffichage = Preferences.PrioriteGUIMenuPrincipal + 0.01f;

            TitresSlides.Add(new KeyValuePair<IVisible, IVisible>(titre, slide));

            SlidesSlider = new HorizontalSlider(Main, this, Curseur, new Vector3(0, -250, 0), 0, 3, 0, 1, Preferences.PrioriteGUIMenuPrincipal + 0.01f);

            AnimationTransition = new TDA.Objets.AnimationTransition();
            AnimationTransition.Duree = 500;
            AnimationTransition.Scene = this;
            AnimationTransition.PrioriteAffichage = Preferences.PrioriteTransitionScene;

            effectuerTransition = false;

            TempsEntreDeuxChangementMusique = 0;

            Main.ControleurJoueursConnectes.JoueurPrincipalDeconnecte += new ControleurJoueursConnectes.JoueurPrincipalDeconnecteHandler(doJoueurPrincipalDeconnecte);
        }

        private void doJoueurPrincipalDeconnecte()
        {
            AnimationTransition.In = false;
            AnimationTransition.Initialize();
            ChoixTransition = "chargement";
            effectuerTransition = true;
        }


        protected override void UpdateLogique(GameTime gameTime)
        {
            if (effectuerTransition)
            {
                AnimationTransition.suivant(gameTime);

                effectuerTransition = !AnimationTransition.estTerminee(gameTime);

                if (!effectuerTransition && !AnimationTransition.In)
                {
                    switch (ChoixTransition)
                    {
                        case "menu": Core.Visuel.Facade.effectuerTransition("AideVersMenu"); break;
                        case "chargement": Core.Visuel.Facade.effectuerTransition("AideVersChargement"); break;
                    }
                }
            }

            else if (Core.Input.Facade.estPeseeUneSeuleFois(Preferences.toucheRetour, Main.JoueursConnectes[0].Manette, this.Nom) ||
                     Core.Input.Facade.estPeseeUneSeuleFois(Preferences.toucheRetourMenu2, Main.JoueursConnectes[0].Manette, this.Nom))
            {
                effectuerTransition = true;
                ChoixTransition = "menu";
                AnimationTransition.In = false;
                AnimationTransition.Initialize();
            }

            else
            {
                if (Core.Input.Facade.estPeseeUneSeuleFois(Preferences.toucheChangerMusique, Main.JoueursConnectes[0].Manette, this.Nom) && TempsEntreDeuxChangementMusique <= 0)
                {
                    Menu menu = (Menu)Core.Visuel.Facade.recupererScene("Menu");
                    menu.changerMusique();
                    TempsEntreDeuxChangementMusique = Preferences.TempsEntreDeuxChangementMusique;
                }

                TempsEntreDeuxChangementMusique -= gameTime.ElapsedGameTime.TotalMilliseconds;
                //TypeWriter.Update(gameTime);

                SlidesSlider.Update(gameTime);
                Curseur.Update(gameTime);
            }
        }


        protected override void UpdateVisuel()
        {
            //ajouterScenable(Lieutenant);
            //ajouterScenable(Bulle);
            //ajouterScenable(Retour);
            ajouterScenable(FondEcran);
            //ajouterScenable(TypeWriter.Texte);
            ajouterScenable(TitresSlides[SlidesSlider.Valeur].Key);
            ajouterScenable(TitresSlides[SlidesSlider.Valeur].Value);
            SlidesSlider.Draw(null);
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
    }
}
