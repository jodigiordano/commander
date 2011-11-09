namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;
    using ProjectMercury.Modifiers;


    class AlienNextWaveStartedAnimation : Animation
    {
        private CelestialBody CelestialBody;
        private Simulator Simulator;

        private Particle Effect;


        public AlienNextWaveStartedAnimation(Simulator simulator, CelestialBody celestialBody)
            : base(2000, VisualPriorities.Default.NextWaveStarted)
        {
            Simulator = simulator;
            CelestialBody = celestialBody;

            Effect = Simulator.Scene.Particles.Get(@"nextWave");
            Effect.VisualPriority = VisualPriorities.Default.NextWaveStarted;
            Effect.Model[0].ReleaseColour = Colors.Default.AlienBright.ToVector3();

            ((RadialGravityModifier) Effect.Model[0].Modifiers[0]).Strength = 100;

            Effect.Trigger(ref CelestialBody.position);
        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            ((RadialGravityModifier) Effect.Model[0].Modifiers[0]).Position = new Vector2(CelestialBody.position.X, CelestialBody.position.Y);

            var delta = CelestialBody.DeltaPosition;

            if (delta != Vector3.Zero)
                Effect.Move(ref delta);
        }
    }
}
