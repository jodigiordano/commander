
namespace EphemereGames.Core.Visual
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class VisualRectangle : IScenable
    {
        public Vector3 Position                 { get; set; }
        public double VisualPriority            { get; set; }
        public Scene Scene                      { get; set; }
        public TypeBlend Blend                  { get; set; }
        public int Id                           { get; private set; }

        private Color Couleur                   { get; set; }
        private Rectangle Rectangle             { get; set; }

        public VisualRectangle(Rectangle rectangle, Color couleur)
        {
            VisualPriority = 0;
            Blend = TypeBlend.Default;
            Couleur = couleur;
            Rectangle = rectangle;
            Id = Visuals.NextHashCode;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Primitives.DrawRect(spriteBatch, Rectangle, Couleur);
        }
    }
}