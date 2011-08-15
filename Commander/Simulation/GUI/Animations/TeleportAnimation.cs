namespace EphemereGames.Commander.Simulation
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Utilities;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class TeleportAnimation : Animation
    {
        private List<Image> Components;

        private Path3D ShakeHorizontal;

        private PinkHoleLite PinkHole;

        private Scene Scene;
        private Color Color;

        private bool FadeIn;
        private float FinalSize;
        private float OriginalSize;


        public TeleportAnimation(Scene scene, List<Image> components, double visualPriority, Color color, bool fadeIn)
            : base(1500, visualPriority)
        {
            Scene = scene;
            Components = components;
            Color = color;
            FadeIn = fadeIn;

            FinalSize = FadeIn ? Components[0].SizeX : 0;
            OriginalSize = Components[0].SizeX;

            if (FadeIn)
            {
                foreach (var c in components)
                    c.SizeX = 0;
            }

            ShakeHorizontal = new Path3D(
                new List<Vector3>()
                {
                    Vector3.Zero,
                    new Vector3(10, 0, 0),
                    new Vector3(-10, 0, 0),
                    new Vector3(25, 0, 0),
                    new Vector3(-25, 0, 0),
                    new Vector3(10, 0, 0),
                    new Vector3(-10, 0, 0),
                    Vector3.Zero
                },
                
                new List<double>()
                {
                    0,
                    250,
                    500,
                    750,
                    1000,
                    1100,
                    1200,
                    1500
                });
        }


        public override void Initialize()
        {
            base.Initialize();

            PinkHole = new PinkHoleLite(Scene, Components[0].Position, Scene.Particles.Get(@"trouRose"), VisualPriority - 0.000001);
            PinkHole.Radius = 1;
            PinkHole.Color = Color;
            PinkHole.ParticleVelocity = 20;
            PinkHole.ParticleGravityStrength = 40;
        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (ElapsedTime > Length * 0.75)
            {
                PinkHole.Radius = Math.Max(PinkHole.Radius - 2f, 0);
                PinkHole.Alpha = (byte) Math.Max(PinkHole.Alpha - 2, 0);
            }
            else
                PinkHole.Radius = Math.Min(PinkHole.Radius + 2f, 50f);

            if (ElapsedTime > Length * 0.50)
            {
                foreach (var c in Components)
                {
                    if (FadeIn)
                        c.SizeX = Math.Min((float) (FinalSize * ((ElapsedTime * 0.75) / (Length / 2))), OriginalSize);
                    else
                        c.SizeX = Math.Max((float) (OriginalSize * (1 - ElapsedTime / Length)), FinalSize);
                }
            }

            PinkHole.Position = Components[0].Position;
            PinkHole.Update();
        }


        public override void Stop()
        {
            base.Stop();

            foreach (var c in Components)
                c.SizeX = OriginalSize;
        }
    }
}
