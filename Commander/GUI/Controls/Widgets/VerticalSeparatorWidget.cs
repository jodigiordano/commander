namespace EphemereGames.Commander
{
    using System;
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class VerticalSeparatorWidget : PanelWidget
    {
        public int DistanceWidgetBeforeAndAfter;

        private Image Image;
        private int visualWidth;


        public VerticalSeparatorWidget()
        {
            Image = new Image("PixelBlanc")
            {
                Size = new Vector2(VisualWidth, 0),
                Origin = Vector2.Zero,
                Alpha = 200
            };

            VisualWidth = 5;
            DistanceWidgetBeforeAndAfter = 0;

            Image.Size = new Vector2(visualWidth, 30);
        }


        public override double VisualPriority
        {
            get
            {
                return Image.VisualPriority;
            }

            set
            {
                Image.VisualPriority = value;
            }
        }


        public override Vector3 Position
        {
            get
            {
                return Image.Position - new Vector3(DistanceWidgetBeforeAndAfter, 0, 0);
            }

            set
            {
                Image.Position = value + new Vector3(DistanceWidgetBeforeAndAfter, 0, 0);
            }
        }


        public override byte Alpha
        {
            get { return Image.Alpha; }
            set { Image.Alpha = Math.Max(value, (byte) 200); }
        }


        public int VisualWidth
        {
            get { return visualWidth; }
            set
            {
                visualWidth = value;
                Image.Size = new Vector2(visualWidth, Image.AbsoluteSize.Y);
            }
        }


        public override Vector3 Dimension
        {
            get { return new Vector3(Image.AbsoluteSize.X + 2 * DistanceWidgetBeforeAndAfter, Image.AbsoluteSize.Y, 0); }
            set { Image.Size = new Vector2(visualWidth, Dimension.Y); }
        }


        protected override bool Click(Circle circle)
        {
            return false;
        }


        protected override bool Hover(Circle circle)
        {
            return false;
        }


        public override void Draw()
        {
            Image.Size = new Vector2(visualWidth, 30);
            Scene.Add(Image);
        }


        public override void Fade(int from, int to, double length)
        {
            var effect = VisualEffects.Fade(from, to, 0, length);

            Image.Alpha = (byte) from;

            Scene.VisualEffects.Add(Image, effect);
        }
    }
}
