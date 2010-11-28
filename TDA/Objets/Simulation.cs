namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Core.Physique;
    using Core.Visuel;
    using Core.Utilities;

    delegate void ObjetDetruitHandler(IObjetPhysique objet);
    delegate void ObjetCreeHandler(IObjetPhysique objet);

    class Simulation : DrawableGameComponent
    {
        public Scene Scene;
        public Main Main;
        public DescripteurScenario DescriptionScenario;

        private List<DrawableGameComponent> Controleurs;
        private ControleurScenario ControleurScenario;
        private ControleurEnnemis ControleurEnnemis;
        private ControleurProjectiles ControleurProjectiles;
        private ControleurCollisions ControleurCollisions;
        private ControleurJoueur ControleurJoueur;
        private ControleurTourelles ControleurTourelles;
        public ControleurSystemePlanetaire ControleurSystemePlanetaire;
        private ControleurVaisseaux ControleurVaisseaux;
        public ControleurMessages ControleurMessages;
        private PanneauDebug PanneauDebug;
        
        public RectanglePhysique Terrain = new RectanglePhysique(-840, -560, 1680, 1120);

        public bool EnPause;
        public EtatPartie Etat                      { get { return ControleurScenario.Etat; } }
        public int NbEnnemis                        { get { return ControleurEnnemis.NbEnnemis; } }
        public int NbProjectiles                    { get { return ControleurProjectiles.NbProjectiles; } }
        public int NbDetectionsCollisionsCeTick     { get { return ControleurCollisions.NbDetectionsCollisionsCeTick; } }

        private bool modeDemo = false;
        public bool ModeDemo
        {
            get { return modeDemo; }
            set
            {
                modeDemo = value;

                ControleurJoueur.ModeDemo = value;
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
                return ControleurJoueur.CorpsSelectionne;
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
            ControleurJoueur = new ControleurJoueur(this);
            ControleurTourelles = new ControleurTourelles(this);
            ControleurSystemePlanetaire = new ControleurSystemePlanetaire(this);
            ControleurScenario = new ControleurScenario(this, new Scenario(this, DescriptionScenario));
            ControleurVaisseaux = new ControleurVaisseaux(this);
            ControleurMessages = new ControleurMessages(this);

            Controleurs = new List<DrawableGameComponent>();
            Controleurs.Add(ControleurScenario);
            Controleurs.Add(ControleurJoueur);
            Controleurs.Add(ControleurEnnemis);
            Controleurs.Add(ControleurProjectiles);
            Controleurs.Add(ControleurTourelles);
            Controleurs.Add(ControleurSystemePlanetaire);
            Controleurs.Add(ControleurCollisions);
            Controleurs.Add(ControleurVaisseaux);
            Controleurs.Add(ControleurMessages);

            ControleurCollisions.Projectiles = ControleurProjectiles.Projectiles;
            ControleurCollisions.Ennemis = ControleurEnnemis.Ennemis;
            ControleurJoueur.CorpsCelestes = ControleurScenario.CorpsCelestes;
            ControleurSystemePlanetaire.CorpsCelestes = ControleurScenario.CorpsCelestes;
            ControleurTourelles.ControleurSystemePlanetaire = ControleurSystemePlanetaire;
            ControleurEnnemis.VaguesInfinies = ControleurScenario.VaguesInfinies;
            ControleurEnnemis.Vagues = ControleurScenario.Vagues;
            ControleurJoueur.VaguesInfinies = ControleurScenario.VaguesInfinies;
            ControleurJoueur.Vagues = ControleurScenario.Vagues;
            ControleurCollisions.Tourelles = ControleurTourelles.Tourelles;
            ControleurJoueur.Joueur = ControleurScenario.Joueur;
            ControleurJoueur.CorpsCelesteAProteger = ControleurScenario.CorpsCelesteAProteger;
            ControleurJoueur.Chemin = ControleurSystemePlanetaire.Chemin;
            ControleurJoueur.CheminProjection = ControleurSystemePlanetaire.CheminProjection;
            ControleurEnnemis.CheminProjection = ControleurSystemePlanetaire.CheminProjection;
            ControleurTourelles.TourellesDepart = ControleurScenario.TourellesDepart;
            ControleurJoueur.Histoire = ControleurScenario.Scenario;
            ControleurJoueur.Ennemis = ControleurEnnemis.Ennemis;
            ControleurEnnemis.Chemin = ControleurSystemePlanetaire.Chemin;
            ControleurCollisions.CorpsCelestes = ControleurScenario.CorpsCelestes;
            ControleurJoueur.OptionsCVDisponibles = ControleurVaisseaux.OptionsDisponibles;
            ControleurCollisions.Mineraux = ControleurEnnemis.Mineraux;
            ControleurEnnemis.ValeurTotalMineraux = ControleurScenario.Scenario.ValeurMinerauxDonnes;
            ControleurEnnemis.PourcentageMinerauxDonnes = ControleurScenario.Scenario.PourcentageMinerauxDonnes;
            ControleurEnnemis.NbPackViesDonnes = ControleurScenario.Scenario.NbPackViesDonnes;
            ControleurVaisseaux.Ennemis = ControleurEnnemis.Ennemis;
            ControleurJoueur.PositionCurseur = PositionCurseur;
            ControleurMessages.Tourelles = ControleurTourelles.Tourelles;
            ControleurMessages.BulleGUI = ControleurJoueur.BulleGUI;
            ControleurJoueur.CompositionProchaineVague = ControleurEnnemis.CompositionProchaineVague;
            ControleurMessages.CorpsCelesteAProteger = ControleurScenario.CorpsCelesteAProteger;
            ControleurMessages.Curseur = ControleurJoueur.Curseur;
            ControleurMessages.CorpsCelestes = ControleurScenario.CorpsCelestes;
            ControleurMessages.Chemin = ControleurSystemePlanetaire.Chemin;
            ControleurMessages.Sablier = ControleurJoueur.Sablier;


            ControleurCollisions.ObjetTouche += new ControleurCollisions.ObjetToucheHandler(ControleurEnnemis.doObjetTouche);
            ControleurJoueur.AchatTourelleDemande += new ControleurJoueur.AchatTourelleDemandeHandler(ControleurTourelles.doAcheterTourelle);
            ControleurEnnemis.VagueTerminee += new ControleurEnnemis.VagueTermineeHandler(ControleurScenario.doVagueTerminee);
            ControleurEnnemis.ObjetDetruit += new ObjetDetruitHandler(ControleurJoueur.doObjetDetruit);
            ControleurCollisions.DansZoneActivation += new ControleurCollisions.DansZoneActivationHandler(ControleurTourelles.doDansZoneActivationTourelle);
            ControleurTourelles.ObjetCree += new ObjetCreeHandler(ControleurProjectiles.doObjetCree);
            ControleurEnnemis.ObjetCree += new ObjetCreeHandler(ControleurSystemePlanetaire.doObjetCree);
            ControleurProjectiles.ObjetDetruit += new ObjetDetruitHandler(ControleurCollisions.doObjetDetruit);
            ControleurEnnemis.VagueDebutee += new ControleurEnnemis.VagueDebuteeHandler(ControleurJoueur.doVagueDebutee);
            ControleurJoueur.VenteTourelleDemande += new ControleurJoueur.VenteTourelleDemandeHandler(ControleurTourelles.doVendreTourelle);
            ControleurTourelles.TourelleVendue += new ControleurTourelles.TourelleVendueHandler(ControleurJoueur.doTourelleVendue);
            ControleurTourelles.TourelleAchetee += new ControleurTourelles.TourelleAcheteeHandler(ControleurJoueur.doTourelleAchetee);
            ControleurEnnemis.EnnemiAtteintFinTrajet += new ControleurEnnemis.EnnemiAtteintFinTrajetHandler(ControleurScenario.doEnnemiAtteintFinTrajet);
            ControleurJoueur.AchatCollecteurDemande += new ControleurJoueur.AchatCollecteurDemandeHandler(ControleurVaisseaux.doAcheterCollecteur);
            ControleurTourelles.TourelleMiseAJour += new ControleurTourelles.TourelleMiseAJourHandler(ControleurJoueur.doTourelleMiseAJour);
            ControleurJoueur.MiseAJourTourelleDemande += new ControleurJoueur.MiseAJourTourelleDemandeHandler(ControleurTourelles.doMettreAJourTourelle);
            ControleurScenario.NouvelEtatPartie += new ControleurScenario.NouvelEtatPartieHandler(ControleurJoueur.doNouvelEtatPartie);
            ControleurSystemePlanetaire.ObjetDetruit += new ObjetDetruitHandler(ControleurTourelles.doObjetDetruit);
            ControleurSystemePlanetaire.ObjetDetruit += new ObjetDetruitHandler(ControleurJoueur.doObjetDetruit);
            ControleurJoueur.ProchaineVagueDemandee += new ControleurJoueur.ProchaineVagueDemandeeHandler(ControleurEnnemis.doProchaineVagueDemandee);
            ControleurVaisseaux.ObjetCree += new ObjetCreeHandler(ControleurProjectiles.doObjetCree);
            ControleurJoueur.AchatDoItYourselfDemande += new ControleurJoueur.AchatDoItYourselfDemandeHandler(ControleurVaisseaux.doAcheterDoItYourself);
            ControleurVaisseaux.ObjetDetruit += new ObjetDetruitHandler(ControleurJoueur.doObjetDetruit);
            ControleurJoueur.DestructionCorpsCelesteDemande += new ControleurJoueur.DestructionCorpsCelesteDemandeHandler(ControleurSystemePlanetaire.doDetruireCorpsCeleste);
            ControleurSystemePlanetaire.ObjetDetruit += new ObjetDetruitHandler(ControleurCollisions.doObjetDetruit);
            ControleurVaisseaux.ObjetCree += new ObjetCreeHandler(ControleurCollisions.doObjetCree);
            ControleurEnnemis.ObjetCree += new ObjetCreeHandler(ControleurCollisions.doObjetCree);
            ControleurJoueur.AchatTheResistanceDemande += new ControleurJoueur.AchatTheResistanceDemandeHandler(ControleurVaisseaux.doAcheterTheResistance);
            ControleurSystemePlanetaire.ObjetDetruit += new ObjetDetruitHandler(ControleurScenario.doObjetDetruit);

            for (int i = 0; i < Controleurs.Count; i++)
                Controleurs[i].Initialize();

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
            ControleurJoueur.Update(gameTime);
            ControleurSystemePlanetaire.Update(gameTime);
            ControleurScenario.Update(gameTime);
            ControleurVaisseaux.Update(gameTime);
            ControleurMessages.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            ControleurCollisions.Draw(null);
            ControleurProjectiles.Draw(null);
            ControleurEnnemis.Draw(null);
            ControleurJoueur.Draw(null);
            ControleurTourelles.Draw(null);
            ControleurSystemePlanetaire.Draw(null);
            ControleurScenario.Draw(null);
            ControleurVaisseaux.Draw(null);
            ControleurMessages.Draw(null);
        }

        public void EtreNotifierNouvelEtatPartie(ControleurScenario.NouvelEtatPartieHandler handler)
        {
            ControleurScenario.NouvelEtatPartie += handler;
        }
    }
}