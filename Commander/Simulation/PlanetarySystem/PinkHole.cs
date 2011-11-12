namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;
    using ProjectMercury.Emitters;
    using ProjectMercury.Modifiers;


    public class PinkHole : CelestialBody
    {
        private Particle Effect;


        public PinkHole(string name, double visualPriority)
            : base(name, visualPriority)
        {

        }


        public override void Initialize()
        {
            base.Initialize();

            Effect = Simulator.Scene.Particles.Get("trouRose");
            Effect.VisualPriority = VisualPriority;
            Radius = (int) Size;
        }


        public Color Color
        {
            set
            {
                ((ColourInterpolatorModifier) Effect.Model[0].Modifiers[2]).MiddleColour = value.ToVector3();
            }
        }


        public BlendType BlendType
        {
            set
            {
                Effect.Blend = value;
            }
        }


        public override double VisualPriority
        {
            get { return base.VisualPriority; }

            set
            {
                base.VisualPriority = value;

                Effect.VisualPriority = value;
            }
        }


        public float Radius
        {
            get
            {
                return ((CircleEmitter) Effect.Model[0]).Radius;
            }

            set
            {
                if (Effect != null)
                    ((CircleEmitter) Effect.Model[0]).Radius = value;
            }
        }


        public override Size Size
        {
            get { return base.Size; }
            set
            {
                base.Size = value;

                Radius = (int) Size;
            }
        }


        public override void Update()
        {
            ((RadialGravityModifier) Effect.Model[0].Modifiers[0]).Position = new Vector2(this.position.X, this.position.Y);

            Vector3 displacement;
            Vector3.Subtract(ref this.position, ref this.LastPosition, out displacement);

            if (displacement != Vector3.Zero)
                Effect.Move(ref displacement);

            Effect.Trigger(ref position);

            base.Update();
        }


        public override void Draw()
        {

        }


        public override CelestialBodyDescriptor GenerateDescriptor()
        {
            var descriptor = new PinkHoleCBDescriptor()
            {
                CanSelect = canSelect,
                Invincible = Invincible,
                Name = Name,
                PathPriority = PathPriority,
                Speed = (int) Speed,
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
    }
}
