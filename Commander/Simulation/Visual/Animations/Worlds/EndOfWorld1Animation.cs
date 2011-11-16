namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;
    using EphemereGames.Commander.Cutscenes;
    using EphemereGames.Core.Visual;
    using EphemereGames.Core.XACTAudio;
    using Microsoft.Xna.Framework;


    class EndOfWorld1Animation : EndOfWorldAnimation
    {
        private List<CelestialBody> CelestialBodiesToDestroy;
        private MothershipAnimation MothershipAnimation;
        private Text TmpEndOfAlpha;


        public EndOfWorld1Animation(WorldScene worldScene)
            : base(worldScene, 30000)
        {
            // Prepare celestial bodies toLocal destroy
            CelestialBodiesToDestroy = new List<CelestialBody>();

            foreach (var cb in worldScene.Simulator.Data.Level.PlanetarySystem)
                if (!cb.FirstOnPath && !cb.LastOnPath)
                    CelestialBodiesToDestroy.Add(cb);

            MothershipAnimation = new MothershipAnimation(Simulator)
            {
                ArrivalZoom = Simulator.CameraController.CameraData.MaxZoomOut,
                DepartureZoom = Simulator.CameraController.CameraData.MaxZoomOut + Simulator.CameraController.CameraData.MaxDelta / 2,
                CelestialBodies = CelestialBodiesToDestroy,
                TimeBeforeArrival = 0,
                TimeArrival = 5500,
                TimeBeforeLights = 5500,
                TimeLights = 2000,
                TimeBeforeDestruction = 7500,
                TimeBeforeDeparture = 17500,
                TimeDeparture = 5000
            };

            MothershipAnimation.StartingPosition = new Vector3(
                worldScene.Simulator.Data.Battlefield.Inner.Center.X,
                worldScene.Simulator.Data.Battlefield.Inner.Top - MothershipAnimation.Mothership.Size.Y / 2 - 360, 0);
            MothershipAnimation.ArrivingPosition = new Vector3(
                Simulator.Data.Battlefield.Inner.Center.X,
                Simulator.Data.Battlefield.Inner.Top - MothershipAnimation.Mothership.Size.Y * 0.2f, 0);
            MothershipAnimation.DeparturePosition = new Vector3(
                Simulator.Data.Battlefield.Inner.Center.X,
                Simulator.Data.Battlefield.Inner.Bottom + MothershipAnimation.Mothership.Size.Y, 0);

            Simulator.CameraController.CameraData.ManualZoom = true;

            // Switch the music
            Main.MusicController.PlayOrResume(worldScene.World.Descriptor.MusicEnd);
            XACTAudio.PlayCue(worldScene.World.Descriptor.SfxEnd, "Sound Bank");

            Simulator.SpawnEnemies = false;

            TmpEndOfAlpha = new Text("End of demo!", "Pixelite", new Vector3(0, 200, 0))
            {
                SizeX = 4,
                Alpha = 0,
                VisualPriority = VisualPriorities.Default.Path + 0.01
            }.CenterIt();
            Simulator.Scene.VisualEffects.Add(TmpEndOfAlpha, VisualEffects.FadeInFrom0(255, 20000, 2000));
            Simulator.Scene.VisualEffects.Add(TmpEndOfAlpha, VisualEffects.FadeOutTo0(255, 25000, 2000));
        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            MothershipAnimation.Update();
        }


        public override void Draw()
        {
            MothershipAnimation.Draw();

            Scene.BeginForeground();
                Scene.Add(TmpEndOfAlpha);
            Scene.EndForeground();
        }
    }
}
