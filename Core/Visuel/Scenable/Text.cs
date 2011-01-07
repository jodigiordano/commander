namespace EphemereGames.Core.Visuel
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;


    public class Text : IScenable
    {
        public Vector2 Origin;
        public Vector2 Size;
        public Vector2 Center { get { return TextSize / 2f; } }
        public Vector2 TextureSize;
        public float Rotation;

        public Vector3 Position { get; set; }
        public TypeBlend Blend { get; set; }
        public float VisualPriority { get; set; }
        public virtual List<IScenable> Components { get; set; }

        public string Data;
        public Color Color;

        private SpriteFont Font;


        public Text(string data, string fontName, Color color, Vector3 position)
        {
            Data = data;
            Font = Core.Persistance.Facade.GetAsset<SpriteFont>(fontName);
            Color = color;
            Position = position;
            Size = new Vector2(1);
        }


        public Text(string fontName)
        {
            Data = "";
            Font = Core.Persistance.Facade.GetAsset<SpriteFont>(fontName);
            Color = Color.White;
            Position = Vector3.Zero;
            Size = new Vector2(1);
        }


        private Vector2 TextSize
        {
            get { return Font.MeasureString(Data); }
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
