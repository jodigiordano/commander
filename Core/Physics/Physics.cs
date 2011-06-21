namespace EphemereGames.Core.Physics
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    

    public static class Physics
    {
        public static void Initialize() {}


        public static bool PixelsCollision(
            ref Matrix transform1, Texture2D texture1, ref Color[] textureData1,
            ref Matrix transform2, Texture2D texture2, ref Color[] textureData2)
        {
            return Collisions.PixelCollision(
                ref transform1, texture1, ref textureData1,
                ref transform2, texture2, ref textureData2);
        }


        public static bool PointRectangleCollision(ref Vector2 point, PhysicalRectangle rectangle)
        {
            return Collisions.PointRectangleCollision(ref point, rectangle);
        }


        public static bool CircleRectangleCollision(Circle circle, PhysicalRectangle rectangle)
        {
            return Collisions.CircleRectangleCollision(circle, rectangle);
        }


        public static bool RectangleRectangleCollision(PhysicalRectangle rectangle, PhysicalRectangle rectangle2)
        {
            return Collisions.RectangleRectangleCollision(rectangle, rectangle2);
        }


        public static bool TriangleRectangleCollision(Triangle triangle, PhysicalRectangle rectangle)
        {
            return Collisions.TriangleRectangleCollision(triangle, rectangle);
        }


        public static bool LineRectangleCollision(Line line, PhysicalRectangle rectangle)
        {
            return Collisions.LineRectangleCollision(line, rectangle);
        }


        public static bool PointCircleCollision(ref Vector3 point, Circle circle)
        {
            return Collisions.PointCircleCollision(ref point, circle);
        }


        public static bool CircleCicleCollision(Circle circle1, Circle circle2)
        {
            return Collisions.CircleCircleCollision(circle1, circle2);
        }


        public static void TransformRectangle(PhysicalRectangle physicalRectangle, float rotationRad, PhysicalRectangle physicalRectangleOut)
        {
            Collisions.TransformRectangle(physicalRectangle, rotationRad, physicalRectangleOut);
        }
    }
}
