namespace EphemereGames.Core.Visual
{
    using System.Text;
    using EphemereGames.Core.Physics;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;


    public class Text : IScenable, IPhysical, IVisual
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
        private bool TextSizeComputed;
        private bool RectangleComputed;
        private Vector2 textSize;
        private PhysicalRectangle rectangle;
        private string data;
        private Vector3 position;
        private Color color;
        private string FontName;


        public Text(string data, string fontName, Color color, Vector3 position)
        {
            FontName = fontName;
            Data = data;
            Font = Core.Persistence.Persistence.GetAsset<SpriteFont>(fontName);
            Color = color;
            Position = position;
            Size = new Vector2(1);
            TextSizeComputed = false;
            RectangleComputed = false;
            Id = Visuals.NextHashCode;
        }


        public Text(string fontName) : this("", fontName, Color.White, Vector3.Zero) {}
        public Text(string text, string fontName) : this(text, fontName, Color.White, Vector3.Zero) { }
        public Text(string text, string fontName, Vector3 position) : this(text, fontName, Color.White, position) { }


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
            get { return Vector2.Multiply(RelativeTextSize, Size); }
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


        public string Data
        {
            get { return data; }
            set
            {
                data = value;

                TextSizeComputed = false;
                RectangleComputed = false;
            }
        }


        public float SizeX
        {
            get { return Size.X; }
            set
            {
                Size = new Vector2(value);

                TextSizeComputed = false;
                RectangleComputed = false;
            }
        }


        public Text CenterIt()
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


        public Text Clone()
        {
            Text i = (Text) this.MemberwiseClone();
            i.Id = Visuals.NextHashCode;

            return i;
        }


        public Text CompartmentalizeIt(int maxWidthPx)
        {
            //todo: use lineSpacing

            // find the absolute size of one letter
            int sizeX = (int) (new Text(@"a", FontName) { Size = Size }.AbsoluteSize.X);
            int maxCharsPerLine = maxWidthPx / sizeX;

            // split the words
            string[] words = data.Split(new char[] { ' ' });

            // add new lines where needed
            StringBuilder newData = new StringBuilder();

            int charCount = 0;

            for (int i = 0; i < words.Length; i++)
            {
                var word = words[i];

                charCount += word.Length;

                if (charCount <= maxCharsPerLine)
                {
                    newData.Append(word);
                    newData.Append(@" ");
                    charCount++;
                }

                else
                {
                    if (i != 0)
                        newData.Append("\n\n");
                    
                    newData.Append(word);

                    if (i != 0 && i != words.Length - 1)
                        newData.Append(" ");

                    charCount = word.Length + 1;
                }
            }

            // sync the new data
            Data = newData.ToString();

            return this;
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


        private Vector3 GetUpperLeft()
        {
            return Position - new Vector3(Origin * Size, 0);
        }


        //useless
        public Rectangle VisiblePart { get; set; }
        public float Speed { get; set; }
    }
}
