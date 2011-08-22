namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using EphemereGames.Commander.Simulation;
    using EphemereGames.Core.Utilities;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    public class AnimationTransitionAlienBattleship : Animation, ITransitionAnimation
    {
        public bool In;

        private List<AlienSpaceship> AlienShips;
        private List<Path2D> PathsIn;
        private List<Path2D> PathsOut;


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


        public AnimationTransitionAlienBattleship(double length, double visualPriority)
            : base(length, visualPriority)
        {
            In = false;

            AlienShips = new List<AlienSpaceship>();
            PathsIn = new List<Path2D>();
            PathsOut = new List<Path2D>();

            for (int i = 0; i < 5; i++)
            {
                AlienSpaceship v = new AlienSpaceship(VisualPriority);
                v.Image.SizeX = 16;
                v.Image.Rotation = MathHelper.PiOver2;
                v.Tentacules.Taille = 16;
                v.Tentacules.Rotation = MathHelper.PiOver2;
                v.Image.Blend = BlendType.Substract;
                v.Tentacules.Blend = BlendType.Substract;

                AlienShips.Add(v);

                PathsIn.Add(new Path2D(new List<Vector2>() { PositionsIn[i].Key, PositionsIn[i].Value }, new List<double>() { 0, this.Length }));
                PathsOut.Add(new Path2D(new List<Vector2>() { PositionsOut[i].Key, PositionsOut[i].Value }, new List<double>() { 0, this.Length }));
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
            for (int i = 0; i < AlienShips.Count; i++)
            {
                var ship = AlienShips[i];

                ship.Image.position = new Vector3(In ? PathsIn[i].GetPosition(ElapsedTime) : PathsOut[i].GetPosition(ElapsedTime), 0);

                Scene.BeginForeground();
                ship.Draw(Scene);
                Scene.EndForeground();
            }
        }
    }
}
