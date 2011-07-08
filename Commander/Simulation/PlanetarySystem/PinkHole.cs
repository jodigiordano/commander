﻿namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;
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
            float visualPriority)
            : base (simulator, name, path, position, 0, size, speed, null, startingPourc, visualPriority)
        {
            Moons = new List<Moon>();

            Effect = effect;
        }


        public Color Couleur
        {
            set
            {
                ((ColourInterpolatorModifier) Effect.ParticleEffect[0].Modifiers[2]).MiddleColour = value.ToVector3();
            }
        }


        public override double VisualPriority
        {
            get
            {
                return VisualPriorityBackup;
            }

            set
            {
                VisualPriorityBackup = Effect.VisualPriority;

                Effect.VisualPriority = value;

                for (int i = 0; i < Turrets.Count; i++)
                    Turrets[i].VisualPriority = value;
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
