namespace EphemereGames.Commander
{
    using System;
    using EphemereGames.Core.Physique;
    using EphemereGames.Core.Visuel;
    using Microsoft.Xna.Framework;
    using ProjectMercury.Emitters;


    class LaserBullet : Bullet
    {
        public Enemy Target;
        public Turret Turret;
        private LigneVisuel MovingEffect2;
        private double LifeSpan;


        public LaserBullet()
            : base()
        {
            Speed = 0;
            Shape = Shape.Line;
            Line = new Ligne(Vector2.Zero, Vector2.Zero);
        }


        public override void Initialize()
        {
            base.Initialize();

            Position = Turret.Position;

            MovingEffect = Scene.Particles.Get("projectileLaserSimple");
            MovingEffect.VisualPriority = PrioriteAffichage + 0.001f;

            Line.Debut = Turret.Position;
            Line.Fin = Target.Position;

            LineEmitter emitter = (LineEmitter)MovingEffect.ParticleEffect[0];
            emitter.Length = Line.Longueur;
            emitter = (LineEmitter)MovingEffect.ParticleEffect[1];
            emitter.Length = Line.Longueur;

            MovingEffect2 = new LigneVisuel(Line.Debut, Line.Fin, new Color(255, 0, 110), 4);
            MovingEffect2.VisualPriority = PrioriteAffichage + 0.002f;

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

            Position = TourelleEmettrice.Position;

            Line.Debut = this.Position;
            Line.Fin = Cible.Position;

            DureeVie -= 16.66;

            if (DureeVie <= 0)
                LifePoints = 0;

            base.Update();
        }


        public override void Draw()
        {
            if (Alive)
                RepresentationVivantAlt.Trigger(ref this.position);

            LineEmitter emetteur = (LineEmitter)RepresentationVivantAlt.ParticleEffect[0];
            LineEmitter emetteur2 = (LineEmitter)RepresentationVivantAlt.ParticleEffect[1];
            emetteur.Angle = MathHelper.Pi + (float)Math.Atan2(Direction.Y, Direction.X);
            emetteur.Length = (Cible.Position - TourelleEmettrice.Position).Length();
            emetteur.TriggerOffset = new Vector2(this.Direction.X * (emetteur.Length / 2), this.Direction.Y * (emetteur.Length / 2));
            emetteur2.Angle = emetteur.Angle;
            emetteur2.Length = emetteur.Length;
            emetteur2.TriggerOffset = emetteur.TriggerOffset;

            RepresentationVivantAlt2.Debut = Line.Debut;
            RepresentationVivantAlt2.Fin = Line.Fin;

            Scene.ajouterScenable(RepresentationVivantAlt2);

            base.Draw();
        }


        public override void DoHit(ILivingObject par)
        {

        }


        public override void DoDie()
        {
            base.DoDie();

            Scene.Particles.Return(RepresentationVivantAlt);

            Bullet.PoolLaserBullets.Return(this);
        }


        public override void DoDieSilent()
        {
            base.DoDieSilent();

            Scene.Particles.Return(RepresentationVivantAlt);

            Bullet.PoolLaserBullets.Return(this);
        }
    }
}