namespace EphemereGames.Commander.Simulation
{
    using System;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;
    using ProjectMercury.Emitters;


    class SpaceshipCursor : Cursor
    {
        private Particle TrailEffect;
        protected bool ShowTrail;
        private Color Color;
        private Vector3 LastPosition;
        private ProjectMercury.VariableFloat MovingReleaseSpeed;
        private ProjectMercury.VariableFloat NotMovingReleaseSpeed;


        public SpaceshipCursor(Scene scene, Vector3 initialPosition, float speed, double visualPriority, Color color, string representation)
            : base(scene, initialPosition, speed, visualPriority)
        {
            SetRepresentation(representation, 4);
            Circle.Radius = Representation.Size.X / 2;

            Color = color;

            Representation.Color = new Color(color.R, color.G, color.B, 20);

            ShowTrail = true;

            TrailEffect = Scene.Particles.Get(@"spaceshipTrail");
            TrailEffect.ParticleEffect[0].ReleaseColour = Color.ToVector3();
            TrailEffect.VisualPriority = Representation.VisualPriority + 0.00001f;

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

            FadeIn();
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


            Representation.Rotation = (MathHelper.PiOver2) + (float)Math.Atan2(Direction.Y, Direction.X);

            if (ShowTrail)
            {
                Vector3 p = Representation.Position;
                TrailEffect.Trigger(ref p);
            }

            base.Draw();
        }
    }
}
