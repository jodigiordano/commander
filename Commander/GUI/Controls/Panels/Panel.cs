namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class Panel : PanelWidget
    {
        public bool Visible;
        public bool ShowFrame;
        public override Vector3 Position { get; set; }
        public override Vector3 Dimension { get; set; }
        public override double VisualPriority { get; set; }

        public Dictionary<string, PanelWidget> Widgets { get; private set; }

        private Image Background;
        private List<Image> Corners;
        private List<Image> Edges;
        private CloseButton CloseButton;
        private Text Title;
        private Image TitleSeparator;
        private Vector3 UpperLeftUsablePosition;


        public Panel(Scene scene, Vector3 position, Vector2 size, double visualPriority, Color color)
        {
            Position = position - new Vector3(size / 2f, 0);
            VisualPriority = visualPriority;

            Background = new Image("PixelBlanc", position)
            {
                Size = size,
                Color = Color.Black,
                VisualPriority = visualPriority + 0.0003,
                Alpha = 200,
                Origin = Vector2.Zero
            };

            Corners = new List<Image>();
            Edges = new List<Image>();

            for (int i = 0; i < 4; i++)
            {
                Corners.Add(new Image("bulleCoin") { VisualPriority = visualPriority + 0.0002, Color = color });
                Edges.Add(new Image("PixelBlanc") { VisualPriority = visualPriority + 0.0002, Origin = Vector2.Zero, Color = color });
            }

            Dimension = new Vector3(size, 0);

            TitleSeparator = new Image("PixelBlanc")
            {
                Color = color,
                VisualPriority = visualPriority + 0.0002,
                Origin = Vector2.Zero,
                Size = new Vector2(size.X, 5)
            };

            CloseButton = new CloseButton(
                new Vector3(Position.X + Dimension.X - 20, Position.Y + 15, 0), visualPriority);

            Visible = true;

            Widgets = new Dictionary<string, PanelWidget>();


            if (Title != null)
            {
                Title.Position = new Vector3(Position.X, Position.Y + 5, 0);
                TitleSeparator.Position = new Vector3(Position.X, Position.Y + Title.TextSize.Y + 5, 0);
            }

            ShowFrame = true;

            Scene = scene;
        }


        public override Scene Scene
        {
            get { return base.Scene; }
            set
            {
                base.Scene = value;
                CloseButton.Scene = value;
            }
        }


        public virtual void AddWidget(string name, PanelWidget widget)
        {
            widget.Scene = Scene;
            widget.VisualPriority = VisualPriority;

            Widgets[name] = widget;
        }


        public virtual void RemoveWidget(string name)
        {
            Widgets.Remove(name);
        }


        public void SetHandler(string widgetName, PanelWidgetHandler handler)
        {
            Widgets[widgetName].Handler = handler;
        }


        public void SetHandler(PanelWidgetHandler handler)
        {
            foreach (var w in Widgets.Values)
                w.Handler = handler;
        }


        public NoneHandler CloseButtonHandler
        {
            set { CloseButton.Handler = value; }
        }


        protected override bool Click(Circle circle)
        {
            if (ShowFrame && CloseButton.DoClick(circle))
                return true;

            if (ClickWidgets(circle))
                return true;

            return false;
        }


        public void SetTitle(string title)
        {
            bool adjustDimension = Title == null;

            Title = new Text(title, "Pixelite") { SizeX = 2, VisualPriority = VisualPriority };

            if (adjustDimension)
                Dimension += new Vector3(0, Title.TextSize.Y + 10, 0);
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


        public override void Draw()
        {
            if (!Visible)
                return;

            Background.Size = new Vector2(Dimension.X + 8, Dimension.Y + 8);
            Background.Position = new Vector3(Position.X - 4, Position.Y - 4, 0);

            Corners[0].Position = new Vector3(Position.X - 4, Position.Y - 4, 0); //Haut droite
            Corners[0].Rotation = 0;

            Corners[1].Position = new Vector3(Position.X - 4, Position.Y + Dimension.Y + 4, 0); //Bas droite
            Corners[1].Rotation = -MathHelper.PiOver2;

            Corners[2].Position = new Vector3(Position.X + Dimension.X + 4, Position.Y - 4, 0); //Haut gauche
            Corners[2].Rotation = MathHelper.PiOver2;

            Corners[3].Position = new Vector3(Position.X + Dimension.X + 4, Position.Y + Dimension.Y + 4, 0); //Bas gauche
            Corners[3].Rotation = MathHelper.Pi;


            Edges[0].Size = new Vector2(Dimension.X, 4); //Haut
            Edges[0].Position = new Vector3(Position.X, Position.Y - 8, 0);

            Edges[1].Size = new Vector2(Dimension.X, 4); //Bas
            Edges[1].Position = new Vector3(Position.X, Position.Y + Dimension.Y + 4, 0);

            Edges[2].Size = new Vector2(4, Dimension.Y); //Gauche
            Edges[2].Position = new Vector3(Position.X - 8, Position.Y, 0);

            Edges[3].Size = new Vector2(4, Dimension.Y); //Droite
            Edges[3].Position = new Vector3(Position.X + Dimension.X + 4, Position.Y, 0);


            if (ShowFrame)
            {
                for (int i = 0; i < 4; i++)
                {
                    Scene.Add(Edges[i]);
                    Scene.Add(Corners[i]);
                }

                Scene.Add(Background);

                if (Title != null)
                {
                    Title.Position = new Vector3(Position.X, Position.Y + 5, 0);
                    TitleSeparator.Position = new Vector3(Position.X, Position.Y + Title.TextSize.Y + 5, 0);

                    Scene.Add(TitleSeparator);
                    Scene.Add(Title);
                }

                CloseButton.Draw();
            }

            DrawWidgets();
        }


        public override void Fade(int from, int to, double length)
        {
            Visible = true;

            var effect = VisualEffects.Fade(from, to, 0, length);

            if (ShowFrame)
            {
                foreach (var corner in Corners)
                {
                    corner.Alpha = (byte) from;
                    Scene.VisualEffects.Add(corner, effect, FadeTerminated);
                }

                foreach (var edge in Edges)
                {
                    edge.Alpha = (byte) from;
                    Scene.VisualEffects.Add(edge, effect);
                }
            }

            if (Title != null)
            {
                Title.Alpha = (byte) from;
                TitleSeparator.Alpha = (byte) from;

                Scene.VisualEffects.Add(Title, effect);
                Scene.VisualEffects.Add(TitleSeparator, effect);
            }

            effect = VisualEffects.Fade(Math.Min(from, 200), Math.Min(to, 200), 0, length);

            Background.Alpha = (byte) Math.Min(from, 200);

            Scene.VisualEffects.Add(Background, effect);

            CloseButton.Fade(from, to, length);

            foreach (var w in Widgets.Values)
                w.Fade(from, to, length);
        }


        protected virtual void DrawWidgets()
        {
            foreach (var w in Widgets.Values)
                w.Draw();
        }


        protected virtual bool ClickWidgets(Circle circle)
        {
            foreach (var w in Widgets.Values)
                if (w.DoClick(circle))
                    return true;

            return false;
        }


        protected virtual Vector3 GetUpperLeftUsableSpace()
        {
            if (Title != null)
                return TitleSeparator.Position + new Vector3(0, 30, 0);
            else
                return new Vector3(Position.X, Position.Y + 30, 0);
        }


        private void FadeTerminated()
        {
            Visible = Background.Alpha != 0;
        }
    }
}
