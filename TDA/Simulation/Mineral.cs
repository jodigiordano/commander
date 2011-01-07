namespace EphemereGames.Commander
{
    using System;
    using EphemereGames.Core.Physique;
    using EphemereGames.Core.Visuel;
    using Microsoft.Xna.Framework;
    using ProjectMercury.Modifiers;

    class Mineral : DrawableGameComponent, IObjetPhysique, IObjetVivant
    {
        private Vector3 AnciennePosition;

        private Vector3 position;
        public Vector3 Position                     { get { return position; } set { position = value; } }

        public float Vitesse                        { get; set; }
        public float Masse                          { get; set; }
        public Vector3 Direction                    { get; set; }
        public float Rotation                       { get; set; }
        public float ResistanceRotation             { get; set; }
        public Forme Forme                          { get; set; }
        public Cercle Cercle                        { get; set; }
        public RectanglePhysique Rectangle          { get; set; }
        public Ligne Ligne                          { get; set; }

        public float PointsVie                      { get; set; }
        public float PointsAttaque                  { get; set; }
        public bool EstVivant                       { get { return TempsExistence > 0; } }

        private Scene Scene;
        private double TempsExistence;
        private ParticuleEffectWrapper RepresentationParticules;
        public int Type;
        private float VitesseRotation;
        private Vector3 Bouncing;

        private static int[] valeurs = new int[] { 10, 25, 50, 1 };

        public int Valeur
        {
            get { return valeurs[Type]; }
        }

        public Mineral(Game main, Scene scene, int type, float prioriteAffichage)
            : base(main)
        {
            this.Scene = scene;
            this.Type = type;

            switch (type)
            {
                case 0:
                    RepresentationParticules = Scene.Particules.recuperer("mineral1");
                    TempsExistence = 12000;
                    Forme = Forme.Cercle;
                    Cercle = new Cercle(Position, 12);
                    break;
                case 1:
                    RepresentationParticules = Scene.Particules.recuperer("mineral2");
                    TempsExistence = 8000;
                    Forme = Forme.Cercle;
                    Cercle = new Cercle(Position, 14);
                    break;
                case 2:
                    RepresentationParticules = Scene.Particules.recuperer("mineral3");
                    TempsExistence = 4000;
                    Forme = Forme.Cercle;
                    Cercle = new Cercle(Position, 14);
                    break;
                case 3:
                    RepresentationParticules = Scene.Particules.recuperer("mineralPointsVie");
                    TempsExistence = 6000;
                    Forme = Forme.Cercle;
                    Cercle = new Cercle(Position, 14);
                    break;
            }

            RepresentationParticules.VisualPriority = prioriteAffichage - 0.001f;

            Vitesse = Main.Random.Next(10, 20);
            VitesseRotation = Main.Random.Next(1, 5) / 100f;
        }

        public override void Update(GameTime gameTime)
        {
            AnciennePosition = this.Position;

            Vitesse = Math.Max(0, Vitesse - 0.5f);

            Position += Direction * Vitesse;

            doBouncing();

            switch (Type)
            {
                case 0:
                    Cercle.Position.X = this.Position.X - 7;
                    Cercle.Position.Y = this.Position.Y - 7;

                    break;
                case 1:
                    Cercle.Position.X = this.Position.X;
                    Cercle.Position.Y = this.Position.Y - 10;

                    break;
                case 2:
                    Cercle.Position.X = this.Position.X;
                    Cercle.Position.Y = this.Position.Y - 12;
                    ((RadialGravityModifier)this.RepresentationParticules.ParticleEffect[1].Modifiers[0]).Position = new Vector2(this.Position.X, this.Position.Y - 12);
                    break;

                case 3:
                    Cercle.Position.X = this.Position.X - 15;
                    Cercle.Position.Y = this.Position.Y + 5;

                    break;
            }

            TempsExistence -= gameTime.ElapsedGameTime.TotalMilliseconds;

            Vector3 deplacement;
            Vector3.Subtract(ref this.position, ref this.AnciennePosition, out deplacement);

            if (deplacement.X != 0 && deplacement.Y != 0)
                this.RepresentationParticules.Deplacer(ref deplacement);

            this.RepresentationParticules.Emettre(ref this.position);
        }


        public void doTouche(IObjetVivant par) {}

        public void doMeurt()
        {
            //Array.Copy
            //(
            //    RepresentationParticules.ParticleEffect[0].Particles,
            //    RepresentationParticules.ParticleEffect[0].ActiveParticlesCount,
            //    RepresentationParticules.ParticleEffect[0].Particles,
            //    0,
            //    RepresentationParticules.ParticleEffect[0].ActiveParticlesCount
            //);

            //Array.Copy
            //(
            //    RepresentationParticules.ParticleEffect[1].Particles,
            //    RepresentationParticules.ParticleEffect[1].ActiveParticlesCount,
            //    RepresentationParticules.ParticleEffect[1].Particles,
            //    0,
            //    RepresentationParticules.ParticleEffect[1].ActiveParticlesCount
            //);


            Scene.Particules.retourner(RepresentationParticules);

            ParticuleEffectWrapper pris = Scene.Particules.recuperer("mineralPris");
            pris.Emettre(ref this.position);
            Scene.Particules.retourner(pris);
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

    }
}
