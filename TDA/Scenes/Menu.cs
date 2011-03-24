namespace EphemereGames.Commander
{
    using System;
    using EphemereGames.Core.Input;
    using EphemereGames.Core.Visuel;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    
    class Menu : SceneMenu
    {
        private Main Main;
        private IVisible NouvellePartie;
        private IVisible QuitterPartie;
        private IVisible ReprendrePartie;
        private IVisible Aide;
        private IVisible Options;
        private IVisible Editeur;
        private IVisible TitreMenu;
        private IVisible NomJoueur;

        public String MusiqueSelectionnee;
        private double TempsEntreDeuxChangementMusique;

        private AnimationTransition AnimationTransition;
        private String ChoixTransition;

        private Simulation Simulation;


        public Menu(Main main)
            : base(Vector2.Zero, 720, 1280)
        {
            Main = main;

            Nom = "Menu";

            NouvellePartie = new IVisible
                    (
                        "save the\nworld",
                        EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"),
                        Color.White,
                        new Vector3(-220, -170, 0)
                    );
            NouvellePartie.Taille = 4f;
            NouvellePartie.VisualPriority = Preferences.PrioriteFondEcran - 0.01f;

            ReprendrePartie = new IVisible
                    (
                        "resume game",
                        EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"),
                        Color.White,
                        new Vector3(-460, 75, 0)
                    );
            ReprendrePartie.VisualPriority = Preferences.PrioriteFondEcran - 0.01f;
            ReprendrePartie.Taille = 4f;

            Options = new IVisible
                    (
                        "options\nn'stuff",
                        EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"),
                        Color.White,
                        new Vector3(-50, -200, 0)
                    );
            Options.Taille = 4f;
            Options.VisualPriority = Preferences.PrioriteFondEcran - 0.01f;

            Aide = new IVisible
                    (
                        "help",
                        EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"),
                        Color.White,
                        new Vector3(-350, 120, 0)
                    );
            Aide.Taille = 4f;
            Aide.VisualPriority = Preferences.PrioriteFondEcran - 0.01f;

            QuitterPartie = new IVisible
                    (
                        "quit",
                        EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"),
                        Color.White,
                        new Vector3(140, 160, 0)
                    );
            QuitterPartie.Taille = 4f;
            QuitterPartie.VisualPriority = Preferences.PrioriteFondEcran - 0.01f;

            Editeur = new IVisible
                    (
                        "editor",
                        EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"),
                        Color.White,
                        new Vector3(-600, 300, 0)
                    );
            Editeur.Taille = 4f;
            Editeur.VisualPriority = Preferences.PrioriteFondEcran - 0.01f;

            TitreMenu = new IVisible
                    (
                        "Commander",
                        EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("PixelBig"),
                        Color.White,
                        new Vector3(0, 0, 0)
                    );
            TitreMenu.Taille = 4;
            TitreMenu.Origine = TitreMenu.Centre;
            TitreMenu.VisualPriority = Preferences.PrioriteGUIMenuPrincipal;

            NomJoueur = new IVisible
                    (
                        "",
                        EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("PixelBig"),
                        Color.White,
                        new Vector3(0, 50, 0)
                    );
            NomJoueur.Taille = 3;
            NomJoueur.VisualPriority = Preferences.PrioriteGUIMenuPrincipal;

            DescripteurScenario descripteurScenario = FactoryScenarios.getDescripteurMenu();

#if !DEBUG
            descripteurScenario.SystemePlanetaire[5].Selectionnable = false;
#endif

            //Simulation = new Simulation(Main, this, FactoryScenarios.getDescripteurTestsPerformance());
            Simulation = new Simulation(Main, this, descripteurScenario);
            Simulation.PositionCurseur = new Vector3(400, 20, 0);
            Simulation.Players = Main.Players;
            Simulation.Initialize();
            Simulation.ModeDemo = true;

            AnimationTransition = new AnimationTransition(this, 500, Preferences.PrioriteTransitionScene);

            MusiqueSelectionnee = Main.MusiquesDisponibles[Main.Random.Next(0, Main.MusiquesDisponibles.Count)];
            Main.MusiquesDisponibles.Remove(MusiqueSelectionnee);
            TempsEntreDeuxChangementMusique = 0;

            Main.PlayersController.PlayerDisconnected += new NoneHandler(doJoueurPrincipalDeconnecte);
        }

        
        private void doJoueurPrincipalDeconnecte()
        {
            Transition = TransitionType.Out;
            AnimationTransition.Initialize();
        }


        protected override void UpdateLogique(GameTime gameTime)
        {
            TempsEntreDeuxChangementMusique -= gameTime.ElapsedGameTime.TotalMilliseconds;
            Simulation.Update(gameTime);
        }


        protected override void InitializeTransition(TransitionType type)
        {
            AnimationTransition.In = (type == TransitionType.In) ? true : false;
            AnimationTransition.Initialize();
        }


        protected override void UpdateTransition(GameTime gameTime)
        {
            AnimationTransition.Update(gameTime);

            if (!AnimationTransition.Finished(gameTime))
                return;

            if (Transition == TransitionType.Out)
                switch (ChoixTransition)
                {
                    case "save the\nworld":
                        EphemereGames.Core.Visuel.Facade.Transite("MenuToNouvellePartie");
                        break;

                    case "quit":
                        if (!Main.TrialMode.Active)
                            Main.Exit();
                        else
#if WINDOWS
                            Main.Exit();
#else
                            EphemereGames.Core.Visuel.Facade.effectuerTransition("MenuToAcheter");
#endif
                        break;

                    case "help":
                        EphemereGames.Core.Visuel.Facade.Transite("MenuToAide");
                        break;

                    case "options": EphemereGames.Core.Visuel.Facade.Transite("MenuToOptions");
                        break;

                    case "editor": EphemereGames.Core.Visuel.Facade.Transite("MenuToEditeur");
                        break;

                    case "resume game":
                        if (Main.GameInProgress != null && !Main.GameInProgress.EstTerminee)
                        {
                            Main.GameInProgress.State = GameState.Running;
                            EphemereGames.Core.Visuel.Facade.Transite("MenuToPartie");
                        }
                        break;

                    case "chargement": EphemereGames.Core.Visuel.Facade.Transite("MenuToChargement");
                        break;
                }

            Transition = TransitionType.None;
        }


        public void ChangeMusic()
        {
            if (TempsEntreDeuxChangementMusique > 0)
                return;

            EphemereGames.Core.Audio.Facade.arreterMusique(MusiqueSelectionnee, true, Preferences.TempsEntreDeuxChangementMusique - 50);
            String ancienneMusique = MusiqueSelectionnee;
            MusiqueSelectionnee = Main.MusiquesDisponibles[Main.Random.Next(0, Main.MusiquesDisponibles.Count)];
            Main.MusiquesDisponibles.Remove(MusiqueSelectionnee);
            Main.MusiquesDisponibles.Add(ancienneMusique);
            EphemereGames.Core.Audio.Facade.jouerMusique(MusiqueSelectionnee, true, 1000, true);
            TempsEntreDeuxChangementMusique = Preferences.TempsEntreDeuxChangementMusique;
        }


        protected override void UpdateVisuel()
        {
            if (Simulation.CorpsCelesteSelectionne != null)
            {
                switch (Simulation.CorpsCelesteSelectionne.Nom)
                {
                    case "save the\nworld":     ajouterScenable(NouvellePartie);    break;
                    case "quit":                ajouterScenable(QuitterPartie);     break;
                    case "help":                ajouterScenable(Aide);              break;
                    case "options":             ajouterScenable(Options);           break;
                    case "editor":
#if DEBUG
                        ajouterScenable(Editeur);
#endif
                        break;
                    case "resume game":
                        if (Main.GameInProgress != null && !Main.GameInProgress.EstTerminee)
                            ajouterScenable(ReprendrePartie);
                        break;
                }
            }

            ajouterScenable(TitreMenu);

            Simulation.Draw();

            if (Transition != TransitionType.None)
                AnimationTransition.Draw(null);
        }


        public override void onFocus()
        {
            base.onFocus();

            Transition = TransitionType.In;

            if (!EphemereGames.Core.Audio.Facade.musiqueJoue(MusiqueSelectionnee))
                EphemereGames.Core.Audio.Facade.jouerMusique(MusiqueSelectionnee, true, 1000, true);
            else
                EphemereGames.Core.Audio.Facade.reprendreMusique(MusiqueSelectionnee, true, 1000);

            EphemereGames.Core.Input.Facade.AddListener(Simulation);
        }

        public override void onFocusLost()
        {
            base.onFocusLost();

            if (Simulation.CorpsCelesteSelectionne != null && Simulation.CorpsCelesteSelectionne.Nom == "resume game" && Main.GameInProgress != null && !Main.GameInProgress.EstTerminee)
                EphemereGames.Core.Audio.Facade.pauserMusique(MusiqueSelectionnee, true, 1000);

            EphemereGames.Core.Input.Facade.RemoveListener(Simulation);
        }


        #region Input Handling

        public override void doMouseButtonPressedOnce(PlayerIndex inputIndex, MouseButton button)
        {
            Player p = Main.Players[inputIndex];

            if (!p.Master)
                return;

            if (button == p.MouseConfiguration.Select)
                beginTransition();
        }


        public override void doGamePadButtonPressedOnce(PlayerIndex inputIndex, Buttons button)
        {
            Player p = Main.Players[inputIndex];

            if (!p.Master)
                return;

            if (button == p.GamePadConfiguration.Select)
                beginTransition();
        }


        public override void doKeyPressedOnce(PlayerIndex inputIndex, Keys key)
        {
            Player p = Main.Players[inputIndex];

            if (!p.Master)
                return;

            if (key == p.KeyboardConfiguration.ChangeMusic)
                ChangeMusic();
        }


        private void beginTransition()
        {
            if (Transition != TransitionType.None)
                return;

            if (Simulation.CorpsCelesteSelectionne == null)
                return;

#if !DEBUG
            if (Simulation.CorpsCelesteSelectionne.Nom == "editor")
                return;
#endif

            if ((Simulation.CorpsCelesteSelectionne.Nom == "resume game" &&
                Main.GameInProgress != null &&
                !Main.GameInProgress.EstTerminee) ||
                
                (Simulation.CorpsCelesteSelectionne.Nom != "resume game"))
            {
                Transition = TransitionType.Out;
                ChoixTransition = Simulation.CorpsCelesteSelectionne.Nom;
            }
        }

        #endregion
    }
}
