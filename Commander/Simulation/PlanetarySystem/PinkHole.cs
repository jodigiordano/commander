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


        public Color Color
        {
            set
            {
                ((ColourInterpolatorModifier) Effect.ParticleEffect[0].Modifiers[2]).MiddleColour = value.ToVector3();
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
                return ((CircleEmitter) Effect.ParticleEffect[0]).Radius;
            }

            set
            {
                ((CircleEmitter) Effect.ParticleEffect[0]).Radius = value;
            }
        }


        public override void Update()
        {
            this.LastPosition = this.Position;
            ((RadialGravityModifier) Effect.ParticleEffect[0].Modifiers[0]).Position = new Vector2(this.position.X, this.position.Y);

            Vector3 deplacement;
            Vector3.Subtract(ref this.position, ref this.LastPosition, out deplacement);
            Effect.Move(ref deplacement);
            Effect.Trigger(ref position);

            base.Update();
        }


        public override void Draw()
        {

        }
    }
}
