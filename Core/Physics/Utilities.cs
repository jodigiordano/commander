namespace EphemereGames.Core.Physics
{
    using System;
    using Microsoft.Xna.Framework;


    public static class Utilities
    {
        public static float VectorToAngle(ref Vector2 vector)
        {
            return (float) Math.Atan2(vector.X, -vector.Y);
        }


        public static float VectorToAngle(ref Vector3 vector)
        {
            return (float) Math.Atan2(vector.X, -vector.Y);
        }


        public static Vector3 AngleToVector(float angle)
        {
            return new Vector3((float) Math.Sin(angle), -(float) Math.Cos(angle), 0);
        }


        public static Vector2 AngleToVector2(float angle)
        {
            return new Vector2((float) Math.Sin(angle), -(float) Math.Cos(angle));
        }


        public static void AngleToVector(float angle, ref Vector2 vector)
        {
            vector.X = (float) Math.Sin(angle);
            vector.Y = -(float) Math.Cos(angle);
        }


        public static void AngleToVector(float angle, Vector3 vector)
        {
            vector.X = (float) Math.Sin(angle);
            vector.Y = -(float) Math.Cos(angle);
            vector.Z = 0;
        }
    }
}
