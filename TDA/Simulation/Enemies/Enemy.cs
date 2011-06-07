namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Physique;
    using EphemereGames.Core.Visuel;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    class Enemy : IObjetPhysique, IPhysicalObject, ILivingObject
    {
        public static int NextID { get { return NEXT_ID++; } }

        public delegate void EnemyHandler(Enemy enemy);
        public event EnemyHandler PathEndReached;

        public int Id;
        public Vector3 Position                                     { get { return position; } set { position = value; } }
        public float Speed                                          { get; set; }
        public float Rotation                                       { get; set; }
        public Vector3 Direction                                    { get; set; }
        public Shape Shape                                          { get; set; }
        public Cercle Circle                                        { get; set; }
        public Ligne Line                                           { get; set; }
        public RectanglePhysique Rectangle                          { get; set; }
        public float LifePoints                                     { get; set; }
        public float StartingLifePoints                             { get; set; }
        public float AttackPoints                                   { get; set; }
        public int CashValue;
        public int PointsValue;
        public float Resistance;
        public bool Alive                                           { get { return LifePoints > 0; } }
        public bool EndPathPreview                                  { get { return Displacement > PathPreview.Length; } }
        public Image Image;
        public Particle ExplodingEffect;
        public Particle MovingEffect;
        public float RotationSpeed;
        public Path Path;
        public Path PathPreview;
        public double Displacement;
        public Simulation Simulation;
        public String Name;
        public EnemyType Type;
        public List<Mineral> Minerals;
        public Vector3 Translation;
        public Vector3 PositionLastHit;
        public Color Color;
        public int Level;
        public double FadeInTime;

        public float ImpulseSpeed;
        public Vector3 ImpulseDirection;
        public double ImpulseTime;

        public float NanobotsInfectionTime;
        public float NanobotsInfectionHitPoints;
        public Particle NanobotsInfectionEffect;
        private Vector3 NanobotsInfectionLastPosition;

        private Particle MultipleLasersEffect;
        private Particle MissileEffect;
        private Particle LaserEffect;
        private Particle SlowMotionEffect;
        private static int NEXT_ID = 0;
        private Vector3 position;
        private float VisualPriority;


        public Enemy()
        {
            Minerals = new List<Mineral>();

            MovingEffect = null;

            AttackPoints = 1;
            PointsValue = 1;

            Shape = Shape.Rectangle;
            Rectangle = new RectanglePhysique(0, 0, 1, 1);
            Circle = new Cercle(Vector3.Zero, 1);
            Type = EnemyType.Asteroid;
            Id = NextID;
            Level = 0;
        }


        public void Initialize()
        {
            if (Image == null)
                LoadAssets();

            MultipleLasersEffect = Simulation.Scene.Particles.Get("etincelleLaser");
            MissileEffect = Simulation.Scene.Particles.Get("etincelleMissile");
            LaserEffect = Simulation.Scene.Particles.Get("etincelleLaserSimple");
            SlowMotionEffect = Simulation.Scene.Particles.Get("etincelleSlowMotionTouche");
            ExplodingEffect = Simulation.Scene.Particles.Get("explosionEnnemi");
            NanobotsInfectionEffect = Simulation.Scene.Particles.Get("nanobots");
            NanobotsInfectionEffect.ParticleEffect[0].ReleaseColour = Color.Red.ToVector3();

            VisualPriority = EnemiesFactory.GetVisualPriority(Type, 0);
            Image.VisualPriority = VisualPriority;
            ExplodingEffect.VisualPriority = VisualPriority - 0.001f;
            MultipleLasersEffect.VisualPriority = VisualPriority - 0.001f;
            MissileEffect.VisualPriority = VisualPriority - 0.001f;
            LaserEffect.VisualPriority = VisualPriority - 0.001f;
            SlowMotionEffect.VisualPriority = VisualPriority - 0.001f;
            NanobotsInfectionEffect.VisualPriority = VisualPriority - 0.001f;

            Resistance = 0;

            Path.Position(Displacement, ref position);

            Rectangle.Width = Rectangle.Height = EnemiesFactory.GetSize(Type);
            Circle.Radius = Rectangle.Width / 2 - 3;

            Rectangle.X = (int)Position.X - Rectangle.Width / 2;
            Rectangle.Y = (int)Position.Y - Rectangle.Height / 2;
            Circle.Position.X = Position.X - Circle.Radius;
            Circle.Position.Y = Position.Y - Circle.Radius;

            ExplodingEffect.ParticleEffect[0].ReleaseColour = Color.ToVector3();
            MovingEffect = null;

            RotationSpeed = Main.Random.Next(-5, 6) / 100.0f;

            Minerals.Clear();

            NanobotsInfectionTime = 0;
            NanobotsInfectionHitPoints = 0;
            NanobotsInfectionLastPosition = Position;
        }


        public void LoadAssets()
        {
            Image = new Image(Name)
            {
                SizeX = 3
            };
        }


        public void Update()
        {
            if (ImpulseTime > 0)
            {
                Translation += ImpulseDirection * ImpulseSpeed;

                ImpulseTime -= 16.66;
            }

            Resistance = Math.Max(Resistance - 0.02f, 0);

            Displacement += Math.Max(this.Speed - this.Resistance, 0);

            Path.Position(Displacement, ref position);
            Vector3.Add(ref position, ref this.Translation, out position);

            if (Displacement > Path.Length)
                NotifyPathEndReached();

            Rectangle.X = (int)Position.X - Rectangle.Width / 2;
            Rectangle.Y = (int)Position.Y - Rectangle.Height / 2;
            Circle.Position.X = Position.X;
            Circle.Position.Y = Position.Y;

            Image.Rotation += RotationSpeed;

            if (NanobotsInfectionTime > 0 && LifePoints > 0)
            {
                LifePoints = Math.Max(0, LifePoints - NanobotsInfectionHitPoints);
                NanobotsInfectionTime -= 16.66f;
            }
        }


        public void Draw()
        {
            if (!Alive)
                return;

            float pourcPath = Path.Pourc(Displacement);

            if (pourcPath > 0.96f)
                VisualPriority = EnemiesFactory.GetVisualPriority(Type, pourcPath);

            Image.Position = Position;
            Image.VisualPriority = VisualPriority + pourcPath / 1000f;

            Simulation.Scene.ajouterScenable(Image);

            if (MovingEffect != null)
                MovingEffect.Trigger(ref this.position);

            if (NanobotsInfectionTime > 0)
            {
                Vector3 deplacement;
                Vector3.Subtract(ref this.position, ref NanobotsInfectionLastPosition, out deplacement);

                if (deplacement.X != 0 && deplacement.Y != 0)
                {
                    NanobotsInfectionEffect.Move(ref deplacement);
                    NanobotsInfectionLastPosition = this.position;
                }

                NanobotsInfectionEffect.Trigger(ref this.position);
            }
        }


        public void DoHit(ILivingObject by)
        {
            if (!(by is SlowMotionBullet))
                this.LifePoints -= by.AttackPoints;

            Bullet p = by as Bullet;

            if (p != null)
            {
                PositionLastHit = p.Position;

                if (p is MultipleLasersBullet)
                {
                    MultipleLasersEffect.Trigger(ref this.position);
                }
                else if (p is LaserBullet)
                {
                    LaserEffect.Trigger(ref this.position);
                }
                else if (p is MissileBullet)
                {
                    MissileEffect.Trigger(ref this.position);
                }
                else if (p is SlowMotionBullet)
                {
                    float pointsAttaqueEffectif = (this.Type == EnemyType.Comet) ? p.AttackPoints * 3 : p.AttackPoints;

                    this.Resistance = (float)Math.Min(this.Resistance + pointsAttaqueEffectif, 0.75 * this.Speed);
                    SlowMotionEffect.Trigger(ref this.position);
                }
                else if (p is NanobotsBullet)
                {
                    NanobotsBullet nb = p as NanobotsBullet;

                    NanobotsInfectionTime = nb.InfectionTime;
                    NanobotsInfectionHitPoints = nb.AttackPoints;
                }

                return;
            }


            CorpsCeleste c = by as CorpsCeleste;

            if (c != null)
            {
                PositionLastHit = c.Position;
                return;
            }


            ShootingStar s = by as ShootingStar;

            if (s != null)
            {
                PositionLastHit = s.Position;

                return;
            }
        }


        public void DoDie()
        {
            LifePoints = 0;

            if (ExplodingEffect != null)
            {
                Vector3 direction = this.Position - PositionLastHit;
                direction.Normalize();
                direction *= 150;

                ExplodingEffect.ParticleEffect[0].ReleaseImpulse = new Vector2(direction.X, direction.Y);
                ExplodingEffect.Trigger(ref this.position);

                Simulation.Scene.Particles.Return(ExplodingEffect);
            }

            PathEndReached = null;

            Simulation.Scene.Particles.Return(MultipleLasersEffect);
            Simulation.Scene.Particles.Return(MissileEffect);
            Simulation.Scene.Particles.Return(LaserEffect);
            Simulation.Scene.Particles.Return(SlowMotionEffect);
            Simulation.Scene.Particles.Return(NanobotsInfectionEffect);
        }


        public void Destroy()
        {
            Simulation.EnemiesFactory.ReturnEnemy(this);
        }


        public void UnloadAssets()
        {

        }


        private void NotifyPathEndReached()
        {
            if (PathEndReached != null)
                PathEndReached(this);
        }


        public override int GetHashCode()
        {
            return Id;
        }
    }
}