namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using EphemereGames.Core.Visuel;
    using EphemereGames.Core.Physique;

    class TheResistance : IObjetPhysique
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
        {
            Simulation = simulation;
            CorpsCelesteDepart = corpsCelesteDepart;
            Ennemis = ennemis;

            EntreAuBercail = false;

            Vaisseaux = new List<Vaisseau>();

            Vaisseau v = new Vaisseau(simulation);
            v.Representation = new IVisible(EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>("Resistance1"), Vector3.Zero);
            v.Representation.Taille = 4;
            v.Representation.Origine = v.Representation.Centre;
            v.Representation.VisualPriority = Preferences.PrioriteSimulationCorpsCeleste - 0.1f;
            v.CadenceTir = 100;
            v.PuissanceProjectile = 10;
            v.RotationMaximaleRad = 0.15f;
            v.CorpsCelesteDepart = corpsCelesteDepart;
            Vaisseaux.Add(v);

            v = new Vaisseau(simulation);
            v.Representation = new IVisible(EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>("Resistance2"), Vector3.Zero);
            v.Representation.Taille = 4;
            v.Representation.Origine = v.Representation.Centre;
            v.Representation.VisualPriority = Preferences.PrioriteSimulationCorpsCeleste - 0.1f;
            v.CadenceTir = 200;
            v.PuissanceProjectile = 30;
            v.RotationMaximaleRad = 0.05f;
            v.CorpsCelesteDepart = corpsCelesteDepart;
            Vaisseaux.Add(v);

            v = new Vaisseau(simulation);
            v.Representation = new IVisible(EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>("Resistance3"), Vector3.Zero);
            v.Representation.Taille = 4;
            v.Representation.Origine = v.Representation.Centre;
            v.Representation.VisualPriority = Preferences.PrioriteSimulationCorpsCeleste - 0.1f;
            v.CadenceTir = 500;
            v.PuissanceProjectile = 100;
            v.RotationMaximaleRad = 0.2f;
            v.CorpsCelesteDepart = corpsCelesteDepart;
            Vaisseaux.Add(v);
        }

        public void Update(GameTime gameTime)
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


        public void Draw(GameTime gameTime)
        {
            foreach (var vaisseau in Vaisseaux)
                vaisseau.Draw(gameTime);
        }

        #region IObjetPhysique Membres

        public Vector3 Position { get; set; }
        public float Vitesse { get; set; }
        public Vector3 Direction { get; set; }
        public float Rotation { get; set; }
        public Forme Forme { get; set; }
        public Cercle Cercle { get; set; }
        public RectanglePhysique Rectangle { get; set; }
        public Ligne Ligne { get; set; }

        #endregion
    }
}
