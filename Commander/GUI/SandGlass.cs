namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Utilities;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class SandGlass : IObjetPhysique
    {
        public double RemainingTime;

        public Vector3 Position             { get; set; }
        public RectanglePhysique Rectangle  { get; set; }
        public Shape Shape                  { get; set; }

        private Scene Scene;
        private Image Image;
        private List<Image> Pixels;
        private int Progress;
        private int PreviousProgress;
        private double MinimumTime;
        private RotationEffect RotationEffect;

        private const int NB_PIXELS = 48;
        private const int SIZE_PIXEL = 4;


        private static List<Vector3> RelativePositions = new List<Vector3>()
        {
            new Vector3(2, 2, 0),
            new Vector3(3, 2, 0),
            new Vector3(4, 2, 0),
            new Vector3(5, 2, 0),
            new Vector3(6, 2, 0),
            new Vector3(7, 2, 0),
            new Vector3(2, 3, 0),
            new Vector3(3, 3, 0),
            new Vector3(4, 3, 0),
            new Vector3(5, 3, 0),
            new Vector3(6, 3, 0),
            new Vector3(7, 3, 0),
            new Vector3(2, 4, 0),
            new Vector3(3, 4, 0),
            new Vector3(4, 4, 0),
            new Vector3(5, 4, 0),
            new Vector3(6, 4, 0),
            new Vector3(7, 4, 0),
            new Vector3(3, 5, 0),
            new Vector3(4, 5, 0),
            new Vector3(5, 5, 0),
            new Vector3(6, 5, 0),
            new Vector3(4, 6, 0),
            new Vector3(5, 6, 0),

            new Vector3(4, 14, 0),
            new Vector3(6, 14, 0),
            new Vector3(3, 14, 0),
            new Vector3(5, 14, 0),
            new Vector3(4, 13, 0),
            new Vector3(5, 13, 0),
            new Vector3(6, 13, 0),
            new Vector3(5, 12, 0),
            new Vector3(7, 14, 0),
            new Vector3(3, 13, 0),
            new Vector3(6, 12, 0),
            new Vector3(2, 14, 0),
            new Vector3(7, 13, 0),
            new Vector3(7, 12, 0),
            new Vector3(4, 12, 0),
            new Vector3(6, 11, 0),
            new Vector3(2, 13, 0),
            new Vector3(2, 12, 0),
            new Vector3(5, 11, 0),
            new Vector3(3, 12, 0),
            new Vector3(4, 11, 0),
            new Vector3(3, 11, 0),
            new Vector3(4, 10, 0),
            new Vector3(5, 10, 0)
        };


        public SandGlass(Main main, Scene scene, double minimumTime, Vector3 position, double visualPriority)
        {
            this.Scene = scene;
            this.MinimumTime = minimumTime;
            this.Position = position;

            Pixels = new List<Image>(NB_PIXELS);

            Image = new Image("sablier", position)
            {
                SizeX = 4,
                VisualPriority = visualPriority
            };

            for (int i = 0; i < NB_PIXELS; i++)
            {
                Image iv = new Image("PixelBlanc")
                {
                    Color = new Color(255, 0, 220, 255),
                    SizeX = SIZE_PIXEL,
                    Position = this.Position + (RelativePositions[i] - new Vector3(Image.Origin.X, Image.Origin.Y, 0)) * SIZE_PIXEL,
                    VisualPriority = Image.VisualPriority - 0.01f
                };

                Vector3 origin = (this.Position - iv.Position);

                iv.Origin = new Vector2(origin.X, origin.Y) / SIZE_PIXEL;
                iv.Position += origin;

                Pixels.Add(iv);
            }

            Progress = 0;
            PreviousProgress = 0;

            Shape = Shape.Rectangle;
            Vector2 representationSize = Image.AbsoluteSize;
            Rectangle = new RectanglePhysique(
                (int) (Image.Position.X - representationSize.X / 2),
                (int) (Image.Position.Y - representationSize.Y / 2),
                (int) representationSize.X,
                (int) representationSize.Y);
        }


        public bool IsFlipping
        {
            get
            {
                return (RotationEffect != null && !RotationEffect.Finished);
            }
        }


        public void Update()
        {
            if (RotationEffect != null && RotationEffect.Finished)
            {
                Image.Rotation = 0;
                Progress = 0;

                RotationEffect = null;
            }

            if (RemainingTime >= MinimumTime)
                return;

            Progress = (int)Math.Ceiling(((MinimumTime - RemainingTime) / MinimumTime) * (NB_PIXELS / 2));
        }


        //public void Show()
        //{
        //    Scene.Add(Image);

        //    AddUpperPixels();
        //}


        //public void Hide()
        //{
        //    Scene.Remove(Image);

        //    foreach (var pixel in Pixels)
        //        Scene.Remove(pixel);
        //}


        public void Draw()
        {
            int nbPixels = NB_PIXELS / 2;

            for (int i = Progress; i < nbPixels; i++)
                Scene.Add(Pixels[i]);

            for (int i = 0; i < Progress; i++)
                Scene.Add(Pixels[nbPixels + i]);

            //if (PreviousProgress >= nbPixels)
            //{
            //    AddUpperPixels();
            //    RemoveLowerPixels();
            //    PreviousProgress = 0;
            //}
            
            //int delta = Progress - PreviousProgress;

            //for (int i = PreviousProgress; i < Progress; i++)
            //    Scene.Remove(Pixels[i]);

            //for (int i = PreviousProgress; i < Progress; i++)
            //    Scene.Add(Pixels[nbPixels + i]);

            //PreviousProgress = Progress;

            Scene.Add(Image);

            // rotation
            for (int i = 0; i < NB_PIXELS; i++)
                Pixels[i].Rotation = Image.Rotation;
        }


        //private void AddUpperPixels()
        //{
        //    for (int i = 0; i < NB_PIXELS / 2; i++)
        //        Scene.Add(Pixels[i]);
        //}


        //private void RemoveLowerPixels()
        //{
        //    for (int i = 1; i <= NB_PIXELS / 2; i++)
        //        Scene.Remove(Pixels[NB_PIXELS - i]);
        //}


        public void Flip()
        {
            if (RotationEffect != null && !RotationEffect.Finished)
                return;

            RotationEffect = new RotationEffect();
            RotationEffect.Length = 500;
            RotationEffect.Progress = AbstractEffect.ProgressType.Linear;
            RotationEffect.Quantite = MathHelper.Pi;

            Scene.Effects.Add(Image, RotationEffect);
        }


        public void FadeOut(double time)
        {
            Scene.Effects.Add(Image, Core.Visual.PredefinedEffects.FadeOutTo0(255, 0, time));

            foreach (var pixel in Pixels)
                Scene.Effects.Add(pixel, Core.Visual.PredefinedEffects.FadeOutTo0(255, 0, time));
        }


        public void FadeIn(double time)
        {
            Scene.Effects.Add(Image, Core.Visual.PredefinedEffects.FadeInFrom0(255, 0, time));

            foreach (var pixel in Pixels)
                Scene.Effects.Add(pixel, Core.Visual.PredefinedEffects.FadeInFrom0(255, 0, time));
        }


        #region IObjetPhysique Membres
        public float Speed { get; set; }
        public Vector3 Direction { get; set; }
        public float Rotation { get; set; }
        public Circle Circle { get; set; }
        public Line Line { get; set; }
        #endregion
    }
}
