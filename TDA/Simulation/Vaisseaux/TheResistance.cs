namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Core.Visuel;
    using Core.Physique;

    class TheResistance : DrawableGameComponent
    {
        public double TempsActif;
        public bool EntreAuBercail;

        private Simulation Simulation;
        private List<Vaisseau> Vaisseaux;
        private List<Ennemi> Ennemis;
        public CorpsCeleste CorpsCelesteDepart;

        public virtual bool Actif
        {
            get { return TempsActif > 0; }
        }

        public bool CibleAtteinte
        {
            get { return Vaisseaux[0].CibleAtteinte && Vaisseaux[1].CibleAtteinte && Vaisseaux[2].CibleAtteinte; }
        }

        public TheResistance(Simulation simulation, CorpsCeleste corpsCelesteDepart, List<Ennemi> ennemis)
            : base(simulation.Main)
        {
            Simulation = simulation;
            CorpsCelesteDepart = corpsCelesteDepart;
            Ennemis = ennemis;

            EntreAuBercail = false;

            Vaisseaux = new List<Vaisseau>();

            Vaisseau v = new Vaisseau(simulation);
            v.Representation = new IVisible(Core.Persistance.Facade.recuperer<Texture2D>("Resistance1"), Vector3.Zero, Simulation.Scene);
            v.Representation.Taille = 4;
            v.Representation.Origine = v.Representation.Centre;
            v.Representation.PrioriteAffichage = Preferences.PrioriteSimulationCorpsCeleste - 0.1f;
            v.CadenceTir = 100;
            v.PuissanceProjectile = 10;
            v.RotationMaximaleRad = 0.15f;
            v.CorpsCelesteDepart = corpsCelesteDepart;
            Vaisseaux.Add(v);

            v = new Vaisseau(simulation);
            v.Representation = new IVisible(Core.Persistance.Facade.recuperer<Texture2D>("Resistance2"), Vector3.Zero, Simulation.Scene);
            v.Representation.Taille = 4;
            v.Representation.Origine = v.Representation.Centre;
            v.Representation.PrioriteAffichage = Preferences.PrioriteSimulationCorpsCeleste - 0.1f;
            v.CadenceTir = 200;
            v.PuissanceProjectile = 30;
            v.RotationMaximaleRad = 0.05f;
            v.CorpsCelesteDepart = corpsCelesteDepart;
            Vaisseaux.Add(v);

            v = new Vaisseau(simulation);
            v.Representation = new IVisible(Core.Persistance.Facade.recuperer<Texture2D>("Resistance3"), Vector3.Zero, Simulation.Scene);
            v.Representation.Taille = 4;
            v.Representation.Origine = v.Representation.Centre;
            v.Representation.PrioriteAffichage = Preferences.PrioriteSimulationCorpsCeleste - 0.1f;
            v.CadenceTir = 500;
            v.PuissanceProjectile = 100;
            v.RotationMaximaleRad = 0.2f;
            v.CorpsCelesteDepart = corpsCelesteDepart;
            Vaisseaux.Add(v);
        }

        public override void Update(GameTime gameTime)
        {
            TempsActif -= gameTime.ElapsedGameTime.TotalMilliseconds;

            for (int i = 0; i < Vaisseaux.Count; i++)
            {
                if (!Vaisseaux[i].EnAttaque)
                {
                    Vector3 direction = ((Ennemis.Count > i) ? Ennemis[i].Position : CorpsCelesteDepart.Position) - Vaisseaux[i].Position;
                    direction.Normalize();

                    Vaisseaux[i].PositionVisee = ((Ennemis.Count > i) ? Ennemis[i].Position : CorpsCelesteDepart.Position) + direction * 100;
                }
            }

            foreach (var vaisseau in Vaisseaux)
            {

                vaisseau.doModeAutomatique();
                vaisseau.Update(gameTime);
            }
        }

        private List<Projectile> projectilesCeTick = new List<Projectile>();
        public List<Projectile> ProjectilesCeTick(GameTime gameTime)
        {
            projectilesCeTick.Clear();

            foreach (var vaisseau in Vaisseaux)
                projectilesCeTick.AddRange(vaisseau.ProjectilesCeTick(gameTime));

            return projectilesCeTick;
        }


        public void doDisparaitre()
        {

            foreach (var vaisseau in Vaisseaux)
            {
                vaisseau.PositionVisee = CorpsCelesteDepart.Position;
                vaisseau.doDisparaitre();
            }
        }


        public override void Draw(GameTime gameTime)
        {
            foreach (var vaisseau in Vaisseaux)
                vaisseau.Draw(gameTime);
        }
    }
}
