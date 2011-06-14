namespace EphemereGames.Commander
{
    using System;
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;
    using ProjectMercury.Modifiers;


    class Mineral : IObjetPhysique, ILivingObject
    {
        public Vector3 Position                     { get { return position; } set { position = value; } }
        public float Speed                          { get; set; }
        public Vector3 Direction                    { get; set; }
        public Shape Shape                          { get; set; }
        public Circle Circle                        { get; set; }
        public bool Alive                           { get { return TempsExistence > 0 && LifePoints > 0; } }
        public float LifePoints                     { get; set; }
        public MineralType Type                     { get { return Definition.Type; } }
        public int Value                            { get { return Definition.Value; } }
        public MineralDefinition Definition;
        public double VisualPriority;
        public Scene Scene;

        private Vector3 AnciennePosition;
        private Vector3 position;
        private Vector3 Bouncing;
        private double TempsExistence;
        private Particle RepresentationParticules;


        public void Initialize()
        {
            Speed = Main.Random.Next(10, 20);
            RepresentationParticules = Scene.Particles.Get(Definition.ParticulesRepresentation);
            RepresentationParticules.VisualPriority = VisualPriority - 0.001f;
            TempsExistence = Definition.TimeAlive;
            Circle = new Circle(this, Definition.Radius);
            LifePoints = 1;
        }


        public void Update()
        {
            AnciennePosition = this.Position;

            Speed = Math.Max(0, Speed - 0.5f);

            Position += Direction * Speed;

            doBouncing();

            //Cercle.Position.X = Position.X - Origin.X;
            //Cercle.Position.Y = Position.Y - Origin.Y;

            if (Type == MineralType.Cash150)
                ((RadialGravityModifier) RepresentationParticules.ParticleEffect[1].Modifiers[0]).Position = new Vector2(Position.X - Definition.Origin.X, Position.Y - Definition.Origin.Y);

            TempsExistence -= Preferences.TargetElapsedTimeMs;

            Vector3 deplacement;
            Vector3.Subtract(ref this.position, ref this.AnciennePosition, out deplacement);

            if (deplacement.X != 0 && deplacement.Y != 0)
                this.RepresentationParticules.Move(ref deplacement);

            this.RepresentationParticules.Trigger(ref this.position);
        }


        public void DoHit(ILivingObject par) {}


        public void DoDie()
        {
            Scene.Particles.Return(RepresentationParticules);

            if (TempsExistence <= 0)
                return;

            Particle pris = Scene.Particles.Get(@"mineralPris");
            pris.Trigger(ref this.position);
            Scene.Particles.Return(pris);
            Scene.Animations.Add(new MineralTakenAnimation(Scene, Definition, Position, RepresentationParticules.VisualPriority - 0.0001));

            string sfx = (Type == MineralType.Life1) ? "sfxLifePack" : "sfxMoney" + Main.Random.Next(1, 4);

            EphemereGames.Core.Audio.Audio.jouerEffetSonore("Partie", sfx);
        }


        private void doBouncing()
        {
            if (Position.X > 640 - Preferences.DeadZoneXbox.X - 20)
            {
                Bouncing.X = -Math.Abs(Bouncing.X) + -Math.Abs(Speed);
                Bouncing.Y = Bouncing.Y + Speed;

                Speed = 0;
            }

            if (Position.X < -640 + Preferences.DeadZoneXbox.X + Circle.Radius)
            {
                Bouncing.X = Math.Abs(Bouncing.X) + Math.Abs(Speed);
                Bouncing.Y = Bouncing.Y + Speed;

                Speed = 0;
            }

            if (Position.Y > 370 - Preferences.DeadZoneXbox.Y - Circle.Radius)
            {
                Bouncing.X = Bouncing.X + Speed;
                Bouncing.Y = -Math.Abs(Bouncing.Y) - Math.Abs(Speed);

                Speed = 0;
            }

            if (Position.Y < -370 + Preferences.DeadZoneXbox.Y + Circle.Radius)
            {
                Bouncing.X = Bouncing.X + Speed;
                Bouncing.Y = Math.Abs(Bouncing.Y) + Math.Abs(Speed);

                Speed = 0;
            }

            Position += Bouncing;

            if (Bouncing.X > 0)
                Bouncing.X = Math.Max(0, Bouncing.X - 0.5f);
            else if (Bouncing.X < 0)
                Bouncing.X = Math.Min(0, Bouncing.X + 0.5f);

            if (Bouncing.Y > 0)
                Bouncing.Y = Math.Max(0, Bouncing.Y - 0.5f);
            else if (Bouncing.Y < 0)
                Bouncing.Y = Math.Min(0, Bouncing.Y + 0.5f);
        }

        #region Useless
        public float AttackPoints { get; set; }
        public RectanglePhysique Rectangle { get; set; }
        public Line Line { get; set; }
        public float Masse { get; set; }
        public float Rotation { get; set; }
        public float ResistanceRotation { get; set; }
        #endregion
    }
}
