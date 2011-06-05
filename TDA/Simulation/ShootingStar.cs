namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Physique;
    using EphemereGames.Core.Utilities;
    using EphemereGames.Core.Visuel;
    using Microsoft.Xna.Framework;
    using ProjectMercury.Emitters;
    using ProjectMercury.Modifiers;


    class ShootingStar : IObjetPhysique, ILivingObject
    {
        public Simulation Simulation;
        private bool ContentLoaded;

        public Vector3 Position         { get { return position; } set { position = value; } }
        public Forme Forme              { get; set; }
        public Cercle Cercle            { get; set; }
        public float ZoneImpact         { get; set; } 

        public float LifePoints         { get; set; }
        public float AttackPoints       { get; set; }
        public bool Alive               { get { return RemainingTime > 0; } }

        private Trajet3D Path;
        private double Length;
        private double RemainingTime;
        private Matrix RotationMatrix;
        private ParticuleEffectWrapper Effect;
        private Vector3 position;
        private Vector3 TrailDirection;
        private List<Vector3> Positions;
        private List<double> Times;


        private static Vector3[] Couleurs = new Vector3[]
        {
            new Vector3(1, 0, 0),
            new Vector3(0, 1, 0),
            new Vector3(1, 0, 1),
            new Vector3(1, 0.5f, 0)
        };


        public ShootingStar()
        {
            Forme = Forme.Cercle;
            Cercle = new Cercle(Vector3.Zero, 20);
            ZoneImpact = 70f;

            LifePoints = float.MaxValue / 2;
            AttackPoints = float.MaxValue / 2;

            Positions = new List<Vector3>(3);
            Times = new List<double>(3);

            for (int i = 0; i < 3; i++)
            {
                Positions.Add(Vector3.Zero);
                Times.Add(0);
            }

            Path = new Trajet3D();

            ContentLoaded = false;
        }


        public void LoadContent()
        {
            if (!ContentLoaded)
            {
                Effect = Simulation.Scene.Particules.recuperer("etoileFilante");
                Effect.VisualPriority = Preferences.PrioriteFondEcran - 0.01f;

                ContentLoaded = true;
            }
        }


        public void Initialize()
        {
            Vector3 startingPosition = new Vector3(Simulation.Terrain.Width / 2 + 100, 0, 0);
            Vector3 middlePosition;
            Vector3 endingPosition;

            float rotation = MathHelper.ToRadians(Main.Random.Next(-360, 360));

            Matrix.CreateRotationZ(rotation, out RotationMatrix);
            Vector3.Transform(ref startingPosition, ref RotationMatrix, out startingPosition);

            middlePosition = -startingPosition;

            endingPosition = startingPosition;
            rotation = (Main.Random.Next(0, 2) == 1) ? -MathHelper.PiOver2 : MathHelper.PiOver2;
            Matrix.CreateRotationZ(rotation, out RotationMatrix);
            Vector3.Transform(ref endingPosition, ref RotationMatrix, out endingPosition);
            endingPosition.Normalize();
            Vector3.Multiply(ref endingPosition, Main.Random.Next(200, 400), out endingPosition);

            double time = Main.Random.Next(800, 2000);

            Positions[0] = startingPosition;
            Positions[1] = endingPosition;
            Positions[2] = middlePosition;

            Times[0] = 0;
            Times[1] = time / 2;
            Times[2] = time * 2;

            Path.Initialize(Positions, Times);

            RemainingTime = Length = time * 2;

            ((ColourInterpolatorModifier) Effect.ParticleEffect[0].Modifiers[1]).InitialColour = Couleurs[Main.Random.Next(0, Couleurs.Length)];

            Cercle.Position = startingPosition;
        }


        public void Update()
        {
            RemainingTime = Math.Max(0, RemainingTime - 16.66f);

            double relativeTime = Length - RemainingTime;

            Path.Direction(relativeTime, out TrailDirection);
            ((ConeEmitter) Effect.ParticleEffect[0]).Direction = (float) Math.Atan2(TrailDirection.Y, TrailDirection.X) - MathHelper.Pi;

            Path.Position(relativeTime, ref position);

            Cercle.Position = position;

            Effect.Emettre(ref position);
        }


        public void DoHit(ILivingObject par) { }


        public void DoDie()
        {
            
        }


        #region Useless
        public float Vitesse { get; set; }
        public Vector3 Direction { get; set; }
        public RectanglePhysique Rectangle { get; set; }
        public Ligne Ligne { get; set; }
        public float Masse { get; set; }
        public float Rotation { get; set; }
        public float ResistanceRotation { get; set; }
        #endregion
    }
}
