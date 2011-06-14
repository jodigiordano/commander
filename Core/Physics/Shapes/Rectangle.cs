namespace EphemereGames.Core.Physics
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    
    public class RectanglePhysique
    {
        public Rectangle RectanglePrimitif;

        public RectanglePhysique()
        {
            RectanglePrimitif = new Rectangle();
        }

        public RectanglePhysique(int x, int y, int width, int height)
        {
            RectanglePrimitif = new Rectangle(x, y, width, height);
        }

        public RectanglePhysique(Rectangle rectangle)
        {
            RectanglePrimitif = rectangle;
        }

        public int X
        {
            get { return RectanglePrimitif.X; }
            set { RectanglePrimitif.X = value; }
        }

        public int Y
        {
            get { return RectanglePrimitif.Y; }
            set { RectanglePrimitif.Y = value; }
        }

        public int Width
        {
            get { return RectanglePrimitif.Width; }
            set { RectanglePrimitif.Width = value; }
        }

        public int Height
        {
            get { return RectanglePrimitif.Height; }
            set { RectanglePrimitif.Height = value; }
        }

        public int Left
        {
            get { return RectanglePrimitif.Left; }
        }

        public int Right
        {
            get { return RectanglePrimitif.Right; }
        }

        public int Top
        {
            get { return RectanglePrimitif.Top; }
        }

        public int Bottom
        {
            get { return RectanglePrimitif.Bottom; }
        }

        public bool Intersects(RectanglePhysique autre)
        {
            bool resultat;

            this.RectanglePrimitif.Intersects(ref autre.RectanglePrimitif, out resultat);

            return resultat;
        }


        public bool Includes(Vector3 point)
        {
            return (point.X >= this.Left && point.X <= this.Right && point.Y >= this.Top && point.Y <= this.Bottom);
        }


        public void set(ref Rectangle rectangle)
        {
            this.RectanglePrimitif.X = rectangle.X;
            this.RectanglePrimitif.Y = rectangle.Y;
            this.RectanglePrimitif.Width = rectangle.Width;
            this.RectanglePrimitif.Height = rectangle.Height;
        }


        public void set(int x, int y, int width, int height)
        {
            RectanglePrimitif.X = x;
            RectanglePrimitif.Y = y;
            RectanglePrimitif.Width = width;
            RectanglePrimitif.Height = height;
        }


        public Vector2 pointIntersection(Line ligne)
        {
            Vector2 pointA, pointB, pointC, pointD;

            // Haut

            pointA = new Vector2(Left, Top);
            pointB = new Vector2(Right, Top);
            pointC = ligne.DebutV2;
            pointD = ligne.FinV2;

            if (Collisions.collisionLigneLigne(ref pointA, ref pointB, ref pointC, ref pointD))
                return ligne.pointIntersection(ref pointA, ref pointB);


            // Bas

            pointA.X = Left; pointA.Y = Bottom;
            pointB.X = Right; pointB.Y = Bottom;
            pointC = ligne.DebutV2;
            pointD = ligne.FinV2;

            if (Collisions.collisionLigneLigne(ref pointA, ref pointB, ref pointC, ref pointD))
                return ligne.pointIntersection(ref pointA, ref pointB);


            // Gauche

            pointA.X = Left; pointA.Y = Top;
            pointB.X = Left; pointB.Y = Bottom;
            pointC = ligne.DebutV2;
            pointD = ligne.FinV2;

            if (Collisions.collisionLigneLigne(ref pointA, ref pointB, ref pointC, ref pointD))
                return ligne.pointIntersection(ref pointA, ref pointB);


            // Gauche

            pointA.X = Right; pointA.Y = Top;
            pointB.X = Right; pointB.Y = Bottom;
            pointC = ligne.DebutV2;
            pointD = ligne.FinV2;

            if (Collisions.collisionLigneLigne(ref pointA, ref pointB, ref pointC, ref pointD))
                return ligne.pointIntersection(ref pointA, ref pointB);


            throw new Exception("ne pas appeler s'il n'y a pas lieu.");
        }
    }
}
