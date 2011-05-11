namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using EphemereGames.Core.Physique;

    class ControleurVaisseaux : DrawableGameComponent
    {
        public event PhysicalObjectHandler ObjetCree;
        public event PhysicalObjectHandler ObjetDetruit;

        public Dictionary<PowerUpType, bool> AvailableSpaceships;
        public List<Ennemi> Ennemis;
        public Vector3 NextInputVaisseau;
        public HumanBattleship HumanBattleship;

        private Simulation Simulation;
        private VaisseauDoItYourself VaisseauDoItYourself;
        private VaisseauCollecteur VaisseauCollecteur;
        private TheResistance TheResistance;

        public bool VaisseauControllablePresent { get { return VaisseauDoItYourself != null || VaisseauCollecteur != null; } }

        public bool VaisseauCollecteurActif
        {
            get { return VaisseauCollecteur != null && VaisseauCollecteur.Actif; }
            set { if (VaisseauCollecteur != null) VaisseauCollecteur.Actif = value; }
        }

        public ControleurVaisseaux(Simulation simulation)
            : base(simulation.Main)
        {
            this.Simulation = simulation;
            this.AvailableSpaceships = new Dictionary<PowerUpType, bool>();
        }


        public override void Initialize()
        {
            this.AvailableSpaceships.Add(PowerUpType.Spaceship, true);
            this.AvailableSpaceships.Add(PowerUpType.Collector, true);
            this.AvailableSpaceships.Add(PowerUpType.TheResistance, true);
        }


        public override void Update(GameTime gameTime)
        {
            this.AvailableSpaceships[PowerUpType.Spaceship] = (VaisseauDoItYourself == null || !VaisseauDoItYourself.Actif);
            this.AvailableSpaceships[PowerUpType.Collector] = (VaisseauCollecteur == null || !VaisseauCollecteur.Actif);
            this.AvailableSpaceships[PowerUpType.TheResistance] = (TheResistance == null || !TheResistance.Actif);

            traiterVaisseauDoItYourself(gameTime);
            traiterVaisseauCollecteur(gameTime);
            traiterTheResistance(gameTime);

            NextInputVaisseau = Vector3.Zero;
        }


        private void traiterTheResistance(GameTime gameTime)
        {
            if (TheResistance != null)
            {
                TheResistance.Update(gameTime);

                if (!TheResistance.Actif && !TheResistance.EntreAuBercail)
                {
                    TheResistance.EntreAuBercail = true;
                    TheResistance.doDisparaitre();

                    EphemereGames.Core.Audio.Facade.jouerEffetSonore("Partie", "sfxPowerUpResistanceOut");
                }

                if (!TheResistance.EntreAuBercail)
                {
                    List<Projectile> projectiles = TheResistance.ProjectilesCeTick(gameTime);

                    for (int i = 0; i < projectiles.Count; i++)
                        notifyObjetCree(projectiles[i]);
                }

                if (TheResistance.EntreAuBercail && TheResistance.CibleAtteinte)
                    TheResistance = null;
            }
        }


        private void traiterVaisseauCollecteur(GameTime gameTime)
        {
            if (VaisseauCollecteur != null)
            {
                VaisseauCollecteur.NextInput = NextInputVaisseau;
                VaisseauCollecteur.Update(gameTime);

                if (!VaisseauCollecteur.Actif && !VaisseauCollecteur.ModeAutomatique)
                {
                    VaisseauCollecteur.ModeAutomatique = true;
                    VaisseauCollecteur.doDisparaitre();

                    EphemereGames.Core.Audio.Facade.jouerEffetSonore("Partie", "sfxPowerUpCollecteurOut");

                    notifyObjetDetruit(VaisseauCollecteur);
                }

                if (VaisseauCollecteur.ModeAutomatique && VaisseauCollecteur.CibleAtteinte)
                    VaisseauCollecteur = null;
            }
        }


        private void traiterVaisseauDoItYourself(GameTime gameTime)
        {
            if (VaisseauDoItYourself != null)
            {
                VaisseauDoItYourself.NextInput = NextInputVaisseau;
                VaisseauDoItYourself.Update(gameTime);

                if (!VaisseauDoItYourself.Actif && !VaisseauDoItYourself.ModeAutomatique)
                {
                    VaisseauDoItYourself.ModeAutomatique = true;
                    VaisseauDoItYourself.doDisparaitre();

                    EphemereGames.Core.Audio.Facade.jouerEffetSonore("Partie", "sfxPowerUpDoItYourselfOut");

                    notifyObjetDetruit(VaisseauDoItYourself);
                }

                if (!VaisseauDoItYourself.ModeAutomatique)
                {
                    List<Projectile> projectiles = VaisseauDoItYourself.ProjectilesCeTick(gameTime);

                    for (int i = 0; i < projectiles.Count; i++)
                        notifyObjetCree(projectiles[i]);
                }

                if (VaisseauDoItYourself.ModeAutomatique && VaisseauDoItYourself.CibleAtteinte)
                {
                    VaisseauDoItYourself = null;
                }
            }
        }


        public override void Draw(GameTime gameTime)
        {
            if (VaisseauDoItYourself != null)
                VaisseauDoItYourself.Draw(null);

            if (VaisseauCollecteur != null)
                VaisseauCollecteur.Draw(null);

            if (TheResistance != null)
                TheResistance.Draw(null);
        }


        protected virtual void notifyObjetCree(IObjetPhysique objet)
        {
            if (ObjetCree != null)
                ObjetCree(objet);
        }


        protected virtual void notifyObjetDetruit(IObjetPhysique objet)
        {
            if (ObjetDetruit != null)
                ObjetDetruit(objet);
        }


        public void doAcheterDoItYourself()
        {
            VaisseauDoItYourself = new VaisseauDoItYourself(Simulation);
            VaisseauDoItYourself.CadenceTir = 100;
            VaisseauDoItYourself.PuissanceProjectile = 50;
            VaisseauDoItYourself.Position = HumanBattleship.Position; 
            VaisseauDoItYourself.PrioriteAffichage = Preferences.PrioriteSimulationCorpsCeleste - 0.1f;
            VaisseauDoItYourself.Bouncing = new Vector3(Main.Random.Next(-25, 25), Main.Random.Next(-25, 25), 0);
            VaisseauDoItYourself.TempsActif = 5000;
            VaisseauDoItYourself.ObjetDepart = HumanBattleship;
            this.AvailableSpaceships[PowerUpType.Spaceship] = false;

            EphemereGames.Core.Audio.Facade.jouerEffetSonore("Partie", "sfxPowerUpDoItYourselfIn");

            notifyObjetCree(VaisseauDoItYourself);
        }


        public void doAcheterCollecteur()
        {
            VaisseauCollecteur = new VaisseauCollecteur(Simulation);
            VaisseauCollecteur.Position = HumanBattleship.Position;
            VaisseauCollecteur.PrioriteAffichage = Preferences.PrioriteSimulationCorpsCeleste - 0.1f;
            VaisseauCollecteur.Bouncing = new Vector3(Main.Random.Next(-25, 25), Main.Random.Next(-25, 25), 0);
            VaisseauCollecteur.ObjetDepart = HumanBattleship;
            this.AvailableSpaceships[PowerUpType.Collector] = false;

            EphemereGames.Core.Audio.Facade.jouerEffetSonore("Partie", "sfxPowerUpCollecteurIn");

            notifyObjetCree(VaisseauCollecteur);
        }


        public void doAcheterTheResistance()
        {
            TheResistance = new TheResistance(Simulation);
            TheResistance.TempsActif = 20000;
            TheResistance.Initialize(HumanBattleship, Ennemis);

            this.AvailableSpaceships[PowerUpType.TheResistance] = false;

            EphemereGames.Core.Audio.Facade.jouerEffetSonore("Partie", "sfxPowerUpResistanceIn");

            notifyObjetCree(TheResistance);
        }
    }
}
