//=============================================================================
//
// 
//
//=============================================================================

namespace Core.Physique
{
    using Microsoft.Xna.Framework;
    using System;

    public class Ligne
    {

        //=============================================================================
        // Attributs
        //=============================================================================

        public Vector3 Debut;
        public Vector3 Fin;


        //=============================================================================
        // Constructeurs
        //=============================================================================

        public Ligne(Vector2 Debut, Vector2 Fin)
        {
            this.Debut = new Vector3(Debut, 0);
            this.Fin = new Vector3(Fin, 0);
        }


        public Ligne(Vector3 Debut, Vector3 Fin)
        {
            this.Debut = Debut;
            this.Fin = Fin;
        }


        //=============================================================================
        // Services
        //=============================================================================

        public Vector2 DebutV2
        {
            get { return new Vector2(Debut.X, Debut.Y); }
        }


        public Vector2 FinV2
        {
            get { return new Vector2(Fin.X, Fin.Y); }
        }


        public float Longueur
        {
            get
            {
                return (Fin - Debut).Length();
            }
        }


        public Vector3 pointPlusProche(Vector3 point)
        {
            double xDelta = Fin.X - Debut.X;
            double yDelta = Fin.Y - Debut.Y;

            if (xDelta == 0 && yDelta == 0)
                return point;

            double u = ((point.X - Debut.X) * xDelta + (point.Y - Debut.Y) * yDelta) / (xDelta * xDelta + yDelta * yDelta);

            return (u < 0) ? Debut :
                   (u > 1) ? Fin :
                   new Vector3((float) (Debut.X + u * xDelta), (float) (Debut.Y + u * yDelta), Debut.Z);
        }


        public static void PointPlusProche(ref Vector3 ligneDebut, ref Vector3 ligneFin, ref Vector3 point, ref Vector3 resultat)
        {
            double xDelta = ligneFin.X - ligneDebut.X;
            double yDelta = ligneFin.Y - ligneDebut.Y;
            double zDelta = ligneFin.Z - ligneDebut.Z;

            if (xDelta == 0 && yDelta == 0)
            {
                resultat.X = point.X;
                resultat.Y = point.Y;
                resultat.Z = point.Z;

                return;
            }

            double u = ((point.X - ligneDebut.X) * xDelta + (point.Y - ligneDebut.Y) * yDelta) / (xDelta * xDelta + yDelta * yDelta);

            if (u < 0)
            {
                resultat.X = ligneDebut.X;
                resultat.Y = ligneDebut.Y;
                resultat.Z = ligneDebut.Z;

                return;
            }

            if (u > 1)
            {
                resultat.X = ligneFin.X;
                resultat.Y = ligneFin.Y;
                resultat.Z = ligneFin.Z;

                return;
            }


            resultat.X = (float)(ligneDebut.X + u * xDelta);
            resultat.Y = (float)(ligneDebut.Y + u * yDelta);
            resultat.Z = (float)(ligneDebut.Z + u * zDelta);
        }


        public double distanceDuPointSquared(Vector3 point)
        {
            return (point - pointPlusProche(point)).LengthSquared();
        }


        public void directionRelative(float radians, ref Vector3 resultat)
        {
          Vector3 ligne = Fin - Debut;
          Vector3 direction;
          Matrix matriceRotation;

          Matrix.CreateRotationZ(-(MathHelper.PiOver2 + MathHelper.PiOver2), out matriceRotation);
          Vector3.Transform(ref ligne, ref matriceRotation, out direction);

          direction.Normalize();

          resultat = direction;
        }


        public Vector2 pointIntersection(Ligne autreLigne)
        {
            float slopOfA = getSlop();
            float bOfA = getB();

            float slopOfB = autreLigne.getSlop();
            float bOfB = autreLigne.getB();

            float slopSum = slopOfA - slopOfB;
            float bSum = bOfB - bOfA;

            float x = bSum / slopSum;
            float y = slopOfB * x + bOfB;

            return new Vector2(x, y);
        }

        public Vector2 pointIntersection(ref Vector2 autreDebut, ref Vector2 autreFin)
        {
            float slopOfA = getSlop();
            float bOfA = getB();

            float slopOfB = (autreDebut.Y - autreFin.Y) / (autreDebut.X - autreFin.X);
            float bOfB = autreDebut.Y - (autreDebut.X * slopOfB);

            float slopSum = slopOfA - slopOfB;
            float bSum = bOfB - bOfA;

            float x = bSum / slopSum;
            float y = slopOfB * x + bOfB;

            return new Vector2(x, y);
        }


        public float getSlop()
        {
            return (Debut.Y - Fin.Y) / (Debut.X - Fin.X);
        }


        public float getB()
        {
            float slop = getSlop();

            return Debut.Y - (Debut.X * slop);
        }
    }
}
