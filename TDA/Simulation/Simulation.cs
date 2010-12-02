namespace TDA
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Core.Physique;
    using Core.Visuel;
    using Core.Input;

    delegate void PhysicalObjectHandler(IObjetPhysique obj);

    class Simulation : DrawableGameComponent
    {
        public Scene Scene;
        public Main Main;
        public DescripteurScenario DescriptionScenario;
        public bool Debug;

        private ControleurScenario ControleurScenario;
        private ControleurEnnemis ControleurEnnemis;
        private ControleurProjectiles ControleurProjectiles;
        private ControleurCollisions ControleurCollisions;
        private PlayerController PlayerController;
        private ControleurTourelles ControleurTourelles;
        public ControleurSystemePlanetaire ControleurSystemePlanetaire;
        private ControleurVaisseaux ControleurVaisseaux;
        public ControleurMessages ControleurMessages;
        private GUIController GUIController;
        
        public RectanglePhysique Terrain = new RectanglePhysique(-840, -560, 1680, 1120);

        public bool EnPause;
        public EtatPartie Etat                      { get { return ControleurScenario.Etat; } }

        private bool modeDemo = false;
        public bool ModeDemo
        {
            get { return modeDemo; }
            set
            {
                modeDemo = value;

                PlayerController.ModeDemo = value;
                ControleurScenario.ModeDemo = value;
            }
        }


        private bool modeEditeur = false;
        public bool ModeEditeur
        {
            get { return modeEditeur; }
            set
            {
                modeEditeur = value;

                ControleurScenario.ModeEditeur = value;
            }
        }

        public bool InitParticules = true;


        public CorpsCeleste CorpsCelesteSelectionne
        {
            get
            {
                return PlayerController.CorpsSelectionne;
            }
        }

        public Vector3 PositionCurseur; 


        public Simulation(Main main, Scene scene, DescripteurScenario scenario)
            : base(main)
        {
            Scene = scene;
            Main = main;
            DescriptionScenario = scenario;

            Core.Input.Facade.considerToutesTouches(Main.JoueursConnectes[0].Manette, Scene.Nom);

#if XBOX || MANETTE_WINDOWS
            Core.Input.Facade.considerThumbsticks(Main.JoueursConnectes[0].Manette, null, Scene.Nom);
#endif

#if DEBUG
            this.Debug = true;
#else
            this.Debug = false;
#endif
        }


        public override void Initialize()
        {
            if (InitParticules)
            {
                Scene.Particules.vider();
                Scene.Particules.ajouter("projectileMissileDeplacement");
                Scene.Particules.ajouter("projectileBaseExplosion");
                Scene.Particules.ajouter("etoile");
                Scene.Particules.ajouter("etoileBleue");
                Scene.Particules.ajouter("planeteGazeuse");
                Scene.Particules.ajouter("etoilesScintillantes");
                Scene.Particules.ajouter("projectileMissileExplosion");
                Scene.Particules.ajouter("projectileLaserSimple");
                Scene.Particules.ajouter("projectileLaserMultiple");
                Scene.Particules.ajouter("selectionCorpsCeleste");
                Scene.Particules.ajouter("traineeMissile");
                Scene.Particules.ajouter("etincelleLaser");
                Scene.Particules.ajouter("toucherTerre");
                Scene.Particules.ajouter("anneauTerreMeurt");
                Scene.Particules.ajouter("bouleTerreMeurt");
                Scene.Particules.ajouter("missileAlien");
                Scene.Particules.ajouter("implosionAlien");
                Scene.Particules.ajouter("explosionEnnemi");
                Scene.Particules.ajouter("mineral1");
                Scene.Particules.ajouter("mineral2");
                Scene.Particules.ajouter("mineral3");
                Scene.Particules.ajouter("mineralPointsVie");
                Scene.Particules.ajouter("mineralPris");
                Scene.Particules.ajouter("etincelleMissile");
                Scene.Particules.ajouter("etincelleLaserSimple");
                Scene.Particules.ajouter("etincelleSlowMotion");
                Scene.Particules.ajouter("etincelleSlowMotionTouche");
                Scene.Particules.ajouter("etoileFilante");
                Scene.Particules.ajouter("trouRose");
            }

            ControleurCollisions = new ControleurCollisions(this);
            ControleurProjectiles = new ControleurProjectiles(this);
            ControleurEnnemis = new ControleurEnnemis(this);
            PlayerController = new PlayerController(this);
            ControleurTourelles = new ControleurTourelles(this);
            ControleurSystemePlanetaire = new ControleurSystemePlanetaire(this);
            ControleurScenario = new ControleurScenario(this, new Scenario(this, DescriptionScenario));
            ControleurVaisseaux = new ControleurVaisseaux(this);
            ControleurMessages = new ControleurMessages(this);
            GUIController = new GUIController(this);

            ControleurCollisions.Projectiles = ControleurProjectiles.Projectiles;
            ControleurCollisions.Ennemis = ControleurEnnemis.Ennemis;
            PlayerController.CorpsCelestes = ControleurScenario.CorpsCelestes;
            ControleurSystemePlanetaire.CorpsCelestes = ControleurScenario.CorpsCelestes;
            ControleurTourelles.ControleurSystemePlanetaire = ControleurSystemePlanetaire;
            ControleurEnnemis.VaguesInfinies = ControleurScenario.VaguesInfinies;
            ControleurEnnemis.Vagues = ControleurScenario.Vagues;
            ControleurCollisions.Tourelles = ControleurTourelles.Tourelles;
            PlayerController.Joueur = ControleurScenario.Joueur;
            PlayerController.CorpsCelesteAProteger = ControleurScenario.CorpsCelesteAProteger;
            PlayerController.Chemin = ControleurSystemePlanetaire.Chemin;
            PlayerController.CheminProjection = ControleurSystemePlanetaire.CheminProjection;
            ControleurEnnemis.CheminProjection = ControleurSystemePlanetaire.CheminProjection;
            ControleurTourelles.TourellesDepart = ControleurScenario.TourellesDepart;
            PlayerController.Ennemis = ControleurEnnemis.Ennemis;
            ControleurEnnemis.Chemin = ControleurSystemePlanetaire.Chemin;
            ControleurCollisions.CorpsCelestes = ControleurScenario.CorpsCelestes;
            PlayerController.OptionsCVDisponibles = ControleurVaisseaux.OptionsDisponibles;
            ControleurCollisions.Mineraux = ControleurEnnemis.Mineraux;
            ControleurEnnemis.ValeurTotalMineraux = ControleurScenario.Scenario.ValeurMinerauxDonnes;
            ControleurEnnemis.PourcentageMinerauxDonnes = ControleurScenario.Scenario.PourcentageMinerauxDonnes;
            ControleurEnnemis.NbPackViesDonnes = ControleurScenario.Scenario.NbPackViesDonnes;
            ControleurVaisseaux.Ennemis = ControleurEnnemis.Ennemis;
            ControleurMessages.Tourelles = ControleurTourelles.Tourelles;
            ControleurMessages.BulleGUI = PlayerController.BulleGUI;
            ControleurMessages.CorpsCelesteAProteger = ControleurScenario.CorpsCelesteAProteger;
            ControleurMessages.CorpsCelestes = ControleurScenario.CorpsCelestes;
            ControleurMessages.Chemin = ControleurSystemePlanetaire.Chemin;
            ControleurMessages.Sablier = GUIController.SandGlass;
            GUIController.CompositionNextWave = ControleurEnnemis.CompositionProchaineVague;
            PlayerController.SandGlass = GUIController.SandGlass;
            GUIController.CelestialBodies = ControleurSystemePlanetaire.CorpsCelestes;
            GUIController.Scenario = ControleurScenario.Scenario;
            GUIController.Enemies = ControleurEnnemis.Ennemis;
            ControleurMessages.Curseur = GUIController.Cursor;
            PlayerController.Joueur.Position = PositionCurseur;
            PlayerController.Cursor = GUIController.Cursor;
            GUIController.InfiniteWaves = ControleurScenario.VaguesInfinies;
            GUIController.Waves = ControleurScenario.Vagues;

            ControleurCollisions.ObjetTouche += new ControleurCollisions.ObjetToucheHandler(ControleurEnnemis.doObjetTouche);
            PlayerController.AchatTourelleDemande += new PlayerController.TurretTypeCelestialObjectTurretSpotHandler(ControleurTourelles.doAcheterTourelle);
            ControleurEnnemis.VagueTerminee += new ControleurEnnemis.VagueTermineeHandler(ControleurScenario.doVagueTerminee);
            ControleurEnnemis.ObjetDetruit += new PhysicalObjectHandler(PlayerController.doObjetDetruit);
            ControleurCollisions.DansZoneActivation += new ControleurCollisions.DansZoneActivationHandler(ControleurTourelles.doDansZoneActivationTourelle);
            ControleurTourelles.ObjetCree += new PhysicalObjectHandler(ControleurProjectiles.doObjetCree);
            ControleurEnnemis.ObjetCree += new PhysicalObjectHandler(ControleurSystemePlanetaire.doObjetCree);
            ControleurProjectiles.ObjetDetruit += new PhysicalObjectHandler(ControleurCollisions.doObjetDetruit);
            ControleurEnnemis.VagueDebutee += new ControleurEnnemis.VagueDebuteeHandler(GUIController.doWaveStarted);
            PlayerController.VenteTourelleDemande += new PlayerController.CelestialObjectTurretSpotHandler(ControleurTourelles.doVendreTourelle);
            ControleurTourelles.TourelleVendue += new ControleurTourelles.TourelleVendueHandler(PlayerController.doTourelleVendue);
            ControleurTourelles.TourelleAchetee += new ControleurTourelles.TourelleAcheteeHandler(PlayerController.doTourelleAchetee);
            ControleurEnnemis.EnnemiAtteintFinTrajet += new ControleurEnnemis.EnnemiAtteintFinTrajetHandler(ControleurScenario.doEnnemiAtteintFinTrajet);
            PlayerController.AchatCollecteurDemande += new PlayerController.CelestialObjectHandler(ControleurVaisseaux.doAcheterCollecteur);
            ControleurTourelles.TourelleMiseAJour += new ControleurTourelles.TourelleMiseAJourHandler(PlayerController.doTourelleMiseAJour);
            PlayerController.MiseAJourTourelleDemande += new PlayerController.CelestialObjectTurretSpotHandler(ControleurTourelles.doMettreAJourTourelle);
            ControleurSystemePlanetaire.ObjetDetruit += new PhysicalObjectHandler(ControleurTourelles.doObjetDetruit);
            ControleurSystemePlanetaire.ObjetDetruit += new PhysicalObjectHandler(PlayerController.doObjetDetruit);
            PlayerController.ProchaineVagueDemandee += new PlayerController.NoneHandler(ControleurEnnemis.doProchaineVagueDemandee);
            ControleurVaisseaux.ObjetCree += new PhysicalObjectHandler(ControleurProjectiles.doObjetCree);
            PlayerController.AchatDoItYourselfDemande += new PlayerController.CelestialObjectHandler(ControleurVaisseaux.doAcheterDoItYourself);
            ControleurVaisseaux.ObjetDetruit += new PhysicalObjectHandler(PlayerController.doObjetDetruit);
            PlayerController.DestructionCorpsCelesteDemande += new PlayerController.CelestialObjectHandler(ControleurSystemePlanetaire.doDetruireCorpsCeleste);
            ControleurSystemePlanetaire.ObjetDetruit += new PhysicalObjectHandler(ControleurCollisions.doObjetDetruit);
            ControleurVaisseaux.ObjetCree += new PhysicalObjectHandler(ControleurCollisions.doObjetCree);
            ControleurEnnemis.ObjetCree += new PhysicalObjectHandler(ControleurCollisions.doObjetCree);
            PlayerController.AchatTheResistanceDemande += new PlayerController.CelestialObjectHandler(ControleurVaisseaux.doAcheterTheResistance);
            ControleurSystemePlanetaire.ObjetDetruit += new PhysicalObjectHandler(ControleurScenario.doObjetDetruit);

            ControleurEnnemis.EnnemiAtteintFinTrajet += new ControleurEnnemis.EnnemiAtteintFinTrajetHandler(this.doEnnemiAtteintFinTrajet);
            ControleurScenario.NouvelEtatPartie += new ControleurScenario.NouvelEtatPartieHandler(this.doGameStateChanged);
            ControleurSystemePlanetaire.ObjetDetruit += new PhysicalObjectHandler(this.doCorpsCelesteDetruit);
            PlayerController.SelectedCelestialBodyChanged += new PlayerController.CelestialObjectHandler(GUIController.doSelectedCelestialBodyChanged);
            ControleurEnnemis.VagueDebutee += new ControleurEnnemis.VagueDebuteeHandler(GUIController.doNextWave);
            PlayerController.CashChanged += new PlayerController.IntegerHandler(GUIController.doCashChanged);
            PlayerController.ScoreChanged += new PlayerController.IntegerHandler(GUIController.doScoreChanged);
            ControleurScenario.NouvelEtatPartie += new ControleurScenario.NouvelEtatPartieHandler(GUIController.doGameStateChanged);
            PlayerController.AchatCollecteurDemande += new PlayerController.CelestialObjectHandler(GUIController.doSpaceshipBuyed);
            PlayerController.AchatDoItYourselfDemande += new PlayerController.CelestialObjectHandler(GUIController.doSpaceshipBuyed);
            ControleurVaisseaux.ObjetDetruit += new PhysicalObjectHandler(GUIController.doObjectDestroyed);

            ControleurScenario.Initialize();
            PlayerController.Initialize();
            ControleurEnnemis.Initialize();
            ControleurProjectiles.Initialize();
            ControleurTourelles.Initialize();
            ControleurSystemePlanetaire.Initialize();
            ControleurCollisions.Initialize();
            ControleurVaisseaux.Initialize();
            ControleurMessages.Initialize();
            GUIController.Initialize();

            ModeDemo = modeDemo;
            ModeEditeur = modeEditeur;
        }


        public override void Update(GameTime gameTime)
        {
            if (EnPause)
                return;

            ControleurCollisions.Update(gameTime);
            ControleurProjectiles.Update(gameTime);
            ControleurTourelles.Update(gameTime); //doit etre fait avant le controleurEnnemi pour les associations ennemis <--> tourelles
            ControleurEnnemis.Update(gameTime);
            PlayerController.Update(gameTime);
            ControleurSystemePlanetaire.Update(gameTime);
            ControleurScenario.Update(gameTime);
            ControleurVaisseaux.Update(gameTime);
            ControleurMessages.Update(gameTime);
            GUIController.Update(gameTime);

            HandleInput();
        }

        public override void Draw(GameTime gameTime)
        {
            ControleurCollisions.Draw(null);
            ControleurProjectiles.Draw(null);
            ControleurEnnemis.Draw(null);
            PlayerController.Draw();
            ControleurTourelles.Draw(null);
            ControleurSystemePlanetaire.Draw(null);
            ControleurScenario.Draw(null);
            ControleurVaisseaux.Draw(null);
            ControleurMessages.Draw(null);
            GUIController.Draw();
        }

        public void EtreNotifierNouvelEtatPartie(ControleurScenario.NouvelEtatPartieHandler handler)
        {
            ControleurScenario.NouvelEtatPartie += handler;
        }

        public void HandleInput()
        {
            ControleurCollisions.Debug = this.Debug && (Core.Input.Facade.estPesee(Preferences.toucheDebug, this.Main.JoueursConnectes[0].Manette, this.Scene.Nom));

            if (ControleurVaisseaux.VaisseauControllablePresent)
            {
#if XBOX || MANETTE_WINDOWS
                Vector2 donneesThumbstick = Core.Input.Facade.positionThumbstick(this.Main.JoueursConnectes[0].Manette, true, this.Scene.Nom);
#else
                Vector2 donneesThumbstick = Core.Input.Facade.positionDeltaSouris(this.Main.JoueursConnectes[0].Manette, this.Scene.Nom);
#endif

                ControleurVaisseaux.NextInputVaisseau = donneesThumbstick;
            }

            if (ControleurVaisseaux.VaisseauCollecteurActif && Core.Input.Facade.estPeseeUneSeuleFois(Preferences.toucheRetour, this.Main.JoueursConnectes[0].Manette, this.Scene.Nom))
                ControleurVaisseaux.VaisseauCollecteurActif = false;

            //todo
            if (Core.Input.Facade.estPesee(Preferences.toucheVueAvancee, this.Main.JoueursConnectes[0].Manette, this.Scene.Nom))
            {
                GUIController.doShowAdvancedView();
            }

            else
            {
                GUIController.doHideAdvancedView();
            }

            //todo
#if XBOX || MANETTE_WINDOWS
            Vector2 positionThumb = Core.Input.Facade.positionThumbstick(Main.JoueursConnectes[0].Manette, true, Scene.Nom);

            ControleurJoueur.doGamePadJoystickMoved(PlayerIndex.One, Buttons.LeftStick, new Vector3(positionThumb.X * GUIController.Cursor.Vitesse, -positionThumb.Y * GUIController.Cursor.Vitesse, 0));
#else
            Vector2 positionSouris = Core.Input.Facade.positionDeltaSouris(Main.JoueursConnectes[0].Manette, Scene.Nom);
            PlayerController.doMouseMoved(PlayerIndex.One, new Vector3(positionSouris, 0));
#endif
        }

        private void doEnnemiAtteintFinTrajet(Ennemi ennemi, CorpsCeleste corpsCeleste)
        {
            if (Etat == EtatPartie.Gagnee)
                return;

            if (!this.ModeDemo && this.Etat != EtatPartie.Perdue)
            {
                foreach (var joueur in this.Main.JoueursConnectes)
                    Core.Input.Facade.vibrerManette(joueur.Manette, 300, 0.5f, 0.5f);
            }
        }

        private void doGameStateChanged(EtatPartie etat)
        {
            if (etat != EtatPartie.Gagnee && etat != EtatPartie.Perdue)
                return;

#if WINDOWS && !MANETTE_WINDOWS
            Core.Input.Facade.considerTouches(
                this.Main.JoueursConnectes[0].Manette,
                new List<Microsoft.Xna.Framework.Input.Keys>() { Preferences.toucheRetourMenu, Preferences.toucheRetourMenu2 },
                this.Scene.Nom);

            Core.Input.Facade.considerTouches(
                this.Main.JoueursConnectes[0].Manette,
                new List<BoutonSouris>() { Preferences.toucheSelection, Preferences.toucheRetour },
                this.Scene.Nom);
#else
            Core.Input.Facade.considerTouches(
                    this.Main.JoueursConnectes[0].Manette,
                    new List<Microsoft.Xna.Framework.Input.Buttons>() { Preferences.toucheRetourMenu, Preferences.toucheRetourMenu2 },
                    this.Scene.Nom);

            Core.Input.Facade.considerThumbsticks(
                    this.Main.JoueursConnectes[0].Manette,
                    new List<Microsoft.Xna.Framework.Input.Buttons>() { },
                    this.Scene.Nom);
#endif
        }

        private void doCorpsCelesteDetruit(IObjetPhysique objet)
        {
            Core.Input.Facade.vibrerManette(this.Main.JoueursConnectes[0].Manette, 300, 0.5f, 0.5f);
        }
    }
}