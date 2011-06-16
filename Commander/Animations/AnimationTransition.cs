namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Utilities;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    public class AnimationTransition : Animation, ITransitionAnimation
    {
        public bool In;

        private List<AlienSpaceship> AlienShips;
        private List<Path2D> Paths;

        private static int LastTimeChoice = 0;
        private static string LastTimeChoiceAutre = "";
        private static Random Random = new Random();


        private static List<KeyValuePair<Vector2, Vector2>> PositionsIn = new List<KeyValuePair<Vector2,Vector2>>()
        {
            new KeyValuePair<Vector2, Vector2>(new Vector2(0, 200), new Vector2(0, 1000)),
            new KeyValuePair<Vector2, Vector2>(new Vector2(500, 0), new Vector2(500, 1200)),
            new KeyValuePair<Vector2, Vector2>(new Vector2(-100, -300), new Vector2(-100, 900)),
            new KeyValuePair<Vector2, Vector2>(new Vector2(-500, 400), new Vector2(-500, 1000)),
            new KeyValuePair<Vector2, Vector2>(new Vector2(-550, -200), new Vector2(-550, 1000)),
        };


        private static List<KeyValuePair<Vector2, Vector2>> PositionsOut = new List<KeyValuePair<Vector2, Vector2>>()
        {
            new KeyValuePair<Vector2, Vector2>(new Vector2(0, -1000), new Vector2(0, 200)),
            new KeyValuePair<Vector2, Vector2>(new Vector2(500, -800), new Vector2(500, 0)),
            new KeyValuePair<Vector2, Vector2>(new Vector2(-100, -800), new Vector2(-100, -300)),
            new KeyValuePair<Vector2, Vector2>(new Vector2(-500, -800), new Vector2(-500, 400)),
            new KeyValuePair<Vector2, Vector2>(new Vector2(-550, -800), new Vector2(-550, -200)),
        };

        
        private static List<string> NamesOthers = new List<string>()
        {
            "planete21",
            "Asteroid",
            "Comet",
            "Plutoid",
            "Centaur",
            "Trojan",
            "Meteoroid",
            "tourelleBase3",
            "tourelleGravitationnelleAntenne",
            "tourelleLaserCanon",
            "tourelleLaserMultiple3",
            "tourelleMissileCanon"
        };


        public AnimationTransition(double length, double visualPriority)
            : base(length, visualPriority)
        {
            In = false;
        }



        public void Initialize(TransitionType type)
        {
            base.Initialize();

            In = type == TransitionType.In;

            AlienShips = new List<AlienSpaceship>();
            Paths = new List<Path2D>();

            if (!In)
            {
                LastTimeChoice = Random.Next(0, 2);
                LastTimeChoiceAutre = NamesOthers[Random.Next(0, NamesOthers.Count)];
            }


            for (int i = 0; i < 5; i++)
            {
                AlienSpaceship v = new AlienSpaceship(Scene, this.VisualPriority);
                v.Representation.SizeX = 16;
                v.Representation.Rotation = MathHelper.PiOver2;
                v.Tentacules.Taille = 16;
                v.Tentacules.Rotation = MathHelper.PiOver2;
                v.Representation.Blend = TypeBlend.Substract;
                v.Tentacules.Blend = TypeBlend.Substract;

                AlienShips.Add(v);

                if (In)
                    Paths.Add(new Path2D(new List<Vector2>() { PositionsIn[i].Key, PositionsIn[i].Value }, new List<double>() { 0, this.Length }));
                else
                    Paths.Add(new Path2D(new List<Vector2>() { PositionsOut[i].Key, PositionsOut[i].Value }, new List<double>() { 0, this.Length }));
            }
        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }


        public override void Draw()
        {
            for (int i = 0; i < AlienShips.Count; i++)
            {
                AlienShips[i].Representation.position = new Vector3(Paths[i].position(RelativeTime), 0);
                AlienShips[i].Draw();
            }
        }
    }
}
