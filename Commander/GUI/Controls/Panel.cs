namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class Panel
    {
        public bool Visible;
        public CloseButton CloseButton;

        private Scene Scene;
        private Image Background;
        private List<Image> Corners;
        private List<Image> Edges;
        private PhysicalRectangle Dimension;
        private double VisualPriority;

        private Text Title;
        private Image TitleSeparator;



        public Panel(Scene scene, Vector3 position, Vector2 size, double visualPriority, Color color)
        {
            Scene = scene;
            VisualPriority = visualPriority;

            Background = new Image("PixelBlanc", position)
            {
                Size = size,
                Color = Color.Black,
                VisualPriority = visualPriority + 0.000003,
                Alpha = 100,
                Origin = Vector2.Zero
            };

            Corners = new List<Image>();
            Edges = new List<Image>();

            for (int i = 0; i < 4; i++)
            {
                Corners.Add(new Image("bulleCoin") { VisualPriority = visualPriority + 0.000002, Color = color });
                Edges.Add(new Image("PixelBlanc") { VisualPriority = visualPriority + 0.000002, Origin = Vector2.Zero, Color = color });
            }

            Dimension = new PhysicalRectangle(
                (int) (position.X - size.X / 2), 
                (int) (position.Y - size.Y / 2),
                (int) size.X,
                (int) size.Y);

            TitleSeparator = new Image("PixelBlanc")
            {
                Color = color,
                VisualPriority = visualPriority + 0.000002,
                Origin = Vector2.Zero,
                Size = new Vector2(size.X, 5)
            };

            CloseButton = new CloseButton(Scene, new Vector3(Dimension.Right - 20, Dimension.Top + 15, 0), visualPriority);

            Visible = true;
        }


        public void SetTitle(string title)
        {
            bool adjustDimension = Title == null;

            Title = new Text(title, "Pixelite") { SizeX = 2, VisualPriority = VisualPriority };

            if (adjustDimension)
            {
                Dimension.Height += (int) Title.TextSize.Y + 10;
            }
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
            }
        }


        public void Draw()
        {
            if (!Visible)
                return;

            Background.Size = new Vector2(Dimension.Width + 8, Dimension.Height + 8);
            Background.Position = new Vector3(Dimension.Left - 4, Dimension.Top - 4, 0);

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


            for (int i = 0; i < 4; i++)
            {
                Scene.Add(Edges[i]);
                Scene.Add(Corners[i]);
            }

            Scene.Add(Background);

            if (Title != null)
            {
                Title.Position = new Vector3(Dimension.Left, Dimension.Top + 5, 0);
                TitleSeparator.Position = new Vector3(Dimension.Left, Dimension.Top + Title.TextSize.Y + 5, 0);

                Scene.Add(TitleSeparator);
                Scene.Add(Title);
            }

            CloseButton.Draw();
        }


        public void Fade(int from, int to, double length)
        {
            Visible = true;

            var effect = VisualEffects.Fade(from, to, 0, length);
            effect.TerminatedCallback = FadeTerminated;

            foreach (var corner in Corners)
            {
                corner.Alpha = (byte) from;
                Scene.VisualEffects.Add(corner, effect);
            }

            foreach (var edge in Edges)
            {
                edge.Alpha = (byte) from;
                Scene.VisualEffects.Add(edge, effect);
            }

            if (Title != null)
            {
                Title.Alpha = (byte) from;
                TitleSeparator.Alpha = (byte) from;

                Scene.VisualEffects.Add(Title, effect);
                Scene.VisualEffects.Add(TitleSeparator, effect);
            }

            effect = VisualEffects.Fade(Math.Min(from, 100), Math.Min(to, 100), 0, length);

            Background.Alpha = (byte) Math.Min(from, 100);

            Scene.VisualEffects.Add(Background, effect);

            CloseButton.Fade(from, to, length);
        }


        private void FadeTerminated()
        {
            Visible = Background.Alpha != 0;
        }
    }
}
