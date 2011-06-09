namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Utilities;
    using EphemereGames.Core.Visuel;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;


    public class AnimationTransition : Animation
    {
        public bool In;

        private List<AlienSpaceship> AlienShips;
        private List<Trajet3D> Paths;

        private static int LastTimeChoice = 0;
        private static String LastTimeChoiceAutre = "";
        private static Random Random = new Random();


        private static List<KeyValuePair<Vector3, Vector3>> PositionsIn = new List<KeyValuePair<Vector3,Vector3>>()
        {
            new KeyValuePair<Vector3, Vector3>(new Vector3(0, 200, 0), new Vector3(0, 1000, 0)),
            new KeyValuePair<Vector3, Vector3>(new Vector3(500, 0, 0), new Vector3(500, 1200, 0)),
            new KeyValuePair<Vector3, Vector3>(new Vector3(-100, -300, 0), new Vector3(-100, 900, 0)),
            new KeyValuePair<Vector3, Vector3>(new Vector3(-500, 400, 0), new Vector3(-500, 1000, 0)),
            new KeyValuePair<Vector3, Vector3>(new Vector3(-550, -200, 0), new Vector3(-550, 1000, 0)),
        };


        private static List<KeyValuePair<Vector3, Vector3>> PositionsOut = new List<KeyValuePair<Vector3, Vector3>>()
        {
            new KeyValuePair<Vector3, Vector3>(new Vector3(0, -1000, 0), new Vector3(0, 200, 0)),
            new KeyValuePair<Vector3, Vector3>(new Vector3(500, -800, 0), new Vector3(500, 0, 0)),
            new KeyValuePair<Vector3, Vector3>(new Vector3(-100, -800, 0), new Vector3(-100, -300, 0)),
            new KeyValuePair<Vector3, Vector3>(new Vector3(-500, -800, 0), new Vector3(-500, 400, 0)),
            new KeyValuePair<Vector3, Vector3>(new Vector3(-550, -800, 0), new Vector3(-550, -200, 0)),
        };

        
        private static List<String> NamesOthers = new List<string>()
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


        public override void Initialize()
        {
            base.Initialize();

            AlienShips = new List<AlienSpaceship>();
            Paths = new List<Trajet3D>();

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
                    Paths.Add(new Trajet3D(new List<Vector3>() { PositionsIn[i].Key, PositionsIn[i].Value }, new List<double>() { 0, this.Length }));
                else
                    Paths.Add(new Trajet3D(new List<Vector3>() { PositionsOut[i].Key, PositionsOut[i].Value }, new List<double>() { 0, this.Length }));
            }
        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }


        public override void Draw(SpriteBatch spriteBatch)
        {

            if (ShowAliens)
            {
                for (int i = 0; i < AlienShips.Count; i++)
                {
                    Paths[i].Position(RelativeTime, ref AlienShips[i].Representation.position);
                    AlienShips[i].Draw(null);
                }
            }

            else
            {
                for (int i = 0; i < Others.Count; i++)
                {
                    Paths[i].Position(RelativeTime, ref Others[i].position);
                    Scene.ajouterScenable(Others[i]);
                }
            }
        }
    }
}
