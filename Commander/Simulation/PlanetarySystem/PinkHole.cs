namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;
    using ProjectMercury.Emitters;
    using ProjectMercury.Modifiers;


    class PinkHole : CelestialBody
    {
        private Particle Effect;


        public PinkHole(
            Simulator simulator,
            string name,
            Vector3 path,
            Vector3 position,
            Size size,
            float speed,
            Particle effect,
            int startingPourc,
            double visualPriority)
            : base (simulator, name, path, position, 0, size, speed, null, startingPourc, visualPriority, false)
        {
            Effect = effect;
            Effect.VisualPriority = visualPriority;
        }


        public PinkHole(Simulator simulator, CelestialBodyDescriptor celestialBodyDescriptor, double visualPriority)
            : base(
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
            celestialBodyDescriptor.HasMoons)
        {
            Effect = simulator.Scene.Particles.Get(celestialBodyDescriptor.ParticleEffect);
            Effect.VisualPriority = visualPriority;
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
            get
            {
                return base.VisualPriority;
            }

            set
            {
                VisualPriorityBackup = Effect.VisualPriority;

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
                ((CircleEmitter) Effect.Model[0]).Radius = value;
            }
        }


        public override void Update()
        {
            this.LastPosition = this.Position;
            ((RadialGravityModifier) Effect.Model[0].Modifiers[0]).Position = new Vector2(this.position.X, this.position.Y);

            Vector3 deplacement;
            Vector3.Subtract(ref this.position, ref this.LastPosition, out deplacement);
            Effect.Move(ref deplacement);
            Effect.Trigger(ref position);

            base.Update();
        }


        public override void Draw()
        {

        }


        public override CelestialBodyDescriptor GenerateDescriptor()
        {
            var d = base.GenerateDescriptor();

            d.ParticleEffect = Effect.Name;
            d.HasMoons = false;

            return d;
        }
    }
}
