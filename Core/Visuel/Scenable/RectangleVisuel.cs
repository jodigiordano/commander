
namespace EphemereGames.Core.Visuel
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class RectangleVisuel : IScenable
    {
        public Vector3 Position                 { get; set; }
        public double VisualPriority            { get; set; }
        public Scene Scene                      { get; set; }
        public TypeBlend Blend                  { get; set; }
        public int Id                           { get; private set; }

        private Color Couleur                   { get; set; }
        private Rectangle Rectangle             { get; set; }

        public RectangleVisuel(Rectangle rectangle, Color couleur)
        {
            VisualPriority = 0;
            Blend = TypeBlend.Default;
            Couleur = couleur;
            Rectangle = rectangle;
            Id = Facade.NextHashCode;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Primitives.DrawRect(spriteBatch, Rectangle, Couleur);
        }
    }
}