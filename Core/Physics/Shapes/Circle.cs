namespace EphemereGames.Core.Physics
{
    using Microsoft.Xna.Framework;
    

    public class Circle
    {
        public float Radius { get; set; }
        private IObjetPhysique Obj;
        public Vector3 Position;


        public Circle(Vector3 position, float radius)
        {
            Position = position;
            Radius = radius;
        }


        public Circle(IObjetPhysique objet, float radius)
        {
            Obj = objet;
            Position = objet.Position;
            Radius = radius;
        }


        public bool Intersects(PhysicalRectangle rectangle)
        {
            if (Obj != null)
                Position = Obj.Position;

            // centre du cercle dans l'axe des X de rectangle (dessous, dessus)
            if (this.Position.X >= rectangle.Left && this.Position.X <= rectangle.Right &&
                this.Position.Y < rectangle.Bottom + Radius && this.Position.Y > rectangle.Top - Radius)
                return true;

            float r2 = Radius * Radius;
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
               (this.Position.X > rectangle.Left - Radius && this.Position.X < rectangle.Right + Radius))
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
            if (Obj != null)
                Position = Obj.Position;

            //return Math.Pow((point.X - this.Position.X), 2) + Math.Pow((point.Y - this.Position.Y), 2) < Math.Pow(Rayon, 2);
            float posX = point.X - Position.X;
            posX *= posX;
            float posY = point.Y - Position.Y;
            posY *= posY;
            
            return posX + posY < Radius * Radius;
        }


        public bool Intersects(ref Vector3 point)
        {
            if (Obj != null)
                Position = Obj.Position;

            //return Math.Pow((point.X - this.Position.X), 2) + Math.Pow((point.Y - this.Position.Y), 2) < Math.Pow(Rayon, 2);
            float posX = point.X - Position.X;
            posX *= posX;
            float posY = point.Y - Position.Y;
            posY *= posY;

            return posX + posY < Radius * Radius;
        }


        public bool Intersects(Circle autre)
        {
            if (Obj != null)
                Position = Obj.Position;

            if (autre.Obj != null)
                autre.Position = autre.Obj.Position;

            float distanceCollision = this.Radius + autre.Radius;
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


        public bool Outside(Vector3 point)
        {
            if (this.Position.Z != point.Z)
                return false;

            return !Intersects(ref point);
        }


        public Vector2 pointPlusProcheCirconference(ref Vector2 point)
        {
            Vector2 centreToPoint = new Vector2(point.X - Position.X, point.Y - Position.Y);
            float longueur = centreToPoint.Length();

            return new Vector2(this.Position.X + centreToPoint.X / longueur * Radius, this.Position.Y + centreToPoint.Y / longueur * Radius);
        }


        public Vector3 NearestPointToCircumference(Vector3 point)
        {
            return NearestPointToCircumference(ref point);
        }


        public Vector3 NearestPointToCircumference(ref Vector3 point)
        {
            Vector2 centreToPoint = new Vector2(point.X - Position.X, point.Y - Position.Y);
            float longueur = centreToPoint.Length();

            return new Vector3(
                this.Position.X + centreToPoint.X / longueur * Radius,
                this.Position.Y + centreToPoint.Y / longueur * Radius,
                point.Z);
        }


        public Rectangle Rectangle
        {
            get
            {
                return new Rectangle((int) (this.Position.X - this.Radius), (int) (this.Position.Y - this.Radius), (int) (this.Radius * 2), (int) (this.Radius * 2));
            }
        }
    }
}