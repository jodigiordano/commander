﻿namespace TDA
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Core.Physique;

    class ControleurVaisseaux : DrawableGameComponent
    {
        public event PhysicalObjectHandler ObjetCree;
        public event PhysicalObjectHandler ObjetDetruit;

        public Dictionary<PowerUp, bool> OptionsDisponibles;
        public List<Ennemi> Ennemis;
        public Vector3 NextInputVaisseau;

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
            this.OptionsDisponibles = new Dictionary<PowerUp, bool>();
        }


        public override void Initialize()
        {
            this.OptionsDisponibles.Add(PowerUp.DoItYourself, true);
            this.OptionsDisponibles.Add(PowerUp.CollectTheRent, true);
            this.OptionsDisponibles.Add(PowerUp.TheResistance, true);
        }


        public override void Update(GameTime gameTime)
        {
            this.OptionsDisponibles[PowerUp.DoItYourself] = (VaisseauDoItYourself == null || !VaisseauDoItYourself.Actif);
            this.OptionsDisponibles[PowerUp.CollectTheRent] = (VaisseauCollecteur == null || !VaisseauCollecteur.Actif);
            this.OptionsDisponibles[PowerUp.TheResistance] = (TheResistance == null || !TheResistance.Actif);

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

                    Core.Audio.Facade.jouerEffetSonore("Partie", "sfxPowerUpResistanceOut");
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

                    Core.Audio.Facade.jouerEffetSonore("Partie", "sfxPowerUpCollecteurOut");

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

                    Core.Audio.Facade.jouerEffetSonore("Partie", "sfxPowerUpDoItYourselfOut");

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


        public void doAcheterDoItYourself(CorpsCeleste corpsCeleste)
        {
            VaisseauDoItYourself = new VaisseauDoItYourself(Simulation);
            VaisseauDoItYourself.CadenceTir = 100;
            VaisseauDoItYourself.PuissanceProjectile = 50;
            VaisseauDoItYourself.Position = corpsCeleste.Position; 
            VaisseauDoItYourself.PrioriteAffichage = Preferences.PrioriteSimulationCorpsCeleste - 0.1f;
            VaisseauDoItYourself.Bouncing = new Vector3(Main.Random.Next(-25, 25), Main.Random.Next(-25, 25), 0);
            VaisseauDoItYourself.TempsActif = 5000;
            VaisseauDoItYourself.CorpsCelesteDepart = corpsCeleste;
            this.OptionsDisponibles[PowerUp.DoItYourself] = false;

            Core.Audio.Facade.jouerEffetSonore("Partie", "sfxPowerUpDoItYourselfIn");

            notifyObjetCree(VaisseauDoItYourself);
        }


        public void doAcheterCollecteur(CorpsCeleste corpsCeleste)
        {
            VaisseauCollecteur = new VaisseauCollecteur(Simulation);
            VaisseauCollecteur.Position = corpsCeleste.Position;
            VaisseauCollecteur.PrioriteAffichage = Preferences.PrioriteSimulationCorpsCeleste - 0.1f;
            VaisseauCollecteur.Bouncing = new Vector3(Main.Random.Next(-25, 25), Main.Random.Next(-25, 25), 0);
            VaisseauCollecteur.CorpsCelesteDepart = corpsCeleste;
            this.OptionsDisponibles[PowerUp.CollectTheRent] = false;

            Core.Audio.Facade.jouerEffetSonore("Partie", "sfxPowerUpCollecteurIn");

            notifyObjetCree(VaisseauCollecteur);
        }


        public void doAcheterTheResistance(CorpsCeleste corpsCeleste)
        {
            TheResistance = new TheResistance(Simulation, corpsCeleste, Ennemis);
            TheResistance.TempsActif = 20000;
            this.OptionsDisponibles[PowerUp.TheResistance] = false;

            Core.Audio.Facade.jouerEffetSonore("Partie", "sfxPowerUpResistanceIn");

            notifyObjetCree(TheResistance);
        }
    }
}