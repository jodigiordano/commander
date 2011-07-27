namespace EphemereGames.Commander.Simulation
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class ScoreStars : IVisual, IPhysicalObject
    {
        private List<Image> Images;
        private int BrightCount;
        private Scene Scene;
        private int DistanceX;
        private byte AlphaBright;
        private byte AlphaNotBright;


        public ScoreStars(Scene scene, int brightCount, double visualPriority)
        {
            Scene = scene;
            BrightCount = brightCount;
            DistanceX = 15;
            AlphaBright = 255;
            AlphaNotBright = 100;

            Images = new List<Image>();

            for (int i = 0; i < 3; i++)
            {
                Images.Add(new Image("Star")
                {
                    SizeX = 0.25f,
                    VisualPriority = visualPriority,
                    Origin = Vector2.Zero,
                    Alpha = (byte) (i < brightCount ? AlphaBright : AlphaNotBright)
                });
            }
        }


        public Color Color
        {
            get
            {
                return Images[0].Color;
            }
            set
            {
                foreach (var img in Images)
                    img.Color = value;
            }
        }


        public byte Alpha
        {
            get
            {
                return Images[0].Alpha;
            }
            set
            {
                for (int i = 0; i < Images.Count; i++)
                    Images[i].Alpha = (byte) (i < BrightCount ? Math.Min(value, AlphaBright) : Math.Min(value, AlphaNotBright));
            }
        }

        public Vector3 Position
        {
            get
            {
                return Images[0].Position;
            }
            set
            {
                for (int i = 0; i < Images.Count; i++)
                {
                    var img = Images[i];

                    img.Position = value + new Vector3(i * (img.AbsoluteSize.X + DistanceX), 0, 0);
                }
            }
        }


        public Vector2 Size
        {
            get { return new Vector2(Images[0].AbsoluteSize.X * Images.Count + DistanceX * Images.Count - 1, Images[0].AbsoluteSize.Y); }
            set { throw new System.NotImplementedException(); }
        }


        public void Draw()
        {
            foreach (var img in Images)
                Scene.Add(img);
        }


        public Rectangle VisiblePart
        {
            set { throw new System.NotImplementedException(); }
        }


        public Vector2 Origin
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }


        public float Speed
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }


        public float Rotation
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }
    }
}
