namespace EphemereGames.Commander
{
    using System;
    using EphemereGames.Core.Physique;
    using EphemereGames.Core.Visuel;
    using Microsoft.Xna.Framework;
    using ProjectMercury.Emitters;


    class MultipleLasersBullet : Bullet
    {
        private const int LONGUEUR_LIGNE = 1500;

        private double LifeSpan;
        public Enemy Target;
        public Vector3 TargetOffset;
        public Turret Turret;
        private LigneVisuel MovingEffect2;
        private bool Orphelin;


        public MultipleLasersBullet()
            : base()
        {
            Speed = 0;
            Shape = Shape.Line;
            Line = new Ligne(Vector2.Zero, Vector2.Zero);
        }


        public override void Initialize()
        {
            base.Initialize();

            Position = Turret.Position + TargetOffset;
            
            Line.Debut = Position;
            Line.Fin = Direction * LONGUEUR_LIGNE;

            MovingEffect = Scene.Particles.Get("projectileLaserMultiple");
            LineEmitter emitter = (LineEmitter)MovingEffect.ParticleEffect[0];
            emitter.Length = LONGUEUR_LIGNE;
            MovingEffect.VisualPriority = PrioriteAffichage + 0.001f;

            MovingEffect2 = new LigneVisuel(this.Position, this.Position + this.Direction * LONGUEUR_LIGNE, new Color(255, 216, 0, 100), 4);
            MovingEffect2.VisualPriority = PrioriteAffichage + 0.002f;

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

            Line.Debut = this.Position;
            Line.Fin = this.Position + this.Direction * LONGUEUR_LIGNE;

            LifeSpan -= 16.66;

            if (LifeSpan <= 0)
                LifePoints = 0;

            base.Update();
        }


        public override void Show()
        {
            Scene.Add(MovingEffect2);
        }


        public override void Hide()
        {
            Scene.Remove(MovingEffect2);
        }


        public override void Draw()
        {
            LineEmitter emitter = (LineEmitter)MovingEffect.ParticleEffect[0];
            emitter.Angle = MathHelper.Pi + (float)Math.Atan2(Direction.Y, Direction.X);
            emitter.TriggerOffset = new Vector2(this.Direction.X * (LONGUEUR_LIGNE/2), this.Direction.Y * (LONGUEUR_LIGNE/2));

            MovingEffect2.Start = Line.Debut;
            MovingEffect2.End = Line.Fin;

            base.Draw();
        }


        public override void DoHit(ILivingObject par) {}
    }
}