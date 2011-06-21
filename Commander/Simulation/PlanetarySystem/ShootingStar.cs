namespace EphemereGames.Commander.Simulation
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Utilities;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;
    using ProjectMercury.Emitters;
    using ProjectMercury.Modifiers;


    class ShootingStar : IObjetPhysique, ILivingObject
    {
        public Scene Scene;
        public PhysicalRectangle Terrain;
        private bool ContentLoaded;

        public Vector3 Position         { get { return position; } set { position = value; } }
        public Shape Shape              { get; set; }
        public Circle Circle            { get; set; }
        public float ZoneImpact         { get; set; } 

        public float LifePoints         { get; set; }
        public float AttackPoints       { get; set; }
        public bool Alive               { get { return RemainingTime > 0; } }

        private Path2D Path;
        private double Length;
        private double RemainingTime;
        private Matrix RotationMatrix;
        private Particle Effect;
        private Vector3 position;
        private Vector2 TrailDirection;
        private List<Vector2> Positions;
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
            Shape = Shape.Circle;
            Circle = new Circle(Vector3.Zero, 20);
            ZoneImpact = 70f;

            LifePoints = float.MaxValue / 2;
            AttackPoints = float.MaxValue / 2;

            Positions = new List<Vector2>(3);
            Times = new List<double>(3);

            for (int i = 0; i < 3; i++)
            {
                Positions.Add(Vector2.Zero);
                Times.Add(0);
            }

            Path = new Path2D();

            ContentLoaded = false;
        }


        public void LoadContent()
        {
            if (!ContentLoaded)
            {
                Effect = Scene.Particles.Get(@"etoileFilante");
                Effect.VisualPriority = Preferences.PrioriteFondEcran - 0.01f;

                ContentLoaded = true;
            }
        }


        public void Initialize()
        {
            Vector2 startingPosition = new Vector2(Terrain.Width / 2 + 100, 0);
            Vector2 middlePosition;
            Vector2 endingPosition;

            float rotation = MathHelper.ToRadians(Main.Random.Next(-360, 360));

            Matrix.CreateRotationZ(rotation, out RotationMatrix);
            Vector2.Transform(ref startingPosition, ref RotationMatrix, out startingPosition);

            middlePosition = -startingPosition;

            endingPosition = startingPosition;
            rotation = (Main.Random.Next(0, 2) == 1) ? -MathHelper.PiOver2 : MathHelper.PiOver2;
            Matrix.CreateRotationZ(rotation, out RotationMatrix);
            Vector2.Transform(ref endingPosition, ref RotationMatrix, out endingPosition);
            endingPosition.Normalize();
            Vector2.Multiply(ref endingPosition, Main.Random.Next(200, 400), out endingPosition);

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

            Circle.Position = new Vector3(startingPosition.X, startingPosition.Y, 0);
        }


        public void Update()
        {
            RemainingTime = Math.Max(0, RemainingTime - Preferences.TargetElapsedTimeMs);

            double relativeTime = Length - RemainingTime;

            TrailDirection = Path.direction(relativeTime);
            ((ConeEmitter) Effect.ParticleEffect[0]).Direction = (float) Math.Atan2(TrailDirection.Y, TrailDirection.X) - MathHelper.Pi;

            Vector2 p = Path.position(relativeTime);
            position = new Vector3(p.X, p.Y, 0);

            Circle.Position = position;

            Effect.Trigger(ref position);
        }


        public void DoHit(ILivingObject par) { }


        public void DoDie()
        {
            
        }


        #region Useless
        public float Speed { get; set; }
        public Vector3 Direction { get; set; }
        public PhysicalRectangle Rectangle { get; set; }
        public Line Line { get; set; }
        public float Masse { get; set; }
        public float Rotation { get; set; }
        public float ResistanceRotation { get; set; }
        #endregion
    }
}
