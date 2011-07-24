namespace EphemereGames.Core.Visual
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;


    public class VisualRectangle : IScenable
    {
        public Vector3 Position                 { get; set; }
        public double VisualPriority            { get; set; }
        public Scene Scene                      { get; set; }
        public BlendType Blend                  { get; set; }
        public int Id                           { get; private set; }

        private Color Color;
        private Rectangle Rectangle;
        private bool Filled;


        public VisualRectangle(Rectangle rectangle, Color color) : this(rectangle, color, false) { }


        public VisualRectangle(Rectangle rectangle, Color color, bool filled)
        {
            VisualPriority = 0;
            Blend = BlendType.Default;
            Color = color;
            Rectangle = rectangle;
            Id = Visuals.NextHashCode;
            Filled = filled;
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            Primitives.DrawRect(spriteBatch, Rectangle, Color, Filled);
        }
    }
}