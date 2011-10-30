namespace EphemereGames.Commander.Simulation
{
    using System;
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;
    using ProjectMercury.Emitters;


    class MultipleLasersBullet : Bullet
    {
        private const int LONGUEUR_LIGNE = 1500;

        private double LifeSpan;
        public IDestroyable Target;
        public Vector3 TargetOffset;
        public Turret Turret;
        private VisualLine MovingEffect2;
        private bool Orphelin;


        public MultipleLasersBullet()
            : base()
        {
            Speed = 0;
            Shape = Shape.Line;
            Line = new Line(Vector2.Zero, Vector2.Zero);
        }


        public override void Initialize()
        {
            base.Initialize();

            Position = Turret.Position + TargetOffset;
            
            Line.Start = Position;
            Line.End = Direction * LONGUEUR_LIGNE;

            MovingEffect = Scene.Particles.Get(@"projectileLaserMultiple");
            LineEmitter emitter = (LineEmitter)MovingEffect.Model[0];
            emitter.Length = LONGUEUR_LIGNE;
            MovingEffect.VisualPriority = VisualPriority + 0.001f;

            MovingEffect2 = new VisualLine(this.Position, this.Position + this.Direction * LONGUEUR_LIGNE, new Color(255, 216, 0, 100), 4);
            MovingEffect2.VisualPriority = VisualPriority + 0.002f;

            LifePoints = Int16.MaxValue;
            LifeSpan = 800;
            Orphelin = false;
        }


        public override void Update()
        {
            Position = Turret.Position + TargetOffset;

            if (!Orphelin && Target.Alive)
            {
                Vector3 newDirection = (Target.Position + TargetOffset) - this.Position;
                newDirection.Normalize();

                this.Direction = newDirection;
            }

            else
                Orphelin = true;

            Line.Start = this.Position;
            Line.End = this.Position + this.Direction * LONGUEUR_LIGNE;

            LifeSpan -= Preferences.TargetElapsedTimeMs;

            if (LifeSpan <= 0)
                LifePoints = 0;

            base.Update();
        }


        //public override void Show()
        //{
        //    Scene.Add(MovingEffect2);
        //}


        //public override void Hide()
        //{
        //    Scene.Remove(MovingEffect2);
        //}


        public override void Draw()
        {
            LineEmitter emitter = (LineEmitter)MovingEffect.Model[0];
            emitter.Angle = MathHelper.Pi + (float)Math.Atan2(Direction.Y, Direction.X);
            emitter.TriggerOffset = new Vector2(this.Direction.X * (LONGUEUR_LIGNE/2), this.Direction.Y * (LONGUEUR_LIGNE/2));

            MovingEffect2.Start = Line.Start;
            MovingEffect2.End = Line.End;

            Scene.Add(MovingEffect2);

            base.Draw();
        }


        public override void DoHit(ILivingObject par) {}
    }
}