namespace EphemereGames.Commander.Simulation
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Audio;
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;
    using ProjectMercury.Emitters;


    abstract class Turret : IObjetPhysique, IPhysicalObject
    {
        public Vector3 RelativePosition;
        public Vector3 Position                     { get; set; }
        public Image CanonImage;
        public Image BaseImage;
        public bool Disabled                        { get { return DisabledOverride && DisabledCounter > 0; } set { DisabledOverride = value; } }
        public virtual Enemy EnemyWatched          { get; set; }
        public double TimeLastBullet;
        public TurretType Type                      { get; protected set; }
        public string Name                          { get; protected set; }
        public bool CanSell;
        public bool CanUpdate                       { get { return CanUpdateOverride && (!Disabled && !ActualLevel.Equals(Levels.Last)); } set { CanUpdateOverride = value; } }
        public int BuyPrice                         { get { return ActualLevel.Value.BuyPrice; } }
        public int UpdatePrice                      { get { return (ActualLevel.Equals(Levels.Last)) ? ActualLevel.Value.BuyPrice : ActualLevel.Next.Value.BuyPrice; } }
        public int SellPrice                        { get { return ActualLevel.Value.SellPrice; } }
        public float Range                          { get { return ActualLevel.Value.Range * Simulation.TurretsFactory.BoostLevels[BoostMultiplier].RangeMultiplier; } }
        public double FireRate                      { get { return ActualLevel.Value.FireRate * Simulation.TurretsFactory.BoostLevels[BoostMultiplier].FireRateMultiplier; } }
        public int NbCanons                         { get { return ActualLevel.Value.NbCanons; } }
        public double BuildingTime                  { get { return ActualLevel.Value.BuildingTime; } }
        public BulletType BulletType                { get { return ActualLevel.Value.Bullet; } }
        public bool Watcher;
        public Color Color;
        public bool BackActiveThisTick              { get; private set; }
        public bool BackActiveThisTickOverride;
        public bool Visible;
        public Shape Shape                          { get; set; }
        public Circle Circle                        { get; set; }
        public CelestialBody CelestialBody;
        public bool ToPlaceMode;
        public bool CanPlace;
        public bool ShowRange;
        public bool ShowForm;
        public int BoostMultiplier;
        public bool Wander;
        public bool PlayerControlled;
        public bool UpdatePosition;

        protected LinkedList<TurretLevel> Levels;
        protected string SfxShooting;

        protected Simulator Simulation;
        private bool DisabledOverride;
        private bool CanUpdateOverride;
        private float RotationWander = 0;
        private LinkedListNode<TurretLevel> actualLevel;
        public float DisabledCounter;
        private float DisabledAnnounciationCounter;
        private Image DisabledProgressBarImage;
        private Image DisabledBarImage;
        protected double VisualPriorityBackup;
        private List<Bullet> Bullets = new List<Bullet>();
        public Image RangeImage;
        public byte RangeAlpha;
        public FadeColorEffect RangeEffect;
        private Image FormImage;
        private Particle BoostGlow;

        private Matrix rotationMatrix;

        
        public Turret(Simulator simulation)
        {
            Simulation = simulation;
            TimeLastBullet = 0;
            Type = TurretType.Unknown;
            Name = "Unknown";
            CanSell = true;
            DisabledCounter = 0;
            CanUpdateOverride = true;
            DisabledOverride = true;
            DisabledProgressBarImage = new Image("PixelBlanc", Vector3.Zero)
            {
                Size = new Vector2(36, 4),
                Color = new Color(255, 0, 220, 255),
                Origin = new Vector2()
            };
            DisabledBarImage = new Image("BarreInactivite", Vector3.Zero)
            {
                SizeX = 3f
            };
            DisabledAnnounciationCounter = float.NaN;
            Watcher = true;
            VisualPriorityBackup = Preferences.PrioriteSimulationTourelle;
            BackActiveThisTickOverride = false;
            BackActiveThisTick = false;
            Visible = true;
            Shape = Core.Physics.Shape.Circle;
            Circle = new Circle(this, 30);
            ToPlaceMode = false;
            CelestialBody = null;
            RangeAlpha = 100;
            RangeImage = new Image("CercleBlanc", Vector3.Zero)
            {
                Color = new Color(Color.R, Color.G, Color.B, RangeAlpha),
                VisualPriority = Preferences.PrioriteGUIEtoiles - 0.001f
            };
            ShowRange = false;
            CanPlace = true;
            FormImage = new Image("CercleBlanc", Vector3.Zero)
            {
                Color = new Color(Color.R, Color.G, Color.B, 100),
                VisualPriority = Preferences.PrioriteSimulationTourelle + 0.001f
            };
            ShowForm = true;
            BoostMultiplier = 0;
            Wander = true;
            PlayerControlled = false;
            UpdatePosition = true;

            BoostGlow = Simulation.Scene.Particles.Get(@"boosterTurret");
            BoostGlow.VisualPriority = this.VisualPriorityBackup + 0.006f;

            CircleEmitter emitter = (CircleEmitter) BoostGlow.ParticleEffect[0];

            emitter.Radius = this.Circle.Radius;
            emitter.ReleaseScale.Value = 50;
            emitter.ReleaseScale.Variation = 10;
            emitter.Term = this.Circle.Radius / 300f;
            emitter.ReleaseColour = this.Color.ToVector3();
        }


        public int Level
        {
            get { return ActualLevel.Value.Level; }

            set
            {
                if (value <= ActualLevel.Value.Level || value > 10)
                    return;

                CanUpdate = true;
                DisabledOverride = false;

                for (int i = value - ActualLevel.Value.Level; i > 0; i--)
                    Upgrade();

                DisabledCounter = 0;
                DisabledAnnounciationCounter = float.NaN;
                DisabledOverride = true;
            }
        }


        protected LinkedListNode<TurretLevel> ActualLevel
        {
            get { return actualLevel; }
            set
            {
                actualLevel = value;
                DisabledCounter = (float) value.Value.BuildingTime;
                DisabledAnnounciationCounter = (float) value.Value.BuildingTime;
            }
        }


        public virtual double VisualPriority
        {
            get
            {
                return this.VisualPriorityBackup;
            }

            set
            {
                this.VisualPriorityBackup = value;

                BaseImage.VisualPriority = value;
                CanonImage.VisualPriority = value + 0.00001;

                DisabledBarImage.VisualPriority = value - 0.00002;
                DisabledProgressBarImage.VisualPriority = value - 0.00003;

                FormImage.VisualPriority = BaseImage.VisualPriority + 0.00004;
            }
        }


        public void DoDie() { }


        public virtual void Update()
        {
            BackActiveThisTick = false;

            Circle.Position = this.Position;

            DisabledCounter = Math.Max(DisabledCounter - Preferences.TargetElapsedTimeMs, 0);
            DisabledAnnounciationCounter -= Preferences.TargetElapsedTimeMs;

            TimeLastBullet -= Preferences.TargetElapsedTimeMs;

            if (Type != TurretType.SlowMotion && Type != TurretType.Gravitational && Type != TurretType.Alien)
                DoWanderRotation();

            if (DisabledAnnounciationCounter < 0 && !Simulation.DemoMode)
            {
                if (!this.BackActiveThisTickOverride)
                    Audio.PlaySfx(@"Partie", @"sfxTourelleMiseAJour");

                DisabledAnnounciationCounter = float.NaN;
                BackActiveThisTick = true;
            }
        }


        private void DoWanderRotation()
        {
            if (EnemyWatched == null && Wander)
            {
                if (RotationWander > 0)
                    RotationWander = Math.Max(0, RotationWander - 0.001f);
                else if (RotationWander < 0)
                    RotationWander = Math.Min(0, RotationWander + 0.001f);
                else
                    RotationWander = Main.Random.Next(-10, 11) / 100.0f;
            }
        }


        public void Fire()
        {
            TimeLastBullet = FireRate;
        }


        public void StopFire()
        {
            TimeLastBullet = Double.MaxValue;
        }


        public List<Bullet> BulletsThisTick()
        {
            Bullets.Clear();

            if (Disabled || (EnemyWatched == null && !PlayerControlled) || Watcher)
                return Bullets;

            if (TimeLastBullet <= 0)
            {
                Vector3 direction = (PlayerControlled) ? Direction : EnemyWatched.Position - this.Position;
                direction.Normalize();
                Matrix.CreateRotationZ(MathHelper.PiOver2, out rotationMatrix);
                Vector3 directionUnitairePerpendiculaire = Vector3.Transform(direction, rotationMatrix);
                directionUnitairePerpendiculaire.Normalize();
                TurretBoostLevel boostLevel = Simulation.TurretsFactory.BoostLevels[BoostMultiplier];
                
                switch (BulletType)
                {
                    case BulletType.Base:

                        for (int i = 0; i < NbCanons; i++)
                        {
                            Vector3 translation = directionUnitairePerpendiculaire * BulletsSources[NbCanons - 1][i];

                            BasicBullet p = (BasicBullet) Simulation.BulletsFactory.Get(BulletType.Base);

                            p.Position = this.Position + translation;
                            p.Direction = direction;
                            p.AttackPoints = ActualLevel.Value.BulletHitPoints * boostLevel.BulletHitPointsMultiplier;
                            p.Speed = ActualLevel.Value.BulletSpeed * boostLevel.BulletSpeedMultiplier;
                            p.VisualPriority = this.CanonImage.VisualPriority;
                            
                            Bullets.Add(p);
                        }
                        break;

                    case BulletType.Missile:
                    case BulletType.Missile2:
                        MissileBullet pm = (MissileBullet) Simulation.BulletsFactory.Get(BulletType.Missile);

                        pm.Position = this.Position;
                        pm.Direction = EnemyWatched.Position - this.Position;
                        pm.Target = EnemyWatched;
                        pm.AttackPoints = ActualLevel.Value.BulletHitPoints * boostLevel.BulletHitPointsMultiplier;
                        pm.VisualPriority = this.CanonImage.VisualPriority;
                        pm.Speed = ActualLevel.Value.BulletSpeed * boostLevel.BulletSpeedMultiplier;
                        pm.ExplosionRange = ActualLevel.Value.BulletExplosionRange * boostLevel.BulletExplosionRangeMultiplier;
                        pm.Big = BulletType == BulletType.Missile2;

                        Bullets.Add(pm);
                        
                        break;

                    case BulletType.LaserMultiple:
                        for (int i = 0; i < NbCanons; i++)
                        {
                            MultipleLasersBullet pLM = (MultipleLasersBullet) Simulation.BulletsFactory.Get(BulletType.LaserMultiple);

                            pLM.Turret = this;
                            pLM.TargetOffset = directionUnitairePerpendiculaire * BulletsSources[NbCanons - 1][i];
                            pLM.Target = EnemyWatched;
                            pLM.Direction = EnemyWatched.Position - this.Position;
                            pLM.AttackPoints = ActualLevel.Value.BulletHitPoints * boostLevel.BulletHitPointsMultiplier;
                            pLM.VisualPriority = this.CanonImage.VisualPriority;
                            
                            Bullets.Add(pLM);
                        }
                        break;

                    case BulletType.LaserSimple:
                        LaserBullet pLS = (LaserBullet) Simulation.BulletsFactory.Get(BulletType.LaserSimple);

                        pLS.Turret = this;
                        pLS.Target = EnemyWatched;
                        pLS.Direction = EnemyWatched.Position - this.Position;
                        pLS.AttackPoints = ActualLevel.Value.BulletHitPoints * boostLevel.BulletHitPointsMultiplier;
                        pLS.VisualPriority = this.CanonImage.VisualPriority;
                        
                        ((LaserTurret)this).ActiveBullet = pLS;

                        Bullets.Add(pLS);
                        break;

                    case BulletType.Gunner:
                        GunnerBullet gb = (GunnerBullet) Simulation.BulletsFactory.Get(BulletType.Gunner);

                        gb.Turret = this;
                        gb.Target = EnemyWatched;
                        gb.Direction = EnemyWatched.Position - this.Position;
                        gb.AttackPoints = ActualLevel.Value.BulletHitPoints * boostLevel.BulletHitPointsMultiplier;
                        gb.VisualPriority = this.CanonImage.VisualPriority;
                        
                        ((GunnerTurret) this).ActiveBullet = gb;

                        Bullets.Add(gb);
                        break;

                    case BulletType.SlowMotion:
                        SlowMotionBullet pSM = (SlowMotionBullet) Simulation.BulletsFactory.Get(BulletType.SlowMotion);

                        pSM.Position = this.Position;
                        pSM.Radius = Range;
                        pSM.AttackPoints = ActualLevel.Value.BulletHitPoints * boostLevel.BulletHitPointsMultiplier;
                        pSM.VisualPriority = this.CanonImage.VisualPriority;
                        
                        Bullets.Add(pSM);
                        break;

                    case BulletType.Nanobots:
                        NanobotsBullet nb = (NanobotsBullet) Simulation.BulletsFactory.Get(BulletType.Nanobots);

                        nb.Position = this.Position;
                        nb.Direction = direction;
                        nb.AttackPoints = ActualLevel.Value.BulletHitPoints * boostLevel.BulletHitPointsMultiplier;
                        nb.Speed = ActualLevel.Value.BulletSpeed * boostLevel.BulletSpeedMultiplier;
                        nb.ExplosionRange = ActualLevel.Value.BulletExplosionRange * boostLevel.BulletExplosionRangeMultiplier;
                        nb.VisualPriority = this.CanonImage.VisualPriority;
                        
                        Bullets.Add(nb);
                        break;

                    case BulletType.RailGun:
                        RailGunBullet rgb = (RailGunBullet) Simulation.BulletsFactory.Get(BulletType.RailGun);

                        rgb.Position = this.Position;
                        rgb.Direction = direction;
                        rgb.AttackPoints = ActualLevel.Value.BulletHitPoints * boostLevel.BulletHitPointsMultiplier;
                        rgb.Speed = ActualLevel.Value.BulletSpeed * boostLevel.BulletSpeedMultiplier;
                        rgb.ExplosionRange = ActualLevel.Value.BulletExplosionRange * boostLevel.BulletExplosionRangeMultiplier;
                        rgb.VisualPriority = this.CanonImage.VisualPriority;
                        
                        Bullets.Add(rgb);
                        break;
                }

                TimeLastBullet = FireRate;
            }

            if (Bullets.Count != 0)
                Audio.PlaySfx(@"Partie", SfxShooting);

            return Bullets;
        }


        //public virtual void Show()
        //{
        //    Simulation.Scene.Add(CanonImage);
        //    Simulation.Scene.Add(BaseImage);
        //    Simulation.Scene.Add(DisabledProgressBarImage);
        //    Simulation.Scene.Add(DisabledBarImage);
        //    Simulation.Scene.Add(RangeImage);
        //    Simulation.Scene.Add(FormImage);

        //    DisabledProgressBarImage.Color.A = 0;
        //    DisabledBarImage.Color.A = 0;
        //    RangeImage.Color.A = 0;
        //    FormImage.Color.A = 0;
        //}


        //public virtual void Hide()
        //{
        //    Simulation.Scene.Remove(CanonImage);
        //    Simulation.Scene.Remove(BaseImage);
        //    Simulation.Scene.Remove(DisabledProgressBarImage);
        //    Simulation.Scene.Remove(DisabledBarImage);
        //    Simulation.Scene.Remove(RangeImage);
        //    Simulation.Scene.Remove(FormImage);
        //}


        public virtual void Draw()
        {
            CanonImage.position = this.Position;
            BaseImage.position = this.Position;

            if (EnemyWatched != null)
            {
                Vector3 direction = EnemyWatched.Position - this.Position;

                CanonImage.Rotation = MathHelper.PiOver2 + (float)Math.Atan2(direction.Y, direction.X);
            }

            else if (Wander)
                CanonImage.Rotation += RotationWander;

            else
                CanonImage.Rotation = Rotation;


            if ((Disabled || PlayerControlled && TimeLastBullet != Double.MaxValue && TimeLastBullet > 0) && !Simulation.DemoMode && !ToPlaceMode)
            {
                DisabledBarImage.Position = this.Position;

                float pourcTemps = (PlayerControlled) ?
                    (float) (1 - TimeLastBullet / FireRate) :
                    (float) (DisabledCounter / BuildingTime);

                DisabledProgressBarImage.Size = new Vector2(pourcTemps * 30, 8);
                DisabledProgressBarImage.Position = DisabledBarImage.Position - new Vector3(16, 4, 0);

                Simulation.Scene.Add(DisabledProgressBarImage);
                Simulation.Scene.Add(DisabledBarImage);
            }


            if (ShowRange)
            {
                RangeImage.Position = this.Position;
                RangeImage.SizeX = (Range / 100) * 2;

                Simulation.Scene.Add(RangeImage);
            }


            if (ShowForm)
            {
                FormImage.Position = this.Position;
                FormImage.SizeX = (Circle.Radius / 100) * 2;

                Simulation.Scene.Add(FormImage);
            }


            if (ToPlaceMode)
            {
                CanonImage.Color = (CanPlace) ? Color.White : new Color(255, 0, 0, 100);
                BaseImage.Color = (CanPlace) ? Color.White : new Color(255, 0, 0, 100);
                RangeImage.Color = (CanPlace) ? new Color(Color.R, Color.G, Color.B, RangeAlpha) : new Color(255, 0, 0, RangeAlpha);
            }


            if (BoostMultiplier > 0)
            {
                Vector3 pos = this.Position;
                BoostGlow.Trigger(ref pos);
            }


            Simulation.Scene.Add(CanonImage);
            Simulation.Scene.Add(BaseImage);
        }


        public virtual bool Upgrade()
        {
            if (!CanUpdate)
                return false;

            if (ActualLevel.Value.BaseImageName != ActualLevel.Next.Value.BaseImageName)
            {
                //if (BaseImage != null)
                //    Simulation.Scene.Remove(BaseImage);
                
                BaseImage = new Image(ActualLevel.Next.Value.BaseImageName);
                //Simulation.Scene.Add(BaseImage);
            }

            if (ActualLevel.Value.CanonImageName != ActualLevel.Next.Value.CanonImageName)
            {
                //if (CanonImage != null)
                //    Simulation.Scene.Remove(CanonImage);
                
                CanonImage = new Image(ActualLevel.Next.Value.CanonImageName);
                //Simulation.Scene.Add(CanonImage);
            }

            VisualPriority = this.VisualPriorityBackup;

            ActualLevel = ActualLevel.Next;

            CanonImage.SizeX = 3;
            BaseImage.SizeX = 3;
            CanonImage.Origin = new Vector2(6, 8);

            if (!Simulation.DemoMode && !Simulation.WorldMode && ActualLevel.Value.Level != 1)
            {
                RangeEffect = (FadeColorEffect) Simulation.Scene.VisualEffects.Add(RangeImage, VisualEffects.FadeOutTo0(RangeImage.Alpha, 300, 500), UpgradeFadeCompleted);
                ShowRange = true;
            }

            return true;
        }


        private void UpgradeFadeCompleted()
        {
            RangeEffect = null;
            RangeImage.Alpha = RangeAlpha;
            ShowRange = false;
        }


        private static List<List<int>> BulletsSources = new List<List<int>>()
        {
            new List<int>(new int[] { 0 }),
            new List<int>(new int[] { -7, 7 }),
            new List<int>(new int[] { -7, 0, 7 }),
            new List<int>(new int[] { -14, -7, 7, 14 }),
            new List<int>(new int[] { -14, -7, 0, 7, 14 })
        };


        //useless
        #region IObjetPhysique Members
        public float Speed { get; set; }
        public Vector3 Direction { get; set; }
        public float Rotation { get; set; }
        public RectanglePhysique Rectangle { get; set; }
        public Line Line { get; set; }
        #endregion
    }
}
