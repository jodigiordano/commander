namespace EphemereGames.Commander.Simulation
{
    using System;
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;
    using ProjectMercury.Emitters;


    class LaserBullet : Bullet
    {
        public IDestroyable Target;
        public Turret Turret;
        private VisualLine MovingEffect2;
        private double LifeSpan;


        public LaserBullet()
            : base()
        {
            Speed = 0;
            Shape = Shape.Line;
            Line = new Line(Vector2.Zero, Vector2.Zero);
        }


        public override void Initialize()
        {
            base.Initialize();

            Position = Turret.Position;

            MovingEffect = Scene.Particles.Get(@"projectileLaserSimple");
            MovingEffect.VisualPriority = VisualPriority + 0.001f;

            Line.Debut = Turret.Position;
            Line.Fin = Target.Position;

            LineEmitter emitter = (LineEmitter)MovingEffect.Model[0];
            emitter.Length = Line.Longueur;
            emitter = (LineEmitter)MovingEffect.Model[1];
            emitter.Length = Line.Longueur;

            MovingEffect2 = new VisualLine(Line.Debut, Line.Fin, new Color(255, 0, 110), 4);
            MovingEffect2.VisualPriority = VisualPriority + 0.002f;

            LifePoints = Int16.MaxValue;
            LifeSpan = 800;
        }


        public override void Update()
        {
            if (Target.Alive)
            {
                Vector3 nouvelleDirection = Target.Position - this.Position;
                nouvelleDirection.Normalize();

                this.Direction = nouvelleDirection;
            }

            else
            {
                LifePoints = 0;
                return;
            }

            Position = Turret.Position;

            Line.Debut = this.Position;
            Line.Fin = Target.Position;

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
            LineEmitter emitter1 = (LineEmitter)MovingEffect.Model[0];
            emitter1.Angle = MathHelper.Pi + (float)Math.Atan2(Direction.Y, Direction.X);
            emitter1.Length = (Target.Position - Turret.Position).Length();
            emitter1.TriggerOffset = new Vector2(this.Direction.X * (emitter1.Length / 2), this.Direction.Y * (emitter1.Length / 2));
            
            LineEmitter emitter2 = (LineEmitter) MovingEffect.Model[1];
            emitter2.Angle = emitter1.Angle;
            emitter2.Length = emitter1.Length;
            emitter2.TriggerOffset = emitter1.TriggerOffset;

            MovingEffect2.Start = Line.Debut;
            MovingEffect2.End = Line.Fin;

            Scene.Add(MovingEffect2);

            base.Draw();
        }


        public override void DoHit(ILivingObject par) {}
    }
}