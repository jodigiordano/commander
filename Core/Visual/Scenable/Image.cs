namespace EphemereGames.Core.Visual
{
    using EphemereGames.Core.Physics;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using SpriteSheetRuntime;


    public class Image : IScenable, IPhysical, IVisual
    {
        public Vector2 Origin           { get; set; }
        public Vector2 Size             { get; set; }
        public Vector2 Center;
        public Vector2 TextureSize;

        public Vector3 position;
        public Vector3 Position         { get { return position; } set { position = value; RectangleComputed = false; } }
        public float Rotation           { get; set; }
        public BlendType Blend          { get; set; }
        public double VisualPriority    { get; set; }
        public int Id                   { get; private set; }

        public Rectangle VisiblePart    { get { return visiblePart; } set { visiblePart = value; } }
        public Rectangle visiblePart;
        public SpriteEffects Effect;
        public string TextureName       { get; private set; }

        private Texture2D Texture;

        private bool RectangleComputed;
        private PhysicalRectangle rectangle;
        public Color color;


        public Image(string imageName) : this(imageName, Vector3.Zero, @"spritesheet") {}


        public Image(string imageName, Vector3 position) : this(imageName, position, @"spritesheet") { }


        public Image(string imageName, Vector3 position, string spriteSheetName)
        {
            TextureName = imageName;

            LoadTexture(spriteSheetName);

            Center = TextureSize / 2f;
            Position = position;
            Origin = Center;
            Size = Vector2.One;
            Rotation = 0f;
            Blend = BlendType.Alpha;
            Color = Color.White;
            Effect = SpriteEffects.None;
            VisualPriority = 0;
            Id = Visuals.NextHashCode;
            RectangleComputed = false;
        }


        public float SizeX
        {
            get { return Size.X; }
            set { Size = new Vector2(value); RectangleComputed = false; }
        }


        public Vector2 AbsoluteSize
        {
            get
            {
                return Size * TextureSize;
            }
        }


        public Color Color
        {
            get { return color; }
            set { color = value; }
        }



        public byte Alpha
        {
            get { return color.A; }
            set { color.A = value; }
        }


        public PhysicalRectangle GetRectangle()
        {
            if (rectangle == null)
                rectangle = new PhysicalRectangle();

            if (RectangleComputed)
                return rectangle;

            var upperLeft = GetUpperLeft();
            var size = AbsoluteSize;

            rectangle.X = (int) upperLeft.X;
            rectangle.Y = (int) upperLeft.Y;
            rectangle.Width = (int) size.X;
            rectangle.Height = (int) size.Y;

            RectangleComputed = true;

            return rectangle;
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


        private void LoadTexture(string spriteSheetName)
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
                Persistence.Persistence.GetAsset<SpriteSheet>(spriteSheetName);

            Texture = spritesheet.Texture;
            VisiblePart = spritesheet.SourceRectangle(TextureName);
            TextureSize = new Vector2(VisiblePart.Width, VisiblePart.Height);
        }


        public float Speed { get; set; }


        private Vector3 GetUpperLeft()
        {
            return Position - new Vector3(Origin * Size, 0);
        }
    }
}
