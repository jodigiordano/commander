namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using EphemereGames.Core.Visuel;
    using EphemereGames.Core.Physique;
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
        public String Name                          { get; protected set; }
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
        public Cercle Circle                        { get; set; }
        public CorpsCeleste CelestialBody;
        public bool ToPlaceMode;
        public bool CanPlace;
        public bool ShowRange;
        public bool ShowForm;
        public int BoostMultiplier;
        public bool Wander;
        public bool PlayerControlled;
        public bool UpdatePosition;

        protected LinkedList<TurretLevel> Levels;
        protected String SfxShooting;

        protected Simulation Simulation;
        private bool DisabledOverride;
        private bool CanUpdateOverride;
        private float RotationWander = 0;
        private LinkedListNode<TurretLevel> actualLevel;
        public float DisabledCounter;
        private float DisabledAnnounciationCounter;
        private Image DisabledProgressBarImage;
        private Image DisabledBarImage;
        protected float VisualPriorityBackup;
        private List<Bullet> Bullets = new List<Bullet>();
        private Image RangeImage;
        private Image FormImage;
        private Particle BoostGlow;

        private Matrix rotationMatrix;

        
        public Turret(Simulation simulation)
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
            Shape = Core.Physique.Shape.Circle;
            Circle = new Cercle(this, 30);
            ToPlaceMode = false;
            CelestialBody = null;
            RangeImage = new Image("CercleBlanc", Vector3.Zero)
            {
                Color = new Color(Color.R, Color.G, Color.B, 100),
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


        public virtual float VisualPriority
        {
            get
            {
                return this.VisualPriorityBackup;
            }

            set
            {
                this.VisualPriorityBackup = value;

                BaseImage.VisualPriority = value - 0.0002f;
                CanonImage.VisualPriority = value - 0.0001f;

                DisabledBarImage.VisualPriority = value - 0.0003f;
                DisabledProgressBarImage.VisualPriority = value - 0.0004f;

                FormImage.VisualPriority = BaseImage.VisualPriority + 0.0005f;
            }
        }


        public void DoDie() { }


        public virtual void Update()
        {
            BackActiveThisTick = false;

            Circle.Position = this.Position;

            DisabledCounter = Math.Max(DisabledCounter - 16.66f, 0);
            DisabledAnnounciationCounter -= 16.66f;

            TimeLastBullet -= 16.66f;

            if (Type != TurretType.SlowMotion && Type != TurretType.Gravitational && Type != TurretType.Alien)
                DoWanderRotation();

            if (DisabledAnnounciationCounter < 0 && !Simulation.ModeDemo)
            {
                if (!this.BackActiveThisTickOverride)
                    EphemereGames.Core.Audio.Facade.jouerEffetSonore("Partie", "sfxTourelleMiseAJour");

                DisabledAnnounciationCounter = float.NaN;
                BackActiveThisTick = true;
            }

            if (BoostGlow == null)
            {
                BoostGlow = Simulation.Scene.Particles.Get("boosterTurret");
                BoostGlow.VisualPriority = this.VisualPriorityBackup + 0.006f;

                CircleEmitter emitter = (CircleEmitter) BoostGlow.ParticleEffect[0];

                emitter.Radius = this.Circle.Radius;
                emitter.ReleaseScale.Value = 50;
                emitter.ReleaseScale.Variation = 10;
                emitter.Term = this.Circle.Radius / 300f;
                emitter.ReleaseColour = this.Color.ToVector3();
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

                            BasicBullet p = Bullet.PoolBasicBullets.Get();

                            p.Scene = Simulation.Scene;
                            p.Position = this.Position + translation;
                            p.Direction = direction;
                            p.AttackPoints = ActualLevel.Value.BulletHitPoints * boostLevel.BulletHitPointsMultiplier;
                            p.Speed = ActualLevel.Value.BulletSpeed * boostLevel.BulletSpeedMultiplier;
                            p.PrioriteAffichage = this.CanonImage.VisualPriority;
                            p.Initialize();

                            Bullets.Add(p);
                        }
                        break;

                    case BulletType.Missile:
                        MissileBullet pm = Bullet.PoolMissileBullets.Get();
                        pm.Scene = Simulation.Scene;
                        pm.Position = this.Position;
                        pm.Direction = EnemyWatched.Position - this.Position;
                        pm.Target = EnemyWatched;
                        pm.AttackPoints = ActualLevel.Value.BulletHitPoints * boostLevel.BulletHitPointsMultiplier;
                        pm.PrioriteAffichage = this.CanonImage.VisualPriority;
                        pm.Speed = ActualLevel.Value.BulletSpeed * boostLevel.BulletSpeedMultiplier;
                        pm.ZoneImpact = ActualLevel.Value.BulletExplosionRange * boostLevel.BulletExplosionRangeMultiplier;
                        pm.Initialize();
                        pm.Image = pm.Image1;
                        Bullets.Add(pm);
                        break;

                    case BulletType.Missile2:
                        MissileBullet p2 = Bullet.PoolMissileBullets.Get();
                        p2.Scene = Simulation.Scene;
                        p2.Position = this.Position;
                        p2.Direction = EnemyWatched.Position - this.Position;
                        p2.Target = EnemyWatched;
                        p2.AttackPoints = ActualLevel.Value.BulletHitPoints * boostLevel.BulletHitPointsMultiplier;
                        p2.PrioriteAffichage = this.CanonImage.VisualPriority;
                        p2.Speed = ActualLevel.Value.BulletSpeed * boostLevel.BulletSpeedMultiplier;
                        p2.ZoneImpact = ActualLevel.Value.BulletExplosionRange * boostLevel.BulletExplosionRangeMultiplier;
                        p2.Initialize();
                        p2.Image = p2.Image2;
                        Bullets.Add(p2);
                        break;

                    case BulletType.LaserMultiple:
                        for (int i = 0; i < NbCanons; i++)
                        {
                            MultipleLasersBullet pLM = Bullet.PoolMultipleLasersBullets.Get();
                            pLM.Scene = Simulation.Scene;
                            pLM.TourelleEmettrice = this;
                            pLM.CibleOffset = directionUnitairePerpendiculaire * BulletsSources[NbCanons - 1][i];
                            pLM.Cible = EnemyWatched;
                            pLM.Direction = EnemyWatched.Position - this.Position;
                            pLM.AttackPoints = ActualLevel.Value.BulletHitPoints * boostLevel.BulletHitPointsMultiplier;
                            pLM.PrioriteAffichage = this.CanonImage.VisualPriority;
                            pLM.Initialize();

                            Bullets.Add(pLM);
                        }
                        break;

                    case BulletType.LaserSimple:
                        LaserBullet pLS = Bullet.PoolLaserBullets.Get();
                        pLS.Scene = Simulation.Scene;
                        pLS.TourelleEmettrice = this;
                        pLS.Cible = EnemyWatched;
                        pLS.Direction = EnemyWatched.Position - this.Position;
                        pLS.AttackPoints = ActualLevel.Value.BulletHitPoints * boostLevel.BulletHitPointsMultiplier;
                        pLS.PrioriteAffichage = this.CanonImage.VisualPriority;
                        pLS.Initialize();

                        ((LaserTurret)this).ActiveBullet = pLS;

                        Bullets.Add(pLS);
                        break;

                    case BulletType.Gunner:
                        GunnerBullet gb = Bullet.PoolGunnerBullets.Get();
                        gb.Scene = Simulation.Scene;
                        gb.Turret = this;
                        gb.Target = EnemyWatched;
                        gb.Direction = EnemyWatched.Position - this.Position;
                        gb.AttackPoints = ActualLevel.Value.BulletHitPoints * boostLevel.BulletHitPointsMultiplier;
                        gb.PrioriteAffichage = this.CanonImage.VisualPriority;
                        gb.Initialize();

                        ((GunnerTurret) this).ActiveBullet = gb;

                        Bullets.Add(gb);
                        break;

                    case BulletType.SlowMotion:
                        SlowMotionBullet pSM = Bullet.PoolSlowMotionBullets.Get();
                        pSM.Scene = Simulation.Scene;
                        pSM.Position = this.Position;
                        pSM.Rayon = Range;
                        pSM.AttackPoints = ActualLevel.Value.BulletHitPoints * boostLevel.BulletHitPointsMultiplier;
                        pSM.PrioriteAffichage = this.CanonImage.VisualPriority;
                        pSM.Initialize();

                        Bullets.Add(pSM);
                        break;

                    case BulletType.Nanobots:
                        NanobotsBullet nb = Bullet.PoolNanobotsBullets.Get();
                        nb.Scene = Simulation.Scene;
                        nb.Position = this.Position;
                        nb.Direction = direction;
                        nb.AttackPoints = ActualLevel.Value.BulletHitPoints * boostLevel.BulletHitPointsMultiplier;
                        nb.Speed = ActualLevel.Value.BulletSpeed * boostLevel.BulletSpeedMultiplier;
                        nb.ZoneImpact = ActualLevel.Value.BulletExplosionRange * boostLevel.BulletExplosionRangeMultiplier;
                        nb.PrioriteAffichage = this.CanonImage.VisualPriority;
                        nb.Initialize();

                        Bullets.Add(nb);
                        break;

                    case BulletType.RailGun:
                        RailGunBullet rgb = Bullet.PoolRailGunBullets.Get();
                        rgb.Scene = Simulation.Scene;
                        rgb.Position = this.Position;
                        rgb.Direction = direction;
                        rgb.AttackPoints = ActualLevel.Value.BulletHitPoints * boostLevel.BulletHitPointsMultiplier;
                        rgb.Speed = ActualLevel.Value.BulletSpeed * boostLevel.BulletSpeedMultiplier;
                        rgb.ZoneImpact = ActualLevel.Value.BulletExplosionRange * boostLevel.BulletExplosionRangeMultiplier;
                        rgb.PrioriteAffichage = this.CanonImage.VisualPriority;
                        rgb.Initialize();

                        Bullets.Add(rgb);
                        break;
                }

                TimeLastBullet = FireRate;
            }

            if (Bullets.Count != 0)
                EphemereGames.Core.Audio.Facade.jouerEffetSonore("Partie", SfxShooting);

            return Bullets;
        }


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
            {
                CanonImage.Rotation += RotationWander;
            }

            else
            {
                CanonImage.Rotation = Rotation;
            }

            Simulation.Scene.ajouterScenable(CanonImage);
            Simulation.Scene.ajouterScenable(BaseImage);


            if ((Disabled || PlayerControlled && TimeLastBullet != Double.MaxValue && TimeLastBullet > 0) && !Simulation.ModeDemo && !ToPlaceMode)
            {
                DisabledBarImage.Position = this.Position;

                float pourcTemps = (PlayerControlled) ?
                    (float) (1 - TimeLastBullet / FireRate) :
                    (float) (DisabledCounter / BuildingTime);

                DisabledProgressBarImage.Size = new Vector2(pourcTemps * 30, 8);
                DisabledProgressBarImage.Position = DisabledBarImage.Position - new Vector3(16, 4, 0);
                Simulation.Scene.ajouterScenable(DisabledProgressBarImage);
                Simulation.Scene.ajouterScenable(DisabledBarImage);
            }

            if (ShowRange)
            {
                RangeImage.Position = this.Position;
                RangeImage.Color = new Color(Color.R, Color.G, Color.B, 100);
                RangeImage.SizeX = (Range / 100) * 2;
                Simulation.Scene.ajouterScenable(RangeImage);
            }

            if (ShowForm)
            {
                FormImage.Position = this.Position;
                FormImage.SizeX = (Circle.Radius / 100) * 2;
                Simulation.Scene.ajouterScenable(FormImage);
            }

            if (ToPlaceMode)
            {
                CanonImage.Color = (CanPlace) ? Color.White : new Color(255, 0, 0, 100);
                BaseImage.Color = (CanPlace) ? Color.White : new Color(255, 0, 0, 100);
                RangeImage.Color = (CanPlace) ? new Color(Color.R, Color.G, Color.B, 100) : new Color(255, 0, 0, 100);
            }

            if (BoostMultiplier > 0)
            {
                Vector3 pos = this.Position;
                BoostGlow.Trigger(ref pos);
            }
        }


        public virtual bool Upgrade()
        {
            if (!CanUpdate)
                return false;

            if (ActualLevel.Value.BaseImageName != ActualLevel.Next.Value.BaseImageName)
            {
                BaseImage = new Image(ActualLevel.Next.Value.BaseImageName, Vector3.Zero);
            }

            if (ActualLevel.Value.CanonImageName != ActualLevel.Next.Value.CanonImageName)
            {
                CanonImage = new Image(ActualLevel.Next.Value.CanonImageName, Vector3.Zero);
            }

            VisualPriority = this.VisualPriorityBackup;

            ActualLevel = ActualLevel.Next;

            CanonImage.SizeX = 3;
            BaseImage.SizeX = 3;
            CanonImage.Origin = new Vector2(6, 8);

            return true;
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
        public Ligne Line { get; set; }
        #endregion
    }
}
