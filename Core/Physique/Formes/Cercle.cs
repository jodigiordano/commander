namespace Core.Physique
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    
    public class Cercle
    {
        //=================================================================
        // Variables d'instances
        //=================================================================

        public float Rayon { get; set; }                    // Rayon du cercle
        private IObjetPhysique objet;                       // Objet de référence pour maj automatique de la position
        public Vector3 Position;


        //=================================================================
        //  Constructeurs
        //=================================================================

        public Cercle(Vector3 position, float rayon)
        {
            this.Position.X = position.X;
            this.Position.Y = position.Y;
            this.Position.Z = position.Z;
            this.Rayon = rayon;
        }


        public Cercle(IObjetPhysique objet, float rayon)
        {
            this.objet = objet;
            Position.X = objet.Position.X;
            Position.Y = objet.Position.Y;
            Position.Z = objet.Position.Z;
            this.Rayon = rayon;
        }


        //=================================================================
        // Services
        //=================================================================

        public bool Intersects(RectanglePhysique rectangle)
        {
            if (objet != null)
            {
                Position.X = objet.Position.X;
                Position.Y = objet.Position.Y;
                Position.Z = objet.Position.Z;
            }

            // centre du cercle dans l'axe des X de rectangle (dessous, dessus)
            if (this.Position.X >= rectangle.Left && this.Position.X <= rectangle.Right &&
                this.Position.Y < rectangle.Bottom + Rayon && this.Position.Y > rectangle.Top - Rayon)
                return true;

            float r2 = Rayon * Rayon;
            float val1 = Position.X - rectangle.Left;
            val1 *= val1;
            float val2 = Position.Y - rectangle.Top;
            val2 *= val2;
            // centre du cercle dans les coins à gauche de rectangle
            if (this.Position.Y < rectangle.Top && (val1 + val2 < r2))
                return true;

            val2 = Position.Y - rectangle.Bottom;
            val2 *= val2;
            if (this.Position.Y > rectangle.Bottom && (val1 + val2 < r2))
                return true;

            // centre du cercle dans l'axe des Y de rectangle (gauche, droite)
            if ((this.Position.Y >= rectangle.Top && this.Position.Y <= rectangle.Bottom) &&
               (this.Position.X > rectangle.Left - Rayon && this.Position.X < rectangle.Right + Rayon))
                return true;

            val1 = Position.X - rectangle.Right;
            val1 *= val1;
            float val3 = Position.Y - rectangle.Top;
            val3 *= val3;
            // centre du cercle dans les coins à droite de rectangle
            if ((this.Position.X > rectangle.Right && this.Position.Y < rectangle.Top) && (val1 + val3 < r2))
                return true;

            if ((this.Position.X > rectangle.Right && this.Position.Y > rectangle.Bottom) && (val1 + val2 < r2))
                return true;

            // pas de collision
            return false;
        }


        public bool Intersects(ref Vector2 point)
        {
            if (objet != null)
            {
                Position.X = objet.Position.X;
                Position.Y = objet.Position.Y;
                Position.Z = objet.Position.Z;
            }

            //return Math.Pow((point.X - this.Position.X), 2) + Math.Pow((point.Y - this.Position.Y), 2) < Math.Pow(Rayon, 2);
            float posX = point.X - Position.X;
            posX *= posX;
            float posY = point.Y - Position.Y;
            posY *= posY;
            
            return posX + posY < Rayon * Rayon;
        }


        public bool Intersects(ref Vector3 point)
        {
            if (objet != null)
            {
                Position.X = objet.Position.X;
                Position.Y = objet.Position.Y;
                Position.Z = objet.Position.Z;
            }

            //return Math.Pow((point.X - this.Position.X), 2) + Math.Pow((point.Y - this.Position.Y), 2) < Math.Pow(Rayon, 2);
            float posX = point.X - Position.X;
            posX *= posX;
            float posY = point.Y - Position.Y;
            posY *= posY;

            return posX + posY < Rayon * Rayon;
        }


        public bool Intersects(Cercle autre)
        {
            float distanceCollision = this.Rayon + autre.Rayon;
            float distanceX = this.Position.X - autre.Position.X;
            float distanceY = this.Position.Y - autre.Position.Y;

            return (distanceCollision * distanceCollision > (distanceX * distanceX) + (distanceY * distanceY));
        }


        public bool Outside(ref Vector2 point)
        {
            return !Intersects(ref point);
        }


        public bool Outside(ref Vector3 point)
        {
            if (this.Position.Z != point.Z)
                return false;

            return !Intersects(ref point);
        }


        public Vector2 pointPlusProcheCirconference(ref Vector2 point)
        {
            Vector2 centreToPoint = new Vector2(point.X - Position.X, point.Y - Position.Y);
            float longueur = centreToPoint.Length();

            return new Vector2(this.Position.X + centreToPoint.X / longueur * Rayon, this.Position.Y + centreToPoint.Y / longueur * Rayon);
        }


        public Rectangle Rectangle
        {
            get
            {
                return new Rectangle((int) (this.Position.X - this.Rayon), (int) (this.Position.Y - this.Rayon), (int) (this.Rayon * 2), (int) (this.Rayon * 2));
            }
        }
    }
}