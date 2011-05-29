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
        public float Vitesse                        { get; set; }
        public Vector3 Direction                    { get; set; }
        public Forme Forme                          { get; set; }
        public Cercle Cercle                        { get; set; }
        public bool Alive                       { get { return TempsExistence > 0; } }
        public MineralType Type                     { get { return Definition.Type; } }
        public int Value                            { get { return Definition.Value; } }
        public MineralDefinition Definition;

        private Vector3 AnciennePosition;
        private Vector3 position;
        private Scene Scene;
        private Vector3 Bouncing;
        private double TempsExistence;
        private ParticuleEffectWrapper RepresentationParticules;


        public Mineral(Scene scene, MineralDefinition definition, float visualPriority)
        {
            Scene = scene;
            Definition = definition;
            Vitesse = Main.Random.Next(10, 20);
            RepresentationParticules = scene.Particules.recuperer(definition.ParticulesRepresentation);
            RepresentationParticules.VisualPriority = visualPriority - 0.001f;
            TempsExistence = definition.TimeAlive;
            Cercle = new Cercle(this, definition.Radius);
        }


        public void Update(GameTime gameTime)
        {
            AnciennePosition = this.Position;

            Vitesse = Math.Max(0, Vitesse - 0.5f);

            Position += Direction * Vitesse;

            doBouncing();

            //Cercle.Position.X = Position.X - Origin.X;
            //Cercle.Position.Y = Position.Y - Origin.Y;

            if (Type == MineralType.Cash150)
                ((RadialGravityModifier) RepresentationParticules.ParticleEffect[1].Modifiers[0]).Position = new Vector2(Position.X - Definition.Origin.X, Position.Y - Definition.Origin.Y);

            TempsExistence -= gameTime.ElapsedGameTime.TotalMilliseconds;

            Vector3 deplacement;
            Vector3.Subtract(ref this.position, ref this.AnciennePosition, out deplacement);

            if (deplacement.X != 0 && deplacement.Y != 0)
                this.RepresentationParticules.Deplacer(ref deplacement);

            this.RepresentationParticules.Emettre(ref this.position);
        }


        public void DoHit(ILivingObject par) {}


        public void DoDie()
        {
            Scene.Particules.retourner(RepresentationParticules);

            ParticuleEffectWrapper pris = Scene.Particules.recuperer("mineralPris");
            pris.Emettre(ref this.position);
            Scene.Particules.retourner(pris);
            Scene.Animations.Insert(new MineralTakenAnimation(Scene, Definition, Position));
        }


        private void doBouncing()
        {
            if (Position.X > 640 - Preferences.DeadZoneXbox.X - 20)
            {
                Bouncing.X = -Math.Abs(Bouncing.X) + -Math.Abs(Vitesse);
                Bouncing.Y = Bouncing.Y + Vitesse;

                Vitesse = 0;
            }

            if (Position.X < -640 + Preferences.DeadZoneXbox.X + Cercle.Radius)
            {
                Bouncing.X = Math.Abs(Bouncing.X) + Math.Abs(Vitesse);
                Bouncing.Y = Bouncing.Y + Vitesse;

                Vitesse = 0;
            }

            if (Position.Y > 370 - Preferences.DeadZoneXbox.Y - Cercle.Radius)
            {
                Bouncing.X = Bouncing.X + Vitesse;
                Bouncing.Y = -Math.Abs(Bouncing.Y) - Math.Abs(Vitesse);

                Vitesse = 0;
            }

            if (Position.Y < -370 + Preferences.DeadZoneXbox.Y + Cercle.Radius)
            {
                Bouncing.X = Bouncing.X + Vitesse;
                Bouncing.Y = Math.Abs(Bouncing.Y) + Math.Abs(Vitesse);

                Vitesse = 0;
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
        public Ligne Ligne { get; set; }
        public float Masse { get; set; }
        public float Rotation { get; set; }
        public float ResistanceRotation { get; set; }
        public float LifePoints { get; set; }
        #endregion
    }
}
