namespace EphemereGames.Core.Visuel
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;


    public class Text : IScenable
    {
        public Vector2 Origin;
        public Vector2 Size;
        public Vector2 Center;
        public Vector2 TextureSize;
        public float Rotation;

        public Vector3 Position { get; set; }
        public TypeBlend Blend { get; set; }
        public float VisualPriority { get; set; }
        public virtual List<IScenable> Components { get; set; }

        public string Data;
        public Vector2 TextSize;
        public Color Color;

        private SpriteFont Font;


        public Text(string data, SpriteFont font, Color color, Vector3 position)
        {
            Data = data;
            Font = font;
            Color = color;
            Position = position;
            TextSize = Font.MeasureString(Data);
            Center = TextSize / 2f;
        }


        public float SizeX
        {
            get { return Size.X; }
            set { Size = new Vector2(value); }
        }


        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(
                Font,
                Data,
                new Vector2(Position.X, Position.Y),
                Color,
                Rotation,
                Origin,
                Size,
                SpriteEffects.None,
                0);
        }
    }
}
