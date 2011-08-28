namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using EphemereGames.Core.Utilities;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    public class AnimationTransitionAsteroids : Animation, ITransitionAnimation
    {
        public bool In;

        private List<Image> Asteroids;
        private List<Path2D> PathsIn;
        private List<Path2D> PathsOut;
        private List<float> RotationSpeeds;


        private static List<Vector2> Positions = new List<Vector2>()
        {
            new Vector2(-600, -300),
            new Vector2(-550, 200),
            new Vector2(-500, 0),
            new Vector2(-400, -200),
            new Vector2(-300, 100),
            new Vector2(-300, 200),
            new Vector2(-200, -250),
            new Vector2(0, 0),
            new Vector2(0, -300),
            new Vector2(200, -100),
            new Vector2(200, 50),
            new Vector2(350, 350),
            new Vector2(400, -300),
            new Vector2(450, 100),
            new Vector2(500, -50),
            new Vector2(550, 350),
            new Vector2(600, -350),
            new Vector2(0, 300)
        };


        private static List<string> PossibleImages = new List<string>()
        {
            @"Asteroid", @"Centaur", @"Plutoid", @"Meteoroid", @"Damacloid", @"Vulcanoid", @"Comet"
        };


        public AnimationTransitionAsteroids(double length, double visualPriority)
            : base(length, visualPriority)
        {
            In = false;

            Asteroids = new List<Image>();
            PathsIn = new List<Path2D>();
            PathsOut = new List<Path2D>();
            RotationSpeeds = new List<float>();

            for (int i = 0; i < Positions.Count; i++)
            {
                var img = new Image(PossibleImages[Main.Random.Next(0, PossibleImages.Count)]);

                img.SizeX = 36;
                img.Blend = BlendType.Substract;

                Asteroids.Add(img);

                var posX = Main.Random.Next(900, 1500);

                PathsIn.Add(new Path2D(new List<Vector2>() { Positions[i], new Vector2(posX * 1.5f, Positions[i].Y) }, new List<double>() { 0, Duration }));
                PathsOut.Add(new Path2D(new List<Vector2>() { new Vector2(-posX, Positions[i].Y), Positions[i] }, new List<double>() { 0, Duration }));
                RotationSpeeds.Add(((float) (Main.Random.NextDouble() * 0.2)));
            }
        }



        public void Initialize(TransitionType type)
        {
            base.Initialize();

            In = type == TransitionType.In;
        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }


        public override void Draw()
        {

            Scene.BeginForeground();
            
            for (int i = 0; i < Asteroids.Count; i++)
            {
                var img = Asteroids[i];

                img.Position = new Vector3(In ? PathsIn[i].GetPosition(ElapsedTime) : PathsOut[i].GetPosition(ElapsedTime), 0);
                img.Rotation += RotationSpeeds[i];

                Scene.Add(img);
            }

            Scene.EndForeground();
        }
    }
}
