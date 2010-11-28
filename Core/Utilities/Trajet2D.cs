//=====================================================================
//
// Un trajet qui est défini par un ensemble de points (Vecteur2D)
// et les temps (un autre ensemble...) où l'on passe par ces points.
// Habituellement, on passe par le premier point au temps 0.
//
// Note : ce qui n'est pas (encore) défini : ce qui se passe lorsque
// l'on demande une position à l'extérieur des bornes du trajet
// (avant ou après). XNA propose une manière de le définir si
// nécessaire.
//
//=====================================================================

namespace Core.Utilities
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;

    [Serializable()] 
    public class Trajet2D
    {

        //=====================================================================
        // Attributs
        //=====================================================================

        public Curve TrajetX { get; set; }
        public Curve TrajetY { get; set; }

        private KeyValuePair<double[], Vector2[]> positionsTemps;
        private CurveTangent tangeantes = CurveTangent.Smooth;

        private bool recalculerTangeantes = false;
        private double tempsRecalculerTangeantes = 0;
        private CurveTangent typeRecalculerTangeantes = CurveTangent.Linear;


        //=====================================================================
        // Constructeur
        //=====================================================================

        public Trajet2D()
        {
            TrajetX = new Curve();
            TrajetY = new Curve();
            tangeantes = CurveTangent.Smooth;
        }

        public Trajet2D(Vector2[] positions, double[] gameTimes) {
            TrajetX = new Curve();
            TrajetY = new Curve();

            for (int i = 0; i < positions.Length; i++) {
                TrajetX.Keys.Add(new CurveKey((float)gameTimes[i], positions[i].X));
                TrajetY.Keys.Add(new CurveKey((float)gameTimes[i], positions[i].Y));
            }

            TrajetX.ComputeTangents(tangeantes);
            TrajetY.ComputeTangents(tangeantes);

            TrajetX.PostLoop = CurveLoopType.Linear;
            TrajetX.PreLoop = CurveLoopType.Linear;
            TrajetY.PostLoop = CurveLoopType.Linear;
            TrajetY.PreLoop = CurveLoopType.Linear;
        }


        //=====================================================================
        // Getters / Setters
        //=====================================================================

        [ContentSerializerIgnore]
        public KeyValuePair<double[], Vector2[]> PositionsTemps
        {
            get { return positionsTemps; }
            set
            {
                positionsTemps = value;

                for (int i = 0; i < value.Key.Length; i++)
                {
                    TrajetX.Keys.Add(new CurveKey((float)value.Key[i], value.Value[i].X));
                    TrajetY.Keys.Add(new CurveKey((float)value.Key[i], value.Value[i].Y));
                }

                TrajetX.ComputeTangents(tangeantes);
                TrajetY.ComputeTangents(tangeantes);

                TrajetX.PostLoop = CurveLoopType.Linear;
                TrajetX.PreLoop = CurveLoopType.Linear;
                TrajetY.PostLoop = CurveLoopType.Linear;
                TrajetY.PreLoop = CurveLoopType.Linear;
            }
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
            }
        }


        //
        // Position de départ sur le trajet
        //

        public Vector2 positionDepart()
        {
            return new Vector2(
                TrajetX.Evaluate(0.0f),
                TrajetY.Evaluate(0.0f)
            );
        }

        public Vector2 positionFin()
        {
            return new Vector2(
                TrajetX.Keys[TrajetX.Keys.Count - 1].Value,
                TrajetY.Evaluate(TrajetX.Keys[TrajetX.Keys.Count - 1].Value)
            );
        }


        //
        // Position sur le trajet selon un certain +temps+
        //

        public Vector2 position(double temps)
        {
            if (recalculerTangeantes && temps >= tempsRecalculerTangeantes)
            {
                Tangentes = typeRecalculerTangeantes;
                recalculerTangeantes = false;
            }

            return new Vector2( // On ajoute un tick au temps (si tu ne comprends pas pourquoi, demande-moi, car j'ai pas le goût d'écrire l'explication ici lol)
                TrajetX.Evaluate((float)temps + 1000 / 60.0f),
                TrajetY.Evaluate((float)temps + 1000 / 60.0f)
            );
        }

        //
        // Position relative sur le trajet (par rapport à l'origine du trajet) selon un certain +temps+
        //

        public Vector2 positionRelative(double temps)
        {
            return new Vector2(
                TrajetX.Evaluate((float)temps) - TrajetX.Evaluate(0f),
                TrajetY.Evaluate((float)temps) - TrajetY.Evaluate(0f)
            );
        }


        //
        // Rotation nécessaire pour pointer en direction du trajet (en radians)
        //

        public float rotation(double temps) {
            Vector2 directionActuelle = direction(temps);

            return (MathHelper.PiOver2) + (float)Math.Atan2(directionActuelle.Y, directionActuelle.X);
        }


        //
        // Vecteur qui pointe en direction du trajet
        //

        public Vector2 direction(double temps)
        {
            Vector2 positionAvant = position(temps);
            Vector2 positionApres = position(temps + 1);

            Vector2 direction = positionApres - positionAvant;

            direction.Normalize();

            return direction;
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

        public static Trajet2D CreerVitesse(Type type, double temps)
        {
            Trajet2D trajet = new Trajet2D();

            switch (type)
            {
                case Type.Lineaire:
                    trajet.PositionsTemps = new KeyValuePair<double[], Vector2[]>(
                        new double[] {0, temps},
                        new Vector2[] {new Vector2(0, 0), new Vector2(1, 1)}
                    );
                    break;

                case Type.Exponentiel:
                    trajet.PositionsTemps = new KeyValuePair<double[], Vector2[]>(
                        new double[] { 0, temps / 2, temps },
                        new Vector2[] { new Vector2(0, 0), new Vector2(0.8f, 0.1f), new Vector2(1, 1) }
                    );
                    break;

                case Type.Logarithmique:
                    trajet.PositionsTemps = new KeyValuePair<double[], Vector2[]>(
                        new double[] { 0, temps / 2, temps },
                        new Vector2[] { new Vector2(0, 0), new Vector2(0.1f, 0.8f), new Vector2(1, 1) }
                    );
                    break;
            }

            return trajet;
        }
    }
}
