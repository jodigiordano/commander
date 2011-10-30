namespace EphemereGames.Core.Physics
{
    using Microsoft.Xna.Framework;


    public class Line
    {
        public Vector3 Start;
        public Vector3 End;


        public Line(Vector2 start, Vector2 end)
        {
            Start = new Vector3(start, 0);
            End = new Vector3(end, 0);
        }


        public Line(Vector3 start, Vector3 end)
        {
            Start = start;
            End = end;
        }


        public Vector2 StartV2
        {
            get { return new Vector2(Start.X, Start.Y); }
        }


        public Vector2 EndV2
        {
            get { return new Vector2(End.X, End.Y); }
        }


        public float Length
        {
            get
            {
                return (End - Start).Length();
            }
        }


        public Vector3 NearestPoint(Vector3 point)
        {
            double xDelta = End.X - Start.X;
            double yDelta = End.Y - Start.Y;

            if (xDelta == 0 && yDelta == 0)
                return point;

            double u = ((point.X - Start.X) * xDelta + (point.Y - Start.Y) * yDelta) / (xDelta * xDelta + yDelta * yDelta);

            return (u < 0) ? Start :
                   (u > 1) ? End :
                   new Vector3((float) (Start.X + u * xDelta), (float) (Start.Y + u * yDelta), Start.Z);
        }


        public static void NearestPoint(ref Vector3 start, ref Vector3 end, ref Vector3 point, ref Vector3 result)
        {
            double xDelta = end.X - start.X;
            double yDelta = end.Y - start.Y;
            double zDelta = end.Z - start.Z;

            if (xDelta == 0 && yDelta == 0)
            {
                result.X = point.X;
                result.Y = point.Y;
                result.Z = point.Z;

                return;
            }

            double u = ((point.X - start.X) * xDelta + (point.Y - start.Y) * yDelta) / (xDelta * xDelta + yDelta * yDelta);

            if (u < 0)
            {
                result.X = start.X;
                result.Y = start.Y;
                result.Z = start.Z;

                return;
            }

            if (u > 1)
            {
                result.X = end.X;
                result.Y = end.Y;
                result.Z = end.Z;

                return;
            }


            result.X = (float)(start.X + u * xDelta);
            result.Y = (float)(start.Y + u * yDelta);
            result.Z = (float)(start.Z + u * zDelta);
        }


        public double DistanceFromPointSquared(Vector3 point)
        {
            return (point - NearestPoint(point)).LengthSquared();
        }


        public void RelativeDirection(float radians, ref Vector3 resultat)
        {
          Vector3 ligne = End - Start;
          Vector3 direction;
          Matrix matriceRotation;

          Matrix.CreateRotationZ(-(MathHelper.PiOver2 + MathHelper.PiOver2), out matriceRotation);
          Vector3.Transform(ref ligne, ref matriceRotation, out direction);

          direction.Normalize();

          resultat = direction;
        }


        public Vector2 IntersectPoint(Line other)
        {
            float slopOfA = GetSlop();
            float bOfA = GetB();

            float slopOfB = other.GetSlop();
            float bOfB = other.GetB();

            float slopSum = slopOfA - slopOfB;
            float bSum = bOfB - bOfA;

            float x = bSum / slopSum;
            float y = slopOfB * x + bOfB;

            return new Vector2(x, y);
        }


        public Vector2 IntersectPoint(ref Vector2 otherStart, ref Vector2 otherEnd)
        {
            float slopOfA = GetSlop();
            float bOfA = GetB();

            float slopOfB = (otherStart.Y - otherEnd.Y) / (otherStart.X - otherEnd.X);
            float bOfB = otherStart.Y - (otherStart.X * slopOfB);

            float slopSum = slopOfA - slopOfB;
            float bSum = bOfB - bOfA;

            float x = bSum / slopSum;
            float y = slopOfB * x + bOfB;

            return new Vector2(x, y);
        }


        private float GetSlop()
        {
            return (Start.Y - End.Y) / (Start.X - End.X);
        }


        private float GetB()
        {
            float slop = GetSlop();

            return Start.Y - (Start.X * slop);
        }
    }
}
