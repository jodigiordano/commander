namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class DisableBar : IVisual
    {
        public Vector3 Position;
        public float PercentageDone;

        private Image ProgressBarImage;
        private Image BarImage;
        private Scene Scene;


        public DisableBar(Scene scene)
        {
            Scene = scene;

            ProgressBarImage = new Image("PixelBlanc", Vector3.Zero)
            {
                Size = new Vector2(36, 4),
                Color = new Color(255, 0, 220, 255),
                Origin = new Vector2()
            };

            BarImage = new Image("BarreInactivite", Vector3.Zero)
            {
                SizeX = 3f
            };
        }


        public double VisualPriority
        {
            set
            {
                BarImage.VisualPriority = value;
                ProgressBarImage.VisualPriority = value - 0.00001;
            }
        }


        public byte Alpha
        {
            get
            {
                return ProgressBarImage.Alpha;
            }
            set
            {
                ProgressBarImage.Alpha = value;
                BarImage.Alpha = value;
            }
        }


        public void Draw()
        {
            BarImage.Position = Position;

            ProgressBarImage.Size = new Vector2(PercentageDone * 30, 8);
            ProgressBarImage.Position = BarImage.Position - new Vector3(16, 4, 0);

            Scene.Add(ProgressBarImage);
            Scene.Add(BarImage);
        }


        Rectangle IVisual.VisiblePart
        {
            set { throw new System.NotImplementedException(); }
        }


        Vector2 IVisual.Origin
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }


        Vector2 IVisual.Size
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }


        Color IVisual.Color
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }
    }
}
