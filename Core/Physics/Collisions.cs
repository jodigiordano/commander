namespace EphemereGames.Core.Physics
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;


    class Collisions
    {
        private enum LineDirection
        {
            Clockwise,
            CounterClockwise,
            Line
        }


        public static bool LineLineCollision(
            ref Vector2 line1Start, ref Vector2 line1End,
            ref Vector2 line2Start, ref Vector2 line2End)
        {
            LineDirection test1a, test1b, test2a, test2b;


            test1a = GetLineDirection(ref line1Start, ref line1End, ref line2Start);
            test1b = GetLineDirection(ref line1Start, ref line1End, ref line2End);

            if (test1a != test1b)
            {

                test2a = GetLineDirection(ref line2Start, ref line2End, ref line1Start);
                test2b = GetLineDirection(ref line2Start, ref line2End, ref line1End);

                if (test2a != test2b)
                    return true;
            }

            return false;
        }


        public static bool LineRectangleCollision(ref Vector2 lineStart, ref Vector2 lineEnd, PhysicalRectangle rectangle)
        {
            Vector2 v1 = new Vector2(rectangle.X, rectangle.Y);
            Vector2 v2 = new Vector2(rectangle.X, rectangle.Y + rectangle.Height);
            Vector2 v3 = new Vector2(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height);
            Vector2 v4 = new Vector2(rectangle.X + rectangle.Width, rectangle.Y);

            if (
             LineLineCollision(ref lineStart, ref lineEnd, ref v1, ref v2) ||
             LineLineCollision(ref lineStart, ref lineEnd, ref v1, ref v4) ||
             LineLineCollision(ref lineStart, ref lineEnd, ref v4, ref v3) ||
             LineLineCollision(ref lineStart, ref lineEnd, ref v2, ref v3))
                return true;

            return false;
        }


        public static bool LineRectangleCollision(Line line, PhysicalRectangle rectangle)
        {
            Vector2 debut = line.DebutV2;
            Vector2 fin = line.FinV2;

            return LineRectangleCollision(ref debut, ref fin, rectangle);
        }


        public static bool TriangleRectangleCollision(Triangle triangle, PhysicalRectangle rectangle)
        {
            Vector2 s1 = triangle.Sommet1;
            Vector2 s2 = triangle.Sommet2;
            Vector2 s3 = triangle.Sommet3;

            if (LineRectangleCollision(ref s1, ref s2, rectangle) ||
                LineRectangleCollision(ref s2, ref s3, rectangle) ||
                LineRectangleCollision(ref s3, ref s1, rectangle))
                return true;

            return false;
        }


        public static bool PointRectangleCollision(ref Vector2 point, PhysicalRectangle rectangle)
        {
            return (point.X >= rectangle.Left && point.X <= rectangle.Right &&
                    point.Y >= rectangle.Top && point.Y <= rectangle.Bottom);
        }


        public static bool CircleRectangleCollision(Circle circle, PhysicalRectangle rectangle)
        {
            return circle.Intersects(rectangle);
        }


        public static bool RectangleRectangleCollision(PhysicalRectangle rectangle1, PhysicalRectangle rectangle2)
        {
            return rectangle1.Intersects(rectangle2);
        }


        public static bool CircleCircleCollision(Circle circle1, Circle circle2)
        {
            return circle1.Intersects(circle2);
        }


        public static bool PointCircleCollision(ref Vector3 point, Circle circle)
        {
            return circle.Intersects(ref point);
        }


        public static bool PixelCollision(
            ref Matrix transform1, Texture2D texture1, ref Color[] textureData1,
            ref Matrix transform2, Texture2D texture2, ref Color[] textureData2) {
            
            // Calculate a matrix which transforms from A's local space into
            // world space and then into B's local space
            Matrix transformAToB = transform1 * Matrix.Invert(transform2);

            // When a point moves in A's local space, it moves in B's local space with a
            // fixed direction and distance proportional to the movement in A.
            // This algorithm steps through A one pixel at a time along A's X and Y axes
            // Calculate the analogous steps in B:
            Vector2 stepX = Vector2.TransformNormal(Vector2.UnitX, transformAToB);
            Vector2 stepY = Vector2.TransformNormal(Vector2.UnitY, transformAToB);

            // Calculate the top left corner of A in B's local space
            // This variable will be reused to keep track of the start of each row
            Vector2 yPosInB = Vector2.Transform(Vector2.Zero, transformAToB);

            // For each row of pixels in A
            for (int yA = 0; yA < texture1.Height; yA++)
            {
                // Start at the beginning of the row
                Vector2 posInB = yPosInB;

                // For each pixel in this row
                for (int xA = 0; xA < texture1.Width; xA++)
                {
                    // Round to the nearest pixel
                    int xB = (int)Math.Round(posInB.X);
                    int yB = (int)Math.Round(posInB.Y);

                    // If the pixel lies within the bounds of B
                    if (0 <= xB && xB < texture2.Width &&
                        0 <= yB && yB < texture2.Height)
                    {
                        // Get the colors of the overlapping pixels
                        Color colorA = textureData1[xA + yA * texture1.Width];
                        Color colorB = textureData2[xB + yB * texture2.Width];

                        // If both pixels are not completely transparent,
                        if (colorA.A != 0 && colorB.A != 0)
                        {
                            // then an intersection has been found
                            return true;
                        }
                    }

                    // Move to the next pixel in the row
                    posInB.X = posInB.X + stepX.X;
                    posInB.Y = posInB.Y + stepX.Y;
                }

                // Move to the next row
                yPosInB.X = yPosInB.X + stepY.X;
                yPosInB.Y = yPosInB.Y + stepY.Y;
            }

            // No intersection found
            return false;
        }

        private static Matrix RotationMatrix;
        public static void TransformRectangle(PhysicalRectangle r, float rotationRad, PhysicalRectangle outR)
        {
            Matrix.CreateRotationZ(rotationRad, out RotationMatrix);
            Vector2 upperLeft = new Vector2(-r.Width / 2, -r.Height / 2);
            Vector2 downRight = new Vector2(r.Width / 2, r.Height / 2);

            Vector2.Transform(ref upperLeft, ref RotationMatrix, out upperLeft);
            Vector2.Transform(ref downRight, ref RotationMatrix, out downRight);

            Vector2 heightWidth;
            Vector2.Subtract(ref upperLeft, ref downRight, out heightWidth);

            heightWidth.X = Math.Abs(heightWidth.X);
            heightWidth.Y = Math.Abs(heightWidth.Y);

            outR.X = (int)(r.X + (r.Width / 2) - heightWidth.X / 2);
            outR.Y = (int)(r.Y - (r.Height / 2) - heightWidth.Y / 2);
            outR.Width = (int) heightWidth.X;
            outR.Height = (int) heightWidth.Y;
        }


        private static LineDirection GetLineDirection(ref Vector2 pt1, ref Vector2 pt2, ref Vector2 pt3)
        {
            float tester =
                (pt2.X - pt1.X) * (pt3.Y - pt1.Y) -
                (pt3.X - pt1.X) * (pt2.Y - pt1.Y);

            if (tester > 0)
                return LineDirection.CounterClockwise;
            
            if (tester < 0)
                return LineDirection.Clockwise;

            return LineDirection.Line;
        }
    }
}
