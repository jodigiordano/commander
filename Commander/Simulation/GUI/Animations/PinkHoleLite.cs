namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;
    using ProjectMercury.Emitters;
    using ProjectMercury.Modifiers;


    class PinkHoleLite
    {
        public Vector3 Position { get { return position; } set { LastPosition = position;  position = value; } }


        private Scene Scene;
        private Vector3 LastPosition;
        private Particle Effect;
        private double visualPriority;
        private Vector3 position;


        public PinkHoleLite(
            Scene scene,
            Vector3 position,
            Particle effect,
            double visualPriority)
        {
            Effect = effect;
            Scene = scene;
            Position = position;
            VisualPriority = visualPriority;
        }


        public Color Color
        {
            get
            {
                var color = ((ColourInterpolatorModifier) Effect.ParticleEffect[0].Modifiers[2]).MiddleColour;

                return new Color(color);
            }

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


        public double VisualPriority
        {
            get
            {
                return VisualPriority;
            }

            set
            {
                visualPriority = value;

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


        public float ParticleVelocity
        {
            set
            {
                ((VelocityClampModifier) Effect.ParticleEffect[0].Modifiers[1]).MaximumVelocity = value;
            }
        }


        public float ParticleGravityStrength
        {
            set
            {
                ((RadialGravityModifier) Effect.ParticleEffect[0].Modifiers[0]).Strength = value;
            }
        }


        public byte Alpha
        {
            get { return Color.A; }
            set
            {
                var current = Color;
                current.A = value;

                ((ColourInterpolatorModifier) Effect.ParticleEffect[0].Modifiers[2]).MiddleColour = current.ToVector3();
            }
        }


        public void Update()
        {
            this.LastPosition = this.Position;
            ((RadialGravityModifier) Effect.ParticleEffect[0].Modifiers[0]).Position = new Vector2(this.position.X, this.position.Y);

            Vector3 deplacement;
            Vector3.Subtract(ref this.position, ref this.LastPosition, out deplacement);
            Effect.Move(ref deplacement);
            Effect.Trigger(ref position);
        }
    }
}
