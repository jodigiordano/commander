namespace EphemereGames.Commander.Simulation
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;

    class Enemy : ICollidable, IPhysical, ILivingObject, IDestroyable
    {
        public Vector3 Position                                     { get { return position; } set { position = value; } }
        public float Speed                                          { get; set; }
        public float Rotation                                       { get; set; }
        public Vector3 Direction                                    { get; set; }
        public Shape Shape                                          { get; set; }
        public Circle Circle                                        { get; set; }
        public Line Line                                           { get; set; }
        public PhysicalRectangle Rectangle                          { get; set; }
        public float LifePoints                                     { get; set; }
        public float StartingLifePoints                             { get; set; }
        public float AttackPoints                                   { get; set; }
        public int CashValue;
        public int PointsValue;
        public float Resistance;
        public bool Alive                                           { get { return LifePoints > 0 && !EndOfPathReached; } }
        public bool EndPathPreview                                  { get { return Displacement > PathPreview.Length; } }
        public Image Image;
        public Particle ExplodingEffect;
        public Particle MovingEffect;
        public float RotationSpeed;
        public Path Path;
        public Path PathPreview;
        public double Displacement;
        public Simulator Simulator;
        public string Name;
        public EnemyType Type;
        public List<Mineral> Minerals;
        public Vector3 Translation;
        public Vector3 PositionLastHit;
        public Color Color;
        public int Level;
        public double FadeInTime;
        public int WaveId;
        public bool EndOfPathReached;
        public bool BeingHit                                    { get { return BeingHitCounter > 0; } }
        public double BeingHitPourc                             { get { return Math.Max(0, BeingHitCounter / BeingHitShowTime); }}

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
        public double VisualPriority;

        private const int BeingHitShowTime = 250;
        private double BeingHitCounter;
        private Text CashValueText;


        public Enemy()
        {
            Minerals = new List<Mineral>();

            MovingEffect = null;

            AttackPoints = 1;
            PointsValue = 1;

            Shape = Shape.Rectangle;
            Rectangle = new PhysicalRectangle(0, 0, 1, 1);
            Circle = new Circle(Vector3.Zero, 1);
            Type = EnemyType.Asteroid;
            Level = 0;
            WaveId = -1;
            EndOfPathReached = false;

            CashValueText = new Text(@"Pixelite") { SizeX = 2 };
        }


        public void Initialize()
        {
            if (Image == null)
                LoadAssets();

            MultipleLasersEffect = Simulator.Scene.Particles.Get(@"etincelleLaser");
            MissileEffect = Simulator.Scene.Particles.Get(@"etincelleMissile");
            LaserEffect = Simulator.Scene.Particles.Get(@"etincelleLaserSimple");
            SlowMotionEffect = Simulator.Scene.Particles.Get(@"etincelleSlowMotionTouche");
            ExplodingEffect = Simulator.Scene.Particles.Get(@"explosionEnnemi");
            NanobotsInfectionEffect = Simulator.Scene.Particles.Get(@"nanobots");
            NanobotsInfectionEffect.ParticleEffect[0].ReleaseColour = Color.Red.ToVector3();

            VisualPriority = Simulator.TweakingController.EnemiesFactory.GetVisualPriority(Type, 0);
            Image.VisualPriority = VisualPriority;
            ExplodingEffect.VisualPriority = VisualPriority - 0.001f;
            MultipleLasersEffect.VisualPriority = VisualPriority - 0.001f;
            MissileEffect.VisualPriority = VisualPriority - 0.001f;
            LaserEffect.VisualPriority = VisualPriority - 0.001f;
            SlowMotionEffect.VisualPriority = VisualPriority - 0.001f;
            NanobotsInfectionEffect.VisualPriority = VisualPriority - 0.001f;

            Resistance = 0;

            Path.Position(Displacement, ref position);

            Rectangle.Width = Rectangle.Height = Simulator.TweakingController.EnemiesFactory.GetSize(Type);
            Circle.Radius = Rectangle.Width / 2;

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

            EndOfPathReached = false;

            BeingHitCounter = 0;
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
            if (EndOfPathReached)
                return;

            if (ImpulseTime > 0)
            {
                Translation += ImpulseDirection * ImpulseSpeed;

                ImpulseTime -= Preferences.TargetElapsedTimeMs;
            }

            Resistance = Math.Max(Resistance - 0.01f, 0);

            //Displacement += Path.LengthDelta;
            Displacement += Math.Max(Speed - Resistance, 0);

            Path.Position(Displacement, ref position);
            Vector3.Add(ref position, ref Translation, out position);

            if (Displacement > Path.Length)
            {
                EndOfPathReached = true;
                return;
            }

            Rectangle.X = (int)Position.X - Rectangle.Width / 2;
            Rectangle.Y = (int)Position.Y - Rectangle.Height / 2;
            Circle.Position.X = Position.X;
            Circle.Position.Y = Position.Y;

            Image.Rotation += RotationSpeed;

            if (NanobotsInfectionTime > 0 && LifePoints > 0)
            {
                LifePoints = Math.Max(0, LifePoints - NanobotsInfectionHitPoints);
                NanobotsInfectionTime -= Preferences.TargetElapsedTimeMs;
            }

            BeingHitCounter -= Preferences.TargetElapsedTimeMs;
        }


        public void Draw()
        {
            if (!Alive)
                return;

            float pourcPath = Path.GetPercentage(Displacement);

            VisualPriority = Simulator.TweakingController.EnemiesFactory.GetVisualPriority(Type, pourcPath);

            Image.Position = Position;
            Image.VisualPriority = VisualPriority;

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

            Simulator.Scene.Add(Image);
        }


        public void DoHit(ILivingObject by)
        {
            if (!(by is SlowMotionBullet))
                LifePoints -= by.AttackPoints;

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

                    this.Resistance = (float)Math.Min(this.Resistance + pointsAttaqueEffectif, 0.95 * this.Speed);
                    SlowMotionEffect.Trigger(ref this.position);
                }
                else if (p is NanobotsBullet)
                {
                    NanobotsBullet nb = p as NanobotsBullet;

                    NanobotsInfectionTime = nb.InfectionTime;
                    NanobotsInfectionHitPoints = nb.AttackPoints;
                }

                BeingHitCounter = BeingHitShowTime;

                return;
            }


            CelestialBody c = by as CelestialBody;

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

                Simulator.Scene.Particles.Return(ExplodingEffect);
            }

            Simulator.Scene.Particles.Return(MultipleLasersEffect);
            Simulator.Scene.Particles.Return(MissileEffect);
            Simulator.Scene.Particles.Return(LaserEffect);
            Simulator.Scene.Particles.Return(SlowMotionEffect);
            Simulator.Scene.Particles.Return(NanobotsInfectionEffect);


            if (!Simulator.DemoMode && Simulator.State == GameState.Running && CashValue > 0)
                Simulator.Scene.Animations.Add(new CashTakenAnimation(CashValue, Position, VisualPriorities.Default.EnemyCashAnimation));
        }
    }
}
