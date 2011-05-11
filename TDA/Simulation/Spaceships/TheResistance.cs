namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using EphemereGames.Core.Physique;
    using EphemereGames.Core.Visuel;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;


    class TheResistance : IObjetPhysique, PowerUp
    {
        public double TempsActif;
        public bool EntreAuBercail;
        public Vector3 BuyPosition { get; set; }

        private Simulation Simulation;
        private List<Vaisseau> Vaisseaux;
        private List<Ennemi> Ennemis;
        public IObjetPhysique CorpsCelesteDepart;


        public TheResistance(Simulation simulation)
        {
            Simulation = simulation;

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
            Vaisseaux.Add(v);

            v = new Vaisseau(simulation);
            v.Representation = new IVisible(EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>("Resistance2"), Vector3.Zero);
            v.Representation.Taille = 4;
            v.Representation.Origine = v.Representation.Centre;
            v.Representation.VisualPriority = Preferences.PrioriteSimulationCorpsCeleste - 0.1f;
            v.CadenceTir = 200;
            v.PuissanceProjectile = 30;
            v.RotationMaximaleRad = 0.05f;
            Vaisseaux.Add(v);

            v = new Vaisseau(simulation);
            v.Representation = new IVisible(EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>("Resistance3"), Vector3.Zero);
            v.Representation.Taille = 4;
            v.Representation.Origine = v.Representation.Centre;
            v.Representation.VisualPriority = Preferences.PrioriteSimulationCorpsCeleste - 0.1f;
            v.CadenceTir = 500;
            v.PuissanceProjectile = 100;
            v.RotationMaximaleRad = 0.2f;
            Vaisseaux.Add(v);
        }


        public void Initialize(IObjetPhysique corpsCelesteDepart, List<Ennemi> ennemis)
        {
            CorpsCelesteDepart = corpsCelesteDepart;
            Ennemis = ennemis;

            foreach (var spaceship in Vaisseaux)
            {
                spaceship.ObjetDepart = CorpsCelesteDepart;

                if (corpsCelesteDepart != null)
                    spaceship.Position = corpsCelesteDepart.Position;
            }
        }


        public byte AlphaChannel
        {
            set
            {
                foreach (var spaceship in Vaisseaux)
                    spaceship.Representation.Couleur.A = value;
            }
        }


        public virtual bool Actif
        {
            get { return TempsActif > 0; }
        }


        public bool CibleAtteinte
        {
            get { return Vaisseaux[0].CibleAtteinte && Vaisseaux[1].CibleAtteinte && Vaisseaux[2].CibleAtteinte; }
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


        public PowerUpType Type
        {
            get { return PowerUpType.TheResistance; }
        }


        public string BuyImage
        {
            get { return "TheResistance"; }
        }

        public int BuyPrice
        {
            get { return 250; }
        }
    }
}
