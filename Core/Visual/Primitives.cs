namespace EphemereGames.Core.Visual
{
    using System;
    using EphemereGames.Core.Physics;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;


    static class Primitives
    {
        private static Texture2D Pixel;


        public static void Initialize(Texture2D pixel)
        {
            Pixel = pixel;
        }


        public static void DrawLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color)
        {
            int distance = (int)Vector2.Distance(start, end);

            Vector2 connection = end - start;
            Vector2 baseVector = new Vector2(1, 0);

            float alpha = (float)Math.Atan2(end.Y - start.Y, end.X - start.X);

            if (Pixel != null)
                spriteBatch.Draw(Pixel, new Rectangle((int)start.X, (int)start.Y, distance, 1),
                    null, color, alpha, new Vector2(0, 0), SpriteEffects.None, 0);
        }


        public static void DrawRect(SpriteBatch spriteBatch, Rectangle rect, Color color, bool filled)
        {
            if (filled)
                spriteBatch.Draw(Pixel, rect, null, color, 0, Vector2.Zero, SpriteEffects.None, 0);
            else
                DrawRectEdges(spriteBatch, rect, color);
        }


        public static void DrawTriangle(SpriteBatch spriteBatch, Triangle triangle, Color color)
        {
            DrawLine(spriteBatch, triangle.Sommet1, triangle.Sommet2, color);
            DrawLine(spriteBatch, triangle.Sommet2, triangle.Sommet3, color);
            DrawLine(spriteBatch, triangle.Sommet3, triangle.Sommet1, color);
        }


        private static void DrawRectEdges(SpriteBatch spriteBatch, Rectangle rect, Color color)
        {
            // left
            DrawLine(spriteBatch, new Vector2(rect.X, rect.Y), new Vector2(rect.X, rect.Y + rect.Height), color);

            // top
            DrawLine(spriteBatch, new Vector2(rect.X, rect.Y), new Vector2(rect.X + rect.Width, rect.Y), color);

            // bottom
            DrawLine(spriteBatch, new Vector2(rect.X, rect.Y + rect.Height),
            new Vector2(rect.X + rect.Width, rect.Y + rect.Height), color);

            // right
            DrawLine(spriteBatch, new Vector2(rect.X + rect.Width, rect.Y),
                new Vector2(rect.X + rect.Width, rect.Y + rect.Height), color);
        }
    }
}