namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using Core.Visuel;
    using Core.Physique;

    class PlayerController
    {
        public CorpsCeleste CorpsCelesteAProteger;
        public Chemin Chemin;
        public Chemin CheminProjection;
        public List<CorpsCeleste> CorpsCelestes;
        public List<Ennemi> Ennemis;
        public JoueurCommun Joueur;
        public Dictionary<PowerUp, bool> OptionsCVDisponibles;
        public Sablier SandGlass;
        public Cursor Cursor;
        public bool ModeDemo;


        // Donnees externes obtenues via des evenements
        private int VaguesRestantes;
        private double TempsProchaineVague;


        // Selection actuelle
        public CorpsCeleste CorpsSelectionne;
        private Emplacement EmplacementSelectionne;
        private Dictionary<int, bool> OptionsPourTourellesAchetees;
        private int OptionPourTourelleAcheteeSelectionne;
        private Dictionary<PowerUp, bool> OptionsPourCorpsCeleste;
        private PowerUp OptionPourCorpsCelesteSelectionne;
        private SelectedCelestialBodyController SelectedCelestialBodyController;
        private ControleurNavigationTourellesAchat NavigationTourellesAchat;

        //TMP: Destruction
        private CorpsCeleste ProjectionEnCours;
        private IVisible ZoneDestruction;

        // GUI
        private PanneauCorpsCeleste PanneauCorpsCeleste;
        public Bulle BulleGUI { get { return PanneauCorpsCeleste.Bulle; } }

        public delegate void CelestialObjectHandler(CorpsCeleste celestialObject);
        public event CelestialObjectHandler AchatDoItYourselfDemande;
        public event CelestialObjectHandler DestructionCorpsCelesteDemande;
        public event CelestialObjectHandler AchatCollecteurDemande;
        public event CelestialObjectHandler AchatTheResistanceDemande;
        public event CelestialObjectHandler SelectedCelestialBodyChanged;

        public delegate void TurretTypeCelestialObjectTurretSpotHandler(TypeTourelle typeTourelle, CorpsCeleste corpsCeleste, Emplacement emplacement);
        public event TurretTypeCelestialObjectTurretSpotHandler AchatTourelleDemande;

        public delegate void CelestialObjectTurretSpotHandler(CorpsCeleste corpsCeleste, Emplacement emplacement);
        public event CelestialObjectTurretSpotHandler VenteTourelleDemande;
        public event CelestialObjectTurretSpotHandler MiseAJourTourelleDemande;

        public delegate void NoneHandler();
        public event NoneHandler ProchaineVagueDemandee;
        
        public delegate void IntegerHandler(int score);
        public event IntegerHandler ScoreChanged;
        public event IntegerHandler CashChanged;


        private Simulation Simulation;


        public PlayerController(Simulation simulation)
        {
            this.Simulation = simulation;

            CorpsCelestes = new List<CorpsCeleste>();

            PanneauCorpsCeleste = new PanneauCorpsCeleste(Simulation);
        }


        public void Initialize()
        {
            OptionsPourTourellesAchetees = new Dictionary<int, bool>();
            OptionsPourCorpsCeleste = new Dictionary<PowerUp, bool>();

            NavigationTourellesAchat = new ControleurNavigationTourellesAchat(Joueur, CheminProjection);
            SelectedCelestialBodyController = new SelectedCelestialBodyController(Simulation, CorpsCelestes, Cursor);
            PanneauCorpsCeleste.Initialize();
            PanneauCorpsCeleste.TourellesAchat = NavigationTourellesAchat.Tourelles;
            PanneauCorpsCeleste.OptionsPourTourelleAchetee = OptionsPourTourellesAchetees;
            PanneauCorpsCeleste.OptionsPourCorpsCelesteSelectionne = OptionsPourCorpsCeleste;

            CorpsSelectionne = null;
            EmplacementSelectionne = null; //-1;

            ZoneDestruction = new IVisible(Core.Persistance.Facade.recuperer<Texture2D>("CercleBlanc"), Vector3.Zero, Simulation.Scene);
            ZoneDestruction.Couleur = new Color(Color.Red, 100);
            ZoneDestruction.Origine = ZoneDestruction.Centre;
            ZoneDestruction.PrioriteAffichage = Preferences.PrioriteGUIEtoiles - 0.002f;

            SelectedCelestialBodyController.SelectedCelestialBodyChanged += new SelectedCelestialBodyController.SelectedCelestialBodyChangedHandler(doSelectedCelestialBodyChanged);
            SelectedCelestialBodyController.SelectedTurretSpotChanged += new SelectedCelestialBodyController.SelectedTurretSpotChangedHandler(doSelectedTurretSpotChanged);

            setWidgets();

            notifyCashChanged(Joueur.ReserveUnites);
            notifyScoreChanged(Joueur.Pointage);
        }


        private void doSelectedTurretSpotChanged(Emplacement turretSpot)
        {
            this.EmplacementSelectionne = turretSpot;
            this.OptionPourTourelleAcheteeSelectionne = 1;
        }


        private void doSelectedCelestialBodyChanged(CorpsCeleste celestialBody)
        {
            this.CorpsSelectionne = celestialBody;

            notifySelectedCelestialBodyChanged(celestialBody);
        }


        private void notifySelectedCelestialBodyChanged(CorpsCeleste celestialBody)
        {
            if (SelectedCelestialBodyChanged != null)
                SelectedCelestialBodyChanged(celestialBody);
        }


        private void notifyDestructionCorpsCelesteDemande(CorpsCeleste corpsCeleste)
        {
            if (DestructionCorpsCelesteDemande != null)
                DestructionCorpsCelesteDemande(corpsCeleste);
        }


        private void notifyCashChanged(int cash)
        {
            if (CashChanged != null)
                CashChanged(cash);
        }


        private void notifyScoreChanged(int score)
        {
            if (ScoreChanged != null)
                ScoreChanged(score);
        }


        private void notifyDoItYourselfDemande(CorpsCeleste corpsCeleste)
        {
            if (AchatDoItYourselfDemande != null)
                AchatDoItYourselfDemande(corpsCeleste);
        }


        private void notifyAchatTourelleDemande(TypeTourelle typeTourelle, CorpsCeleste corpsCeleste, Emplacement emplacement)
        {
            if (AchatTourelleDemande != null)
                AchatTourelleDemande(typeTourelle, corpsCeleste, emplacement);
        }


        private void notifyVenteTourelleDemande(CorpsCeleste corpsCeleste, Emplacement emplacement)
        {
            if (VenteTourelleDemande != null)
                VenteTourelleDemande(corpsCeleste, emplacement);
        }


        private void notifyAchatCollecteurDemande(CorpsCeleste corpsCeleste)
        {
            if (AchatCollecteurDemande != null)
                AchatCollecteurDemande(corpsCeleste);
        }


        private void notifyAchatTheResistanceDemande(CorpsCeleste corpsCeleste)
        {
            if (AchatTheResistanceDemande != null)
                AchatTheResistanceDemande(corpsCeleste);
        }


        private void notifyMiseAJourTourelleDemande(CorpsCeleste corpsCeleste, Emplacement emplacement)
        {
            if (MiseAJourTourelleDemande != null)
                MiseAJourTourelleDemande(corpsCeleste, emplacement);
        }


        private void notifyProchaineVagueDemandee()
        {
            if (ProchaineVagueDemandee != null)
                ProchaineVagueDemandee();
        }


        public void doObjetDetruit(IObjetPhysique objet)
        {
            Ennemi ennemi = objet as Ennemi;

            if (ennemi != null)
            {
                Joueur.ReserveUnites += ennemi.ValeurUnites;
                Joueur.Pointage += ennemi.ValeurPoints;

                notifyCashChanged(Joueur.ReserveUnites);
                notifyScoreChanged(Joueur.Pointage);

                return;
            }


            CorpsCeleste corpsCeleste = objet as CorpsCeleste;

            if (corpsCeleste != null)
            {
                if (CorpsSelectionne == corpsCeleste)
                {
                    SelectedCelestialBodyController.doCelestialBodyDestroyed();
                    EmplacementSelectionne = null; //-1;
                    NavigationTourellesAchat.CorpsCelesteSelectionne = CorpsSelectionne;
                    NavigationTourellesAchat.EmplacementSelectionne = EmplacementSelectionne;
                    NavigationTourellesAchat.Update(null);
                    ProjectionEnCours = null;
                    setWidgets();
                }

                return;
            }


            Mineral mineral = objet as Mineral;

            if (mineral != null)
            {
                if (mineral.Type == 3)
                {
                    Joueur.PointsDeVie += mineral.Valeur;
                    CorpsCelesteAProteger.PointsVie += mineral.Valeur;
                }
                else
                {
                    Joueur.ReserveUnites += mineral.Valeur;
                    notifyCashChanged(Joueur.ReserveUnites);
                }

                return;
            }
        }


        public void Update(GameTime gameTime)
        {
            SelectedCelestialBodyController.Update(gameTime);
            NavigationTourellesAchat.CorpsCelesteSelectionne = CorpsSelectionne;
            NavigationTourellesAchat.EmplacementSelectionne = EmplacementSelectionne;
            NavigationTourellesAchat.Update(gameTime);
            OptionPourCorpsCelesteSelectionne = (CorpsSelectionne != null && EmplacementSelectionne == null) ? OptionPourCorpsCelesteSelectionne : PowerUp.Aucune;

            if (CorpsSelectionne != null && EmplacementSelectionne != null && EmplacementSelectionne.EstOccupe)
                determinerOptionsDisponiblesPourTourellesAchetees();
            else if (CorpsSelectionne != null && EmplacementSelectionne == null)
                determinerOptionsDisponiblesPourCorpsCeleste();


            // Cycler dans les choix des tourelles a acheter (Droite 2)
            if (Core.Input.Facade.estPeseeUneSeuleFois(Preferences.toucheSelectionSuivant, Simulation.Main.JoueursConnectes[0].Manette, Simulation.Scene.Nom))
                NavigationTourellesAchat.Suivant();

            // Cycler dans les choix des tourelles a acheter (Gauche 2)
            if (Core.Input.Facade.estPeseeUneSeuleFois(Preferences.toucheSelectionPrecedent, Simulation.Main.JoueursConnectes[0].Manette, Simulation.Scene.Nom))
                NavigationTourellesAchat.Precedent();

            // Cycler dans les choix des tourelles actives (Droite 2)
            if (EmplacementSelectionne != null &&
                EmplacementSelectionne.EstOccupe &&
                OptionsPourTourellesAchetees.Count != 0 &&
                Core.Input.Facade.estPeseeUneSeuleFois(Preferences.toucheSelectionSuivant, Simulation.Main.JoueursConnectes[0].Manette, Simulation.Scene.Nom))
            {
                OptionPourTourelleAcheteeSelectionne = (OptionPourTourelleAcheteeSelectionne + 1) % OptionsPourTourellesAchetees.Count;

                //ark ark ark
                if (!OptionsPourTourellesAchetees[OptionPourTourelleAcheteeSelectionne])
                    OptionPourTourelleAcheteeSelectionne = (OptionPourTourelleAcheteeSelectionne + 1) % OptionsPourTourellesAchetees.Count;
            }

            // Cycler dans les choix des tourelles actives (Gauche 2)
            if (EmplacementSelectionne != null &&
                EmplacementSelectionne.EstOccupe &&
                OptionsPourTourellesAchetees.Count != 0 &&
                Core.Input.Facade.estPeseeUneSeuleFois(Preferences.toucheSelectionPrecedent, Simulation.Main.JoueursConnectes[0].Manette, Simulation.Scene.Nom))
            {
                OptionPourTourelleAcheteeSelectionne -= 1;

                if (OptionPourTourelleAcheteeSelectionne == -1)
                    OptionPourTourelleAcheteeSelectionne = OptionsPourTourellesAchetees.Count - 1;

                //ark ark ark
                if (!OptionsPourTourellesAchetees[OptionPourTourelleAcheteeSelectionne])
                    OptionPourTourelleAcheteeSelectionne -= 1;
            }


            // Cycler dans les choix du corps celeste actif
            if (CorpsSelectionne != null &&
                EmplacementSelectionne == null &&
                OptionsPourCorpsCeleste.Count != 0 &&
                Core.Input.Facade.estPeseeUneSeuleFois(Preferences.toucheSelectionSuivant, Simulation.Main.JoueursConnectes[0].Manette, Simulation.Scene.Nom))
            {
                OptionPourCorpsCelesteSelectionne = ProchainPowerUpDisponiblePourCorpsCeleste;
            }


            // Cycler dans les choix du corps celeste actif
            if (!ModeDemo &&
                CorpsSelectionne != null &&
                EmplacementSelectionne == null &&
                OptionsPourCorpsCeleste.Count != 0 &&
                Core.Input.Facade.estPeseeUneSeuleFois(Preferences.toucheSelectionPrecedent, Simulation.Main.JoueursConnectes[0].Manette, Simulation.Scene.Nom))
            {
                OptionPourCorpsCelesteSelectionne = PrecedentPowerUpDisponiblePourCorpsCeleste;
            }

            // Acheter un vaisseau Do-It-Yourself
            if (!ModeDemo &&
                CorpsSelectionne != null &&
                EmplacementSelectionne == null &&
                OptionPourCorpsCelesteSelectionne == 0 &&
                Core.Input.Facade.estPeseeUneSeuleFois(Preferences.toucheSelection, Simulation.Main.JoueursConnectes[0].Manette, Simulation.Scene.Nom))
            {
                notifyDoItYourselfDemande(CorpsSelectionne);
                this.Joueur.ReserveUnites -= CorpsSelectionne.PrixDoItYourself;
                notifyCashChanged(Joueur.ReserveUnites);
            }


            // Acheter un vaisseau Collecteur
            if (!ModeDemo && 
                CorpsSelectionne != null &&
                EmplacementSelectionne == null &&
                OptionPourCorpsCelesteSelectionne == PowerUp.CollectTheRent &&
                Core.Input.Facade.estPeseeUneSeuleFois(Preferences.toucheSelection, Simulation.Main.JoueursConnectes[0].Manette, Simulation.Scene.Nom))
            {
                notifyAchatCollecteurDemande(CorpsSelectionne);
                this.Joueur.ReserveUnites -= CorpsSelectionne.PrixCollecteur;
                notifyCashChanged(Joueur.ReserveUnites);

                Core.Input.Facade.ignorerToucheCeTick(Simulation.Main.JoueursConnectes[0].Manette, Preferences.toucheSelection);
            }

            // Detruire un corps celeste
            if (!ModeDemo &&
                CorpsSelectionne != null &&
                EmplacementSelectionne == null &&
                OptionPourCorpsCelesteSelectionne == PowerUp.FinalSolution &&
                Core.Input.Facade.estPeseeUneSeuleFois(Preferences.toucheSelection, Simulation.Main.JoueursConnectes[0].Manette, Simulation.Scene.Nom))
            {
                this.Joueur.ReserveUnites -= CorpsSelectionne.PrixDestruction;
                notifyCashChanged(Joueur.ReserveUnites);
                OptionPourCorpsCelesteSelectionne = PowerUp.Aucune;
                notifyDestructionCorpsCelesteDemande(CorpsSelectionne);
            }

            // Utiliser les services de la resistance
            if (!ModeDemo &&
                CorpsSelectionne != null &&
                EmplacementSelectionne == null &&
                OptionPourCorpsCelesteSelectionne == PowerUp.TheResistance &&
                Core.Input.Facade.estPeseeUneSeuleFois(Preferences.toucheSelection, Simulation.Main.JoueursConnectes[0].Manette, Simulation.Scene.Nom))
            {
                notifyAchatTheResistanceDemande(CorpsSelectionne);
                this.Joueur.ReserveUnites -= CorpsSelectionne.PrixTheResistance;
                notifyCashChanged(Joueur.ReserveUnites);

                Core.Input.Facade.ignorerToucheCeTick(Simulation.Main.JoueursConnectes[0].Manette, Preferences.toucheSelection);
            }

            // Acheter une tourelle
            if (NavigationTourellesAchat.Selectionne != null &&
                Core.Input.Facade.estPeseeUneSeuleFois(Preferences.toucheSelection, Simulation.Main.JoueursConnectes[0].Manette, Simulation.Scene.Nom))
            {
                notifyAchatTourelleDemande(NavigationTourellesAchat.Selectionne.Type, CorpsSelectionne, EmplacementSelectionne);
            }

            // Vendre une tourelle
            else if (EmplacementSelectionne != null &&
                EmplacementSelectionne.EstOccupe &&
                OptionPourTourelleAcheteeSelectionne == 0 &&
                Core.Input.Facade.estPeseeUneSeuleFois(Preferences.toucheSelection, Simulation.Main.JoueursConnectes[0].Manette, Simulation.Scene.Nom))
                notifyVenteTourelleDemande(CorpsSelectionne, EmplacementSelectionne);

            // Mettre a jour une tourelle
            else if (EmplacementSelectionne != null &&
                EmplacementSelectionne.EstOccupe &&
                OptionPourTourelleAcheteeSelectionne == 1 &&
                Core.Input.Facade.estPeseeUneSeuleFois(Preferences.toucheSelection, Simulation.Main.JoueursConnectes[0].Manette, Simulation.Scene.Nom))
                notifyMiseAJourTourelleDemande(CorpsSelectionne, EmplacementSelectionne);

            if (!ModeDemo && (Core.Input.Facade.estPeseeUneSeuleFois(Preferences.toucheProchaineVague, Simulation.Main.JoueursConnectes[0].Manette, Simulation.Scene.Nom) ||
               (Core.Input.Facade.estPeseeUneSeuleFois(Preferences.toucheSelection, Simulation.Main.JoueursConnectes[0].Manette, Simulation.Scene.Nom) &&
                Core.Physique.Facade.collisionCercleRectangle(Cursor.Cercle, SandGlass.Rectangle))))
                notifyProchaineVagueDemandee();

            determinerProjectionDestruction();

            setWidgets();

            if (!ModeDemo)
            {
                PanneauCorpsCeleste.Update(gameTime);
            }
        }


        private void setWidgets()
        {
            PanneauCorpsCeleste.TourellePourAchatSelectionne = NavigationTourellesAchat.Selectionne;
            PanneauCorpsCeleste.CorpsSelectionne = CorpsSelectionne;
            PanneauCorpsCeleste.EmplacementSelectionne = EmplacementSelectionne;
            PanneauCorpsCeleste.OptionPourTourelleAcheteeSelectionne = OptionPourTourelleAcheteeSelectionne;
            PanneauCorpsCeleste.OptionPourCorpsCelesteSelectionne = OptionPourCorpsCelesteSelectionne;
            PanneauCorpsCeleste.Refresh();
        }


        public void Draw()
        {
            Chemin.Draw(null);

            if (!ModeDemo)
            {
                PanneauCorpsCeleste.Draw(null);

                if (ProjectionEnCours != null || NavigationTourellesAchat.ProjectionEnCours != null)
                {
                    CheminProjection.Draw(null);

                    int compteurEnnemisFinChemin = 0;

                    for (int i = 0; i < Ennemis.Count; i++)
                    {
                        Ennemi ennemi = Ennemis[i];

                        ennemi.DrawProjection(null);

                        if (ennemi.FinCheminProjection)
                            compteurEnnemisFinChemin++;
                    }
                }

                if (OptionPourCorpsCelesteSelectionne == PowerUp.FinalSolution && CorpsSelectionne != null)
                {
                    ZoneDestruction.Position = CorpsSelectionne.Position;
                    ZoneDestruction.Taille = (CorpsSelectionne.ZoneImpactDestruction.Rayon / 100) * 2;
                    Simulation.Scene.ajouterScenable(ZoneDestruction);
                }
            }
        }


        public void doMouseMoved(PlayerIndex inputIndex, Vector3 delta)
        {
            Joueur.Position += delta;
            Joueur.VerifyFrame(Cursor.Width, Cursor.Height);
            Cursor.Position = Joueur.Position;
        }


        public void doGamePadJoystickMoved(PlayerIndex inputIndex, Buttons button, Vector3 delta)
        {
            Joueur.Position += delta * Cursor.Vitesse;
            Joueur.VerifyFrame(Cursor.Width, Cursor.Height);
            Cursor.Position = Joueur.Position;
        }


        public void doTourelleAchetee(Tourelle tourelle)
        {
            this.Joueur.ReserveUnites -= tourelle.PrixAchat;
            notifyCashChanged(Joueur.ReserveUnites);

            NavigationTourellesAchat.doTourelleAchetee(tourelle);
            NavigationTourellesAchat.Update(null);
            determinerOptionsDisponiblesPourTourellesAchetees();

            if (tourelle.Type == TypeTourelle.Gravitationnelle && !Simulation.ModeDemo)
                Core.Audio.Facade.jouerEffetSonore("Partie", "sfxTourelleGravitationnelleAchetee");

            OptionPourTourelleAcheteeSelectionne = 1;
        }


        public void doTourelleVendue(Tourelle tourelle)
        {
            ProjectionEnCours = null;
            this.Joueur.ReserveUnites += tourelle.PrixVente;
            notifyCashChanged(Joueur.ReserveUnites);

            if (tourelle.Type == TypeTourelle.Gravitationnelle)
                Core.Audio.Facade.jouerEffetSonore("Partie", "sfxTourelleGravitationnelleAchetee");
            else
                Core.Audio.Facade.jouerEffetSonore("Partie", "sfxTourelleVendue");
        }


        public void doTourelleMiseAJour(Tourelle tourelle)
        {
            this.Joueur.ReserveUnites -= tourelle.PrixAchat; //parce qu'effectue une fois la tourelle mise a jour
            notifyCashChanged(Joueur.ReserveUnites);
            this.OptionPourTourelleAcheteeSelectionne = 1; //parce que c'etait l'option selectionnee avant la mise a jour
            PanneauCorpsCeleste.OptionPourTourelleAcheteeSelectionne = OptionPourTourelleAcheteeSelectionne;
        }


        private void determinerOptionsDisponiblesPourTourellesAchetees()
        {
            bool majEtaitIndisponible = (OptionsPourTourellesAchetees.Count >= 2 && !OptionsPourTourellesAchetees[1]);

            OptionsPourTourellesAchetees.Clear();

            OptionsPourTourellesAchetees.Add(0, EmplacementSelectionne.Tourelle.PeutVendre);
            OptionsPourTourellesAchetees.Add(1, EmplacementSelectionne.Tourelle.PeutMettreAJour && EmplacementSelectionne.Tourelle.PrixMiseAJour <= Joueur.ReserveUnites);

            //par defaut, l'option selectionnee est la mise a jour (quand on choisi un emplacement ou une mise a jour est terminee)
            if (EmplacementSelectionne.Tourelle.RetourDeInactiveCeTick)
                OptionPourTourelleAcheteeSelectionne = 1;

            //des que l'option de maj redevient disponible, elle est selectionnee
            if (majEtaitIndisponible && OptionsPourTourellesAchetees[1])
                OptionPourTourelleAcheteeSelectionne = 1;

            //change automatiquement la selection de cette option quand elle n'est pas disponible
            if (!OptionsPourTourellesAchetees[1] && OptionPourTourelleAcheteeSelectionne == 1)
                OptionPourTourelleAcheteeSelectionne = 0;
        }


        private void determinerOptionsDisponiblesPourCorpsCeleste()
        {
            OptionsPourCorpsCeleste.Clear();

            OptionsPourCorpsCeleste.Add(PowerUp.DoItYourself, OptionsCVDisponibles[PowerUp.DoItYourself] && CorpsSelectionne.PeutAvoirDoItYourself && CorpsSelectionne.PrixDoItYourself <= Joueur.ReserveUnites);
            OptionsPourCorpsCeleste.Add(PowerUp.FinalSolution, CorpsSelectionne.PeutDetruire && CorpsSelectionne.PrixDestruction <= Joueur.ReserveUnites);
            OptionsPourCorpsCeleste.Add(PowerUp.CollectTheRent, OptionsCVDisponibles[PowerUp.CollectTheRent] && CorpsSelectionne.PeutAvoirCollecteur && CorpsSelectionne.PrixCollecteur <= Joueur.ReserveUnites);
            OptionsPourCorpsCeleste.Add(PowerUp.TheResistance, OptionsCVDisponibles[PowerUp.TheResistance] && CorpsSelectionne.PeutAvoirTheResistance && CorpsSelectionne.PrixTheResistance <= Joueur.ReserveUnites);

            if (OptionPourCorpsCelesteSelectionne == PowerUp.Aucune ||
                OptionPourCorpsCelesteSelectionne != PowerUp.Aucune && !OptionsPourCorpsCeleste[OptionPourCorpsCelesteSelectionne])
            {
                for (int i = 0; i < OptionsPourCorpsCeleste.Count; i++)
                {
                    OptionPourCorpsCelesteSelectionne = (OptionsPourCorpsCeleste[(PowerUp)i]) ? (PowerUp)i : PowerUp.Aucune;

                    if (OptionPourCorpsCelesteSelectionne != PowerUp.Aucune)
                        break;
                }
            }
        }

        private PowerUp ProchainPowerUpDisponiblePourCorpsCeleste
        {
            get
            {
                PowerUp pActuel = OptionPourCorpsCelesteSelectionne;

                int nbChoix = OptionsPourCorpsCeleste.Count;
                int numVerifier = (int)pActuel;
                int prochain = -1;

                for (int i = 1; i < nbChoix; i++)
                {
                    numVerifier += 1;

                    if (numVerifier >= nbChoix)
                        numVerifier = 0;

                    if (OptionsPourCorpsCeleste[(PowerUp)numVerifier])
                    {
                        prochain = numVerifier;
                        break;
                    }
                }

                return (PowerUp)prochain;
            }
        }


        private PowerUp PrecedentPowerUpDisponiblePourCorpsCeleste
        {
            get
            {
                PowerUp pActuel = OptionPourCorpsCelesteSelectionne;

                int nbChoix = OptionsPourCorpsCeleste.Count;
                int numVerifier = (int)pActuel;
                int precedent = -1;

                for (int i = 1; i < nbChoix; i++)
                {
                    numVerifier -= 1;

                    if (numVerifier < 0)
                        numVerifier = nbChoix - 1;

                    if (OptionsPourCorpsCeleste[(PowerUp)numVerifier])
                    {
                        precedent = numVerifier;
                        break;
                    }
                }

                return (PowerUp)precedent;
            }
        }


        private void determinerProjectionDestruction()
        {
            if (ProjectionEnCours == null && OptionPourCorpsCelesteSelectionne == PowerUp.FinalSolution ||
                EmplacementSelectionne != null && EmplacementSelectionne.EstOccupe && EmplacementSelectionne.Tourelle.Type == TypeTourelle.Gravitationnelle &&
                OptionPourTourelleAcheteeSelectionne == 0 && OptionsPourTourellesAchetees[OptionPourTourelleAcheteeSelectionne])
            {
                CheminProjection.enleverCorpsCeleste(CorpsSelectionne);
                ProjectionEnCours = CorpsSelectionne;
            }

            else if (ProjectionEnCours != null && OptionPourCorpsCelesteSelectionne != PowerUp.FinalSolution &&
                     NavigationTourellesAchat.ProjectionEnCours == null)
            {
                CheminProjection.ajouterCorpsCeleste(ProjectionEnCours);
                ProjectionEnCours = null;
            }
        }
    }
}
