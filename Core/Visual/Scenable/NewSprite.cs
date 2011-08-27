namespace EphemereGames.Core.Visual
{
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using SpriteSheetRuntime;
    

    public class NewSprite : IVisual, IPhysical, IScenable
    {
        public Vector2 Origin           { get { return origin; } set { origin = value; AttributeChanged = true; } }
        public Vector2 Size             { get { return size; } set { size = value; AttributeChanged = true; } }
        public float SizeX              { get { return size.X; } set { size = new Vector2(value, value); AttributeChanged = true; }}
        public Color Color              { get { return color; } set { color = value; AttributeChanged = true; } }
        public byte Alpha               { get { return color.A; } set { color.A = value; AttributeChanged = true; } }
        public int Id                   { get; private set; }
        public Vector3 Position         { get { return position; } set { position = value; AttributeChanged = true; } }
        public double VisualPriority    { get { return visualPriority; } set { visualPriority = value; AttributeChanged = true; } }
        public BlendType Blend          { get { return blend; } set { blend = value; AttributeChanged = true; } }
        public float Rotation           { get { return rotation; } set { rotation = value; AttributeChanged = true; } }

        private SpriteSheet Spritesheet;
        private List<Image> Images;
        private List<int> Sequence;
        private bool Cyclic;
        private float Frequency;
        private double ElapsedTime;
        private int CurrentFrame;
        private bool CurrentFrameChanged;
        private bool AttributeChanged;
        private bool Active;
        private Vector2 TextureSize;
        private Vector2 Center;

        private Vector2 origin;
        private Vector2 size;
        private Color color;
        private Vector3 position;
        private double visualPriority;
        private BlendType blend;
        private float rotation;


        public NewSprite(string sheetName, List<int> sequence, float frequency, bool cyclic)
        {
            Spritesheet = Persistence.Persistence.GetAsset<SpriteSheet>(sheetName);
            Sequence = sequence;
            Frequency = frequency;
            Cyclic = cyclic;

            ElapsedTime = 0;
            CurrentFrame = 0;

            Images = new List<Image>();

            for (int i = 0; i < Spritesheet.ImagesCount; i++)
                Images.Add(new Image(Spritesheet.GetImageName(i), Vector3.Zero, sheetName));

            Active = true;

            TextureSize = Images[0].TextureSize;
            Center = TextureSize / 2f;
            Position = position;
            Origin = Center;
            Size = Vector2.One;
            Rotation = 0f;
            Blend = BlendType.Alpha;
            Color = Color.White;
            VisualPriority = 0;
            Id = Visuals.NextHashCode;

            CurrentFrameChanged = true;
            AttributeChanged = true;
        }


        public void Update()
        {
            if (!Active)
                return;

            ElapsedTime += 16.66;

            SetCurrentFrame();
        }


        public void Start()
        {
            Active = true;
        }


        public void Stop()
        {
            Active = false;
        }


        public void NextFrame()
        {
            ElapsedTime += Frequency;
            SetCurrentFrame();
        }


        public void FirstFrame()
        {
            ElapsedTime = 0;
            SetCurrentFrame();
        }


        public Rectangle VisiblePart
        {
            set { throw new System.NotImplementedException(); }
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            if (CurrentFrameChanged || AttributeChanged)
                Synchronize();

            Images[CurrentFrame].Draw(spriteBatch);

            CurrentFrameChanged = false;
            AttributeChanged = false;
        }


        private void Synchronize()
        {
            var img = Images[CurrentFrame];

            img.Blend = Blend;
            img.VisualPriority = VisualPriority;
            img.Position = Position;
            img.Color = Color;
            img.Size = Size;
            img.Origin = Origin;
            img.Rotation = Rotation;
        }


        private void SetCurrentFrame()
        {
            var before = CurrentFrame;

            CurrentFrame = Sequence[(int) (ElapsedTime / Frequency) % Sequence.Count];

            if (before != CurrentFrame)
                CurrentFrameChanged = true;
        }


        public float Speed
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); } 
        }
    }
}
