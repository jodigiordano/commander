namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using EphemereGames.Core.Visuel;
    using Microsoft.Xna.Framework.Input;
    using EphemereGames.Core.Input;

    class Aide : Scene
    {
        private Main Main;
        private IVisible FondEcran;
        private IVisible Bulle;
        private IVisible Lieutenant;
        private TextTypeWriter TypeWriter;
        private List<KeyValuePair<IVisible,IVisible>> TitresSlides;
        private HorizontalSlider SlidesSlider;
        private Cursor Curseur;

        private AnimationTransition AnimationTransition;
        private String ChoixTransition;
        private double TempsEntreDeuxChangementMusique;

        public Aide(Main main)
            : base(Vector2.Zero, 720, 1280)
        {
            Main = main;

            Nom = "Aide";

            Lieutenant = new IVisible(EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>("lieutenant"), new Vector3(120, -420, 0));
            Lieutenant.Taille = 8;
            Lieutenant.Origine = new Vector2(0, Lieutenant.Texture.Height);
            Lieutenant.Rotation = MathHelper.Pi;
            Lieutenant.VisualPriority = Preferences.PrioriteGUIMenuPrincipal;

            Bulle = new IVisible(EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>("bulleRenversee"), new Vector3(80, -150, 0));
            Bulle.Taille = 8;
            Bulle.VisualPriority = Preferences.PrioriteGUIMenuPrincipal + 0.02f;

            FondEcran = new IVisible(EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>("fondecran7"), Vector3.Zero);
            FondEcran.Origine = FondEcran.Centre;
            FondEcran.VisualPriority = Preferences.PrioriteGUIMenuPrincipal + 0.03f;

            TypeWriter = new TextTypeWriter
            (
                Main,
                "",
                Color.Black,
                new Vector3(170, -130, 0),
                EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"),
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
            TypeWriter.Texte.VisualPriority = Preferences.PrioriteGUIMenuPrincipal;

            Curseur = new Cursor(Main, this, Vector3.Zero, 10, Preferences.PrioriteGUIMenuPrincipal);

            TitresSlides = new List<KeyValuePair<IVisible, IVisible>>();

            IVisible titre, slide;
            
            titre = new IVisible("Help:Controls", EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"), Color.White, new Vector3(0, -300, 0));
            titre.Taille = 4;
            titre.Origine = titre.Centre;
            titre.VisualPriority = Preferences.PrioriteGUIMenuPrincipal + 0.01f;

#if WINDOWS && !MANETTE_WINDOWS
            slide = new IVisible(EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>("HelpControlsWin"), new Vector3(0, 50, 0));
#else
            slide = new IVisible(EphemereGames.Core.Persistance.Facade.recuperer<Texture2D>("HelpControls"), new Vector3(0, 50, 0));
#endif
            
            slide.Origine = slide.Centre;
            slide.VisualPriority = Preferences.PrioriteGUIMenuPrincipal + 0.01f;

            TitresSlides.Add(new KeyValuePair<IVisible,IVisible>(titre, slide));

            titre = new IVisible("Help:Battlefield", EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"), Color.White, new Vector3(0, -300, 0));
            titre.Taille = 4;
            titre.Origine = titre.Centre;
            titre.VisualPriority = Preferences.PrioriteGUIMenuPrincipal + 0.01f;

            slide = new IVisible(EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>("HelpBattlefield"), new Vector3(0, 50, 0));
            slide.Origine = slide.Centre;
            slide.VisualPriority = Preferences.PrioriteGUIMenuPrincipal + 0.01f;

            TitresSlides.Add(new KeyValuePair<IVisible, IVisible>(titre, slide));

            titre = new IVisible("Help:Mercenaries", EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"), Color.White, new Vector3(0, -300, 0));
            titre.Taille = 4;
            titre.Origine = titre.Centre;
            titre.VisualPriority = Preferences.PrioriteGUIMenuPrincipal + 0.01f;

            slide = new IVisible(EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>("HelpMercenaries"), new Vector3(0, 50, 0));
            slide.Origine = slide.Centre;
            slide.VisualPriority = Preferences.PrioriteGUIMenuPrincipal + 0.01f;

            TitresSlides.Add(new KeyValuePair<IVisible, IVisible>(titre, slide));

            titre = new IVisible("Help:The Resistance", EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"), Color.White, new Vector3(0, -300, 0));
            titre.Taille = 4;
            titre.Origine = titre.Centre;
            titre.VisualPriority = Preferences.PrioriteGUIMenuPrincipal + 0.01f;

            slide = new IVisible(EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>("HelpTheResistance"), new Vector3(0, 50, 0));
            slide.Origine = slide.Centre;
            slide.VisualPriority = Preferences.PrioriteGUIMenuPrincipal + 0.01f;

            TitresSlides.Add(new KeyValuePair<IVisible, IVisible>(titre, slide));

            SlidesSlider = new HorizontalSlider(Main, this, Curseur, new Vector3(0, -250, 0), 0, 3, 0, 1, Preferences.PrioriteGUIMenuPrincipal + 0.01f);

            AnimationTransition = new AnimationTransition(this, 500, Preferences.PrioriteTransitionScene);

            TempsEntreDeuxChangementMusique = 0;

            Main.PlayersController.PlayerDisconnected += new NoneHandler(doJoueurPrincipalDeconnecte);
        }

        private void doJoueurPrincipalDeconnecte()
        {
            ChoixTransition = "chargement";
            Transition = TransitionType.Out;
        }


        protected override void UpdateLogique(GameTime gameTime)
        {
            if (Transition != TransitionType.None)
                return;

            TempsEntreDeuxChangementMusique -= gameTime.ElapsedGameTime.TotalMilliseconds;
            SlidesSlider.Update(gameTime);
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
                    switch (ChoixTransition)
                    {
                        case "menu": EphemereGames.Core.Visuel.Facade.Transite("AideToMenu"); break;
                        case "chargement": EphemereGames.Core.Visuel.Facade.Transite("AideToChargement"); break;
                    }

                Transition = TransitionType.None;
            }
        }


        protected override void UpdateVisuel()
        {
            ajouterScenable(FondEcran);
            ajouterScenable(TitresSlides[SlidesSlider.Valeur].Key);
            ajouterScenable(TitresSlides[SlidesSlider.Valeur].Value);
            SlidesSlider.Draw(null);
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
                SlidesSlider.doClick();

            if (button == p.MouseConfiguration.Back)
                beginTransition();
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

            if (key == p.KeyboardConfiguration.Back || key == p.KeyboardConfiguration.Cancel)
                beginTransition();

            if (key == p.KeyboardConfiguration.ChangeMusic)
                beginChangeMusic();
        }


        public override void doGamePadButtonPressedOnce(PlayerIndex inputIndex, Buttons button)
        {
            Player p = Main.Players[inputIndex];

            if (!p.Master)
                return;

            if (button == p.GamePadConfiguration.Select)
                SlidesSlider.doClick();

            if (button == p.GamePadConfiguration.Cancel)
                beginTransition();

            if (button == p.GamePadConfiguration.ChangeMusic)
                beginChangeMusic();
        }


        private void beginTransition()
        {
            if (Transition != TransitionType.None)
                return;

            Transition = TransitionType.Out;
            ChoixTransition = "menu";
        }


        private void beginChangeMusic()
        {
            if (TempsEntreDeuxChangementMusique > 0)
                return;

            Menu menu = (Menu)EphemereGames.Core.Visuel.Facade.GetScene("Menu");
            menu.ChangeMusic();
            TempsEntreDeuxChangementMusique = Preferences.TempsEntreDeuxChangementMusique;
        }
    }
}
