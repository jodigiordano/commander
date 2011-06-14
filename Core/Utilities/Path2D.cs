namespace EphemereGames.Core.Utilities
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;


    public class Path2D
    {
        private List<Microsoft.Xna.Framework.Curve> Curves;
        private double Length;


        public Path2D() : this(new List<Vector2>(), new List<double>()) { }


        public Path2D(List<Vector2> positions, List<double> gameTimes)
        {
            Curves = new List<Microsoft.Xna.Framework.Curve>(2);

            for (int i = 0; i < 2; i++)
                Curves.Add(new Microsoft.Xna.Framework.Curve() { PreLoop = Microsoft.Xna.Framework.CurveLoopType.Constant, PostLoop = Microsoft.Xna.Framework.CurveLoopType.Constant });

            Length = 0;

            Initialize(positions, gameTimes);
        }


        public void Initialize(List<Vector2> positions, List<double> gameTimes)
        {
            Initialize(positions, gameTimes, (int) Math.Min(positions.Count, gameTimes.Count));
        }


        public void Initialize(List<Vector2> positions, List<double> gameTimes, int qty)
        {
            for (int i = 0; i < Curves.Count; i++)
            {
                Microsoft.Xna.Framework.Curve c = Curves[i];

                c.Keys.Clear();

                for (int j = 0; j < qty; j++)
                    c.Keys.Add(new Microsoft.Xna.Framework.CurveKey((float) gameTimes[j], (i == 0) ? positions[j].X : positions[j].Y));
            }

            for (int j = 0; j < Curves.Count; j++)
                Curves[j].ComputeTangents(Microsoft.Xna.Framework.CurveTangent.Smooth);

            Length = (qty > 0) ? gameTimes[qty - 1] : 0;
        }


        public Vector2 positionDepart()
        {
            return new Vector2(
                Curves[0].Evaluate(0.0f),
                Curves[1].Evaluate(0.0f)
            );
        }

        public Vector2 positionFin()
        {
            return new Vector2(
                Curves[0].Keys[Curves[0].Keys.Count - 1].Value,
                Curves[1].Evaluate(Curves[0].Keys[Curves[0].Keys.Count - 1].Value)
            );
        }


        public Vector2 position(double temps)
        {
            return new Vector2( // On ajoute un tick au temps (si tu ne comprends pas pourquoi, demande-moi, car j'ai pas le goût d'écrire l'explication ici lol)
                Curves[0].Evaluate((float) temps + 1000 / 60.0f),
                Curves[1].Evaluate((float) temps + 1000 / 60.0f)
            );
        }


        public Vector2 positionRelative(double temps)
        {
            return new Vector2(
                Curves[0].Evaluate((float) temps) - Curves[0].Evaluate(0f),
                Curves[1].Evaluate((float) temps) - Curves[1].Evaluate(0f)
            );
        }


        public float rotation(double temps) {
            Vector2 directionActuelle = direction(temps);

            return (MathHelper.PiOver2) + (float)Math.Atan2(directionActuelle.Y, directionActuelle.X);
        }


        public Vector2 direction(double temps)
        {
            Vector2 positionAvant = position(temps);
            Vector2 positionApres = position(temps + 1);

            Vector2 direction = positionApres - positionAvant;

            direction.Normalize();

            return direction;
        }


        public static Path2D CreerVitesse(EphemereGames.Core.Utilities.Path3D.CurveType type, double time)
        {
            Path2D path = new Path2D();

            switch (type)
            {
                case EphemereGames.Core.Utilities.Path3D.CurveType.Linear:
                    path.Initialize(new List<Vector2>() { Vector2.Zero, Vector2.One }, new List<double> { 0, time });
                    break;

                case EphemereGames.Core.Utilities.Path3D.CurveType.Exponential:
                    path.Initialize(new List<Vector2>() { Vector2.Zero, new Vector2(0.8f, 0.1f), Vector2.One }, new List<double> { 0, time / 2, time });
                    break;

                case EphemereGames.Core.Utilities.Path3D.CurveType.Log:
                    path.Initialize(new List<Vector2>() { Vector2.Zero, new Vector2(0.1f, 0.8f), Vector2.One }, new List<double> { 0, time / 2, time });
                    break;
            }

            return path;
        }
    }
}
