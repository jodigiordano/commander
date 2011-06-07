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
        Nanobots,
        RailGun
    };


    abstract class Bullet : IObjetPhysique, ILivingObject
    {
        public static Pool<BasicBullet> PoolBasicBullets = new Pool<BasicBullet>();
        public static Pool<MultipleLasersBullet> PoolMultipleLasersBullets = new Pool<MultipleLasersBullet>();
        public static Pool<LaserBullet> PoolLaserBullets = new Pool<LaserBullet>();
        public static Pool<MissileBullet> PoolMissileBullets = new Pool<MissileBullet>();
        public static Pool<SlowMotionBullet> PoolSlowMotionBullets = new Pool<SlowMotionBullet>();
        public static Pool<GunnerBullet> PoolGunnerBullets = new Pool<GunnerBullet>();
        public static Pool<NanobotsBullet> PoolNanobotsBullets = new Pool<NanobotsBullet>();
        public static Pool<RailGunBullet> PoolRailGunBullets = new Pool<RailGunBullet>();

        protected Vector3 position;
        public Vector3 Position                                     { get { return position; } set { position = value; } }
        public float Speed                                          { get; set; }
        public float Masse                                          { get; set; }
        public float Rotation                                       { get; set; }
        public float ResistanceRotation                             { get; set; }
        public Vector3 Direction                                    { get; set; }
        public Shape Shape                                          { get; set; }
        public Cercle Circle                                        { get; set; }
        public Ligne Line                                           { get; set; }

        public RectanglePhysique Rectangle                          { get; set; }
        public float LifePoints                                     { get; set; }
        public float AttackPoints                                   { get; set; }

        public Scene Scene;
        public float PrioriteAffichage;
        public bool Alive                                           { get { return LifePoints > 0; } }
        public Image Image                                          { get; set; }
        public Particle ExplodingEffect                             { get; set; }
        public Particle MovingEffect                                { get; set; }
        public float DeflectZone;


        public virtual void Initialize()
        {
            Shape = Shape.Rectangle;

            if (Direction != Vector3.Zero)
            {
                Vector3 direction = Direction;
                direction.Normalize();
                Direction = direction;
            }

            DeflectZone = 100;
        }


        public virtual void Update()
        {
            switch (Shape)
            {
                case Shape.Rectangle:
                    Rectangle.X = (int)Position.X - Rectangle.Width / 2;
                    Rectangle.Y = (int)Position.Y - Rectangle.Height / 2;
                    break;

                case Shape.Circle:
                    Circle.Position = this.Position;
                    Rectangle.X = (int) (Position.X - Circle.Radius);
                    Rectangle.Y = (int) (Position.Y - Circle.Radius);
                    Rectangle.Width = (int) (Circle.Radius * 2);
                    Rectangle.Height = (int) (Circle.Radius * 2);
                    break;
            }
        }


        public virtual void Draw()
        {
            if (Alive && MovingEffect != null)
                MovingEffect.Trigger(ref this.position);

            if (Alive && Image != null)
            {
                Image.Position = this.Position;

                Scene.ajouterScenable(Image);
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
                ExplodingEffect.Trigger(ref this.position);

            if (MovingEffect != null)
                Scene.Particles.Return(MovingEffect);

            if (ExplodingEffect != null)
                Scene.Particles.Return(ExplodingEffect);
        }


        public virtual void DoDieSilent()
        {
            LifePoints = 0;

            if (MovingEffect != null)
                Scene.Particles.Return(MovingEffect);

            if (ExplodingEffect != null)
                Scene.Particles.Return(ExplodingEffect);
        }
    }
}