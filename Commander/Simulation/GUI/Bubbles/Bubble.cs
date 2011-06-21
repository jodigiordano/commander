namespace EphemereGames.Commander.Simulation
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;


    class Bubble
    {
        protected Simulator Simulation;

        private List<Image> Corners;
        private List<Image> Edges;
        protected Image Filter;
        private Image Bla;

        public PhysicalRectangle Dimension;
        public int BlaPosition;


        public Bubble(Simulator simulator, PhysicalRectangle dimension, double prioriteAffichage)
        {
            this.Simulation = simulator;
            this.Dimension = dimension;

            Bla = new Image("bulleBlabla");
            Bla.VisualPriority = prioriteAffichage;
            Bla.Origin = Vector2.Zero;

            Filter = new Image("PixelBlanc")
            {
                Color = new Color(0, 0, 0, 200),
                VisualPriority = prioriteAffichage + 0.00002f,
                Origin = Vector2.Zero
            };

            Corners = new List<Image>();
            Edges = new List<Image>();

            for (int i = 0; i < 4; i++)
            {
                Image iv = new Image("bulleCoin");
                iv.VisualPriority = prioriteAffichage + 0.00001f;

                Corners.Add(iv);

                iv = new Image("PixelBlanc");
                iv.VisualPriority = prioriteAffichage + 0.00001f;
                iv.Origin = Vector2.Zero;

                Edges.Add(iv);
            }

            Color = Color.White;
        }


        public Color Color
        {
            set
            {
                for (int i = 0; i < 4; i++)
                {
                    Edges[i].Color = value;
                    Corners[i].Color = value;
                }

                Bla.Color = value;
            }
        }


        public virtual void Draw()
        {
            Dimension.Height = Math.Max(Dimension.Height, 30);

            Filter.Size = new Vector2(Dimension.Width + 8, Dimension.Height + 8);
            Filter.Position = new Vector3(Dimension.Left - 4, Dimension.Top - 4, 0);

            Corners[0].Position = new Vector3(Dimension.Left - 4, Dimension.Top - 4, 0); //Haut droite
            Corners[0].Rotation = 0;

            Corners[1].Position = new Vector3(Dimension.Left - 4, Dimension.Bottom + 4, 0); //Bas droite
            Corners[1].Rotation = -MathHelper.PiOver2;

            Corners[2].Position = new Vector3(Dimension.Right + 4, Dimension.Top - 4, 0); //Haut gauche
            Corners[2].Rotation = MathHelper.PiOver2;

            Corners[3].Position = new Vector3(Dimension.Right + 4, Dimension.Bottom + 4, 0); //Bas gauche
            Corners[3].Rotation = MathHelper.Pi;


            Edges[0].Size = new Vector2(Dimension.Width, 4); //Haut
            Edges[0].Position = new Vector3(Dimension.Left, Dimension.Top - 8, 0);

            Edges[1].Size = new Vector2(Dimension.Width, 4); //Bas
            Edges[1].Position = new Vector3(Dimension.Left, Dimension.Bottom + 4, 0);

            Edges[2].Size = new Vector2(4, Dimension.Height); //Gauche
            Edges[2].Position = new Vector3(Dimension.Left - 8, Dimension.Top, 0);

            Edges[3].Size = new Vector2(4, Dimension.Height); //Droite
            Edges[3].Position = new Vector3(Dimension.Right + 4, Dimension.Top, 0);

            switch (BlaPosition)
            {
                case 0: // haut gauche
                    Bla.Position = new Vector3(Dimension.Left - 4, Dimension.Top + 12, 0);
                    Bla.Origin = new Vector2(Bla.AbsoluteSize.X, 0);
                    Bla.Rotation = 0;
                    Bla.Effect = SpriteEffects.None;
                    break;

                case 1: // haut droite
                    Bla.Position = new Vector3(Dimension.Right + 4, Dimension.Top + 12, 0);
                    Bla.Origin = Vector2.Zero;
                    Bla.Rotation = 0;
                    Bla.Effect = SpriteEffects.FlipHorizontally;
                    break;
                case 2: // bas droite
                    Bla.Position = new Vector3(Dimension.Right + 16, Dimension.Bottom - 32, 0);
                    Bla.Origin = Bla.Center;
                    Bla.Rotation = MathHelper.Pi;
                    Bla.Effect = SpriteEffects.None;
                    break;
                case 3: // bas gauche
                    Bla.Position = new Vector3(Dimension.Left + 16, Dimension.Bottom + 16, 0);
                    Bla.Origin = Bla.Center;
                    Bla.Rotation = -MathHelper.PiOver2;
                    Bla.Effect = SpriteEffects.None;
                    break;
            }


            for (int i = 0; i < 4; i++)
            {
                Simulation.Scene.Add(Edges[i]);
                Simulation.Scene.Add(Corners[i]);
            }

            Simulation.Scene.Add(Filter);
            Simulation.Scene.Add(Bla);
        }


        public virtual void FadeIn(double time)
        {
            foreach (var coin in Corners)
            {
                coin.Color.A = 0;
                Simulation.Scene.VisualEffects.Add(coin, VisualEffects.FadeInFrom0(255, 0, time));
            }

            foreach (var contour in Edges)
            {
                contour.Color.A = 0;
                Simulation.Scene.VisualEffects.Add(contour, VisualEffects.FadeInFrom0(255, 0, time));
            }

            Filter.Color.A = 0;
            Bla.Color.A = 0;

            Simulation.Scene.VisualEffects.Add(Filter, VisualEffects.FadeInFrom0(128, 0, time));
            Simulation.Scene.VisualEffects.Add(Bla, VisualEffects.FadeInFrom0(255, 0, time));
        }


        public virtual void FadeOut(double time)
        {
            foreach (var coin in Corners)
                Simulation.Scene.VisualEffects.Add(coin, VisualEffects.FadeOutTo0(255, 0, time));

            foreach (var contour in Edges)
                Simulation.Scene.VisualEffects.Add(contour, VisualEffects.FadeOutTo0(255, 0, time));

            Simulation.Scene.VisualEffects.Add(Filter, VisualEffects.FadeOutTo0(128, 0, time));
            Simulation.Scene.VisualEffects.Add(Bla, VisualEffects.FadeOutTo0(255, 0, time));
        }
    }
}
