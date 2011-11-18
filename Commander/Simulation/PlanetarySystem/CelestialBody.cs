namespace EphemereGames.Commander.Simulation
{
    using System;
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    public abstract class CelestialBody : ILivingObject, IDestroyable, IComparable<CelestialBody>
    {
        public string Name;
        public CelestialBodyDescriptor Descriptor;
        internal virtual Simulator Simulator { get; set; }

        private double visualPriority;
        public virtual double VisualPriority                    { get { return visualPriority; } set { visualPriority = value; } }

        public int PathPriority;

        public Vector3 position;
        public Vector3 Position                                 { get { return position; } set { LastPosition = position; position = value; } }
        public Vector3 LastPosition;
        
        public virtual float Rotation                           { get; set; }
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
        public bool StraightLine;
        public bool StayOnPathUponDeath;

        public float ZoneImpactDestruction;
        internal SimPlayer PlayerCheckedIn;
        public bool SlowDeath;
        public bool SilentDeath;

        public virtual byte Alpha { get; set; }
        
        private Particle DieEffect1;
        private Particle DieEffect2;

        internal CelestialBodySteeringBehavior SteeringBehavior;
        internal CelestialBodyTurretsController TurretsController;

        public float Speed { get { return SteeringBehavior.Speed; } set { SteeringBehavior.Speed = value; } }

        //editor
        public bool canSelect;


        public CelestialBody(string name, double visualPriority)
        {
            Name = name;
            this.visualPriority = visualPriority;

            LifePoints = float.MaxValue;
            PathPriority = int.MinValue;
            CanSelect = true;
            CanSelectOverride = false;
            Invincible = false;
            LastOnPath = false;
            FirstOnPath = false;
            StraightLine = false;
            PlayerCheckedIn = null;
            AliveOverride = false;
            ShowPath = false;
            FollowPath = false;
            SlowDeath = false;
            SilentDeath = false;
            StayOnPathUponDeath = false;
            ZoneImpactDestruction = 0;

            Shape = Shape.Circle;
            Circle = new Circle(Position, 0);

            SteeringBehavior = new CelestialBodySteeringBehavior(this);
            TurretsController = new CelestialBodyTurretsController(this);
        }


        public virtual void Initialize()
        {
            TurretsController.Simulator = Simulator;
        }


        public Vector3 DeltaPosition
        {
            get { return Position - LastPosition; }
        }


        #region Size

        private Size size;

        public virtual Size Size
        {
            get { return size; }
            set
            {
                size = value;

                int sizeInt = (int) size;

                Circle.Radius = sizeInt;

                TurretsController.SetSize(sizeInt);
            }
        }

        #endregion


        public virtual void Update()
        {
            LastPosition = position;

            SteeringBehavior.Move();
            Simulator.Data.Battlefield.Clamp(ref position, ((int) Size) / 2);

            Circle.Position = Position;

            TurretsController.Update();
        }


        public virtual void Draw()
        {
            if (!Alive)
                return;

            TurretsController.Draw();
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


        public virtual void DoDie()
        {
            if (Invincible)
                return;

            LifePoints = Math.Min(LifePoints, 0);

            DieEffect1 = Simulator.Scene.Particles.Get(@"bouleTerreMeurt");
            DieEffect2 = Simulator.Scene.Particles.Get(SlowDeath ? @"anneauTerreMeurt2" : @"anneauTerreMeurt");

            DieEffect1.VisualPriority = VisualPriority - 0.000001;
            DieEffect2.VisualPriority = VisualPriority - 0.000001;

            DieEffect1.Trigger(ref position);
            DieEffect2.Trigger(ref position);

            Simulator.Scene.Particles.Return(DieEffect1);
            Simulator.Scene.Particles.Return(DieEffect2);
        }


        public int CompareTo(CelestialBody other)
        {
            if (PathPriority > other.PathPriority)
                return 1;

            if (PathPriority < other.PathPriority)
                return -1;

            return 0;
        }


        public abstract CelestialBodyDescriptor GenerateDescriptor();
    }
}
