namespace EphemereGames.Core.Visuel
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using EphemereGames.Core.Physique;


    public class Image : IScenable, IPhysicalObject
    {
        public Vector2 Origin;
        public Vector2 Size;
        public Vector2 Center;
        public Vector2 TextureSize;

        public Vector3 position;
        public Vector3 Position { get { return position; } set { position = value; } }
        public float Rotation { get; set; }
        public TypeBlend Blend { get; set; }
        public double VisualPriority { get; set; }
        public int Id { get; private set; }

        public Rectangle VisiblePart;
        public SpriteEffects Effect;
        public Color Color;

        private Texture2D Texture;
        private string TextureName;


        public Image(string imageName) : this(imageName, Vector3.Zero) {}


        public Image(string imageName, Vector3 position)
        {
            TextureName = imageName;
            Texture = Persistance.Facade.GetAsset<Texture2D>(imageName);
            TextureSize = new Vector2(Texture.Width, Texture.Height);
            Center = TextureSize / 2f;
            Position = position;
            Origin = Center;
            Size = Vector2.One;
            Rotation = 0f;
            Blend = TypeBlend.Alpha;
            Color = Color.White;
            Effect = SpriteEffects.None;
            VisiblePart = new Rectangle(0, 0, (int) TextureSize.X, (int) TextureSize.Y);
            VisualPriority = 0;
            Id = Facade.NextHashCode;
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
            i.Id = Facade.NextHashCode;

            return i;
        }


        //useless shit
        public float Speed { get; set; }
    }
}
