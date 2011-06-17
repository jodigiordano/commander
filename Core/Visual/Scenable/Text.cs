namespace EphemereGames.Core.Visual
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using EphemereGames.Core.Physics;


    public class Text : IScenable, IPhysicalObject, IVisual
    {
        public Vector2 Origin           { get; set; }
        public Vector2 Size             { get; set; }
        public Vector2 Center           { get { return RelativeTextSize / 2f; } }
        public Vector2 TextureSize;
        public float Rotation           { get; set; }

        public Vector3 Position         { get; set; }
        public TypeBlend Blend          { get; set; }
        public double VisualPriority    { get; set; }
        public int Id                   { get; private set; }
        public Color Color;

        private SpriteFont Font;
        private bool TextSizeComputed;
        private Vector2 textSize;
        private string data;


        public Text(string data, string fontName, Color color, Vector3 position)
        {
            Data = data;
            Font = Core.Persistence.Persistence.GetAsset<SpriteFont>(fontName);
            Color = color;
            Position = position;
            Size = new Vector2(1);
            TextSizeComputed = false;
            Id = Visuals.NextHashCode;
        }


        public Text(string fontName) : this("", fontName, Color.White, Vector3.Zero) {}
        public Text(string text, string fontName) : this(text, fontName, Color.White, Vector3.Zero) { }
        public Text(string text, string fontName, Vector3 position) : this(text, fontName, Color.White, position) { }


        public Vector2 TextSize
        {
            get { return Vector2.Multiply(RelativeTextSize, Size); }
        }


        public byte Alpha
        {
            get { return Color.A; }
            set { Color.A = value; }
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


        public Text CenterIt()
        {
            Origin = Center;

            return this;
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

        //useless
        public Rectangle VisiblePart { get; set; }
        public float Speed { get; set; }
    }
}
