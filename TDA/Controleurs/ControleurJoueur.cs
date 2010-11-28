namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Core.Visuel;
    using Core.Utilities;
    using Core.Physique;

    class ControleurJoueur : DrawableGameComponent
    {
        // Donnees externes
        public CorpsCeleste CorpsCelesteAProteger;
        public Chemin Chemin;
        public Chemin CheminProjection;
        public List<CorpsCeleste> CorpsCelestes;
        public VaguesInfinies VaguesInfinies;
        public LinkedList<Vague> Vagues;
        public List<Ennemi> Ennemis;
        public Scenario Histoire;
        public JoueurCommun Joueur;
        public Dictionary<PowerUp, bool> OptionsCVDisponibles;
        public Vector3 PositionCurseur;
        public Dictionary<TypeEnnemi, DescripteurEnnemi> CompositionProchaineVague;

        private bool modeDemo;
        public bool ModeDemo
        {
            get { return modeDemo; }
            set
            {
                modeDemo = value;

                SelectionCorpsCeleste.ModeDemo = value;
            }
        }


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

        //TMP: Destruction
        private CorpsCeleste ProjectionEnCours;
        private IVisible ZoneDestruction;

        // GUI
        private PointsDeVieJoueur PointsDeVie;
        private PointsDeVieJoueur PointsDeVieProjection;
        private AnnonciationHistoire AnnonciationHistoire;
        private AnnonciationVictoireDefaite AnnonciationVictoireDefaite;
        private VueAvancee VueAvancee;
        private ControleurNavigationCorpsCelestes NavigationCorpsCelestesV2;
        private ControleurNavigationTourellesAchat NavigationTourellesAchat;
        private PanneauGeneral PanneauGeneral;
        private SelectionCorpsCeleste SelectionCorpsCeleste;
        private PanneauCorpsCeleste PanneauCorpsCeleste;
        public Curseur Curseur;
        public Bulle BulleGUI { get { return PanneauCorpsCeleste.Bulle; } }
        public Sablier Sablier { get { return PanneauGeneral.Sablier; } }

        // Evenements

        public delegate void AchatDoItYourselfDemandeHandler(CorpsCeleste corpsCeleste);
        public event AchatDoItYourselfDemandeHandler AchatDoItYourselfDemande;

        public delegate void DestructionCorpsCelesteDemandeHandler(CorpsCeleste corpsCeleste);
        public event DestructionCorpsCelesteDemandeHandler DestructionCorpsCelesteDemande;

        public delegate void AchatTourelleDemandeHandler(TypeTourelle typeTourelle, CorpsCeleste corpsCeleste, Emplacement emplacement);
        public event AchatTourelleDemandeHandler AchatTourelleDemande;

        public delegate void VenteTourelleDemandeHandler(CorpsCeleste corpsCeleste, Emplacement emplacement);
        public event VenteTourelleDemandeHandler VenteTourelleDemande;

        public delegate void MiseAJourTourelleDemandeHandler(CorpsCeleste corpsCeleste, Emplacement emplacement);
        public event MiseAJourTourelleDemandeHandler MiseAJourTourelleDemande;

        public delegate void AchatCollecteurDemandeHandler(CorpsCeleste corpsCeleste);
        public event AchatCollecteurDemandeHandler AchatCollecteurDemande;

        public delegate void AchatTheResistanceDemandeHandler(CorpsCeleste corpsCeleste);
        public event AchatTheResistanceDemandeHandler AchatTheResistanceDemande;

        public delegate void ProchaineVagueDemandeeHandler();
        public event ProchaineVagueDemandeeHandler ProchaineVagueDemandee;
        

        // Autre
        private Simulation Simulation;


        public ControleurJoueur(Simulation simulation)
            : base(simulation.Main)
        {
            this.Simulation = simulation;

            CorpsCelestes = new List<CorpsCeleste>();

            PanneauCorpsCeleste = new PanneauCorpsCeleste(Simulation);
            Curseur = new Curseur(Simulation.Main, Simulation.Scene, Vector3.Zero, 10, Preferences.PrioriteGUIPanneauGeneral);
            PanneauGeneral = new PanneauGeneral(Simulation, new Vector3(400, -260, 0));
        }


        public override void Initialize()
        {
            PanneauGeneral.CompositionProchaineVague = CompositionProchaineVague;

            OptionsPourTourellesAchetees = new Dictionary<int, bool>();
            OptionsPourCorpsCeleste = new Dictionary<PowerUp, bool>();

            PointsDeVie = new PointsDeVieJoueur(Simulation, CorpsCelesteAProteger, new Vector3(-120, 0, 0), new Color(255, 0, 220));
            PointsDeVieProjection = new PointsDeVieJoueur(Simulation, CorpsCelesteAProteger, new Vector3(-120, 75, 0), Color.White);
            VueAvancee = new VueAvancee(Simulation, Ennemis, CorpsCelestes);
            AnnonciationHistoire = new AnnonciationHistoire(Simulation, Histoire);
            AnnonciationVictoireDefaite = new AnnonciationVictoireDefaite(Simulation, CorpsCelestes);
            NavigationTourellesAchat = new ControleurNavigationTourellesAchat(Joueur, CheminProjection);
            SelectionCorpsCeleste = new SelectionCorpsCeleste(Simulation);
            //Curseur = new Curseur(Simulation.Main, Simulation.Scene, PositionCurseur, 10, Preferences.PrioriteGUIPanneauGeneral);
            Curseur.Position = PositionCurseur;
            NavigationCorpsCelestesV2 = new ControleurNavigationCorpsCelestes(Simulation, CorpsCelestes, Curseur);
            PanneauCorpsCeleste.Initialize();
            PanneauCorpsCeleste.TourellesAchat = NavigationTourellesAchat.Tourelles;
            PanneauCorpsCeleste.OptionsPourTourelleAchetee = OptionsPourTourellesAchetees;
            PanneauCorpsCeleste.OptionsPourCorpsCelesteSelectionne = OptionsPourCorpsCeleste;
            PanneauGeneral.Curseur = Curseur;

            CorpsSelectionne = NavigationCorpsCelestesV2.CorpsCelesteSelectionne;
            EmplacementSelectionne = null; //-1;

            VaguesRestantes = (VaguesInfinies == null) ? Vagues.Count : -1;
            TempsProchaineVague = Vagues.First.Value.TempsApparition;

            ZoneDestruction = new IVisible(Core.Persistance.Facade.recuperer<Texture2D>("CercleBlanc"), Vector3.Zero, Simulation.Scene);
            ZoneDestruction.Couleur = new Color(Color.Red, 100);
            ZoneDestruction.Origine = ZoneDestruction.Centre;
            ZoneDestruction.PrioriteAffichage = Preferences.PrioriteGUIEtoiles - 0.002f;

            

            setWidgets();
        }


        protected virtual void notifyDestructionCorpsCelesteDemande(CorpsCeleste corpsCeleste)
        {
            if (DestructionCorpsCelesteDemande != null)
                DestructionCorpsCelesteDemande(corpsCeleste);
        }


        protected virtual void notifyDoItYourselfDemande(CorpsCeleste corpsCeleste)
        {
            if (AchatDoItYourselfDemande != null)
                AchatDoItYourselfDemande(corpsCeleste);
        }


        protected virtual void notifyAchatTourelleDemande(TypeTourelle typeTourelle, CorpsCeleste corpsCeleste, Emplacement emplacement)
        {
            if (AchatTourelleDemande != null)
                AchatTourelleDemande(typeTourelle, corpsCeleste, emplacement);
        }


        protected virtual void notifyVenteTourelleDemande(CorpsCeleste corpsCeleste, Emplacement emplacement)
        {
            if (VenteTourelleDemande != null)
                VenteTourelleDemande(corpsCeleste, emplacement);
        }


        protected virtual void notifyAchatCollecteurDemande(CorpsCeleste corpsCeleste)
        {
            if (AchatCollecteurDemande != null)
                AchatCollecteurDemande(corpsCeleste);
        }


        protected virtual void notifyAchatTheResistanceDemande(CorpsCeleste corpsCeleste)
        {
            if (AchatTheResistanceDemande != null)
                AchatTheResistanceDemande(corpsCeleste);
        }


        protected virtual void notifyMiseAJourTourelleDemande(CorpsCeleste corpsCeleste, Emplacement emplacement)
        {
            if (MiseAJourTourelleDemande != null)
                MiseAJourTourelleDemande(corpsCeleste, emplacement);
        }


        protected virtual void notifyProchaineVagueDemandee()
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

                return;
            }


            CorpsCeleste corpsCeleste = objet as CorpsCeleste;

            if (corpsCeleste != null)
            {
                if (CorpsSelectionne == corpsCeleste)
                {
                    NavigationCorpsCelestesV2.doCorpsCelesteDetruit();
                    CorpsSelectionne = NavigationCorpsCelestesV2.CorpsCelesteSelectionne;
                    EmplacementSelectionne = null; //-1;
                    NavigationTourellesAchat.CorpsCelesteSelectionne = CorpsSelectionne;
                    NavigationTourellesAchat.EmplacementSelectionne = EmplacementSelectionne;
                    NavigationTourellesAchat.Update(null);
                    ProjectionEnCours = null;
                    setWidgets();
                }

                return;
            }


            VaisseauDoItYourself vaisseau = objet as VaisseauDoItYourself;

            if (vaisseau != null)
            {
                Curseur.Position = vaisseau.Position;
                Curseur.doShow();

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
                    Joueur.ReserveUnites += mineral.Valeur;

                return;
            }
        }


        public override void Update(GameTime gameTime)
        {
            if (VaguesRestantes > 0)
                TempsProchaineVague = Math.Max(0, TempsProchaineVague - gameTime.ElapsedGameTime.TotalMilliseconds);

            NavigationCorpsCelestesV2.Update(gameTime);
            CorpsSelectionne = NavigationCorpsCelestesV2.CorpsCelesteSelectionne;

            EmplacementSelectionne = NavigationCorpsCelestesV2.EmplacementSelectionne; //(NavigationCorpsCelestesV2.CorpsCelesteSelectionneChange) ? -1 : EmplacementSelectionne;
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
                Curseur.doHide();
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
                Curseur.doHide();

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
                Core.Physique.Facade.collisionCercleRectangle(Curseur.Cercle, PanneauGeneral.Sablier.Rectangle))))
                notifyProchaineVagueDemandee();

            determinerProjectionDestruction();

            setWidgets();

            SelectionCorpsCeleste.Update(gameTime);
            Curseur.Update(gameTime);

            if (!ModeDemo)
            {
                PanneauGeneral.Update(gameTime);
                PointsDeVie.Update(gameTime);
                PointsDeVieProjection.Update(gameTime);
                PanneauCorpsCeleste.Update(gameTime);
                AnnonciationHistoire.Update(gameTime);
                AnnonciationVictoireDefaite.Update(gameTime);
                VueAvancee.Update(gameTime);
                VueAvancee.Visible = Core.Input.Facade.estPesee(Preferences.toucheVueAvancee, Simulation.Main.JoueursConnectes[0].Manette, Simulation.Scene.Nom);
            }
        }


        private void setWidgets()
        {
            PanneauGeneral.VaguesRestantes = VaguesRestantes;
            PanneauGeneral.TempsProchaineVague = TempsProchaineVague;
            PanneauGeneral.Pointage = Joueur.Pointage;
            PanneauGeneral.ReserveUnites = Joueur.ReserveUnites;

            if (SelectionCorpsCeleste.CorpsSelectionne != CorpsSelectionne)
                SelectionCorpsCeleste.CorpsSelectionne = CorpsSelectionne;

            PanneauCorpsCeleste.TourellePourAchatSelectionne = NavigationTourellesAchat.Selectionne;
            PanneauCorpsCeleste.CorpsSelectionne = CorpsSelectionne;
            PanneauCorpsCeleste.EmplacementSelectionne = EmplacementSelectionne;
            PanneauCorpsCeleste.OptionPourTourelleAcheteeSelectionne = OptionPourTourelleAcheteeSelectionne;
            PanneauCorpsCeleste.OptionPourCorpsCelesteSelectionne = OptionPourCorpsCelesteSelectionne;
            PanneauCorpsCeleste.Refresh();


            VueAvancee.EmplacementSelectionne = EmplacementSelectionne;
        }


        public override void Draw(GameTime gameTime)
        {
            SelectionCorpsCeleste.Draw(null);
            Curseur.Draw();
            Chemin.Draw(null);

            if (!ModeDemo)
            {
                PanneauGeneral.Draw(gameTime);
                PointsDeVie.Draw(gameTime);
                PanneauCorpsCeleste.Draw(null);
                VueAvancee.Draw(null);
                AnnonciationHistoire.Draw(null);
                AnnonciationVictoireDefaite.Draw(null);

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

                    //PointsDeVieProjection.PointsDeVieOffset = -compteurEnnemisFinChemin;
                    //PointsDeVieProjection.Draw(null);
                }

                if (OptionPourCorpsCelesteSelectionne == PowerUp.FinalSolution && CorpsSelectionne != null)
                {
                    ZoneDestruction.Position = CorpsSelectionne.Position;
                    ZoneDestruction.Taille = (CorpsSelectionne.ZoneImpactDestruction.Rayon / 100) * 2;
                    Simulation.Scene.ajouterScenable(ZoneDestruction);
                }
            }
        }


        public void doVagueDebutee()
        {
            PanneauGeneral.Sablier.tourner();
            TempsProchaineVague = double.MaxValue;

            Core.Audio.Facade.jouerEffetSonore("Partie", "sfxNouvelleVague");

            if (VaguesInfinies != null || --VaguesRestantes == 0)
                return;

            //Degeux
            LinkedListNode<Vague> vagueSuivante = Vagues.First;

            for (int i = 0; i < Vagues.Count - VaguesRestantes; i++)
                vagueSuivante = vagueSuivante.Next;

            TempsProchaineVague = vagueSuivante.Value.TempsApparition;
        }


        public void doTourelleAchetee(Tourelle tourelle)
        {
            this.Joueur.ReserveUnites -= tourelle.PrixAchat;

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

            if (tourelle.Type == TypeTourelle.Gravitationnelle)
                Core.Audio.Facade.jouerEffetSonore("Partie", "sfxTourelleGravitationnelleAchetee");
            else
                Core.Audio.Facade.jouerEffetSonore("Partie", "sfxTourelleVendue");
        }


        public void doNouvelEtatPartie(EtatPartie nouvelEtat)
        {
            AnnonciationVictoireDefaite.doNouvelEtat(nouvelEtat);
        }


        public void doTourelleMiseAJour(Tourelle tourelle)
        {
            this.Joueur.ReserveUnites -= tourelle.PrixAchat; //parce qu'effectue une fois la tourelle mise a jour
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
            if (NavigationCorpsCelestesV2.EmplacementSelectionneChange || EmplacementSelectionne.Tourelle.RetourDeInactiveCeTick)
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
