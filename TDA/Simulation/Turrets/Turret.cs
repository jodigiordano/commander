namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using EphemereGames.Core.Visuel;
    using EphemereGames.Core.Physique;


    abstract class Turret : IObjetPhysique
    {
        public Vector3 RelativePosition;
        public Vector3 Position                     { get; set; }
        public Image CanonImage;
        public Image BaseImage;
        public bool Disabled                        { get { return DisabledOverride && DisabledCounter > 0; } set { DisabledOverride = value; } }
        public virtual Ennemi EnemyWatched          { get; set; }
        public double TimeLastBullet;
        public TurretType Type                      { get; protected set; }
        public String Name                          { get; protected set; }
        public bool CanSell;
        public bool CanUpdate                       { get { return CanUpdateOverride && (!Disabled && !ActualLevel.Equals(Levels.Last)); } set { CanUpdateOverride = value; } }
        public int BuyPrice                         { get { return ActualLevel.Value.BuyPrice; } }
        public int UpdatePrice                      { get { return (ActualLevel.Equals(Levels.Last)) ? ActualLevel.Value.BuyPrice : ActualLevel.Next.Value.BuyPrice; } }
        public int SellPrice                        { get { return ActualLevel.Value.SellPrice; } }
        public Cercle Range                         { get { return ActualLevel.Value.Range; } }
        public double ShootingFrequency             { get { return ActualLevel.Value.ShootingFrequency; } }
        public int NbCanons                         { get { return ActualLevel.Value.NbCanons; } }
        public double BuildingTime                  { get { return ActualLevel.Value.BuildingTime; } }
        public BulletType Bullet                    { get { return ActualLevel.Value.Bullet; } }
        public bool Watcher;
        public Color Color;
        public bool BackActiveThisTick              { get; private set; }
        public bool BackActiveThisTickOverride;
        public bool Visible;
        public Forme Forme                          { get; set; }
        public Cercle Cercle                        { get; set; }
        public CorpsCeleste CelestialBody;
        public bool ToPlaceMode;
        public bool CanPlace;
        public bool ShowRange;
        public bool ShowForm;

        protected LinkedList<TurretLevel> Levels;
        protected String SfxShooting;

        protected Simulation Simulation;
        private bool DisabledOverride;
        private bool CanUpdateOverride;
        private float RotationWander = 0;
        private LinkedListNode<TurretLevel> actualLevel;
        private float DisabledCounter;
        private float DisabledAnnounciationCounter;
        private Image DisabledProgressBarImage;
        private Image DisabledBarImage;
        protected float VisualPriorityBackup;
        private List<Projectile> Bullets = new List<Projectile>();
        private Image RangeImage;
        private Image FormImage;

        
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
            Forme = Core.Physique.Forme.Cercle;
            Cercle = new Cercle(this, 30);
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
            set
            {
                this.VisualPriorityBackup = value;

                BaseImage.VisualPriority = value - 0.002f;
                CanonImage.VisualPriority = value - 0.001f;

                DisabledBarImage.VisualPriority = value - 0.003f;
                DisabledProgressBarImage.VisualPriority = value - 0.004f;

                FormImage.VisualPriority = BaseImage.VisualPriority + 0.005f;
            }
        }


        public void DoDie() { }


        public virtual void Update(GameTime gameTime)
        {
            BackActiveThisTick = false;

            Range.Position = this.Position;
            Cercle.Position = this.Position;

            DisabledCounter = MathHelper.Clamp(DisabledCounter - (float) gameTime.ElapsedGameTime.TotalMilliseconds, 0, float.MaxValue);
            DisabledAnnounciationCounter -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            TimeLastBullet -= gameTime.ElapsedGameTime.TotalMilliseconds;

            if (Type != TurretType.SlowMotion && Type != TurretType.Gravitational && Type != TurretType.Alien)
                DoWanderRotation(gameTime);

            if (DisabledAnnounciationCounter < 0 && !Simulation.ModeDemo)
            {
                if (!this.BackActiveThisTickOverride)
                    EphemereGames.Core.Audio.Facade.jouerEffetSonore("Partie", "sfxTourelleMiseAJour");

                DisabledAnnounciationCounter = float.NaN;
                BackActiveThisTick = true;
            }
        }


        private void DoWanderRotation(GameTime gameTime)
        {
            if (EnemyWatched == null)
            {
                if (RotationWander > 0)
                    RotationWander = Math.Max(0, RotationWander - 0.001f);
                else if (RotationWander < 0)
                    RotationWander = Math.Min(0, RotationWander + 0.001f);
                else
                    RotationWander = Main.Random.Next(-10, 11) / 100.0f;
            }
        }


        public List<Projectile> BulletsThisTick(GameTime gameTime)
        {
            Bullets.Clear();

            if (Disabled || EnemyWatched == null || Watcher)
                return Bullets;

            if (TimeLastBullet <= 0)
            {
                Vector3 direction = EnemyWatched.Position - this.Position;
                Matrix matriceRotation = Matrix.CreateRotationZ(MathHelper.PiOver2);
                Vector3 directionUnitairePerpendiculaire = Vector3.Transform(direction, matriceRotation);
                directionUnitairePerpendiculaire.Normalize();
                
                switch (Bullet)
                {
                    case BulletType.Base:

                        for (int i = 0; i < NbCanons; i++)
                        {
                            Vector3 translation = directionUnitairePerpendiculaire * BulletsSources[NbCanons - 1][i];

                            ProjectileBase p = Projectile.PoolProjectilesBase.recuperer();

                            p.Scene = Simulation.Scene;
                            p.Position = this.Position + translation;
                            p.Direction = direction;
                            p.PointsAttaque = ActualLevel.Value.BulletHitPoints;
                            p.PrioriteAffichage = this.CanonImage.VisualPriority;
                            p.Initialize();

                            Bullets.Add(p);
                        }
                        break;

                    case BulletType.Missile:
                        ProjectileMissile pm = Projectile.PoolProjectilesMissile.recuperer();
                        pm.Scene = Simulation.Scene;
                        pm.Position = this.Position;
                        pm.Direction = EnemyWatched.Position - this.Position;
                        pm.Cible = EnemyWatched;
                        pm.PointsAttaque = ActualLevel.Value.BulletHitPoints;
                        pm.PrioriteAffichage = this.CanonImage.VisualPriority;
                        pm.Vitesse = ActualLevel.Value.BulletSpeed;
                        pm.ZoneImpact = ActualLevel.Value.BulletExplosionRange;
                        pm.Initialize();
                        pm.RepresentationVivant.Texture = EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>("ProjectileMissile1");
                        pm.RepresentationVivant.Taille = 1;
                        pm.Rectangle.Width = pm.RepresentationVivant.Rectangle.Width;
                        pm.Rectangle.Height = pm.RepresentationVivant.Rectangle.Height;
                        Bullets.Add(pm);
                        break;

                    case BulletType.Missile2:
                        ProjectileMissile p2 = Projectile.PoolProjectilesMissile.recuperer();
                        p2.Scene = Simulation.Scene;
                        p2.Position = this.Position;
                        p2.Direction = EnemyWatched.Position - this.Position;
                        p2.Cible = EnemyWatched;
                        p2.PointsAttaque = ActualLevel.Value.BulletHitPoints;
                        p2.PrioriteAffichage = this.CanonImage.VisualPriority;
                        p2.Vitesse = ActualLevel.Value.BulletSpeed;
                        p2.ZoneImpact = ActualLevel.Value.BulletExplosionRange;
                        p2.Initialize();
                        p2.RepresentationVivant.Texture = EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>("ProjectileMissile2");
                        p2.RepresentationVivant.Taille = 2;
                        p2.Rectangle.Width = p2.RepresentationVivant.Rectangle.Width;
                        p2.Rectangle.Height = p2.RepresentationVivant.Rectangle.Height;
                        Bullets.Add(p2);
                        break;

                    case BulletType.LaserMultiple:
                        for (int i = 0; i < NbCanons; i++)
                        {
                            ProjectileLaserMultiple pLM = Projectile.PoolProjectilesLaserMultiple.recuperer();
                            pLM.Scene = Simulation.Scene;
                            pLM.TourelleEmettrice = this;
                            pLM.CibleOffset = directionUnitairePerpendiculaire * BulletsSources[NbCanons - 1][i];
                            pLM.Cible = EnemyWatched;
                            pLM.Direction = EnemyWatched.Position - this.Position;
                            pLM.PointsAttaque = ActualLevel.Value.BulletHitPoints;
                            pLM.PrioriteAffichage = this.CanonImage.VisualPriority;
                            pLM.Initialize();

                            Bullets.Add(pLM);
                        }
                        break;

                    case BulletType.LaserSimple:
                        ProjectileLaserSimple pLS = Projectile.PoolProjectilesLaserSimple.recuperer();
                        pLS.Scene = Simulation.Scene;
                        pLS.TourelleEmettrice = this;
                        pLS.Cible = EnemyWatched;
                        pLS.Direction = EnemyWatched.Position - this.Position;
                        pLS.PointsAttaque = ActualLevel.Value.BulletHitPoints;
                        pLS.PrioriteAffichage = this.CanonImage.VisualPriority;
                        pLS.Initialize();

                        ((LaserTurret)this).ActiveBullet = pLS;

                        Bullets.Add(pLS);
                        break;

                    case BulletType.SlowMotion:
                        ProjectileSlowMotion pSM = Projectile.PoolProjectilesSlowMotion.recuperer();
                        pSM.Scene = Simulation.Scene;
                        pSM.Position = this.Position;
                        pSM.Rayon = ActualLevel.Value.Range.Radius;
                        pSM.PointsAttaque = ActualLevel.Value.BulletHitPoints;
                        pSM.PrioriteAffichage = this.CanonImage.VisualPriority;
                        pSM.Initialize();

                        Bullets.Add(pSM);
                        break;
                }

                TimeLastBullet = ShootingFrequency;
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

            else
                CanonImage.Rotation += RotationWander;

            Simulation.Scene.ajouterScenable(CanonImage);
            Simulation.Scene.ajouterScenable(BaseImage);


            if (Disabled && !Simulation.ModeDemo && !ToPlaceMode)
            {
                DisabledBarImage.Position = this.Position;

                float pourcTemps = (float)(DisabledCounter / ActualLevel.Value.BuildingTime);

                DisabledProgressBarImage.Size = new Vector2(pourcTemps * 30, 8);
                DisabledProgressBarImage.Position = DisabledBarImage.Position - new Vector3(16, 4, 0);
                Simulation.Scene.ajouterScenable(DisabledProgressBarImage);
                Simulation.Scene.ajouterScenable(DisabledBarImage);
            }

            if (ShowRange)
            {
                RangeImage.Position = this.Position;
                RangeImage.Color = new Color(Color.R, Color.G, Color.B, 100);
                RangeImage.SizeX = (Range.Radius / 100) * 2;
                Simulation.Scene.ajouterScenable(RangeImage);
            }

            if (ShowForm)
            {
                FormImage.Position = this.Position;
                FormImage.SizeX = (Cercle.Radius / 100) * 2;
                Simulation.Scene.ajouterScenable(FormImage);
            }

            if (ToPlaceMode)
            {
                CanonImage.Color = (CanPlace) ? Color.White : new Color(255, 0, 0, 100);
                BaseImage.Color = (CanPlace) ? Color.White : new Color(255, 0, 0, 100);
                RangeImage.Color = (CanPlace) ? new Color(Color.R, Color.G, Color.B, 100) : new Color(255, 0, 0, 100);
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
        public float Vitesse { get; set; }
        public Vector3 Direction { get; set; }
        public float Rotation { get; set; }
        public RectanglePhysique Rectangle { get; set; }
        public Ligne Ligne { get; set; }
        #endregion
    }
}
