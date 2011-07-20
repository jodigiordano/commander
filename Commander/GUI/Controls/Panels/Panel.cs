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
        public bool ShowBackground;
        public bool ShowCloseButton;
        public override Vector3 Dimension { get; set; }
        public override double VisualPriority { get; set; }

        public List<KeyValuePair<string, PanelWidget>> Widgets { get; private set; }

        private Image Background;
        private List<Image> Corners;
        private List<Image> Edges;
        private CloseButton CloseButton;
        private Text Title;
        private Image TitleSeparator;
        private Vector3 UpperLeftUsablePosition;
        private byte backgroundAlpha;
        private Vector2 padding;
        private Vector3 position;

        protected bool RecomputePositions;


        public Panel(Scene scene, Vector3 position, Vector2 size, double visualPriority, Color color)
        {
            Position = position - new Vector3(size / 2f, 0);
            VisualPriority = visualPriority;

            RecomputePositions = true;

            Background = new Image("PixelBlanc", position)
            {
                Size = size,
                Color = Color.Black,
                VisualPriority = visualPriority + 0.0003,
                Origin = Vector2.Zero
            };
            BackgroundAlpha = 200;

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

            Widgets = new List<KeyValuePair<string, PanelWidget>>();

            ShowFrame = true;
            ShowBackground = true;
            ShowCloseButton = true;

            Scene = scene;

            Padding = Vector2.Zero;
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


        public byte BackgroundAlpha
        {
            get { return backgroundAlpha; }
            set
            {
                backgroundAlpha = value;
                Background.Alpha = value;
            }
        }


        public override byte Alpha
        {
            get
            {
                if (Widgets.Count != 0)
                    return Widgets[0].Value.Alpha;

                return backgroundAlpha;
            }

            set
            {
                Background.Alpha = Math.Min(value, BackgroundAlpha);

                foreach (var w in Widgets)
                    w.Value.Alpha = value;
            }
        }


        public Vector2 Padding
        {
            get { return padding; }
            set
            {
                padding = value;
                RecomputePositions = true;
            }
        }


        public override Vector3 Position
        {
            get { return position; }
            set
            {
                position = value;
                RecomputePositions = true;
            }
        }


        public virtual void AddWidget(string name, PanelWidget widget)
        {
            widget.Scene = Scene;
            widget.VisualPriority = VisualPriority;

            Widgets.Add(new KeyValuePair<string, PanelWidget>(name, widget));

            RecomputePositions = true;
        }


        public virtual void RemoveWidget(string name)
        {
            for (int i = Widgets.Count - 1; i > -1; i--)
                if (Widgets[i].Key == name)
                {
                    Widgets.RemoveAt(i);
                    break;
                }

            RecomputePositions = true;
        }


        public PanelWidget GetWidgetByName(string name)
        {
            foreach (var w in Widgets)
                if (w.Key == name)
                    return w.Value;

            return null;
        }


        public virtual void ClearWidgets()
        {
            Widgets.Clear();

            RecomputePositions = true;
        }


        public virtual void SetClickHandler(string widgetName, PanelWidgetHandler handler)
        {
            foreach (var w in Widgets)
                if (w.Key == widgetName)
                {
                    w.Value.ClickHandler = handler;
                    break;
                }
        }


        public virtual void SetClickHandler(PanelWidgetHandler handler)
        {
            foreach (var w in Widgets)
                w.Value.ClickHandler = handler;
        }


        public void SetHoverHandler(string widgetName, PanelWidgetHandler handler)
        {
            foreach (var w in Widgets)
                if (w.Key == widgetName)
                {
                    w.Value.HoverHandler = handler;
                    break;
                }
        }


        public void SetHoverHandler(PanelWidgetHandler handler)
        {
            foreach (var w in Widgets)
                w.Value.HoverHandler = handler;
        }


        public NoneHandler CloseButtonHandler
        {
            set { CloseButton.Handler = value; }
        }


        protected override bool Click(Circle circle)
        {
            if (ShowCloseButton && CloseButton.DoClick(circle))
                return true;

            if (ClickWidgets(circle))
                return true;

            return false;
        }


        protected override bool Hover(Circle circle)
        {
            return Physics.CircleRectangleCollision(circle, Background.GetRectangle());
        }


        public void SetTitle(string title)
        {
            bool adjustDimension = Title == null;

            Title = new Text(title, "Pixelite")
            {
                SizeX = 2,
                VisualPriority = VisualPriority,
                Position = new Vector3(Position.X, Position.Y + 5, 0)
            };

            TitleSeparator.Position = new Vector3(Position.X, Position.Y + Title.TextSize.Y + 5, 0);

            if (adjustDimension)
            {
                Padding += new Vector2(0, 30);
                Dimension += new Vector3(0, Title.TextSize.Y + 10, 0);
            }
        }


        public bool OnlyShowWidgets
        {
            get { return !ShowBackground && !ShowCloseButton && !ShowFrame; }
            set
            {
                ShowFrame = !value;
                ShowBackground = !value;
                ShowCloseButton = !value;
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


        public override void Draw()
        {
            if (!Visible)
                return;

            if (RecomputePositions)
            {
                ComputePositions();
                RecomputePositions = false;
            }

            if (ShowBackground)
            {
                Background.Size = new Vector2(Dimension.X + 8, Dimension.Y + 8);
                Background.Position = new Vector3(Position.X - 4, Position.Y - 4, 0);

                Scene.Add(Background);
            }

            if (ShowFrame)
            {
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


                for (int i = 0; i < 4; i++)
                {
                    Scene.Add(Edges[i]);
                    Scene.Add(Corners[i]);
                }
            }

            if (ShowCloseButton)
            {
                CloseButton.Draw();
            }

            if (Title != null)
            {
                Title.Position = new Vector3(Position.X, Position.Y + 5, 0);
                TitleSeparator.Position = new Vector3(Position.X, Position.Y + Title.TextSize.Y + 5, 0);

                Scene.Add(TitleSeparator);
                Scene.Add(Title);
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

            effect = VisualEffects.Fade(Math.Min(from, BackgroundAlpha), Math.Min(to, BackgroundAlpha), 0, length);

            Background.Alpha = (byte) Math.Min(from, BackgroundAlpha);

            Scene.VisualEffects.Add(Background, effect);

            CloseButton.Fade(from, to, length);

            foreach (var w in Widgets)
                w.Value.Fade(from, to, length);
        }


        protected virtual void DrawWidgets()
        {
            foreach (var w in Widgets)
                w.Value.Draw();
        }


        protected virtual bool ClickWidgets(Circle circle)
        {
            foreach (var w in Widgets)
                if (w.Value.DoClick(circle))
                    return true;

            return false;
        }


        protected virtual Vector3 GetUpperLeftUsableSpace()
        {
            if (Title != null)
                return TitleSeparator.Position + new Vector3(Padding.X, Padding.Y, 0);
            else
                return new Vector3(Position.X + Padding.X, Position.Y + Padding.Y, 0);
        }


        protected virtual void ComputePositions()
        {

        }


        private void FadeTerminated()
        {
            Visible = Background.Alpha != 0;
        }
    }
}
