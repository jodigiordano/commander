namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using EphemereGames.Core.Utilities;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    public class AnimationTransitionAlienMothership : Animation, ITransitionAnimation
    {
        public bool In;

        private Mothership Mothership;
        private Path2D PathIn;
        private Path2D PathOut;

        private static KeyValuePair<Vector2, Vector2> PositionOut =
            new KeyValuePair<Vector2, Vector2>(new Vector2(0, -2000), new Vector2(0, 0));

        private static KeyValuePair<Vector2, Vector2> PositionIn =
                new KeyValuePair<Vector2, Vector2>(new Vector2(0, 0), new Vector2(0, 2000));


        public AnimationTransitionAlienMothership(double length, double visualPriority)
            : base(length, visualPriority)
        {
            In = false;

            Mothership = new Mothership(VisualPriority)
            {
                Blend = BlendType.Substract,
                SizeX = 12
            };

            PathIn = new Path2D(new List<Vector2>() { PositionIn.Key, PositionIn.Value }, new List<double>() { 0, Length });
            PathOut = new Path2D(new List<Vector2>() { PositionOut.Key, PositionOut.Value }, new List<double>() { 0, Length });
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
            Mothership.Position = new Vector3(In ? PathIn.GetPosition(ElapsedTime) : PathOut.GetPosition(ElapsedTime), 0);

            Scene.BeginForeground();
            Mothership.Draw(Scene);
            Scene.EndForeground();
        }
    }
}
