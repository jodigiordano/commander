namespace EphemereGames.Commander
{
    using Microsoft.Xna.Framework;


    class LuneMatrice : Lune
    {
        public LuneMatrice(Simulation simulation, CorpsCeleste corpsCeleste, int alpha)
            : base(simulation, corpsCeleste, alpha)
        {
            PositionRelative = new Vector3(corpsCeleste.Circle.Radius + Main.Random.Next(10, 30), 0, 0);

            Matrix.CreateRotationZ((float)(MathHelper.TwoPi / TempsRotationActuel), out MatriceRotation);
            Vector3.Transform(ref PositionRelative, ref MatriceRotation, out Position);
            Vector3.Add(ref Position, ref CorpsCeleste.position, out Position);
        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            float enRadians = (float) (TempsRotationActuel * (MathHelper.TwoPi / TempsRotation));

            Matrix.CreateRotationZ(enRadians, out MatriceRotation);
            Vector3.Transform(ref PositionRelative, ref MatriceRotation, out Position);
            Vector3.Add(ref Position, ref CorpsCeleste.position, out Position);
        }
    }
}
