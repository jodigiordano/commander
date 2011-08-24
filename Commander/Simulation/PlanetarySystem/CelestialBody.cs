namespace EphemereGames.Commander.Simulation
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;
    using ProjectMercury.Emitters;


    class CelestialBody : ILivingObject, ICollidable, IComparable<CelestialBody>
    {
        public string Name;
        public List<Turret> Turrets = new List<Turret>();
        public Image Image;
        public int PathPriority;
        public Size Size;
        public Vector3 position;
        public Vector3 Position                                 { get { return position; } set { this.LastPosition = position; position = value; } }
        public Vector3 BasePosition;
        public Vector3 Path;
        protected Vector3 LastPosition;
        public float Speed                                      { get; set; }        
        public double ActualRotationTime;
        private int StartingPourc;
        
        public float Rotation                                   { get; set; }
        public Vector3 Direction                                { get; set; }
        public Shape Shape                                      { get; set; }
        public Circle Circle                                    { get; set; }
        public Line Line                                        { get; set; }
        public PhysicalRectangle Rectangle                      { get; set; }
        public float LifePoints                                 { get; set; }
        public float AttackPoints                               { get; set; }
        public bool AliveOverride;
        public bool Alive                                       { get { return AliveOverride || LifePoints > 0; } }
        public bool CanSelect                                   { get { return CanSelectOverride || canSelect; } set { canSelect = value; } }
        public bool CanSelectOverride;
        public bool Invincible;
        public bool LastOnPath;
        public bool FirstOnPath;
        public bool ShowPath;
        public bool FollowPath;
        public bool HasMoons;
        public bool StraightLine;
        public List<Moon> Moons;
        public bool FakeHasGravitationalTurret;
        public Circle InnerTurretZone;
        public Circle OuterTurretZone;
        public bool ShowTurretsZone;
        public float ZoneImpactDestruction;
        public bool DarkSide;
        public string PartialImageName;
        public bool canSelect; //only for editor
        public Turret StartingPathTurret;
        public SimPlayer PlayerCheckedIn;
        public bool FakeHasGravitationalTurretLv2;

        protected Simulator Simulator;
        protected double VisualPriorityBackup;
        
        private Particle DieEffect1;
        private Particle DieEffect2;
        private Particle DarkSideEffect;
        private float PathRotation;
        protected Matrix RotationMatrix;
        private Image TurretsZoneImage;


        public CelestialBody(
            Simulator simulator,
            string name,
            Vector3 path,
            Vector3 basePosition,
            float rotationRad,
            Size size,
            float speed,
            string partialImageName,
            int startingPourc,
            double visualPriority,
            bool hasMoons)
        {
            Simulator = simulator;
            Name = name;
            PartialImageName = partialImageName;
            Size = size;

            LifePoints = float.MaxValue;
            PathPriority = int.MinValue;
            CanSelect = true;
            CanSelectOverride = false;
            Invincible = false;
            LastOnPath = false;
            FirstOnPath = false;
            StraightLine = false;
            DarkSide = false;
            VisualPriorityBackup = visualPriority;
            StartingPourc = startingPourc;
            PathRotation = 0;

            Shape = Shape.Circle;
            Circle = new Circle(Position, 0);
            InnerTurretZone = new Circle(Position, 0);
            OuterTurretZone = new Circle(Position, 0);

            SetSize(size);
            SetImage(partialImageName);
            SetRotation(rotationRad);

            BasePosition = basePosition;
            Position = LastPosition = Path = path;
            
            Speed = float.MaxValue;
            SetSpeed(speed);

            InitMoons();

            TurretsZoneImage = new Image("CercleBlanc", Vector3.Zero);
            TurretsZoneImage.Color = new Color(255, 255, 255, 100);
            TurretsZoneImage.VisualPriority = VisualPriorities.Default.TurretRange;
            ShowTurretsZone = false;

            ZoneImpactDestruction = 0;

            DarkSideEffect = Simulator.Scene.Particles.Get(@"darkSideEffect");
            DarkSideEffect.VisualPriority = VisualPriority + 0.000001;
            ((CircleEmitter) DarkSideEffect.ParticleEffect[0]).Radius = Circle.Radius;

            PlayerCheckedIn = null;
            AliveOverride = false;
            ShowPath = false;
            FollowPath = false;
            HasMoons = true;
            FakeHasGravitationalTurretLv2 = false;
        }


        public CelestialBody(Simulator simulator, CelestialBodyDescriptor celestialBodyDescriptor, double visualPriority) : this(
            simulator, 
            celestialBodyDescriptor.Name,
            celestialBodyDescriptor.Path,
            celestialBodyDescriptor.Position,
            celestialBodyDescriptor.Rotation,
            celestialBodyDescriptor.Size,
            celestialBodyDescriptor.Speed,
            celestialBodyDescriptor.Image,
            celestialBodyDescriptor.StartingPosition,
            visualPriority,
            celestialBodyDescriptor.HasMoons) { }


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

                if (DarkSideEffect != null)
                    DarkSideEffect.VisualPriority = value + 0.000001;
            }
        }


        public Vector3 DeltaPosition
        {
            get { return Position - LastPosition; }
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
                InnerTurretZone.Radius = size * 0.9f;
                OuterTurretZone.Radius = size * 1.2f;
            }

            else
            {
                InnerTurretZone.Radius = size * 0.9f;
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
            double actualPourc = (Speed == float.MaxValue) ? 0 : ActualRotationTime / Speed;

            Speed = speed;
            ActualRotationTime = Speed * actualPourc;

            Move();
        }


        public void SetRotation(float rotationRad)
        {
            PathRotation = rotationRad;

            Matrix.CreateRotationZ(PathRotation, out RotationMatrix);
        }


        public void SetHasMoons(bool value)
        {
            HasMoons = value;

            InitMoons();
        }


        public void AddToStartingPath(bool visible)
        {
            if (StartingPathTurret != null)
                return;

            var t = Simulator.TurretsFactory.Create(TurretType.Gravitational);

            t.CanSell = visible;
            t.CanUpdate = visible;
            t.Level = 1;
            t.BackActiveThisTickOverride = true;
            t.Visible = visible;
            t.CelestialBody = this;
            t.RelativePosition = Circle.NearestPointToCircumference(
                new Vector3(Main.Random.Next((int) -Circle.Radius * 2, (int) Circle.Radius * 2), Main.Random.Next((int) -Circle.Radius * 2, (int) Circle.Radius * 2), 0));

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


        public Turret GravitationalTurret
        {
            get
            {
                for (int i = 0; i < Turrets.Count; i++)
                    if (Turrets[i].Type == TurretType.Gravitational)
                        return Turrets[i];

                return null;
            }
        }


        public virtual void Update()
        {
            if (Speed != float.MaxValue)
                ActualRotationTime = (ActualRotationTime + Preferences.TargetElapsedTimeMs) % Speed;

            Move();

            Circle.Position = Position;
            InnerTurretZone.Position = Position;
            OuterTurretZone.Position = Position;

            for (int i = 0; i < Moons.Count; i++)
                Moons[i].Update();
        }


        public virtual void Draw()
        {
            if (!Alive)
                return;

            if (Image != null)
            {
                Image.position = this.Position;
                Simulator.Scene.Add(Image);

                if (FollowPath)
                {
                    Vector3 diff = Position - LastPosition;

                    Image.Rotation = Core.Physics.Utilities.VectorToAngle(ref diff);
                }
            }

            foreach (var moon in Moons)
                moon.Draw();

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
            Particle hitEffect = Simulator.Scene.Particles.Get(@"toucherTerre");

            if (hitEffect != null)
            {
                hitEffect.VisualPriority = VisualPriority - 0.000001;

                hitEffect.Trigger(ref position);
                Simulator.Scene.Particles.Return(hitEffect);
            }

            if (Invincible)
                return;

            LifePoints -= par.AttackPoints;
        }


        public void DoDie()
        {
            if (Invincible)
                return;

            LifePoints = Math.Min(LifePoints, 0);

            DieEffect1 = Simulator.Scene.Particles.Get(@"bouleTerreMeurt");
            DieEffect2 = Simulator.Scene.Particles.Get(Simulator.CutsceneMode ? @"anneauTerreMeurt2" : @"anneauTerreMeurt");

            DieEffect1.VisualPriority = VisualPriority - 0.000001;
            DieEffect2.VisualPriority = VisualPriority - 0.000001;

            DieEffect1.Trigger(ref position);
            DieEffect2.Trigger(ref position);

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


        public Vector3 GetPositionAtPerc(float perc)
        {
            Vector3 result = new Vector3();

            Move(1, MathHelper.Clamp(perc, 0, 1), ref Path, ref BasePosition, ref RotationMatrix, ref result);

            return result;
        }


        public static void Move(double rotationTime, double actualRotationTime, ref Vector3 basePosition, ref Vector3 offset, ref Matrix rotationMatrix, ref Vector3 result)
        {
            if (rotationTime == float.MaxValue)
            {
                result.X = basePosition.X * (float) Math.Cos(0);
                result.Y = basePosition.Y * (float) Math.Sin(0);
                result.Z = 0;
            }

            else
            {
                result.X = basePosition.X * (float) Math.Cos((MathHelper.TwoPi / rotationTime) * actualRotationTime);
                result.Y = basePosition.Y * (float) Math.Sin((MathHelper.TwoPi / rotationTime) * actualRotationTime);
                result.Z = 0;
            }

            Vector3.Transform(ref result, ref rotationMatrix, out result);
            Vector3.Add(ref result, ref offset, out result);
        }


        public virtual CelestialBodyDescriptor GenerateDescriptor()
        {
            return new CelestialBodyDescriptor()
            {
                CanSelect = canSelect,
                Image = PartialImageName,
                InBackground = false,
                Invincible = Invincible,
                Name = Name,
                Position = BasePosition,
                PathPriority = PathPriority,
                Path = Path,
                Speed = (int) Speed,
                Size = Size,
                Rotation = PathRotation,
                HasGravitationalTurret = StartingPathTurret != null,
                HasMoons = HasMoons,
                FollowPath = FollowPath,
                StraightLine = StraightLine
            };
        }


        public void Move()
        {
            LastPosition = position;

            Move(Speed, ActualRotationTime, ref Path, ref BasePosition, ref RotationMatrix, ref position);
        }


        private void InitMoons()
        {
            Moons = new List<Moon>();
            int nbLunes = HasMoons ? Main.Random.Next(0, 3) : 0;

            for (int i = 0; i < nbLunes; i++)
            {
                Moon lune;

                if ((Main.Random.Next(0, 2) == 0))
                    lune = new MoonMatrix(Simulator, this, 50);
                else
                    lune = new MoonPath(Simulator, this, 50);

                Moons.Add(lune);
            }
        }


        private string GetImageName(Size size, string partialName)
        {
            return partialName + ((size == Size.Small) ? 1 : (size == Size.Normal) ? 2 : 3).ToString();
        }
    }
}
