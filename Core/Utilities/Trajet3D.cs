namespace EphemereGames.Core.Utilities
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;

    [Serializable()] 
    public class Trajet3D
    {
        public Curve TrajetX;
        public Curve TrajetY;
        public Curve TrajetZ;

        private CurveTangent tangeantes = CurveTangent.Smooth;

        private bool recalculerTangeantes = false;
        private double tempsRecalculerTangeantes = 0;
        private CurveTangent typeRecalculerTangeantes = CurveTangent.Linear;

        private Vector3 _tmp1, _tmp2;
        private double MaxTime;


        public Trajet3D() : this(new List<Vector3>(), new List<double>()) { }


        public Trajet3D(List<Vector3> positions, List<double> gameTimes)
        {
            TrajetX = new Curve();
            TrajetY = new Curve();
            TrajetZ = new Curve();
            tangeantes = CurveTangent.Smooth;
            MaxTime = 0;

            Initialize(positions, gameTimes);
        }


        public void Initialize(List<Vector3> positions, List<double> gameTimes)
        {
            Initialize(positions, gameTimes, (int) Math.Min(positions.Count, gameTimes.Count));
        }


        public void Initialize(List<Vector3> positions, List<double> gameTimes, int qty)
        {
            TrajetX.Keys.Clear();
            TrajetY.Keys.Clear();
            TrajetZ.Keys.Clear();

            for (int i = 0; i < qty; i++)
            {
                TrajetX.Keys.Add(new CurveKey((float) gameTimes[i], positions[i].X));
                TrajetY.Keys.Add(new CurveKey((float) gameTimes[i], positions[i].Y));
                TrajetZ.Keys.Add(new CurveKey((float) gameTimes[i], positions[i].Z));
            }

            TrajetX.ComputeTangents(tangeantes);
            TrajetY.ComputeTangents(tangeantes);
            TrajetZ.ComputeTangents(tangeantes);

            TrajetX.PostLoop = CurveLoopType.Linear;
            TrajetX.PreLoop = CurveLoopType.Linear;
            TrajetY.PostLoop = CurveLoopType.Linear;
            TrajetY.PreLoop = CurveLoopType.Linear;
            TrajetZ.PostLoop = CurveLoopType.Linear;
            TrajetZ.PreLoop = CurveLoopType.Linear;

            MaxTime = (qty > 0) ? gameTimes[qty - 1] : 0;
        }


        //
        // Pre et Post Loop
        //

        [ContentSerializer(Optional = true)]
        public CurveLoopType PostLoop
        {
            get { return TrajetX.PostLoop; }
            set
            {
                TrajetX.PostLoop = value;
                TrajetY.PostLoop = value;
                TrajetZ.PostLoop = value;
            }
        }

        [ContentSerializer(Optional = true)]
        public CurveLoopType PreLoop
        {
            get { return TrajetX.PreLoop; }
            set
            {
                TrajetX.PreLoop = value;
                TrajetY.PreLoop = value;
                TrajetY.PreLoop = value;
            }
        }


        //
        // Setter des tangentes droites
        //

        [ContentSerializer(Optional = true)]
        public CurveTangent Tangentes
        {
            get { return tangeantes; }
            set
            {
                tangeantes = value;

                TrajetX.ComputeTangents(tangeantes);
                TrajetY.ComputeTangents(tangeantes);
                TrajetZ.ComputeTangents(tangeantes);
            }
        }


        //
        // Position de départ sur le trajet
        //

        public void PositionDepart(ref Vector3 resultat)
        {
            resultat.X = TrajetX.Evaluate(0.0f);
            resultat.Y = TrajetY.Evaluate(0.0f);
            resultat.Z = TrajetZ.Evaluate(0.0f);
        }


        //
        // Position sur le trajet selon un certain +temps+
        //

        public void Position(double temps, ref Vector3 resultat)
        {
            if (recalculerTangeantes && temps >= tempsRecalculerTangeantes)
            {
                Tangentes = typeRecalculerTangeantes;
                recalculerTangeantes = false;
            }

            resultat.X = TrajetX.Evaluate((float)temps);
            resultat.Y = TrajetY.Evaluate((float)temps);
            resultat.Z = TrajetZ.Evaluate((float)temps);
        }


        public float Pourc(double deplacement)
        {
            return (MaxTime != 0) ? (float) (deplacement / MaxTime) : 0;
        }


        //
        // Position relative sur le trajet (par rapport à l'origine du trajet) selon un certain +temps+
        //

        public void PositionRelative(double temps, ref Vector3 resultat)
        {
            resultat.X = TrajetX.Evaluate((float)temps) - TrajetX.Evaluate(0f);
            resultat.Y = TrajetY.Evaluate((float)temps) - TrajetY.Evaluate(0f);
            resultat.Z = TrajetZ.Evaluate((float)temps) - TrajetZ.Evaluate(0f);
        }


        //
        // Rotation nécessaire pour pointer en direction du trajet (en radians)
        //

        public float rotation(double temps) {
            Direction(temps, out _tmp1);

            return (MathHelper.PiOver2) + (float)Math.Atan2(_tmp1.Y, _tmp1.X);
        }


        //
        // Vecteur qui pointe en direction du trajet
        //

        public void Direction(double temps, out Vector3 resultat)
        {
            Position(temps, ref _tmp1);
            Position(temps + 1, ref _tmp2);

            Vector3.Subtract(ref _tmp2, ref _tmp1, out resultat);

            resultat.Normalize();
        }


        //
        // Recalculer les tangeantes plus tard
        //

        public void recalculerTangentes(CurveTangent type, double temps)
        {
            recalculerTangeantes = true;
            tempsRecalculerTangeantes = temps;
            typeRecalculerTangeantes = type;
        }


        //
        // Calcul d'une tangeante particulière
        //

        public void calculerTangente(int position, CurveTangent tangeante)
        {
            TrajetX.ComputeTangent(position, tangeante);
            TrajetY.ComputeTangent(position, tangeante);
            TrajetZ.ComputeTangent(position, tangeante);
        }


        //=====================================================================
        // Factory
        //=====================================================================

        public enum Type
        {
            Lineaire,
            Exponentiel,
            Logarithmique
        }

        public static Trajet3D CreerVitesse(Type type, double temps)
        {
            Trajet3D trajet = new Trajet3D();

            switch (type)
            {
                case Type.Lineaire:
                    trajet.Initialize(new List<Vector3>() { Vector3.Zero, Vector3.One }, new List<double> { 0 , temps });
                    break;

                case Type.Exponentiel:
                    trajet.Initialize(new List<Vector3>() { Vector3.Zero, new Vector3(0.8f, 0.1f, 0.1f), Vector3.One }, new List<double> { 0, temps / 2, temps });
                    break;

                case Type.Logarithmique:
                    trajet.Initialize(new List<Vector3>() { Vector3.Zero, new Vector3(0.1f, 0.8f, 0.8f), Vector3.One }, new List<double> { 0, temps / 2, temps });
                    break;
            }

            return trajet;
        }
    }
}
