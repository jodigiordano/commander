namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Core.Input;
    using Core.Visuel;
    using Core.Utilities;
    using Core.Physique;
    
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
        public Partie PartieEnCours;

        public String MusiqueSelectionnee;
        private double TempsEntreDeuxChangementMusique;

        private Objets.AnimationTransition AnimationTransition;
        private bool effectuerTransition;
        private String ChoixTransition;

        private Simulation Simulation;

        public Menu(Main main)
            : base(Vector2.Zero, 720, 1280)
        {
            Main = main;

            Nom = "Menu";
            EnPause = true;
            EstVisible = false;
            EnFocus = false;

            NouvellePartie = new IVisible
                    (
                        "save the\nworld",
                        Core.Persistance.Facade.recuperer<SpriteFont>("Pixelite"),
                        Color.White,
                        new Vector3(-220, -170, 0),
                        this
                    );
            NouvellePartie.Taille = 4f;
            NouvellePartie.PrioriteAffichage = Preferences.PrioriteFondEcran - 0.01f;

            ReprendrePartie = new IVisible
                    (
                        "resume game",
                        Core.Persistance.Facade.recuperer<SpriteFont>("Pixelite"),
                        Color.White,
                        new Vector3(-460, 75, 0),
                        this
                    );
            ReprendrePartie.PrioriteAffichage = Preferences.PrioriteFondEcran - 0.01f;
            ReprendrePartie.Taille = 4f;

            Options = new IVisible
                    (
                        "options\nn'stuff",
                        Core.Persistance.Facade.recuperer<SpriteFont>("Pixelite"),
                        Color.White,
                        new Vector3(-50, -200, 0),
                        this
                    );
            Options.Taille = 4f;
            Options.PrioriteAffichage = Preferences.PrioriteFondEcran - 0.01f;

            Aide = new IVisible
                    (
                        "help",
                        Core.Persistance.Facade.recuperer<SpriteFont>("Pixelite"),
                        Color.White,
                        new Vector3(-350, 120, 0),
                        this
                    );
            Aide.Taille = 4f;
            Aide.PrioriteAffichage = Preferences.PrioriteFondEcran - 0.01f;

            QuitterPartie = new IVisible
                    (
                        "quit",
                        Core.Persistance.Facade.recuperer<SpriteFont>("Pixelite"),
                        Color.White,
                        new Vector3(140, 160, 0),
                        this
                    );
            QuitterPartie.Taille = 4f;
            QuitterPartie.PrioriteAffichage = Preferences.PrioriteFondEcran - 0.01f;

            Editeur = new IVisible
                    (
                        "editor",
                        Core.Persistance.Facade.recuperer<SpriteFont>("Pixelite"),
                        Color.White,
                        new Vector3(-600, 300, 0),
                        this
                    );
            Editeur.Taille = 4f;
            Editeur.PrioriteAffichage = Preferences.PrioriteFondEcran - 0.01f;

            TitreMenu = new IVisible
                    (
                        "Commander",
                        Core.Persistance.Facade.recuperer<SpriteFont>("PixelBig"),
                        Color.White,
                        new Vector3(0, 0, 0),
                        this
                    );
            TitreMenu.Taille = 4;
            TitreMenu.Origine = TitreMenu.Centre;
            TitreMenu.PrioriteAffichage = Preferences.PrioriteGUIMenuPrincipal;

            NomJoueur = new IVisible
                    (
                        "",
                        Core.Persistance.Facade.recuperer<SpriteFont>("PixelBig"),
                        Color.White,
                        new Vector3(0, 50, 0),
                        this
                    );
            NomJoueur.Taille = 3;
            NomJoueur.PrioriteAffichage = Preferences.PrioriteGUIMenuPrincipal;

            DescripteurScenario descripteurScenario = FactoryScenarios.getDescripteurMenu();

#if !DEBUG
            descripteurScenario.SystemePlanetaire[5].Selectionnable = false;
#endif

            //Simulation = new Simulation(Main, this, FactoryScenarios.getDescripteurTestsPerformance());
            Simulation = new Simulation(Main, this, descripteurScenario);
            Simulation.PositionCurseur = new Vector3(400, 20, 0);
            Simulation.Initialize();
            Simulation.ModeDemo = true;

            AnimationTransition = new TDA.Objets.AnimationTransition();
            AnimationTransition.Duree = 500;
            AnimationTransition.Scene = this;
            AnimationTransition.PrioriteAffichage = Preferences.PrioriteTransitionScene;

            effectuerTransition = false;

            MusiqueSelectionnee = Main.MusiquesDisponibles[Main.Random.Next(0, Main.MusiquesDisponibles.Count)];
            Main.MusiquesDisponibles.Remove(MusiqueSelectionnee);
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
                        case "save the\nworld":
                            Core.Visuel.Facade.effectuerTransition("MenuVersNouvellePartie");
                            break;

                        case "quit":
                            if (!Main.ModeTrial.Actif)
                                Main.Exit();
                            else
#if WINDOWS
                                Main.Exit();
#else
                                Core.Visuel.Facade.effectuerTransition("MenuVersAcheter");
#endif
                            break;

                        case "help":
                            Core.Visuel.Facade.effectuerTransition("MenuVersAide");
                            break;

                        case "options": Core.Visuel.Facade.effectuerTransition("MenuVersOptions");
                            break;

                        case "editor":
                            //this.Partie = new Editeur(Main);
                            //Core.Visuel.Facade.mettreAJourScene("Editeur", new EditeurV2(Main));
                            Core.Visuel.Facade.effectuerTransition("MenuVersEditeur");
                            break;

                        case "resume game":
                            if (PartieEnCours != null && !PartieEnCours.EstTerminee)
                                Core.Visuel.Facade.effectuerTransition("MenuVersPartie");
                            break;

                        case "chargement": Core.Visuel.Facade.effectuerTransition("MenuVersChargement");
                            break;
                    }
                }
            }

            else if (Simulation.CorpsCelesteSelectionne != null && Core.Input.Facade.estPeseeUneSeuleFois(Preferences.toucheSelection, Main.JoueursConnectes[0].Manette, this.Nom))
            {
                effectuerTransition =
                    (Simulation.CorpsCelesteSelectionne.Nom == "resume game" && PartieEnCours != null && !PartieEnCours.EstTerminee ||
                    Simulation.CorpsCelesteSelectionne.Nom == "save the\nworld" ||
                    Simulation.CorpsCelesteSelectionne.Nom == "quit" ||
                    Simulation.CorpsCelesteSelectionne.Nom == "help" ||
                    Simulation.CorpsCelesteSelectionne.Nom == "options" ||
                    Simulation.CorpsCelesteSelectionne.Nom == "editor");
                AnimationTransition.In = false;
                AnimationTransition.Initialize();
                ChoixTransition = Simulation.CorpsCelesteSelectionne.Nom;
            }

            else if (Core.Input.Facade.estPeseeUneSeuleFois(Preferences.toucheChangerMusique, Main.JoueursConnectes[0].Manette, this.Nom) && TempsEntreDeuxChangementMusique <= 0)
            {
                changerMusique();
                TempsEntreDeuxChangementMusique = Preferences.TempsEntreDeuxChangementMusique;
            }

            TempsEntreDeuxChangementMusique -= gameTime.ElapsedGameTime.TotalMilliseconds;
            Simulation.Update(gameTime);

            //Vector2 positionSouris = Core.Input.Facade.positionSouris(Main.JoueursConnectes[0].Manette, this.Nom);
            //positionSouris += this.Camera.Origine;

            //if (!Core.Physique.Facade.collisionPointRectangle(ref positionSouris, Main.Fenetre))
            //{
            //    Vector2 nouvellePosition = Main.Fenetre.pointIntersection(new Ligne(this.Camera.Origine, positionSouris));
            //    Core.Input.Facade.setPositionSouris(Main.JoueursConnectes[0].Manette, this.Nom, ref nouvellePosition);
            //}
        }

        public void changerMusique()
        {
            Core.Audio.Facade.arreterMusique(MusiqueSelectionnee, true, Preferences.TempsEntreDeuxChangementMusique - 50);
            String ancienneMusique = MusiqueSelectionnee;
            MusiqueSelectionnee = Main.MusiquesDisponibles[Main.Random.Next(0, Main.MusiquesDisponibles.Count)];
            Main.MusiquesDisponibles.Remove(MusiqueSelectionnee);
            Main.MusiquesDisponibles.Add(ancienneMusique);
            Core.Audio.Facade.jouerMusique(MusiqueSelectionnee, true, 1000, true);
        }


        protected override void UpdateVisuel()
        {
            if (this.EnFocus)
            {
                if (Simulation.CorpsCelesteSelectionne != null)
                {
                    switch (Simulation.CorpsCelesteSelectionne.Nom)
                    {
                        case "save the\nworld":    ajouterScenable(NouvellePartie);    break;
                        case "quit":        ajouterScenable(QuitterPartie);     break;
                        case "help":        ajouterScenable(Aide);              break;
                        case "options":     ajouterScenable(Options);           break;
                        case "editor":      ajouterScenable(Editeur);           break;
                        case "resume game":
                            if (PartieEnCours != null && !PartieEnCours.EstTerminee)
                                ajouterScenable(ReprendrePartie);
                            break;
                    }
                }

                ajouterScenable(TitreMenu);

                //NomJoueur.Texte = Main.JoueursConnectes[0].JoueurPhysique.Gamertag;
                //NomJoueur.position.X = 525 - NomJoueur.Rectangle.Width;
                //ajouterScenable(NomJoueur);

                Simulation.Draw(null);

                if (effectuerTransition)
                    AnimationTransition.Draw(null);
            }
        }


        public override void onFocus()
        {
            base.onFocus();

            effectuerTransition = true;
            AnimationTransition.In = true;
            AnimationTransition.Initialize();

            if (!Core.Audio.Facade.musiqueJoue(MusiqueSelectionnee))
                Core.Audio.Facade.jouerMusique(MusiqueSelectionnee, true, 1000, true);
            else
                Core.Audio.Facade.reprendreMusique(MusiqueSelectionnee, true, 1000);
        }

        public override void onFocusLost()
        {
            base.onFocusLost();

            if (Simulation.CorpsCelesteSelectionne != null && Simulation.CorpsCelesteSelectionne.Nom == "resume game" && PartieEnCours != null && !PartieEnCours.EstTerminee)
                Core.Audio.Facade.pauserMusique(MusiqueSelectionnee, true, 1000);
        }
    }
}
