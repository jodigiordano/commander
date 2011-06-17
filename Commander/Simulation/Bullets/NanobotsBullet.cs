namespace EphemereGames.Commander.Simulation
{
    using System;
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;
    using ProjectMercury.Emitters;


    class NanobotsBullet : Bullet
    {
        public float InfectionTime;
        private double Length;
        private Vector3 LastPosition;


        public NanobotsBullet()
            : base()
        {
            Shape = Shape.Circle;
            Circle = new Core.Physics.Circle(Vector3.Zero, 0);
            Rectangle = new RectanglePhysique();
            Explosive = true;
        }


        public override void Initialize()
        {
            base.Initialize();

            Circle.Radius = ExplosionRange;
            Circle.Position = Position;

            LastPosition = Position;

            MovingEffect = Scene.Particles.Get(@"planeteGazeuse");
            CircleEmitter ce = (CircleEmitter) MovingEffect.ParticleEffect[0];
            ce.Radius = Circle.Radius - 5;

            MovingEffect.VisualPriority = VisualPriority + 0.001f;
            LifePoints = Int16.MaxValue;
            Length = 5000;
            InfectionTime = 4000;
        }


        public override void Update()
        {
            Position += Speed * Direction;

            Length -= Preferences.TargetElapsedTimeMs;

            if (Length <= 0)
                LifePoints = 0;

            base.Update();
        }


        public override void Draw()
        {
            Vector3 deplacement;
            Vector3.Subtract(ref this.position, ref LastPosition, out deplacement);

            if (deplacement.X != 0 && deplacement.Y != 0)
            {
                MovingEffect.Move(ref deplacement);
                LastPosition = this.position;
            }

            base.Draw();
        }


        public override void DoHit(ILivingObject by) {}
    }
}