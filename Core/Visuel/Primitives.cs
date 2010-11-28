namespace Core.Visuel
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Core.Physique;

    static class Primitives
    {
      private static Texture2D pixel;

      public static void init(Texture2D texture)
      {
          pixel = texture;
      }

      //Calculates the distances and the angle and than draws a line
      public static void DrawLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color)
      {
          int distance = (int)Vector2.Distance(start, end);

          Vector2 connection = end - start;
          Vector2 baseVector = new Vector2(1, 0);

          float alpha = (float)Math.Atan2(end.Y - start.Y, end.X - start.X);

          if (pixel != null)
              spriteBatch.Draw(pixel, new Rectangle((int)start.X, (int)start.Y, distance, 1),
                  null, color, alpha, new Vector2(0, 0), SpriteEffects.None, 0);
      }

      //Draws a rect with the help of DrawLine
      public static void DrawRect(SpriteBatch sprite, Rectangle rect, Color color)
      {
          // | left
          DrawLine(sprite, new Vector2(rect.X, rect.Y), new Vector2(rect.X, rect.Y + rect.Height), color);
          // - top
          DrawLine(sprite, new Vector2(rect.X, rect.Y), new Vector2(rect.X + rect.Width, rect.Y ), color);
          // - bottom
          DrawLine(sprite, new Vector2(rect.X, rect.Y + rect.Height),
              new Vector2(rect.X + rect.Width, rect.Y + rect.Height), color);
          // | right
          DrawLine(sprite, new Vector2(rect.X + rect.Width, rect.Y),
              new Vector2(rect.X + rect.Width, rect.Y + rect.Height), color);
      }

      public static void DrawTriangle(SpriteBatch sprite, Triangle triangle, Color color)
      {
          DrawLine(sprite, triangle.Sommet1, triangle.Sommet2, color);
          DrawLine(sprite, triangle.Sommet2, triangle.Sommet3, color);
          DrawLine(sprite, triangle.Sommet3, triangle.Sommet1, color);
      }
    }
}