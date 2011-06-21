namespace EphemereGames.Core.Visual
{
    using EphemereGames.Core.Physics;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using SpriteSheetRuntime;


    public class Image : IScenable, IPhysicalObject, IVisual
    {
        public Vector2 Origin           { get; set; }
        public Vector2 Size             { get; set; }
        public Vector2 Center;
        public Vector2 TextureSize;

        public Vector3 position;
        public Vector3 Position         { get { return position; } set { position = value; } }
        public float Rotation           { get; set; }
        public TypeBlend Blend          { get; set; }
        public double VisualPriority    { get; set; }
        public int Id                   { get; private set; }

        public Rectangle VisiblePart    { get; set; }
        public SpriteEffects Effect;
        public Color Color;
        public string TextureName       { get; private set; }

        private Texture2D Texture;


        public Image(string imageName) : this(imageName, Vector3.Zero) {}


        public Image(string imageName, Vector3 position)
        {
            TextureName = imageName;

            LoadTexture();

            Center = TextureSize / 2f;
            Position = position;
            Origin = Center;
            Size = Vector2.One;
            Rotation = 0f;
            Blend = TypeBlend.Alpha;
            Color = Color.White;
            Effect = SpriteEffects.None;
            VisualPriority = 0;
            Id = Visuals.NextHashCode;
        }


        public float SizeX
        {
            get { return Size.X; }
            set { Size = new Vector2(value); }
        }


        public Vector2 AbsoluteSize
        {
            get
            {
                return Size * TextureSize;
            }
        }


        public byte Alpha
        {
            get { return Color.A; }
            set { Color.A = value; }
        }


        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                Texture,
                new Vector2(Position.X, Position.Y),
                VisiblePart,
                Color,
                Rotation,
                Origin,
                Size,
                Effect,
                0);
        }


        public Image Clone()
        {
            Image i = (Image) this.MemberwiseClone();
            i.Id = Visuals.NextHashCode;

            return i;
        }


        private void LoadTexture()
        {
            Texture2D result = Persistence.Persistence.GetAsset<Texture2D>(TextureName);

            if (result != null)
            {
                Texture = result;
                TextureSize = new Vector2(Texture.Width, Texture.Height);
                VisiblePart = new Rectangle(0, 0, (int) TextureSize.X, (int) TextureSize.Y);
                return;
            }

            SpriteSheet spritesheet =
                Persistence.Persistence.GetAsset<SpriteSheet>("spritesheet");

            Texture = spritesheet.Texture;
            VisiblePart = spritesheet.SourceRectangle(TextureName);
            TextureSize = new Vector2(VisiblePart.Width, VisiblePart.Height);
        }


        public float Speed { get; set; }
    }
}
