namespace EphemereGames.Core.Visuel
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;


    public class Text : IScenable
    {
        public Vector2 Origin;
        public Vector2 Size;
        public Vector2 Center { get { return RelativeTextSize / 2f; } }
        public Vector2 TextureSize;
        public float Rotation;

        public Vector3 Position { get; set; }
        public TypeBlend Blend { get; set; }
        public float VisualPriority { get; set; }
        public virtual List<IScenable> Components { get; set; }
        public Color Color;

        private SpriteFont Font;
        private bool TextSizeComputed;
        private Vector2 textSize;
        private string data;


        public Text(string data, string fontName, Color color, Vector3 position)
        {
            Data = data;
            Font = Core.Persistance.Facade.GetAsset<SpriteFont>(fontName);
            Color = color;
            Position = position;
            Size = new Vector2(1);
            TextSizeComputed = false;
        }


        public Text(string fontName) : this("", fontName, Color.White, Vector3.Zero) {}


        public Vector2 TextSize
        {
            get { return Vector2.Multiply(RelativeTextSize, Size); }
        }


        public string Data
        {
            get { return data; }
            set
            {
                data = value;
                TextSizeComputed = false;
            }
        }


        public float SizeX
        {
            get { return Size.X; }
            set
            {
                Size = new Vector2(value);
                TextSizeComputed = false;
            }
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


        private Vector2 RelativeTextSize
        {
            get
            {
                if (TextSizeComputed)
                    return textSize;

                textSize = Font.MeasureString(Data);
                TextSizeComputed = true;

                return textSize;
            }
        }
    }
}
