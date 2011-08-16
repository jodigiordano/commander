namespace EphemereGames.Core.Visual
{
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;


    public class ColoredText : IScenable, IPhysical, IVisual
    {
        public Vector2 Origin           { get; set; }
        public Vector2 Size             { get; set; }
        public Vector2 Center           { get { return RelativeTextSize / 2f; } }
        public Vector2 TextureSize;
        public float Rotation           { get; set; }

        public BlendType Blend          { get; set; }
        public double VisualPriority    { get; set; }
        public int Id                   { get; private set; }

        private SpriteFont Font;
        private bool RelativeTextSizeComputed;
        private bool AbsoluteTextSizeComputed;
        private bool RectangleComputed;
        private Vector2 relativeTextSize;
        private Vector2 absoluteTextSize;
        private PhysicalRectangle rectangle;
        private List<string> data;
        private List<Vector2> relativeDataSizes;
        private List<Vector2> absoluteDataSizes;
        private Color[] colors;
        private Vector3 position;


        public ColoredText(List<string> data, Color[] colors, string fontName, Vector3 position)
        {
            absoluteDataSizes = new List<Vector2>();
            relativeDataSizes = new List<Vector2>();

            Data = data;
            Font = Core.Persistence.Persistence.GetAsset<SpriteFont>(fontName);
            Colors = colors;
            Position = position;
            Size = new Vector2(1);
            RelativeTextSizeComputed = false;
            AbsoluteTextSizeComputed = false;
            RectangleComputed = false;
            Id = Visuals.NextHashCode;
        }


        public Vector3 Position
        {
            get { return position; }
            set
            {
                position = value;

                RectangleComputed = false;
            }
        }


        public Vector2 AbsoluteSize
        {
            get
            {
                if (AbsoluteTextSizeComputed)
                    return absoluteTextSize;

                absoluteTextSize = Vector2.Multiply(RelativeTextSize, Size);

                absoluteDataSizes.Clear();

                for (int i = 0; i < relativeDataSizes.Count; i++)
                    absoluteDataSizes.Add(Vector2.Multiply(relativeDataSizes[i], Size));

                AbsoluteTextSizeComputed = true;

                return absoluteTextSize;
            }
        }


        public Color Color
        {
            get { return colors[0]; }
            set { colors[0] = value; }
        }


        public Color[] Colors
        {
            get { return colors; }
            set { colors = value; }
        }


        public byte Alpha
        {
            get { return colors[0].A; }
            set
            {
                for (int i = 0; i < Colors.Length; i++)
                    Colors[i].A = value;
            }
        }


        public List<string> Data
        {
            get { return data; }
            set
            {
                data = value;

                RelativeTextSizeComputed = false;
                AbsoluteTextSizeComputed = false;
                RectangleComputed = false;
            }
        }


        public void SetData(int index, string value)
        {
            Data[index] = value;

            RelativeTextSizeComputed = false;
            AbsoluteTextSizeComputed = false;
            RectangleComputed = false;
        }


        public void SetColor(int index, Color color)
        {
            Colors[index] = color;
        }


        public float SizeX
        {
            get { return Size.X; }
            set
            {
                Size = new Vector2(value);

                RelativeTextSizeComputed = false;
                AbsoluteTextSizeComputed = false;
                RectangleComputed = false;
            }
        }


        public ColoredText CenterIt()
        {
            Origin = Center;

            return this;
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
            var position = new Vector2(Position.X, Position.Y);
            
            for (int i = 0; i < Data.Count; i++)
            {
                if (i > 0)
                    position.X += absoluteDataSizes[i - 1].X;

                spriteBatch.DrawString(
                    Font,
                    Data[i],
                    position,
                    Colors[i],
                    Rotation,
                    Origin,
                    Size,
                    SpriteEffects.None,
                    0);
            }
        }


        public ColoredText Clone()
        {
            ColoredText i = (ColoredText) MemberwiseClone();
            i.Id = Visuals.NextHashCode;

            return i;
        }


        private Vector2 RelativeTextSize
        {
            get
            {
                if (RelativeTextSizeComputed)
                    return relativeTextSize;

                relativeDataSizes.Clear();

                foreach (var d in Data)
                    relativeDataSizes.Add(Font.MeasureString(d));

                relativeTextSize = relativeDataSizes[0];

                for (int i = 1; i < relativeDataSizes.Count; i++)
                    relativeTextSize.X += relativeDataSizes[i].X;

                RelativeTextSizeComputed = true;

                return relativeTextSize;
            }
        }


        private Vector3 GetUpperLeft()
        {
            return Position - new Vector3(Origin * Size, 0);
        }


        //useless
        public Rectangle VisiblePart { get; set; }
        public float Speed { get; set; }
    }
}
