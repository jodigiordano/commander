namespace EphemereGames.Commander.Simulation
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;


    class Bubble : IVisual, IPhysical
    {
        protected Simulator Simulator;

        private List<Image> Corners;
        private List<Image> Edges;
        protected Image Filter;
        private Image Bla;

        public PhysicalRectangle Dimension;
        public int BlaPosition;
        public byte FilterAlpha;


        public Bubble(Simulator simulator, PhysicalRectangle dimension, double visualPriority)
        {
            Simulator = simulator;
            Dimension = dimension;
            FilterAlpha = 200;

            Bla = new Image("bulleBlabla")
            {
                VisualPriority = visualPriority,
                Origin = Vector2.Zero
            };

            Filter = new Image("PixelBlanc")
            {
                Color = new Color(0, 0, 0, FilterAlpha),
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


        public virtual Color Color
        {
            get
            {
                return Bla.Color;
            }

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


        public virtual byte Alpha
        {
            get
            {
                return Bla.Alpha;
            }
            set
            {
                foreach (var corner in Corners)
                    corner.Alpha = value;

                foreach (var edge in Edges)
                    edge.Alpha = value;

                Filter.Alpha = value;
                Bla.Alpha = value;

                Filter.Alpha = Math.Min(value, (byte) FilterAlpha);
            }
        }


        public virtual Vector3 Position
        {
            get
            {
                return new Vector3(Dimension.X, Dimension.Y, 0);
            }
            set
            {
                Dimension.X = (int) value.X;
                Dimension.Y = (int) value.Y;
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
                Simulator.Scene.Add(Edges[i]);
                Simulator.Scene.Add(Corners[i]);
            }

            Simulator.Scene.Add(Filter);
            Simulator.Scene.Add(Bla);
        }


        public virtual void Fade(int from, int to, double length, Core.IntegerHandler callback)
        {
            Alpha = (byte) from;

            var effect = VisualEffects.Fade(from, to, 0, length);

            foreach (var corner in Corners)
                Simulator.Scene.VisualEffects.Add(corner, effect, callback);

            foreach (var edge in Edges)
                Simulator.Scene.VisualEffects.Add(edge, effect);

            Simulator.Scene.VisualEffects.Add(Bla, effect);

            effect = VisualEffects.Fade(Math.Min(from, FilterAlpha), Math.Min(to, FilterAlpha), 0, length);

            Simulator.Scene.VisualEffects.Add(Filter, effect);
        }


        public virtual void FadeIn(double length)
        {
            Fade(0, 255, length, null);
        }


        public virtual void FadeOut(double length)
        {
            Fade(Bla.Alpha, 0, length, null);
        }


        protected void ClampPositionInView()
        {
            Dimension.X = (int) MathHelper.Clamp(Dimension.X, Simulator.Data.Battlefield.Inner.Left, Simulator.Data.Battlefield.Inner.Right - Dimension.Width / 2);
            Dimension.Y = (int) MathHelper.Clamp(Dimension.Y, Simulator.Data.Battlefield.Inner.Top + Dimension.Height / 2, Simulator.Data.Battlefield.Inner.Bottom - Dimension.Height / 2);
        }


        float IPhysical.Speed
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }


        float IPhysical.Rotation
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }


        Rectangle IVisual.VisiblePart
        {
            set { throw new NotImplementedException(); }
        }


        Vector2 IVisual.Origin
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }


        Vector2 IVisual.Size
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }
    }
}
