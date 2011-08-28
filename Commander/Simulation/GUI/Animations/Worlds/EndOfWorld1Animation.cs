namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Commander.Cutscenes;
    using Microsoft.Xna.Framework;


    class EndOfWorld1Animation : EndOfWorldAnimation
    {
        private Path Path;
        private List<CelestialBody> CelestialBodiesToDestroy;
        private MothershipAnimation MothershipAnimation;


        public EndOfWorld1Animation(WorldScene worldScene)
            : base(worldScene, 30000)
        {
            Path = Simulator.PlanetarySystemController.Path;

            // Prepare celestial bodies to destroy
            CelestialBodiesToDestroy = new List<CelestialBody>();

            foreach (var cb in Path.CelestialBodies)
                if (cb != Path.FirstCelestialBody || cb != Path.LastCelestialBody)
                    CelestialBodiesToDestroy.Add(cb);

            MothershipAnimation = new MothershipAnimation(Simulator)
            {
                ArrivalZoom = 0.7f,
                DepartureZoom = 1f,
                CelestialBodies = CelestialBodiesToDestroy,
                TimeBeforeArrival = 0,
                TimeArrival = 5500,
                TimeBeforeLights = 5500,
                TimeLights = 5000,
                TimeBeforeDestruction = 10500,
                TimeBeforeDeparture = 20500,
                TimeDeparture = 5000
            };

            // Switch the music

            // World 2 warp glow & text "Go to world 2" appears
            // Modify bounding box to match the camera
            // On next focus, recreate the path
            // Stop spawning enemies
            // Explose planets slowly without sfx
            // Fix warp visual priority
            // Show only warp

            // To check:
            // Don't permit to go in a level
            // Clear game in progress
            // Hide help bar
        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            MothershipAnimation.Update();
        }


        public override void Draw()
        {
            MothershipAnimation.Draw();
        }
    }
}
