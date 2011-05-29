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
        SlowMotion,
        Gunner,
        Nanobots
    };

    abstract class Projectile : IObjetPhysique, ILivingObject
    {
        public static Pool<ProjectileBase> PoolProjectilesBase = new Pool<ProjectileBase>();
        public static Pool<ProjectileLaserMultiple> PoolProjectilesLaserMultiple = new Pool<ProjectileLaserMultiple>();
        public static Pool<ProjectileLaserSimple> PoolProjectilesLaserSimple = new Pool<ProjectileLaserSimple>();
        public static Pool<ProjectileMissile> PoolProjectilesMissile = new Pool<ProjectileMissile>();
        public static Pool<ProjectileSlowMotion> PoolProjectilesSlowMotion = new Pool<ProjectileSlowMotion>();
        public static Pool<GunnerBullet> PoolGunnerBullets = new Pool<GunnerBullet>();
        public static Pool<NanobotsBullet> PoolNanobotsBullets = new Pool<NanobotsBullet>();

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
        public float LifePoints                                     { get; set; }
        public float AttackPoints                                   { get; set; }

        public Scene Scene;
        public float PrioriteAffichage;
        public bool Alive                                           { get { return LifePoints > 0; } }
        public IVisible RepresentationVivant                        { get; set; }
        public ParticuleEffectWrapper ExplodingEffect               { get; set; }
        public ParticuleEffectWrapper MovingEffect                  { get; set; }


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


        public virtual void Update()
        {
            switch (Forme)
            {
                case Forme.Rectangle:
                    Rectangle.X = (int)Position.X - Rectangle.Width / 2;
                    Rectangle.Y = (int)Position.Y - Rectangle.Height / 2;
                    break;

                case Forme.Cercle:
                    Cercle.Position = this.Position;
                    Rectangle.X = (int) (Position.X - Cercle.Radius);
                    Rectangle.Y = (int) (Position.Y - Cercle.Radius);
                    Rectangle.Width = (int) (Cercle.Radius * 2);
                    Rectangle.Height = (int) (Cercle.Radius * 2);
                    break;
            }
        }


        public virtual void Draw()
        {
            if (Alive && MovingEffect != null)
                MovingEffect.Emettre(ref this.position);

            if (Alive && RepresentationVivant != null)
            {
                RepresentationVivant.Position = this.Position;

                Scene.ajouterScenable(RepresentationVivant);
            }
        }


        public virtual void DoHit(ILivingObject by)
        {
            LifePoints = 0;
        }


        public virtual void DoDie()
        {
            LifePoints = 0;

            if (ExplodingEffect != null)
                ExplodingEffect.Emettre(ref this.position);

            if (MovingEffect != null)
                Scene.Particules.retourner(MovingEffect);

            if (ExplodingEffect != null)
                Scene.Particules.retourner(ExplodingEffect);
        }


        public virtual void DoDieSilent()
        {
            LifePoints = 0;

            if (MovingEffect != null)
                Scene.Particules.retourner(MovingEffect);

            if (ExplodingEffect != null)
                Scene.Particules.retourner(ExplodingEffect);
        }
    }
}