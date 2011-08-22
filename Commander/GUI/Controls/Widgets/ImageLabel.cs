namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class ImageLabel : PanelWidget
    {
        public Vector3 DistanceBetweenImages = new Vector3(10, 0, 0);
        public Vector3 DistanceBetweenImageAndText = new Vector3(15, 0, 0);

        private List<Image> Images;
        private Text Text;
        private PhysicalRectangle Rectangle;

        public bool CanHover;


        public ImageLabel(Image image, Text text)
            : this(new List<Image>() { image }, text)
        {

        }


        public ImageLabel(List<Image> images, Text text)
        {
            Images = images;

            foreach (var img in images)
                img.Origin = Vector2.Zero;
            
            Text = text;

            CanHover = false;
            Rectangle = new PhysicalRectangle();
        }


        public void SetData(string data)
        {
            Text.Data = data;
        }


        public override double VisualPriority
        {
            get
            {
                return Images[0].VisualPriority;
            }

            set
            {
                foreach (var img in Images)
                    img.VisualPriority = value;

                Text.VisualPriority = value;
            }
        }


        public override Vector3 Position
        {
            get
            {
                return Images[0].Position;
            }

            set
            {
                Images[0].Position = value;

                for (int i = 1; i < Images.Count; i++)
                {
                    var previous = Images[i - 1];
                    var actual = Images[i];

                    actual.Position = previous.Position + new Vector3(previous.AbsoluteSize.X + DistanceBetweenImages.X, DistanceBetweenImages.Y, 0);
                }

                Text.Position = Images[Images.Count - 1].Position + new Vector3(Images[Images.Count - 1].AbsoluteSize.X + DistanceBetweenImageAndText.X, DistanceBetweenImageAndText.Y, 0);
            }
        }


        public override byte Alpha
        {
            get { return Text.Alpha; }
            set
            {
                Text.Alpha = value;

                foreach (var i in Images)
                    i.Alpha = value;
            }
        }


        public override Vector3 Dimension
        {
            get
            {
                return new Vector3(
                    Text.Position.X + Text.AbsoluteSize.X - Images[0].Position.X,
                    Images[0].AbsoluteSize.Y,
                    0);
            }

            set { }
        }


        protected override bool Click(Circle circle)
        {
            return false;
        }


        protected override bool Hover(Circle circle)
        {
            if (!CanHover)
                return false;

            SyncRectangle();

            return Physics.CircleRectangleCollision(circle, Rectangle);
        }


        public override void Draw()
        {
            foreach (var img in Images)
                Scene.Add(img);

            Scene.Add(Text);
        }


        public override void Fade(int from, int to, double length)
        {
            var effect = VisualEffects.Fade(from, to, 0, length);

            Text.Alpha = (byte) from;
            
            foreach (var img in Images)
            {
                img.Alpha = (byte) from;
                Scene.VisualEffects.Add(img, effect);
            }

            Scene.VisualEffects.Add(Text, effect);
        }


        private void SyncRectangle()
        {
            var dimension = Dimension;

            Rectangle.X = (int) Position.X;
            Rectangle.Y = (int) Position.Y;
            Rectangle.Width = (int) dimension.X;
            Rectangle.Height = (int) dimension.Y;
        }
    }
}
