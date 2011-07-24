namespace EphemereGames.Core.Visual
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class VisualLine : IScenable
    {
        public Vector3 Position             { get; set; }
        public double VisualPriority        { get; set; }
        public Scene Scene                  { get; set; }
        public BlendType Blend              { get; set; }
        public int Id                       { get; private set; }

        public Color Color                  { get; set; }
        public Vector3 Start                { get; set; }
        public Vector3 End                  { get; set; }
        public int Tickness                 { get; set; }

        private static Matrix MatriceRotation = Matrix.CreateRotationZ(MathHelper.PiOver2);


        public VisualLine(Vector2 start, Vector2 end, Color color) :
            this(new Vector3(start, 0), new Vector3(end, 0), color, 1) {}


        public VisualLine(Vector3 start, Vector3 end, Color color) :
            this(start, end, color, 1) { }


        public VisualLine(Vector3 start, Vector3 end, Color color, int tickness)
        {
            VisualPriority = 0;
            Blend = BlendType.Default;
            Color = color;

            Start = start;
            End = end;

            Tickness = tickness;

            Id = Visuals.NextHashCode;
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            int offsetDebut = -Tickness / 2;

            Vector3 direction = End - Start;
            Vector3 directionUnitairePerpendiculaire = Vector3.Transform(direction, MatriceRotation);
            directionUnitairePerpendiculaire.Normalize();

            Vector2 directionUnitairePerpendiculaire2 = new Vector2(directionUnitairePerpendiculaire.X, directionUnitairePerpendiculaire.Y);

            for (int i = 0; i < Tickness; i++)
            {
                Vector2 translation = directionUnitairePerpendiculaire2 * (offsetDebut + i);

                Primitives.DrawLine(spriteBatch, new Vector2(Start.X, Start.Y) + translation, new Vector2(End.X, End.Y) + translation, Color);

            }
        }
    }
}