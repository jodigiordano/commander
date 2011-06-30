namespace EphemereGames.Commander.Simulation
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;
    using ProjectMercury.Emitters;


    class CelestialBody : ILivingObject, IObjetPhysique, IComparable<CelestialBody>
    {
        public string Name;
        public List<Turret> Turrets = new List<Turret>();
        public Image Image;
        public int PathPriority;
        public Vector3 position;
        public Vector3 Position                                 { get { return position; } set { this.AnciennePosition = position; position = value; } }
        public float Speed                                      { get; set; }
        public float Rotation                                   { get; set; }
        public Vector3 Direction                                { get; set; }
        public Shape Shape                                      { get; set; }
        public Circle Circle                                    { get; set; }
        public Line Line                                       { get; set; }
        public PhysicalRectangle Rectangle                      { get; set; }
        public float LifePoints                                 { get; set; }
        public float AttackPoints                               { get; set; }
        public bool Alive                                       { get { return LifePoints > 0; } }
        public bool Selectionnable;
        public bool Invincible;
        public bool LastOnPath;
        public bool FirstOnPath;
        public Vector3 Offset;
        public List<Moon> Lunes;
        public bool HasGravitationalTurretBypass;
        public double VisualPriorityBackup;
        public Circle InnerTurretZone;
        public Circle OuterTurretZone;
        public bool ShowTurretsZone;
        public float ZoneImpactDestruction;
        public bool DarkSide;

        protected Simulator Simulator;
        protected Vector3 AnciennePosition;
        protected double ActualRotationTime;
        protected Vector3 PositionBase;

        private Particle DieEffect1;
        private Particle DieEffect2;
        private Particle DarkSideEffect;
        private Matrix RotationMatrix;
        private Image TurretsZoneImage;
        private Simulator simulator;
        private CelestialBodyDescriptor celestialBodyDescriptor;

        public string PartialImageName;
        private Size Size;
        private int StartingPourc;
        public Turret StartingPathTurret;

        public SimPlayer PlayerCheckedIn;


        public CelestialBody(
            Simulator simulator,
            string name,
            Vector3 startingPosition,
            Vector3 offset,
            Size size,
            float speed,
            string partialImageName,
            int startingPourc,
            double visualPriority)
        {
            Simulator = simulator;
            Name = name;
            PartialImageName = partialImageName;
            Size = size;

            LifePoints = float.MaxValue;
            PathPriority = 0;
            Selectionnable = true;
            Invincible = false;
            LastOnPath = false;
            FirstOnPath = false;
            DarkSide = false;
            VisualPriorityBackup = visualPriority;
            StartingPourc = startingPourc;

            Shape = Shape.Circle;
            Circle = new Circle(Position, 0);
            InnerTurretZone = new Circle(Position, 0);
            OuterTurretZone = new Circle(Position, 0);

            SetSize(size);
            SetImage(partialImageName);

            Offset = offset;
            Position = AnciennePosition = PositionBase = startingPosition;
            SetSpeed(speed);

            InitMoons();

            TurretsZoneImage = new Image("CercleBlanc", Vector3.Zero);
            TurretsZoneImage.Color = new Color(255, 255, 255, 100);
            TurretsZoneImage.VisualPriority = Preferences.PrioriteGUIEtoiles - 0.002f;
            ShowTurretsZone = false;

            ZoneImpactDestruction = 0;

            DarkSideEffect = Simulator.Scene.Particles.Get(@"darkSideEffect");
            DarkSideEffect.VisualPriority = VisualPriority + 0.0001f;
            ((CircleEmitter) DarkSideEffect.ParticleEffect[0]).Radius = Circle.Radius;

            PlayerCheckedIn = null;
        }


        public CelestialBody(Simulator simulator, CelestialBodyDescriptor celestialBodyDescriptor, double visualPriority) : this(
            simulator, 
            celestialBodyDescriptor.Name,
            celestialBodyDescriptor.Position,
            celestialBodyDescriptor.Offset,
            celestialBodyDescriptor.Size,
            celestialBodyDescriptor.Speed,
            celestialBodyDescriptor.Image,
            celestialBodyDescriptor.StartingPosition,
            visualPriority) { }


        public virtual double VisualPriority
        {
            get
            {
                return VisualPriorityBackup;
            }

            set
            {
                VisualPriorityBackup = Image.VisualPriority;

                if (Image != null)
                    Image.VisualPriority = value;

                for (int i = 0; i < Turrets.Count; i++)
                    Turrets[i].VisualPriority = value;
            }
        }


        public void SetSize(Size s)
        {
            Size = s;

            int size = (int) s;

            Circle.Radius = size;

            if (size <= (int) Size.Small)
            {
                InnerTurretZone.Radius = size;
                OuterTurretZone.Radius = size * 1.4f;
            }

            else if (size <= (int) Size.Normal)
            {
                InnerTurretZone.Radius = size * 0.8f;
                OuterTurretZone.Radius = size * 1.2f;
            }

            else
            {
                InnerTurretZone.Radius = size * 0.5f;
                OuterTurretZone.Radius = size * 1.0f;
            }
        }


        public void SetImage(string partialImageName)
        {
            PartialImageName = partialImageName;

            if (PartialImageName != null)
            {
                Image = new Image(GetImageName(Size, PartialImageName)) { SizeX = 6, VisualPriority = VisualPriorityBackup };
            }
        }


        public void SetSpeed(float speed)
        {
            Speed = speed;
            ActualRotationTime = Math.Max(ActualRotationTime, Speed * (StartingPourc / 100.0f));

            if (Speed != 0)
                Move();
        }


        public void AddToStartingPath()
        {
            if (StartingPathTurret != null)
                return;

            var t = Simulator.TurretsFactory.Create(TurretType.Gravitational);

            t.CanSell = false;
            t.CanUpdate = false;
            t.Level = 1;
            t.BackActiveThisTickOverride = true;
            t.Visible = false;
            t.CelestialBody = this;
            t.Position = this.Position;

            Turrets.Add(t);

            StartingPathTurret = t;
        }


        public void RemoveFromStartingPath()
        {
            if (StartingPathTurret == null)
                return;

            Turrets.Remove(StartingPathTurret);

            StartingPathTurret = null;
        }


        public bool HasGravitationalTurret
        {
            get
            {
                if (HasGravitationalTurretBypass)
                    return true;

                for (int i = 0; i < Turrets.Count; i++)
                    if (Turrets[i].Type == TurretType.Gravitational)
                        return true;

                return false;
            }
        }


        public bool HasLevel2GravitationalTurret
        {
            get
            {
                for (int i = 0; i < Turrets.Count; i++)
                    if (Turrets[i].Type == TurretType.Gravitational && Turrets[i].Level >= 2)
                        return true;

                return false;
            }
        }


        public virtual void Update()
        {
            if (Speed != 0)
            {
                ActualRotationTime = (ActualRotationTime + Preferences.TargetElapsedTimeMs) % Speed;

                Move();
            }

            Circle.Position = Position;
            InnerTurretZone.Position = Position;
            OuterTurretZone.Position = Position;

            for (int i = 0; i < Lunes.Count; i++)
                Lunes[i].Update();
        }


        public virtual void Draw()
        {
            if (this.LifePoints <= 0)
                return;

            if (Image != null)
            {
                Image.position = this.Position;
                Simulator.Scene.Add(Image);
            }

            foreach (var lune in Lunes)
                lune.Draw();

            if (ShowTurretsZone)
            {
                TurretsZoneImage.Position = OuterTurretZone.Position;
                TurretsZoneImage.SizeX = (OuterTurretZone.Radius / 100) * 2;

                Simulator.Scene.Add(TurretsZoneImage);
            }

            if (DarkSide)
            {
                DarkSideEffect.Trigger(ref position);
            }
        }


        public void DoHit(ILivingObject par)
        {
            Particle toucherTerre = Simulator.Scene.Particles.Get(@"toucherTerre");

            if (toucherTerre != null)
            {
                toucherTerre.VisualPriority = VisualPriorityBackup - 0.001f;

                toucherTerre.Trigger(ref this.position);
                Simulator.Scene.Particles.Return(toucherTerre);
            }

            if (Invincible)
                return;

            this.LifePoints -= par.AttackPoints;
        }


        public void DoDie()
        {
            if (Invincible)
                return;

            this.LifePoints = Math.Min(this.LifePoints, 0);

            DieEffect1 = Simulator.Scene.Particles.Get(@"bouleTerreMeurt");
            DieEffect2 = Simulator.Scene.Particles.Get(@"anneauTerreMeurt");

            DieEffect1.VisualPriority = VisualPriorityBackup - 0.001f;
            DieEffect2.VisualPriority = VisualPriorityBackup - 0.001f;

            DieEffect1.Trigger(ref this.position);
            DieEffect2.Trigger(ref this.position);
            Simulator.Scene.Particles.Return(DieEffect1);
            Simulator.Scene.Particles.Return(DieEffect2);
            Simulator.Scene.Particles.Return(DarkSideEffect);
        }


        public int CompareTo(CelestialBody other)
        {
            if (PathPriority > other.PathPriority)
                return 1;

            if (PathPriority < other.PathPriority)
                return -1;

            return 0;
        }


        private void InitMoons()
        {
            Lunes = new List<Moon>();
            int nbLunes = Main.Random.Next(0, 3);

            for (int i = 0; i < nbLunes; i++)
            {
                Moon lune;

                if ((Main.Random.Next(0, 2) == 0))
                    lune = new MoonMatrix(Simulator, this, 50);
                else
                    lune = new MoonPath(Simulator, this, 50);

                Lunes.Add(lune);
            }
        }


        private string GetImageName(Size size, string partialName)
        {
            return partialName + ((size == Size.Small) ? 1 : (size == Size.Normal) ? 2 : 3).ToString();
        }


        private void Move()
        {
            this.AnciennePosition = position;

            this.position.X = this.PositionBase.X * (float) Math.Cos((MathHelper.TwoPi / Speed) * ActualRotationTime);
            this.position.Y = this.PositionBase.Y * (float) Math.Sin((MathHelper.TwoPi / Speed) * ActualRotationTime);

            Vector3.Add(ref this.position, ref this.Offset, out this.position);
        }


        public static void Move(double tempsRotation, double tempsRotationActuel, ref Vector3 positionBase, ref Vector3 offset, ref Vector3 resultat)
        {
            resultat.X = positionBase.X * (float) Math.Cos((MathHelper.TwoPi / tempsRotation) * tempsRotationActuel);
            resultat.Y = positionBase.Y * (float) Math.Sin((MathHelper.TwoPi / tempsRotation) * tempsRotationActuel);
            resultat.Z = 0;

            Vector3.Add(ref resultat, ref offset, out resultat);
        }

        
        public virtual CelestialBodyDescriptor GenerateDescriptor()
        {
            return new CelestialBodyDescriptor()
            {
                CanSelect = Selectionnable,
                Image = PartialImageName,
                InBackground = false,
                Invincible = Invincible,
                Name = Name,
                Offset = Offset,
                PathPriority = PathPriority,
                Position = PositionBase,
                Speed = (int) Speed,
                Size = Size,
                HasGravitationalTurret = StartingPathTurret != null
            };
        }
    }
}
