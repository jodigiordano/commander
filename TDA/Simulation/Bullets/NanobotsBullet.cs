namespace EphemereGames.Commander
{
    using System;
    using EphemereGames.Core.Physique;
    using EphemereGames.Core.Visuel;
    using Microsoft.Xna.Framework;
    using ProjectMercury.Emitters;


    class NanobotsBullet : Bullet
    {
        public float ZoneImpact;
        public float InfectionTime;
        private double Length;
        private Vector3 LastPosition;


        public NanobotsBullet()
            : base()
        {
            Shape = Shape.Circle;
            Circle = new Core.Physique.Cercle(Vector3.Zero, 0);
            Rectangle = new RectanglePhysique();
        }


        public override void Initialize()
        {
            base.Initialize();

            Circle.Radius = ZoneImpact;
            Circle.Position = Position;

            LastPosition = Position;

            MovingEffect = Scene.Particles.Get("planeteGazeuse");
            CircleEmitter ce = (CircleEmitter) MovingEffect.ParticleEffect[0];
            ce.Radius = Circle.Radius - 5;

            MovingEffect.VisualPriority = PrioriteAffichage + 0.001f;
            LifePoints = Int16.MaxValue;
            Length = 5000;
            InfectionTime = 4000;
        }


        public override void Update()
        {
            Position += Speed * Direction;

            Length -= 16.66;

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