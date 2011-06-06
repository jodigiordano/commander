namespace EphemereGames.Commander
{
    using System;
    using EphemereGames.Core.Physique;
    using EphemereGames.Core.Visuel;
    using Microsoft.Xna.Framework;
    using ProjectMercury.Modifiers;


    class Mineral : IObjetPhysique, ILivingObject
    {
        public Vector3 Position                     { get { return position; } set { position = value; } }
        public float Speed                          { get; set; }
        public Vector3 Direction                    { get; set; }
        public Shape Shape                          { get; set; }
        public Cercle Circle                        { get; set; }
        public bool Alive                           { get { return TempsExistence > 0; } }
        public MineralType Type                     { get { return Definition.Type; } }
        public int Value                            { get { return Definition.Value; } }
        public MineralDefinition Definition;

        private Vector3 AnciennePosition;
        private Vector3 position;
        private Scene Scene;
        private Vector3 Bouncing;
        private double TempsExistence;
        private Particle RepresentationParticules;


        public Mineral(Scene scene, MineralDefinition definition, float visualPriority)
        {
            Scene = scene;
            Definition = definition;
            Speed = Main.Random.Next(10, 20);
            RepresentationParticules = scene.Particules.Get(definition.ParticulesRepresentation);
            RepresentationParticules.VisualPriority = visualPriority - 0.001f;
            TempsExistence = definition.TimeAlive;
            Circle = new Cercle(this, definition.Radius);
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

            TempsExistence -= 16.66;

            Vector3 deplacement;
            Vector3.Subtract(ref this.position, ref this.AnciennePosition, out deplacement);

            if (deplacement.X != 0 && deplacement.Y != 0)
                this.RepresentationParticules.Move(ref deplacement);

            this.RepresentationParticules.Trigger(ref this.position);
        }


        public void DoHit(ILivingObject par) {}


        public void DoDie()
        {
            Scene.Particules.Return(RepresentationParticules);

            Particle pris = Scene.Particules.Get("mineralPris");
            pris.Trigger(ref this.position);
            Scene.Particules.Return(pris);
            Scene.Animations.Insert(new MineralTakenAnimation(Scene, Definition, Position));
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
        public Ligne Line { get; set; }
        public float Masse { get; set; }
        public float Rotation { get; set; }
        public float ResistanceRotation { get; set; }
        public float LifePoints { get; set; }
        #endregion
    }
}
