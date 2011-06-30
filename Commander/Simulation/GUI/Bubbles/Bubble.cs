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


        public Bubble(Simulator simulator, PhysicalRectangle dimension, double visualPriority)
        {
            this.Simulation = simulator;
            this.Dimension = dimension;

            Bla = new Image("bulleBlabla");
            Bla.VisualPriority = visualPriority;
            Bla.Origin = Vector2.Zero;

            Filter = new Image("PixelBlanc")
            {
                Color = new Color(0, 0, 0, 200),
                VisualPriority = visualPriority + 0.000002,
                Origin = Vector2.Zero
            };

            Corners = new List<Image>();
            Edges = new List<Image>();

            for (int i = 0; i < 4; i++)
            {
                Corners.Add(new Image("bulleCoin") { VisualPriority = visualPriority + 0.000001 });
                Edges.Add(new Image("PixelBlanc") { VisualPriority = visualPriority + 0.000001, Origin = Vector2.Zero });
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
                    Bla.Position = new Vector3(Dimension.Right + 16, Dimension.Bottom - 16, 0);
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


        public virtual void Fade(int from, int to, double length, Core.NoneHandler callback)
        {
            var effect = VisualEffects.Fade(from, to, 0, length);

            foreach (var coin in Corners)
            {
                coin.Color.A = (byte) from;
                Simulation.Scene.VisualEffects.Add(coin, effect, callback);
            }

            foreach (var contour in Edges)
            {
                contour.Color.A = (byte) from;
                Simulation.Scene.VisualEffects.Add(contour, effect);
            }

            Filter.Color.A = (byte) from;
            Bla.Color.A = (byte) from;

            Simulation.Scene.VisualEffects.Add(Bla, effect);

            effect = VisualEffects.Fade(Math.Min(from, 128), Math.Min(to, 128), 0, length);

            Filter.Alpha = (byte) Math.Max(from, 128);

            Simulation.Scene.VisualEffects.Add(Filter, effect);
        }


        public virtual void FadeIn(double length)
        {
            Fade(0, 255, length, null);
        }


        public virtual void FadeOut(double length)
        {
            Fade(Bla.Alpha, 0, length, null);
        }
    }
}
