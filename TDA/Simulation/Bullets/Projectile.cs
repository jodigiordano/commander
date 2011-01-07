namespace EphemereGames.Commander
{
    using EphemereGames.Core.Physique;
    using EphemereGames.Core.Utilities;
    using EphemereGames.Core.Visuel;
    using Microsoft.Xna.Framework;

    enum BulletType
    {
        Base,
        Missile,
        Missile2,
        LaserMultiple,
        LaserSimple,
        Aucun,
        SlowMotion
    };

    abstract class Projectile : IObjetPhysique, IObjetVivant
    {
        public static Pool<ProjectileBase> PoolProjectilesBase = new Pool<ProjectileBase>();
        public static Pool<ProjectileLaserMultiple> PoolProjectilesLaserMultiple = new Pool<ProjectileLaserMultiple>();
        public static Pool<ProjectileLaserSimple> PoolProjectilesLaserSimple = new Pool<ProjectileLaserSimple>();
        public static Pool<ProjectileMissile> PoolProjectilesMissile = new Pool<ProjectileMissile>();
        public static Pool<ProjectileSlowMotion> PoolProjectilesSlowMotion = new Pool<ProjectileSlowMotion>();

        protected Vector3 position;
        public Vector3 Position                                     { get { return position; } set { position = value; } }
        public float Vitesse                                        { get; set; }
        public float Masse                                          { get; set; }
        public float Rotation                                       { get; set; }
        public float ResistanceRotation                             { get; set; }
        public Vector3 Direction                                    { get; set; }
        public Forme Forme                                          { get; set; }
        public Cercle Cercle                                        { get; set; }
        public Ligne Ligne                                          { get; set; }

        public RectanglePhysique Rectangle                          { get; set; }
        public float PointsVie                                      { get; set; }
        public float PointsAttaque                                  { get; set; }

        public Scene Scene;
        public float PrioriteAffichage;
        public bool EstVivant                                       { get { return PointsVie > 0; } }
        public IVisible RepresentationVivant                        { get; set; }
        public ParticuleEffectWrapper RepresentationExplose         { get; set; }
        public ParticuleEffectWrapper RepresentationDeplacement     { get; set; }


        public virtual void Initialize()
        {
            Forme = Forme.Rectangle;

            if (Direction != Vector3.Zero)
            {
                Vector3 direction = Direction;
                direction.Normalize();
                Direction = direction;
            }
        }


        public virtual void Update(GameTime gameTime)
        {
            switch (Forme)
            {
                case Forme.Rectangle:
                    Rectangle.X = (int)Position.X - Rectangle.Width / 2;
                    Rectangle.Y = (int)Position.Y - Rectangle.Height / 2;
                    break;

                case Forme.Cercle:
                    Cercle.Position = this.Position;
                    break;
            }

            if (EstVivant && RepresentationDeplacement != null)
                RepresentationDeplacement.Emettre(ref this.position);
        }


        public virtual void Draw(GameTime gameTime)
        {
            if (EstVivant && RepresentationVivant != null)
            {
                RepresentationVivant.Position = this.Position;

                Scene.ajouterScenable(RepresentationVivant);
            }
        }


        public virtual void doTouche(IObjetVivant par)
        {
            PointsVie = 0;
        }


        public virtual void doMeurt()
        {
            PointsVie = 0;

            if (RepresentationExplose != null)
                RepresentationExplose.Emettre(ref this.position);

            if (RepresentationDeplacement != null)
                Scene.Particules.retourner(RepresentationDeplacement);

            if (RepresentationExplose != null)
                Scene.Particules.retourner(RepresentationExplose);
        }

        public virtual void doMeurtSilencieusement()
        {
            PointsVie = 0;

            if (RepresentationDeplacement != null)
                Scene.Particules.retourner(RepresentationDeplacement);

            if (RepresentationExplose != null)
                Scene.Particules.retourner(RepresentationExplose);
        }

        public void getRectangle(ref Rectangle rectangle)
        {
            rectangle.X = this.Rectangle.X;
            rectangle.Y = this.Rectangle.Y;
            rectangle.Width = this.Rectangle.Width;
            rectangle.Height = this.Rectangle.Height;
        }
    }
}