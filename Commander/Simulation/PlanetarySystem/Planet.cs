namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;
    using ProjectMercury.Emitters;


    public class Planet : CelestialBody
    {
        public Image Image;
        public bool IsALevel;
        public bool DarkSide;

        private Particle DarkSideEffect;
        private List<Moon> Moons;


        public Planet(string name, string imageName, double visualPriority)
            : base(name, visualPriority)
        {
            IsALevel = true;

            Moons = new List<Moon>();

            HasMoons = true;
            ImageName = imageName;
        }


        internal override Simulator Simulator
        {
            get { return base.Simulator; }
            set
            {
                base.Simulator = value;

                foreach (var m in Moons)
                    m.Simulator = value;
            }
        }


        public override void Initialize()
        {
            base.Initialize();

            DarkSideEffect = Simulator.Scene.Particles.Get(@"darkSideEffect");
            DarkSideEffect.VisualPriority = VisualPriority + 0.000001;
            ((CircleEmitter) DarkSideEffect.Model[0]).Radius = Circle.Radius;
        }


        public override CelestialBodyDescriptor GenerateDescriptor()
        {
            var descriptor = new PlanetCBDescriptor()
            {
                CanSelect = canSelect,
                Image = ImageName,
                HasMoons = HasMoons,
                IsALevel = IsALevel,
                Invincible = Invincible,
                Name = Name,
                PathPriority = PathPriority,
                Speed = (int) SteeringBehavior.Speed,
                Size = Size,
                FollowPath = FollowPath,
                StraightLine = StraightLine,
            };

            descriptor.Position = SteeringBehavior.BasePosition;
            descriptor.Path = SteeringBehavior.Path;
            descriptor.Rotation = SteeringBehavior.PathRotation;
            descriptor.HasGravitationalTurret = TurretsController.StartingPathTurret != null;

            return descriptor;
        }


        public override double VisualPriority
        {
            get { return base.VisualPriority; }

            set
            {
                base.VisualPriority = value;

                Image.VisualPriority = value;

                if (DarkSideEffect != null)
                    DarkSideEffect.VisualPriority = value + 0.000001;
            }
        }


        public override void Update()
        {
            base.Update();

            foreach (var m in Moons)
                m.Update();
        }


        public override byte Alpha
        {
            get { return base.Alpha; }
            set
            {
                base.Alpha = value;

                Image.Alpha = value;
            }
        }


        public override Size Size
        {
            get { return base.Size; }
            set
            {
                base.Size = value;

                ImageName = ImageName;
            }
        }


        public override void Draw()
        {
            if (!Alive)
                return;

            base.Draw();

            Image.position = Position;
            Simulator.Scene.Add(Image);

            if (FollowPath)
            {
                Vector3 diff = Position - LastPosition;

                Image.Rotation = Core.Physics.Utilities.VectorToAngle(ref diff);
            }

            foreach (var m in Moons)
                m.Draw();

            if (DarkSide)
            {
                DarkSideEffect.Trigger(ref position);
            }
        }


        public override float Rotation
        {
            get { return base.Rotation; }
            set
            {
                base.Rotation = value;

                Image.Rotation = value;
            }
        }


        public override void DoDie()
        {
            base.DoDie();

            Simulator.Scene.Particles.Return(DarkSideEffect);
        }


        #region Moons

        private bool hasMoons;

        public bool HasMoons
        {
            get { return hasMoons; }

            set
            {
                hasMoons = value;

                Moons.Clear();
                int moonsCount = HasMoons ? Main.Random.Next(0, 3) : 0;

                for (int i = 0; i < moonsCount; i++)
                {
                    Moon m;

                    if ((Main.Random.Next(0, 2) == 0))
                        m = new MoonMatrix(this, 50);
                    else
                        m = new MoonPath(this, 50);

                    m.Simulator = Simulator;

                    Moons.Add(m);
                }
            }
        }

        #endregion


        #region Image

        private string imageName;


        public string ImageName
        {
            get { return imageName; }
            set
            {
                imageName = value;

                Image = new Image(GetFullImageName(Size, imageName)) { SizeX = 6, VisualPriority = VisualPriority };
            }
        }


        private string GetFullImageName(Size size, string partialName)
        {
            return partialName + ((size == Size.Small) ? 1 : (size == Size.Normal) ? 2 : 3).ToString();
        }

        #endregion
    }
}
