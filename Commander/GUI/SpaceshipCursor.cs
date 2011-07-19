namespace EphemereGames.Commander.Simulation
{
    using System;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;
    using ProjectMercury.Emitters;


    class SpaceshipCursor : Cursor
    {
        private Particle TrailEffect;
        private Particle TrailEffect2;
        protected bool ShowTrail;
        private Color Color;
        private Vector3 LastPosition;
        private ProjectMercury.VariableFloat MovingReleaseSpeed;
        private ProjectMercury.VariableFloat NotMovingReleaseSpeed;


        public SpaceshipCursor(Scene scene, Vector3 initialPosition, float speed, double visualPriority, Color color, string image)
            : base(scene, initialPosition, speed, visualPriority, image, false)
        {
            FrontImage.SizeX = 4;

            SetBackImage(image + "Back", 4, color);
            Circle.Radius = FrontImage.Size.X / 2;

            Color = color;

            ShowTrail = true;

            TrailEffect = Scene.Particles.Get(@"spaceshipTrail");
            TrailEffect.ParticleEffect[0].ReleaseColour = Color.ToVector3();
            TrailEffect.VisualPriority = BackImage.VisualPriority + 0.00001;

            TrailEffect2 = Scene.Particles.Get(@"spaceshipTrail2");
            TrailEffect2.VisualPriority = BackImage.VisualPriority + 0.00002;

            LastPosition = Position;

            NotMovingReleaseSpeed = new ProjectMercury.VariableFloat()
            {
                Value = 50,
                Variation = 0
            };

            MovingReleaseSpeed = new ProjectMercury.VariableFloat()
            {
                Value = 0,
                Variation = 25
            };

            TrailEffect.ParticleEffect[0].ReleaseSpeed = NotMovingReleaseSpeed;
            TrailEffect2.ParticleEffect[0].ReleaseSpeed = NotMovingReleaseSpeed;

            FadeIn();
        }


        public float Size
        {
            set
            {
                FrontImage.SizeX = value;
                BackImage.SizeX = value;
            }
        }


        public override void FadeIn()
        {
            ShowTrail = true;
            base.FadeIn();
        }


        public override void FadeOut()
        {
            ShowTrail = false;
            base.FadeOut();
        }


        public override void Draw()
        {
            if (LastPosition == Position)
            {
                TrailEffect.ParticleEffect[0].ReleaseSpeed = NotMovingReleaseSpeed;
                ((ConeEmitter) TrailEffect.ParticleEffect[0]).Direction += 0.1f;
                ((ConeEmitter) TrailEffect.ParticleEffect[0]).Direction %= MathHelper.TwoPi;
            }

            else
            {
                TrailEffect.ParticleEffect[0].ReleaseSpeed = MovingReleaseSpeed;
            }

            LastPosition = Position;


            FrontImage.Rotation = (MathHelper.PiOver2) + (float)Math.Atan2(Direction.Y, Direction.X);
            BackImage.Rotation = FrontImage.Rotation;

            if (ShowTrail)
            {
                Vector3 p = FrontImage.Position;
                TrailEffect.Trigger(ref p);
            }

            base.Draw();
        }
    }
}
